/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
#if NETSTANDARD2_0_OR_GREATER
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MndpTray.Protocol
{
    /// <summary>
    /// Platform specific utilities
    /// </summary>
    internal static class PlatformSpec
    {
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetOsName()
        {
            var osPlatform = GetOsPlatform();

            if (osPlatform == OSPlatform.Windows)
                return "Windows";

            if (osPlatform == OSPlatform.Linux)
                return GetDataFromOsRelease("NAME") + " Linux";

            return String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetOsVersion()
        {
            var osPlatform = GetOsPlatform();

            if (osPlatform == OSPlatform.Windows)
                return System.Runtime.InteropServices.RuntimeInformation.OSDescription;

            if (osPlatform == OSPlatform.Linux)
                return GetDataFromOsRelease("VERSION");

            return String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetManufacturer()
        {
            var osPlatform = GetOsPlatform();

            if (osPlatform == OSPlatform.Windows)
                return "Windows";

            if (osPlatform == OSPlatform.Linux)
            {
                if (File.Exists("/sys/devices/virtual/dmi/id/product_name"))
                {
                    return File.ReadAllText("/sys/devices/virtual/dmi/id/product_name");
                }
                else
                    return "Linux";
            }

            return String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static OSPlatform GetOsPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return OSPlatform.Windows;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return OSPlatform.OSX;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return OSPlatform.Linux;

            return OSPlatform.Create("Other Platform");
        }
    }
}

#endif
