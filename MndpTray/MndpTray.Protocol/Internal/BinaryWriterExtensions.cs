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
    /// BinaryWriter extension
    /// </summary>
    internal static class BinaryWriterExtensions
    {
        #region Methods
        /// <summary>
        /// Write UInt16 value reversed byte order
        /// </summary>
        /// <param name="self">self object</param>
        /// <param name="data">ushort data</param>
        internal static void WriteUInt16Reverse(this BinaryWriter self, ushort data)
        {
            byte[] b = BitConverter.GetBytes(data);
            Array.Reverse(b);

            self.Write(b);
        }

        /// <summary>
        /// Write UInt32 value reversed byte order
        /// <param name="self">self object</param>
        /// <param name="data">uint data</param>
        /// </summary>
        internal static void WriteUInt32Reverse(this BinaryWriter self, uint data)
        {
            byte[] b = BitConverter.GetBytes(data);
            Array.Reverse(b);

            self.Write(b);
        }
        #endregion
    }
}