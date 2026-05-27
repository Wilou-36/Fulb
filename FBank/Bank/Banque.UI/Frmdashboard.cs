using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Bank.Banque.Business;
using Bank.Banque.Models;

namespace Bank.Banque.UI
{
    internal class FrmDashboard : Form
    {
        // ── Couleurs ──────────────────────────────────────────────────────────
        private static readonly Color BleuFonce = Color.FromArgb(26, 58, 107);
        private static readonly Color BleuClair = Color.FromArgb(208, 220, 236);
        private static readonly Color BlancPage = Color.FromArgb(240, 244, 248);
        private static readonly Color TextePrinc = Color.FromArgb(26, 42, 74);
        private static readonly Color Vert = Color.FromArgb(42, 138, 42);
        private static readonly Color Rouge = Color.FromArgb(192, 57, 43);

        // ── Données ───────────────────────────────────────────────────────────
        private readonly User _user;
        private readonly AccountService _accountService = new AccountService();
        private readonly TransactionService _txService = new TransactionService();
        private List<Account> _accounts = new List<Account>();
        private Account _selectedAccount;

        // ── Contrôles ─────────────────────────────────────────────────────────
        private Label lblBienvenue = new Label();
        private ComboBox cbxComptes = new ComboBox();
        private Label lblSolde = new Label();
        private DataGridView dgvTransactions = new DataGridView();
        private Label lblStatus = new Label();

        public FrmDashboard(User user)
        {
            _user = user;
            InitializeComponent();
            ChargerDonnees();
        }

        private void InitializeComponent()
        {
            this.Text = "Fulbank — Tableau de bord";
            this.Size = new Size(860, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BlancPage;
            this.Font = new Font("Segoe UI", 9f);

            // ── En-tête ───────────────────────────────────────────────────────
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = BleuFonce
            };
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

            // ── Boutons d'action ──────────────────────────────────────────────
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
                    Tag = lbl
                };
                btn.FlatAppearance.BorderColor = BleuClair;
                btn.Click += BtnAction_Click;
                this.Controls.Add(btn);
                bx += 145;
            }

            // ── Historique des transactions ───────────────────────────────────
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
            dgvTransactions.ColumnHeadersDefaultCellStyle.BackColor = BleuFonce;
            dgvTransactions.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTransactions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvTransactions.EnableHeadersVisualStyles = false;
            this.Controls.Add(dgvTransactions);

            // ── Barre de statut ───────────────────────────────────────────────
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

        // ── Chargement des données ────────────────────────────────────────────
        private void ChargerDonnees()
        {
            _accounts = _accountService.GetComptesByUser(_user.Id);
            cbxComptes.Items.Clear();
            foreach (var acc in _accounts)
                cbxComptes.Items.Add($"{acc.NumAccount}");

            if (_accounts.Count > 0)
                cbxComptes.SelectedIndex = 0;
        }

        private void CbxComptes_Changed(object sender, EventArgs e)
        {
            if (cbxComptes.SelectedIndex < 0) return;
            _selectedAccount = _accounts[cbxComptes.SelectedIndex];
            lblSolde.Text = _selectedAccount.Solde.ToString("C2");
            ChargerTransactions();
        }

        private void ChargerTransactions()
        {
            if (_selectedAccount is null) return;
            dgvTransactions.DataSource = null;
            dgvTransactions.Columns.Clear();

            var transactions = _txService.GetHistorique(_selectedAccount.Id);

            dgvTransactions.DataSource = transactions;
            lblStatus.Text = $"{transactions.Count} transaction(s) chargée(s)";

            // Coloration des lignes crédit / débit
            foreach (DataGridViewRow row in dgvTransactions.Rows)
            {
                if (row.DataBoundItem is Transaction t)
                    row.DefaultCellStyle.ForeColor = t.IsCredit ? Vert : Rouge;
            }
        }

        // ── Boutons d'action ──────────────────────────────────────────────────
        private void BtnAction_Click(object sender, EventArgs e)
        {
            if (_selectedAccount is null)
            {
                MessageBox.Show("Veuillez sélectionner un compte.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string tag = ((Button)sender).Tag?.ToString() ?? "";

            if (tag.Contains("Dépôt"))
                new FrmRetrait(_selectedAccount, depot: true).ShowDialog();
            else if (tag.Contains("Retrait"))
                new FrmRetrait(_selectedAccount, depot: false).ShowDialog();
            else if (tag.Contains("Virement"))
                new FrmVirement(_selectedAccount).ShowDialog();
            else if (tag.Contains("Crypto"))
                new FrmCrypto(_selectedAccount).ShowDialog();

            // Rafraîchir après l'opération
            _selectedAccount = _accountService.GetCompte(_selectedAccount.Id);
            lblSolde.Text = _selectedAccount.Solde.ToString("C2");
            ChargerTransactions();
        }

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