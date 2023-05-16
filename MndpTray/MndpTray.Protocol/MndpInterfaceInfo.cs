/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Protocol
{
    /// <summary>
    /// Mikrotik discovery message host interface information.
    /// </summary>
    public class MndpInterfaceInfo : IMndpInterfaceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MndpInterfaceInfo"/> class.
        /// </summary>
        /// <param name="broadcastAddress">Interface broadcast address.</param>
        /// <param name="interfaceName">Interface name.</param>
        /// <param name="macAddress">Interface mac address 12 hexdigit.</param>
        /// <param name="unicastAddress">Interface unicast address.</param>
        /// <param name="unicastIpv6Address">Interface Ipv6 unicast address.</param>
        public MndpInterfaceInfo(string broadcastAddress, string interfaceName, string macAddress, string unicastAddress, string unicastIpv6Address)
        {
            this.BroadcastAddress = broadcastAddress;
            this.InterfaceName = interfaceName;
            this.MacAddress = macAddress;
            this.UnicastAddress = unicastAddress;
            this.UnicastIPv6Address = unicastIpv6Address;
        }

        #region Props

        /// <summary>
        /// Gets interface brodcast IPv4 address.
        /// </summary>
        /// <example>192.168.0.255 .</example>
        public string BroadcastAddress { get; }

        /// <summary>
        /// Gets interface name.
        /// </summary>
        public string InterfaceName { get; }

        /// <summary>
        /// Gets interface mac address.
        /// </summary>
        /// <example>AABBCCDDEEFF .</example>
        public string MacAddress { get; }

        /// <summary>
        /// Gets interface Ipv4 unicast address.
        /// </summary>
        /// <example>192.168.0.1 .</example>
        public string UnicastAddress { get; }

        /// <summary>
        /// Gets interface Ipv6 unicast address.
        /// </summary>
        /// <example>fe80::2d1f:d9f4:4c05:a200 .</example>
        public string UnicastIPv6Address { get; }

        #endregion Props
    }
}