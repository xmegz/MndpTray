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

        #region Fields

        private Thread _sendHostInfoThread;
        private bool _sendHostInfoIsRunning;
        private bool _sendHostInfoNow;

        #endregion Fields

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
                MndpLog.Exception(nameof(MndpSender), nameof(Send), ex);
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
                var t = new Thread(_sendHostInfoWork);
                this._sendHostInfoIsRunning = true;
                t.Start();
                this._sendHostInfoThread = t;
            }
            catch (Exception ex)
            {
                MndpLog.Exception(nameof(MndpSender), nameof(Start), ex);
            }

            return false;
        }

        public bool Stop()
        {
            try
            {
                if (this._sendHostInfoThread != null)
                {
                    this._sendHostInfoIsRunning = false;

                    if (this._sendHostInfoThread.IsAlive == true)
                    {
                        this._sendHostInfoIsRunning = false;
                        this._sendHostInfoThread.Join(1000);
                    }

                    if (this._sendHostInfoThread.IsAlive == true)
                    {
                        this._sendHostInfoThread.Interrupt();
                        this._sendHostInfoThread.Join(1000);
                    }

                    if (this._sendHostInfoThread.IsAlive == true)
                    {
                        this._sendHostInfoThread.Abort();
                    }

                    this._sendHostInfoThread = null;
                }
            }
            catch (Exception ex)
            {
                MndpLog.Exception(nameof(MndpSender), nameof(Stop), ex);
            }

            return false;
        }

        private void _sendHostInfoWork()
        {
            try
            {
                ulong sequence = 0;
                DateTime nextSend = DateTime.Now;

                MndpMessageEx msg = new MndpMessageEx();
                MndpHostInfo info = MndpHostInfo.Instance;

                msg.BoardName = info.BoardName;
                msg.Identity = info.Identity;
                msg.Platform = info.Platform;
                msg.SoftwareId = info.SoftwareId;
                msg.Version = info.Version;
                msg.Ttl = 0;
                msg.Type = 0;
                msg.Unpack = 0;

                while (this._sendHostInfoIsRunning)
                {
                    Thread.Sleep(100);

                    if ((nextSend < DateTime.Now) || (this._sendHostInfoNow))
                    {
                        nextSend = DateTime.Now.AddSeconds(HOST_INFO_SEND_INTERVAL);
                        this._sendHostInfoNow = false;

                        var interfaces = info.InterfaceInfos;

                        msg.Sequence = (ushort)(sequence++);
                        msg.Uptime = info.UpTime;

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
                MndpLog.Exception(nameof(MndpSender), nameof(_sendHostInfoWork), ex);
            }
        }

        #endregion Methods
    }
}