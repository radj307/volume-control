using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HotkeyLib
{
    /// <summary>Contains all of the raw windows API calls and types, and exposes convenience methods for usage.</summary>
    public static class HotkeyAPI
    {
        #region Members
        /// <summary>The minimum allowable hotkey ID number.</summary>
        public const int MinID = 0x0000;
        /// <summary>The maximum allowable hotkey ID number. </summary>
        public const int MaxID = 0xBFFF;
        private static int _currentID = MinID;
        #endregion Members

        #region Methods
        /// <summary>
        /// Gets a <i>mostly unique</i> ID number.<br/>
        /// IDs will only repeat once <see cref="MaxID"/> is reached; this is a limit of the Windows API.
        /// </summary>
        /// <returns>A hotkey ID number ready for use.</returns>
        public static int GetID()
        {
            if (_currentID + 1 >= MaxID) // loop back around
                _currentID = MinID;
            _currentID++;
            return _currentID;
        }
        #endregion Methods

        #region InteropFunctions
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        /// <summary>
        /// This is a wrapper function for the 'RegisterHotKey' function from user32.dll
        /// </summary>
        /// <param name="hWnd">The owner window's handle.</param>
        /// <param name="id">The hotkeys ID.</param>
        /// <param name="fsModifiers">A bitfield defining which (if any) modifiers are required by the hotkey.</param>
        /// <param name="vk">The primary keyboard key.</param>
        /// <returns>True when registration was successful.</returns>
        public static bool RegisterHotkey(IntPtr hWnd, int id, uint fsModifiers, Keys vk)
            => RegisterHotKey(hWnd, id, fsModifiers, Convert.ToUInt32(vk)) != 0;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);
        /// <summary>
        /// This is a wrapper function for the 'UnregisterHotKey' function from user32.dll.
        /// </summary>
        /// <param name="hWnd">The owner window's handle.</param>
        /// <param name="id">The hotkey's ID.</param>
        /// <returns>True when unregistration was successful.</returns>
        public static bool UnregisterHotkey(IntPtr hWnd, int id)
            => UnregisterHotKey(hWnd, id) != 0;
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
        private static extern int FormatMessage(
            FORMAT_MESSAGE dwFlags,
            IntPtr lpSource,
            int dwMessageId,
            uint dwLanguageId,
            out StringBuilder msgOut,
            int nSize,
            IntPtr Arguments
        );

        /// <summary>Gets the last error to have occurred from within the unmanaged C++ Windows API.</summary>
        /// <returns>An <see cref="Int32"/> representing the HResult of the last error.</returns>
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
        #endregion InteropFunctions

        #region InteropConstants
        /// <summary>
        /// Windows Message Hotkey
        /// </summary>
        public const uint WM_HOTKEY = 0x312;
        /// <summary>
        /// Alt Modifier bitfield flag value.
        /// </summary>
        public const uint MOD_ALT = 0x1;
        /// <summary>
        /// Ctrl modifier bitfield flag value.
        /// </summary>
        public const uint MOD_CONTROL = 0x2;
        /// <summary>
        /// Shift modifier bitfield flag value.
        /// </summary>
        public const uint MOD_SHIFT = 0x4;
        /// <summary>
        /// Windows modifier bitfield flag value.
        /// </summary>
        public const uint MOD_WIN = 0x8;
        #endregion InteropConstants
    }
}
