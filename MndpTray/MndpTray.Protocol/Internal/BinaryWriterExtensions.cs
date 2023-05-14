namespace MndpTray.Protocol
{
    using System;
    using System.IO;

    /// <summary>
    /// BinaryWriter extension
    /// </summary>
    public static class BinaryWriterExtensions
    {
        /// <summary>
        /// Write UInt16 value reversed byte order
        /// </summary>
        /// <param name="self"></param>
        /// <param name="data"></param>
        public static void WriteUInt16Reverse(this BinaryWriter self, ushort data)
        {
            byte[] b = BitConverter.GetBytes(data);
            Array.Reverse(b);

            self.Write(b);
        }

        /// <summary>
        /// Write UInt32 value reversed byte order
        /// </summary>
        public static void WriteUInt32Reverse(this BinaryWriter self, uint data)
        {
            byte[] b = BitConverter.GetBytes(data);
            Array.Reverse(b);

            self.Write(b);
        }
    }
}