/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Protocol
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Mikrotik discovery message listener.
    /// </summary>
    public class MndpListener
    {
        #region Consts

        private const int MESSAGE_KEEP_TIME = 240;
        private const int UDP_PORT = 5678;
        private static readonly IPAddress IP_ADDRESS = IPAddress.Any;

        #endregion Consts

        #region Static

        static MndpListener()
        {
            Instance = new MndpListener();
        }

        /// <summary>
        /// Gets singleton instance.
        /// </summary>
        public static MndpListener Instance { get; }

        #endregion Static

        #region Fields

        private readonly ConcurrentDictionary<string, MndpMessageEx> _dictMessages = new ConcurrentDictionary<string, MndpMessageEx>();
        private UdpClient _udpClient;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Get received message dictionary (Key = Sender mac address).
        /// </summary>
        /// <returns>Is Sucess? .</returns>
        public Dictionary<string, MndpMessageEx> GetMessages()
        {
            var ret = new Dictionary<string, MndpMessageEx>();

            try
            {
                this.ClearOldMessages();

                foreach (var i in this._dictMessages)
                {
                    ret.Add(i.Key, (MndpMessageEx)i.Value.Clone());
                }
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(MndpListener), nameof(this.Start), ex);
            }

            return ret;
        }

        /// <summary>
        /// Start listening process.
        /// </summary>
        /// <returns>Is success? .</returns>
        public bool Start()
        {
            if (this._udpClient == null)
            {
                this._dictMessages.Clear();

                var client = new UdpClient();

                try
                {
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    client.ExclusiveAddressUse = false;
                    client.Client.Bind(new IPEndPoint(IP_ADDRESS, UDP_PORT));
                    client.BeginReceive(new AsyncCallback(this.Receive), null);

                    this._udpClient = client;

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Exception(nameof(MndpListener), nameof(this.Start), ex);

                    try
                    {
                        client.Close();
                    }
                    catch
                    {
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Stop listening process.
        /// </summary>
        /// <returns>Is sucess? .</returns>
        public bool Stop()
        {
            try
            {
                if (this._udpClient != null)
                {
                    this._dictMessages.Clear();

                    this._udpClient.Close();
                    this._udpClient = null;

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(MndpListener), nameof(this.Stop), ex);
            }

            return false;
        }

        private void ClearOldMessages()
        {
            DateTime limit = DateTime.Now.AddSeconds(-MESSAGE_KEEP_TIME);

            List<string> listRemove = new List<string>();

            foreach (var i in this._dictMessages)
            {
                if (i.Value.ReceiveDateTime < limit)
                {
                    listRemove.Add(i.Key);
                }
            }

            foreach (var i in listRemove)
            {
                this._dictMessages.TryRemove(i, out _);
            }
        }

        private void Receive(IAsyncResult ar)
        {
            try
            {
                if ((ar != null) && (this._udpClient != null))
                {
                    IPEndPoint ip = new IPEndPoint(IP_ADDRESS, UDP_PORT);
                    byte[] bytes = this._udpClient.EndReceive(ar, ref ip);

                    var msg = new MndpMessageEx
                    {
                        UnicastAddress = ip.Address.ToString(),
                        ReceiveDateTime = DateTime.Now,
                    };

                    if (msg.Read(bytes))
                    {
                        this._dictMessages[msg.MacAddress] = msg;
                        this.RaiseOnDeviceDiscovered((MndpMessageEx)msg.Clone());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(MndpListener), nameof(this.Receive), ex);
            }

            try
            {
                this._udpClient?.BeginReceive(this.Receive, new object());
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(MndpListener), nameof(this.Receive), ex);
            }
        }
        #endregion Methods

        #region Events
        /// <summary>
        /// New device discovered event
        /// </summary>
        public class DeviceDiscoveredEventArgs
        {
            /// <summary>
            /// Constuct
            /// </summary>
            /// <param name="message">New device discovered event</param>
            public DeviceDiscoveredEventArgs(MndpMessageEx message)
            {
                this.Message = message;
            }

            /// <summary>
            /// Discovery message
            /// </summary>
            public MndpMessageEx Message { get; private set; }
        }

        /// <summary>
        /// New device discovery event
        /// </summary>
        public event EventHandler<DeviceDiscoveredEventArgs> OnDeviceDiscovered;

        private void RaiseOnDeviceDiscovered(MndpMessageEx message)
        {
            OnDeviceDiscovered?.Invoke(this, new DeviceDiscoveredEventArgs(message));
        }
        #endregion
    }
}