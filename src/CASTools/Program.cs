using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XMODS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Debug.WriteLine("Starting app");
            #if NETCORE
             Application.SetDefaultFont(new Font(new FontFamily("Microsoft Sans Serif"), 8f));
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
            #endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.Run(new Form1());
        }
    }
}
