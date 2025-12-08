namespace FBank
{
    partial class newUser2
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblpassword = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMDP = new System.Windows.Forms.TextBox();
            this.txtTrueMDP = new System.Windows.Forms.TextBox();
            this.btnPrecedent = new System.Windows.Forms.Button();
            this.btnFinaliser = new System.Windows.Forms.Button();
            this.btnAnnulation = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Rockwell", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(216, 161);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(613, 59);
            this.label1.TabIndex = 0;
            this.label1.Text = "création de mot de passe";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // lblpassword
            // 
            this.lblpassword.AutoSize = true;
            this.lblpassword.Font = new System.Drawing.Font("Rockwell", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblpassword.Location = new System.Drawing.Point(267, 293);
            this.lblpassword.Name = "lblpassword";
            this.lblpassword.Size = new System.Drawing.Size(110, 25);
            this.lblpassword.TabIndex = 1;
            this.lblpassword.Text = "password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Rockwell", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(205, 363);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(252, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "confirmation password ";
            // 
            // txtMDP
            // 
            this.txtMDP.Location = new System.Drawing.Point(559, 293);
            this.txtMDP.Name = "txtMDP";
            this.txtMDP.Size = new System.Drawing.Size(201, 20);
            this.txtMDP.TabIndex = 3;
            this.txtMDP.TextChanged += new System.EventHandler(this.txtMDP_TextChanged);
            // 
            // txtTrueMDP
            // 
            this.txtTrueMDP.Location = new System.Drawing.Point(559, 363);
            this.txtTrueMDP.Name = "txtTrueMDP";
            this.txtTrueMDP.Size = new System.Drawing.Size(201, 20);
            this.txtTrueMDP.TabIndex = 4;
            this.txtTrueMDP.TextChanged += new System.EventHandler(this.txtTrueMDP_TextChanged);
            // 
            // btnPrecedent
            // 
            this.btnPrecedent.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrecedent.Location = new System.Drawing.Point(434, 504);
            this.btnPrecedent.Name = "btnPrecedent";
            this.btnPrecedent.Size = new System.Drawing.Size(186, 61);
            this.btnPrecedent.TabIndex = 5;
            this.btnPrecedent.Text = "étape précédente";
            this.btnPrecedent.UseVisualStyleBackColor = true;
            this.btnPrecedent.Click += new System.EventHandler(this.btnPrecedent_Click);
            // 
            // btnFinaliser
            // 
            this.btnFinaliser.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFinaliser.Location = new System.Drawing.Point(683, 504);
            this.btnFinaliser.Name = "btnFinaliser";
            this.btnFinaliser.Size = new System.Drawing.Size(186, 61);
            this.btnFinaliser.TabIndex = 6;
            this.btnFinaliser.Text = "finaliser création de compte";
            this.btnFinaliser.UseVisualStyleBackColor = true;
            this.btnFinaliser.Click += new System.EventHandler(this.btnFinaliser_Click);
            // 
            // btnAnnulation
            // 
            this.btnAnnulation.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnnulation.Location = new System.Drawing.Point(164, 504);
            this.btnAnnulation.Name = "btnAnnulation";
            this.btnAnnulation.Size = new System.Drawing.Size(211, 58);
            this.btnAnnulation.TabIndex = 15;
            this.btnAnnulation.Text = "Annuler la création d\'un nouveau compte";
            this.btnAnnulation.UseVisualStyleBackColor = true;
            this.btnAnnulation.Click += new System.EventHandler(this.btnAnnulation_Click);
            // 
            // newUser2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 608);
            this.Controls.Add(this.btnAnnulation);
            this.Controls.Add(this.btnFinaliser);
            this.Controls.Add(this.btnPrecedent);
            this.Controls.Add(this.txtTrueMDP);
            this.Controls.Add(this.txtMDP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblpassword);
            this.Controls.Add(this.label1);
            this.Name = "newUser2";
            this.Text = "newUser2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblpassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMDP;
        private System.Windows.Forms.TextBox txtTrueMDP;
        private System.Windows.Forms.Button btnPrecedent;
        private System.Windows.Forms.Button btnFinaliser;
        private System.Windows.Forms.Button btnAnnulation;
    }
}