using MndpTray.Protocol;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading;

namespace MndpService.Core
{
    class Program
    {

        static void Main()
        {
            AssemblyLoadContext.Default.Unloading += Default_Unloading;
            Console.CancelKeyPress += Console_CancelKeyPress;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Start();

            while (true)
            {
                Log("Running...");
                Thread.Sleep(2000);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log("UnhandledException...");
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Log("CancelKeyPress...");
            Stop();
        }

        private static void Default_Unloading(AssemblyLoadContext obj)
        {
            Log("Unloading...");
            Stop();
        }

        private static void Start()
        {
            Log("Starting...");
            MndpSender.Instance.Start(MndpHostInfo.Instance);
        }

        private static void Stop()
        {
            Log("Stopping...");
            MndpSender.Instance.Stop();
        }

        private static readonly bool LOG_FILE_IS_ENABLED = File.Exists(GetLogFileName("log"));
        private static readonly object LOG_FILE_LOCK = new object();
        private static readonly string LOG_FILE_NAME = GetLogFileName("log");

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
              
                str = string.Concat("<", DateTime.Now.ToString(), "> ", str, Environment.NewLine);

                Console.Write(str);

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




    }
}
