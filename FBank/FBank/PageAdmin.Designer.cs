namespace FBank
{
    partial class PageAdmin
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
            this.txtPage = new System.Windows.Forms.Label();
            this.btnClient = new System.Windows.Forms.Button();
            this.btnSupp = new System.Windows.Forms.Button();
            this.btnConsulte = new System.Windows.Forms.Button();
            this.btnAction = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCeaAdmin = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtPage
            // 
            this.txtPage.AutoSize = true;
            this.txtPage.Font = new System.Drawing.Font("Rockwell", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPage.Location = new System.Drawing.Point(136, 143);
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(795, 59);
            this.txtPage.TabIndex = 0;
            this.txtPage.Text = "Bienvenue chère administrateur";
            this.txtPage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnClient
            // 
            this.btnClient.Font = new System.Drawing.Font("Rockwell", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClient.Location = new System.Drawing.Point(195, 281);
            this.btnClient.Name = "btnClient";
            this.btnClient.Size = new System.Drawing.Size(210, 84);
            this.btnClient.TabIndex = 1;
            this.btnClient.Text = "Créer de nouveau client";
            this.btnClient.UseVisualStyleBackColor = true;
            this.btnClient.Click += new System.EventHandler(this.btnClient_Click);
            // 
            // btnSupp
            // 
            this.btnSupp.Font = new System.Drawing.Font("Rockwell", 15.75F, System.Drawing.FontStyle.Bold);
            this.btnSupp.Location = new System.Drawing.Point(195, 397);
            this.btnSupp.Name = "btnSupp";
            this.btnSupp.Size = new System.Drawing.Size(210, 75);
            this.btnSupp.TabIndex = 2;
            this.btnSupp.Text = "Supprimer un client";
            this.btnSupp.UseVisualStyleBackColor = true;
            this.btnSupp.Click += new System.EventHandler(this.btnSupp_Click);
            // 
            // btnConsulte
            // 
            this.btnConsulte.Font = new System.Drawing.Font("Rockwell", 15.75F, System.Drawing.FontStyle.Bold);
            this.btnConsulte.Location = new System.Drawing.Point(441, 281);
            this.btnConsulte.Name = "btnConsulte";
            this.btnConsulte.Size = new System.Drawing.Size(203, 84);
            this.btnConsulte.TabIndex = 3;
            this.btnConsulte.Text = "Consulter des comptes";
            this.btnConsulte.UseVisualStyleBackColor = true;
            // 
            // btnAction
            // 
            this.btnAction.Font = new System.Drawing.Font("Rockwell", 15.75F, System.Drawing.FontStyle.Bold);
            this.btnAction.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnAction.Location = new System.Drawing.Point(441, 401);
            this.btnAction.Name = "btnAction";
            this.btnAction.Size = new System.Drawing.Size(203, 71);
            this.btnAction.TabIndex = 4;
            this.btnAction.Text = "Cours des actions";
            this.btnAction.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Rockwell", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(821, 514);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(214, 59);
            this.button1.TabIndex = 5;
            this.button1.Text = "se déconnecter";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCeaAdmin
            // 
            this.btnCeaAdmin.Font = new System.Drawing.Font("Rockwell", 15.75F, System.Drawing.FontStyle.Bold);
            this.btnCeaAdmin.Location = new System.Drawing.Point(686, 281);
            this.btnCeaAdmin.Name = "btnCeaAdmin";
            this.btnCeaAdmin.Size = new System.Drawing.Size(195, 84);
            this.btnCeaAdmin.TabIndex = 6;
            this.btnCeaAdmin.Text = "Créer un nouvel Admin";
            this.btnCeaAdmin.UseVisualStyleBackColor = true;
            this.btnCeaAdmin.Click += new System.EventHandler(this.btnCeaAdmin_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Rockwell", 15.75F, System.Drawing.FontStyle.Bold);
            this.button2.Location = new System.Drawing.Point(686, 401);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(195, 66);
            this.button2.TabIndex = 7;
            this.button2.Text = "Supprimer un Admin";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // PageAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 608);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnCeaAdmin);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnAction);
            this.Controls.Add(this.btnConsulte);
            this.Controls.Add(this.btnSupp);
            this.Controls.Add(this.btnClient);
            this.Controls.Add(this.txtPage);
            this.Name = "PageAdmin";
            this.Text = "PageAdmin";
            this.Load += new System.EventHandler(this.PageAdmin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label txtPage;
        private System.Windows.Forms.Button btnClient;
        private System.Windows.Forms.Button btnSupp;
        private System.Windows.Forms.Button btnConsulte;
        private System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnCeaAdmin;
        private System.Windows.Forms.Button button2;
    }
}