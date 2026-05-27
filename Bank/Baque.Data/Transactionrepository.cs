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
    //  TransactionRepository — Accès aux données de la table transactions
    //  FK : transactions.account_id → accounts.id  (RESTRICT DELETE)
    //
    //  Opérations disponibles :
    //    Add()                    → INSERT — enregistre une opération financière
    //    GetById()                → SELECT WHERE id
    //    GetByAccountId()         → SELECT WHERE account_id, trié par date DESC
    //    GetByAccountIdAndType()  → SELECT avec filtre sur le type (DEPOT, VIREMENT…)
    //
    //  ⚠ Pas de méthode Update() ni Delete() intentionnellement :
    //    L'historique des transactions est immuable pour des raisons de traçabilité
    //    comptable. Une transaction enregistrée ne peut pas être modifiée.
    //
    //  Conversion enum ↔ string :
    //    À l'écriture : transaction.Type.ToString() → "DEPOT", "VIREMENT", etc.
    //    À la lecture  : Enum.Parse(typeof(TransactionType), r["type"].ToString())
    // ══════════════════════════════════════════════════════════════════════════
    internal class TransactionRepository
    {
        // Instance Singleton de DbConnection — partagée avec tous les repositories
        private readonly DbConnection _db = DbConnection.Instance;

        // ══════════════════════════════════════════════════════════════════════
        //  CREATE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Insère une nouvelle transaction en base et retourne son Id généré.
        /// Appelée par AccountService (dépôt, retrait), TransactionService (virement)
        /// et CryptoService (achat/vente crypto).
        /// Le champ Details peut être null → converti en DBNull.Value pour SQL.
        /// </summary>
        /// <param name="transaction">Transaction à insérer.</param>
        /// <returns>Id auto-incrémenté généré par SQL Server.</returns>
        public int Add(Transaction transaction)
        {
            // OUTPUT INSERTED.id retourne l'Id sans requête supplémentaire
            const string sql = @"
        INSERT INTO transactions
            (account_id, type, montant, date_transaction, details, created_at, updated_at)
        OUTPUT INSERTED.id
        VALUES
            (@accountId, @type, @montant, @dateTransaction, @details, @createdAt, @updatedAt)";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@accountId", transaction.AccountId);
                // Conversion de l'enum en string pour la colonne NVARCHAR(20) avec CHECK
                // ex : TransactionType.DEPOT → "DEPOT"
                cmd.Parameters.AddWithValue("@type", transaction.Type.ToString());
                cmd.Parameters.AddWithValue("@montant", transaction.Montant);
                cmd.Parameters.AddWithValue("@dateTransaction", transaction.DateTransaction);

                // Details est nullable en base → DBNull.Value si null ou vide
                if (string.IsNullOrEmpty(transaction.Details))
                    cmd.Parameters.AddWithValue("@details", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@details", transaction.Details);

                cmd.Parameters.AddWithValue("@createdAt", transaction.CreatedAt);
                cmd.Parameters.AddWithValue("@updatedAt", transaction.UpdatedAt);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  READ
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Retourne une transaction par sa clé primaire.
        /// Utilisé par TransactionService.GetTransaction() pour accéder
        /// aux détails d'une opération spécifique.
        /// </summary>
        /// <param name="id">Clé primaire de la transaction.</param>
        /// <returns>L'objet <see cref="Transaction"/> ou null si introuvable.</returns>
        public Transaction GetById(int id)
        {
            const string sql = "SELECT * FROM transactions WHERE id = @id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapRow(reader);

                    return null;
                }
            }
        }

        /// <summary>
        /// Retourne toutes les transactions d'un compte, triées de la plus
        /// récente à la plus ancienne (ORDER BY date_transaction DESC).
        /// Utilisé par FrmDashboard pour l'historique et par TransactionService
        /// pour le calcul des totaux crédits/débits sur une période.
        /// </summary>
        /// <param name="accountId">FK account_id du compte concerné.</param>
        /// <returns>
        /// Liste complète des transactions : dépôts, retraits, virements, crypto.
        /// Vide si aucune opération n'a encore été effectuée.
        /// </returns>
        public List<Transaction> GetByAccountId(int accountId)
        {
            // ORDER BY date_transaction DESC : du plus récent au plus ancien
            const string sql = @"
        SELECT * FROM transactions
        WHERE account_id = @accountId
        ORDER BY date_transaction DESC";

            List<Transaction> transactions = new List<Transaction>();

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@accountId", accountId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        transactions.Add(MapRow(reader));
                    }
                }
            }

            return transactions;
        }

        /// <summary>
        /// Retourne les transactions d'un compte filtrées par type d'opération.
        /// Utile pour afficher uniquement les virements, les cryptos, etc.
        /// Utilisé par TransactionService.GetHistoriqueParType().
        /// </summary>
        /// <param name="accountId">FK account_id du compte.</param>
        /// <param name="type">
        /// Type de transaction souhaité (DEPOT, RETRAIT, VIREMENT,
        /// ACHAT_CRYPTO ou VENTE_CRYPTO).
        /// </param>
        /// <returns>Liste filtrée, triée par date décroissante.</returns>
        public List<Transaction> GetByAccountIdAndType(int accountId, TransactionType type)
        {
            const string sql = @"
        SELECT * FROM transactions
        WHERE account_id = @accountId
        AND type = @type
        ORDER BY date_transaction DESC";

            List<Transaction> transactions = new List<Transaction>();

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@accountId", accountId);
                // Conversion de l'enum en string pour la comparaison SQL
                cmd.Parameters.AddWithValue("@type", type.ToString());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        transactions.Add(MapRow(reader));
                    }
                }
            }

            return transactions;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  MAPPING SqlDataReader → Transaction
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Convertit une ligne du SqlDataReader en objet <see cref="Transaction"/>.
        /// Points d'attention :
        ///   - Type : conversion string → enum via Enum.Parse
        ///     (ex : "DEPOT" → TransactionType.DEPOT)
        ///   - Details : peut être DBNull en base → null en C# si absent
        /// </summary>
        private static Transaction MapRow(SqlDataReader r) => new Transaction()
        {
            Id = (int)r["id"],
            AccountId = (int)r["account_id"],
            // Enum.Parse convertit "DEPOT", "VIREMENT", etc. en TransactionType
            Type = (TransactionType)Enum.Parse(typeof(TransactionType), r["type"].ToString()),
            Montant = (decimal)r["montant"],
            DateTransaction = (DateTime)r["date_transaction"],
            // Gestion du NULL SQL : DBNull.Value → null en C#
            Details = r["details"] == DBNull.Value ? null : r["details"].ToString(),
            CreatedAt = (DateTime)r["created_at"],
            UpdatedAt = (DateTime)r["updated_at"],
        };
    }
}