using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Types
{
    /// <summary>
    /// This is a variant type object used by the Win32 API to store values.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct PROPVARIANT
    {
        #region Members
        /// <summary>
        /// Value type tag.
        /// </summary>
        [FieldOffset(0)] public short vt;
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        [FieldOffset(2)] public short wReserved1;
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        [FieldOffset(4)] public short wReserved2;
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        [FieldOffset(6)] public short wReserved3;

        #region VariantTypes
        /// <summary>
        /// VT_I1, Version 1
        /// </summary>
        [FieldOffset(8)] public sbyte cVal;
        /// <summary>
        /// VT_UI1
        /// </summary>
        [FieldOffset(8)] public byte bVal;
        /// <summary>
        /// VT_I2
        /// </summary>
        [FieldOffset(8)] public short iVal;
        /// <summary>
        /// VT_UI2
        /// </summary>
        [FieldOffset(8)] public ushort uiVal;
        /// <summary>
        /// VT_I4
        /// </summary>
        [FieldOffset(8)] public int lVal;
        /// <summary>
        /// VT_UI4
        /// </summary>
        [FieldOffset(8)] public uint ulVal;
        /// <summary>
        /// VT_INT, Version 1
        /// </summary>
        [FieldOffset(8)] public int intVal;
        /// <summary>
        /// VT_UINT, Version 1
        /// </summary>
        [FieldOffset(8)] public uint uintVal;
        /// <summary>
        /// VT_I8
        /// </summary>
        [FieldOffset(8)] public long hVal;
        /// <summary>
        /// VT_UI8
        /// </summary>
        [FieldOffset(8)] public long uhVal;
        /// <summary>
        /// VT_R4
        /// </summary>
        [FieldOffset(8)] public float fltVal;
        /// <summary>
        /// VT_R8
        /// </summary>
        [FieldOffset(8)] public double dblVal;
        /// <summary>
        /// VT_BOOL
        /// </summary>
        [FieldOffset(8)] public short boolVal;
        /// <summary>
        /// VT_ERROR
        /// </summary>
        [FieldOffset(8)] public int scode;
        /// <summary>
        /// VT_CY
        /// </summary>
        [FieldOffset(8)] public CY cyVal;
        /// <summary>
        /// VT_DATE
        /// </summary>
        [FieldOffset(8)] public double date;
        /// <summary>
        /// VT_FILETIME
        /// </summary>
        [FieldOffset(8)] public FILETIME filetime;
        /// <summary>
        /// VT_CLSID
        /// </summary>
        [FieldOffset(8)] public Guid puuid;
        /// <summary>
        /// VT_CF
        /// </summary>
        [FieldOffset(8)] public CLIPDATA pclipdata;
        /// <summary>
        /// VT_BSTR
        /// </summary>
        [FieldOffset(8)] public string bstrVal;
        /// <summary>
        /// VT_BSTRBLOB
        /// </summary>
        [FieldOffset(8)] public BSTRBLOB bstrblobVal;
        /// <summary>
        /// VT_BLOB, VT_BLOBOBJECT
        /// </summary>
        [FieldOffset(8)] public BLOB blob;
        #endregion VariantTypes

        /// <summary>
        /// Accessor <see cref="IntPtr"/> for types that aren't explicitly specified in this structure.
        /// </summary>
        [FieldOffset(8)] private readonly IntPtr rawValue;
        #endregion Members

        /// <summary>
        /// Creates a new PropVariant containing a long value
        /// </summary>
        public static PROPVARIANT FromLong(long value) => new() { vt = (short)VarEnum.VT_I8, hVal = value };

        public VarEnum Type => (VarEnum)vt;
        public object? Value => Type switch
        {
            VarEnum.VT_I1 => bVal,
            VarEnum.VT_I2 => iVal,
            VarEnum.VT_I4 => lVal,
            VarEnum.VT_I8 => hVal,
            VarEnum.VT_INT => iVal,
            VarEnum.VT_UI4 => ulVal,
            VarEnum.VT_UI8 => uhVal,
            VarEnum.VT_LPWSTR => Marshal.PtrToStringUni(rawValue),
            VarEnum.VT_BLOB or VarEnum.VT_VECTOR | VarEnum.VT_UI1 => blob.GetAsArrayOfBytes(),
            VarEnum.VT_CLSID => puuid,
            VarEnum.VT_BOOL => boolVal switch
            {
                -1 => true,
                0 => false,
                _ => rawValue,
            },
            VarEnum.VT_FILETIME => DateTime.FromFileTime(((long)filetime.dwHighDateTime << 32) + filetime.dwLowDateTime),
            _ => rawValue,
        };

        [DllImport("ole32.dll")]
        internal static extern int PropVariantClear(ref PROPVARIANT pVar);

        [DllImport("ole32.dll")]
        internal static extern int PropVariantClear(IntPtr pVar);
    }
}
