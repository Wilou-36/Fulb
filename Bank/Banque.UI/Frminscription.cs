using System;
using System.Drawing;
using System.Windows.Forms;
using Bank.Banque.Data;
using Bank.Banque.Models;
using Bank.Banque.Business;

namespace Bank.Banque.UI
{
    internal class FrmInscription : Form
    {
        // ── Couleurs ──────────────────────────────────────────────────────────
        private static readonly Color BleuFonce = Color.FromArgb(26, 58, 107);
        private static readonly Color BleuClair = Color.FromArgb(208, 220, 236);
        private static readonly Color BlancPage = Color.FromArgb(240, 244, 248);
        private static readonly Color TextePrinc = Color.FromArgb(26, 42, 74);
        private static readonly Color Vert = Color.FromArgb(42, 138, 42);

        // ── Contrôles ─────────────────────────────────────────────────────────
        private TextBox txtNom = new TextBox();
        private TextBox txtPrenom = new TextBox();
        private TextBox txtEmail = new TextBox();
        private TextBox txtPassword = new TextBox();
        private TextBox txtConfirm = new TextBox();
        private Button btnEye1 = new Button();
        private Button btnEye2 = new Button();
        private Label lblErreur = new Label();
        private Label lblSucces = new Label();
        private Button btnCreer = new Button();
        private Button btnRetour = new Button();
        private bool pwd1Visible = false;
        private bool pwd2Visible = false;

        // ── Repositories & Services ───────────────────────────────────────────
        private readonly UserRepository _userRepo = new UserRepository();
        private readonly AccountService _accountService = new AccountService();

        public FrmInscription()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Fulbank — Créer un compte";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BlancPage;
            this.Font = new Font("Segoe UI", 9f);

            // ── En-tête ───────────────────────────────────────────────────────
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = BleuFonce
            };
            pnlHeader.Controls.Add(new Label
            {
                Text = "  🏛  Fulbank",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                AutoSize = false,
                Width = 460,
                Height = 46,
                TextAlign = ContentAlignment.BottomLeft,
                Location = new Point(0, 4)
            });
            pnlHeader.Controls.Add(new Label
            {
                Text = "  Création de votre espace client",
                ForeColor = Color.FromArgb(180, 205, 230),
                Font = new Font("Segoe UI", 9f),
                AutoSize = false,
                Width = 460,
                Height = 24,
                TextAlign = ContentAlignment.TopLeft,
                Location = new Point(0, 52)
            });
            this.Controls.Add(pnlHeader);

            int y = 98;

            // ── Champs ────────────────────────────────────────────────────────
            AddLabel("Nom :", new Point(30, y)); y += 22;
            txtNom = CreateInput(new Point(30, y)); y += 44;

            AddLabel("Prénom :", new Point(30, y)); y += 22;
            txtPrenom = CreateInput(new Point(30, y)); y += 44;

            AddLabel("Adresse email :", new Point(30, y)); y += 22;
            txtEmail = CreateInput(new Point(30, y)); y += 44;

            // ── Mot de passe ──────────────────────────────────────────────────
            AddLabel("Mot de passe :", new Point(30, y)); y += 22;
            txtPassword = CreateInput(new Point(30, y));
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Width = 310;

            btnEye1 = CreateEyeBtn(new Point(344, y));
            btnEye1.Click += (s, e) =>
            {
                pwd1Visible = !pwd1Visible;
                txtPassword.UseSystemPasswordChar = !pwd1Visible;
                btnEye1.Text = pwd1Visible ? "🙈" : "👁";
            };
            y += 44;

            // ── Confirmation mot de passe ─────────────────────────────────────
            AddLabel("Confirmer le mot de passe :", new Point(30, y)); y += 22;
            txtConfirm = CreateInput(new Point(30, y));
            txtConfirm.UseSystemPasswordChar = true;
            txtConfirm.Width = 310;

            btnEye2 = CreateEyeBtn(new Point(344, y));
            btnEye2.Click += (s, e) =>
            {
                pwd2Visible = !pwd2Visible;
                txtConfirm.UseSystemPasswordChar = !pwd2Visible;
                btnEye2.Text = pwd2Visible ? "🙈" : "👁";
            };
            y += 44;

            // ── Message erreur ────────────────────────────────────────────────
            lblErreur = new Label
            {
                Location = new Point(30, y),
                AutoSize = false,
                Width = 400,
                Height = 36,
                ForeColor = Color.FromArgb(192, 57, 43),
                Font = new Font("Segoe UI", 9f),
                Visible = false
            };
            this.Controls.Add(lblErreur);

            // ── Message succès ────────────────────────────────────────────────
            lblSucces = new Label
            {
                Location = new Point(30, y),
                AutoSize = false,
                Width = 400,
                Height = 36,
                ForeColor = Vert,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Visible = false
            };
            this.Controls.Add(lblSucces);
            y += 44;

            // ── Bouton Créer ──────────────────────────────────────────────────
            btnCreer = new Button
            {
                Text = "✔  Créer mon compte",
                Location = new Point(30, y),
                Size = new Size(400, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = BleuFonce,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCreer.FlatAppearance.BorderSize = 0;
            btnCreer.Click += BtnCreer_Click;
            this.AcceptButton = btnCreer;
            this.Controls.Add(btnCreer);
            y += 46;

            // ── Bouton Retour ─────────────────────────────────────────────────
            btnRetour = new Button
            {
                Text = "← Retour à la connexion",
                Location = new Point(30, y),
                Size = new Size(400, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = BleuClair,
                ForeColor = TextePrinc,
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand
            };
            btnRetour.FlatAppearance.BorderColor = Color.FromArgb(112, 144, 184);
            btnRetour.FlatAppearance.BorderSize = 1;
            btnRetour.Click += (s, e) => { this.Close(); new FrmLogin().Show(); };
            this.Controls.Add(btnRetour);

            // ── Barre de statut ───────────────────────────────────────────────
            var pnlStatus = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 26,
                BackColor = BleuClair
            };
            pnlStatus.Controls.Add(new Label
            {
                Text = "  🔒 Connexion sécurisée — vos données sont chiffrées",
                Location = new Point(6, 4),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(58, 74, 106)
            });
            this.Controls.Add(pnlStatus);
        }

        // ── Logique de création ───────────────────────────────────────────────
        private void BtnCreer_Click(object sender, EventArgs e)
        {
            lblErreur.Visible = false;
            lblSucces.Visible = false;

            // ── Validation des champs ─────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(txtNom.Text) ||
                string.IsNullOrWhiteSpace(txtPrenom.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowError("Veuillez remplir tous les champs obligatoires.");
                return;
            }

            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                ShowError("Adresse email invalide.");
                return;
            }

            if (txtPassword.Text.Length < 6)
            {
                ShowError("Le mot de passe doit contenir au moins 6 caractères.");
                return;
            }

            if (txtPassword.Text != txtConfirm.Text)
            {
                ShowError("Les mots de passe ne correspondent pas.");
                return;
            }

            // ── Vérification unicité email ────────────────────────────────────
            if (_userRepo.GetByEmail(txtEmail.Text.Trim()) != null)
            {
                ShowError("Cette adresse email est déjà utilisée.");
                return;
            }

            try
            {
                btnCreer.Enabled = false;
                btnCreer.Text = "Création en cours...";

                // ── 1. Création de l'utilisateur ──────────────────────────────
                var user = new User
                {
                    Nom = txtNom.Text.Trim(),
                    Prenom = txtPrenom.Text.Trim(),
                    Email = txtEmail.Text.Trim().ToLower(),
                    Password = txtPassword.Text,   // ← hacher avec BCrypt en production
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                user.Id = _userRepo.Add(user);

                // ── 2. Création du compte bancaire associé ────────────────────
                var account = _accountService.CreerCompte(user.Id);

                // ── 3. Confirmation ───────────────────────────────────────────
                ShowSucces(
                    $"✔  Compte créé avec succès !\n" +
                    $"Numéro de compte : {account.NumAccount}");

                // Redirection automatique vers la connexion après 2,5 secondes
                var timer = new System.Windows.Forms.Timer { Interval = 2500 };
                timer.Tick += (s, ev) =>
                {
                    timer.Stop();
                    this.Close();
                    new FrmLogin().Show();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                btnCreer.Enabled = true;
                btnCreer.Text = "✔  Créer mon compte";
                ShowError($"Erreur : {ex.Message}");
            }
        }

        // ── Helpers d'affichage ───────────────────────────────────────────────
        private void ShowError(string msg)
        {
            lblSucces.Visible = false;
            lblErreur.Text = "⚠  " + msg;
            lblErreur.Visible = true;
        }

        private void ShowSucces(string msg)
        {
            lblErreur.Visible = false;
            lblSucces.Text = msg;
            lblSucces.Visible = true;
            btnCreer.Enabled = false;
        }

        // ── Helpers de construction ───────────────────────────────────────────
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

        private TextBox CreateInput(Point loc)
        {
            var tb = new TextBox
            {
                Location = loc,
                Width = 400,
                Height = 28,
                Font = new Font("Segoe UI", 10f),
                ForeColor = TextePrinc,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(tb);
            return tb;
        }

        private Button CreateEyeBtn(Point loc)
        {
            var btn = new Button
            {
                Text = "👁",
                Location = loc,
                Size = new Size(50, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(90, 112, 144),
                Font = new Font("Segoe UI", 11f),
                Cursor = Cursors.Hand,
                TabStop = false
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(112, 144, 184);
            btn.FlatAppearance.BorderSize = 1;
            this.Controls.Add(btn);
            return btn;
        }
    }
}