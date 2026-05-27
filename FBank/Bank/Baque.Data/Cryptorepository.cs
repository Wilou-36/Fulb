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
    /// Accès aux données de la table <c>crypto_wallets</c>.
    /// FK : crypto_wallets.account_id → accounts.id
    /// Contrainte unique : (account_id, symbole)
    /// </summary>
    internal class CryptoRepository
    {
        private readonly DbConnection _db = DbConnection.Instance;

        // ── CREATE ────────────────────────────────────────────────────────────
        /// <summary>Insère un nouveau wallet crypto et retourne son Id généré.</summary>
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
                cmd.Parameters.AddWithValue("@symbole", wallet.Symbole.ToUpper());
                cmd.Parameters.AddWithValue("@quantite", wallet.Quantite);
                cmd.Parameters.AddWithValue("@createdAt", wallet.CreatedAt);
                cmd.Parameters.AddWithValue("@updatedAt", wallet.UpdatedAt);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // ── READ ──────────────────────────────────────────────────────────────
        /// <summary>Retourne un wallet par son Id.</summary>
        public Cryptowallet GetById(int id)
        {
            const string sql = "SELECT * FROM crypto_wallets WHERE id = @id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? MapRow(reader) : null;
                }
            }
        }

        /// <summary>
        /// Retourne tous les wallets d'un compte (FK account_id).
        /// </summary>
        public List<Cryptowallet> GetByAccountId(int accountId)
        {
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
        /// Retourne le wallet d'un compte pour un symbole donné (contrainte unique).
        /// </summary>
        public Cryptowallet GetByAccountIdAndSymbole(int accountId, string symbole)
        {
            const string sql = @"
                SELECT * FROM crypto_wallets
                WHERE account_id = @accountId AND symbole = @symbole";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@accountId", accountId);
                cmd.Parameters.AddWithValue("@symbole", symbole.ToUpper());

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapRow(reader);

                    return null;
                }
            }
        }

        // ── UPDATE ────────────────────────────────────────────────────────────
        /// <summary>Met à jour la quantité détenue dans un wallet.</summary>
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
                cmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@id", walletId);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        /// <summary>Supprime un wallet par son Id.</summary>
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

        // ── Mapping ───────────────────────────────────────────────────────────
        private static Cryptowallet MapRow(SqlDataReader r) => new Cryptowallet()
        {
            Id = (int)r["id"],
            AccountId = (int)r["account_id"],
            Symbole = r["symbole"].ToString(),
            Quantite = (decimal)r["quantite"],
            CreatedAt = (DateTime)r["created_at"],
            UpdatedAt = (DateTime)r["updated_at"],
        };
    }
}
