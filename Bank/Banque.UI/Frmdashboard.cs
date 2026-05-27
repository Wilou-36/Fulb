using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Bank.Banque.Business;
using Bank.Banque.Models;

namespace Bank.Banque.UI
{
    // ══════════════════════════════════════════════════════════════════════════
    //  FrmDashboard — Tableau de bord principal
    //  Affiché après une connexion réussie dans FrmLogin.
    //
    //  Fonctionnalités :
    //    - Sélection d'un compte via ComboBox (un user peut avoir plusieurs comptes)
    //    - Affichage du solde en temps réel
    //    - Accès aux actions : Dépôt, Retrait, Virement, Crypto
    //    - Historique des transactions dans un DataGridView coloré
    //    - Bouton Déconnexion → retour sur FrmLogin
    //
    //  Flux de rafraîchissement :
    //    Après chaque opération (dépôt, retrait…), le solde et l'historique
    //    sont rechargés depuis la base pour garantir la cohérence.
    // ══════════════════════════════════════════════════════════════════════════
    internal class FrmDashboard : Form
    {
        // ── Palette de couleurs (charte graphique Fulbank) ────────────────────
        private static readonly Color BleuFonce = Color.FromArgb(26, 58, 107);
        private static readonly Color BleuClair = Color.FromArgb(208, 220, 236);
        private static readonly Color BlancPage = Color.FromArgb(240, 244, 248);
        private static readonly Color TextePrinc = Color.FromArgb(26, 42, 74);
        private static readonly Color Vert = Color.FromArgb(42, 138, 42); // transactions crédit
        private static readonly Color Rouge = Color.FromArgb(192, 57, 43); // transactions débit

        // ── Données métier ────────────────────────────────────────────────────
        private readonly User _user;           // utilisateur connecté
        private readonly AccountService _accountService = new AccountService();
        private readonly TransactionService _txService = new TransactionService();
        private List<Account> _accounts = new List<Account>(); // comptes du user
        private Account _selectedAccount;                      // compte actif

        // ── Contrôles de l'interface ──────────────────────────────────────────
        private Label lblBienvenue = new Label();
        private ComboBox cbxComptes = new ComboBox();
        private Label lblSolde = new Label();
        private DataGridView dgvTransactions = new DataGridView();
        private Label lblStatus = new Label();

        /// <summary>
        /// Constructeur : reçoit l'utilisateur connecté depuis FrmLogin.
        /// </summary>
        public FrmDashboard(User user)
        {
            _user = user;
            InitializeComponent();
            ChargerDonnees(); // chargement initial des comptes
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CONSTRUCTION DE L'INTERFACE
        // ══════════════════════════════════════════════════════════════════════
        private void InitializeComponent()
        {
            this.Text = "Fulbank — Tableau de bord";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BlancPage;
            this.Font = new Font("Segoe UI", 9f);

            // ── En-tête bleu : message de bienvenue + bouton déconnexion ──────
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = BleuFonce
            };
            // Affiche "Bonjour, Prénom Nom" en haut à gauche
            lblBienvenue = new Label
            {
                Text = $"Bonjour, {_user.Prenom} {_user.Nom}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                AutoSize = false,
                Width = 500,
                Height = 60,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(20, 0)
            };
            // Bouton déconnexion : ferme le dashboard et rouvre FrmLogin
            var btnDeconnexion = new Button
            {
                Text = "Déconnexion",
                Location = new Point(720, 15),
                Size = new Size(110, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(180, 30, 30),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand
            };
            btnDeconnexion.FlatAppearance.BorderSize = 0;
            btnDeconnexion.Click += (s, e) => { this.Close(); new FrmLogin().Show(); };
            pnlHeader.Controls.AddRange(new Control[] { lblBienvenue, btnDeconnexion });
            this.Controls.Add(pnlHeader);

            // ── Sélecteur de compte ───────────────────────────────────────────
            // Permet de choisir parmi les comptes de l'utilisateur connecté
            // L'événement SelectedIndexChanged recharge le solde et l'historique
            int y = 80;
            AddLabel("Compte sélectionné :", new Point(20, y));
            cbxComptes = new ComboBox
            {
                Location = new Point(160, y - 2),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f)
            };
            cbxComptes.SelectedIndexChanged += CbxComptes_Changed;
            this.Controls.Add(cbxComptes);

            // ── Carte solde ───────────────────────────────────────────────────
            // Panel bleu affichant le solde disponible du compte sélectionné
            var pnlSolde = new Panel
            {
                Location = new Point(20, 115),
                Size = new Size(220, 80),
                BackColor = BleuFonce
            };
            var lblSoldeLib = new Label
            {
                Text = "Solde disponible",
                ForeColor = Color.FromArgb(180, 205, 230),
                Font = new Font("Segoe UI", 9f),
                Location = new Point(12, 10),
                AutoSize = true
            };
            // lblSolde est mis à jour par CbxComptes_Changed et après chaque opération
            lblSolde = new Label
            {
                Text = "—",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                Location = new Point(10, 32),
                AutoSize = true
            };
            pnlSolde.Controls.AddRange(new Control[] { lblSoldeLib, lblSolde });
            this.Controls.Add(pnlSolde);

            // ── Boutons d'action (Dépôt, Retrait, Virement, Crypto) ───────────
            // Chaque bouton porte son libellé en Tag pour être identifié dans BtnAction_Click
            string[] actions = { "💳  Dépôt", "💸  Retrait", "↔  Virement", "₿  Crypto" };
            int bx = 260;
            foreach (var lbl in actions)
            {
                var btn = new Button
                {
                    Text = lbl,
                    Location = new Point(bx, 115),
                    Size = new Size(130, 80),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    ForeColor = BleuFonce,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    Tag = lbl  // utilisé dans BtnAction_Click pour identifier l'action
                };
                btn.FlatAppearance.BorderColor = BleuClair;
                btn.Click += BtnAction_Click;
                this.Controls.Add(btn);
                bx += 145; // espacement horizontal entre les boutons
            }

            // ── Historique des transactions ───────────────────────────────────
            // DataGridView en lecture seule, coloré selon le sens de l'opération
            AddLabel("Historique des transactions :", new Point(20, 215));

            dgvTransactions = new DataGridView
            {
                Location = new Point(20, 240),
                Size = new Size(810, 280),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            };
            // Style d'en-tête personnalisé aux couleurs Fulbank
            dgvTransactions.ColumnHeadersDefaultCellStyle.BackColor = BleuFonce;
            dgvTransactions.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTransactions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvTransactions.EnableHeadersVisualStyles = false;
            this.Controls.Add(dgvTransactions);

            // ── Barre de statut en bas ────────────────────────────────────────
            // Affiche le nombre de transactions chargées après sélection d'un compte
            var pnlStatus = new Panel { Dock = DockStyle.Bottom, Height = 26, BackColor = BleuClair };
            lblStatus = new Label
            {
                Text = "Prêt",
                Location = new Point(10, 4),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(58, 74, 106)
            };
            pnlStatus.Controls.Add(lblStatus);
            this.Controls.Add(pnlStatus);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CHARGEMENT DES DONNÉES
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Charge tous les comptes de l'utilisateur connecté et peuple le ComboBox.
        /// Appelé une seule fois à l'ouverture du formulaire.
        /// </summary>
        private void ChargerDonnees()
        {
            // Récupère les comptes via AccountService → AccountRepository
            _accounts = _accountService.GetComptesByUser(_user.Id);
            cbxComptes.Items.Clear();

            // Ajoute chaque numéro de compte dans le sélecteur
            foreach (var acc in _accounts)
                cbxComptes.Items.Add($"{acc.NumAccount}");

            // Sélectionne automatiquement le premier compte disponible
            if (_accounts.Count > 0)
                cbxComptes.SelectedIndex = 0;
        }

        /// <summary>
        /// Déclenché quand l'utilisateur change de compte dans le ComboBox.
        /// Met à jour le solde affiché et recharge l'historique.
        /// </summary>
        private void CbxComptes_Changed(object sender, EventArgs e)
        {
            if (cbxComptes.SelectedIndex < 0) return;

            // Synchronise l'objet compte sélectionné avec l'index du ComboBox
            _selectedAccount = _accounts[cbxComptes.SelectedIndex];
            lblSolde.Text = _selectedAccount.Solde.ToString("C2");

            ChargerTransactions(); // recharge l'historique pour ce compte
        }

        /// <summary>
        /// Charge et affiche l'historique des transactions du compte sélectionné.
        /// Colorie les lignes en vert (crédit) ou rouge (débit) selon le type.
        /// </summary>
        private void ChargerTransactions()
        {
            if (_selectedAccount is null) return;

            // Réinitialisation de la grille avant rechargement
            dgvTransactions.DataSource = null;
            dgvTransactions.Columns.Clear();

            // Chargement des transactions via TransactionService
            var transactions = _txService.GetHistorique(_selectedAccount.Id);
            dgvTransactions.DataSource = transactions;

            lblStatus.Text = $"{transactions.Count} transaction(s) chargée(s)";

            // Coloration ligne par ligne : vert = crédit, rouge = débit
            // IsCredit est une propriété calculée de la classe Transaction
            foreach (DataGridViewRow row in dgvTransactions.Rows)
            {
                if (row.DataBoundItem is Transaction t)
                    row.DefaultCellStyle.ForeColor = t.IsCredit ? Vert : Rouge;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  GESTION DES BOUTONS D'ACTION
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Gestionnaire commun aux 4 boutons d'action (Dépôt, Retrait, Virement, Crypto).
        /// Identifie l'action via la propriété Tag du bouton cliqué
        /// et ouvre le formulaire correspondant en mode dialog.
        /// Après fermeture : rafraîchit le solde et l'historique.
        /// </summary>
        private void BtnAction_Click(object sender, EventArgs e)
        {
            // Vérification préalable : un compte doit être sélectionné
            if (_selectedAccount is null)
            {
                MessageBox.Show("Veuillez sélectionner un compte.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Lecture du Tag pour identifier quel bouton a été cliqué
            string tag = ((Button)sender).Tag?.ToString() ?? "";

            // Ouverture du formulaire approprié en mode ShowDialog (bloquant)
            if (tag.Contains("Dépôt"))
                new FrmRetrait(_selectedAccount, depot: true).ShowDialog();
            else if (tag.Contains("Retrait"))
                new FrmRetrait(_selectedAccount, depot: false).ShowDialog();
            else if (tag.Contains("Virement"))
                new FrmVirement(_selectedAccount).ShowDialog();
            else if (tag.Contains("Crypto"))
                new FrmCrypto(_selectedAccount).ShowDialog();

            // ── Rafraîchissement post-opération ───────────────────────────────
            // Rechargement depuis la base pour afficher le nouveau solde
            _selectedAccount = _accountService.GetCompte(_selectedAccount.Id);
            lblSolde.Text = _selectedAccount.Solde.ToString("C2");
            ChargerTransactions();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  HELPERS DE CONSTRUCTION D'INTERFACE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Crée et ajoute un Label de formulaire (libellé de section).
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
    }
}