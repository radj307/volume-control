using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Types
{
    [StructLayout(LayoutKind.Explicit)]
    public struct BLOB
    {
        [FieldOffset(0)] public uint cbSize;
        [FieldOffset(4)] public IntPtr pBlobData; //< byte[]

        public bool FitsType(int byteSize) => cbSize % byteSize == 0;
        public bool FitsType(Type type) => FitsType(Marshal.SizeOf(type));
        public bool FitsType<T>() => FitsType(typeof(T));

        /// <summary>
        /// Interprets a blob as an array of structs
        /// </summary>
        public T[] GetAsArrayOf<T>()
        {
            uint outSize = (uint)Marshal.SizeOf(Activator.CreateInstance(typeof(T))!);

            uint remainder = cbSize % outSize;
            if (remainder != 0)
                throw new InvalidCastException($"Cannot cast blob with size {cbSize} to type '{typeof(T).FullName}' with size {outSize}, because there would be {remainder} unused bytes!");

            uint itemCount = cbSize / outSize;
            var array = new T[itemCount];

            for (uint i = 0; i < itemCount; ++i)
            {
                array[i] = (T)Activator.CreateInstance(typeof(T))!;
                Marshal.PtrToStructure<T>(new IntPtr((long)pBlobData + i * outSize));
            }
            return array;
        }
        public byte[] GetAsArrayOfBytes() => GetAsArrayOf<byte>();
    }
}
