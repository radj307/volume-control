using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
namespace ToastifyAPI.Native.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct ShStockIconInfo : IEquatable<ShStockIconInfo>
    {
        public UInt32 cbSize;
        public IntPtr hIcon;
        public Int32 iSysIconIndex;
        public Int32 iIcon;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szPath;

        /// <inheritdoc />
        public bool Equals(ShStockIconInfo other)
        {
            return this.cbSize == other.cbSize &&
                   this.hIcon.Equals(other.hIcon) &&
                   this.iSysIconIndex == other.iSysIconIndex &&
                   this.iIcon == other.iIcon &&
                   string.Equals(this.szPath, other.szPath);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            return obj is ShStockIconInfo info && this.Equals(info);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)this.cbSize;
                hashCode = (hashCode * 397) ^ this.hIcon.GetHashCode();
                hashCode = (hashCode * 397) ^ this.iSysIconIndex;
                hashCode = (hashCode * 397) ^ this.iIcon;
                hashCode = (hashCode * 397) ^ (this.szPath != null ? this.szPath.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}