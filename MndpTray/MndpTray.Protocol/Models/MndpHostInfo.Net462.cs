/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
#if NET462_OR_GREATER
namespace MndpTray.Protocol
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Management;
    using System.Net;
    using System.Net.NetworkInformation;

    /// <summary>
    /// Mikrotik discovery message host information provider.
    /// </summary>
    public partial class MndpHostInfo : IMndpHostInfo
    {
        /// <summary>
        /// Gets host platform (From management object ComputerSystem.Manufacturer).
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
                    if (Log.IsEnabled)
                        Log.Exception(nameof(MndpHostInfo), nameof(this.Platform), ex);
                }

                return string.Empty;
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

                    return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", string.Empty).ToString();
                }
                catch (Exception ex)
                {
                    if (Log.IsEnabled)
                        Log.Exception(nameof(MndpHostInfo), nameof(this.Version), ex);
                }

                return string.Empty;
            }
        }
    }
}
#endif
