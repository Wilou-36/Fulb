using System;
using System.Drawing;
using System.Windows.Forms;
using Bank.Banque.Business;
using Bank.Banque.Models;

namespace Bank.Banque.UI
{
    /// <summary>
    /// Formulaire utilisé pour les dépôts ET les retraits.
    /// Le paramètre <c>depot</c> détermine le mode.
    /// </summary>
    internal class FrmRetrait : Form
    {
        private static readonly Color BleuFonce = Color.FromArgb(26, 58, 107);
        private static readonly Color BleuClair = Color.FromArgb(208, 220, 236);
        private static readonly Color BlancPage = Color.FromArgb(240, 244, 248);
        private static readonly Color TextePrinc = Color.FromArgb(26, 42, 74);

        private readonly Account _account;
        private readonly bool _depot;
        private readonly AccountService _service = new AccountService();

        private TextBox txtMontant = new TextBox();
        private TextBox txtDetails = new TextBox();
        private Label lblSolde = new Label();
        private Label lblErreur = new Label();
        private Button btnValider = new Button();

        public FrmRetrait(Account account, bool depot)
        {
            _account = account;
            _depot = depot;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            string titre = _depot ? "Dépôt" : "Retrait";
            this.Text = $"Fulbank — {titre}";
            this.Size = new Size(400, 360);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = BlancPage;
            this.Font = new Font("Segoe UI", 9f);

            // ── En-tête ───────────────────────────────────────────────────────
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
            AddLabel($"Compte : {_account.NumAccount}", new Point(30, y)); y += 26;

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

            AddLabel("Montant (€) :", new Point(30, y)); y += 22;
            txtMontant = CreateInput(new Point(30, y)); y += 44;

            AddLabel("Motif / détails :", new Point(30, y)); y += 22;
            txtDetails = CreateInput(new Point(30, y)); y += 44;

            // ── Erreur ────────────────────────────────────────────────────────
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

            // ── Boutons ───────────────────────────────────────────────────────
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
            this.AcceptButton = btnValider;
            this.Controls.Add(btnValider);
        }

        private void BtnValider_Click(object sender, EventArgs e)
        {
            lblErreur.Visible = false;

            if (!decimal.TryParse(txtMontant.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal montant) || montant <= 0)
            {
                ShowError("Veuillez saisir un montant valide et positif.");
                return;
            }

            try
            {
                if (_depot)
                    _service.Deposer(_account.Id, montant, txtDetails.Text.Trim());
                else
                    _service.Retirer(_account.Id, montant, txtDetails.Text.Trim());

                string msg = _depot
                    ? $"Dépôt de {montant:C2} effectué avec succès."
                    : $"Retrait de {montant:C2} effectué avec succès.";

                MessageBox.Show(msg, "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                ForeColor = Color.FromArgb(26, 42, 74)
            });
        }

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