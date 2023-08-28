/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Protocol.Test
{
    using System;
    using System.Threading;

    /// <summary>
    /// Startup Class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Startup Method.
        /// </summary>
        public static void Main()
        {
            MndpListener.Instance.Start();
            MndpListener.Instance.OnDeviceDiscovered += Instance_OnDeviceDiscovered;
            MndpSender.Instance.Start(MndpHostInfo.Instance);

            Console.WriteLine("--- Start ---");
            Console.WriteLine("Press any key to stop");

            while (!Console.KeyAvailable)
                Thread.Sleep(100);

            Console.WriteLine("--- Stop ---");

            MndpListener.Instance.Stop();
            MndpSender.Instance.Stop();
        }

        private static void Instance_OnDeviceDiscovered(object sender, MndpListener.DeviceDiscoveredEventArgs e)
        {
            Console.WriteLine(e.Message.ToString());
        }
    }
}