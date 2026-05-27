using System;
using System.Drawing;
using System.Windows.Forms;
using Bank.Banque.Business;
using Bank.Banque.Data;
using Bank.Banque.Models;

namespace Bank.Banque.UI
{
    // ══════════════════════════════════════════════════════════════════════════
    //  FrmLogin — Formulaire de connexion utilisateur
    //  Premier formulaire affiché au lancement de l'application (voir Program.cs).
    //
    //  Flux :
    //    1. L'utilisateur saisit son email + mot de passe
    //    2. BtnLogin_Click() interroge UserRepository.GetByEmail()
    //    3. Si valide → ouvre FrmDashboard et se ferme
    //    4. Si invalide → affiche un message d'erreur en rouge
    //
    //  Lien "Pas encore client ?" → ouvre FrmInscription en dialog
    //  puis revient sur FrmLogin.
    // ══════════════════════════════════════════════════════════════════════════
    internal class FrmLogin : Form
    {
        // ── Palette de couleurs (charte graphique Fulbank) ────────────────────
        // Utilisées sur tous les formulaires pour assurer la cohérence visuelle
        private static readonly Color BleuFonce = Color.FromArgb(26, 58, 107);
        private static readonly Color BleuMoyen = Color.FromArgb(42, 79, 143);
        private static readonly Color BleuClair = Color.FromArgb(208, 220, 236);
        private static readonly Color BlancPage = Color.FromArgb(240, 244, 248);
        private static readonly Color TextePrinc = Color.FromArgb(26, 42, 74);

        // ── Contrôles du formulaire ───────────────────────────────────────────
        private TextBox txtEmail = new TextBox();   // champ email (identifiant)
        private TextBox txtPassword = new TextBox();   // champ mot de passe (masqué)
        private Button btnLogin = new Button();    // bouton de soumission
        private Button btnEye = new Button();    // bouton afficher/masquer mdp
        private Label lblErreur = new Label();     // message d'erreur (rouge)
        private Label lblStatus = new Label();     // barre de statut en bas
        private bool pwdVisible = false;           // état d'affichage du mdp

        // ── Accès aux données ─────────────────────────────────────────────────
        // UserRepository utilisé directement ici car FrmLogin n'a besoin
        // que de la lecture par email — pas de logique métier complexe
        private readonly UserRepository _userRepo = new UserRepository();

        public FrmLogin()
        {
            InitializeComponent();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CONSTRUCTION DE L'INTERFACE
        // ══════════════════════════════════════════════════════════════════════
        private void InitializeComponent()
        {
            this.Text = "Fulbank — Connexion";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BlancPage;
            this.Font = new Font("Segoe UI", 9f);

            // ── En-tête bleu : logo + sous-titre ──────────────────────────────
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

            // y = curseur vertical pour positionner les contrôles dynamiquement
            int y = 115;

            // ── Champ email ───────────────────────────────────────────────────
            // Sert d'identifiant unique — correspond à users.email (UNIQUE NOT NULL)
            AddLabel("Adresse email :", new Point(40, y)); y += 22;
            txtEmail = CreateInput(new Point(40, y), "exemple@banque.fr"); y += 44;

            // ── Champ mot de passe ────────────────────────────────────────────
            // Masqué par défaut via UseSystemPasswordChar
            // Le bouton btnEye permet de basculer l'affichage
            AddLabel("Mot de passe :", new Point(40, y)); y += 22;
            txtPassword = CreateInput(new Point(40, y), "••••••••");
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Width = 290; // réduit pour laisser la place au bouton œil

            // Bouton afficher / masquer le mot de passe
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
                TabStop = false  // exclu de la navigation par Tab
            };
            btnEye.FlatAppearance.BorderColor = Color.FromArgb(112, 144, 184);
            btnEye.Click += (s, e) =>
            {
                // Bascule entre texte visible et caractères masqués
                pwdVisible = !pwdVisible;
                txtPassword.UseSystemPasswordChar = !pwdVisible;
                btnEye.Text = pwdVisible ? "🙈" : "👁";
            };
            this.Controls.Add(btnEye);
            y += 44;

            // ── Zone d'affichage des erreurs ──────────────────────────────────
            // Masquée par défaut, rendue visible par ShowError()
            lblErreur = new Label
            {
                Location = new Point(40, y),
                AutoSize = false,
                Width = 340,
                Height = 22,
                ForeColor = Color.FromArgb(192, 57, 43), // rouge erreur
                Font = new Font("Segoe UI", 9f),
                Visible = false
            };
            this.Controls.Add(lblErreur);
            y += 28;

            // ── Bouton principal de connexion ─────────────────────────────────
            // AcceptButton = déclenché par la touche Entrée
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

            // ── Lien vers le formulaire d'inscription ─────────────────────────
            // ShowDialog() bloque FrmLogin tant que FrmInscription est ouverte
            // puis Show() le remet en avant après fermeture
            var lnkInscription = new LinkLabel
            {
                Text = "Pas encore client ? Créer un compte",
                Location = new Point(40, y + 44),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                LinkColor = Color.FromArgb(26, 106, 191)
            };
            lnkInscription.Click += (s, e) =>
            {
                this.Hide();
                new FrmInscription().ShowDialog();
                this.Show();
            };
            this.Controls.Add(lnkInscription);

            // ── Barre de statut ───────────────────────────────────────────────
            // Affiche l'état de la connexion au serveur + messages d'authentification
            var pnlStatus = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 26,
                BackColor = BleuClair
            };
            // Point vert = indicateur visuel de connexion au serveur
            var dot = new Panel
            {
                Location = new Point(8, 9),
                Size = new Size(8, 8),
                BackColor = Color.FromArgb(42, 138, 42) // vert = connecté
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

        // ══════════════════════════════════════════════════════════════════════
        //  LOGIQUE DE CONNEXION
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Déclenché par le clic sur "Se connecter" ou la touche Entrée.
        /// Vérifie les champs, interroge la base via UserRepository,
        /// puis ouvre FrmDashboard si les credentials sont corrects.
        /// </summary>
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // Réinitialisation des messages précédents
            lblErreur.Visible = false;
            lblStatus.Text = "Authentification en cours...";

            string email = txtEmail.Text.Trim();
            string pwd = txtPassword.Text;

            // Validation des champs vides avant tout appel base de données
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pwd))
            {
                ShowError("Veuillez remplir tous les champs.");
                return;
            }

            try
            {
                // Recherche de l'utilisateur par email (index UNIQUE en base)
                var user = _userRepo.GetByEmail(email);

                // Vérification : utilisateur inexistant OU mot de passe incorrect
                if (user is null || !VerifierMotDePasse(pwd, user.Password))
                {
                    ShowError("Email ou mot de passe incorrect.");
                    return;
                }

                // Connexion réussie → ouverture du tableau de bord
                lblStatus.Text = "Connexion réussie — Chargement...";
                this.Hide();
                new FrmDashboard(user).ShowDialog(); // passe l'objet User complet
                this.Close(); // ferme définitivement FrmLogin après déconnexion
            }
            catch (Exception ex)
            {
                // Capture les erreurs SQL (serveur injoignable, credentials invalides…)
                ShowError($"Erreur de connexion : {ex.Message}");
            }
        }

        /// <summary>
        /// Affiche un message d'erreur rouge sous les champs de saisie.
        /// </summary>
        private void ShowError(string msg)
        {
            lblErreur.Text = "⚠  " + msg;
            lblErreur.Visible = true;
            lblStatus.Text = "Échec de connexion";
        }

        /// <summary>
        /// Compare le mot de passe saisi avec celui stocké en base.
        /// ⚠ En production : remplacer par BCrypt.Verify(saisi, stocke)
        /// pour comparer avec le hash stocké.
        /// </summary>
        private static bool VerifierMotDePasse(string saisi, string stocke) =>
            saisi == stocke;

        // ══════════════════════════════════════════════════════════════════════
        //  HELPERS DE CONSTRUCTION D'INTERFACE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Crée et ajoute un Label de formulaire (libellé de champ).
        /// Factorisé pour éviter la répétition du code de style.
        /// </summary>
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

        /// <summary>
        /// Crée et ajoute un TextBox de saisie standardisé.
        /// Le paramètre placeholder est conservé pour documentation — WinForms
        /// ne supporte pas nativement le placeholder sans sous-classement.
        /// </summary>
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