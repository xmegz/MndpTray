using System;
using System.IO;
using System.Reflection;

namespace MndpTray.Protocol
{
    public static class Debug
    {
        #region Variables

        private static object LOG_FILE_LOCK = new object();
        private static string LOG_FILE_NAME = Assembly.GetEntryAssembly().GetLocationWithExtension("txt");
        private static bool LOG_IS_ENABLED = File.Exists(LOG_FILE_NAME);

        #endregion Variables

        public static void Exception(string className, string methodName, Exception ex)
        {
            try
            {
                Info("{0}, {1} Exception:\r\n{2}", className, methodName, ex.ToString());
            }
            catch { }
        }

        public static void Info(string format, params object[] args)
        {
            try
            {
                String str = String.Format(format, args);

                System.Diagnostics.Debug.WriteLine(str);
                _writeToFile(str);
            }
            catch { }
        }

        private static void _writeToFile(string str)
        {
            if (!LOG_IS_ENABLED) return;

            str = string.Concat("<", DateTime.Now.ToString(), "> ", str, "\r\n");

            lock (LOG_FILE_LOCK)
            {
                File.AppendAllText(LOG_FILE_NAME, str);
            }
        }
    }
}