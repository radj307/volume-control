using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Struct
{
    public struct BLOB
    {
        /// <summary>
        /// Length of binary object.
        /// </summary>
        public int Length;
        /// <summary>
        /// Pointer to buffer storing data.
        /// </summary>
        public IntPtr Data;
    }
}
