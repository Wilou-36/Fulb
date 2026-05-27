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
    /// Accès aux données de la table <c>accounts</c>.
    /// FK : accounts.user_id → users.id
    /// </summary>
    internal class AccountRepository
    {
        private readonly DbConnection _db = DbConnection.Instance;

        // ── CREATE ────────────────────────────────────────────────────────────
        /// <summary>Insère un nouveau compte et retourne son Id généré.</summary>
        public int Add(Account account)
        {
            const string sql = @"
        INSERT INTO accounts (user_id, num_account, solde, created_at, updated_at)
        OUTPUT INSERTED.id
        VALUES (@userId, @numAccount, @solde, @createdAt, @updatedAt)";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", account.UserId);
                cmd.Parameters.AddWithValue("@numAccount", account.NumAccount);
                cmd.Parameters.AddWithValue("@solde", account.Solde);
                cmd.Parameters.AddWithValue("@createdAt", account.CreatedAt);
                cmd.Parameters.AddWithValue("@updatedAt", account.UpdatedAt);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // ── READ ──────────────────────────────────────────────────────────────
        /// <summary>Retourne un compte par son Id.</summary>
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

                    return null;
                }
            }
        }

        /// <summary>Retourne tous les comptes d'un utilisateur (FK user_id).</summary>
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

        /// <summary>Retourne un compte par son numéro unique.</summary>
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

                    return null;
                }
            }
        }

        // ── UPDATE ────────────────────────────────────────────────────────────
        /// <summary>Met à jour le solde d'un compte.</summary>
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
                cmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@id", accountId);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        /// <summary>Supprime un compte par son Id.</summary>
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

        // ── Mapping ───────────────────────────────────────────────────────────
        private static Account MapRow(SqlDataReader r) => new Account()
        {
            Id = (int)r["id"],
            UserId = (int)r["user_id"],
            NumAccount = r["num_account"].ToString(),
            Solde = (decimal)r["solde"],
            CreatedAt = (DateTime)r["created_at"],
            UpdatedAt = (DateTime)r["updated_at"],
        };
    }
}
