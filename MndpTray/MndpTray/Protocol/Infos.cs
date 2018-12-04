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
    public static class HostInfo
    {
        #region Static

        public static string GetBoardName()
        {
            return "x86";
        }

        public static string GetIdentity()
        {
            try
            {
                return Dns.GetHostEntry("").HostName;
            }
            catch (Exception ex)
            {
                Debug.Exception(nameof(HostInfo), nameof(GetIdentity), ex);
            }

            return Environment.MachineName;
        }

        public static string GetPlatform()
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
                Debug.Exception(nameof(HostInfo), nameof(GetPlatform), ex);
            }

            return String.Empty;
        }

        public static string GetSoftwareId(string from = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", string valueName = "DigitalProductId")
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
                Debug.Exception(nameof(HostInfo), nameof(GetSoftwareId), ex);
                return null;
            }
            finally
            {
                key?.Close();
                hive?.Close();
            }
        }

        public static TimeSpan GetUpTime()
        {
            return TimeSpan.FromMilliseconds(GetTickCount64());
        }

        public static string GetVersion()
        {
            try
            {
                return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
            }
            catch (Exception ex)
            {
                Debug.Exception(nameof(HostInfo), nameof(GetVersion), ex);
            }

            return String.Empty;
        }

        [DllImport("kernel32")]
        private static extern UInt64 GetTickCount64();

        #endregion Static
    }

    public class InterfaceInfo
    {
        #region Static

        public static List<InterfaceInfo> GetInfos()
        {
            var ret = new List<InterfaceInfo>();

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

                            ret.Add(new InterfaceInfo(unicastAddress.Address.ToString(), @interface.Name, @interface.GetPhysicalAddress().ToString(), broadcast.ToString()));
                        }
                    }
            }
            catch (Exception ex)
            {
                Debug.Exception(nameof(InterfaceInfo), nameof(GetInfos), ex);
            }

            return ret;
        }

        #endregion Static

        #region Props

        public String BroadcastAddress { get; }
        public String InterfaceName { get; }
        public String MacAddress { get; }
        public String SenderAddress { get; }

        #endregion Props

        #region Methods

        public InterfaceInfo(string address, string interfaceName, string macAddress, string broadcastAddress)
        {
            this.SenderAddress = address;
            this.InterfaceName = interfaceName;
            this.MacAddress = macAddress;
            this.BroadcastAddress = broadcastAddress;
        }

        #endregion Methods
    }
}