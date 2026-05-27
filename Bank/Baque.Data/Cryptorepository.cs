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
    //  CryptoRepository — Accès aux données de la table crypto_wallets
    //  FK : crypto_wallets.account_id → accounts.id  (CASCADE DELETE)
    //  Contrainte unique : (account_id, symbole)
    //    → Un compte ne peut avoir qu'un seul wallet par devise (BTC, ETH…)
    //
    //  Opérations CRUD disponibles :
    //    Add()                        → INSERT — crée un nouveau wallet
    //    GetById()                    → SELECT WHERE id
    //    GetByAccountId()             → SELECT WHERE account_id (tous les wallets du compte)
    //    GetByAccountIdAndSymbole()   → SELECT WHERE account_id AND symbole (wallet unique)
    //    UpdateQuantite()             → UPDATE quantite uniquement (après achat/vente)
    //    Delete()                     → DELETE WHERE id
    //
    //  Le symbole est TOUJOURS stocké en majuscules (.ToUpper()) pour garantir
    //  la cohérence avec la contrainte unique et les comparaisons en base.
    // ══════════════════════════════════════════════════════════════════════════
    internal class CryptoRepository
    {
        // Instance Singleton de DbConnection — partagée avec tous les repositories
        private readonly DbConnection _db = DbConnection.Instance;

        // ══════════════════════════════════════════════════════════════════════
        //  CREATE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Insère un nouveau portefeuille crypto en base et retourne son Id généré.
        /// Appelé par CryptoService.AcheterCrypto() uniquement lors du premier achat
        /// d'une devise pour un compte donné (cas wallet inexistant).
        /// Le symbole est forcé en majuscules pour respecter la contrainte unique.
        /// </summary>
        /// <param name="wallet">Wallet à insérer (Id non requis, généré par IDENTITY).</param>
        /// <returns>Id auto-incrémenté généré par SQL Server.</returns>
        public int Add(Cryptowallet wallet)
        {
            const string sql = @"
                INSERT INTO crypto_wallets (account_id, symbole, quantite, created_at, updated_at)
                OUTPUT INSERTED.id
                VALUES (@accountId, @symbole, @quantite, @createdAt, @updatedAt)";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@accountId", wallet.AccountId);
                // Stockage en majuscules : garantit la cohérence avec la contrainte unique
                cmd.Parameters.AddWithValue("@symbole", wallet.Symbole.ToUpper());
                cmd.Parameters.AddWithValue("@quantite", wallet.Quantite);
                cmd.Parameters.AddWithValue("@createdAt", wallet.CreatedAt);
                cmd.Parameters.AddWithValue("@updatedAt", wallet.UpdatedAt);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  READ
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Retourne un wallet par sa clé primaire.
        /// Usage rare — préférer GetByAccountIdAndSymbole() dans la plupart des cas.
        /// </summary>
        /// <param name="id">Clé primaire du wallet.</param>
        /// <returns>L'objet <see cref="Cryptowallet"/> ou null si introuvable.</returns>
        public Cryptowallet GetById(int id)
        {
            const string sql = "SELECT * FROM crypto_wallets WHERE id = @id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    // Opérateur ternaire : retourne le wallet ou null
                    return reader.Read() ? MapRow(reader) : null;
                }
            }
        }

        /// <summary>
        /// Retourne tous les portefeuilles crypto d'un compte bancaire.
        /// Chaque ligne représente une devise différente (BTC, ETH, SOL…).
        /// Trié par symbole pour un affichage alphabétique cohérent dans FrmCrypto.
        /// Utilisé par CryptoService.GetWallets() et GetValeurTotalePortefeuille().
        /// </summary>
        /// <param name="accountId">FK account_id du compte propriétaire.</param>
        /// <returns>
        /// Liste des wallets triée alphabétiquement par symbole.
        /// Vide si aucun achat de crypto n'a encore été effectué.
        /// </returns>
        public List<Cryptowallet> GetByAccountId(int accountId)
        {
            // ORDER BY symbole : tri alphabétique BTC, BNB, ETH, SOL, USDT…
            const string sql = @"
                SELECT * FROM crypto_wallets
                WHERE account_id = @accountId
                ORDER BY symbole";

            var wallets = new List<Cryptowallet>();

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@accountId", accountId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        wallets.Add(MapRow(reader));

                    return wallets;
                }
            }
        }

        /// <summary>
        /// Retourne le wallet d'un compte pour un symbole donné.
        /// Exploite la contrainte unique (account_id, symbole) :
        /// garantit qu'au plus un résultat est retourné.
        /// Méthode clé utilisée par CryptoService avant chaque achat/vente :
        ///   - Si null → premier achat → Add() pour créer le wallet
        ///   - Si non null → achat/vente → UpdateQuantite()
        /// </summary>
        /// <param name="accountId">FK account_id du compte.</param>
        /// <param name="symbole">Symbole de la crypto (insensible à la casse, forcé en majuscules).</param>
        /// <returns>L'objet <see cref="Cryptowallet"/> ou null si le compte ne détient pas cette devise.</returns>
        public Cryptowallet GetByAccountIdAndSymbole(int accountId, string symbole)
        {
            const string sql = @"
                SELECT * FROM crypto_wallets
                WHERE account_id = @accountId AND symbole = @symbole";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@accountId", accountId);
                // ToUpper() obligatoire pour correspondre aux valeurs stockées en majuscules
                cmd.Parameters.AddWithValue("@symbole", symbole.ToUpper());

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapRow(reader);

                    return null; // aucun wallet pour ce couple (compte, devise)
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  UPDATE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Met à jour uniquement la quantité détenue dans un wallet.
        /// UPDATE partiel pour des raisons de performance : seule la quantité change.
        /// updated_at est forcé à DateTime.UtcNow pour tracer chaque opération.
        /// Appelée par CryptoService.AcheterCrypto() et VendreCrypto()
        /// après le calcul de la nouvelle quantité.
        /// </summary>
        /// <param name="walletId">Id du wallet à modifier.</param>
        /// <param name="nouvelleQuantite">Nouvelle quantité calculée par CryptoService.</param>
        /// <returns>true si au moins une ligne a été modifiée.</returns>
        public bool UpdateQuantite(int walletId, decimal nouvelleQuantite)
        {
            const string sql = @"
                UPDATE crypto_wallets
                SET quantite   = @quantite,
                    updated_at = @updatedAt
                WHERE id = @id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@quantite", nouvelleQuantite);
                cmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow); // horodatage automatique
                cmd.Parameters.AddWithValue("@id", walletId);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  DELETE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Supprime un wallet par son Id.
        /// Utilisé si l'on souhaite clôturer un portefeuille vide.
        /// ⚠ Le compte parent n'est pas affecté (pas de CASCADE sur cette direction).
        /// </summary>
        /// <param name="id">Id du wallet à supprimer.</param>
        /// <returns>true si la suppression a réussi.</returns>
        public bool Delete(int id)
        {
            const string sql = "DELETE FROM crypto_wallets WHERE id = @id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  MAPPING SqlDataReader → Cryptowallet
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Convertit une ligne du SqlDataReader en objet <see cref="Cryptowallet"/>.
        /// Méthode privée et statique : centralisée pour éviter la duplication
        /// dans GetById(), GetByAccountId() et GetByAccountIdAndSymbole().
        /// Quantite est en DECIMAL(20,8) SQL → decimal C# (précision conservée).
        /// </summary>
        private static Cryptowallet MapRow(SqlDataReader r) => new Cryptowallet()
        {
            Id = (int)r["id"],
            AccountId = (int)r["account_id"],       // FK → accounts.id
            Symbole = r["symbole"].ToString(),     // stocké en majuscules
            Quantite = (decimal)r["quantite"],      // DECIMAL(20,8) → 8 décimales
            CreatedAt = (DateTime)r["created_at"],
            UpdatedAt = (DateTime)r["updated_at"],
        };
    }
}