using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Banque.Data
{
    /// <summary>
    /// Gère la connexion unique à la base de données SQL Server.
    /// Pattern Singleton — une seule chaîne de connexion dans toute l'application.
    /// </summary>
    internal class DbConnection
    {
        // ── Singleton ─────────────────────────────────────────────────────────
        private static DbConnection _instance;
        private static readonly object _lock = new object();

        public static DbConnection Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DbConnection();
                    }

                    return _instance;
                }
            }
        }

        // ── Chaîne de connexion ───────────────────────────────────────────────
        private readonly string _connectionString;

        private DbConnection()
        {
            var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "10.23.180.38";
            var name = Environment.GetEnvironmentVariable("DB_NAME") ?? "F_bank";
            var user = Environment.GetEnvironmentVariable("DB_USER") ?? "F_Admin";
            var pass = Environment.GetEnvironmentVariable("DB_PASS") ?? "Fu1b@nk";

            // Modifier Server, Database, User Id et Password selon l'environnement
            _connectionString = $"Server={host};Database={name};Uid={user};Pwd={pass};";
        }

        // ── Méthode principale ────────────────────────────────────────────────
        /// <summary>
        /// Ouvre et retourne une nouvelle connexion SQL prête à l'emploi.
        /// Le appelant est responsable de la fermer (using).
        /// </summary>
        public SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);

            if (connection.State != ConnectionState.Open)
                connection.Open();

            return connection;
        }

        // ── Test de connectivité ──────────────────────────────────────────────
        /// <summary>
        /// Vérifie que la base de données est joignable.
        /// Utile au démarrage de l'application.
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    return conn.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
