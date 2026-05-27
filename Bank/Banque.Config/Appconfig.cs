using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace Bank.Banque.Config
{
	// ══════════════════════════════════════════════════════════════════════════
	//  AppConfig — Lecture centralisée du fichier appsettings.json
	//  Permet de changer la base de données SANS recompiler le projet.
	//
	//  Fichier lu : appsettings.json (à côté de l'exécutable .exe)
	//
	//  Ordre de priorité pour la connexion :
	//    1. Variables d'environnement (DB_HOST, DB_NAME, DB_USER, DB_PASS)
	//    2. appsettings.json
	//    3. Valeurs par défaut codées en dur (fallback ultime)
	//
	//  Utilisation :
	//    string cs = AppConfig.Instance.ConnectionString;
	// ══════════════════════════════════════════════════════════════════════════
	internal class AppConfig
	{
		// ── Singleton ─────────────────────────────────────────────────────────
		private static AppConfig _instance;
		private static readonly object _lock = new object();

		public static AppConfig Instance
		{
			get
			{
				lock (_lock)
				{
					if (_instance == null)
						_instance = new AppConfig();
					return _instance;
				}
			}
		}

		// ── Propriétés exposées ───────────────────────────────────────────────
		public string Host { get; private set; }
		public string Database { get; private set; }
		public string User { get; private set; }
		public string Password { get; private set; }
		public int Port { get; private set; }
		public bool TrustCert { get; private set; }
		public int Timeout { get; private set; }
		public string AppNom { get; private set; }
		public string Version { get; private set; }

		// ── Chaîne de connexion construite automatiquement ────────────────────
		public string ConnectionString =>
			$"Server={Host},{Port};" +
			$"Database={Database};" +
			$"Uid={User};" +
			$"Pwd={Password};" +
			$"Connect Timeout={Timeout};" +
			$"TrustServerCertificate={(TrustCert ? "True" : "False")};";

		// ── Constructeur privé : lecture du fichier ───────────────────────────
		private AppConfig()
		{
			// Valeurs par défaut (fallback si fichier absent ou mal formé)
			Host = "10.23.180.38";
			Database = "F_bank";
			User = "F_Admin";
			Password = "Fu1b@nk";
			Port = 1433;
			TrustCert = true;
			Timeout = 30;
			AppNom = "FulBank";
			Version = "1.0.0";

			// Tentative de lecture du fichier appsettings.json
			TryLoadFromFile();

			// Les variables d'environnement écrasent toujours le fichier
			// (utile pour les déploiements automatisés ou les VM)
			OverrideFromEnvironment();
		}

		// ══════════════════════════════════════════════════════════════════════
		//  LECTURE DU FICHIER JSON
		// ══════════════════════════════════════════════════════════════════════

		private void TryLoadFromFile()
		{
			try
			{
				// Chemin du fichier : même dossier que l'exécutable
				string exeDir = AppDomain.CurrentDomain.BaseDirectory;
				string path = Path.Combine(exeDir, "appsettings.json");

				if (!File.Exists(path))
				{
					// Fichier absent → on affiche une info et on garde les valeurs par défaut
					Console.WriteLine($"[AppConfig] appsettings.json introuvable à : {path}");
					Console.WriteLine("[AppConfig] Utilisation des valeurs par défaut.");
					return;
				}

				string json = File.ReadAllText(path);
				var doc = JsonDocument.Parse(json);
				var root = doc.RootElement;

				// Lecture de la section "Database"
				if (root.TryGetProperty("Database", out var db))
				{
					if (db.TryGetProperty("Host", out var h)) Host = h.GetString() ?? Host;
					if (db.TryGetProperty("Name", out var n)) Database = n.GetString() ?? Database;
					if (db.TryGetProperty("User", out var u)) User = u.GetString() ?? User;
					if (db.TryGetProperty("Password", out var p)) Password = p.GetString() ?? Password;
					if (db.TryGetProperty("Port", out var po))
					{
						// Accepte "1433" (string) ou 1433 (nombre)
						if (po.ValueKind == JsonValueKind.Number)
							Port = po.GetInt32();
						else if (int.TryParse(po.GetString(), out int portParsed))
							Port = portParsed;
					}
					if (db.TryGetProperty("TrustServerCertificate", out var t))
						TrustCert = t.GetBoolean();
					if (db.TryGetProperty("ConnectTimeout", out var ct))
						Timeout = ct.GetInt32();
				}

				// Lecture de la section "Application"
				if (root.TryGetProperty("Application", out var app))
				{
					if (app.TryGetProperty("Nom", out var nom)) AppNom = nom.GetString() ?? AppNom;
					if (app.TryGetProperty("Version", out var ver)) Version = ver.GetString() ?? Version;
				}

				Console.WriteLine($"[AppConfig] Connexion chargée → {Host}:{Port}/{Database}");
			}
			catch (Exception ex)
			{
				// Fichier mal formé → on garde les valeurs par défaut
				Console.WriteLine($"[AppConfig] Erreur lecture appsettings.json : {ex.Message}");
			}
		}

		// ══════════════════════════════════════════════════════════════════════
		//  VARIABLES D'ENVIRONNEMENT (priorité maximale)
		// ══════════════════════════════════════════════════════════════════════

		private void OverrideFromEnvironment()
		{
			// Une variable d'environnement définie écrase toujours le fichier JSON
			string envHost = Environment.GetEnvironmentVariable("DB_HOST");
			string envName = Environment.GetEnvironmentVariable("DB_NAME");
			string envUser = Environment.GetEnvironmentVariable("DB_USER");
			string envPass = Environment.GetEnvironmentVariable("DB_PASS");
			string envPort = Environment.GetEnvironmentVariable("DB_PORT");

			if (!string.IsNullOrWhiteSpace(envHost)) Host = envHost;
			if (!string.IsNullOrWhiteSpace(envName)) Database = envName;
			if (!string.IsNullOrWhiteSpace(envUser)) User = envUser;
			if (!string.IsNullOrWhiteSpace(envPass)) Password = envPass;
			if (!string.IsNullOrWhiteSpace(envPort) && int.TryParse(envPort, out int p)) Port = p;
		}

		// ══════════════════════════════════════════════════════════════════════
		//  RECHARGEMENT À CHAUD (optionnel)
		// ══════════════════════════════════════════════════════════════════════

		/// <summary>
		/// Recharge la configuration depuis le fichier sans redémarrer l'application.
		/// Utile pour FrmParamConnexion : l'utilisateur modifie le fichier
		/// puis clique sur "Tester" → on recharge sans relancer l'exe.
		/// </summary>
		public static void Recharger()
		{
			lock (_lock)
			{
				_instance = new AppConfig();
			}
		}
	}
}