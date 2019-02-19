using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;

namespace MndpService
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (!Environment.UserInteractive)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new MndpService()
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                ProjectInstaller.Go(args);
            }
        }

        #region Fields

        private static readonly bool LOG_FILE_IS_ENABLED = File.Exists(LOG_FILE_NAME);
        private static readonly object LOG_FILE_LOCK = new object();
        private static readonly string LOG_FILE_NAME = GetLogFileName("log");

        #endregion Fields

        #region Event Handlers

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Log("CurrentDomain_UnhandledException {0}", e.ExceptionObject.ToString());
            }
            catch { }
        }

        #endregion Event Handlers

        #region Methods

        public static string GetLogFileName(string extension)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            string dir = Path.GetDirectoryName(assembly.Location);
            string file = Path.GetFileNameWithoutExtension(assembly.Location);

            file = Path.Combine(dir, file);
            return file + "." + extension;
        }

        public static void Log(string format, params object[] args)
        {
            try
            {
                String str = String.Format(format, args);
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
            catch { }
        }

        #endregion Methods
    }
}