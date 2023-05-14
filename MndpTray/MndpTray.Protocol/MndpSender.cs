namespace MndpTray.Protocol
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// Mikrotik discovery message sender.
    /// </summary>
    public class MndpSender
    {
        #region Static

        static MndpSender()
        {
            Instance = new MndpSender();
        }

        /// <summary>
        /// Gets sigleton instance.
        /// </summary>
        public static MndpSender Instance { get; }

        #endregion Static

        #region Const

        private const int HOST_INFO_SEND_INTERVAL = 30;
        private const int UDP_PORT = 5678;
        private static readonly IPAddress IP_ADDRESS = IPAddress.Broadcast;

        #endregion Const

        #region Fields

        private Thread _sendHostInfoThread;
        private IMndpHostInfo _hostInfo;
        private bool _sendHostInfoIsRunning;
        private bool _sendHostInfoNow;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Send message.
        /// </summary>
        /// <param name="msg">Mndp message.</param>
        /// <returns>Is success?.</returns>
        public bool Send(MndpMessageEx msg)
        {
            try
            {
                EndPoint ep;
                Socket s;

                IPAddress broadcastAddress = IP_ADDRESS;

                if (msg.BroadcastAddress != null)
                {
                    broadcastAddress = IPAddress.Parse(msg.BroadcastAddress);
                }

                using (s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);

                    ep = new IPEndPoint(broadcastAddress, UDP_PORT);

                    byte[] data = msg.Write();

                    s.SendTo(data, ep);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(MndpSender), nameof(this.Send), ex);
            }

            return false;
        }

        /// <summary>
        /// Sender thread notification to send immediately.
        /// </summary>
        public void SendHostInfoNow()
        {
            this._sendHostInfoNow = true;
        }

        /// <summary>
        /// Start sending process.
        /// </summary>
        /// <param name="hostInfo">Current host information.</param>
        /// <returns>Is success?.</returns>
        public bool Start(IMndpHostInfo hostInfo)
        {
            try
            {
                Thread t = new Thread(this.SendHostInfoWork);
                this._sendHostInfoIsRunning = true;
                t.Start();
                this._sendHostInfoThread = t;
                this._hostInfo = hostInfo;
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(MndpSender), nameof(this.Start), ex);
            }

            return false;
        }

        /// <summary>
        /// Stop sending process.
        /// </summary>
        /// <returns>Is success?.</returns>
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
                    this._hostInfo = null;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(MndpSender), nameof(this.Stop), ex);
            }

            return false;
        }

        private void SendHostInfoWork()
        {
            try
            {
                ulong sequence = 0;
                DateTime nextSendDateTime = DateTime.Now;

                MndpMessageEx msg = new MndpMessageEx
                {
                    BoardName = this._hostInfo.BoardName,
                    Identity = this._hostInfo.Identity,
                    Platform = this._hostInfo.Platform,
                    SoftwareId = this._hostInfo.SoftwareId,
                    Version = this._hostInfo.Version,
                    Ttl = 0,
                    Type = 0,
                    Unpack = 0,
                };

                while (this._sendHostInfoIsRunning)
                {
                    Thread.Sleep(100);

                    if ((nextSendDateTime < DateTime.Now) || this._sendHostInfoNow)
                    {
                        nextSendDateTime = DateTime.Now.AddSeconds(HOST_INFO_SEND_INTERVAL);
                        this._sendHostInfoNow = false;

                        System.Collections.Generic.List<IMndpInterfaceInfo> interfaces = this._hostInfo.InterfaceInfos;

                        msg.Sequence = (ushort)(sequence++);
                        msg.Uptime = this._hostInfo.UpTime;

                        foreach (IMndpInterfaceInfo i in interfaces)
                        {
                            msg.BroadcastAddress = i.BroadcastAddress;
                            msg.InterfaceName = i.InterfaceName;
                            msg.MacAddress = i.MacAddress;
                            msg.UnicastAddress = i.UnicastAddress;
                            msg.UnicastIPv6Address = i.UnicastIPv6Address;

                            this.Send((MndpMessageEx)msg.Clone());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(MndpSender), nameof(this.SendHostInfoWork), ex);
            }
        }

        #endregion Methods
    }
}