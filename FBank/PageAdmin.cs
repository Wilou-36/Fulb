csharp FBank\PageAdmin.cs
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
        private readonly loginAdmin _loginForm;

        public PageAdmin()
        {
            InitializeComponent();
        }

        // Nouveau constructeur qui reçoit la référence du formulaire de connexion
        public PageAdmin(loginAdmin loginForm) : this()
        {
            _loginForm = loginForm;
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
            // Déconnexion : réafficher le formulaire de login si on a la référence,
            // sinon en créer un nouveau. Puis fermer PageAdmin.
            if (_loginForm != null)
            {
                _loginForm.Show();
            }
            else
            {
                var loginAdminForm = new loginAdmin();
                loginAdminForm.Show();
            }

            this.Close();
        }
    }
}