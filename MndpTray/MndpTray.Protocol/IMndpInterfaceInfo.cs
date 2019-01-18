namespace MndpTray.Protocol
{
    /// <summary>
    /// Mikrotik discovery message host interface information
    /// </summary>
    public interface IMndpInterfaceInfo
    {
        /// <summary>
        /// Interface brodcast IPv4 address
        /// </summary>
        /// <example>192.168.0.255</example>
        string BroadcastAddress { get; }

        /// <summary>
        /// Interface name
        /// </summary>
        string InterfaceName { get; }

        /// <summary>
        /// Interface mac address
        /// </summary>
        /// <example>AABBCCDDEEFF</example>
        string MacAddress { get; }

        /// <summary>
        /// Interface Ipv4 unicast address
        /// </summary>
        /// <example>192.168.0.1</example>
        string UnicastAddress { get; }
    }
}