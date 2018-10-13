using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace MndpTray
{
    public static class MndpHost
    {
        #region props
        public static String BoardName { get; }
        public static String Identity { get; }
        public static List<MndpInterface> InterfaceInfo { get { return GetBroadcastAddress(); } }
        public static String Platform { get; }
        public static UInt16 Sequence { get { return GetSequence(); } }       
        public static String SoftwareId { get; }
        public static TimeSpan Uptime { get { return GetUpTime(); } }
        public static String Version { get; }
        #endregion props


        private static UInt64 _sequence = 0;
        private static object _sequenceLock = new object();

        static MndpHost()
        {
            BoardName = GetManufacturerName();
            Identity = Environment.MachineName;
            Platform = (Environment.Is64BitProcess) ? "Amd64" : "X86";
            SoftwareId = Environment.UserName;
            Version = GetReleaseId();
        }

        public static void SendMessage()
        {
            try
            {
                var lInterfaceInfo = MndpHost.InterfaceInfo;
                var sequence = MndpHost.Sequence;

                foreach (var i in lInterfaceInfo)
                {
                    var msg = new MndpMessageEx();

                    msg.BoardName = MndpHost.BoardName;
                    msg.Identity = MndpHost.Identity;
                    msg.InterfaceName = i.InterfaceName;
                    msg.MacAddress = i.MacAddress;
                    msg.Platform = MndpHost.Platform;
                    msg.ReceiveDateTime = DateTime.Now;
                    msg.SenderAddress = i.Address;
                    msg.Sequence = sequence;
                    msg.SoftwareId = MndpHost.SoftwareId;
                    msg.Ttl = 0;
                    msg.Type = 0;
                    msg.Unpack = 0;
                    msg.Uptime = MndpHost.Uptime;
                    msg.Version = MndpHost.Version;

                    MndpListener.Instance.Send(msg);
                }
            }
            catch { }
        }


        private static UInt16 GetSequence()
        {
            lock (_sequenceLock)
            {
                _sequence++;
                return (UInt16)_sequence;
            }
        }

        private static List<MndpInterface> GetBroadcastAddress()
        {
            List<MndpInterface> lmi = new List<MndpInterface>();

            try
            {
                foreach (var i in NetworkInterface.GetAllNetworkInterfaces()
                                                  .Where(a => a.OperationalStatus == OperationalStatus.Up && a.NetworkInterfaceType != NetworkInterfaceType.Loopback && a.GetPhysicalAddress().ToString().Length>=12)                                                  
                                                  .ToList())
                    foreach (var ua in i.GetIPProperties().UnicastAddresses)
                    {
                        if (ua.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ua.IPv4Mask != null)
                        {
                            var addressInt = BitConverter.ToInt32(ua.Address.GetAddressBytes(), 0);
                            var maskInt = BitConverter.ToInt32(ua.IPv4Mask.GetAddressBytes(), 0);
                            var broadcastInt = addressInt | ~maskInt;
                            var broadcast = new IPAddress(BitConverter.GetBytes(broadcastInt));

                            lmi.Add(new MndpInterface(ua.Address.ToString(),i.Name, i.GetPhysicalAddress().ToString(), broadcast.ToString()));
                        }
                    }
            }
            catch { }

            return lmi;
        }

        private static String GetManufacturerName()
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
            catch { }

            return String.Empty;
        }

        private static string GetReleaseId()
        {
            try
            {
                return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
            }
            catch { }

            return String.Empty;
        }

        [DllImport("kernel32")]
        private static extern UInt64 GetTickCount64();

        private static TimeSpan GetUpTime()
        {
            return TimeSpan.FromMilliseconds(GetTickCount64());
        }

        public class MndpInterface
        {
            public MndpInterface(string address,string interfaceName, string macAddress, string broadcastAddress)
            {
                this.Address = address;
                this.InterfaceName = interfaceName;
                this.MacAddress = macAddress;
                this.BroadcastAddress = broadcastAddress;
            }

            #region props

            public String Address { get; }
            public String BroadcastAddress { get; }
            public String InterfaceName { get; }
            public String MacAddress { get; }

            #endregion props
        }      
    }
}