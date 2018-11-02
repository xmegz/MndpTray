using System;
using System.IO;
using System.Reflection;

namespace MndpTray.Protocol
{
    public static class MndpDebug
    {
        #region Variables
        private static string LOG_FILE_NAME  = Assembly.GetEntryAssembly().GetLocationWithExtension("txt");
        private static bool   LOG_IS_ENABLED = File.Exists(LOG_FILE_NAME);
        private static object LOG_FILE_LOCK  = new object();
        #endregion

        private static void File_WriteLine(string str)
        {
            if (!LOG_IS_ENABLED) return;

            str = string.Concat("<", DateTime.Now.ToString(), "> ", str, "\r\n");

            lock (LOG_FILE_LOCK)
            {
                File.AppendAllText(LOG_FILE_NAME, str);
            }
        }

        public static void Debug(string format, params object[] args)
        {
            try
            {
                String str = String.Format(format, args);

                System.Diagnostics.Debug.WriteLine(str);
                File_WriteLine(str);
            }
            catch { }
        }

        public static void DebugException(string className, string methodName, Exception ex)
        {
            try
            {
                Debug("{0}, {1} Exception:\r\n {2}", className, methodName, ex.ToString());
            }
            catch { }
        }
    }
}