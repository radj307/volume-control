using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Helpers;
using VolumeControl.Log;

namespace VolumeControl.Core.Input
{
    internal static class WindowsHotkeyAPI
    {
        private static LogWriter Log => FLog.Log;

        #region ID
        public const int MinID = 0x0000;
        public const int MaxID = 0xBFFF;
        public const int TotalUniqueIDs = MaxID - MinID; //< 45,151

        /// <summary>
        /// Gets the next available hotkey ID number.
        /// </summary>
        public static int NextID
        {
            get
            {
                int id = Interlocked.Increment(ref _id);
                if (id >= MaxID)
                {  // if we exceed the max ID, loop back to the min id
                    _ = Interlocked.Exchange(ref _id, MinID);
                    ++OverflowCounter;
                    Log.Error(new OverflowException($"The Hotkey ID sequencer has exceeded 0x{MaxID:X4}, and was reset to 0x{MinID:X4}. ({OverflowCounter} time{(OverflowCounter.Equals(1) ? "" : "s")})"));
                }
                return id;
            }
        }
        private static int _id = MinID;

        /// <summary>
        /// Resets the <see cref="NextID"/> counter and the <see cref="OverflowCounter"/>.
        /// This should be called whenever all hotkeys are guaranteed to have been deleted.
        /// </summary>
        public static void ResetIDs()
        {
            Interlocked.Exchange(ref _id, MinID);
            OverflowCounter = 0;
        }
        #endregion ID

        #region HasOverflown
        /// <summary>
        /// Gets the number of times that <see cref="NextID"/> has exceeded <see cref="MaxID"/> and was reset to <see cref="MinID"/>.
        /// </summary>
        public static uint OverflowCounter { get; private set; } = 0u;
        public static bool HasOverflown => OverflowCounter > 0u;
        #endregion HasOverflown

        #region WindowsAPI
        #region User32
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);
        #endregion User32

        /// <summary>Parent pointer for a message-only window.</summary>
        private static readonly IntPtr HWND_MESSAGE = new(-3);
        internal const int WM_HOTKEY = 0x312;

        /// <summary>
        /// A message-only window wrapper class that allows adding and removing hook handler methods.
        /// </summary>
        private class MessageOnlyWindow : IDisposable
        {
            internal MessageOnlyWindow() => hWndSource = new(0, 0, 0, 0, 0, 0, 0, null, HWND_MESSAGE);
            ~MessageOnlyWindow() => this.Dispose();

            /// <summary>The <see cref="HwndSource"/> class associated with the message-only window used for hotkeys.</summary>
            private readonly HwndSource hWndSource;

            public IntPtr Handle => hWndSource.Handle;

            /// <inheritdoc cref="HwndSource.AddHook(HwndSourceHook)"/>
            public void AddHook(HwndSourceHook hook) => hWndSource.AddHook(hook);
            /// <inheritdoc cref="HwndSource.RemoveHook(HwndSourceHook)"/>
            public void RemoveHook(HwndSourceHook hook) => hWndSource.RemoveHook(hook);

            /// <inheritdoc/>
            public void Dispose()
            {
                hWndSource.Dispose();
                GC.SuppressFinalize(this);
            }
        }
        /// <summary>
        /// The message-only window instance used for hotkeys.
        /// </summary>
        private static readonly MessageOnlyWindow messageOnlyWindow = new();

        /// <inheritdoc cref="KeyInterop.VirtualKeyFromKey(Key)"/>
        private static uint ToVirtualKey(EFriendlyKey key) => (uint)KeyInterop.VirtualKeyFromKey((Key)key);

        /// <summary>
        /// Registers <paramref name="hk"/> with the windows API.
        /// </summary>
        /// <param name="hk">The <see cref="Hotkey"/> to register.</param>
        /// <returns><see langword="true"/> when successful; otherwise <see langword="false"/></returns>
        public static bool Register(Hotkey hk)
        {
            int rc = RegisterHotKey(messageOnlyWindow.Handle, hk.ID, (uint)hk.Modifier, ToVirtualKey(hk.Key));
            switch (rc)
            {
            case 0:
                (int hr, string msg) = GetWin32Error.GetLastWin32Error();
                Log.Error($"Hotkey {hk.ID} registration failed:  '{msg}' (HRESULT: {hr})");
                hk.SetError(msg);
                return false;
            default:
                hk.UnsetError();
                messageOnlyWindow.AddHook(hk.MessageHook);
                return true;
            }
        }
        /// <summary>
        /// Unregisters <paramref name="hk"/> with the windows API.
        /// </summary>
        /// <param name="hk">The <see cref="Hotkey"/> to unregister.</param>
        /// <returns><see langword="true"/> when successful; otherwise <see langword="false"/></returns>
        public static bool Unregister(Hotkey hk)
        {
            messageOnlyWindow.RemoveHook(hk.MessageHook);
            int rc = UnregisterHotKey(messageOnlyWindow.Handle, hk.ID);
            switch (rc)
            {
            case 0:
                (int hr, string msg) = GetWin32Error.GetLastWin32Error();
                Log.Error($"Hotkey {hk.ID} unregistration failed:  '{msg}' (HRESULT: {hr})");
                if (!hr.Equals(1419)) // "Hot key is not registered."
                    hk.SetError(msg);
                return false;
            default:
                hk.UnsetError();
                return true;
            }
        }
        /// <summary>
        /// Reregisters <paramref name="hk"/> with the windows API.
        /// </summary>
        /// <param name="hk">The <see cref="Hotkey"/> to reregister.</param>
        /// <returns><see langword="true"/> when registration was successful; otherwise <see langword="false"/></returns>
        public static bool Reregister(Hotkey hk)
        {
            _ = Unregister(hk);
            return Register(hk);
        }
        #endregion WindowsAPI
    }
}
