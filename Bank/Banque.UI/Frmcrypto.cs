using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Bank.Banque.Business;
using Bank.Banque.Models;

namespace Bank.Banque.UI
{
    // ══════════════════════════════════════════════════════════════════════════
    //  FrmCrypto — Gestion des portefeuilles de cryptomonnaies
    //
    //  Fonctionnalités :
    //    - Sélection d'une crypto parmi BTC, ETH, SOL, USDT, BNB
    //    - Saisie d'une quantité → estimation du coût en temps réel
    //    - Achat  : débite le compte en euros + crédite le wallet crypto
    //    - Vente  : débite le wallet crypto + crédite le compte en euros
    //    - Grille des wallets : liste tous les portefeuilles du compte
    //
    //  Note production :
    //    Les cours sont actuellement simulés dans un dictionnaire local.
    //    En production, brancher _cours sur l'API CoinGecko :
    //    GET https://api.coingecko.com/api/v3/coins/markets?vs_currency=eur
    // ══════════════════════════════════════════════════════════════════════════
    internal class FrmCrypto : Form
    {
        // ── Palette de couleurs (charte graphique Fulbank) ────────────────────
        private static readonly Color BleuFonce = Color.FromArgb(26, 58, 107);
        private static readonly Color BleuClair = Color.FromArgb(208, 220, 236);
        private static readonly Color BlancPage = Color.FromArgb(240, 244, 248);
        private static readonly Color TextePrinc = Color.FromArgb(26, 42, 74);
        private static readonly Color Vert = Color.FromArgb(42, 138, 42); // bouton acheter
        private static readonly Color Orange = Color.FromArgb(200, 100, 20); // estimation coût

        // ── Données reçues depuis FrmDashboard ────────────────────────────────
        private readonly Account _account;
        private readonly CryptoService _cryptoService = new CryptoService();

        // ── Cours simulés ─────────────────────────────────────────────────────
        // Dictionnaire symbole → cours en euros
        // ⚠ TODO production : remplacer par un appel API Crypto
        private Dictionary<string, decimal> _cours = CryptoApiService.GetCours();

        // ── Contrôles du formulaire ───────────────────────────────────────────
        private ComboBox cbxSymbole = new ComboBox();     // sélecteur de crypto
        private TextBox txtQuantite = new TextBox();      // quantité à acheter/vendre
        private Label lblSoldeEuros = new Label();        // solde disponible en euros
        private Label lblCours = new Label();        // cours unitaire affiché
        private Label lblCoutEstime = new Label();        // estimation coût total
        private Label lblErreur = new Label();        // message d'erreur
        private DataGridView dgvWallets = new DataGridView(); // grille des wallets
        private Button btnAcheter = new Button();       // bouton achat
        private Button btnVendre = new Button();       // bouton vente

        /// <summary>
        /// Constructeur : reçoit le compte bancaire depuis FrmDashboard.
        /// </summary>
        public FrmCrypto(Account account)
        {
            _account = account;
            InitializeComponent();
            ChargerWallets(); // chargement initial des portefeuilles
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CONSTRUCTION DE L'INTERFACE
        // ══════════════════════════════════════════════════════════════════════
        private void InitializeComponent()
        {
            this.Text = "Fulbank — Portefeuille Crypto";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = BlancPage;
            this.Font = new Font("Segoe UI", 9f);

            // ── En-tête bleu ──────────────────────────────────────────────────
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

            // ── Solde disponible en euros ─────────────────────────────────────
            // Mis à jour après chaque achat/vente pour refléter le nouveau solde
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

            // ── Sélecteur de cryptomonnaie ────────────────────────────────────
            // DropDownList = sélection uniquement parmi les valeurs prédéfinies
            // L'événement SelectedIndexChanged déclenche RefreshCours()
            AddLabel("Cryptomonnaie :", new Point(20, y));
            cbxSymbole = new ComboBox
            {
                Location = new Point(140, y - 2),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f)
            };
            // Peuple le ComboBox depuis les clés du dictionnaire _cours
            foreach (var k in _cours.Keys) cbxSymbole.Items.Add(k);
            cbxSymbole.SelectedIndex = 0;
            cbxSymbole.SelectedIndexChanged += RefreshCours;

            // Label affichant le cours unitaire de la crypto sélectionnée
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

            // ── Champ quantité + estimation coût ─────────────────────────────
            // L'événement TextChanged recalcule l'estimation en temps réel
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

            // Affiche le coût total estimé : quantité × cours
            lblCoutEstime = new Label
            {
                Location = new Point(300, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Orange
            };
            this.Controls.Add(lblCoutEstime);
            y += 44;

            // ── Boutons Acheter / Vendre ──────────────────────────────────────
            // CreateActionBtn détecte "Acheter" ou "Vendre" dans le texte pour
            // attacher le bon gestionnaire d'événement
            btnAcheter = CreateActionBtn("💰  Acheter", Vert, new Point(20, y));
            btnVendre = CreateActionBtn("💵  Vendre", Color.FromArgb(150, 50, 10), new Point(175, y));
            y += 50;

            // ── Zone d'erreur ─────────────────────────────────────────────────
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
            // Affiche tous les portefeuilles du compte (un par devise)
            // Rechargée après chaque achat ou vente
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

            // Initialisation du label cours au chargement
            RefreshCours(null, EventArgs.Empty);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  MISE À JOUR DU COURS ET DE L'ESTIMATION
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Déclenché quand l'utilisateur change de crypto dans le ComboBox.
        /// Met à jour le label de cours et recalcule l'estimation.
        /// </summary>
        private void RefreshCours(object sender, EventArgs e)
        {
            string sym = cbxSymbole.SelectedItem?.ToString() ?? "";
            if (_cours.TryGetValue(sym, out decimal c))
                lblCours.Text = $"Cours : {c:C2} / unité";

            RefreshEstimation(null, EventArgs.Empty); // recalcul immédiat
        }

        /// <summary>
        /// Déclenché à chaque modification du champ quantité.
        /// Calcule et affiche le coût total estimé (quantité × cours).
        /// </summary>
        private void RefreshEstimation(object sender, EventArgs e)
        {
            string sym = cbxSymbole.SelectedItem?.ToString() ?? "";
            if (!_cours.TryGetValue(sym, out decimal cours)) return;

            // Parsing robuste : accepte virgule et point comme séparateur
            if (decimal.TryParse(
                    txtQuantite.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal qte) && qte > 0)
                lblCoutEstime.Text = $"≈ {qte * cours:C2}"; // affichage du coût total
            else
                lblCoutEstime.Text = ""; // vide si saisie invalide
        }

        // ══════════════════════════════════════════════════════════════════════
        //  ACHAT DE CRYPTOMONNAIE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Déclenché par le bouton "Acheter".
        /// Valide la saisie, appelle CryptoService.AcheterCrypto(),
        /// puis rafraîchit le solde et la grille des wallets.
        /// </summary>
        private void BtnAcheter_Click(object sender, EventArgs e)
        {
            // Validation commune à l'achat et à la vente
            if (!ValiderSaisie(out string sym, out decimal qte, out decimal cours)) return;
            try
            {
                // Achat : débite le compte en euros + crée/incrémente le wallet
                _cryptoService.AcheterCrypto(_account.Id, sym, qte, cours);

                MessageBox.Show($"Achat de {qte:F8} {sym} effectué.", "Succès",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Rechargement de la grille des wallets après l'achat
                ChargerWallets();

                // Rechargement du solde depuis la base (le solde a été débité)
                var updated = new AccountService().GetCompte(_account.Id);
                lblSoldeEuros.Text = $"Solde disponible : {updated.Solde:C2}";
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  VENTE DE CRYPTOMONNAIE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Déclenché par le bouton "Vendre".
        /// Valide la saisie, appelle CryptoService.VendreCrypto(),
        /// puis rafraîchit le solde et la grille des wallets.
        /// </summary>
        private void BtnVendre_Click(object sender, EventArgs e)
        {
            if (!ValiderSaisie(out string sym, out decimal qte, out decimal cours)) return;
            try
            {
                // Vente : débite le wallet crypto + crédite le compte en euros
                _cryptoService.VendreCrypto(_account.Id, sym, qte, cours);

                MessageBox.Show($"Vente de {qte:F8} {sym} effectuée.", "Succès",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                ChargerWallets();

                // Rechargement du solde depuis la base (le solde a été crédité)
                var updated = new AccountService().GetCompte(_account.Id);
                lblSoldeEuros.Text = $"Solde disponible : {updated.Solde:C2}";
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CHARGEMENT DE LA GRILLE DES WALLETS
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Recharge et affiche tous les wallets du compte dans le DataGridView.
        /// Appelée à l'ouverture du formulaire et après chaque achat/vente.
        /// </summary>
        private void ChargerWallets()
        {
            lblErreur.Visible = false;
            dgvWallets.DataSource = null;
            dgvWallets.Columns.Clear();

            // Chargement via CryptoService → CryptoRepository
            // SELECT * FROM crypto_wallets WHERE account_id = @accountId
            var wallets = _cryptoService.GetWallets(_account.Id);
            dgvWallets.DataSource = wallets;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  HELPERS
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Valide les champs communs aux opérations d'achat et de vente.
        /// Retourne false et affiche une erreur si la saisie est invalide.
        /// </summary>
        /// <param name="sym">Symbole de la crypto sélectionnée (ex : BTC).</param>
        /// <param name="qte">Quantité parsée depuis txtQuantite.</param>
        /// <param name="cours">Cours correspondant au symbole sélectionné.</param>
        private bool ValiderSaisie(out string sym, out decimal qte, out decimal cours)
        {
            sym = cbxSymbole.SelectedItem?.ToString() ?? "";
            cours = _cours.TryGetValue(sym, out decimal c) ? c : 0;
            qte = 0;
            lblErreur.Visible = false;

            // Vérification du symbole sélectionné
            if (string.IsNullOrEmpty(sym))
            { ShowError("Sélectionnez une cryptomonnaie."); return false; }

            // Vérification de la quantité : doit être un décimal positif
            if (!decimal.TryParse(
                    txtQuantite.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out qte) || qte <= 0)
            { ShowError("Saisissez une quantité valide et positive."); return false; }

            return true;
        }

        /// <summary>Affiche un message d'erreur rouge.</summary>
        private void ShowError(string msg)
        {
            lblErreur.Text = "⚠  " + msg;
            lblErreur.Visible = true;
        }

        /// <summary>Crée et ajoute un Label de formulaire stylisé.</summary>
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
        /// Crée un bouton d'action (Acheter ou Vendre) et attache
        /// automatiquement le gestionnaire d'événement selon son texte.
        /// </summary>
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
            // Attache BtnAcheter_Click ou BtnVendre_Click selon le texte du bouton
            if (text.Contains("Acheter")) btn.Click += BtnAcheter_Click;
            else btn.Click += BtnVendre_Click;
            this.Controls.Add(btn);
            return btn;
        }
    }
}