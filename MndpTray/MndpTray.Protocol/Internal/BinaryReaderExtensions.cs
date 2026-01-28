/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Protocol.Internal
{
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
            ushort value = self.ReadUInt16();
            return (ushort)((value >> 8) | (value << 8));
        }

        /// <summary>
        /// Read UInt32 value reversed byte order
        /// </summary>
        /// <param name="self">self object</param>
        /// <returns>uint value</returns>
        internal static uint ReadUInt32Reverse(this BinaryReader self)
        {
            uint value = self.ReadUInt32(); // Reads 4 bytes in little-endian order
            return (value >> 24) |
                   ((value >> 8) & 0x0000FF00) |
                   ((value << 8) & 0x00FF0000) |
                   (value << 24);
        }
        #endregion
    }
}