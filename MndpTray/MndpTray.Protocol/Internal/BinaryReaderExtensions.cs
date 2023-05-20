/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Protocol
{
    using System;
    using System.IO;

    /// <summary>
    /// Binary Reader Extensions
    /// </summary>
    internal static class BinaryReaderExtensions
    {
        #region Methods
        /// <summary>
        /// Read UIn16 value reversed byte order
        /// </summary>
        /// <param name="self">self object</param>
        /// <returns>UInt16 value</returns>
        internal static ushort ReadUInt16Reverse(this BinaryReader self)
        {
            byte[] b = self.ReadBytes(2);
            Array.Reverse(b);

            return BitConverter.ToUInt16(b, 0);
        }

        /// <summary>
        /// Read UInt32 value reversed byte order
        /// </summary>
        /// <param name="self">self object</param>
        /// <returns>uint value</returns>
        internal static uint ReadUInt32Reverse(this BinaryReader self)
        {
            byte[] b = self.ReadBytes(4);
            Array.Reverse(b);

            return BitConverter.ToUInt32(b, 0);
        }
        #endregion
    }
}