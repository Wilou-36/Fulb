using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FBank
{
    public partial class CGUAdmin : Form
    {
        public CGUAdmin()
        {
            InitializeComponent();
            ConfigurerInterface();
        }

        private void CGUAdmin_Load(object sender, EventArgs e)
        {

        }

        private void ConfigurerInterface()
        {
            // Configuration du formulaire
            this.Text = "Charte d'Utilisation - Employés FULBANK";
            this.Size = new Size(950, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Panel principal
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 20, 30, 80)
            };

            // Titre principal
            Label titreLabel = new Label
            {
                Text = "CHARTE D'UTILISATION DES SYSTÈMES D'INFORMATION",
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = false,
                Width = 880,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(30, 20)
            };

            Label sousTitreLabel = new Label
            {
                Text = "FULBANK - Espace Employés",
                Font = new Font("Segoe UI", 14f, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(30, 80)
            };

            Label dateLabel = new Label
            {
                Text = "Date de mise à jour : 7 décembre 2025",
                Font = new Font("Segoe UI", 10f, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true,
                Location = new Point(30, 110)
            };

            // Panel d'avertissement
            Panel warningPanel = new Panel
            {
                Location = new Point(30, 140),
                Size = new Size(880, 60),
                BackColor = Color.FromArgb(255, 243, 224),
                Padding = new Padding(15)
            };

            Label warningLabel = new Label
            {
                Text = "⚠️ IMPORTANT : Cette charte définit les règles d'utilisation des systèmes d'information.\n" +
                       "Le non-respect peut entraîner des sanctions disciplinaires pouvant aller jusqu'au licenciement.",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 81, 0),
                AutoSize = false,
                Size = new Size(850, 45),
                Location = new Point(15, 8)
            };
            warningPanel.Controls.Add(warningLabel);

            // RichTextBox pour le contenu avec scroll
            RichTextBox contentBox = new RichTextBox
            {
                Location = new Point(30, 210),
                Size = new Size(880, 400),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            // Ajouter le contenu de la charte
            contentBox.Text = ObtenirTexteCharte();

            // Panel pour les boutons
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(30, 15, 30, 15)
            };

            // CheckBox d'acceptation
            CheckBox acceptCheckBox = new CheckBox
            {
                Text = "J'ai lu et j'accepte la Charte d'Utilisation des Systèmes d'Information",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = false,
                Size = new Size(600, 40),
                Location = new Point(30, 20)
            };

            // Bouton Accepter et Signer
            Button btnAccepter = new Button
            {
                Text = "Accepter et Signer",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(63, 81, 181),
                Size = new Size(200, 45),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false,
                Location = new Point(700, 15)
            };
            btnAccepter.FlatAppearance.BorderSize = 0;

            // Bouton Refuser
            Button btnRefuser = new Button
            {
                Text = "Refuser",
                Font = new Font("Segoe UI", 11f, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.FromArgb(240, 240, 240),
                Size = new Size(120, 45),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Location = new Point(560, 15)
            };
            btnRefuser.FlatAppearance.BorderSize = 0;

            // Événement CheckBox
            acceptCheckBox.CheckedChanged += (s, e) =>
            {
                btnAccepter.Enabled = acceptCheckBox.Checked;
                btnAccepter.BackColor = acceptCheckBox.Checked
                    ? Color.FromArgb(63, 81, 181)
                    : Color.FromArgb(200, 200, 200);
            };

            // Événement bouton Accepter
            btnAccepter.Click += (s, e) =>
            {
                // Enregistrer la signature avec date/heure
                DialogResult confirm = MessageBox.Show(
                    "En cliquant sur OK, vous certifiez avoir lu et accepté l'intégralité de la Charte.\n\n" +
                    "Votre acceptation sera enregistrée avec la date et l'heure actuelles.\n\n" +
                    "Continuer ?",
                    "Confirmation de signature",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);

                if (confirm == DialogResult.OK)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            };

            // Événement bouton Refuser
            btnRefuser.Click += (s, e) =>
            {
                DialogResult confirm = MessageBox.Show(
                    "Le refus de la Charte d'Utilisation empêchera l'accès aux systèmes d'information.\n\n" +
                    "Êtes-vous sûr de vouloir refuser ?",
                    "Confirmation de refus",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            };

            // Effet hover sur les boutons
            btnAccepter.MouseEnter += (s, e) =>
            {
                if (btnAccepter.Enabled)
                    btnAccepter.BackColor = Color.FromArgb(48, 63, 159);
            };
            btnAccepter.MouseLeave += (s, e) =>
            {
                if (btnAccepter.Enabled)
                    btnAccepter.BackColor = Color.FromArgb(63, 81, 181);
            };

            btnRefuser.MouseEnter += (s, e) =>
            {
                btnRefuser.BackColor = Color.FromArgb(220, 220, 220);
            };
            btnRefuser.MouseLeave += (s, e) =>
            {
                btnRefuser.BackColor = Color.FromArgb(240, 240, 240);
            };

            // Ajout des contrôles
            buttonPanel.Controls.Add(acceptCheckBox);
            buttonPanel.Controls.Add(btnRefuser);
            buttonPanel.Controls.Add(btnAccepter);

            mainPanel.Controls.Add(titreLabel);
            mainPanel.Controls.Add(sousTitreLabel);
            mainPanel.Controls.Add(dateLabel);
            mainPanel.Controls.Add(warningPanel);
            mainPanel.Controls.Add(contentBox);

            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
        }

        private string ObtenirTexteCharte()
        {
            return @"
                1. OBJET ET CHAMP D'APPLICATION

                La présente Charte d'Utilisation des Systèmes d'Information définit les règles applicables à l'utilisation des ressources informatiques de FULBANK S.A. par ses employés.

                Elle s'applique à tous les collaborateurs, quelle que soit leur fonction ou la nature de leur contrat.

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                2. DÉFINITIONS

                • Employé : Tout collaborateur de FULBANK disposant d'un accès aux systèmes
                • SI : Systèmes d'Information (applications, bases de données, réseaux, équipements)
                • Identifiants : Login et mot de passe personnel
                • Données sensibles : Informations clients, données financières et cryptographiques
                • Accès administrateur : Droits étendus de gestion

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                3. ACCÈS AUX SYSTÈMES

                3.1 Attribution des accès
                Les accès sont attribués par le service informatique selon le principe du moindre privilège (accès minimal nécessaire).

                3.2 Identifiants personnels
                • Personnels et nominatifs
                • Strictement confidentiels
                • Ne jamais les partager
                • Toute action engage la responsabilité de l'employé

                3.3 Gestion des mots de passe
                • Minimum 12 caractères (majuscules, minuscules, chiffres, spéciaux)
                • Changement tous les 90 jours
                • Ne jamais noter le mot de passe
                • Utiliser l'authentification 2FA

                3.4 Verrouillage obligatoire
                Verrouiller sa session (Ctrl+Alt+Suppr) lors de toute absence du poste.

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                4. UTILISATION DES SYSTÈMES

                4.1 Usage professionnel
                Les ressources sont destinées à un usage strictement professionnel. Usage personnel modéré toléré durant les pauses.

                4.2 INTERDICTIONS FORMELLES
                • Accéder à des données sans autorisation
                • Modifier/supprimer des données sans justification
                • Contourner les dispositifs de sécurité
                • Installer des logiciels non autorisés
                • Consulter des contenus illicites
                • Téléchargements illégaux
                • Consulter des comptes clients sans motif légitime

                4.3 Navigation internet
                Interdiction stricte de consulter :
                • Sites pornographiques
                • Sites violents ou discriminatoires
                • Sites contraires aux valeurs de l'entreprise

                4.4 Messagerie professionnelle
                • Privilégier les échanges professionnels
                • Ne pas transmettre d'informations sensibles sans chiffrement
                • Signaler immédiatement tout email suspect (phishing)

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                5. CONFIDENTIALITÉ ET SECRET BANCAIRE

                5.1 Secret professionnel
                Obligation absolue de secret professionnel sur toutes les informations clients.

                5.2 Protection des données clients
                • Consulter uniquement les données nécessaires
                • Ne jamais divulguer d'informations à des tiers
                • Respecter le RGPD
                • Signaler toute violation de données

                5.3 Données cryptographiques
                Les clés privées, seeds et wallets sont soumis à des mesures de sécurité renforcées. Accès strictement réservé aux personnes habilitées.

                5.4 INTERDICTION ABSOLUE
                • Utiliser les informations clients à des fins personnelles
                • Effectuer des opérations pour son propre compte
                • Divulguer des informations confidentielles (même après le départ)

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                6. SÉCURITÉ INFORMATIQUE

                6.1 Vigilance quotidienne
                • Maintenir les logiciels à jour
                • Ne pas cliquer sur des liens suspects
                • Signaler tout incident immédiatement
                • Respecter les procédures de sauvegarde

                6.2 Gestion des équipements
                Les équipements (PC, téléphones, tablettes) :
                • Restent propriété de FULBANK
                • Ne doivent pas être utilisés par des tiers
                • Restitution obligatoire en fin de contrat

                6.3 Télétravail
                En télétravail :
                • Utiliser uniquement le matériel fourni par FULBANK
                • Connexion via VPN sécurisé obligatoire
                • Environnement confidentiel
                • Pas d'accès par des tiers

                6.4 Signalement des incidents
                Signalement immédiat :
                • Au responsable hiérarchique
                • Au service informatique : security@fulbank.com
                • Hotline sécurité 24/7 en cas d'urgence

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                7. TRAÇABILITÉ ET CONTRÔLE

                7.1 Traçabilité totale
                Toutes les actions sont tracées et conservées :
                • Connexions/déconnexions
                • Accès aux données clients
                • Opérations effectuées
                • Modifications de paramètres

                7.2 Droit de contrôle
                FULBANK se réserve le droit de contrôler l'utilisation des ressources dans le respect de la législation.

                7.3 Investigations
                En cas de suspicion, FULBANK peut analyser l'activité informatique de l'employé.

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                8. SANCTIONS

                8.1 Sanctions disciplinaires
                Selon la gravité :
                • Avertissement écrit
                • Mise à pied
                • Rétrogradation
                • Licenciement pour faute grave

                8.2 Sanctions pénales possibles
                • Violation du secret professionnel (art. 226-13)
                • Atteinte aux systèmes informatiques (art. 323-1)
                • Vol de données (art. 311-1)
                • Abus de confiance (art. 314-1)

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                9. DÉPART DE L'ENTREPRISE

                Lors du départ :
                • Restitution de tout équipement
                • Désactivation immédiate des accès
                • Maintien du secret professionnel à vie

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                10. CONTACT

                Service Informatique : support-it@fulbank.com
                RSSI : rssi@fulbank.com
                DRH : rh@fulbank.com

                FULBANK S.A. - Filiale du Groupe VYROS

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                ENGAGEMENT

                En acceptant cette charte, je reconnais avoir pris connaissance des règles d'utilisation des systèmes d'information de FULBANK et m'engage à les respecter.

                Je comprends que le non-respect peut entraîner des sanctions disciplinaires pouvant aller jusqu'au licenciement, ainsi que d'éventuelles poursuites pénales.

                Mon acceptation sera enregistrée avec la date et l'heure actuelles.
                ";
        }
    }

}

