using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using VolumeControl.Core.Helpers;
using VolumeControl.Core.Input.Exceptions;
using VolumeControl.Log;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// API for interacting with the global hotkey functionality exposed in the Win32 API.
    /// </summary>
    internal static class WindowsHotkeyAPI
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
        internal static ushort NextID
        {
            get
            {
                ushort id = Convert.ToUInt16(Interlocked.Increment(ref _nextID));
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
            internal MessageOnlyWindow()
            {
                hWndSource = new(0, 0, 0, 0, 0, 0, 0, null, HWND_MESSAGE);
                hWndSource.Disposed += this.HWndSource_Disposed;

                // make sure the HwndSource is valid and can receive messages
                if (TestHwndSource())
                {
                    FLog.Trace($"[{nameof(WindowsHotkeyAPI)}] Message-only window successfully initialized.");
                }
                else
                {
                    FLog.Critical($"[{nameof(WindowsHotkeyAPI)}] Failed to create a message-only window; hotkeys will not work!");
                }
            }
            #endregion Constructor

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

            #region AddHook
            /// <inheritdoc cref="HwndSource.AddHook(HwndSourceHook)"/>
            public void AddHook(HwndSourceHook hook) => hWndSource.AddHook(hook);
            #endregion AddHook

            #region RemoveHook
            /// <inheritdoc cref="HwndSource.RemoveHook(HwndSourceHook)"/>
            public void RemoveHook(HwndSourceHook hook) => hWndSource.RemoveHook(hook);
            #endregion RemoveHook

            #region TestHwndSource
            private bool TestHwndSource()
            {
                const int WM_NULL = 0;
                bool received = false;

                // create a temporary hook to receive the test message & set received to true
                var testHook = new HwndSourceHook((IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
                {
                    switch (msg)
                    {
                    case WM_NULL:
                        received = true;
                        break;
                    }
                    return IntPtr.Zero;
                });

                hWndSource.AddHook(testHook);
                _ = SendMessage(hWndSource.Handle, WM_NULL, 0, 0);
                hWndSource.RemoveHook(testHook);

                return received;
            }
            #endregion TestHwndSource

            #region (P/Invoke)
            [DllImport("user32.dll")]
            private static extern int SendMessage(IntPtr hWnd, uint Msg, long wParam, long lParam);
            #endregion (P/Invoke)

            #endregion Methods

            #region EventHandlers

            #region HwndSource
            private void HWndSource_Disposed(object? sender, EventArgs e)
            {
                FLog.Trace($"[{nameof(WindowsHotkeyAPI)}] Disposed of message-only window.");
                Dispose();
            }
            #endregion HwndSource

            #endregion EventHandlers

            #region IDisposable Implementation
            ~MessageOnlyWindow() => this.Dispose();
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
        /// Adds the specified <paramref name="hook"/> to the internal Message-Only Window's message hooks.
        /// </summary>
        /// <param name="hook">An object that can accept hotkey messages.</param>
        public static void AddHook(IHotkeyMessageHook hook)
        {
            _messageOnlyWindow.AddHook(hook.MessageHook);
        }
        /// <summary>
        /// Removes the specified <paramref name="hook"/> from the internal Message-Only Window's message hooks.
        /// </summary>
        /// <param name="hook">An object that can accept hotkey messages.</param>
        public static void RemoveHook(IHotkeyMessageHook hook)
        {
            _messageOnlyWindow.RemoveHook(hook.MessageHook);
        }
        /// <summary>
        /// Registers the specified <paramref name="hotkey"/>.
        /// </summary>
        /// <param name="hotkey">An <see cref="IHotkey"/> instance to register.</param>
        /// <exception cref="HotkeyRegistrationException">Hotkey registration failed.</exception>
        public static void Register(IHotkey hotkey)
        {
            if (RegisterHotKey(_messageOnlyWindow.Handle, hotkey.ID, (uint)hotkey.Modifiers, (uint)KeyInterop.VirtualKeyFromKey((Key)hotkey.Key)) != 0)
            {
                AddHook(hotkey);
                if (FLog.FilterEventType(EventType.TRACE))
                    FLog.Trace($"[{nameof(WindowsHotkeyAPI)}] Successfully registered hotkey {hotkey.ID} \"{hotkey.Name}\" ({hotkey.GetStringRepresentation()})");
            }
            else
            { // failure
                (int hr, string msg) = GetWin32Error.GetLastWin32Error();
                var ex = new HotkeyRegistrationException(hr, msg);
                FLog.Error($"[{nameof(WindowsHotkeyAPI)}] An error occurred while registering hotkey {hotkey.ID} \"{hotkey.Name}\" ({hotkey.GetStringRepresentation()}):", ex);
                throw ex;
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
            {
                RemoveHook(hotkey);
                if (FLog.FilterEventType(EventType.TRACE))
                    FLog.Trace($"[{nameof(WindowsHotkeyAPI)}] Successfully unregistered hotkey {hotkey.ID} \"{hotkey.Name}\" ({hotkey.GetStringRepresentation()})");
            }
            else
            { // failure
                (int hr, string msg) = GetWin32Error.GetLastWin32Error();
                var ex = new HotkeyRegistrationException(hr, msg);
                if (FLog.FilterEventType(EventType.TRACE))
                    FLog.Trace($"[{nameof(WindowsHotkeyAPI)}] An exception occurred while unregistering hotkey {hotkey.ID} \"{hotkey.Name}\" ({hotkey.GetStringRepresentation()}):", ex);
                throw ex;
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
