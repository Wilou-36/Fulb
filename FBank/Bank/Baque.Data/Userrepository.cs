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
    /// Accès aux données de la table <c>users</c>.
    /// </summary>
    internal class UserRepository
    {
        private readonly DbConnection _db = DbConnection.Instance;

        // ── CREATE ────────────────────────────────────────────────────────────
        /// <summary>Insère un nouvel utilisateur et retourne son Id généré.</summary>
        public int Add(User user)
        {
            const string sql = @"
        INSERT INTO users (nom, prenom, email, password, created_at, updated_at)
        OUTPUT INSERTED.id
        VALUES (@nom, @prenom, @email, @password, @createdAt, @updatedAt)";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@nom", user.Nom);
                cmd.Parameters.AddWithValue("@prenom", user.Prenom);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@createdAt", user.CreatedAt);
                cmd.Parameters.AddWithValue("@updatedAt", user.UpdatedAt);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // ── READ ──────────────────────────────────────────────────────────────
        /// <summary>Retourne un utilisateur par son Id, ou null s'il n'existe pas.</summary>
        public User GetById(int id)
        {
            const string sql = "SELECT * FROM users WHERE id = @id";

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

        /// <summary>Retourne un utilisateur par son email, ou null s'il n'existe pas.</summary>
        public User GetByEmail(string email)
        {
            const string sql = "SELECT * FROM users WHERE email = @email";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@email", email);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapRow(reader);

                    return null;
                }
            }
        }

        /// <summary>Retourne tous les utilisateurs.</summary>
        public List<User> GetAll()
        {
            const string sql = "SELECT * FROM users ORDER BY nom, prenom";

            List<User> users = new List<User>();

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(MapRow(reader));
                }
            }

            return users;
        }

        // ── UPDATE ────────────────────────────────────────────────────────────
        /// <summary>Met à jour les informations d'un utilisateur.</summary>
        public bool Update(User user)
        {
            const string sql = @"
        UPDATE users
        SET nom        = @nom,
            prenom     = @prenom,
            email      = @email,
            password   = @password,
            updated_at = @updatedAt
        WHERE id = @id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@nom", user.Nom);
                cmd.Parameters.AddWithValue("@prenom", user.Prenom);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@id", user.Id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        /// <summary>Supprime un utilisateur par son Id.</summary>
        public bool Delete(int id)
        {
            const string sql = "DELETE FROM users WHERE id = @id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ── Mapping ───────────────────────────────────────────────────────────
        private static User MapRow(SqlDataReader r) => new User()
        {
            Id = (int)r["id"],
            Nom = r["nom"].ToString(),
            Prenom = r["prenom"].ToString(),
            Email = r["email"].ToString(),
            Password = r["password"].ToString(),
            CreatedAt = (DateTime)r["created_at"],
            UpdatedAt = (DateTime)r["updated_at"],
        };
    }
}
