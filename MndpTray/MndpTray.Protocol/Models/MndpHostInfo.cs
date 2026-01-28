/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Protocol
{
    using MndpTray.Protocol.Internal;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Reflection;

    /// <summary>
    /// Mikrotik discovery message host information provider.
    /// </summary>
    public partial class MndpHostInfo : IMndpHostInfo
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

        #region Fields

        /// <summary>
        /// software id (e.g. MndpTray - 2.2.0).
        /// </summary>
        protected string _softwareId = "";

        #endregion

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
                    if (Log.IsEnabled)
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
                    if (Log.IsEnabled)
                        Log.Exception(nameof(MndpHostInfo), nameof(this.Identity), ex);
                }

                return Environment.MachineName;
            }
        }

        /// <summary>
        /// Custom software id (e.g. MndpTray - 2.2.0).
        /// </summary>
        public string SoftwareId { get { return this._softwareId; } }


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
                        if (Log.IsEnabled)
                            Log.Exception(nameof(MndpHostInfo), nameof(this.InterfaceInfos), ex);
                    }

                    return new List<IMndpInterfaceInfo>(ret);
                }
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

        #endregion Props

        #region Methods

        /// <summary>
        /// Sets custom software id from assembly name and version.
        /// </summary>
        /// <param name="assembly"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetSoftwareIdFromAssemblyName(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var assemblyName = assembly.GetName();

            this._softwareId = string.Concat(assemblyName.Name, " - ", assemblyName.Version.ToString(3));
        }

        #endregion
    }
}