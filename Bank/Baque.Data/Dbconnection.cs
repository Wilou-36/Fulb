using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Banque.Data
{
    // ══════════════════════════════════════════════════════════════════════════
    //  DbConnection — Gestionnaire de connexion SQL Server
    //  Pattern : Singleton thread-safe (double-checked locking via lock)
    //
    //  Responsabilité unique : fournir une SqlConnection ouverte à tous
    //  les repositories (AccountRepository, UserRepository, etc.)
    //  sans dupliquer la chaîne de connexion dans chaque classe.
    //
    //  Configuration : les paramètres sont lus depuis les variables
    //  d'environnement au démarrage, avec des valeurs par défaut codées en dur.
    //  Cela permet de déployer sur différents environnements (dev, VM, prod)
    //  sans recompiler le projet.
    //
    //  Variables d'environnement supportées :
    //    DB_HOST → adresse IP ou hostname du serveur SQL Server
    //    DB_NAME → nom de la base de données
    //    DB_USER → identifiant SQL Server
    //    DB_PASS → mot de passe SQL Server
    // ══════════════════════════════════════════════════════════════════════════
    internal class DbConnection
    {
        // ── Singleton thread-safe ─────────────────────────────────────────────
        // _instance : référence unique partagée dans toute l'application
        // _lock     : objet de verrouillage pour garantir la thread-safety
        private static DbConnection _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Point d'accès global à l'instance unique de DbConnection.
        /// Utilise un verrou pour éviter les créations concurrentes
        /// dans un contexte multi-thread.
        /// </summary>
        public static DbConnection Instance
        {
            get
            {
                lock (_lock)
                {
                    // Création de l'instance uniquement si elle n'existe pas encore
                    if (_instance == null)
                    {
                        _instance = new DbConnection();
                    }

                    return _instance;
                }
            }
        }

        // ── Chaîne de connexion ───────────────────────────────────────────────
        // Construite une seule fois dans le constructeur privé
        // et réutilisée à chaque appel à GetConnection()
        private readonly string _connectionString;

        /// <summary>
        /// Constructeur privé : empêche l'instanciation directe depuis l'extérieur.
        /// Lit les paramètres de connexion depuis les variables d'environnement
        /// avec des valeurs de repli (fallback) pour le développement local.
        /// </summary>
        private DbConnection()
        {
            // Lecture des variables d'environnement
            // ?? "valeur" = valeur par défaut si la variable n'est pas définie
            var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "10.23.180.38";
            var name = Environment.GetEnvironmentVariable("DB_NAME") ?? "F_bank";
            var user = Environment.GetEnvironmentVariable("DB_USER") ?? "F_Admin";
            var pass = Environment.GetEnvironmentVariable("DB_PASS") ?? "Fu1b@nk";

            // Construction de la chaîne de connexion SQL Server
            // Uid/Pwd = authentification SQL Server (pas Windows Authentication)
            _connectionString = $"Server={host};Database={name};Uid={user};Pwd={pass};";
        }

        // ══════════════════════════════════════════════════════════════════════
        //  MÉTHODES PUBLIQUES
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Crée, ouvre et retourne une nouvelle connexion SQL Server.
        /// ⚠ Le code appelant est responsable de la fermeture via un bloc using.
        /// Exemple d'utilisation :
        ///   using (var conn = _db.GetConnection())
        ///   using (var cmd = new SqlCommand(sql, conn)) { ... }
        /// </summary>
        /// <returns>Une <see cref="SqlConnection"/> dans l'état Open.</returns>
        public SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);

            // Ouverture uniquement si pas déjà ouverte
            // (protection contre les appels redondants)
            if (connection.State != ConnectionState.Open)
                connection.Open();

            return connection;
        }

        /// <summary>
        /// Teste la connectivité à la base de données.
        /// À appeler au démarrage de l'application (ex: dans FrmLogin.Load)
        /// pour afficher une erreur claire si le serveur est inaccessible.
        /// </summary>
        /// <returns>
        /// true  → la connexion est établie avec succès.
        /// false → le serveur est inaccessible (IP incorrecte, port fermé, credentials invalides…).
        /// </returns>
        public bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    // Vérifie que la connexion est bien dans l'état Open
                    return conn.State == ConnectionState.Open;
                }
            }
            catch
            {
                // Toute exception (SqlException, timeout…) retourne false
                // sans propager l'erreur — l'appelant gère l'affichage du message
                return false;
            }
        }
    }
}