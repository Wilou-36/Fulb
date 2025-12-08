using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using FBank.database;

namespace FBank
{
    public partial class newUser : Form
    {
        private bool _isSaved;

        public newUser()
        {
            InitializeComponent();

            // Forcer la police des TextBox à 12pt
            SetTextBoxesFont(this, 12f);

            // Ajouter le logo
            LogoHelper.AddLogoToForm(this);

            _isSaved = false;
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

        private void btnSuivant_Click(object sender, EventArgs e)
        {
            // Tenter d'enregistrer le client avant de passer à l'étape suivante.
            TrySaveIfNeeded();

            // Ouvrir l'étape suivante
            newUser2 newUser2Form = new newUser2();
            newUser2Form.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private void txtNom_TextChanged(object sender, EventArgs e)
        {
            TrySaveIfNeeded();
        }

        private void txtPrenom_TextChanged(object sender, EventArgs e)
        {
            TrySaveIfNeeded();
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            TrySaveIfNeeded();
        }

        private void dtpNaissance_ValueChanged(object sender, EventArgs e)
        {
            TrySaveIfNeeded();
        }

        private void txtProfession_TextChanged(object sender, EventArgs e)
        {
            TrySaveIfNeeded();
        }

        private void txtRevenu_TextChanged(object sender, EventArgs e)
        {
            TrySaveIfNeeded();
        }

        /// <summary>
        /// Vérifie les champs requis et enregistre une seule fois le client dans la base si ce n'est pas encore fait.
        /// </summary>
        private void TrySaveIfNeeded()
        {
            if (_isSaved)
                return;

            // Valider les champs requis : nom et prénom (email facultatif selon votre règle)
            var nom = txtNom?.Text?.Trim();
            var prenom = txtPrenom?.Text?.Trim();
            var email = txtEmail?.Text?.Trim();

            if (string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(prenom))
                return; // pas assez d'infos pour enregistrer

            try
            {
                SaveClient(nom, prenom, email, dtpNaissance?.Value, txtProfession?.Text?.Trim(), ParseDecimalOrNull(txtRevenu?.Text));
                _isSaved = true;

                // Optionnel : indiquer visuellement que le client a été enregistré
                MessageBox.Show("Client enregistré avec succès.", "Enregistrement", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Ne pas spammer l'utilisateur à chaque saisie : afficher une seule erreur si échec critique
                MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal? ParseDecimalOrNull(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (decimal.TryParse(text, out var v))
                return v;

            return null;
        }

        /// <summary>
        /// Enregistre le client dans la table `clients`. Colonnes optionnelles : date_naissance, profession, revenu.
        /// Utilise une requête paramétrée pour éviter les injections SQL.
        /// </summary>
        private void SaveClient(string nom, string prenom, string email, DateTime? dateNaissance, string profession, decimal? revenu)
        {
            // Cette requête suppose que les colonnes optionnelles existent (date_naissance, profession, revenu).
            // Si votre table n'a que (nom, prenom, email), adaptez la requête en conséquence.
            const string sql = @"
INSERT INTO clients (nom, prenom, email, date_naissance, profession, revenu)
VALUES (@nom, @prenom, @email, @date_naissance, @profession, @revenu);
";

            var parameters = new[]
            {
                new MySqlParameter("@nom", nom ?? (object)DBNull.Value),
                new MySqlParameter("@prenom", prenom ?? (object)DBNull.Value),
                new MySqlParameter("@email", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email),
                new MySqlParameter("@date_naissance", dateNaissance.HasValue ? (object)dateNaissance.Value : DBNull.Value),
                new MySqlParameter("@profession", string.IsNullOrWhiteSpace(profession) ? (object)DBNull.Value : profession),
                new MySqlParameter("@revenu", revenu.HasValue ? (object)revenu.Value : DBNull.Value)
            };

            DbManager.ExecuteNonQuery(sql, parameters);
        }

        private void btnAnnulation_Click(object sender, EventArgs e)
        {
            PageAdmin pageAdminForm = new PageAdmin();
            pageAdminForm.Show();
        }
    }
}
