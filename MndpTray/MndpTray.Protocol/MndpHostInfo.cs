using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;

namespace MndpTray.Protocol
{
    /// <summary>
    /// Mikrotik discovery message host information provider
    /// </summary>
    public class MndpHostInfo : IMndpHostInfo
    {
        #region Static

        static MndpHostInfo()
        {
            Instance = new MndpHostInfo();
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static MndpHostInfo Instance { get; }

        #endregion Static

        #region Props

        /// <summary>
        /// Host board name (x86)
        /// </summary>
        public string BoardName => "x86";

        /// <summary>
        /// Host indentity (Host dns name)
        /// </summary>
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
                    Log.Exception(nameof(MndpHostInfo), nameof(this.Identity), ex);
                }

                return Environment.MachineName;
            }
        }

        /// <summary>
        /// Host interface info
        /// </summary>
        public List<IMndpInterfaceInfo> InterfaceInfos
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
                        Log.Exception(nameof(MndpHostInfo), nameof(this.InterfaceInfos), ex);
                    }

                    return new List<IMndpInterfaceInfo>(ret);
                }
            }
        }

        /// <summary>
        /// Host platform (From management object ComputerSystem.Manufacturer)
        /// </summary>
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
                    Log.Exception(nameof(MndpHostInfo), nameof(this.Platform), ex);
                }

                return String.Empty;
            }
        }

        /// <summary>
        /// Host software id (From Registry DigitalProductId)
        /// </summary>
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
                        Log.Exception(nameof(MndpHostInfo), nameof(this.SoftwareId), ex);
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

        /// <summary>
        /// Host uptime (From 64-bit TickCount)
        /// </summary>
        public TimeSpan UpTime
        {
            get
            {
                return TimeSpan.FromMilliseconds(NativeMethods.GetTickCount64());
            }
        }

        /// <summary>
        /// Host software version (From Registry ProductName)
        /// </summary>
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
                    Log.Exception(nameof(MndpHostInfo), nameof(this.Version), ex);
                }

                return String.Empty;
            }
        }

        #endregion Props      
    }
}