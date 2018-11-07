using System;
using System.IO;
using System.Reflection;

namespace MndpTray.Protocol
{
    public static class AssembyExtension
    {
        public static string GetLocationWithExtension(this Assembly self, string extension)
        {
            string dir = Path.GetDirectoryName(self.Location);
            string file = Path.GetFileNameWithoutExtension(self.Location);

            file = Path.Combine(dir, file);
            return file + "." + extension;
        }
    }

    public static class BinaryReaderExtensions
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

    public static class BinaryWriterExtensions
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