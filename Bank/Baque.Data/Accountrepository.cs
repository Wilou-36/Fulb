using Bank.Banque.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Banque.Data
{
    // ══════════════════════════════════════════════════════════════════════════
    //  AccountRepository — Accès aux données de la table accounts
    //  FK : accounts.user_id → users.id
    //
    //  Opérations CRUD disponibles :
    //    Add()              → INSERT — crée un compte, retourne l'Id généré
    //    GetById()          → SELECT WHERE id
    //    GetByUserId()      → SELECT WHERE user_id (tous les comptes d'un user)
    //    GetByNumAccount()  → SELECT WHERE num_account (recherche par numéro)
    //    UpdateSolde()      → UPDATE solde uniquement (opération fréquente)
    //    Delete()           → DELETE WHERE id
    //
    //  UpdateSolde() est la méthode la plus utilisée : appelée après chaque
    //  dépôt, retrait, virement ou opération crypto via AccountService.
    // ══════════════════════════════════════════════════════════════════════════
    internal class AccountRepository
    {
        // Instance Singleton de DbConnection — partagée avec tous les repositories
        private readonly DbConnection _db = DbConnection.Instance;

        // ══════════════════════════════════════════════════════════════════════
        //  CREATE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Insère un nouveau compte bancaire en base et retourne son Id généré.
        /// Appelé par AccountService.CreerCompte() après vérification de l'utilisateur.
        /// OUTPUT INSERTED.id évite une seconde requête SELECT pour récupérer l'Id.
        /// </summary>
        /// <param name="account">Compte à insérer (Id non requis, généré par IDENTITY).</param>
        /// <returns>Id auto-incrémenté généré par SQL Server.</returns>
        public int Add(Account account)
        {
            // OUTPUT INSERTED.id : retourne l'Id généré sans requête supplémentaire
            const string sql = @"
        INSERT INTO accounts (user_id, num_account, solde, created_at, updated_at)
        OUTPUT INSERTED.id
        VALUES (@userId, @numAccount, @solde, @createdAt, @updatedAt)";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", account.UserId);     // FK → users.id
                cmd.Parameters.AddWithValue("@numAccount", account.NumAccount); // numéro unique FR+timestamp
                cmd.Parameters.AddWithValue("@solde", account.Solde);      // 0.00 à la création
                cmd.Parameters.AddWithValue("@createdAt", account.CreatedAt);
                cmd.Parameters.AddWithValue("@updatedAt", account.UpdatedAt);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  READ
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Retourne un compte bancaire par sa clé primaire.
        /// Utilisé par AccountService.GetCompte() et après chaque opération
        /// pour recharger le solde mis à jour depuis la base.
        /// </summary>
        /// <param name="id">Clé primaire du compte.</param>
        /// <returns>L'objet <see cref="Account"/> ou null si introuvable.</returns>
        public Account GetById(int id)
        {
            const string sql = "SELECT * FROM accounts WHERE id = @id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapRow(reader);

                    return null; // aucun compte avec cet id
                }
            }
        }

        /// <summary>
        /// Retourne tous les comptes bancaires d'un utilisateur (via FK user_id).
        /// Utilisé par FrmDashboard pour peupler le ComboBox de sélection de compte.
        /// Un utilisateur peut posséder plusieurs comptes (relation 1-*).
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur propriétaire.</param>
        /// <returns>Liste des comptes. Vide si l'utilisateur n'a pas encore de compte.</returns>
        public List<Account> GetByUserId(int userId)
        {
            const string sql = "SELECT * FROM accounts WHERE user_id = @userId";

            var accounts = new List<Account>();

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        accounts.Add(MapRow(reader));
                    }
                }
            }

            return accounts;
        }

        /// <summary>
        /// Retourne un compte bancaire par son numéro unique.
        /// Utilisé par FrmVirement pour trouver le compte destinataire
        /// saisi par l'utilisateur, avec vérification en temps réel.
        /// La colonne num_account possède une contrainte UNIQUE en base.
        /// </summary>
        /// <param name="numAccount">Numéro de compte au format FR+timestamp.</param>
        /// <returns>L'objet <see cref="Account"/> ou null si le numéro n'existe pas.</returns>
        public Account GetByNumAccount(string numAccount)
        {
            const string sql = "SELECT * FROM accounts WHERE num_account = @numAccount";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@numAccount", numAccount);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapRow(reader);

                    return null; // numéro introuvable
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  UPDATE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Met à jour uniquement le solde d'un compte bancaire.
        /// Méthode ciblée (UPDATE partiel) pour des raisons de performance :
        /// seul le solde change, pas le numéro de compte ou le user_id.
        /// updated_at est forcé à DateTime.UtcNow pour tracer la modification.
        /// Appelée par :
        ///   - AccountService.Deposer() / Retirer()
        ///   - TransactionService.EffectuerVirement() (deux fois : source et destination)
        ///   - CryptoService.AcheterCrypto() / VendreCrypto()
        /// </summary>
        /// <param name="accountId">Id du compte à modifier.</param>
        /// <param name="nouveauSolde">Nouveau solde calculé par le service appelant.</param>
        /// <returns>true si au moins une ligne a été modifiée.</returns>
        public bool UpdateSolde(int accountId, decimal nouveauSolde)
        {
            const string sql = @"
        UPDATE accounts
        SET solde      = @solde,
            updated_at = @updatedAt
        WHERE id = @id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@solde", nouveauSolde);
                cmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow); // horodatage automatique
                cmd.Parameters.AddWithValue("@id", accountId);

                // ExecuteNonQuery() retourne le nombre de lignes affectées
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  DELETE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Supprime un compte bancaire par son Id.
        /// ⚠ CASCADE DELETE en base via FK :
        ///   - crypto_wallets.account_id → supprime les wallets associés
        /// ⚠ RESTRICT DELETE via FK :
        ///   - transactions.account_id → échec si des transactions existent
        ///     (l'historique ne peut pas être supprimé)
        /// </summary>
        /// <param name="id">Id du compte à supprimer.</param>
        /// <returns>true si la suppression a réussi.</returns>
        public bool Delete(int id)
        {
            const string sql = "DELETE FROM accounts WHERE id = @id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  MAPPING SqlDataReader → Account
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Convertit une ligne du SqlDataReader en objet <see cref="Account"/>.
        /// Méthode privée et statique : centralisée pour éviter la duplication
        /// dans GetById(), GetByUserId() et GetByNumAccount().
        /// Les noms r["colonne"] doivent correspondre exactement aux colonnes SQL.
        /// </summary>
        private static Account MapRow(SqlDataReader r) => new Account()
        {
            Id = (int)r["id"],
            UserId = (int)r["user_id"],        // FK → users.id
            NumAccount = r["num_account"].ToString(),
            Solde = (decimal)r["solde"],       // DECIMAL(15,2) → decimal C#
            CreatedAt = (DateTime)r["created_at"],
            UpdatedAt = (DateTime)r["updated_at"],
        };
    }
}