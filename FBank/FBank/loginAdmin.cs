using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using FBank.database;

namespace FBank
{
    public partial class loginAdmin : Form
    {
        public loginAdmin()
        {
            InitializeComponent();

            // Forcer la police des TextBox à 12pt
            SetTextBoxesFont(this, 12f);

            // S'assurer que le TextBox de mot de passe affiche des '*' pour chaque caractère.
            txtPass.PasswordChar = '*';
            txtPass.UseSystemPasswordChar = false;

            // Ajouter le logo
            LogoHelper.AddLogoToForm(this);
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var username = textBox1.Text.Trim();
            var password = txtPass.Text; // Remarque : stockage actuel non sécurisé dans la DB

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Veuillez saisir le nom d'utilisateur et le mot de passe.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (AuthenticateAdmin(username, password))
                {
                    // Connexion réussie — ouvrir PageAdmin et masquer la fenêtre de connexion.
                    var pageAdmin = new PageAdmin();

                    // Quand PageAdmin est fermé, réafficher la fenêtre de connexion (optionnel).
                    pageAdmin.FormClosed += (s, args) => this.Show();

                    pageAdmin.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect.", "Échec", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la connexion : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool AuthenticateAdmin(string username, string password)
        {
            // Requête paramétrée pour éviter les injections SQL
            const string sql = "SELECT COUNT(*) FROM admin WHERE nom = @nom AND mot_de_passe_hash = @pwd";

            var result = DbManager.ExecuteScalar(sql,
                new MySqlParameter("@nom", username),
                new MySqlParameter("@pwd", password));

            // Convertir le résultat en int (peut être long selon le provider)
            var count = Convert.ToInt32(result ?? 0);
            return count > 0;
        }

        private void btnConnexion_Click(object sender, EventArgs e)
        {
            // Déléguer au même flux que button1_Click pour conserver le comportement identique
            button1_Click(sender, e);
        }

        private void txtPass_TextChanged(object sender, EventArgs e)
        {
            // Garantir que le TextBox reste masqué, utile si le designer ou d'autres parties du code modifient la propriété.
            var tb = sender as TextBox;
            if (tb != null && tb.PasswordChar != '*')
            {
                tb.PasswordChar = '*';
                tb.UseSystemPasswordChar = false;
            }
        }
    }
}
