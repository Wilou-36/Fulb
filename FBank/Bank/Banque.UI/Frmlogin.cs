using System;
using System.Drawing;
using System.Windows.Forms;
using Bank.Banque.Business;
using Bank.Banque.Data;
using Bank.Banque.Models;

namespace Bank.Banque.UI
{
    internal class FrmLogin : Form
    {
        // ── Couleurs ──────────────────────────────────────────────────────────
        private static readonly Color BleuFonce = Color.FromArgb(26, 58, 107);
        private static readonly Color BleuMoyen = Color.FromArgb(42, 79, 143);
        private static readonly Color BleuClair = Color.FromArgb(208, 220, 236);
        private static readonly Color BlancPage = Color.FromArgb(240, 244, 248);
        private static readonly Color TextePrinc = Color.FromArgb(26, 42, 74);

        // ── Contrôles ─────────────────────────────────────────────────────────
        private TextBox txtEmail = new TextBox();
        private TextBox txtPassword = new TextBox();
        private Button btnLogin = new Button();
        private Button btnEye = new Button();
        private Label lblErreur = new Label();
        private Label lblStatus = new Label();
        private bool pwdVisible = false;

        // ── Services ──────────────────────────────────────────────────────────
        private readonly UserRepository _userRepo = new UserRepository();

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Fulbank — Connexion";
            this.Size = new Size(420, 420);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BlancPage;
            this.Font = new Font("Segoe UI", 9f);

            // ── En-tête ───────────────────────────────────────────────────────
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = BleuFonce
            };
            var lblTitre = new Label
            {
                Text = "Fulbank",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                AutoSize = false,
                Width = 420,
                Height = 55,
                TextAlign = ContentAlignment.BottomCenter,
                Location = new Point(0, 10)
            };
            var lblSous = new Label
            {
                Text = "Connexion à votre espace client",
                ForeColor = Color.FromArgb(180, 205, 230),
                Font = new Font("Segoe UI", 9f),
                AutoSize = false,
                Width = 420,
                Height = 22,
                TextAlign = ContentAlignment.TopCenter,
                Location = new Point(0, 65)
            };
            pnlHeader.Controls.AddRange(new Control[] { lblTitre, lblSous });
            this.Controls.Add(pnlHeader);

            int y = 115;

            // ── Email ─────────────────────────────────────────────────────────
            AddLabel("Adresse email :", new Point(40, y)); y += 22;
            txtEmail = CreateInput(new Point(40, y), "exemple@banque.fr"); y += 44;

            // ── Mot de passe ──────────────────────────────────────────────────
            AddLabel("Mot de passe :", new Point(40, y)); y += 22;
            txtPassword = CreateInput(new Point(40, y), "••••••••");
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Width = 290;

            btnEye = new Button
            {
                Text = "👁",
                Location = new Point(333, y),
                Size = new Size(47, 28),
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
            y += 44;

            // ── Erreur ────────────────────────────────────────────────────────
            lblErreur = new Label
            {
                Location = new Point(40, y),
                AutoSize = false,
                Width = 340,
                Height = 22,
                ForeColor = Color.FromArgb(192, 57, 43),
                Font = new Font("Segoe UI", 9f),
                Visible = false
            };
            this.Controls.Add(lblErreur);
            y += 28;

            // ── Bouton connexion ──────────────────────────────────────────────
            btnLogin = new Button
            {
                Text = "Se connecter",
                Location = new Point(40, y),
                Size = new Size(340, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = BleuFonce,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;
            this.AcceptButton = btnLogin;
            this.Controls.Add(btnLogin);

            // ── Lien inscription ──────────────────────────────────────────────
            var lnkInscription = new LinkLabel
            {
                Text = "Pas encore client ? Créer un compte",
                Location = new Point(40, y + 44),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                LinkColor = Color.FromArgb(26, 106, 191)
            };
            lnkInscription.Click += (s, e) => { this.Hide(); new FrmInscription().ShowDialog(); this.Show(); };
            this.Controls.Add(lnkInscription);

            // ── Barre de statut ───────────────────────────────────────────────
            var pnlStatus = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 26,
                BackColor = BleuClair
            };
            var dot = new Panel
            {
                Location = new Point(8, 9),
                Size = new Size(8, 8),
                BackColor = Color.FromArgb(42, 138, 42)
            };
            lblStatus = new Label
            {
                Text = "Prêt — Serveur connecté",
                Location = new Point(22, 4),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(58, 74, 106)
            };
            pnlStatus.Controls.AddRange(new Control[] { dot, lblStatus });
            this.Controls.Add(pnlStatus);
        }

        // ── Logique de connexion ──────────────────────────────────────────────
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblErreur.Visible = false;
            lblStatus.Text = "Authentification en cours...";

            string email = txtEmail.Text.Trim();
            string pwd = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pwd))
            {
                ShowError("Veuillez remplir tous les champs.");
                return;
            }

            try
            {
                var user = _userRepo.GetByEmail(email);

                if (user is null || !VerifierMotDePasse(pwd, user.Password))
                {
                    ShowError("Email ou mot de passe incorrect.");
                    return;
                }

                lblStatus.Text = "Connexion réussie — Chargement...";
                this.Hide();
                new FrmDashboard(user).ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Erreur de connexion : {ex.Message}");
            }
        }

        private void ShowError(string msg)
        {
            lblErreur.Text = "⚠  " + msg;
            lblErreur.Visible = true;
            lblStatus.Text = "Échec de connexion";
        }

        /// <summary>
        /// À remplacer par BCrypt.Verify() en production.
        /// </summary>
        private static bool VerifierMotDePasse(string saisi, string stocke) =>
            saisi == stocke;

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

        private TextBox CreateInput(Point loc, string placeholder)
        {
            var tb = new TextBox
            {
                Location = loc,
                Width = 340,
                Height = 28,
                Font = new Font("Segoe UI", 10f),
                ForeColor = TextePrinc,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(tb);
            return tb;
        }
    }
}