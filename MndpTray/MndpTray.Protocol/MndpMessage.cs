/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Text;

    /// <summary>
    /// Mikrotik discovery message .
    /// </summary>
    public class MndpMessage
    {
        #region Consts

        private const ushort TLV_TYPE_BOARD_NAME = 12;
        private const ushort TLV_TYPE_IDENTITY = 5;
        private const ushort TLV_TYPE_INTERFACE_NAME = 16;
        private const ushort TLV_TYPE_IPV6 = 15;
        private const ushort TLV_TYPE_MAC_ADDRESS = 1;
        private const ushort TLV_TYPE_PLATFORM = 8;
        private const ushort TLV_TYPE_SOFTWAREID = 11;
        private const ushort TLV_TYPE_UNPACK = 14;
        private const ushort TLV_TYPE_UPTIME = 10;
        private const ushort TLV_TYPE_VERSION = 7;

        #endregion Consts

        #region Props

        /// <summary>
        /// Gets or sets sender board name.
        /// </summary>
        public string BoardName { get; set; }

        /// <summary>
        /// Gets or sets sender device name.
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// Gets or sets sender interface name.
        /// </summary>
        public string InterfaceName { get; set; }

        /// <summary>
        /// Gets or sets sender unicast IPv6 address.
        /// </summary>
        public string UnicastIPv6Address { get; set; }

        /// <summary>
        /// Gets or sets sender mac address.
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        /// Gets or sets sender platform.
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets send sequence number.
        /// </summary>
        public ushort Sequence { get; set; }

        /// <summary>
        /// Gets or sets sender software id.
        /// </summary>
        public string SoftwareId { get; set; }

        /// <summary>
        /// Gets or sets ???.
        /// </summary>
        public byte Ttl { get; set; }

        /// <summary>
        /// Gets or sets ???.
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Gets or sets ???.
        /// </summary>
        public byte Unpack { get; set; }

        /// <summary>
        /// Gets or sets time elapsed since sender boot.
        /// </summary>
        public TimeSpan Uptime { get; set; }

        /// <summary>
        /// Gets or sets sender software version.
        /// </summary>
        public string Version { get; set; }

        #endregion Props

        #region Methods

        /// <summary>
        /// Parse from raw udp data bytes.
        /// </summary>
        /// <param name="data">raw udp data.</param>
        /// <returns>Parse is sucess? .</returns>
        public bool Read(byte[] data)
        {
            try
            {
                var tlvMessage = new TlvMessage();
                bool ok = tlvMessage.Read(data);

                if (!ok)
                {
                    return false;
                }

                var enc = Encoding.GetEncoding(28591);

                this.Type = tlvMessage.Type;
                this.Ttl = tlvMessage.Ttl;
                this.Sequence = tlvMessage.Sequence;

                foreach (var i in tlvMessage.Items)
                {
                    switch (i.Type)
                    {
                        case TLV_TYPE_MAC_ADDRESS:
                            this.MacAddress = new PhysicalAddress(i.Value).ToString();
                            break;

                        case TLV_TYPE_IDENTITY:
                            this.Identity = enc.GetString(i.Value);
                            break;

                        case TLV_TYPE_VERSION:
                            this.Version = enc.GetString(i.Value);
                            break;

                        case TLV_TYPE_PLATFORM:
                            this.Platform = enc.GetString(i.Value);
                            break;

                        case TLV_TYPE_UPTIME:
                            this.Uptime = TimeSpan.FromSeconds(BitConverter.ToUInt32(i.Value, 0));
                            break;

                        case TLV_TYPE_SOFTWAREID:
                            this.SoftwareId = enc.GetString(i.Value);
                            break;

                        case TLV_TYPE_BOARD_NAME:
                            this.BoardName = enc.GetString(i.Value);
                            break;

                        case TLV_TYPE_UNPACK:
                            this.Unpack = i.Value[0];
                            break;

                        case TLV_TYPE_INTERFACE_NAME:
                            this.InterfaceName = enc.GetString(i.Value);
                            break;

                        case TLV_TYPE_IPV6:
                            this.UnicastIPv6Address = new IPAddress(i.Value).ToString();
                            break;

                        default: break;
                    }
                }

                Log.Info("{0} Read,{2}{1}{2}", nameof(MndpMessage), this.ToString(), Environment.NewLine);

                return true;
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(MndpMessage), nameof(this.Read), ex);
            }

            return false;
        }

        /// <summary>
        /// Debug message.
        /// </summary>
        /// <returns>Debug string.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.Type), this.Type);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.Ttl), this.Ttl);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.Sequence), this.Sequence);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.MacAddress), this.MacAddress);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.Identity), this.Identity);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.Version), this.Version);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.Platform), this.Platform);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.Uptime), this.Uptime.ToString());
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.SoftwareId), this.SoftwareId);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.BoardName), this.BoardName);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.Unpack), this.Unpack);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.InterfaceName), this.InterfaceName);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.UnicastIPv6Address), this.UnicastIPv6Address);

            return sb.ToString();
        }

        /// <summary>
        /// Convert message to raw udp data.
        /// </summary>
        /// <returns>Raw udp data.</returns>
        public byte[] Write()
        {
            try
            {
                Log.Info("{0} Write,{2}{1}{2}", nameof(MndpMessage), this.ToString(), Environment.NewLine);

                var tlvMessage = new TlvMessage()
                {
                    Type = this.Type,
                    Ttl = this.Ttl,
                    Sequence = this.Sequence,
                };

                var enc = Encoding.GetEncoding(28591);

                for (ushort i = 0; i <= 16; i++)
                {
                    switch (i)
                    {
                        case TLV_TYPE_MAC_ADDRESS:
                            tlvMessage.Items.Add(new Tlv(i, PhysicalAddress.Parse(this.MacAddress).GetAddressBytes()));
                            break;

                        case TLV_TYPE_IDENTITY:
                            tlvMessage.Items.Add(new Tlv(i, this.Identity, enc));
                            break;

                        case TLV_TYPE_VERSION:
                            tlvMessage.Items.Add(new Tlv(i, this.Version, enc));
                            break;

                        case TLV_TYPE_PLATFORM:
                            tlvMessage.Items.Add(new Tlv(i, this.Platform, enc));
                            break;

                        case TLV_TYPE_UPTIME:
                            tlvMessage.Items.Add(new Tlv(i, (uint)this.Uptime.TotalSeconds));
                            break;

                        case TLV_TYPE_SOFTWAREID:
                            tlvMessage.Items.Add(new Tlv(i, this.SoftwareId, enc));
                            break;

                        case TLV_TYPE_BOARD_NAME:
                            tlvMessage.Items.Add(new Tlv(i, this.BoardName, enc));
                            break;

                        case TLV_TYPE_UNPACK:
                            tlvMessage.Items.Add(new Tlv(i, new byte[] { this.Unpack }));
                            break;

                        case TLV_TYPE_INTERFACE_NAME:
                            tlvMessage.Items.Add(new Tlv(i, this.InterfaceName, enc));
                            break;

                        case TLV_TYPE_IPV6:
                            if (this.UnicastIPv6Address != null)
                            {
                                tlvMessage.Items.Add(new Tlv(i, IPAddress.Parse(this.UnicastIPv6Address).GetAddressBytes()));
                            }

                            break;

                        default: break;
                    }
                }

                bool ok = tlvMessage.Write(out byte[] data);

                if (!ok)
                {
                    return null;
                }

                return data;
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(MndpMessage), nameof(this.Write), ex);
            }

            return null;
        }

        #endregion Methods

        /// <summary>
        /// T=Type , L=Length , V=Value .
        /// </summary>
        protected class Tlv
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Tlv"/> class.
            /// </summary>
            public Tlv()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Tlv"/> class.
            /// </summary>
            /// <param name="type">Record type.</param>
            /// <param name="length">Record length.</param>
            /// <param name="value">Record value.</param>
            public Tlv(ushort type, ushort length, byte[] value)
            {
                this.Type = type;
                this.Length = length;
                this.Value = value;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Tlv"/> class.
            /// </summary>
            /// <param name="type">Record type.</param>
            /// <param name="data">Record value.</param>
            public Tlv(ushort type, byte[] data)
                : this(type, (ushort)(data?.Length ?? 0), data)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Tlv"/> class.
            /// </summary>
            /// <param name="type">Record type.</param>
            /// <param name="data">Record value.</param>
            public Tlv(ushort type, uint data)
                : this(type, BitConverter.GetBytes(data))
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Tlv"/> class.
            /// </summary>
            /// <param name="type">Record type.</param>
            /// <param name="data">Record value.</param>
            /// <param name="encoding">Record value encoding.</param>
            public Tlv(ushort type, string data, Encoding encoding)
                : this(type, data != null ? encoding?.GetBytes(data) : Array.Empty<byte>())
            {
            }

            #region Props

            /// <summary>
            /// Gets or sets record type.
            /// </summary>
            public ushort Type { get; set; }

            /// <summary>
            /// Gets or sets Record length.
            /// </summary>
            public ushort Length { get; set; }

            /// <summary>
            /// Gets or sets record value.
            /// </summary>
            public byte[] Value { get; set; }

            #endregion Props

            #region Methods

            /// <summary>
            /// Read record from BinaryReader.
            /// </summary>
            /// <param name="br">BinaryReader object.</param>
            /// <returns>Is sucess?.</returns>
            public virtual bool Read(BinaryReader br)
            {
                try
                {
                    this.Type = br.ReadUInt16Reverse();
                    this.Length = br.ReadUInt16Reverse();
                    this.Value = br.ReadBytes(this.Length);

                    if (this.Length > 1400)
                    {
                        throw new Exception("Tlv too long!");
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Exception(nameof(Tlv), nameof(this.Read), ex);
                }

                return false;
            }

            /// <summary>
            /// Debug TLV.
            /// </summary>
            /// <returns>Debug string.</returns>
            public override string ToString()
            {
                var valueHex = string.Empty;
                var valueStr = string.Empty;

                if (this.Value != null)
                {
                    valueHex = BitConverter.ToString(this.Value).Replace("-", ",");
                    valueStr = Encoding.GetEncoding(28591).GetString(this.Value);
                }

                return string.Format(CultureInfo.InvariantCulture,"T:{0}, L:{1}, V:{2}, VS:{3}", this.Type, this.Length, valueHex, valueStr);
            }

            /// <summary>
            /// Write TLV to BinaryWriter.
            /// </summary>
            /// <param name="bw">BinaryWriter object.</param>
            /// <returns>Is success?.</returns>
            public virtual bool Write(BinaryWriter bw)
            {
                try
                {
                    bw.WriteUInt16Reverse(this.Type);
                    bw.WriteUInt16Reverse(this.Length);
                    bw.Write(this.Value);

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Exception(nameof(Tlv), nameof(this.Write), ex);
                }

                return false;
            }

            #endregion Methods
        }

        /// <summary>
        /// Message in TLV collection format.
        /// </summary>
        protected class TlvMessage
        {
            #region Props

            /// <summary>
            /// Gets or sets TLV record list.
            /// </summary>
            public List<Tlv> Items { get; set; } = new List<Tlv>();

            /// <summary>
            /// Gets or sets sequence number.
            /// </summary>
            public ushort Sequence { get; set; }

            /// <summary>
            /// Gets or sets ???.
            /// </summary>
            public byte Ttl { get; set; }

            /// <summary>
            /// Gets or sets ???.
            /// </summary>
            public byte Type { get; set; }

            #endregion Props

            #region Methods

            /// <summary>
            /// Read all records from BinaryReader.
            /// </summary>
            /// <param name="br">BinaryReader object.</param>
            /// <returns>Is sucess? .</returns>
            public bool Read(BinaryReader br)
            {
                this.Type = br.ReadByte();
                this.Ttl = br.ReadByte();
                this.Sequence = br.ReadUInt16Reverse();

                while (br.BaseStream.Length > br.BaseStream.Position)
                {
                    var item = new Tlv();
                    bool ok = item.Read(br);
                    if (!ok)
                    {
                        return false;
                    }

                    this.Items.Add(item);
                }

                return true;
            }

            /// <summary>
            /// Parse TLV records from raw udp data.
            /// </summary>
            /// <param name="data">raw udp data.</param>
            /// <returns>Is success?.</returns>
            public bool Read(byte[] data)
            {
                try
                {
                    if (data.Length < 8)
                    {
                        Log.Info("{0} Read, Message Too Short", nameof(TlvMessage));
                        return false;
                    }

                    MemoryStream ms = new MemoryStream(data);
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            bool ok = this.Read(br);
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Exception(nameof(TlvMessage), nameof(this.Read), ex);
                }

                return false;
            }

            /// <summary>
            /// Debug TLV message.
            /// </summary>
            /// <returns>Debug string.</returns>
            public override string ToString()
            {
                return string.Format(CultureInfo.CurrentCulture,"Ver:{0}, Ttl:{1}, Seq:{2}", this.Type, this.Ttl, this.Sequence);
            }

            /// <summary>
            /// Conver all TLV record to raw udp data.
            /// </summary>
            /// <param name="data">raw udp data.</param>
            /// <returns>Is success?.</returns>
            public bool Write(out byte[] data)
            {
                data = null;

                try
                {
                    MemoryStream ms = new MemoryStream();
                    {
                        using (var bw = new BinaryWriter(ms))
                        {
                            bool ok = this.Write(bw);
                            if (!ok)
                            {
                                return false;
                            }

                            data = ms.ToArray();
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Exception(nameof(TlvMessage), nameof(this.Write), ex);
                }

                return false;
            }

            /// <summary>
            /// Write all record to BinaryWriter.
            /// </summary>
            /// <param name="bw">BinaryWriter object.</param>
            /// <returns>Is success?.</returns>
            public bool Write(BinaryWriter bw)
            {
                bw.Write(this.Type);
                bw.Write(this.Ttl);
                bw.WriteUInt16Reverse(this.Sequence);

                if (this.Items != null)
                {
                    foreach (var i in this.Items)
                    {
                        if (!i.Write(bw))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            #endregion Methods
        }
    }

    /// <summary>
    /// Mikrotik discovery message with extensions.
    /// </summary>
    public class MndpMessageEx : MndpMessage, ICloneable
    {
        #region Props

        /// <summary>
        /// Gets time elapsed since message received.
        /// </summary>
        public double Age
        {
            get { return (DateTime.Now - this.ReceiveDateTime).TotalSeconds; }
        }

        /// <summary>
        /// Gets or Sets sender broadcast address.
        /// </summary>
        public string BroadcastAddress { get; set; }

        /// <summary>
        /// Gets or sets message receive DateTime.
        /// </summary>
        public DateTime ReceiveDateTime { get; set; }

        /// <summary>
        /// Gets or sets sender unicast IPv4 address.
        /// </summary>
        public string UnicastAddress { get; set; }

        /// <summary>
        /// Gets sender mac address formatted ( ':' delimited ).
        /// </summary>
        /// <example>
        /// AA:BB:CC:DD:EE:FF .
        /// </example>
        public string MacAddressDelimited
        {
            get
            {
                if (this.MacAddress == null)
                {
                    return null;
                }

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < this.MacAddress.Length; i++)
                {
                    sb.Append(this.MacAddress[i]);
                    if (i % 2 == 1)
                    {
                        sb.Append(':');
                    }
                }

                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }

                return sb.ToString();
            }
        }

        #endregion Props

        #region Methods

        /// <summary>
        /// Clone object.
        /// </summary>
        /// <returns>Message object.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Debug message.
        /// </summary>
        /// <returns>Debug string.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.ReceiveDateTime), this.ReceiveDateTime);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.UnicastAddress), this.UnicastAddress);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.BroadcastAddress), this.BroadcastAddress);
            sb.Append(base.ToString());

            return sb.ToString();
        }

        #endregion Methods
    }
}