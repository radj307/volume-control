using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using VolumeControl.Log;

namespace HotkeyLib
{
    public static class HotkeyAPI
    {
        #region Members
        public const int MinID = 0x0000, MaxID = 0xBFFF;
        private static int _currentID = MinID;
        #endregion Members

        #region Methods
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

        [Flags]
        public enum FORMAT_MESSAGE : uint
        {
            ALLOCATE_BUFFER = 0x00000100,
            IGNORE_INSERTS = 0x00000200,
            FROM_SYSTEM = 0x00001000,
            ARGUMENT_ARRAY = 0x00002000,
            FROM_HMODULE = 0x00000800,
            FROM_STRING = 0x00000400
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, [Out] StringBuilder lpBuffer, uint nSize, IntPtr Arguments);

        // the version, the sample is built upon:
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer, uint nSize, IntPtr pArguments);

        // the parameters can also be passed as a string array:
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer, uint nSize, string[] Arguments);

        // simplified for usability
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int FormatMessage(
            FORMAT_MESSAGE dwFlags,
            IntPtr lpSource,
            int dwMessageId,
            uint dwLanguageId,
            out StringBuilder msgOut,
            int nSize,
            IntPtr Arguments
        );

        public static int GetLastWin32Error()
            => Marshal.GetLastWin32Error();
        public static string GetLastWin32ErrorString(int err)
        {
            if (err == 0)
                return "";

            StringBuilder msg = new(256);
            _ = FormatMessage(
                FORMAT_MESSAGE.ALLOCATE_BUFFER | FORMAT_MESSAGE.FROM_SYSTEM | FORMAT_MESSAGE.IGNORE_INSERTS,
                IntPtr.Zero,
                err,
                0,
                out msg,
                msg.Capacity,
                IntPtr.Zero
            );
            return msg.ToString().Trim();
        }
        public static string GetLastWin32ErrorString() => GetLastWin32ErrorString(GetLastWin32Error());
        public static (int, string) GetLastError()
        {
            int err = GetLastWin32Error();
            return (err, GetLastWin32ErrorString(err));
        }
        #endregion InteropFunctions

        #region InteropConstants
        public const uint WM_HOTKEY = 0x312;

        public const uint MOD_ALT = 0x1;
        public const uint MOD_CONTROL = 0x2;
        public const uint MOD_SHIFT = 0x4;
        public const uint MOD_WIN = 0x8;

        public enum Error : uint
        {
            HOTKEY_NOT_REGISTERED = 1419,
            HOTKEY_ALREADY_REGISTERED = 1409,
        }

        public const uint ERROR_HOTKEY_ALREADY_REGISTERED = 1409;
        #endregion InteropConstants
    }
}
