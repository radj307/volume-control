using System.Runtime.InteropServices;
using System.Text;

namespace VolumeControl.Core.Helpers
{
    /// <summary>
    /// Exposes the Win32 FormatMessage &amp; GetLastError functions.
    /// </summary>
    public static class GetWin32Error
    {
        #region FormatMessage
        /// <summary>
        /// Possible flags to pass to the <see cref="FormatMessage(FORMAT_MESSAGE, IntPtr, int, uint, out StringBuilder, int, IntPtr)"/> method.
        /// </summary>
        [Flags]
        public enum FORMAT_MESSAGE : uint
        {
            /// <summary/>
            ALLOCATE_BUFFER = 0x00000100,
            /// <summary/>
            IGNORE_INSERTS = 0x00000200,
            /// <summary/>
            FROM_SYSTEM = 0x00001000,
            /// <summary/>
            ARGUMENT_ARRAY = 0x00002000,
            /// <summary/>
            FROM_HMODULE = 0x00000800,
            /// <summary/>
            FROM_STRING = 0x00000400
        }
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int FormatMessage(FORMAT_MESSAGE dwFlags, IntPtr lpSource, int dwMessageId, uint dwLanguageId, out StringBuilder msgOut, int nSize, IntPtr Arguments);
        #endregion FormatMessage

        #region GetLastError
        /// <summary>Gets the last error to have occurred from within the unmanaged C++ Windows API.</summary>
        /// <returns>An <see cref="int"/> representing the HResult of the last error.</returns>
        public static int GetLastError()
            => Marshal.GetLastWin32Error();
        /// <summary>
        /// Gets an error message from the given <paramref name="err"/> HResult.<br/>
        /// This is a convenience method that calls <see cref="FormatMessage(FORMAT_MESSAGE, IntPtr, int, uint, out StringBuilder, int, IntPtr)"/>.
        /// </summary>
        /// <param name="err">The HResult error ID to format into a string.</param>
        /// <returns>The string error message associated with HResult <paramref name="err"/>.</returns>
        public static string FormatError(int err)
        {
            if (err == 0)
                return "";
            _ = FormatMessage(
                FORMAT_MESSAGE.ALLOCATE_BUFFER | FORMAT_MESSAGE.FROM_SYSTEM | FORMAT_MESSAGE.IGNORE_INSERTS,
                IntPtr.Zero,
                err,
                0,
                out StringBuilder msg,
                256,
                IntPtr.Zero
            );
            return msg.ToString().Trim();
        }
        /// <summary>
        /// Gets the last error message to have occurred from within the unmanaged C++ Windows API, as a string.
        /// </summary>
        /// <returns>A string error message.</returns>
        public static string GetLastErrorString() => FormatError(GetLastError());
        /// <summary>
        /// Gets both the last HResult error ID and the associated string error message for the last error message to have occurred from within the unmanaged C++ Windows API.
        /// </summary>
        /// <returns>HResult and string error message as a tuple.</returns>
        public static (int hresult, string message) GetLastWin32Error()
        {
            int err = GetLastError();
            return (err, FormatError(err));
        }
        #endregion GetLastError

    }
}
