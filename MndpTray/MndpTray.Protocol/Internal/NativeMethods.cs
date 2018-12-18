using System;
using System.Runtime.InteropServices;

namespace MndpTray.Protocol
{
    internal static class NativeMethods
    {
        [DllImport("kernel32")]
        internal static extern UInt64 GetTickCount64();
    }
}