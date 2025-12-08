using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using FBank.database;

namespace FBank
{
    public partial class newUser2 : Form
    {
        public newUser2()
        {
            InitializeComponent();

            SetTextBoxesFont(this, 12f);

            // Ajouter le logo
            LogoHelper.AddLogoToForm(this);

            // Configurer les TextBox de mot de passe : masquage et longueur max 4
            if (txtMDP != null)
            {
                txtMDP.PasswordChar = '*';
                txtMDP.UseSystemPasswordChar = false;
                txtMDP.MaxLength = 4;
            }

            if (txtTrueMDP != null)
            {
                txtTrueMDP.PasswordChar = '*';
                txtTrueMDP.UseSystemPasswordChar = false;
                txtTrueMDP.MaxLength = 4;
            }
        }

        private void SetTextBoxesFont(Control parent, float size)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox tb)
                {
                    tb.Font = new Font(tb.Font.FontFamily, size, tb.Font.Style);
                }
                else
                {
                    // Recurse for container controls (Panel, GroupBox, etc.)
                    if (c.HasChildren)
                        SetTextBoxesFont(c, size);
                }
            }
        }

        private void btnPrecedent_Click(object sender, EventArgs e)
        {
            newUser newUserForm = new newUser();
            newUserForm.Show();
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtMDP_TextChanged(object sender, EventArgs e)
        {
            // Optionnel : n'autoriser que les chiffres
            var tb = sender as TextBox;
            if (tb == null) return;
            tb.Text = FilterDigits(tb.Text);
            tb.SelectionStart = tb.Text.Length;
        }

        private void txtTrueMDP_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;
            tb.Text = FilterDigits(tb.Text);
            tb.SelectionStart = tb.Text.Length;
        }

        private string FilterDigits(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var sb = new System.Text.StringBuilder();
            foreach (var ch in input)
            {
                if (char.IsDigit(ch)) sb.Append(ch);
            }
            return sb.ToString();
        }

        private void btnFinaliser_Click(object sender, EventArgs e)
        {
            
            var pwd = txtMDP?.Text ?? string.Empty;
            var pwdConfirm = txtTrueMDP?.Text ?? string.Empty;

            if (pwd.Length != 4 || pwdConfirm.Length != 4)
            {
                MessageBox.Show("Le mot de passe doit contenir exactement 4 caractères.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!pwd.Equals(pwdConfirm, StringComparison.Ordinal))
            {
                MessageBox.Show("Les mots de passe ne correspondent pas.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Récupérer infos clients depuis le formulaire précédent (contrôles existants dans this form)
                // On suppose que les TextBox/contrôles contenant nom/prenom/email sont accessibles sur ce formulaire
                // Si ces données sont uniquement dans newUser, il faudrait les passer ici (ex: via constructeur).
                string nom = null, prenom = null, email = null;
                var ctrlNom = this.FindForm()?.Controls.Find("txtNom", true);
                var ctrlPrenom = this.FindForm()?.Controls.Find("txtPrenom", true);
                var ctrlEmail = this.FindForm()?.Controls.Find("txtEmail", true);
                if (ctrlNom != null && ctrlNom.Length > 0 && ctrlNom[0] is TextBox tNom) nom = tNom.Text.Trim();
                if (ctrlPrenom != null && ctrlPrenom.Length > 0 && ctrlPrenom[0] is TextBox tPrenom) prenom = tPrenom.Text.Trim();
                if (ctrlEmail != null && ctrlEmail.Length > 0 && ctrlEmail[0] is TextBox tEmail) email = tEmail.Text.Trim();

                // Si les champs ne sont pas accessibles depuis ce formulaire, essayer de récupérer via propriétaire
                if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(prenom))
                {
                    if (this.Owner is newUser parentForm)
                    {
                        // supposez que newUser expose des propriétés publiques Nom/Prenom/Email si nécessaire
                        // sinon il est recommandé de transmettre ces valeurs via le constructeur
                    }
                }

                var clientId = GetClientId(nom, prenom, email);
                if (clientId == null)
                {
                    MessageBox.Show("Impossible de trouver le client en base. Assurez-vous d'avoir rempli les informations précédentes et d'avoir sauvegardé.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Mettre à jour le mot de passe (colonne : mot_de_passe_hash)
                const string sqlUpdate = "UPDATE clients SET mot_de_passe_hash = @pwd WHERE id_client = @id";
                var rows = DbManager.ExecuteNonQuery(sqlUpdate,
                    new MySqlParameter("@pwd", pwd),
                    new MySqlParameter("@id", clientId.Value));

                if (rows > 0)
                {
                    MessageBox.Show("Mot de passe enregistré avec succès.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Échec de l'enregistrement du mot de passe.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// Recherche l'ID du client en base en utilisant l'email si fourni, sinon nom+prenom (dernier créé).
        /// </summary>
        private int? GetClientId(string nom, string prenom, string email)
        {
            DataTable dt = null;

            if (!string.IsNullOrWhiteSpace(email))
            {
                const string sqlByEmail = "SELECT id_client FROM clients WHERE email = @email LIMIT 1";
                dt = DbManager.ExecuteQuery(sqlByEmail, new MySqlParameter("@email", email));
            }
            else if (!string.IsNullOrWhiteSpace(nom) && !string.IsNullOrWhiteSpace(prenom))
            {
                const string sqlByName = "SELECT id_client FROM clients WHERE nom = @nom AND prenom = @prenom ORDER BY date_creation DESC LIMIT 1";
                dt = DbManager.ExecuteQuery(sqlByName,
                    new MySqlParameter("@nom", nom),
                    new MySqlParameter("@prenom", prenom));
            }
            else
            {
                return null;
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0]["id_client"]);
            }

            return null;
        }

        private void btnAnnulation_Click(object sender, EventArgs e)
        {
            PageAdmin pageAdminForm = new PageAdmin();
            pageAdminForm.Show();
        }
    }
}
