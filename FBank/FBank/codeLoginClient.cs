using System;
using System.Drawing;
using System.Windows.Forms;

namespace FBank
{
    public partial class codeLoginClient : Form
    {
        public codeLoginClient()
        {
            InitializeComponent();

            // Forcer le masquage et la longueur max à 4 au runtime
            if (textBox1 != null)
            {
                textBox1.PasswordChar = '*';
                textBox1.UseSystemPasswordChar = false;
                textBox1.MaxLength = 4;
                textBox1.Font = new Font(textBox1.Font.FontFamily, 14f, textBox1.Font.Style);
            }

            // Ajouter le logo
            LogoHelper.AddLogoToForm(this);
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            AppendDigit("1");
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            AppendDigit("2");
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            AppendDigit("3");
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            AppendDigit("4");
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            AppendDigit("5");
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            AppendDigit("6");
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            AppendDigit("7");
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            AppendDigit("8");
        }

        private void btn9_Click(object sender, EventArgs e)
        {
            AppendDigit("9");
        }

        private void btn0_Click(object sender, EventArgs e)
        {
            AppendDigit("0");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void AppendDigit(string digit)
        {
            if (textBox1 == null)
                return;

            // Respecter la longueur maximale définie (4)
            if (textBox1.Text.Length >= textBox1.MaxLength)
                return;

            textBox1.Text += digit;
            // placer le curseur à la fin pour une bonne UX
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.SelectionLength = 0;
        }
    }
}
