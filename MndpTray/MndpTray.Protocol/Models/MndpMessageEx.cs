/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Protocol
{
    using System;
    using System.Text;

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
                    return null;

                StringBuilder sb = new StringBuilder(this.MacAddress.Length*2);

                for (int i = 0; i < this.MacAddress.Length; i++)
                {
                    sb.Append(this.MacAddress[i]);

                    if (i % 2 == 1)
                        sb.Append(':');
                }

                if (sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);

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
            var sb = new StringBuilder(128);

            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.ReceiveDateTime), this.ReceiveDateTime);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.UnicastAddress), this.UnicastAddress);
            sb.AppendFormat("\t{0}:{1}," + Environment.NewLine, nameof(this.BroadcastAddress), this.BroadcastAddress);
            
            sb.Append(base.ToString());

            return sb.ToString();
        }

        #endregion Methods
    }
}