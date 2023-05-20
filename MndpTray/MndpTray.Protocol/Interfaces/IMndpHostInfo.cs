/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Protocol
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Mndp Host information.
    /// </summary>
    public interface IMndpHostInfo
    {
        /// <summary>
        /// Gets host board name.
        /// </summary>
        string BoardName { get; }

        /// <summary>
        /// Gets host identity.
        /// </summary>
        string Identity { get; }

        /// <summary>
        /// Gets host interface information.
        /// </summary>
        List<IMndpInterfaceInfo> InterfaceInfos { get; }

        /// <summary>
        /// Gets host platform.
        /// </summary>
        string Platform { get; }

        /// <summary>
        /// Gets host sotware id.
        /// </summary>
        string SoftwareId { get; }

        /// <summary>
        /// Gets host uptime.
        /// </summary>
        TimeSpan UpTime { get; }

        /// <summary>
        /// Gets host software version.
        /// </summary>
        string Version { get; }
    }
}