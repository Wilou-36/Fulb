using System;
using System.Windows.Forms;
using Bank.Banque.UI;

namespace Bank
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmLogin());  
        }
    }
}