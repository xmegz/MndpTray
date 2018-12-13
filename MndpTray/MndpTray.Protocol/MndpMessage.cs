using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;

namespace MndpTray.Protocol
{
    /// <summary>
    /// Mikrotik discovery message
    /// </summary>
    public class MndpMessage
    {
        #region Consts

        private const ushort TLV_TYPE_BOARD_NAME = 12;
        private const ushort TLV_TYPE_IDENTITY = 5;
        private const ushort TLV_TYPE_INTERFACE_NAME = 16;
        private const ushort TLV_TYPE_MAC_ADDRESS = 1;
        private const ushort TLV_TYPE_PLATFORM = 8;
        private const ushort TLV_TYPE_SOFTWAREID = 11;
        private const ushort TLV_TYPE_UNPACK = 14;
        private const ushort TLV_TYPE_UPTIME = 10;
        private const ushort TLV_TYPE_VERSION = 7;

        #endregion Consts

        #region Props

        public string BoardName { get; set; }
        public string Identity { get; set; }
        public string InterfaceName { get; set; }
        public string MacAddress { get; set; }
        public string Platform { get; set; }
        public ushort Sequence { get; set; }
        public string SoftwareId { get; set; }
        public byte Ttl { get; set; }
        public byte Type { get; set; }
        public byte Unpack { get; set; }
        public TimeSpan Uptime { get; set; }
        public string Version { get; set; }

        #endregion Props

        #region Methods

        public bool Read(byte[] data)
        {
            try
            {
                var tlvMessage = new TlvMessage();
                bool ok = tlvMessage.Read(data);

                if (!ok) return false;

                var enc = Encoding.GetEncoding(1250);

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

                        default: break;
                    }
                }

                MndpLog.Info("{0} Read,\r\n{1}\r\n", nameof(MndpMessage), this.ToString());

                return true;
            }
            catch (Exception ex)
            {
                MndpLog.Exception(nameof(MndpMessage), nameof(Read), ex);
            }

            return false;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("\t{0}:{1},\r\n ", nameof(this.Type), this.Type);
            sb.AppendFormat("\t{0}:{1},\r\n ", nameof(this.Ttl), this.Ttl);
            sb.AppendFormat("\t{0}:{1},\r\n ", nameof(this.Sequence), this.Sequence);
            sb.AppendFormat("\t{0}:{1},\r\n ", nameof(this.MacAddress), this.MacAddress);
            sb.AppendFormat("\t{0}:{1},\r\n ", nameof(this.Identity), this.Identity);
            sb.AppendFormat("\t{0}:{1},\r\n ", nameof(this.Version), this.Version);
            sb.AppendFormat("\t{0}:{1},\r\n ", nameof(this.Platform), this.Platform);
            sb.AppendFormat("\t{0}:{1},\r\n ", nameof(this.Uptime), this.Uptime.ToString());
            sb.AppendFormat("\t{0}:{1},\r\n ", nameof(this.SoftwareId), this.SoftwareId);
            sb.AppendFormat("\t{0}:{1},\r\n ", nameof(this.BoardName), this.BoardName);
            sb.AppendFormat("\t{0}:{1},\r\n ", nameof(this.Unpack), this.Unpack);
            sb.AppendFormat("\t{0}:{1},\r\n ", nameof(this.InterfaceName), this.InterfaceName);

            return sb.ToString();
        }

        public byte[] Write()
        {
            try
            {
                MndpLog.Info("{0} Write,\r\n{1}\r\n", nameof(MndpMessage), this.ToString());

                var tlvMessage = new TlvMessage()
                {
                    Type = this.Type,
                    Ttl = this.Ttl,
                    Sequence = this.Sequence
                };

                var enc = Encoding.GetEncoding(1250);

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

                        default: break;
                    }
                }

                bool ok = tlvMessage.Write(out byte[] data);

                if (!ok) return null;

                return data;
            }
            catch (Exception ex)
            {
                MndpLog.Exception(nameof(MndpMessage), nameof(Write), ex);
            }

            return null;
        }

        #endregion Methods

        /// <summary>
        /// Type , Length , Value
        /// </summary>
        protected class Tlv
        {
            #region Props

            public ushort Length { get; set; }
            public ushort Type { get; set; }
            public byte[] Value { get; set; }

            #endregion Props

            #region Methods

            public Tlv()
            {
            }

            public Tlv(ushort type, ushort length, byte[] value)
            {
                this.Type = type;
                this.Length = length;
                this.Value = value;
            }

            public Tlv(ushort type, byte[] data) : this(type, (ushort)(data?.Length ?? 0), data)
            {
            }

            public Tlv(ushort type, uint data) : this(type, BitConverter.GetBytes(data))
            {
            }

            public Tlv(ushort type, string data, Encoding enc) : this(type, data != null ? enc.GetBytes(data) : new byte[0])
            {
            }

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
                    MndpLog.Exception(nameof(Tlv), nameof(Read), ex);
                }

                return false;
            }

            public override string ToString()
            {
                var valueHex = "";
                var valueStr = "";

                if (this.Value != null)
                {
                    valueHex = BitConverter.ToString(this.Value).Replace("-", ",");
                    valueStr = Encoding.GetEncoding(1250).GetString(this.Value);
                }

                return String.Format("T:{0}, L:{1}, V:{2}, VS:{3}", this.Type, this.Length, valueHex, valueStr);
            }

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
                    MndpLog.Exception(nameof(Tlv), nameof(Write), ex);
                }

                return false;
            }

            #endregion Methods
        }

        /// <summary>
        /// Message in Tlv collection format
        /// </summary>
        protected class TlvMessage
        {
            #region Props

            public List<Tlv> Items { get; set; } = new List<Tlv>();
            public ushort Sequence { get; set; }
            public byte Ttl { get; set; }
            public byte Type { get; set; }

            #endregion Props

            #region Methods

            public bool Read(BinaryReader br)
            {
                this.Type = br.ReadByte();
                this.Ttl = br.ReadByte();
                this.Sequence = br.ReadUInt16Reverse();

                while (br.BaseStream.Length > br.BaseStream.Position)
                {
                    var item = new Tlv();
                    bool ok = item.Read(br);
                    if (!ok) return false;
                    this.Items.Add(item);
                }

                return true;
            }

            public bool Read(byte[] data)
            {
                try
                {
                    if (data.Length < 8)
                    {
                        MndpLog.Info("{0} Read, Message Too Short", nameof(TlvMessage));
                        return false;
                    }

                    using (MemoryStream ms = new MemoryStream(data))
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
                    MndpLog.Exception(nameof(TlvMessage), nameof(Read), ex);
                }

                return false;
            }

            public override string ToString()
            {
                return String.Format("Ver:{0}, Ttl:{1}, Seq:{2}", this.Type, this.Ttl, this.Sequence);
            }

            public bool Write(out byte[] data)
            {
                data = null;

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (BinaryWriter bw = new BinaryWriter(ms))
                        {
                            bool ok = this.Write(bw);
                            if (!ok) return false;
                            data = ms.ToArray();
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    MndpLog.Exception(nameof(TlvMessage), nameof(Write), ex);
                }

                return false;
            }

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
    }

    /// <summary>
    /// Mikrotik discovery message with extensions
    /// </summary>
    public class MndpMessageEx : MndpMessage, ICloneable
    {
        #region Props

        public double Age { get { return (DateTime.Now - ReceiveDateTime).TotalSeconds; } }
        public string BroadcastAddress { get; set; }
        public DateTime ReceiveDateTime { get; set; }
        public string SenderAddress { get; set; }
        #endregion Props

        #region Methods

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("\t{0}:{1},\r\n", nameof(this.ReceiveDateTime), this.ReceiveDateTime);
            sb.AppendFormat("\t{0}:{1},\r\n", nameof(this.SenderAddress), this.SenderAddress);
            sb.AppendFormat("\t{0}:{1},\r\n", nameof(this.BroadcastAddress), this.BroadcastAddress);
            sb.Append(base.ToString());

            return sb.ToString();
        }

        #endregion Methods
    }
}