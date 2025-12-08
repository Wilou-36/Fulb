using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FBank
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            LogoHelper.AddLogoToForm(this);
        }

        

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            loginAdmin loginAdminForm = new loginAdmin();
            loginAdminForm.Show();
        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            loginClientcs loginClientForm = new loginClientcs();
            loginClientForm.Show();
        }

        
    }
}
