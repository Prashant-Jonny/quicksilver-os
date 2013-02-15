using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quicksilver.Filesystem.Praxis
{
    unsafe class UnsignedWriter
    {
        byte* ptr;
        public UnsignedWriter(byte* pointer)
        {
            ptr = pointer;
        }
        public void Write64(ulong u) {
            *((ulong*)ptr) = *((ulong*)u);
            ptr += 8;
        }
        public void Write32(uint i) {
            *((uint*)ptr) = *((uint*)i);
            ptr += 4;
        }
        public void Write16(ushort s) {
            *((ushort*)ptr) = *((ushort*)s);
            ptr += 4;
        }
        public void Advance(uint length) {
            ptr += length;
        }
    }
}
