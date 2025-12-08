namespace FBApp1
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.clientLog = new System.Windows.Forms.Button();
            this.adminlog = new System.Windows.Forms.Button();
            this.title = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // clientLog
            // 
            this.clientLog.Font = new System.Drawing.Font("Rockwell", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clientLog.Location = new System.Drawing.Point(467, 302);
            this.clientLog.Name = "clientLog";
            this.clientLog.Size = new System.Drawing.Size(199, 51);
            this.clientLog.TabIndex = 0;
            this.clientLog.Text = "Client";
            this.clientLog.UseVisualStyleBackColor = true;
            // 
            // adminlog
            // 
            this.adminlog.Font = new System.Drawing.Font("Rockwell", 26.25F);
            this.adminlog.Location = new System.Drawing.Point(133, 302);
            this.adminlog.Name = "adminlog";
            this.adminlog.Size = new System.Drawing.Size(199, 51);
            this.adminlog.TabIndex = 1;
            this.adminlog.Text = "Admin";
            this.adminlog.UseVisualStyleBackColor = true;
            this.adminlog.Click += new System.EventHandler(this.button2_Click);
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.title.Font = new System.Drawing.Font("Rockwell", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(254, 91);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(385, 162);
            this.title.TabIndex = 2;
            this.title.Text = "Bienvenue chez \r\n      Fulbank\r\n";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 476);
            this.Controls.Add(this.title);
            this.Controls.Add(this.adminlog);
            this.Controls.Add(this.clientLog);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button clientLog;
        private System.Windows.Forms.Button adminlog;
        private System.Windows.Forms.Label title;
    }
}

