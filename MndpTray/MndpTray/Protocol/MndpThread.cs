using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace MndpTray
{
    public class MndpListener
    {
        #region Static

        static MndpListener()
        {
            Instance = new MndpListener();
        }

        public static MndpListener Instance { get; }

        #endregion Static

        #region Constants

        private const int MESSAGE_KEEP_TIME = 240;
        private const int PORT = 5678;
        private static readonly IPAddress IPADDR = IPAddress.Any;

        #endregion Constants

        #region Variables

        private ConcurrentDictionary<string, MndpMessageEx> _dictMessages = new ConcurrentDictionary<string, MndpMessageEx>();
        private UdpClient _udpClient;

        #endregion Variables

        public Dictionary<string, MndpMessageEx> GetMessages()
        {
            var ret = new Dictionary<string, MndpMessageEx>();

            this._clearOldMessages();

            foreach (var i in this._dictMessages)
            {
                ret.Add(i.Key, (MndpMessageEx)i.Value.Clone());
            }

            return ret;
        }

        public bool Send(MndpMessageEx msg, IPAddress broadcastAddress = null)
        {
            try
            {
                EndPoint ep;
                Socket s;

                if (broadcastAddress == null)
                    broadcastAddress = IPAddress.Broadcast;

                using (s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);

                    ep = new IPEndPoint(broadcastAddress, PORT);

                    var data = msg.Write();

                    s.SendTo(data, ep);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug("{0} Send, Exception: {1}", nameof(MndpListener), ex.ToString());
            }

            return false;
        }

        public void Start()
        {
            this._udpClient = new UdpClient();
            this._udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this._udpClient.ExclusiveAddressUse = false;

            this._udpClient.Client.Bind(new IPEndPoint(IPADDR, PORT));
            this._udpClient.BeginReceive(new AsyncCallback(this._receive), null);
        }

        public void Stop()
        {
            try
            {
                if (this._udpClient != null)
                    this._udpClient.Close();

                this._dictMessages.Clear();
            }
            catch (Exception ex)
            {
                Debug("{0} Stop, Exception: {1}", nameof(MndpListener), ex.ToString());
            }
        }

        private static void Debug(string format, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(format + "\r\n", args);
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
                if (ar != null)
                {
                    IPEndPoint ip = new IPEndPoint(IPADDR, PORT);
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
                Debug("{0} _receive, Exception: {1}", nameof(MndpListener), ex.ToString());
            }

            this._udpClient.BeginReceive(_receive, new object());
        }
    }
}