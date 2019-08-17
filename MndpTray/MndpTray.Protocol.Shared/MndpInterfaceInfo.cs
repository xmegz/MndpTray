using System;

namespace MndpTray.Protocol
{
    /// <summary>
    /// Mikrotik discovery message host interface information
    /// </summary>
    public class MndpInterfaceInfo : IMndpInterfaceInfo
    {
        #region Props

        /// <summary>
        /// Interface brodcast IPv4 address
        /// </summary>
        /// <example>192.168.0.255</example>
        public String BroadcastAddress { get; }

        /// <summary>
        /// Interface name
        /// </summary>
        public String InterfaceName { get; }

        /// <summary>
        /// Interface mac address
        /// </summary>
        /// <example>AABBCCDDEEFF</example>
        public String MacAddress { get; }

        /// <summary>
        /// Interface Ipv4 unicast address
        /// </summary>
        /// <example>192.168.0.1</example>
        public String UnicastAddress { get; }

        /// <summary>
        /// Interface Ipv6 unicast address
        /// </summary>
        /// <example>fe80::2d1f:d9f4:4c05:a200</example>
        public String UnicastIPv6Address { get; }

        #endregion Props

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="broadcastAddress">Interface broadcast address</param>
        /// <param name="interfaceName">Interface name</param>
        /// <param name="macAddress">Interface mac address 12 hexdigit</param>
        /// <param name="unicastAddress">Interface unicast address</param>
        /// <param name="unicastIpv6Address">Interface Ipv6 unicast address</param>
        public MndpInterfaceInfo(string broadcastAddress, string interfaceName, string macAddress, string unicastAddress, string unicastIpv6Address)
        {
            this.BroadcastAddress = broadcastAddress;
            this.InterfaceName = interfaceName;
            this.MacAddress = macAddress;
            this.UnicastAddress = unicastAddress;
            this.UnicastIPv6Address = unicastIpv6Address;
        }

        #endregion Methods
    }
}