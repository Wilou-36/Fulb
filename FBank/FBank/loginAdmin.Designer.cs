namespace FBank
{
    partial class loginAdmin
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
            this.lblUser = new System.Windows.Forms.Label();
            this.lblpassword = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.lblConnexion = new System.Windows.Forms.Label();
            this.btnConnexion = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Font = new System.Drawing.Font("Rockwell", 16F, System.Drawing.FontStyle.Bold);
            this.lblUser.Location = new System.Drawing.Point(236, 321);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(70, 27);
            this.lblUser.TabIndex = 0;
            this.lblUser.Text = "USER";
            // 
            // lblpassword
            // 
            this.lblpassword.AutoSize = true;
            this.lblpassword.Font = new System.Drawing.Font("Rockwell", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblpassword.Location = new System.Drawing.Point(236, 387);
            this.lblpassword.Name = "lblpassword";
            this.lblpassword.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblpassword.Size = new System.Drawing.Size(134, 25);
            this.lblpassword.TabIndex = 1;
            this.lblpassword.Text = "PASSWORD";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(396, 320);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(383, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // txtPass
            // 
            this.txtPass.Location = new System.Drawing.Point(396, 386);
            this.txtPass.Name = "txtPass";
            this.txtPass.Size = new System.Drawing.Size(383, 20);
            this.txtPass.TabIndex = 3;
            this.txtPass.TextChanged += new System.EventHandler(this.txtPass_TextChanged);
            // 
            // lblConnexion
            // 
            this.lblConnexion.AutoSize = true;
            this.lblConnexion.Font = new System.Drawing.Font("Rockwell", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnexion.Location = new System.Drawing.Point(281, 144);
            this.lblConnexion.Name = "lblConnexion";
            this.lblConnexion.Size = new System.Drawing.Size(451, 59);
            this.lblConnexion.TabIndex = 4;
            this.lblConnexion.Text = "Connexion Admin";
            // 
            // btnConnexion
            // 
            this.btnConnexion.Font = new System.Drawing.Font("Rockwell", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnexion.Location = new System.Drawing.Point(731, 501);
            this.btnConnexion.Name = "btnConnexion";
            this.btnConnexion.Size = new System.Drawing.Size(161, 50);
            this.btnConnexion.TabIndex = 5;
            this.btnConnexion.Text = "Se connecter";
            this.btnConnexion.UseVisualStyleBackColor = true;
            this.btnConnexion.Click += new System.EventHandler(this.btnConnexion_Click);
            // 
            // loginAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 608);
            this.Controls.Add(this.btnConnexion);
            this.Controls.Add(this.lblConnexion);
            this.Controls.Add(this.txtPass);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lblpassword);
            this.Controls.Add(this.lblUser);
            this.Name = "loginAdmin";
            this.Text = "loginAdmin";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblpassword;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Label lblConnexion;
        private System.Windows.Forms.Button btnConnexion;
    }
}