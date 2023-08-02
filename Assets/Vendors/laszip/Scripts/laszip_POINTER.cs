using System;
using System.Runtime.InteropServices;

namespace Vendors.laszip.Scripts
{
    [StructLayout(LayoutKind.Sequential)]
    public struct laszip_POINTER
    {
        public IntPtr ptr;
    }
}