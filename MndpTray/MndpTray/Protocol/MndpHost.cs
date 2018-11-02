using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;

namespace MndpTray.Protocol
{
    using static MndpDebug;

    public class MndpHost
    {
        #region Static

        static MndpHost()
        {
            Instance = new MndpHost();
        }

        public static MndpHost Instance { get; }

        #endregion Static

        #region Utils

        private static string _getSoftwareId(string from = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", string valueName = "DigitalProductId")
        {
            string possible_chars = "BCDFGHJKMPQRTVWXY2346789";

            RegistryKey hive = null;
            RegistryKey key = null;
            try
            {
                var result = string.Empty;
                hive = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, Environment.MachineName);
                key = hive.OpenSubKey(from, false);
                var k = RegistryValueKind.Unknown;
                try { k = key.GetValueKind(valueName); }
                catch (Exception) { }

                if (k == RegistryValueKind.Unknown)
                {
                    key.Close();
                    hive.Close();
                    hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    key = hive.OpenSubKey(from, false);
                    try { k = key.GetValueKind(valueName); }
                    catch (Exception) { }
                }

                if (k == RegistryValueKind.Binary)
                {
                    var pivot = 0;
                    var bytes = (byte[])key.GetValue(valueName);
                    var ints = new int[16];
                    for (var i = 52; i < 67; ++i) ints[i - 52] = bytes[i];
                    for (var i = 0; i < 25; ++i)
                    {
                        pivot = 0;
                        for (var j = 14; j >= 0; --j)
                        {
                            pivot <<= 8;
                            pivot ^= ints[j];
                            ints[j] = ((int)Math.Truncate(pivot / 24.0));
                            pivot %= 24;
                        }
                        result = possible_chars[pivot] + result;
                        if ((i % 5 == 4) && (i != 24))
                        {
                            result = "-" + result;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                DebugException(nameof(MndpHost), nameof(_getSoftwareId), ex);
                return null;
            }
            finally
            {
                key?.Close();
                hive?.Close();
            }
        }

        private static string _getPlatform()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                if (moc.Count != 0)
                {
                    foreach (ManagementObject mo in mc.GetInstances())
                    {
                        return mo["Manufacturer"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                DebugException(nameof(MndpHost), nameof(_getPlatform), ex);
            }

            return String.Empty;
        }

        private static string _getIdentity()
        {
            try
            {
                return System.Net.Dns.GetHostEntry("").HostName;
            }
            catch (Exception ex)
            {
                DebugException(nameof(MndpHost), nameof(_getIdentity), ex);
            }

            return Environment.MachineName;
        }

        private static List<MndpInterface> _getMndpInterfaces()
        {
            var ret = new List<MndpInterface>();

            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                                          .Where(a => a.OperationalStatus == OperationalStatus.Up && a.NetworkInterfaceType != NetworkInterfaceType.Loopback && a.GetPhysicalAddress().ToString().Length >= 12)
                                          .ToList();

                foreach (var @interface in interfaces)
                    foreach (var unicastAddress in @interface.GetIPProperties().UnicastAddresses)
                    {
                        if (unicastAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && unicastAddress.IPv4Mask != null)
                        {
                            var addressInt = BitConverter.ToInt32(unicastAddress.Address.GetAddressBytes(), 0);
                            var maskInt = BitConverter.ToInt32(unicastAddress.IPv4Mask.GetAddressBytes(), 0);
                            var broadcastInt = addressInt | ~maskInt;
                            var broadcast = new IPAddress(BitConverter.GetBytes(broadcastInt));

                            ret.Add(new MndpInterface(unicastAddress.Address.ToString(), @interface.Name, @interface.GetPhysicalAddress().ToString(), broadcast.ToString()));
                        }
                    }
            }
            catch (Exception ex)
            {
                DebugException(nameof(MndpHost), nameof(_getMndpInterfaces), ex);
            }

            return ret;
        }

        private static string _getBoardName()
        {
            return "x86";
        }

        private static TimeSpan _getUpTime()
        {
            return TimeSpan.FromMilliseconds(GetTickCount64());
        }

        private static string _getVersion()
        {
            try
            {
                return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
            }
            catch (Exception ex)
            {
                DebugException(nameof(MndpHost), nameof(_getVersion), ex);
            }

            return String.Empty;
        }

        [DllImport("kernel32")]
        private static extern UInt64 GetTickCount64();

        #endregion Utils

        #region Consts

        private const uint SEND_INTERVAL = 60;

        #endregion Consts

        #region Variables

        private bool _sendNow;
        private Thread _sendThread;

        #endregion Variables

        #region Methods

        public void SendNow()
        {
            this._sendNow = true;
        }

        public bool Start()
        {
            try
            {
                var t = new Thread(_sendMethod);
                t.Start();
                this._sendThread = t;
            }
            catch (Exception ex)
            {
                DebugException(nameof(MndpHost), nameof(Start), ex);
            }

            return false;
        }

        public bool Stop()
        {
            try
            {
                if (this._sendThread != null)
                {
                    this._sendThread.Abort();
                    this._sendThread = null;
                }
            }
            catch (Exception ex)
            {
                DebugException(nameof(MndpHost), nameof(Stop), ex);
            }

            return false;
        }

        private void _sendMethod()
        {
            try
            {
                ulong sequence = 0;
                DateTime nextSend = DateTime.Now;

                MndpMessageEx msg = new MndpMessageEx();

                msg.BoardName = _getBoardName();
                msg.Identity = _getIdentity();
                msg.Platform = _getPlatform();
                msg.SoftwareId = _getSoftwareId();
                msg.Version = _getVersion();
                msg.Ttl = 0;
                msg.Type = 0;
                msg.Unpack = 0;

                while (true)
                {
                    Thread.Sleep(100);

                    if ((nextSend < DateTime.Now) || (this._sendNow))
                    {
                        nextSend = DateTime.Now.AddSeconds(SEND_INTERVAL);
                        this._sendNow = false;

                        var interfaces = _getMndpInterfaces();

                        msg.Sequence = (ushort)(sequence++);
                        msg.Uptime = _getUpTime();

                        foreach (var i in interfaces)
                        {
                            msg.BroadcastAddress = i.BroadcastAddress;
                            msg.InterfaceName = i.InterfaceName;
                            msg.MacAddress = i.MacAddress;
                            msg.SenderAddress = i.SenderAddress;

                            MndpSender.Instance.Send((MndpMessageEx)msg.Clone());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugException(nameof(MndpHost), nameof(_sendMethod), ex);
            }
        }

        #endregion Methods

        protected class MndpInterface
        {
            #region Props

            public String BroadcastAddress { get; }
            public String InterfaceName { get; }
            public String MacAddress { get; }
            public String SenderAddress { get; }

            #endregion Props

            #region Methods

            public MndpInterface(string address, string interfaceName, string macAddress, string broadcastAddress)
            {
                this.SenderAddress = address;
                this.InterfaceName = interfaceName;
                this.MacAddress = macAddress;
                this.BroadcastAddress = broadcastAddress;
            }

            #endregion Methods
        }
    }
}