using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace MndpTray.Protocol
{
    /// <summary>
    /// Mikrotik discovery message host information provider
    /// </summary>
    public class MndpHostInfo
    {
        #region Static

        static MndpHostInfo()
        {
            Instance = new MndpHostInfo();
        }

        public static MndpHostInfo Instance { get; }

        [DllImport("kernel32")]
        private static extern UInt64 GetTickCount64();

        #endregion Static

        #region Props

        public string BoardName => "x86";

        public string Identity
        {
            get
            {
                try
                {
                    return Dns.GetHostEntry("").HostName;
                }
                catch (Exception ex)
                {
                    MndpLog.Exception(nameof(MndpHostInfo), nameof(Identity), ex);
                }

                return Environment.MachineName;
            }
        }

        public List<MndpInterfaceInfo> InterfaceInfos
        {
            get
            {
                {
                    var ret = new List<MndpInterfaceInfo>();

                    try
                    {
                        var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                                                  .Where(a => (a.OperationalStatus == OperationalStatus.Up) && (a.NetworkInterfaceType != NetworkInterfaceType.Loopback) && (a.GetPhysicalAddress().ToString().Length >= 12))
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

                                    ret.Add(new MndpInterfaceInfo(broadcast.ToString(), @interface.Name, @interface.GetPhysicalAddress().ToString(), unicastAddress.Address.ToString()));
                                }
                            }
                    }
                    catch (Exception ex)
                    {
                        MndpLog.Exception(nameof(MndpHostInfo), nameof(InterfaceInfos), ex);
                    }

                    return ret;
                }
            }
        }

        public string Platform
        {
            get
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
                    MndpLog.Exception(nameof(MndpHostInfo), nameof(Platform), ex);
                }

                return String.Empty;
            }
        }

        public string SoftwareId
        {
            get
            {
                {
                    string from = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
                    string valueName = "DigitalProductId";
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
                        MndpLog.Exception(nameof(MndpHostInfo), nameof(SoftwareId), ex);
                        return null;
                    }
                    finally
                    {
                        key?.Close();
                        hive?.Close();
                    }
                }
            }
        }

        public TimeSpan UpTime
        {
            get
            {
                return TimeSpan.FromMilliseconds(GetTickCount64());
            }
        }

        public string Version
        {
            get
            {
                try
                {
                    return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
                }
                catch (Exception ex)
                {
                    MndpLog.Exception(nameof(MndpHostInfo), nameof(Version), ex);
                }

                return String.Empty;
            }
        }

        #endregion Props

        /// <summary>
        /// Mikrotik discovery message host interface information
        /// </summary>
        public class MndpInterfaceInfo
        {
            #region Props

            public String BroadcastAddress { get; }
            public String InterfaceName { get; }
            public String MacAddress { get; }
            public String SenderAddress { get; }
            
            #endregion Props

            #region Methods

            public MndpInterfaceInfo(string broadcastAddress, string interfaceName, string macAddress, string senderAddress)
            {
                this.BroadcastAddress = broadcastAddress;
                this.InterfaceName = interfaceName;
                this.MacAddress = macAddress;                
                this.SenderAddress = senderAddress;
            }

            #endregion Methods
        }
    }
}