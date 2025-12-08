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
    public partial class PageAdmin : Form
    {
        public PageAdmin()
        {
            InitializeComponent();

            // Ajouter le logo
            LogoHelper.AddLogoToForm(this);
        }

        private void PageAdmin_Load(object sender, EventArgs e)
        {

        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            newUser newUserForm = new newUser();
            newUserForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loginAdmin loginAdminForm = new loginAdmin();
            loginAdminForm.Show();
        }

        private void btnSupp_Click(object sender, EventArgs e)
        {
            dropClient dropClientForm = new dropClient();
            dropClientForm = new dropClient();
        }

        private void btnCeaAdmin_Click(object sender, EventArgs e)
        {
            nexAdmin nexAdminForm = new nexAdmin();
            nexAdminForm.Show();
        }
    }
}
