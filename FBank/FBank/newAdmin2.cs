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
    public partial class newAdmin2 : Form
    {
        public newAdmin2()
        {
            InitializeComponent();
            LogoHelper.AddLogoToForm(this);
        }

        private void btnPrecedent_Click(object sender, EventArgs e)
        {
            nexAdmin nexAdminForm = new nexAdmin();
            nexAdminForm.Show();
        }

        private void btnFinaliser_Click(object sender, EventArgs e)
        {
            CGUAdmin cGUAdminForm = new CGUAdmin();
            cGUAdminForm.Show();
        }

        private void txtMDP_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && tb.PasswordChar != '*')
            {
                tb.PasswordChar = '*';
                tb.UseSystemPasswordChar = false;
            }
        }

        private void txtTrueMDP_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && tb.PasswordChar != '*')
            {
                tb.PasswordChar = '*';
                tb.UseSystemPasswordChar = false;
            }
        }
    }
}
