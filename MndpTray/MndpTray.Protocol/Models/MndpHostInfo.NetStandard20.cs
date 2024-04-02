/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
#if NETSTANDARD2_0_OR_GREATER
namespace MndpTray.Protocol
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Mikrotik discovery message host information provider.
    /// </summary>
    public partial class MndpHostInfo : IMndpHostInfo
    {
        #region Static
        private static string GetDataFromOsRelease(string key)
        {
            key = key.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            key += "=";

            if (File.Exists("/etc/os-release"))
            {
                var lines = File.ReadAllLines("/etc/os-release");
                foreach (var line in lines)
                {
                    if (line.StartsWith(key, StringComparison.Ordinal))
                    {
                        return line.Substring(key.Length).Trim('"', '\'');
                    }
                }
            }

            return String.Empty;
        }
        private static string GetOsName()
        {
            var osPlatform = GetOsPlatform();

            if (osPlatform == OSPlatform.Windows)
                return "Windows";

            if (osPlatform == OSPlatform.Linux)
                return GetDataFromOsRelease("NAME") + " Linux";

            if (osPlatform == OSPlatform.OSX)
                return "OSX";

            return string.Empty;
        }
        private static OSPlatform GetOsPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return OSPlatform.Windows;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return OSPlatform.OSX;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return OSPlatform.Linux;

            return OSPlatform.Create("Other Platform");
        }
        #endregion

        #region Props

        /// <summary>
        /// Gets host platform (Computer Manufacturer).
        /// </summary>
        public string Platform
        {
            get
            {
                try
                {
                    var osPlatform = GetOsPlatform();

                    if (osPlatform == OSPlatform.Windows)
                        return "Windows";

                    if (osPlatform == OSPlatform.OSX)
                        return "OSX";

                    if (osPlatform == OSPlatform.Linux)
                    {
                        if (File.Exists("/sys/devices/virtual/dmi/id/product_name"))
                        {
                            return File.ReadAllText("/sys/devices/virtual/dmi/id/product_name");
                        }
                        else
                            return "Linux";
                    }

                    return string.Empty;
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
                return Environment.UserName;
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

                    var osPlatform = GetOsPlatform();

                    if (osPlatform == OSPlatform.Windows)
                        return RuntimeInformation.OSDescription;

                    if (osPlatform == OSPlatform.Linux)
                        return GetDataFromOsRelease("VERSION");

                    return String.Empty;

                }
                catch (Exception ex)
                {
                    Log.Exception(nameof(MndpHostInfo), nameof(this.Version), ex);
                }

                return string.Empty;
            }
        }

        #endregion
    }
}
#endif