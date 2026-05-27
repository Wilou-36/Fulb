using System;
using System.Drawing;
using System.Windows.Forms;
using Bank.Banque.Business;
using Bank.Banque.Data;
using Bank.Banque.Models;

namespace Bank.Banque.UI
{
    internal class FrmVirement : Form
    {
        private static readonly Color BleuFonce = Color.FromArgb(26, 58, 107);
        private static readonly Color BlancPage = Color.FromArgb(240, 244, 248);
        private static readonly Color TextePrinc = Color.FromArgb(26, 42, 74);

        private readonly Account _accountSource;
        private readonly TransactionService _txService = new TransactionService();
        private readonly AccountRepository _accountRepo = new AccountRepository();

        private TextBox txtCompteDestination = new TextBox();
        private TextBox txtMontant = new TextBox();
        private TextBox txtMotif = new TextBox();
        private Label lblSolde = new Label();
        private Label lblErreur = new Label();
        private Label lblDestInfo = new Label();
        private Button btnValider = new Button();

        public FrmVirement(Account accountSource)
        {
            _accountSource = accountSource;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Fulbank — Virement";
            this.Size = new Size(420, 440);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = BlancPage;
            this.Font = new Font("Segoe UI", 9f);

            // ── En-tête ───────────────────────────────────────────────────────
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
            AddLabel($"Compte source : {_accountSource.NumAccount}", new Point(30, y)); y += 24;

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

            // ── Compte destinataire ───────────────────────────────────────────
            AddLabel("Numéro de compte destinataire :", new Point(30, y)); y += 22;
            txtCompteDestination = CreateInput(new Point(30, y));
            txtCompteDestination.TextChanged += TxtDest_Changed;
            y += 32;

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

            // ── Montant ───────────────────────────────────────────────────────
            AddLabel("Montant (€) :", new Point(30, y)); y += 22;
            txtMontant = CreateInput(new Point(30, y)); y += 44;

            // ── Motif ─────────────────────────────────────────────────────────
            AddLabel("Motif du virement :", new Point(30, y)); y += 22;
            txtMotif = CreateInput(new Point(30, y)); y += 44;

            // ── Erreur ────────────────────────────────────────────────────────
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

            // ── Bouton ────────────────────────────────────────────────────────
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

        // ── Vérification du compte destinataire à la frappe ───────────────────
        private void TxtDest_Changed(object sender, EventArgs e)
        {
            string num = txtCompteDestination.Text.Trim();
            if (string.IsNullOrEmpty(num)) { lblDestInfo.Text = ""; return; }

            var dest = _accountRepo.GetByNumAccount(num);
            lblDestInfo.Text = dest is null
                ? "⚠  Compte introuvable"
                : $"✔  Compte valide";
            lblDestInfo.ForeColor = dest is null
                ? Color.FromArgb(192, 57, 43)
                : Color.FromArgb(26, 106, 30);
        }

        private void BtnValider_Click(object sender, EventArgs e)
        {
            lblErreur.Visible = false;

            string numDest = txtCompteDestination.Text.Trim();

            if (string.IsNullOrEmpty(numDest))
            { ShowError("Saisissez un numéro de compte destinataire."); return; }

            if (!decimal.TryParse(txtMontant.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal montant) || montant <= 0)
            { ShowError("Saisissez un montant valide et positif."); return; }

            var dest = _accountRepo.GetByNumAccount(numDest);
            if (dest is null)
            { ShowError("Compte destinataire introuvable."); return; }

            try
            {
                _txService.EffectuerVirement(_accountSource.Id, dest.Id, montant, txtMotif.Text.Trim());
                MessageBox.Show($"Virement de {montant:C2} effectué vers {numDest}.",
                    "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
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