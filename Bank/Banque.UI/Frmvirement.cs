using System;
using System.Drawing;
using System.Windows.Forms;
using Bank.Banque.Business;
using Bank.Banque.Data;
using Bank.Banque.Models;

namespace Bank.Banque.UI
{
    // ══════════════════════════════════════════════════════════════════════════
    //  FrmVirement — Formulaire de virement entre deux comptes bancaires
    //
    //  Fonctionnement :
    //    1. L'utilisateur saisit le numéro du compte destinataire
    //    2. Vérification en temps réel à chaque frappe (TxtDest_Changed)
    //    3. Saisie du montant et du motif
    //    4. Confirmation → TransactionService.EffectuerVirement()
    //       → débite le compte source ET crédite le compte destination
    //       → enregistre deux transactions ([SORTANT] et [ENTRANT])
    //
    //  Reçoit le compte SOURCE depuis FrmDashboard.
    //  Le compte DESTINATION est recherché par numéro via AccountRepository.
    // ══════════════════════════════════════════════════════════════════════════
    internal class FrmVirement : Form
    {
        // ── Palette de couleurs (charte graphique Fulbank) ────────────────────
        private static readonly Color BleuFonce = Color.FromArgb(26, 58, 107);
        private static readonly Color BlancPage = Color.FromArgb(240, 244, 248);
        private static readonly Color TextePrinc = Color.FromArgb(26, 42, 74);

        // ── Données reçues depuis FrmDashboard ────────────────────────────────
        private readonly Account _accountSource; // compte émetteur (débité)

        // ── Services et repositories ──────────────────────────────────────────
        // TransactionService : logique du virement (débit source + crédit dest)
        private readonly TransactionService _txService = new TransactionService();
        // AccountRepository : recherche du compte destination par numéro
        private readonly AccountRepository _accountRepo = new AccountRepository();

        // ── Contrôles du formulaire ───────────────────────────────────────────
        private TextBox txtCompteDestination = new TextBox(); // numéro compte destinataire
        private TextBox txtMontant = new TextBox(); // montant en euros
        private TextBox txtMotif = new TextBox(); // libellé du virement
        private Label lblSolde = new Label();   // solde source (informatif)
        private Label lblErreur = new Label();   // message d'erreur
        private Label lblDestInfo = new Label();   // feedback validation destinataire
        private Button btnValider = new Button();  // bouton de confirmation

        /// <summary>
        /// Constructeur : reçoit le compte source depuis FrmDashboard.
        /// </summary>
        public FrmVirement(Account accountSource)
        {
            _accountSource = accountSource;
            InitializeComponent();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CONSTRUCTION DE L'INTERFACE
        // ══════════════════════════════════════════════════════════════════════
        private void InitializeComponent()
        {
            this.Text = "Fulbank — Virement";
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
                Text = "  ↔  Effectuer un virement",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            });
            this.Controls.Add(pnlHeader);

            int y = 75;

            // ── Informations du compte source (lecture seule) ─────────────────
            // Rappelle à l'utilisateur depuis quel compte le virement est émis
            AddLabel($"Compte source : {_accountSource.NumAccount}", new Point(30, y)); y += 24;

            // Solde affiché en vert pour aider à saisir un montant cohérent
            lblSolde = new Label
            {
                Text = $"Solde disponible : {_accountSource.Solde:C2}",
                Location = new Point(30, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 106, 30)
            };
            this.Controls.Add(lblSolde);
            y += 36;

            // ── Champ compte destinataire + validation en temps réel ──────────
            // L'événement TextChanged déclenche TxtDest_Changed à chaque frappe
            AddLabel("Numéro de compte destinataire :", new Point(30, y)); y += 22;
            txtCompteDestination = CreateInput(new Point(30, y));
            txtCompteDestination.TextChanged += TxtDest_Changed;
            y += 32;

            // Label de feedback affiché sous le champ destinataire
            // ✔ Compte valide (vert) ou ⚠ Compte introuvable (rouge)
            lblDestInfo = new Label
            {
                Location = new Point(30, y),
                AutoSize = false,
                Width = 360,
                Height = 20,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(42, 79, 143)
            };
            this.Controls.Add(lblDestInfo);
            y += 30;

            // ── Champ montant ─────────────────────────────────────────────────
            AddLabel("Montant (€) :", new Point(30, y)); y += 22;
            txtMontant = CreateInput(new Point(30, y)); y += 44;

            // ── Champ motif ───────────────────────────────────────────────────
            // Optionnel — transmis dans le champ Details des deux transactions
            AddLabel("Motif du virement :", new Point(30, y)); y += 22;
            txtMotif = CreateInput(new Point(30, y)); y += 44;

            // ── Zone d'erreur ─────────────────────────────────────────────────
            lblErreur = new Label
            {
                Location = new Point(30, y),
                AutoSize = false,
                Width = 360,
                Height = 22,
                ForeColor = Color.FromArgb(192, 57, 43),
                Visible = false
            };
            this.Controls.Add(lblErreur);
            y += 28;

            // ── Bouton de confirmation ────────────────────────────────────────
            btnValider = new Button
            {
                Text = "Confirmer le virement",
                Location = new Point(30, y),
                Size = new Size(360, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = BleuFonce,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnValider.FlatAppearance.BorderSize = 0;
            btnValider.Click += BtnValider_Click;
            this.AcceptButton = btnValider;
            this.Controls.Add(btnValider);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  LOGIQUE DE VALIDATION DU DESTINATAIRE (TEMPS RÉEL)
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Déclenché à chaque frappe dans le champ destinataire.
        /// Interroge AccountRepository pour vérifier si le numéro existe en base.
        /// Affiche un feedback visuel immédiat (✔ ou ⚠) sans attendre la soumission.
        /// </summary>
        private void TxtDest_Changed(object sender, EventArgs e)
        {
            string num = txtCompteDestination.Text.Trim();

            // Pas de recherche si le champ est vide
            if (string.IsNullOrEmpty(num)) { lblDestInfo.Text = ""; return; }

            // Recherche en base par numéro de compte (UNIQUE NOT NULL)
            var dest = _accountRepo.GetByNumAccount(num);

            // Feedback coloré selon le résultat
            lblDestInfo.Text = dest is null ? "⚠  Compte introuvable" : "✔  Compte valide";
            lblDestInfo.ForeColor = dest is null
                ? Color.FromArgb(192, 57, 43)  // rouge = introuvable
                : Color.FromArgb(26, 106, 30);  // vert = valide
        }

        // ══════════════════════════════════════════════════════════════════════
        //  LOGIQUE DE SOUMISSION DU VIREMENT
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Déclenché par le clic sur "Confirmer le virement".
        /// Valide tous les champs, récupère le compte destination,
        /// puis appelle TransactionService.EffectuerVirement().
        /// </summary>
        private void BtnValider_Click(object sender, EventArgs e)
        {
            lblErreur.Visible = false;

            string numDest = txtCompteDestination.Text.Trim();

            // Validation : champ destinataire non vide
            if (string.IsNullOrEmpty(numDest))
            { ShowError("Saisissez un numéro de compte destinataire."); return; }

            // Validation : montant positif avec support virgule et point
            if (!decimal.TryParse(
                    txtMontant.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal montant) || montant <= 0)
            { ShowError("Saisissez un montant valide et positif."); return; }

            // Vérification finale du compte destination en base
            var dest = _accountRepo.GetByNumAccount(numDest);
            if (dest is null)
            { ShowError("Compte destinataire introuvable."); return; }

            try
            {
                // Appel au service : débite source + crédite destination
                // + crée deux transactions ([SORTANT] / [ENTRANT])
                _txService.EffectuerVirement(
                    _accountSource.Id, dest.Id, montant, txtMotif.Text.Trim());

                MessageBox.Show(
                    $"Virement de {montant:C2} effectué vers {numDest}.",
                    "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Signale à FrmDashboard que le rafraîchissement est nécessaire
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                // Affiche les erreurs métier (solde insuffisant, comptes identiques…)
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
                ForeColor = TextePrinc
            });
        }

        /// <summary>Crée et ajoute un TextBox de saisie standardisé.</summary>
        private TextBox CreateInput(Point loc)
        {
            var tb = new TextBox
            {
                Location = loc,
                Width = 360,
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