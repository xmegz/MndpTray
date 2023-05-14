namespace MndpTray.Protocol
{
    using System;
    using System.IO;

    /// <summary>
    /// Binary Reader Extensions
    /// </summary>
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Read UIn16 value reversed byte order
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ushort ReadUInt16Reverse(this BinaryReader self)
        {
            byte[] b = self.ReadBytes(2);
            Array.Reverse(b);

            return BitConverter.ToUInt16(b, 0);
        }

        /// <summary>
        /// Read UInt32 value reversed byte order
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static uint ReadUInt32Reverse(this BinaryReader self)
        {
            byte[] b = self.ReadBytes(4);
            Array.Reverse(b);

            return BitConverter.ToUInt32(b, 0);
        }
    }
}