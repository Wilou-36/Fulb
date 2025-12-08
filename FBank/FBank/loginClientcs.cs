using System;
using System.Windows.Forms;

namespace FBank
{
    public partial class loginClientcs : Form
    {
        public loginClientcs()
        {
            InitializeComponent();

            // Ajouter le logo
            LogoHelper.AddLogoToForm(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            codeLoginClient codeloginClientForm = new codeLoginClient();
            codeloginClientForm.Show();
        }
    }
}
