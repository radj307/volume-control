using System;
using System.Runtime.InteropServices;

namespace ToastifyAPI.Native.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DataBlob : IEquatable<DataBlob>
    {
        public int cbData;
        public IntPtr pbData;

        /// <inheritdoc />
        public bool Equals(DataBlob other)
        {
            return this.cbData == other.cbData &&
                   this.pbData.Equals(other.pbData);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            return obj is DataBlob blob && this.Equals(blob);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.cbData * 397) ^ this.pbData.GetHashCode();
            }
        }
    }
}