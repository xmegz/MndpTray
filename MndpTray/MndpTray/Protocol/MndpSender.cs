using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MndpTray.Protocol
{
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

        private const int HOST_INFO_SEND_INTERVAL = 60;
        private const int UDP_PORT = 5678;
        private static readonly IPAddress IP_ADDRESS = IPAddress.Broadcast;

        #endregion Const

        #region Variables

        private Thread _hostInfoSendThread;
        private bool _sendHostInfoNow;

        #endregion Variables

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
                Debug.Exception(nameof(MndpSender), nameof(Send), ex);
            }

            return false;
        }

        public void SendHostInfoNow()
        {
            this._sendHostInfoNow = true;
        }

        public bool Start()
        {
            try
            {
                var t = new Thread(_hostInfoSendMethod);
                t.Start();
                this._hostInfoSendThread = t;
            }
            catch (Exception ex)
            {
                Debug.Exception(nameof(MndpSender), nameof(Start), ex);
            }

            return false;
        }

        public bool Stop()
        {
            try
            {
                if (this._hostInfoSendThread != null)
                {
                    this._hostInfoSendThread.Abort();
                    this._hostInfoSendThread = null;
                }
            }
            catch (Exception ex)
            {
                Debug.Exception(nameof(MndpSender), nameof(Stop), ex);
            }

            return false;
        }

        private void _hostInfoSendMethod()
        {
            try
            {
                ulong sequence = 0;
                DateTime nextSend = DateTime.Now;

                MndpMessageEx msg = new MndpMessageEx();

                msg.BoardName = HostInfo.GetBoardName();
                msg.Identity = HostInfo.GetIdentity();
                msg.Platform = HostInfo.GetPlatform();
                msg.SoftwareId = HostInfo.GetSoftwareId();
                msg.Version = HostInfo.GetVersion();
                msg.Ttl = 0;
                msg.Type = 0;
                msg.Unpack = 0;

                while (true)
                {
                    Thread.Sleep(100);

                    if ((nextSend < DateTime.Now) || (this._sendHostInfoNow))
                    {
                        nextSend = DateTime.Now.AddSeconds(HOST_INFO_SEND_INTERVAL);
                        this._sendHostInfoNow = false;

                        var interfaces = InterfaceInfo.GetInfos();

                        msg.Sequence = (ushort)(sequence++);
                        msg.Uptime = HostInfo.GetUpTime();

                        foreach (var i in interfaces)
                        {
                            msg.BroadcastAddress = i.BroadcastAddress;
                            msg.InterfaceName = i.InterfaceName;
                            msg.MacAddress = i.MacAddress;
                            msg.SenderAddress = i.SenderAddress;

                            this.Send((MndpMessageEx)msg.Clone());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Exception(nameof(MndpSender), nameof(_hostInfoSendMethod), ex);
            }
        }

        #endregion Methods
    }
}