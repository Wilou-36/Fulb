namespace FBank
{
    partial class nexAdmin
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
            this.lblNom = new System.Windows.Forms.Label();
            this.lblPrenom = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnAnnulation = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Rockwell", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(180, 130);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(655, 59);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ajouter un  administrateur";
            // 
            // lblNom
            // 
            this.lblNom.AutoSize = true;
            this.lblNom.Font = new System.Drawing.Font("Rockwell", 14F, System.Drawing.FontStyle.Bold);
            this.lblNom.Location = new System.Drawing.Point(238, 306);
            this.lblNom.Name = "lblNom";
            this.lblNom.Size = new System.Drawing.Size(55, 23);
            this.lblNom.TabIndex = 1;
            this.lblNom.Text = "Nom";
            // 
            // lblPrenom
            // 
            this.lblPrenom.AutoSize = true;
            this.lblPrenom.Font = new System.Drawing.Font("Rockwell", 14F, System.Drawing.FontStyle.Bold);
            this.lblPrenom.Location = new System.Drawing.Point(543, 306);
            this.lblPrenom.Name = "lblPrenom";
            this.lblPrenom.Size = new System.Drawing.Size(85, 23);
            this.lblPrenom.TabIndex = 2;
            this.lblPrenom.Text = "Prenom";
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new System.Drawing.Font("Rockwell", 14F, System.Drawing.FontStyle.Bold);
            this.lblRole.Location = new System.Drawing.Point(238, 400);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(53, 23);
            this.lblRole.TabIndex = 3;
            this.lblRole.Text = "Rôle";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(333, 310);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(141, 20);
            this.textBox1.TabIndex = 4;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(333, 400);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(141, 20);
            this.textBox2.TabIndex = 5;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(643, 310);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(141, 20);
            this.textBox3.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Rockwell", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(563, 501);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(175, 56);
            this.button1.TabIndex = 7;
            this.button1.Text = "Suivant";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnAnnulation
            // 
            this.btnAnnulation.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnnulation.Location = new System.Drawing.Point(310, 499);
            this.btnAnnulation.Name = "btnAnnulation";
            this.btnAnnulation.Size = new System.Drawing.Size(211, 58);
            this.btnAnnulation.TabIndex = 16;
            this.btnAnnulation.Text = "Annuler la création d\'un nouveau compte";
            this.btnAnnulation.UseVisualStyleBackColor = true;
            // 
            // nexAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 608);
            this.Controls.Add(this.btnAnnulation);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lblRole);
            this.Controls.Add(this.lblPrenom);
            this.Controls.Add(this.lblNom);
            this.Controls.Add(this.label1);
            this.Name = "nexAdmin";
            this.Text = "nexAdmin";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblNom;
        private System.Windows.Forms.Label lblPrenom;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnAnnulation;
    }
}