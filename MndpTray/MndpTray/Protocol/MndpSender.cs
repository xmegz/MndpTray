using System;
using System.Net;
using System.Net.Sockets;

namespace MndpTray.Protocol
{    
    using static MndpDebug;

    /// <summary>
    /// Mikrotik discovery message sender
    /// </summary>
    public class MndpSender
    {
        #region Static

        static MndpSender()
        {
            Instance = new MndpSender();
        }

        public static MndpSender Instance { get; }

        #endregion Static

        #region Const

        private const int UDP_PORT = 5678;
        private static readonly IPAddress IP_ADDRESS = IPAddress.Broadcast;

        #endregion Const

        #region Methods
        public bool Send(MndpMessageEx msg)
        {
            try
            {
                EndPoint ep;
                Socket s;

                var broadcastAddress = IP_ADDRESS;

                if (msg.BroadcastAddress != null)
                    broadcastAddress = IPAddress.Parse(msg.BroadcastAddress);

                using (s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);

                    ep = new IPEndPoint(broadcastAddress, UDP_PORT);

                    var data = msg.Write();

                    s.SendTo(data, ep);

                    return true;
                }
            }
            catch (Exception ex)
            {
                DebugException(nameof(MndpSender), nameof(Send), ex);
            }

            return false;
        }

        #endregion
    }
}