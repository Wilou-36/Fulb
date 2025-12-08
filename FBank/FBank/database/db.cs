using System;
using System.Data;
using System.Configuration;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FBank.database
{
    public partial class db : Form
    {
        public db()
        {
            InitializeComponent();
        }

        private void db_Load(object sender, EventArgs e)
        {
            if (DbManager.TestConnection(out string error))
            {
                // Connexion OK.
            }
            else
            {
                MessageBox.Show($"Erreur de connexion : {error}", "DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public static class DbManager
    {
        private static readonly string ConnectionString;

        static DbManager()
        {
            // Lecture de la chaîne depuis App.config si présente.
            var cfg = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
            if (!string.IsNullOrWhiteSpace(cfg))
            {
                ConnectionString = cfg;
                return;
            }

            // Sinon construire depuis les variables d'environnement (avec valeurs par défaut fournies).
            var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "192.168.56.154";
            var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
            var name = Environment.GetEnvironmentVariable("DB_NAME") ?? "Fulb";
            var user = Environment.GetEnvironmentVariable("DB_USER") ?? "Fulb";
            var pass = Environment.GetEnvironmentVariable("DB_PASS") ?? "Fulb";
            var charset = Environment.GetEnvironmentVariable("DB_CHARSET") ?? "utf8mb4";

            ConnectionString = $"Server={host};Port={port};Database={name};Uid={user};Pwd={pass};Charset={charset};SslMode=Preferred;";
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public static bool TestConnection(out string errorMessage)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    errorMessage = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public static DataTable ExecuteQuery(string sql, params MySqlParameter[] parameters)
        {
            var dt = new DataTable();
            using (var conn = GetConnection())
            using (var cmd = new MySqlCommand(sql, conn))
            using (var adapter = new MySqlDataAdapter(cmd))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                adapter.Fill(dt);
            }

            return dt;
        }

        public static int ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, params MySqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}
