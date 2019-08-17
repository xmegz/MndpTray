using System;
using System.Collections.Generic;

namespace MndpTray.Protocol
{
    /// <summary>
    /// Mndp Host information
    /// </summary>
    public interface IMndpHostInfo
    {
        /// <summary>
        /// Host board name
        /// </summary>
        string BoardName { get; }

        /// <summary>
        /// Host identity
        /// </summary>
        string Identity { get; }

        /// <summary>
        /// Host interface information
        /// </summary>
        List<IMndpInterfaceInfo> InterfaceInfos { get; }

        /// <summary>
        /// Host platform
        /// </summary>
        string Platform { get; }

        /// <summary>
        /// Host sotware id
        /// </summary>
        string SoftwareId { get; }

        /// <summary>
        /// Host Uptime
        /// </summary>
        TimeSpan UpTime { get; }

        /// <summary>
        /// Host software version
        /// </summary>
        string Version { get; }
    }
}