using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using VolumeControl.Core.Helpers;
using VolumeControl.Core.Input.Exceptions;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// API for interacting with the global hotkey functionality exposed in the Win32 API.
    /// </summary>
    public static class WindowsHotkeyAPI
    {
        #region ID
        /// <summary>
        /// The minimum valid hotkey ID number.
        /// </summary>
        /// <remarks>
        /// This is defined by the Win32 API, and cannot be changed or circumvented. There are a total of 45,151 possible hotkey IDs.
        /// </remarks>
        public const ushort MinID = 0x0000;
        /// <summary>
        /// The maximum valid hotkey ID number.
        /// </summary>
        /// <remarks>
        /// This is defined by the Win32 API, and cannot be changed or circumvented. There are a total of 45,151 possible hotkey IDs.
        /// </remarks>
        public const ushort MaxID = 0xBFFF;

        /// <summary>
        /// Gets the next available hotkey ID number.
        /// </summary>
        internal static int NextID
        {
            get
            {
                int id = Interlocked.Increment(ref _nextID);
                if (id >= MaxID)
                    throw new Exception($"Exceeded the maximum number of hotkey IDs ({MaxID - MinID})");
                return id;
            }
        }
        private static int _nextID = MinID - 1; //< start at the index prior to the first valid index
        #endregion ID

        #region P/Invoke
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);
        #endregion P/Invoke

        #region MessageOnlyWindow
        /// <summary>
        /// Windows message type for hotkey messages.
        /// </summary>
        public const int WM_HOTKEY = 0x312;
        /// <summary>
        /// A message-only window wrapper class that allows adding and removing hook handler methods.
        /// </summary>
        class MessageOnlyWindow : IDisposable
        {
            #region Constructor
            internal MessageOnlyWindow() => hWndSource = new(0, 0, 0, 0, 0, 0, 0, null, HWND_MESSAGE);
            #endregion Constructor

            #region Finalizer
            ~MessageOnlyWindow() => this.Dispose();
            #endregion Finalizer

            #region Fields
            /// <summary>
            /// The handle for message-only windows.
            /// </summary>
            static readonly IntPtr HWND_MESSAGE = new(-3);
            /// <summary>
            /// The <see cref="HwndSource"/> class associated with the message-only window used for hotkeys.
            /// </summary>
            readonly HwndSource hWndSource;
            #endregion Fields

            #region Properties
            public IntPtr Handle => hWndSource.Handle;
            #endregion Properties

            #region Methods
            /// <inheritdoc cref="HwndSource.AddHook(HwndSourceHook)"/>
            public void AddHook(HwndSourceHook hook) => hWndSource.AddHook(hook);
            /// <inheritdoc cref="HwndSource.RemoveHook(HwndSourceHook)"/>
            public void RemoveHook(HwndSourceHook hook) => hWndSource.RemoveHook(hook);
            #endregion Methods

            #region IDisposable Implementation
            /// <inheritdoc/>
            public void Dispose()
            {
                hWndSource.Dispose();
                GC.SuppressFinalize(this);
            }
            #endregion IDisposable Implementation
        }
        /// <summary>
        /// The message-only window instance.
        /// </summary>
        static readonly MessageOnlyWindow _messageOnlyWindow = new();
        #endregion MessageOnlyWindow

        #region Methods
        /// <summary>
        /// Registers the specified <paramref name="hotkey"/>.
        /// </summary>
        /// <param name="hotkey">An <see cref="IHotkey"/> instance to register.</param>
        /// <exception cref="HotkeyRegistrationException">Hotkey registration failed.</exception>
        public static void Register(IHotkey hotkey)
        {
            if (RegisterHotKey(_messageOnlyWindow.Handle, hotkey.ID, (uint)hotkey.Modifiers, (uint)KeyInterop.VirtualKeyFromKey((Key)hotkey.Key)) != 0)
                _messageOnlyWindow.AddHook(hotkey.MessageHook);
            else
            { // failure
                (int hr, string msg) = GetWin32Error.GetLastWin32Error();
                throw new HotkeyRegistrationException(hr, msg);
            }
        }
        /// <summary>
        /// Attempts to register the specified <paramref name="hotkey"/>.
        /// </summary>
        /// <param name="hotkey">An <see cref="IHotkey"/> instance to register.</param>
        /// <returns><see langword="true"/> when the <paramref name="hotkey"/> was registered successfully; otherwise <see langword="false"/>.</returns>
        public static bool TryRegister(IHotkey hotkey)
        {
            try
            {
                Register(hotkey);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Unregisters the specified <paramref name="hotkey"/>.
        /// </summary>
        /// <param name="hotkey">An <see cref="IHotkey"/> instance to unregister.</param>
        /// <exception cref="HotkeyRegistrationException">Hotkey unregistration failed.</exception>
        public static void Unregister(IHotkey hotkey)
        {
            if (UnregisterHotKey(_messageOnlyWindow.Handle, hotkey.ID) != 0)
                _messageOnlyWindow.RemoveHook(hotkey.MessageHook);
            else
            { // failure
                (int hr, string msg) = GetWin32Error.GetLastWin32Error();
                throw new HotkeyRegistrationException(hr, msg);
            }
        }
        /// <summary>
        /// Attempts to unregister the specified <paramref name="hotkey"/>.
        /// </summary>
        /// <param name="hotkey">An <see cref="IHotkey"/> instance to unregister.</param>
        /// <returns><see langword="true"/> when the <paramref name="hotkey"/> was unregistered successfully; otherwise <see langword="false"/>.</returns>
        public static bool TryUnregister(IHotkey hotkey)
        {
            try
            {
                Unregister(hotkey);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion Methods
    }
}
