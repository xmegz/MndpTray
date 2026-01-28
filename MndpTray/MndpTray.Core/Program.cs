/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Core
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

            if (System.Diagnostics.Process.GetProcessesByName(processName).Length > 1)
                return;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Log("------------------< START >------------------");
            Application.Run(new Visual.NotifyContext());
            Log("-------------------< END >-------------------");
        }

        #region Fields

        private static readonly bool LOG_FILE_IS_ENABLED = File.Exists(GetLogFileName("log"));
        private static readonly object LOG_FILE_LOCK = new object();
        private static readonly string LOG_FILE_NAME = GetLogFileName("log");

        #endregion Fields

        public static void Log(string format, params object[] args)
        {
            try
            {
                string str = string.Format(format, args);
                System.Diagnostics.Debug.WriteLine(str);

                str = string.Concat("<", DateTime.Now.ToString(), "> ", str, Environment.NewLine);

                if (LOG_FILE_IS_ENABLED)
                {
                    lock (LOG_FILE_LOCK)
                    {
                        File.AppendAllText(LOG_FILE_NAME, str);
                    }
                }
            }
            catch
            {
            }
        }

        #region Event Handlers

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Log("CurrentDomain_UnhandledException {0}", e.ExceptionObject.ToString());
            }
            catch
            {
            }
        }

        #endregion Event Handlers

        private static string GetLogFileName(string extension)
        {
            string file = Environment.ProcessPath;
            return file + "." + extension;
        }
    }
}