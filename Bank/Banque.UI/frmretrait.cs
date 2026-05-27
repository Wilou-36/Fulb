using System;
using System.Drawing;
using System.Windows.Forms;
using Bank.Banque.Business;
using Bank.Banque.Models;

namespace Bank.Banque.UI
{
    // ══════════════════════════════════════════════════════════════════════════
    //  FrmRetrait — Formulaire double usage : Dépôt ET Retrait
    //  Le paramètre booléen <c>depot</c> détermine le mode au lancement.
    //
    //  Mode Dépôt  (depot = true)  → appelle AccountService.Deposer()
    //  Mode Retrait (depot = false) → appelle AccountService.Retirer()
    //
    //  Ouvert depuis FrmDashboard via ShowDialog() — retourne DialogResult.OK
    //  si l'opération s'est déroulée avec succès, ce qui déclenche le
    //  rafraîchissement du solde dans FrmDashboard.
    // ══════════════════════════════════════════════════════════════════════════
    internal class FrmRetrait : Form
    {
        // ── Palette de couleurs (charte graphique Fulbank) ────────────────────
        private static readonly Color BleuFonce = Color.FromArgb(26, 58, 107);
        private static readonly Color BleuClair = Color.FromArgb(208, 220, 236);
        private static readonly Color BlancPage = Color.FromArgb(240, 244, 248);
        private static readonly Color TextePrinc = Color.FromArgb(26, 42, 74);

        // ── Données reçues depuis FrmDashboard ────────────────────────────────
        private readonly Account _account; // compte bancaire concerné
        private readonly bool _depot;   // true = dépôt, false = retrait
        private readonly AccountService _service = new AccountService();

        // ── Contrôles du formulaire ───────────────────────────────────────────
        private TextBox txtMontant = new TextBox(); // saisie du montant en euros
        private TextBox txtDetails = new TextBox(); // libellé / motif (optionnel)
        private Label lblSolde = new Label();   // solde actuel (informatif)
        private Label lblErreur = new Label();   // message d'erreur validation
        private Button btnValider = new Button();  // bouton de confirmation

        /// <summary>
        /// Constructeur : reçoit le compte et le mode (dépôt ou retrait).
        /// </summary>
        public FrmRetrait(Account account, bool depot)
        {
            _account = account;
            _depot = depot;
            InitializeComponent();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CONSTRUCTION DE L'INTERFACE
        // ══════════════════════════════════════════════════════════════════════
        private void InitializeComponent()
        {
            // Le titre et les textes s'adaptent dynamiquement selon le mode
            string titre = _depot ? "Dépôt" : "Retrait";

            this.Text = $"Fulbank — {titre}";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = BlancPage;
            this.Font = new Font("Segoe UI", 9f);

            // ── En-tête bleu avec icône et titre dynamique ────────────────────
            // L'emoji change selon le mode : 💳 pour dépôt, 💸 pour retrait
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = BleuFonce };
            pnlHeader.Controls.Add(new Label
            {
                Text = $"  {(titre == "Dépôt" ? "💳" : "💸")}  {titre} sur compte",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            });
            this.Controls.Add(pnlHeader);

            int y = 75;

            // ── Informations du compte (lecture seule, informatif) ─────────────
            AddLabel($"Compte : {_account.NumAccount}", new Point(30, y)); y += 26;

            // Solde actuel affiché en vert — aide l'utilisateur à saisir un montant cohérent
            lblSolde = new Label
            {
                Text = $"Solde actuel : {_account.Solde:C2}",
                Location = new Point(30, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 106, 30)
            };
            this.Controls.Add(lblSolde);
            y += 36;

            // ── Champ montant ─────────────────────────────────────────────────
            // Accepte virgule ou point comme séparateur décimal
            AddLabel("Montant (€) :", new Point(30, y)); y += 22;
            txtMontant = CreateInput(new Point(30, y)); y += 44;

            // ── Champ motif / détails ─────────────────────────────────────────
            // Optionnel — si vide, un libellé automatique est généré par AccountService
            AddLabel("Motif / détails :", new Point(30, y)); y += 22;
            txtDetails = CreateInput(new Point(30, y)); y += 44;

            // ── Zone d'erreur ─────────────────────────────────────────────────
            // Masquée par défaut — affichée si montant invalide ou solde insuffisant
            lblErreur = new Label
            {
                Location = new Point(30, y),
                AutoSize = false,
                Width = 340,
                Height = 22,
                ForeColor = Color.FromArgb(192, 57, 43),
                Visible = false
            };
            this.Controls.Add(lblErreur);
            y += 28;

            // ── Bouton de confirmation ────────────────────────────────────────
            // Le texte s'adapte : "Confirmer le dépôt" ou "Confirmer le retrait"
            btnValider = new Button
            {
                Text = $"Confirmer le {titre.ToLower()}",
                Location = new Point(30, y),
                Size = new Size(340, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = BleuFonce,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnValider.FlatAppearance.BorderSize = 0;
            btnValider.Click += BtnValider_Click;
            this.AcceptButton = btnValider; // déclenchable par Entrée
            this.Controls.Add(btnValider);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  LOGIQUE DE VALIDATION ET D'OPÉRATION
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Déclenché par le clic sur "Confirmer".
        /// Valide le montant, appelle le service approprié,
        /// puis ferme le formulaire avec DialogResult.OK.
        /// </summary>
        private void BtnValider_Click(object sender, EventArgs e)
        {
            lblErreur.Visible = false;

            // Parsing du montant : supporte virgule (fr-FR) et point (en-US)
            if (!decimal.TryParse(
                    txtMontant.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal montant) || montant <= 0)
            {
                ShowError("Veuillez saisir un montant valide et positif.");
                return;
            }

            try
            {
                // Appel du service selon le mode (dépôt ou retrait)
                // AccountService gère les règles métier (solde suffisant, etc.)
                if (_depot)
                    _service.Deposer(_account.Id, montant, txtDetails.Text.Trim());
                else
                    _service.Retirer(_account.Id, montant, txtDetails.Text.Trim());

                // Message de confirmation affiché à l'utilisateur
                string msg = _depot
                    ? $"Dépôt de {montant:C2} effectué avec succès."
                    : $"Retrait de {montant:C2} effectué avec succès.";

                MessageBox.Show(msg, "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // DialogResult.OK signale à FrmDashboard que l'opération a réussi
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                // Les exceptions métier (solde insuffisant…) sont affichées à l'utilisateur
                ShowError(ex.Message);
            }
        }

        /// <summary>Affiche un message d'erreur rouge sous les champs.</summary>
        private void ShowError(string msg)
        {
            lblErreur.Text = "⚠  " + msg;
            lblErreur.Visible = true;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  HELPERS DE CONSTRUCTION D'INTERFACE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>Crée et ajoute un Label de formulaire stylisé.</summary>
        private void AddLabel(string text, Point loc)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Location = loc,
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 42, 74)
            });
        }

        /// <summary>Crée et ajoute un TextBox de saisie standardisé.</summary>
        private TextBox CreateInput(Point loc)
        {
            var tb = new TextBox
            {
                Location = loc,
                Width = 340,
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