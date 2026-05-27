using System;
using System.Data;
using System.Data.SqlClient;
using Bank.Banque.Config;

namespace Bank.Banque.Data
{
    // ══════════════════════════════════════════════════════════════════════════
    //  DbConnection — Gestionnaire de connexion SQL Server
    //  Pattern Singleton thread-safe.
    //
    //  ✔ La chaîne de connexion est lue depuis AppConfig (appsettings.json).
    //  ✔ Pour changer de base de données : modifier appsettings.json UNIQUEMENT.
    //  ✔ Aucune recompilation nécessaire.
    // ══════════════════════════════════════════════════════════════════════════
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
                        _instance = new DbConnection();
                    return _instance;
                }
            }
        }

        // ── Chaîne de connexion lue depuis AppConfig ──────────────────────────
        // AppConfig lit appsettings.json puis les variables d'environnement
        private readonly string _connectionString;

        private DbConnection()
        {
            // Délègue entièrement à AppConfig — aucune valeur codée en dur ici
            _connectionString = AppConfig.Instance.ConnectionString;
        }

        // ── Obtenir une connexion ouverte ─────────────────────────────────────
        public SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return connection;
        }

        // ── Test de connectivité ──────────────────────────────────────────────
        public bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                    return conn.State == ConnectionState.Open;
            }
            catch { return false; }
        }

        // ── Rechargement (après modification de appsettings.json) ─────────────
        /// <summary>
        /// Recrée l'instance avec la nouvelle configuration.
        /// Appeler après AppConfig.Recharger() dans FrmParamConnexion.
        /// </summary>
        public static void Recharger()
        {
            lock (_lock) { _instance = null; }
        }
    }
}