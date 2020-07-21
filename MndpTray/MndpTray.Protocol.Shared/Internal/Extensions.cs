namespace MndpTray.Protocol
{
    using System;
    using System.IO;

    internal static class BinaryReaderExtensions
    {
        internal static ushort ReadUInt16Reverse(this BinaryReader self)
        {
            byte[] b = self.ReadBytes(2);
            Array.Reverse(b);

            return BitConverter.ToUInt16(b, 0);
        }

        internal static uint ReadUInt32Reverse(this BinaryReader self)
        {
            byte[] b = self.ReadBytes(4);
            Array.Reverse(b);

            return BitConverter.ToUInt32(b, 0);
        }
    }

    internal static class BinaryWriterExtensions
    {
        internal static void WriteUInt16Reverse(this BinaryWriter self, ushort data)
        {
            byte[] b = BitConverter.GetBytes(data);
            Array.Reverse(b);

            self.Write(b);
        }

        internal static void WriteUInt32Reverse(this BinaryWriter self, uint data)
        {
            byte[] b = BitConverter.GetBytes(data);
            Array.Reverse(b);

            self.Write(b);
        }
    }
}