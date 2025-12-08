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
    public partial class dropClient : Form
    {
        public dropClient()
        {
            InitializeComponent();

            LogoHelper.AddLogoToForm(this);
        }

        private void dropClient_Load(object sender, EventArgs e)
        {

        }
    }
}
