using System;
using System.IO;

namespace MndpTray.Protocol
{
    internal static class BinaryReaderExtensions
    {
        public static UInt16 ReadUInt16Reverse(this BinaryReader self)
        {
            byte[] b = self.ReadBytes(2);
            Array.Reverse(b);

            return BitConverter.ToUInt16(b, 0);
        }

        public static UInt32 ReadUInt32Reverse(this BinaryReader self)
        {
            byte[] b = self.ReadBytes(4);
            Array.Reverse(b);

            return BitConverter.ToUInt32(b, 0);
        }
    }

    internal static class BinaryWriterExtensions
    {
        public static void WriteUInt16Reverse(this BinaryWriter self, UInt16 data)
        {
            byte[] b = BitConverter.GetBytes(data);
            Array.Reverse(b);

            self.Write(b);
        }

        public static void WriteUInt32Reverse(this BinaryWriter self, UInt32 data)
        {
            byte[] b = BitConverter.GetBytes(data);
            Array.Reverse(b);

            self.Write(b);
        }
    }
}