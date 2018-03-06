using System;
using System.Runtime.InteropServices;

namespace Toastify.Common
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    internal struct Union32 : IEquatable<Union32>, IComparable<Union32>, IComparable
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

        #region Equals / GetHashCode / operator== / operator!=

        /// <inheritdoc />
        public bool Equals(Union32 other)
        {
            return this.number == other.number;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return obj is Union32 && this.Equals((Union32)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.number.GetHashCode();
        }

        /// <inheritdoc />
        public static bool operator ==(Union32 left, Union32 right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public static bool operator !=(Union32 left, Union32 right)
        {
            return !left.Equals(right);
        }

        #endregion Equals / GetHashCode / operator== / operator!=

        #region CompareTo / operator< / operator> / operator<= / operator>=

        /// <inheritdoc />
        public int CompareTo(Union32 other)
        {
            return this.number.CompareTo(other.number);
        }

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (!(obj is Union32))
                throw new ArgumentException($"Object must be of type {nameof(Union32)}");
            return this.CompareTo((Union32)obj);
        }

        /// <inheritdoc />
        public static bool operator <(Union32 left, Union32 right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <inheritdoc />
        public static bool operator >(Union32 left, Union32 right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <inheritdoc />
        public static bool operator <=(Union32 left, Union32 right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <inheritdoc />
        public static bool operator >=(Union32 left, Union32 right)
        {
            return left.CompareTo(right) >= 0;
        }

        #endregion CompareTo / operator< / operator> / operator<= / operator>=

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.number} (0x{this.High:X4} | 0x{this.Low:X4})";
        }
    }
}