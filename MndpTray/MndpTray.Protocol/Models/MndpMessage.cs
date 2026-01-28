/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Protocol
{
    using MndpTray.Protocol.Internal;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Text;

    /// <summary>
    /// Mikrotik discovery message
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

        private const int ENCODING_CODE_PAGE = 28591;

        #region Fields

        private static readonly Encoding ENCODING = Encoding.GetEncoding(ENCODING_CODE_PAGE);

        #endregion

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
                if (data == null || data.Length < 8)
                {
                    if (Log.IsEnabled)
                        Log.Info($"{nameof(MndpMessage)} Read, Message Too Short");

                    return false;
                }

                var tlvMessage = new TlvMessage();

                if (!tlvMessage.Read(data))
                    return false;

                this.Type = tlvMessage.Type;
                this.Ttl = tlvMessage.Ttl;
                this.Sequence = tlvMessage.Sequence;

                foreach (var item in tlvMessage.Items)
                {
                    switch (item.Type)
                    {
                        case TLV_TYPE_MAC_ADDRESS:
                            this.MacAddress = new PhysicalAddress(item.Value).ToString();
                            break;

                        case TLV_TYPE_IDENTITY:
                            this.Identity = ENCODING.GetString(item.Value);
                            break;

                        case TLV_TYPE_VERSION:
                            this.Version = ENCODING.GetString(item.Value);
                            break;

                        case TLV_TYPE_PLATFORM:
                            this.Platform = ENCODING.GetString(item.Value);
                            break;

                        case TLV_TYPE_UPTIME:
                            this.Uptime = item.Value.Length >= 4
                               ? TimeSpan.FromSeconds(BitConverter.ToUInt32(item.Value, 0))
                               : TimeSpan.Zero;
                            break;

                        case TLV_TYPE_SOFTWAREID:
                            this.SoftwareId = ENCODING.GetString(item.Value);
                            break;

                        case TLV_TYPE_BOARD_NAME:
                            this.BoardName = ENCODING.GetString(item.Value);
                            break;

                        case TLV_TYPE_UNPACK:
                            this.Unpack = item.Value.Length > 0 ? item.Value[0] : (byte)0;
                            break;

                        case TLV_TYPE_INTERFACE_NAME:
                            this.InterfaceName = ENCODING.GetString(item.Value);
                            break;

                        case TLV_TYPE_IPV6:
                            this.UnicastIPv6Address = new IPAddress(item.Value).ToString();
                            break;

                        default: break;
                    }
                }

                if (Log.IsEnabled)
                    Log.Info($"{nameof(MndpMessage)} Read,{Environment.NewLine}{this}{Environment.NewLine}");

                return true;
            }
            catch (Exception ex)
            {
                if (Log.IsEnabled)
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
            var sb = new StringBuilder(512);

            sb.AppendLine($"\t{nameof(Type)}:{Type},");
            sb.AppendLine($"\t{nameof(Ttl)}:{Ttl},");
            sb.AppendLine($"\t{nameof(Sequence)}:{Sequence},");
            sb.AppendLine($"\t{nameof(MacAddress)}:{MacAddress},");
            sb.AppendLine($"\t{nameof(Identity)}:{Identity},");
            sb.AppendLine($"\t{nameof(Version)}:{Version},");
            sb.AppendLine($"\t{nameof(Platform)}:{Platform},");
            sb.AppendLine($"\t{nameof(Uptime)}:{Uptime},");
            sb.AppendLine($"\t{nameof(SoftwareId)}:{SoftwareId},");
            sb.AppendLine($"\t{nameof(BoardName)}:{BoardName},");
            sb.AppendLine($"\t{nameof(Unpack)}:{Unpack},");
            sb.AppendLine($"\t{nameof(InterfaceName)}:{InterfaceName},");
            sb.AppendLine($"\t{nameof(UnicastIPv6Address)}:{UnicastIPv6Address},");

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
                if (Log.IsEnabled)
                    Log.Info($"{nameof(MndpMessage)} Write,{Environment.NewLine}{this}{Environment.NewLine}");

                var tlvMessage = new TlvMessage()
                {
                    Type = this.Type,
                    Ttl = this.Ttl,
                    Sequence = this.Sequence,
                };


                for (ushort i = 0; i <= 16; i++)
                {
                    switch (i)
                    {
                        case TLV_TYPE_MAC_ADDRESS:
                            tlvMessage.Items.Add(new Tlv(i, PhysicalAddress.Parse(this.MacAddress).GetAddressBytes()));
                            break;

                        case TLV_TYPE_IDENTITY:
                            tlvMessage.Items.Add(new Tlv(i, this.Identity, ENCODING));
                            break;

                        case TLV_TYPE_VERSION:
                            tlvMessage.Items.Add(new Tlv(i, this.Version, ENCODING));
                            break;

                        case TLV_TYPE_PLATFORM:
                            tlvMessage.Items.Add(new Tlv(i, this.Platform, ENCODING));
                            break;

                        case TLV_TYPE_UPTIME:
                            tlvMessage.Items.Add(new Tlv(i, (uint)this.Uptime.TotalSeconds));
                            break;

                        case TLV_TYPE_SOFTWAREID:
                            tlvMessage.Items.Add(new Tlv(i, this.SoftwareId, ENCODING));
                            break;

                        case TLV_TYPE_BOARD_NAME:
                            tlvMessage.Items.Add(new Tlv(i, this.BoardName, ENCODING));
                            break;

                        case TLV_TYPE_UNPACK:
                            tlvMessage.Items.Add(new Tlv(i, new byte[] { this.Unpack }));
                            break;

                        case TLV_TYPE_INTERFACE_NAME:
                            tlvMessage.Items.Add(new Tlv(i, this.InterfaceName, ENCODING));
                            break;

                        case TLV_TYPE_IPV6:
                            if (this.UnicastIPv6Address != null)
                                tlvMessage.Items.Add(new Tlv(i, IPAddress.Parse(this.UnicastIPv6Address).GetAddressBytes()));

                            break;

                        default: break;
                    }
                }
                if (tlvMessage.Write(out byte[] data))
                    return data;
                else
                    return null;
            }
            catch (Exception ex)
            {
                if (Log.IsEnabled)
                    Log.Exception(nameof(MndpMessage), nameof(this.Write), ex);
            }

            return null;
        }

        #endregion Methods

        #region Internal
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
                        throw new Exception("Tlv too long!");

                    return true;
                }
                catch (Exception ex)
                {
                    if (Log.IsEnabled)
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
                    valueStr = MndpMessage.ENCODING.GetString(this.Value);
                }

                return string.Format(CultureInfo.InvariantCulture, "T:{0}, L:{1}, V:{2}, VS:{3}", this.Type, this.Length, valueHex, valueStr);
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
                    if (Log.IsEnabled)
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

                    if (!item.Read(br))
                        return false;

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
                        if (Log.IsEnabled)
                            Log.Info("{0} Read, Message Too Short", nameof(TlvMessage));

                        return false;
                    }

                    MemoryStream ms = new MemoryStream(data);
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            this.Read(br);
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    if (Log.IsEnabled)
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
                return string.Format(CultureInfo.CurrentCulture, "Ver:{0}, Ttl:{1}, Seq:{2}", this.Type, this.Ttl, this.Sequence);
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
                            if (!this.Write(bw))
                                return false;

                            data = ms.ToArray();
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    if (Log.IsEnabled)
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
                            return false;
                    }
                }

                return true;
            }

            #endregion Methods
        }
        #endregion
    }
}