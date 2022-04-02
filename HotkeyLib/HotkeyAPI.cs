using System.Runtime.InteropServices;
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
        private static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, Keys vk);
        /// <summary>
        /// This is a wrapper function for the 'RegisterHotKey' function from user32.dll
        /// </summary>
        /// <param name="hWnd">The owner controls handle.</param>
        /// <param name="id">The hotkeys ID.</param>
        /// <param name="fsModifiers">A bitfield defining which (if any) modifiers are required by the hotkey.</param>
        /// <param name="vk">The primary keyboard key.</param>
        /// <returns>True when registration was successful.</returns>
        public static bool RegisterHotkey(IntPtr hWnd, int id, uint fsModifiers, Keys vk)
            => RegisterHotKey(hWnd, id, fsModifiers, vk) != 0;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);
        /// <summary>
        /// This is a wrapper function for the 'UnregisterHotKey' function from user32.dll.
        /// </summary>
        /// <param name="hWnd">The owner controls handle.</param>
        /// <param name="id">The hotkey's ID.</param>
        /// <returns>True when unregistration was successful.</returns>
        public static bool UnregisterHotkey(IntPtr hWnd, int id)
            => UnregisterHotKey(hWnd, id) != 0;

        public static int GetLastWin32Error()
            => Marshal.GetLastWin32Error();
        #endregion InteropFunctions

        #region InteropConstants
        public const uint WM_HOTKEY = 0x312;

        public const uint MOD_ALT = 0x1;
        public const uint MOD_CONTROL = 0x2;
        public const uint MOD_SHIFT = 0x4;
        public const uint MOD_WIN = 0x8;

        public const uint ERROR_HOTKEY_ALREADY_REGISTERED = 1409;
        #endregion InteropConstants
    }
}
