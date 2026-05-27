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
    //  UserRepository — Accès aux données de la table users
    //
    //  Opérations CRUD disponibles :
    //    Add()       → INSERT — crée un utilisateur, retourne l'Id généré
    //    GetById()   → SELECT WHERE id
    //    GetByEmail()→ SELECT WHERE email (utilisé par FrmLogin pour l'auth)
    //    GetAll()    → SELECT tous les utilisateurs, trié nom/prénom
    //    Update()    → UPDATE toutes les colonnes modifiables
    //    Delete()    → DELETE WHERE id
    //
    //  Toutes les requêtes utilisent des paramètres (@param) pour prévenir
    //  les injections SQL. Chaque méthode ouvre et ferme sa propre connexion
    //  via DbConnection.Instance.GetConnection() dans un bloc using.
    // ══════════════════════════════════════════════════════════════════════════
    internal class UserRepository
    {
        // Instance Singleton de DbConnection — partagée avec tous les repositories
        private readonly DbConnection _db = DbConnection.Instance;

        // ══════════════════════════════════════════════════════════════════════
        //  CREATE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Insère un nouvel utilisateur en base et retourne son Id généré.
        /// OUTPUT INSERTED.id permet de récupérer l'Id sans requête supplémentaire.
        /// Appelé depuis FrmInscription après validation des champs.
        /// </summary>
        /// <param name="user">Objet User à insérer (Id non requis, généré par SQL).</param>
        /// <returns>Id auto-incrémenté attribué par SQL Server (IDENTITY).</returns>
        public int Add(User user)
        {
            // OUTPUT INSERTED.id : retourne directement l'Id généré par IDENTITY
            const string sql = @"
        INSERT INTO users (nom, prenom, email, password, created_at, updated_at)
        OUTPUT INSERTED.id
        VALUES (@nom, @prenom, @email, @password, @createdAt, @updatedAt)";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                // Paramètres nommés → protection contre les injections SQL
                cmd.Parameters.AddWithValue("@nom", user.Nom);
                cmd.Parameters.AddWithValue("@prenom", user.Prenom);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@createdAt", user.CreatedAt);
                cmd.Parameters.AddWithValue("@updatedAt", user.UpdatedAt);

                // ExecuteScalar() retourne la première colonne de la première ligne
                // Convert.ToInt32 évite un InvalidCastException si le résultat est DBNull
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  READ
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Retourne un utilisateur par sa clé primaire (id).
        /// Utilisé par AccountService.CreerCompte() pour vérifier l'existence
        /// de l'utilisateur avant la création d'un compte.
        /// </summary>
        /// <param name="id">Clé primaire de l'utilisateur.</param>
        /// <returns>L'objet <see cref="User"/> ou null si introuvable.</returns>
        public User GetById(int id)
        {
            const string sql = "SELECT * FROM users WHERE id = @id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    // Read() retourne true si une ligne a été trouvée
                    if (reader.Read())
                        return MapRow(reader);

                    return null; // aucun utilisateur avec cet id
                }
            }
        }

        /// <summary>
        /// Retourne un utilisateur par son email.
        /// Méthode principale utilisée par FrmLogin pour l'authentification :
        /// l'email sert d'identifiant unique (contrainte UNIQUE en base).
        /// Également utilisée par FrmInscription pour vérifier l'unicité de l'email.
        /// </summary>
        /// <param name="email">Adresse email de l'utilisateur.</param>
        /// <returns>L'objet <see cref="User"/> ou null si aucun compte avec cet email.</returns>
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

                    return null; // email non enregistré
                }
            }
        }

        /// <summary>
        /// Retourne la liste complète de tous les utilisateurs,
        /// triée alphabétiquement par nom puis prénom.
        /// Utile pour une interface d'administration ou des exports.
        /// </summary>
        /// <returns>Liste de tous les <see cref="User"/>. Vide si aucun.</returns>
        public List<User> GetAll()
        {
            // ORDER BY nom, prenom → tri alphabétique pour affichage en liste
            const string sql = "SELECT * FROM users ORDER BY nom, prenom";

            List<User> users = new List<User>();

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                // Lecture de toutes les lignes retournées
                while (reader.Read())
                {
                    users.Add(MapRow(reader));
                }
            }

            return users;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  UPDATE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Met à jour les informations personnelles d'un utilisateur existant.
        /// updated_at est forcé à DateTime.UtcNow pour tracer la modification.
        /// </summary>
        /// <param name="user">Objet User avec les nouvelles valeurs (l'Id doit être renseigné).</param>
        /// <returns>true si au moins une ligne a été modifiée, false sinon.</returns>
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
                cmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow); // horodatage automatique
                cmd.Parameters.AddWithValue("@id", user.Id);

                // ExecuteNonQuery() retourne le nombre de lignes affectées
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  DELETE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Supprime un utilisateur par son Id.
        /// ⚠ CASCADE DELETE en base : tous les comptes (accounts) associés
        /// seront également supprimés automatiquement via la FK user_id.
        /// </summary>
        /// <param name="id">Id de l'utilisateur à supprimer.</param>
        /// <returns>true si la suppression a affecté au moins une ligne.</returns>
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

        // ══════════════════════════════════════════════════════════════════════
        //  MAPPING SqlDataReader → User
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Convertit une ligne du SqlDataReader en objet <see cref="User"/>.
        /// Méthode privée et statique : ne dépend d'aucun état de l'instance.
        /// Centralisée ici pour éviter la duplication dans chaque méthode READ.
        /// Les noms de colonnes doivent correspondre exactement à ceux de la table users.
        /// </summary>
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