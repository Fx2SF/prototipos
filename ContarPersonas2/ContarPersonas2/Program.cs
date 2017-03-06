using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContarPersonas2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += Application_ApplicationExit;
            try
            {
                Application.Run(new Form1());

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Debug.WriteLine("Exit sender: " + sender);
            Debug.WriteLine("Ev: " + e);
        }
    }
}
