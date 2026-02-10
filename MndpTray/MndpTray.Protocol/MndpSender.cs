/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Protocol
{
    using MndpTray.Protocol.Internal;
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

        #endregion Const

        #region Fields

        private Thread _sendHostInfoThread;
        private IMndpHostInfo _hostInfo;

        private volatile bool _sendHostInfoThreadIsRunning;
        private volatile bool _sendHostInfoNow;

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
                IPAddress broadcastAddress = IPAddress.Broadcast;

                if (msg.BroadcastAddress != null)
                {
                    if (IPAddress.TryParse(msg.BroadcastAddress, out IPAddress parsedBroadcastAddress) == true)
                        broadcastAddress = parsedBroadcastAddress;
                }

                using (var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);

                    var ep = new IPEndPoint(broadcastAddress, UDP_PORT);
                    byte[] data = msg.Write();
                    int sent = s.SendTo(data, ep);

                    return sent == data.Length;
                }
            }
            catch (Exception ex)
            {
                if (Log.IsEnabled)
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
                lock (this)
                {
                    if (hostInfo == null)
                        return false;

                    if (this._sendHostInfoThread == null)
                    {
                        Thread thread = new Thread(this.SendHostInfoWork);

                        this._sendHostInfoThreadIsRunning = true;
                        this._hostInfo = hostInfo;

                        thread.Start();

                        this._sendHostInfoThread = thread;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (Log.IsEnabled)
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
                lock (this)
                {
                    Thread thread = this._sendHostInfoThread;

                    if (thread == null)
                        return true;

                    this._sendHostInfoThreadIsRunning = false;


                    if (thread.IsAlive)
                        thread.Join(1000);

                    if (thread.IsAlive)
                    {
                        thread.Interrupt();
                        thread.Join(1000);
                    }

                    if (thread.IsAlive)
                        thread.Abort();

                    this._sendHostInfoThread = null;
                    this._hostInfo = null;
                }

                return true;

            }
            catch (Exception ex)
            {
                if (Log.IsEnabled)
                    Log.Exception(nameof(MndpSender), nameof(this.Stop), ex);
            }

            return false;
        }

        private void SendHostInfoWork()
        {
            try
            {
                ulong sequence = 0;
                DateTime nextSendDateTime = DateTime.UtcNow;

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

                while (this._sendHostInfoThreadIsRunning)
                {
                    Thread.Sleep(250);

                    if ((nextSendDateTime < DateTime.UtcNow) || this._sendHostInfoNow)
                    {
                        nextSendDateTime = DateTime.UtcNow.AddSeconds(HOST_INFO_SEND_INTERVAL);
                        this._sendHostInfoNow = false;

                        try
                        {
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
                        catch (Exception ex)
                        {
                            if (Log.IsEnabled)
                                Log.Exception(nameof(MndpSender), nameof(this.SendHostInfoWork), ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Log.IsEnabled)
                    Log.Exception(nameof(MndpSender), nameof(this.SendHostInfoWork), ex);
            }
        }

        #endregion Methods
    }
}