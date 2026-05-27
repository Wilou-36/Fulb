using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using Bank.Banque.Config;
using Bank.Banque.Data;

namespace Bank.Banque.UI
{
    // ══════════════════════════════════════════════════════════════════════════
    //  FrmParamConnexion — Interface graphique de configuration de la base
    //  Accessible depuis FrmLogin via un bouton "Paramètres".
    //
    //  Permet à l'utilisateur de :
    //    - Modifier l'IP / nom d'hôte du serveur SQL
    //    - Changer le nom de la base, le login et le mot de passe
    //    - Tester la connexion en temps réel
    //    - Sauvegarder dans appsettings.json sans recompiler
    // ══════════════════════════════════════════════════════════════════════════
    internal class FrmParamConnexion : Form
    {
        private static readonly Color BleuFonce = Color.FromArgb(26, 58, 107);
        private static readonly Color BleuClair = Color.FromArgb(208, 220, 236);
        private static readonly Color BlancPage = Color.FromArgb(240, 244, 248);
        private static readonly Color TextePrinc = Color.FromArgb(26, 42, 74);
        private static readonly Color Vert = Color.FromArgb(42, 138, 42);

        // ── Contrôles ─────────────────────────────────────────────────────────
        private TextBox txtHost = new TextBox();
        private TextBox txtPort = new TextBox();
        private TextBox txtDatabase = new TextBox();
        private TextBox txtUser = new TextBox();
        private TextBox txtPassword = new TextBox();
        private Button btnEye = new Button();
        private Label lblStatus = new Label();
        private Button btnTester = new Button();
        private Button btnSauver = new Button();
        private bool pwdVisible = false;

        public FrmParamConnexion()
        {
            InitializeComponent();
            ChargerValeurs();
        }

        private void InitializeComponent()
        {
            this.Text = "FulBank — Paramètres de connexion";
            this.Size = new Size(460, 480);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = BlancPage;
            this.Font = new Font("Segoe UI", 9f);

            // ── En-tête ───────────────────────────────────────────────────────
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = BleuFonce };
            pnlHeader.Controls.Add(new Label
            {
                Text = "  ⚙  Configuration de la base de données",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            });
            this.Controls.Add(pnlHeader);

            int y = 72;

            // ── Ligne Serveur + Port ──────────────────────────────────────────
            AddLabel("Serveur (IP ou hostname) :", new Point(30, y)); y += 22;
            txtHost = CreateInput(new Point(30, y), 290);

            AddLabel("Port :", new Point(330, y - 22));
            txtPort = CreateInput(new Point(330, y), 100);
            y += 44;

            // ── Base de données ───────────────────────────────────────────────
            AddLabel("Base de données :", new Point(30, y)); y += 22;
            txtDatabase = CreateInput(new Point(30, y), 400); y += 44;

            // ── Utilisateur ───────────────────────────────────────────────────
            AddLabel("Identifiant SQL :", new Point(30, y)); y += 22;
            txtUser = CreateInput(new Point(30, y), 400); y += 44;

            // ── Mot de passe ──────────────────────────────────────────────────
            AddLabel("Mot de passe :", new Point(30, y)); y += 22;
            txtPassword = CreateInput(new Point(30, y), 354);
            txtPassword.UseSystemPasswordChar = true;

            btnEye = new Button
            {
                Text = "👁",
                Location = new Point(387, y),
                Size = new Size(43, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(90, 112, 144),
                Font = new Font("Segoe UI", 11f),
                Cursor = Cursors.Hand,
                TabStop = false
            };
            btnEye.FlatAppearance.BorderColor = Color.FromArgb(112, 144, 184);
            btnEye.Click += (s, e) =>
            {
                pwdVisible = !pwdVisible;
                txtPassword.UseSystemPasswordChar = !pwdVisible;
                btnEye.Text = pwdVisible ? "🙈" : "👁";
            };
            this.Controls.Add(btnEye);
            y += 48;

            // ── Aperçu chaîne de connexion ────────────────────────────────────
            var lblApercu = new Label
            {
                Text = "Aperçu chaîne de connexion :",
                Location = new Point(30, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = TextePrinc
            };
            this.Controls.Add(lblApercu);
            y += 20;

            lblStatus = new Label
            {
                Location = new Point(30, y),
                AutoSize = false,
                Width = 400,
                Height = 40,
                Font = new Font("Courier New", 8f),
                ForeColor = Color.FromArgb(60, 90, 130),
                BackColor = Color.FromArgb(230, 238, 250)
            };
            this.Controls.Add(lblStatus);
            y += 50;

            // ── Boutons ───────────────────────────────────────────────────────
            btnTester = new Button
            {
                Text = "🔌  Tester la connexion",
                Location = new Point(30, y),
                Size = new Size(185, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = BleuClair,
                ForeColor = BleuFonce,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTester.FlatAppearance.BorderColor = Color.FromArgb(112, 144, 184);
            btnTester.Click += BtnTester_Click;
            this.Controls.Add(btnTester);

            btnSauver = new Button
            {
                Text = "💾  Enregistrer",
                Location = new Point(245, y),
                Size = new Size(185, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = BleuFonce,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSauver.FlatAppearance.BorderSize = 0;
            btnSauver.Click += BtnSauver_Click;
            this.Controls.Add(btnSauver);

            // Mise à jour de l'aperçu à chaque frappe
            txtHost.TextChanged += (s, e) => UpdateApercu();
            txtPort.TextChanged += (s, e) => UpdateApercu();
            txtDatabase.TextChanged += (s, e) => UpdateApercu();
            txtUser.TextChanged += (s, e) => UpdateApercu();
            txtPassword.TextChanged += (s, e) => UpdateApercu();
        }

        // ── Chargement des valeurs depuis AppConfig ───────────────────────────
        private void ChargerValeurs()
        {
            var cfg = AppConfig.Instance;
            txtHost.Text = cfg.Host;
            txtPort.Text = cfg.Port.ToString();
            txtDatabase.Text = cfg.Database;
            txtUser.Text = cfg.User;
            txtPassword.Text = cfg.Password;
            UpdateApercu();
        }

        // ── Mise à jour de l'aperçu en temps réel ────────────────────────────
        private void UpdateApercu()
        {
            lblStatus.Text = $"Server={txtHost.Text},{txtPort.Text};" +
                             $"Database={txtDatabase.Text};" +
                             $"Uid={txtUser.Text};Pwd=***;";
        }

        // ── Test de connexion ─────────────────────────────────────────────────
        private void BtnTester_Click(object sender, EventArgs e)
        {
            btnTester.Enabled = false;
            btnTester.Text = "Test en cours...";

            try
            {
                // Sauvegarde temporaire pour tester sans modifier le fichier
                SauvegarderFichier(temporaire: true);
                AppConfig.Recharger();
                DbConnection.Recharger();

                bool ok = DbConnection.Instance.TestConnection();

                if (ok)
                {
                    btnTester.Text = "✔  Connexion réussie !";
                    btnTester.BackColor = Color.FromArgb(200, 235, 200);
                    btnTester.ForeColor = Vert;
                }
                else
                {
                    btnTester.Text = "✘  Connexion échouée";
                    btnTester.BackColor = Color.FromArgb(255, 220, 220);
                    btnTester.ForeColor = Color.FromArgb(180, 30, 30);
                }
            }
            catch (Exception ex)
            {
                btnTester.Text = "✘  Erreur";
                btnTester.BackColor = Color.FromArgb(255, 220, 220);
                MessageBox.Show($"Erreur : {ex.Message}", "Test de connexion",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTester.Enabled = true;
                // Réinitialisation du style après 3 secondes
                var t = new System.Windows.Forms.Timer { Interval = 3000 };
                t.Tick += (s, ev) =>
                {
                    t.Stop();
                    btnTester.Text = "🔌  Tester la connexion";
                    btnTester.BackColor = BleuClair;
                    btnTester.ForeColor = BleuFonce;
                };
                t.Start();
            }
        }

        // ── Sauvegarde définitive ─────────────────────────────────────────────
        private void BtnSauver_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHost.Text) ||
                string.IsNullOrWhiteSpace(txtDatabase.Text) ||
                string.IsNullOrWhiteSpace(txtUser.Text))
            {
                MessageBox.Show("Serveur, base de données et identifiant sont obligatoires.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SauvegarderFichier(temporaire: false);
                AppConfig.Recharger();
                DbConnection.Recharger();

                MessageBox.Show("Paramètres enregistrés dans appsettings.json.\n" +
                                "La nouvelle configuration est active immédiatement.",
                    "Enregistré", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible d'écrire le fichier : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Écriture du fichier appsettings.json ──────────────────────────────
        private void SauvegarderFichier(bool temporaire)
        {
            var config = new
            {
                Database = new
                {
                    Host = txtHost.Text.Trim(),
                    Name = txtDatabase.Text.Trim(),
                    User = txtUser.Text.Trim(),
                    Password = txtPassword.Text,
                    Port = int.TryParse(txtPort.Text, out int p) ? p : 1433,
                    TrustServerCertificate = true,
                    ConnectTimeout = 30
                },
                Application = new
                {
                    Nom = AppConfig.Instance.AppNom,
                    Version = AppConfig.Instance.Version
                }
            };

            string json = JsonSerializer.Serialize(config,
                new JsonSerializerOptions { WriteIndented = true });

            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string fichier = temporaire ? "appsettings.tmp.json" : "appsettings.json";
            string path = Path.Combine(exeDir, fichier);

            File.WriteAllText(path, json);
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private void AddLabel(string text, Point loc)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Location = loc,
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = TextePrinc
            });
        }

        private TextBox CreateInput(Point loc, int width)
        {
            var tb = new TextBox
            {
                Location = loc,
                Width = width,
                Height = 28,
                Font = new Font("Segoe UI", 10f),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(tb);
            return tb;
        }
    }
}