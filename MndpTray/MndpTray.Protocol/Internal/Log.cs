/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Protocol.Internal
{
    using System;

    /// <summary>
    /// Mikrotik discovery message lib log and debug provider.
    /// </summary>
    public static class Log
    {
        #region Fields

        private static Action<string, object[]> _infoAction;

        #endregion Fields

        #region Properies

        internal static bool IsEnabled { get { return _infoAction != null; } }

        #endregion

        #region Methods

        /// <summary>
        /// Set Debug Format string style delegate.
        /// </summary>
        /// <param name="infoAction">Logging action</param>
        public static void SetInfoAction(Action<string, object[]> infoAction)
        {
            _infoAction = infoAction;
        }

        /// <summary>
        /// Log Exception
        /// </summary>
        /// <param name="className">Called class</param>
        /// <param name="methodName">Called method</param>
        /// <param name="ex">Exception</param>
        internal static void Exception(string className, string methodName, Exception ex)
        {
            try
            {
                _infoAction?.Invoke("{0}, {1} Exception:\r\n{2}", new object[] { className, methodName, ex.ToString() });
            }
            catch
            {
            }
        }

        /// <summary>
        /// Log information
        /// </summary>
        /// <param name="message">Message format</param>
        /// <param name="args">format args</param>
        internal static void Info(string message, params object[] args)
        {
            try
            {
                _infoAction?.Invoke(message, args);
            }
            catch
            {
            }
        }
     
        #endregion Methods
    }
}