using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Bank.Banque.Business;
using Bank.Banque.Models;

namespace Bank.Banque.UI
{
    internal class FrmCrypto : Form
    {
        private static readonly Color BleuFonce = Color.FromArgb(26, 58, 107);
        private static readonly Color BleuClair = Color.FromArgb(208, 220, 236);
        private static readonly Color BlancPage = Color.FromArgb(240, 244, 248);
        private static readonly Color TextePrinc = Color.FromArgb(26, 42, 74);
        private static readonly Color Vert = Color.FromArgb(42, 138, 42);
        private static readonly Color Orange = Color.FromArgb(200, 100, 20);

        private readonly Account _account;
        private readonly CryptoService _cryptoService = new CryptoService();

        // Cours simulés (à brancher sur l'API CoinGecko en production)
        private readonly Dictionary<string, decimal> _cours = new Dictionary<string, decimal>()
        {
            { "BTC",  62000.00m },
            { "ETH",   3100.00m },
            { "SOL",    145.00m },
            { "USDT",     1.00m },
            { "BNB",    580.00m },
        };

        private ComboBox cbxSymbole = new ComboBox();
        private TextBox txtQuantite = new TextBox();
        private Label lblSoldeEuros = new Label();
        private Label lblCours = new Label();
        private Label lblCoutEstime = new Label();
        private Label lblErreur = new Label();
        private DataGridView dgvWallets = new DataGridView();
        private Button btnAcheter = new Button();
        private Button btnVendre = new Button();

        public FrmCrypto(Account account)
        {
            _account = account;
            InitializeComponent();
            ChargerWallets();
        }

        private void InitializeComponent()
        {
            this.Text = "Fulbank — Portefeuille Crypto";
            this.Size = new Size(700, 560);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = BlancPage;
            this.Font = new Font("Segoe UI", 9f);

            // ── En-tête ───────────────────────────────────────────────────────
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = BleuFonce };
            pnlHeader.Controls.Add(new Label
            {
                Text = "  ₿  Portefeuille de cryptomonnaies",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            });
            this.Controls.Add(pnlHeader);

            int y = 70;

            // ── Solde du compte ───────────────────────────────────────────────
            lblSoldeEuros = new Label
            {
                Text = $"Solde disponible : {_account.Solde:C2}",
                Location = new Point(20, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Vert
            };
            this.Controls.Add(lblSoldeEuros);
            y += 36;

            // ── Sélecteur de crypto ───────────────────────────────────────────
            AddLabel("Cryptomonnaie :", new Point(20, y));
            cbxSymbole = new ComboBox
            {
                Location = new Point(140, y - 2),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f)
            };
            foreach (var k in _cours.Keys) cbxSymbole.Items.Add(k);
            cbxSymbole.SelectedIndex = 0;
            cbxSymbole.SelectedIndexChanged += RefreshCours;

            lblCours = new Label
            {
                Location = new Point(270, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = BleuFonce
            };
            this.Controls.Add(cbxSymbole);
            this.Controls.Add(lblCours);
            y += 36;

            // ── Quantité ──────────────────────────────────────────────────────
            AddLabel("Quantité :", new Point(20, y));
            txtQuantite = new TextBox
            {
                Location = new Point(140, y - 2),
                Width = 150,
                Font = new Font("Segoe UI", 10f),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            txtQuantite.TextChanged += RefreshEstimation;
            this.Controls.Add(txtQuantite);

            lblCoutEstime = new Label
            {
                Location = new Point(300, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Orange
            };
            this.Controls.Add(lblCoutEstime);
            y += 44;

            // ── Boutons achat / vente ─────────────────────────────────────────
            btnAcheter = CreateActionBtn("💰  Acheter", Vert, new Point(20, y));
            btnVendre = CreateActionBtn("💵  Vendre", Color.FromArgb(150, 50, 10), new Point(175, y));
            y += 50;

            // ── Erreur ────────────────────────────────────────────────────────
            lblErreur = new Label
            {
                Location = new Point(20, y),
                AutoSize = false,
                Width = 650,
                Height = 22,
                ForeColor = Color.FromArgb(192, 57, 43),
                Visible = false
            };
            this.Controls.Add(lblErreur);
            y += 30;

            // ── Grille des wallets ────────────────────────────────────────────
            AddLabel("Mes portefeuilles :", new Point(20, y)); y += 22;
            dgvWallets = new DataGridView
            {
                Location = new Point(20, y),
                Size = new Size(650, 200),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvWallets.ColumnHeadersDefaultCellStyle.BackColor = BleuFonce;
            dgvWallets.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvWallets.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvWallets.EnableHeadersVisualStyles = false;
            this.Controls.Add(dgvWallets);

            RefreshCours(null, EventArgs.Empty);
        }

        // ── Refresh cours & estimation ────────────────────────────────────────
        private void RefreshCours(object sender, EventArgs e)
        {
            string sym = cbxSymbole.SelectedItem?.ToString() ?? "";
            if (_cours.TryGetValue(sym, out decimal c))
                lblCours.Text = $"Cours : {c:C2} / unité";
            RefreshEstimation(null, EventArgs.Empty);
        }

        private void RefreshEstimation(object sender, EventArgs e)
        {
            string sym = cbxSymbole.SelectedItem?.ToString() ?? "";
            if (!_cours.TryGetValue(sym, out decimal cours)) return;

            if (decimal.TryParse(txtQuantite.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal qte) && qte > 0)
                lblCoutEstime.Text = $"≈ {qte * cours:C2}";
            else
                lblCoutEstime.Text = "";
        }

        // ── Achat ─────────────────────────────────────────────────────────────
        private void BtnAcheter_Click(object sender, EventArgs e)
        {
            if (!ValiderSaisie(out string sym, out decimal qte, out decimal cours)) return;
            try
            {
                _cryptoService.AcheterCrypto(_account.Id, sym, qte, cours);
                MessageBox.Show($"Achat de {qte:F8} {sym} effectué.", "Succès",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChargerWallets();
                lblSoldeEuros.Text = $"Solde disponible : {_cryptoService.GetWallets(_account.Id).Count:F2}";
                // Rafraîchir le solde depuis la base
                var updated = new AccountService().GetCompte(_account.Id);
                lblSoldeEuros.Text = $"Solde disponible : {updated.Solde:C2}";
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        // ── Vente ─────────────────────────────────────────────────────────────
        private void BtnVendre_Click(object sender, EventArgs e)
        {
            if (!ValiderSaisie(out string sym, out decimal qte, out decimal cours)) return;
            try
            {
                _cryptoService.VendreCrypto(_account.Id, sym, qte, cours);
                MessageBox.Show($"Vente de {qte:F8} {sym} effectuée.", "Succès",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChargerWallets();
                var updated = new AccountService().GetCompte(_account.Id);
                lblSoldeEuros.Text = $"Solde disponible : {updated.Solde:C2}";
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        // ── Chargement de la grille des wallets ───────────────────────────────
        private void ChargerWallets()
        {
            lblErreur.Visible = false;
            dgvWallets.DataSource = null;
            dgvWallets.Columns.Clear();

            var wallets = _cryptoService.GetWallets(_account.Id);
            dgvWallets.DataSource = wallets;
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private bool ValiderSaisie(out string sym, out decimal qte, out decimal cours)
        {
            sym = cbxSymbole.SelectedItem?.ToString() ?? "";
            cours = _cours.TryGetValue(sym, out decimal c) ? c : 0;
            qte = 0;
            lblErreur.Visible = false;

            if (string.IsNullOrEmpty(sym))
            { ShowError("Sélectionnez une cryptomonnaie."); return false; }

            if (!decimal.TryParse(txtQuantite.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out qte) || qte <= 0)
            { ShowError("Saisissez une quantité valide et positive."); return false; }

            return true;
        }

        private void ShowError(string msg)
        {
            lblErreur.Text = "⚠  " + msg;
            lblErreur.Visible = true;
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

        private Button CreateActionBtn(string text, Color bg, Point loc)
        {
            var btn = new Button
            {
                Text = text,
                Location = loc,
                Size = new Size(145, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            if (text.Contains("Acheter")) btn.Click += BtnAcheter_Click;
            else btn.Click += BtnVendre_Click;
            this.Controls.Add(btn);
            return btn;
        }
    }
}