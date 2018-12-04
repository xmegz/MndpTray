using System;
using System.Windows.Forms;

namespace MndpTray
{
    static class Program
    {    
        [STAThread]
        static void Main()
        {            
            string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            if (System.Diagnostics.Process.GetProcessesByName(processName).Length>1)
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new NotifyContext());
        }
    }
}
