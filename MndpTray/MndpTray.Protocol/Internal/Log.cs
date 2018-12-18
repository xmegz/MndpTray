using System;

namespace MndpTray.Protocol
{
    /// <summary>
    /// Mikrotik discovery message lib log and debug provider
    /// </summary>
    internal static class Log
    {
        #region Fields

        private static Action<string, object[]> _infoAction = null;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Set Debug Format string style delegate
        /// </summary>
        /// <param name="infoAction"></param>
        internal static void SetInfoAction(Action<string, object[]> infoAction)
        {
            _infoAction = infoAction;
        }

        internal static void Exception(string className, string methodName, Exception ex)
        {
            try
            {
                _infoAction?.Invoke("{0}, {1} Exception:\r\n{2}", new object[] { className, methodName, ex.ToString() });
            }
            catch { }
        }

        internal static void Info(string format, params object[] args)
        {
            try
            {
                _infoAction?.Invoke(format, args);
            }
            catch { }
        }

        #endregion Methods
    }
}