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
    public partial class nexAdmin : Form
    {
        public nexAdmin()
        {
            InitializeComponent();

            LogoHelper.AddLogoToForm(this);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            newAdmin2 newAdmin2Form = new newAdmin2();
            newAdmin2Form.Show();
        }
    }
}
