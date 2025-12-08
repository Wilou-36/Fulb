namespace FBank
{
    partial class newUser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblCrea = new System.Windows.Forms.Label();
            this.lblnomClient = new System.Windows.Forms.Label();
            this.lblPrenomClient = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblRevenu = new System.Windows.Forms.Label();
            this.txtNom = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtProfession = new System.Windows.Forms.TextBox();
            this.txtRevenu = new System.Windows.Forms.TextBox();
            this.txtPrenom = new System.Windows.Forms.TextBox();
            this.dtpNaissance = new System.Windows.Forms.DateTimePicker();
            this.btnSuivant = new System.Windows.Forms.Button();
            this.btnAnnulation = new System.Windows.Forms.Button();
            this.lblProfession = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblCrea
            // 
            this.lblCrea.AutoSize = true;
            this.lblCrea.Font = new System.Drawing.Font("Rockwell", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCrea.Location = new System.Drawing.Point(189, 139);
            this.lblCrea.Name = "lblCrea";
            this.lblCrea.Size = new System.Drawing.Size(639, 46);
            this.lblCrea.TabIndex = 0;
            this.lblCrea.Text = "Formulaire de création de compte";
            // 
            // lblnomClient
            // 
            this.lblnomClient.AutoSize = true;
            this.lblnomClient.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblnomClient.Location = new System.Drawing.Point(125, 256);
            this.lblnomClient.Name = "lblnomClient";
            this.lblnomClient.Size = new System.Drawing.Size(47, 19);
            this.lblnomClient.TabIndex = 1;
            this.lblnomClient.Text = "Nom";
            // 
            // lblPrenomClient
            // 
            this.lblPrenomClient.AutoSize = true;
            this.lblPrenomClient.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrenomClient.Location = new System.Drawing.Point(543, 256);
            this.lblPrenomClient.Name = "lblPrenomClient";
            this.lblPrenomClient.Size = new System.Drawing.Size(71, 19);
            this.lblPrenomClient.TabIndex = 3;
            this.lblPrenomClient.Text = "Prenom";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(543, 317);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 38);
            this.label3.TabIndex = 4;
            this.label3.Text = "Date de \r\nnaissance";
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmail.Location = new System.Drawing.Point(125, 321);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(57, 19);
            this.lblEmail.TabIndex = 5;
            this.lblEmail.Text = "Email";
            // 
            // lblRevenu
            // 
            this.lblRevenu.AutoSize = true;
            this.lblRevenu.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRevenu.Location = new System.Drawing.Point(543, 394);
            this.lblRevenu.Name = "lblRevenu";
            this.lblRevenu.Size = new System.Drawing.Size(82, 38);
            this.lblRevenu.TabIndex = 6;
            this.lblRevenu.Text = "Revenu\r\n Mensuel";
            this.lblRevenu.Click += new System.EventHandler(this.label5_Click);
            // 
            // txtNom
            // 
            this.txtNom.Location = new System.Drawing.Point(230, 257);
            this.txtNom.Name = "txtNom";
            this.txtNom.Size = new System.Drawing.Size(211, 20);
            this.txtNom.TabIndex = 7;
            this.txtNom.TextChanged += new System.EventHandler(this.txtNom_TextChanged);
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(230, 320);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(211, 20);
            this.txtEmail.TabIndex = 8;
            this.txtEmail.TextChanged += new System.EventHandler(this.txtEmail_TextChanged);
            // 
            // txtProfession
            // 
            this.txtProfession.Location = new System.Drawing.Point(230, 393);
            this.txtProfession.Name = "txtProfession";
            this.txtProfession.Size = new System.Drawing.Size(211, 20);
            this.txtProfession.TabIndex = 9;
            this.txtProfession.TextChanged += new System.EventHandler(this.txtProfession_TextChanged);
            // 
            // txtRevenu
            // 
            this.txtRevenu.Location = new System.Drawing.Point(667, 394);
            this.txtRevenu.Name = "txtRevenu";
            this.txtRevenu.Size = new System.Drawing.Size(211, 20);
            this.txtRevenu.TabIndex = 10;
            this.txtRevenu.TextChanged += new System.EventHandler(this.txtRevenu_TextChanged);
            // 
            // txtPrenom
            // 
            this.txtPrenom.Location = new System.Drawing.Point(667, 255);
            this.txtPrenom.Name = "txtPrenom";
            this.txtPrenom.Size = new System.Drawing.Size(211, 20);
            this.txtPrenom.TabIndex = 11;
            this.txtPrenom.TextChanged += new System.EventHandler(this.txtPrenom_TextChanged);
            // 
            // dtpNaissance
            // 
            this.dtpNaissance.Location = new System.Drawing.Point(667, 319);
            this.dtpNaissance.Name = "dtpNaissance";
            this.dtpNaissance.Size = new System.Drawing.Size(211, 20);
            this.dtpNaissance.TabIndex = 12;
            this.dtpNaissance.ValueChanged += new System.EventHandler(this.dtpNaissance_ValueChanged);
            // 
            // btnSuivant
            // 
            this.btnSuivant.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSuivant.Location = new System.Drawing.Point(555, 513);
            this.btnSuivant.Name = "btnSuivant";
            this.btnSuivant.Size = new System.Drawing.Size(211, 58);
            this.btnSuivant.TabIndex = 13;
            this.btnSuivant.Text = "étape suivante";
            this.btnSuivant.UseVisualStyleBackColor = true;
            this.btnSuivant.Click += new System.EventHandler(this.btnSuivant_Click);
            // 
            // btnAnnulation
            // 
            this.btnAnnulation.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnnulation.Location = new System.Drawing.Point(278, 513);
            this.btnAnnulation.Name = "btnAnnulation";
            this.btnAnnulation.Size = new System.Drawing.Size(211, 58);
            this.btnAnnulation.TabIndex = 14;
            this.btnAnnulation.Text = "Annuler la création d\'un nouveau compte";
            this.btnAnnulation.UseVisualStyleBackColor = true;
            this.btnAnnulation.Click += new System.EventHandler(this.btnAnnulation_Click);
            // 
            // lblProfession
            // 
            this.lblProfession.AutoSize = true;
            this.lblProfession.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProfession.Location = new System.Drawing.Point(125, 396);
            this.lblProfession.Name = "lblProfession";
            this.lblProfession.Size = new System.Drawing.Size(92, 19);
            this.lblProfession.TabIndex = 15;
            this.lblProfession.Text = "Profession";
            // 
            // newUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 608);
            this.Controls.Add(this.lblProfession);
            this.Controls.Add(this.btnAnnulation);
            this.Controls.Add(this.btnSuivant);
            this.Controls.Add(this.dtpNaissance);
            this.Controls.Add(this.txtPrenom);
            this.Controls.Add(this.txtRevenu);
            this.Controls.Add(this.txtProfession);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtNom);
            this.Controls.Add(this.lblRevenu);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblPrenomClient);
            this.Controls.Add(this.lblnomClient);
            this.Controls.Add(this.lblCrea);
            this.Name = "newUser";
            this.Text = "newUser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCrea;
        private System.Windows.Forms.Label lblnomClient;
        private System.Windows.Forms.Label lblPrenomClient;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblRevenu;
        private System.Windows.Forms.TextBox txtNom;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtProfession;
        private System.Windows.Forms.TextBox txtRevenu;
        private System.Windows.Forms.TextBox txtPrenom;
        private System.Windows.Forms.DateTimePicker dtpNaissance;
        private System.Windows.Forms.Button btnSuivant;
        private System.Windows.Forms.Button btnAnnulation;
        private System.Windows.Forms.Label lblProfession;
    }
}