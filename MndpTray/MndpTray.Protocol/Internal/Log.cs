namespace MndpTray.Protocol
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

        #region Methods

        /// <summary>
        /// Set Debug Format string style delegate.
        /// </summary>
        /// <param name="infoAction"></param>
        public static void SetInfoAction(Action<string, object[]> infoAction)
        {
            _infoAction = infoAction;
        }

        /// <summary>
        /// Log Exception
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="ex"></param>
        public static void Exception(string className, string methodName, Exception ex)
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
        /// Log Info
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Info(string format, params object[] args)
        {
            try
            {
                _infoAction?.Invoke(format, args);
            }
            catch
            {
            }
        }

        #endregion Methods
    }
}