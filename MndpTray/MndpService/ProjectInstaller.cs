using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Text;

namespace MndpService
{
    /// <summary>
    /// Generic Service installer class
    /// </summary>
    [RunInstaller(true)]
    [DesignerCategory("Code")]
    public class ProjectInstaller : Installer
    {
        #region Fields

        /// <summary>
        /// This class is called by the install utility when installing a service application.
        /// </summary>
        private readonly ServiceInstaller _serviceInstaller;

        /// <summary>
        /// This class is called by installation utilities, such as InstallUtil.exe, when installing a service application.
        /// </summary>
        private readonly ServiceProcessInstaller _serviceProcessInstaller;

        #endregion Fields

        public ProjectInstaller()
        {
            //
            // configure serviceProcessInstaller
            //
            this._serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalService,
                Password = null,
                Username = null
            };

            //
            // configure serviceInstaller
            //
            this._serviceInstaller = new ServiceInstaller
            {
                Description = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>().Description,
                DisplayName = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title,
                ServiceName = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title,
                StartType = ServiceStartMode.Automatic
            };

            //
            // configure projectInstaller
            //
            this.Installers.AddRange(new Installer[] {
            this._serviceProcessInstaller,
            this._serviceInstaller});

            //
            // configure afterInstallEventHandler
            //
            this.AfterInstall += new InstallEventHandler(this.ServiceInstaller_AfterInstall);
        }

        #region Static

        /// <summary>
        /// Get usage
        /// </summary>
        /// <returns>Usage description string</returns>
        public static string GetUsage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Assembly.GetEntryAssembly().FullName);
            sb.AppendLine("Usage:");
            sb.AppendLine(Assembly.GetEntryAssembly().GetName().Name + " Install - Install Service");
            sb.AppendLine(Assembly.GetEntryAssembly().GetName().Name + " Uninstall - Uninstall Service ");
            sb.AppendLine(Assembly.GetEntryAssembly().GetName().Name + " Start - Start Service");
            sb.AppendLine(Assembly.GetEntryAssembly().GetName().Name + " Stop - Stop Service");

            return sb.ToString();
        }

        /// <summary>
        /// Run installer process
        /// </summary>
        /// <param name="args"></param>
        public static void Go(string[] args)
        {
            try
            {
                string cmd = null;

                if ((args != null) && (args.Length > 0))
                    cmd = args[0].ToUpper();

                if (cmd == "Install".ToUpper())
                    GoInstall();
                else if (cmd == "Uninstall".ToUpper())
                    GoUninstall();
                else if (cmd == "Start".ToUpper())
                    GoStart();
                else if (cmd == "Stop".ToUpper())
                    GoStop();
                else
                    Console.WriteLine(GetUsage());

            }
            catch (Exception ex) { Console.WriteLine(ex); }
            Console.ReadLine();
        }

        /// <summary>
        /// Run installer
        /// </summary>
        public static void GoInstall()
        {
            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetEntryAssembly().Location });
        }

        /// <summary>
        /// Run uninstaller
        /// </summary>
        public static void GoUninstall()
        {
            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetEntryAssembly().Location });
        }

        /// <summary>
        /// Start Service
        /// </summary>
        public static void GoStart()
        {
            string ServiceName = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;

            using (ServiceController sc = new ServiceController(ServiceName))
            {
                sc.Start();
            }

            Console.WriteLine("Start Ok!");
        }

        /// <summary>
        /// Stop Service
        /// </summary>
        public static void GoStop()
        {
            string ServiceName = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;

            using (ServiceController sc = new ServiceController(ServiceName))
            {
                sc.Stop();
            }

            Console.WriteLine("Stop Ok!");
        }

        #endregion Static

        #region Event handlers

        private void ServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController(this._serviceInstaller.ServiceName))
            {
                sc.Start();
            }
        }

        #endregion Event handlers
    }
}