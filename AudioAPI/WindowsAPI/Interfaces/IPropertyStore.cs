using AudioAPI.WindowsAPI.Struct;
using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Interfaces
{
    [Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyStore
    {
        [PreserveSig]
        public int GetCount(out int propCount);
        [PreserveSig]
        public int GetAt(int property, out PROPERTYKEY key);
        [PreserveSig]
        public int GetValue(ref PROPERTYKEY key, out PROPVARIANT value);
        [PreserveSig]
        public int SetValue(ref PROPERTYKEY key, ref IntPtr value);
        [PreserveSig]
        public int Commit();
    }
}
