using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;


namespace AudioAPI.WindowsAPI.Struct
{
    /// <summary>
    /// from Propidl.h.
    /// http://msdn.microsoft.com/en-us/library/aa380072(VS.85).aspx
    /// contains a union so we have to do an explicit layout
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct PROPVARIANT
    {
        /// <summary>
        /// Value type tag.
        /// </summary>
        [FieldOffset(0)] public short vt;
        /// <summary>
        /// Reserved1.
        /// </summary>
        [FieldOffset(2)] public short wReserved1;
        /// <summary>
        /// Reserved2.
        /// </summary>
        [FieldOffset(4)] public short wReserved2;
        /// <summary>
        /// Reserved3.
        /// </summary>
        [FieldOffset(6)] public short wReserved3;
        /// <summary>
        /// cVal.
        /// </summary>
        [FieldOffset(8)] public sbyte cVal;
        /// <summary>
        /// bVal.
        /// </summary>
        [FieldOffset(8)] public byte bVal;
        /// <summary>
        /// iVal.
        /// </summary>
        [FieldOffset(8)] public short iVal;
        /// <summary>
        /// uiVal.
        /// </summary>
        [FieldOffset(8)] public ushort uiVal;
        /// <summary>
        /// lVal.
        /// </summary>
        [FieldOffset(8)] public int lVal;
        /// <summary>
        /// ulVal.
        /// </summary>
        [FieldOffset(8)] public uint ulVal;
        /// <summary>
        /// intVal.
        /// </summary>
        [FieldOffset(8)] public int intVal;
        /// <summary>
        /// uintVal.
        /// </summary>
        [FieldOffset(8)] public uint uintVal;
        /// <summary>
        /// hVal.
        /// </summary>
        [FieldOffset(8)] public long hVal;
        /// <summary>
        /// uhVal.
        /// </summary>
        [FieldOffset(8)] public long uhVal;
        /// <summary>
        /// fltVal.
        /// </summary>
        [FieldOffset(8)] public float fltVal;
        /// <summary>
        /// dblVal.
        /// </summary>
        [FieldOffset(8)] public double dblVal;
        //VARIANT_BOOL boolVal;
        /// <summary>
        /// boolVal.
        /// </summary>
        [FieldOffset(8)] public short boolVal;
        /// <summary>
        /// scode.
        /// </summary>
        [FieldOffset(8)] public int scode;
        //CY cyVal;
        //[FieldOffset(8)] private DateTime date; - can cause issues with invalid value
        /// <summary>
        /// Date time.
        /// </summary>
        [FieldOffset(8)] public FILETIME filetime;
        //CLSID* puuid;
        //CLIPDATA* pclipdata;
        //BSTR bstrVal;
        //BSTRBLOB bstrblobVal;
        /// <summary>
        /// Binary large object.
        /// </summary>
        [FieldOffset(8)] public BLOB blobVal;
        //LPSTR pszVal;
        /// <summary>
        /// Pointer value.
        /// </summary>
        [FieldOffset(8)] public IntPtr pointerValue; //LPWSTR 
                                                     //IUnknown* punkVal;
        /*IDispatch* pdispVal;
        IStream* pStream;
        IStorage* pStorage;
        LPVERSIONEDSTREAM pVersionedStream;
        LPSAFEARRAY parray;
        CAC cac;
        CAUB caub;
        CAI cai;
        CAUI caui;
        CAL cal;
        CAUL caul;
        CAH cah;
        CAUH cauh;
        CAFLT caflt;
        CADBL cadbl;
        CABOOL cabool;
        CASCODE cascode;
        CACY cacy;
        CADATE cadate;
        CAFILETIME cafiletime;
        CACLSID cauuid;
        CACLIPDATA caclipdata;
        CABSTR cabstr;
        CABSTRBLOB cabstrblob;
        CALPSTR calpstr;
        CALPWSTR calpwstr;
        CAPROPVARIANT capropvar;
        CHAR* pcVal;
        UCHAR* pbVal;
        SHORT* piVal;
        USHORT* puiVal;
        LONG* plVal;
        ULONG* pulVal;
        INT* pintVal;
        UINT* puintVal;
        FLOAT* pfltVal;
        DOUBLE* pdblVal;
        VARIANT_BOOL* pboolVal;
        DECIMAL* pdecVal;
        SCODE* pscode;
        CY* pcyVal;
        DATE* pdate;
        BSTR* pbstrVal;
        IUnknown** ppunkVal;
        IDispatch** ppdispVal;
        LPSAFEARRAY* pparray;
        PROPVARIANT* pvarVal;
        */

        /// <summary>
        /// Creates a new PropVariant containing a long value
        /// </summary>
        public static PROPVARIANT FromLong(long value)
        {
            return new PROPVARIANT() { vt = (short)VarEnum.VT_I8, hVal = value };
        }

        /// <summary>
        /// Helper method to gets blob data
        /// </summary>
        private byte[] GetBlob()
        {
            var blob = new byte[blobVal.Length];
            Marshal.Copy(blobVal.Data, blob, 0, blob.Length);
            return blob;
        }

        /// <summary>
        /// Interprets a blob as an array of structs
        /// </summary>
        public T[] GetBlobAsArrayOf<T>()
        {
            var blobByteLength = blobVal.Length;
            var singleInstance = (T)Activator.CreateInstance(typeof(T))!;
            var structSize = Marshal.SizeOf(singleInstance);
            if (blobByteLength % structSize != 0)
                throw new InvalidDataException(string.Format("Blob size {0} not a multiple of struct size {1}", blobByteLength, structSize));
            var items = blobByteLength / structSize;
            var array = new T[items];
            for (int n = 0; n < items; n++)
            {
                array[n] = (T)Activator.CreateInstance(typeof(T))!;
                Marshal.PtrToStructure(new IntPtr((long)blobVal.Data + n * structSize), array[n]!);
            }
            return array;
        }

        /// <summary>
        /// Gets the type of data in this PropVariant
        /// </summary>
        public VarEnum DataType => (VarEnum)vt;

        /// <summary>
        /// Property value
        /// </summary>
        public object Value
        {
            get
            {
                VarEnum ve = DataType;
                return ve switch
                {
                    VarEnum.VT_I1 => bVal,
                    VarEnum.VT_I2 => iVal,
                    VarEnum.VT_I4 => lVal,
                    VarEnum.VT_I8 => hVal,
                    VarEnum.VT_INT => iVal,
                    VarEnum.VT_UI4 => ulVal,
                    VarEnum.VT_UI8 => uhVal,
                    VarEnum.VT_LPWSTR => Marshal.PtrToStringUni(pointerValue)!,
                    VarEnum.VT_BLOB or VarEnum.VT_VECTOR | VarEnum.VT_UI1 => GetBlob(),
                    VarEnum.VT_CLSID => Marshal.PtrToStructure<Guid>(pointerValue),
                    VarEnum.VT_BOOL => boolVal switch
                    {
                        -1 => true,
                        0 => false,
                        _ => throw new NotSupportedException("PropVariant VT_BOOL must be either -1 or 0"),
                    },
                    VarEnum.VT_FILETIME => DateTime.FromFileTime(((long)filetime.dwHighDateTime << 32) + filetime.dwLowDateTime),
                    _ => throw new NotImplementedException("PropVariant " + ve),
                };
            }
        }
        public static void Clear(IntPtr ptr)
        {
            _ = PROPVARIANTNative.PropVariantClear(ptr);
        }
    }
    ///// <summary>
    ///// This is a variant type object used by the Win32 API to store values.
    ///// </summary>
    //[StructLayout(LayoutKind.Explicit)]
    //public struct PROPVARIANT
    //{
    //    #region Members
    //    /// <summary>
    //    /// Value type tag.
    //    /// </summary>
    //    [FieldOffset(0)] public short vt;
    //    /// <summary>
    //    /// Reserved for future use.
    //    /// </summary>
    //    [FieldOffset(2)] public short wReserved1;
    //    /// <summary>
    //    /// Reserved for future use.
    //    /// </summary>
    //    [FieldOffset(4)] public short wReserved2;
    //    /// <summary>
    //    /// Reserved for future use.
    //    /// </summary>
    //    [FieldOffset(6)] public short wReserved3;

    //    #region VariantTypes
    //    /// <summary>
    //    /// VT_I1, Version 1
    //    /// </summary>
    //    [FieldOffset(8)] public sbyte cVal;
    //    /// <summary>
    //    /// VT_UI1
    //    /// </summary>
    //    [FieldOffset(8)] public byte bVal;
    //    /// <summary>
    //    /// VT_I2
    //    /// </summary>
    //    [FieldOffset(8)] public short iVal;
    //    /// <summary>
    //    /// VT_UI2
    //    /// </summary>
    //    [FieldOffset(8)] public ushort uiVal;
    //    /// <summary>
    //    /// VT_I4
    //    /// </summary>
    //    [FieldOffset(8)] public int lVal;
    //    /// <summary>
    //    /// VT_UI4
    //    /// </summary>
    //    [FieldOffset(8)] public uint ulVal;
    //    /// <summary>
    //    /// VT_INT, Version 1
    //    /// </summary>
    //    [FieldOffset(8)] public int intVal;
    //    /// <summary>
    //    /// VT_UINT, Version 1
    //    /// </summary>
    //    [FieldOffset(8)] public uint uintVal;
    //    /// <summary>
    //    /// VT_I8
    //    /// </summary>
    //    [FieldOffset(8)] public long hVal;
    //    /// <summary>
    //    /// VT_UI8
    //    /// </summary>
    //    [FieldOffset(8)] public long uhVal;
    //    /// <summary>
    //    /// VT_R4
    //    /// </summary>
    //    [FieldOffset(8)] public float fltVal;
    //    /// <summary>
    //    /// VT_R8
    //    /// </summary>
    //    [FieldOffset(8)] public double dblVal;
    //    /// <summary>
    //    /// VT_BOOL
    //    /// </summary>
    //    [FieldOffset(8)] public short boolVal;
    //    /// <summary>
    //    /// VT_ERROR
    //    /// </summary>
    //    [FieldOffset(8)] public int scode;
    //    /// <summary>
    //    /// VT_CY
    //    /// </summary>
    //    [FieldOffset(8)] public CY cyVal;
    //    /// <summary>
    //    /// VT_DATE
    //    /// </summary>
    //    [FieldOffset(8)] public double date;
    //    /// <summary>
    //    /// VT_FILETIME
    //    /// </summary>
    //    [FieldOffset(8)] public FILETIME filetime;
    //    /// <summary>
    //    /// VT_CLSID
    //    /// </summary>
    //    [FieldOffset(8)] public Guid puuid;
    //    /// <summary>
    //    /// VT_CF
    //    /// </summary>
    //    [FieldOffset(8)] public CLIPDATA pclipdata;
    //    /// <summary>
    //    /// VT_BSTR
    //    /// </summary>
    //    [FieldOffset(8)] public string bstrVal;
    //    /// <summary>
    //    /// VT_BSTRBLOB
    //    /// </summary>
    //    [FieldOffset(8)] public BSTRBLOB bstrblobVal;
    //    /// <summary>
    //    /// VT_BLOB, VT_BLOBOBJECT
    //    /// </summary>
    //    [FieldOffset(8)] public BLOB blob;
    //    #endregion VariantTypes
    //    #endregion Members
    //}
    public static class PROPVARIANTNative
    {
        [DllImport("ole32.dll")]
        internal static extern int PropVariantClear(ref PROPVARIANT pVar);

        [DllImport("ole32.dll")]
        internal static extern int PropVariantClear(IntPtr pVar);
    }
}
