using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WHOperation
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
            //Application.Run(new MDIParent1());
            Application.Run(new Form1());
            //Application.Run(new fLogin());
            //Application.Run(new vendorLabelMaster());
        }
    }
}