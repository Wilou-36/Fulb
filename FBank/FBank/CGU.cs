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
    public partial class CGU : Form
    {
        public CGU()
        {
            InitializeComponent();
            ConfigurerInterface();
        }

        private void CGU_Load(object sender, EventArgs e)
        {

        }

        private void ConfigurerInterface()
        {
            // Configuration du formulaire
            this.Text = "Conditions Générales d'Utilisation - FULBANK";
            this.Size = new Size(900, 700);
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
                Text = "CONDITIONS GÉNÉRALES D'UTILISATION",
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Location = new Point(30, 20)
            };

            Label sousTitreLabel = new Label
            {
                Text = "FULBANK - Application Bancaire",
                Font = new Font("Segoe UI", 14f, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(30, 60)
            };

            Label dateLabel = new Label
            {
                Text = "Date de mise à jour : 7 décembre 2024",
                Font = new Font("Segoe UI", 10f, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true,
                Location = new Point(30, 95)
            };

            // RichTextBox pour le contenu avec scroll
            RichTextBox contentBox = new RichTextBox
            {
                Location = new Point(30, 130),
                Size = new Size(820, 450),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            // Ajouter le contenu des CGU
            contentBox.Text = ObtenirTexteCGU();

            // Panel pour les boutons
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.White,
                Padding = new Padding(30, 15, 30, 15)
            };

            // CheckBox d'acceptation
            CheckBox acceptCheckBox = new CheckBox
            {
                Text = "J'ai lu et j'accepte les Conditions Générales d'Utilisation",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Location = new Point(30, 20)
            };

            // Bouton Accepter
            Button btnAccepter = new Button
            {
                Text = "Accepter et Continuer",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 150, 136),
                Size = new Size(200, 45),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false,
                Location = new Point(650, 12)
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
                Location = new Point(510, 12)
            };
            btnRefuser.FlatAppearance.BorderSize = 0;

            // Événement CheckBox
            acceptCheckBox.CheckedChanged += (s, e) =>
            {
                btnAccepter.Enabled = acceptCheckBox.Checked;
                btnAccepter.BackColor = acceptCheckBox.Checked
                    ? Color.FromArgb(0, 150, 136)
                    : Color.FromArgb(200, 200, 200);
            };

            // Événement bouton Accepter
            btnAccepter.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            // Événement bouton Refuser
            btnRefuser.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            // Effet hover sur les boutons
            btnAccepter.MouseEnter += (s, e) =>
            {
                if (btnAccepter.Enabled)
                    btnAccepter.BackColor = Color.FromArgb(0, 121, 107);
            };
            btnAccepter.MouseLeave += (s, e) =>
            {
                if (btnAccepter.Enabled)
                    btnAccepter.BackColor = Color.FromArgb(0, 150, 136);
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
            mainPanel.Controls.Add(contentBox);

            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
        }

        private string ObtenirTexteCGU()
        {
            return @"
                1. OBJET

                Les présentes Conditions Générales d'Utilisation (CGU) régissent l'utilisation de l'application mobile et web FULBANK, éditée par FULBANK S.A., établissement de crédit agréé.

                L'utilisation de l'application implique l'acceptation pleine et entière des présentes CGU.

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                2. DÉFINITIONS

                • Client : Toute personne physique ou morale titulaire d'un compte FULBANK
                • Application : L'ensemble des services bancaires accessibles via les interfaces web et mobile FULBANK
                • Services : Les opérations bancaires, de trading crypto et de gestion de patrimoine proposées
                • Identifiants : Code client et code PIN permettant l'accès sécurisé aux services

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                3. ACCÈS AUX SERVICES

                3.1 Conditions d'accès
                • Être majeur (18 ans minimum)
                • Avoir ouvert un compte FULBANK
                • Disposer d'un appareil compatible et d'une connexion internet
                • Accepter les présentes CGU

                3.2 Création de compte
                Le Client doit fournir des informations exactes et complètes lors de l'inscription, conformément aux obligations légales de connaissance client (KYC).

                3.3 Sécurité des identifiants
                Le Client s'engage à :
                • Conserver ses identifiants confidentiels
                • Ne jamais les communiquer à des tiers
                • Informer immédiatement FULBANK en cas de perte, vol ou utilisation frauduleuse
                • Utiliser l'authentification biométrique lorsque disponible

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                4. SERVICES BANCAIRES

                4.1 Comptes bancaires
                • Compte courant sans frais de tenue de compte
                • Livrets d'épargne rémunérés selon les taux en vigueur
                • Consultation des soldes et historiques en temps réel
                • Virements SEPA gratuits
                • Cartes bancaires premium avec assurances incluses

                4.2 Trading de cryptomonnaies
                • Achat et vente de Bitcoin, Ethereum, Litecoin
                • Cours en temps réel
                • Exécution instantanée des ordres
                • Frais transparents affichés avant validation
                • Historique détaillé des transactions

                ⚠️ AVERTISSEMENT : Les cryptomonnaies sont des actifs volatils présentant des risques de perte en capital. Le Client reconnaît être informé de ces risques.

                4.3 Gestion de patrimoine
                • Vue consolidée des actifs fiat et crypto
                • Outils d'allocation d'actifs
                • Alertes et notifications personnalisables
                • Reporting fiscal automatique
                • Accès à des conseils personnalisés

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                5. TARIFICATION

                Les tarifs des services sont disponibles dans la grille tarifaire accessible dans l'application et sur le site web.

                FULBANK se réserve le droit de modifier ses tarifs avec un préavis de 2 mois.

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                6. SÉCURITÉ ET PROTECTION DES DONNÉES

                6.1 Mesures de sécurité
                • Cold storage pour 90% des cryptomonnaies
                • Assurance des dépôts selon la réglementation en vigueur
                • Authentification forte et biométrique
                • Monitoring 24/7 des transactions
                • Chiffrement des données de bout en bout

                6.2 Protection des données personnelles
                FULBANK s'engage à protéger les données personnelles conformément au RGPD. Les données collectées sont nécessaires à la fourniture des services bancaires et au respect des obligations légales.

                Pour plus d'informations : consulter notre Politique de Confidentialité.

                6.3 Lutte contre le blanchiment
                FULBANK applique les règles de lutte contre le blanchiment d'argent et le financement du terrorisme. Des contrôles peuvent être effectués sur les transactions.

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                7. RESPONSABILITÉS

                7.1 Responsabilité de FULBANK
                FULBANK s'engage à fournir ses services avec diligence. Sa responsabilité ne peut être engagée en cas de :
                • Force majeure
                • Défaillance des réseaux de télécommunication
                • Utilisation frauduleuse des identifiants du Client
                • Volatilité des marchés cryptographiques

                7.2 Responsabilité du Client
                Le Client est responsable de :
                • L'exactitude des informations fournies
                • La sécurité de ses identifiants
                • Ses décisions d'investissement
                • La conformité fiscale de ses opérations

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                8. RÉCLAMATIONS ET LITIGES

                8.1 Service client
                Le Client peut contacter le service client :
                • Par téléphone : [numéro]
                • Par email : support@fulbank.com
                • Via le chat intégré à l'application

                8.2 Médiation
                En cas de litige, le Client peut saisir le Médiateur bancaire :
                • Adresse : [adresse du médiateur]
                • Email : mediateur@fulbank.com

                8.3 Droit applicable
                Les présentes CGU sont soumises au droit français. En cas de litige non résolu par médiation, les tribunaux français sont compétents.

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                9. DURÉE ET RÉSILIATION

                9.1 Durée
                Le contrat est conclu pour une durée indéterminée.

                9.2 Résiliation
                Chaque partie peut résilier le contrat :
                • Par le Client : à tout moment, sans frais, avec un préavis de 30 jours
                • Par FULBANK : avec un préavis de 2 mois, sauf faute grave du Client

                9.3 Effet de la résiliation
                En cas de résiliation :
                • Les comptes sont clôturés
                • Les soldes sont restitués selon les instructions du Client
                • Les cryptomonnaies sont transférées vers un wallet externe
                • L'accès à l'application est désactivé

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                10. MODIFICATIONS DES CGU

                FULBANK se réserve le droit de modifier les présentes CGU. Le Client est informé par email ou notification 1 mois avant l'entrée en vigueur des modifications.

                L'utilisation continue de l'application après modification vaut acceptation des nouvelles CGU.

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                11. DISPOSITIONS DIVERSES

                11.1 Intégralité du contrat
                Les présentes CGU constituent l'intégralité de l'accord entre FULBANK et le Client.

                11.2 Nullité partielle
                Si une clause est déclarée nulle, les autres clauses restent applicables.

                11.3 Absence de renonciation
                Le fait pour FULBANK de ne pas exercer un droit ne constitue pas une renonciation à ce droit.

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                12. CONTACT

                FULBANK S.A.
                Capital social : [montant]
                RCS : [numéro]
                Siège social : [adresse]
                Email : contact@fulbank.com
                Téléphone : [numéro]

                Autorité de contrôle : ACPR (Autorité de Contrôle Prudentiel et de Résolution)

                ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                En utilisant l'application FULBANK, vous reconnaissez avoir lu, compris et accepté les présentes Conditions Générales d'Utilisation.
                ";
        }
    }
}

