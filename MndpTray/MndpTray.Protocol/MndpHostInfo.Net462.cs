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
                    Log.Exception(nameof(MndpHostInfo), nameof(this.Version), ex);
                }

                return string.Empty;
            }
        }
    }
}
#endif
