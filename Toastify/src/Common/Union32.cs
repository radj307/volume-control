using System;
using System.Runtime.InteropServices;

namespace Toastify.Common
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    internal struct Union32
    {
        public static readonly Union32 Zero;

        [FieldOffset(0)]
        private readonly int number;

        [FieldOffset(0)]
        public readonly ushort Low;

        [FieldOffset(2)]
        public readonly ushort High;

        public Union32(int number)
        {
            this.Low = 0;
            this.High = 0;
            this.number = number;
        }

        public Union32(ushort high, ushort low)
        {
            this.number = 0;
            this.Low = low;
            this.High = high;
        }

        public int ToInt32() => this.number;

        public uint ToUInt32() => unchecked((uint)this.number);

        public IntPtr ToIntPtr() => (IntPtr)this.number;

        public static IntPtr IntPtr(ushort high, ushort low)
        {
            return new Union32(high, low);
        }

        public static explicit operator int(Union32 union) => union.ToInt32();

        public static explicit operator uint(Union32 union) => union.ToUInt32();

        public static implicit operator IntPtr(Union32 union) => union.ToIntPtr();

        public static implicit operator Union32(int @int) => new Union32(@int);
    }
}