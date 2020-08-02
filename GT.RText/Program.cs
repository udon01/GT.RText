using System;
using System.Windows.Forms;
using GT.Shared.Logging;

namespace GT.RText
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new NDB0.Core.NDB0("CarName.dat", new FileWriter());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
