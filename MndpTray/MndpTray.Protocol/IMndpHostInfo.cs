using System;
using System.Collections.Generic;

namespace MndpTray.Protocol
{
    public interface IMndpHostInfo
    {
        string BoardName { get; }
        string Identity { get; }
        List<MndpHostInfo.MndpInterfaceInfo> InterfaceInfos { get; }
        string Platform { get; }
        string SoftwareId { get; }
        TimeSpan UpTime { get; }
        string Version { get; }
    }
}