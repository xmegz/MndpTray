using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if (!NETCOREAPP2_1)
using System.Management;
#endif
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
        public string BoardName
        {
            get
            {
                try
                {                    
                    return (System.Environment.Is64BitOperatingSystem) ? "x64" : "x86";
                }
                catch (Exception ex)
                {
                    Log.Exception(nameof(MndpHostInfo), nameof(this.BoardName), ex);
                }

                return String.Empty;
            }
        }

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
        /// Host platform (From management object ComputerSystem.Manufacturer)
        /// </summary>
        public string Platform
        {
            get
            {
                try
                {
#if (NETCOREAPP2_1)
                    return MndpService.Core.PlatformSpec.GetManufacturer();
#else
                    ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                    ManagementObjectCollection moc = mc.GetInstances();
                    if (moc.Count != 0)
                    {
                        foreach (ManagementObject mo in mc.GetInstances())
                        {
                            return mo["Manufacturer"].ToString();
                        }
                    }
#endif
                }
                catch (Exception ex)
                {
                    Log.Exception(nameof(MndpHostInfo), nameof(this.Platform), ex);
                }

                return String.Empty;
            }
        }

        /// <summary>
        /// Logged In user name
        /// </summary>
        public string SoftwareId
        {
            get
            {
               #if (NETCOREAPP2_1)
               {
                    return String.Empty;
               }
               #else
                {
                    try
                    {                      
                        {
                            SelectQuery query = new SelectQuery(@"Select * from Win32_Process");
                            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                            {
                                foreach (ManagementObject obj in searcher.Get())
                                {
                                    string path = obj["ExecutablePath"] as String;

                                    if (path == null) continue;
                                    if (!path.EndsWith("explorer.exe",StringComparison.InvariantCultureIgnoreCase)) continue;
                                    
                                    string[] ownerInfo = new string[2];
                                    obj.InvokeMethod("GetOwner", (object[])ownerInfo);

                                    return ownerInfo[0];                                    
                                }
                            }
                            return "";
                        }
                    }

                    catch (Exception ex)
                    {
                        Log.Exception(nameof(MndpHostInfo), nameof(this.SoftwareId), ex);
                    }

                    return String.Empty;
                }
               #endif
            }
        }

        /// <summary>
        /// Host uptime (From 64-bit TickCount)
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
        /// Host software version (From Registry ProductName)
        /// </summary>
        public string Version
        {
            get
            {
                try
                {
#if (NETCOREAPP2_1)
                    return MndpService.Core.PlatformSpec.GetOsVersion();
#else
                    return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();                    
#endif

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