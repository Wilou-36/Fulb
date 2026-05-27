using Bank.Banque.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Banque.Data
{
    /// <summary>
    /// Accès aux données de la table <c>transactions</c>.
    /// FK : transactions.account_id → accounts.id
    /// </summary>
    internal class TransactionRepository
    {
        private readonly DbConnection _db = DbConnection.Instance;

        // ── CREATE ────────────────────────────────────────────────────────────
        /// <summary>Insère une transaction et retourne son Id généré.</summary>
        public int Add(Transaction transaction)
        {
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
                cmd.Parameters.AddWithValue("@type", transaction.Type.ToString());
                cmd.Parameters.AddWithValue("@montant", transaction.Montant);
                cmd.Parameters.AddWithValue("@dateTransaction", transaction.DateTransaction);

                if (string.IsNullOrEmpty(transaction.Details))
                    cmd.Parameters.AddWithValue("@details", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@details", transaction.Details);

                cmd.Parameters.AddWithValue("@createdAt", transaction.CreatedAt);
                cmd.Parameters.AddWithValue("@updatedAt", transaction.UpdatedAt);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // ── READ ──────────────────────────────────────────────────────────────
        /// <summary>Retourne une transaction par son Id.</summary>
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
        /// Retourne toutes les transactions d'un compte (FK account_id),
        /// triées de la plus récente à la plus ancienne.
        /// </summary>
        public List<Transaction> GetByAccountId(int accountId)
        {
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
        /// Retourne les transactions d'un compte filtrées par type.
        /// </summary>
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

        // ── Mapping ───────────────────────────────────────────────────────────
        private static Transaction MapRow(SqlDataReader r) => new Transaction()
        {
            Id = (int)r["id"],
            AccountId = (int)r["account_id"],
            Type = (TransactionType)Enum.Parse(typeof(TransactionType), r["type"].ToString()),
            Montant = (decimal)r["montant"],
            DateTransaction = (DateTime)r["date_transaction"],
            Details = r["details"] == DBNull.Value ? null : r["details"].ToString(),
            CreatedAt = (DateTime)r["created_at"],
            UpdatedAt = (DateTime)r["updated_at"],
        };
    }
}