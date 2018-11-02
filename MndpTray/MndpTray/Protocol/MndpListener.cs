using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace MndpTray.Protocol
{
    using static MndpDebug;

    /// <summary>
    /// Mikrotik discovery message listener
    /// </summary>
    public class MndpListener
    {
        #region Static

        static MndpListener()
        {
            Instance = new MndpListener();
        }

        public static MndpListener Instance { get; }

        #endregion Static

        #region Consts

        private const int MESSAGE_KEEP_TIME = 240;
        private const int UDP_PORT = 5678;
        private static readonly IPAddress IP_ADDRESS = IPAddress.Any;

        #endregion Consts

        #region Fields

        private ConcurrentDictionary<string, MndpMessageEx> _dictMessages = new ConcurrentDictionary<string, MndpMessageEx>();
        private UdpClient _udpClient;

        #endregion Fields

        #region Methods
        public Dictionary<string, MndpMessageEx> GetMessages()
        {
            var ret = new Dictionary<string, MndpMessageEx>();

            try
            {
                this._clearOldMessages();

                foreach (var i in this._dictMessages)
                {
                    ret.Add(i.Key, (MndpMessageEx)i.Value.Clone());
                }
            }
            catch (Exception ex)
            {
                DebugException(nameof(MndpListener), nameof(Start), ex);
            }

            return ret;
        }

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
                    client.BeginReceive(new AsyncCallback(this._receive), null);

                    this._udpClient = client;

                    return true;
                }
                catch (Exception ex)
                {
                    DebugException(nameof(MndpListener), nameof(Start), ex);

                    try
                    {
                        client.Close();
                    }
                    catch { }
                }
            }

            return false;
        }

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
                DebugException(nameof(MndpListener), nameof(Stop), ex);
            }

            return false;
        }

        private void _clearOldMessages()
        {
            DateTime limit = DateTime.Now.AddSeconds(-MESSAGE_KEEP_TIME);

            List<string> listRemove = new List<string>();

            foreach (var i in this._dictMessages)
            {
                if (i.Value.ReceiveDateTime < limit)
                    listRemove.Add(i.Key);
            }

            foreach (var i in listRemove)
            {
                this._dictMessages.TryRemove(i, out MndpMessageEx val);
            }
        }

        private void _receive(IAsyncResult ar)
        {
            try
            {
                if ((ar != null) && (this._udpClient != null))
                {
                    IPEndPoint ip = new IPEndPoint(IP_ADDRESS, UDP_PORT);
                    byte[] bytes = this._udpClient.EndReceive(ar, ref ip);

                    var msg = new MndpMessageEx();
                    msg.SenderAddress = ip.Address.ToString();
                    msg.ReceiveDateTime = DateTime.Now;

                    if (msg.Read(bytes))
                    {
                        this._dictMessages[msg.MacAddress] = msg;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugException(nameof(MndpListener), nameof(_receive), ex);
            }

            try
            {
                if (this._udpClient != null)
                    this._udpClient.BeginReceive(_receive, new object());
            }
            catch (Exception ex)
            {
                DebugException(nameof(MndpListener), nameof(_receive), ex);
            }
        }
        #endregion
    }
}