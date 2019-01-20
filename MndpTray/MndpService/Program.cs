using System;
using System.Configuration.Install;
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
                if ((args != null) && (args.Length > 0))
                {
                    string cmd = args[0].ToUpper();

                    if (cmd == nameof(Install).ToUpper()) Install();
                    if (cmd == nameof(Uninstall).ToUpper()) Uninstall();
                }
                else
                {
                    Usage();
                }
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

        private static void Install()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetEntryAssembly().Location });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }

        private static void Uninstall()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetEntryAssembly().Location });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }

        private static void Usage()
        {
            Console.WriteLine(Assembly.GetEntryAssembly().FullName);
            Console.WriteLine("Usage:");
            Console.WriteLine(Assembly.GetEntryAssembly().GetName().Name + " Install - Install Service");
            Console.WriteLine(Assembly.GetEntryAssembly().GetName().Name + " Uninstall - Uninstall Service ");
            Console.ReadLine();
        }
        #endregion Methods
    }
}