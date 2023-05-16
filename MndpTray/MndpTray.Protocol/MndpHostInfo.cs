/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
#if NET462_OR_GREATER
    using System.Management;
#endif
    using System.Net;
    using System.Net.NetworkInformation;
    using Microsoft.Win32;

    /// <summary>
    /// Mikrotik discovery message host information provider.
    /// </summary>
    public class MndpHostInfo : IMndpHostInfo
    {
        #region Static

        static MndpHostInfo()
        {
            Instance = new MndpHostInfo();
        }

        /// <summary>
        /// Gets singleton instance.
        /// </summary>
        public static MndpHostInfo Instance { get; }

        #endregion Static

        #region Props

        /// <summary>
        /// Gets host board name (x86).
        /// </summary>
        public string BoardName
        {
            get
            {
                try
                {
                    return System.Environment.Is64BitOperatingSystem ? "x64" : "x86";
                }
                catch (Exception ex)
                {
                    Log.Exception(nameof(MndpHostInfo), nameof(this.BoardName), ex);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets host indentity (Host dns name).
        /// </summary>
        public string Identity
        {
            get
            {
                try
                {
                    return Dns.GetHostEntry(string.Empty).HostName;
                }
                catch (Exception ex)
                {
                    Log.Exception(nameof(MndpHostInfo), nameof(this.Identity), ex);
                }

                return Environment.MachineName;
            }
        }

        /// <summary>
        /// Gets host interface info.
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
                        {
                            var addresses = @interface.GetIPProperties().UnicastAddresses;
                            foreach (var unicastAddress in addresses)
                            {
                                if (unicastAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && unicastAddress.IPv4Mask != null)
                                {
                                    var addressInt = BitConverter.ToInt32(unicastAddress.Address.GetAddressBytes(), 0);
                                    var maskInt = BitConverter.ToInt32(unicastAddress.IPv4Mask.GetAddressBytes(), 0);
                                    var broadcastInt = addressInt | ~maskInt;
                                    var broadcast = new IPAddress(BitConverter.GetBytes(broadcastInt));
                                    var ipv6Address = addresses.Where(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6).FirstOrDefault()?.Address;

                                    ret.Add(new MndpInterfaceInfo(broadcast.ToString(), @interface.Name, @interface.GetPhysicalAddress().ToString(), unicastAddress.Address.ToString(), ipv6Address?.ToString()));
                                }
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
        /// Gets host platform (From management object ComputerSystem.Manufacturer).
        /// </summary>
        public string Platform
        {
            get
            {
                try
                {
#if NET462_OR_GREATER
                    ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                    ManagementObjectCollection moc = mc.GetInstances();
                    if (moc.Count != 0)
                    {
                        foreach (ManagementObject mo in mc.GetInstances())
                        {
                            return mo["Manufacturer"].ToString();
                        }
                    }
#elif NETSTANDARD2_0_OR_GREATER
                    return PlatformSpec.GetManufacturer();
#else
                    return "";
#endif
                }
                catch (Exception ex)
                {
                    Log.Exception(nameof(MndpHostInfo), nameof(this.Platform), ex);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets logged In user name.
        /// </summary>
        public string SoftwareId
        {
            get
            {

#if NET462_OR_GREATER
                {
                    try
                    {
                        using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT UserName FROM Win32_ComputerSystem"))
                        {
                            foreach (ManagementObject queryObj in searcher.Get())
                            {
                                string userName = null;

                                var obj = queryObj["UserName"];

                                if (obj == null)
                                {
                                    continue;
                                }

                                userName = obj.ToString();

                                if (string.IsNullOrEmpty(userName))
                                {
                                    continue;
                                }

                                userName = userName.Substring(userName.LastIndexOf('\\') + 1);

                                if (string.IsNullOrEmpty(userName))
                                {
                                    continue;
                                }

                                return userName;
                            }
                        }

                        using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(new SelectQuery(@"Select * from Win32_Process")))
                        {
                            foreach (ManagementObject obj in searcher.Get())
                            {
                                string path = obj["ExecutablePath"] as string;

                                if (string.IsNullOrEmpty(path))
                                {
                                    continue;
                                }

                                if (!path.EndsWith("explorer.exe", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    continue;
                                }

                                string[] ownerInfo = new string[2];
                                obj.InvokeMethod("GetOwner", (object[])ownerInfo);

                                if (ownerInfo == null)
                                {
                                    continue;
                                }

                                if (string.IsNullOrEmpty(ownerInfo[0]))
                                {
                                    continue;
                                }

                                return ownerInfo[0];
                            }
                        }

                        return string.Empty;
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(nameof(MndpHostInfo), nameof(this.SoftwareId), ex);
                    }

                    return string.Empty;
                }
#else
                {
                    return String.Empty;
                }
#endif

            }
        }

        /// <summary>
        /// Gets host uptime (From 64-bit TickCount).
        /// </summary>
        public TimeSpan UpTime
        {
            get
            {
                var ticks = Stopwatch.GetTimestamp();
                var uptime = ((double)ticks) / Stopwatch.Frequency;
                return TimeSpan.FromSeconds(uptime);
            }
        }

        /// <summary>
        /// Gets host software version (From Registry ProductName).
        /// </summary>
        public string Version
        {
            get
            {
                try
                {
#if NET462_OR_GREATER
                return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", string.Empty).ToString();
#elif NETSTANDARD2_0_OR_GREATER
                return PlatformSpec.GetOsVersion();
#else
                return "";
#endif
                }
                catch (Exception ex)
                {
                    Log.Exception(nameof(MndpHostInfo), nameof(this.Version), ex);
                }

                return string.Empty;
            }
        }
        #endregion Props
    }
}