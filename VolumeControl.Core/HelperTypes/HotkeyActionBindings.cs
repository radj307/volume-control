using AudioAPI.WindowsAPI;
using AudioAPI.WindowsAPI.Enum;
using HotkeyLib;
using System.ComponentModel;
using System.Reflection;

namespace VolumeControl.Core.HelperTypes
{
    namespace Experimental
    {
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        public class HandlerAttribute : Attribute
        {
            public HandlerAttribute(bool isNoneHandler = false)
            {
                if (IsNoneHandler = isNoneHandler && ++_noneCount > 1)
                    throw new Exception("Multiple 'None' actions are not allowed.");
            }
            public bool IsNoneHandler { get; }
            private static int _noneCount;
        }

        public class ActionBindings
        {
            /// <summary>
            /// Binding endpoint object that uses the <see cref="MethodInfo"/> object to invoke a method using reflection.
            /// </summary>
            public struct ActionBinding
            {
                /// <summary>
                /// 
                /// </summary>
                /// <param name="mInfo">The <see cref="MethodInfo"/> associated with the target action method.</param>
                /// <param name="handlers">The <see cref="ActionHandlers"/> object </param>
                public ActionBinding(MethodInfo mInfo, object? handlerObj)
                {
                    Name = mInfo.Name;
                    MethodInfo = mInfo;
                    HandlerObject = handlerObj;
                }

                public string Name { get; }
                public readonly MethodInfo MethodInfo;
                public readonly object? HandlerObject;

                public void HandleKeyEvent(object? sender, HandledEventArgs? e) => MethodInfo.Invoke(HandlerObject, new[] { sender, e });
            }
            public ActionBindings(IntPtr mainWindowHandle, AudioAPI audioAPI)
            {
                Handlers = new(mainWindowHandle, audioAPI);

                Bindings = new();
                foreach (MethodInfo m in typeof(ActionHandlers).GetMethods().Where(m => m.CustomAttributes.Any(a => a.AttributeType.Equals(typeof(HandlerAttribute)))))
                {
                    Bindings.Add(m.Name, new ActionBinding(m, Handlers));
                }
            }
            internal ActionHandlers Handlers;
            /// <summary>
            /// This is the default handler that does nothing.
            /// </summary>
            /// <remarks>The first handler function with <see cref="HandlerAttribute.IsNoneHandler"/> set to true is always used.</remarks>
            public readonly ActionBinding None;
            public Dictionary<string, ActionBinding> Bindings { get; internal set; }
            public KeyEventHandler this[string actionName]
            {
                get
                {
                    if (!Bindings.ContainsKey(actionName))
                        return Handlers.None;
                    return Bindings[actionName].HandleKeyEvent;
                }
            }

            public List<string> GetActionNames() => Bindings.Select(i => i.Key).ToList();
        }
        public class ActionHandlers
        {
            public ActionHandlers(IntPtr mainWindowHandle, AudioAPI audioAPI)
            {
                MainHWnd = mainWindowHandle;
                AudioAPI = audioAPI;
            }

            private IntPtr MainHWnd { get; }
            private AudioAPI AudioAPI { get; }

            #region Actions
            [Handler(true)] public void None(object? sender, HandledEventArgs e) { }
            [Handler] public void VolumeUp(object? sender, HandledEventArgs e) => AudioAPI.IncrementSessionVolume();
            [Handler] public void VolumeDown(object? sender, HandledEventArgs e) => AudioAPI.DecrementSessionVolume();
            [Handler] public void ToggleMute(object? sender, HandledEventArgs e) => AudioAPI.ToggleSessionMute();
            [Handler] public void NextTrack(object? sender, HandledEventArgs e) => SendKeyboardEvent(EVirtualKeyCode.VK_MEDIA_NEXT_TRACK);
            [Handler] public void PreviousTrack(object? sender, HandledEventArgs e) => SendKeyboardEvent(EVirtualKeyCode.VK_MEDIA_PREV_TRACK);
            [Handler] public void TogglePlayback(object? sender, HandledEventArgs e) => SendKeyboardEvent(EVirtualKeyCode.VK_MEDIA_PLAY_PAUSE);
            [Handler] public void NextTarget(object? sender, HandledEventArgs e) => AudioAPI.SelectNextSession();
            [Handler] public void PreviousTarget(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousSession();
            [Handler] public void ToggleTargetLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = !AudioAPI.LockSelectedSession;
            [Handler] public void NextDevice(object? sender, HandledEventArgs e) => AudioAPI.SelectNextDevice();
            [Handler] public void PreviousDevice(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousDevice();
            [Handler] public void ToggleDeviceLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedDevice = !AudioAPI.LockSelectedDevice;
            [Handler] public void BringToForeground(object? sender, HandledEventArgs e) => User32.SetWindowPos(MainHWnd, User32.HWND_TOP, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
            [Handler] public void SendToBackground(object? sender, HandledEventArgs e) => User32.SetWindowPos(MainHWnd, User32.HWND_BOTTOM, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
            [Handler] public void Minimize(object? sender, HandledEventArgs e) => User32.ShowWindow(MainHWnd, User32.ECmdShow.SW_MINIMIZE);
            [Handler] public void UnMinimize(object? sender, HandledEventArgs e) => User32.ShowWindow(MainHWnd, User32.ECmdShow.SW_RESTORE);
            #endregion Actions

            #region Statics
            private static void SendKeyboardEvent(EVirtualKeyCode vk, byte scanCode = 0xAA, byte flags = 1) => User32.KeyboardEvent(vk, scanCode, flags, IntPtr.Zero);
            #endregion Statics
        }
    }

    /// <summary>
    /// This enumerator defines the various types of actions that hotkeys may be configured to perform.
    /// </summary>
    /// <remarks>
    /// To add additional hotkey actions, follow this process:<br/>
    /// <list type="number" start="1">
    /// <item><description>Add a new value to the <see cref="EHotkeyAction"/> enumerator with a descriptive name.</description></item>
    /// <item><description>Create a <see cref="HotkeyLib.KeyEventHandler"/> that will be called whenever a bound hotkey is pressed within the <see cref="HotkeyActionBindings"/> class.<br/>The method must have a signature similar to the following:
    /// <code language="cs">
    /// void MyHandlerName(<see cref="object?"/> sender, <see cref="System.ComponentModel.HandledEventArgs"/> e){}
    /// </code>
    /// </description></item>
    /// <item><description>Add a new entry to <see cref="HotkeyActionBindings.Bindings"/>.<br/>The key is the <see cref="EHotkeyAction"/> enum you added in step 1.<br/>The value is the handler you created in step 2.</description></item>
    /// </list>
    /// </remarks>
    public enum EHotkeyAction : byte
    {
        None,
        VolumeUp,
        VolumeDown,
        ToggleMute,
        NextTrack,
        PreviousTrack,
        TogglePlayback,
        NextTarget,
        PreviousTarget,
        ToggleTargetLock,
        NextDevice,
        PreviousDevice,
        ToggleDeviceLock,
        BringToForeground,
        SendToBackground,
        Minimize,
        UnMinimize,
    }
    /// <summary>
    /// Contains hotkey event handlers and methods to access them.
    /// </summary>
    /// <remarks>
    /// To add additional hotkey actions, follow this process:<br/>
    /// <list type="number" start="1">
    /// <item><description>Add a new value to the <see cref="EHotkeyAction"/> enumerator with a descriptive name.</description></item>
    /// <item><description>Create a <see cref="HotkeyLib.KeyEventHandler"/> that will be called whenever a bound hotkey is pressed within the <see cref="HotkeyActionBindings"/> class.<br/>The method must have a signature similar to the following:
    /// <code language="cs">
    /// void MyHandlerName(<see cref="object?"/> sender, <see cref="HandledEventArgs"/> e){}
    /// </code>
    /// </description></item>
    /// <item><description>Add a new entry to <see cref="Bindings"/>.<br/>The key is the <see cref="EHotkeyAction"/> enum you added in step 1.<br/>The value is the handler you created in step 2.</description></item>
    /// </list>
    /// </remarks>
    public class HotkeyActionBindings
    {
        #region Constructors
        public HotkeyActionBindings(IntPtr hWndMain, AudioAPI audioAPI)
        {
            MainHWnd = hWndMain;
            AudioAPI = audioAPI;

            Bindings = new()
            {
                { EHotkeyAction.None, null },
                { // VOLUME UP
                    EHotkeyAction.VolumeUp,
                    IncreaseVolume
                },
                { // VOLUME DOWN
                    EHotkeyAction.VolumeDown,
                    DecreaseVolume
                },
                { // TOGGLE MUTE
                    EHotkeyAction.ToggleMute,
                    ToggleMute
                },
                { // NEXT TRACK
                    EHotkeyAction.NextTrack,
                    NextTrack
                },
                { // PREVIOUS TRACK
                    EHotkeyAction.PreviousTrack,
                    PreviousTrack
                },
                { // TOGGLE PLAYBACK
                    EHotkeyAction.TogglePlayback,
                    TogglePlayback
                },
                {
                    EHotkeyAction.NextTarget,
                    NextTarget
                },
                {
                    EHotkeyAction.PreviousTarget,
                    PreviousTarget
                },
                {
                    EHotkeyAction.ToggleTargetLock,
                    ToggleTargetLock
                },
                {
                    EHotkeyAction.NextDevice,
                    NextDevice
                },
                {
                    EHotkeyAction.PreviousDevice,
                    PreviousDevice
                },
                {
                    EHotkeyAction.ToggleDeviceLock,
                    ToggleDeviceLock
                },
                {
                    EHotkeyAction.BringToForeground,
                    BringToForeground
                },
                {
                    EHotkeyAction.SendToBackground,
                    SendToBackground
                },
                {
                    EHotkeyAction.Minimize,
                    Minimize
                },
                {
                    EHotkeyAction.UnMinimize,
                    UnMinimize
                }
            };
        }
        #endregion Constructors

        #region Properties
        private IntPtr MainHWnd { get; }
        private AudioAPI AudioAPI { get; }
        public Dictionary<EHotkeyAction, KeyEventHandler?> Bindings { get; set; }
        public HotkeyLib.KeyEventHandler? this[EHotkeyAction action]
        {
            get => Bindings[action];
            set => Bindings[action] = value;
        }
        #endregion Properties

        #region Statics
        private static void SendKeyboardEvent(EVirtualKeyCode vk, byte scanCode = 0xAA, byte flags = 1) => User32.KeyboardEvent(vk, scanCode, flags, IntPtr.Zero);
        #endregion Statics

        #region Actions
        private void IncreaseVolume(object? sender, HandledEventArgs e) => AudioAPI.IncrementSessionVolume();
        private void DecreaseVolume(object? sender, HandledEventArgs e) => AudioAPI.DecrementSessionVolume();
        private void ToggleMute(object? sender, HandledEventArgs e) => AudioAPI.ToggleSessionMute();
        private void NextTrack(object? sender, HandledEventArgs e) => SendKeyboardEvent(EVirtualKeyCode.VK_MEDIA_NEXT_TRACK);
        private void PreviousTrack(object? sender, HandledEventArgs e) => SendKeyboardEvent(EVirtualKeyCode.VK_MEDIA_PREV_TRACK);
        private void TogglePlayback(object? sender, HandledEventArgs e) => SendKeyboardEvent(EVirtualKeyCode.VK_MEDIA_PLAY_PAUSE);
        private void NextTarget(object? sender, HandledEventArgs e) => AudioAPI.SelectNextSession();
        private void PreviousTarget(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousSession();
        private void ToggleTargetLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = !AudioAPI.LockSelectedSession;
        private void NextDevice(object? sender, HandledEventArgs e) => AudioAPI.SelectNextDevice();
        private void PreviousDevice(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousDevice();
        private void ToggleDeviceLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedDevice = !AudioAPI.LockSelectedDevice;
        private void BringToForeground(object? sender, HandledEventArgs e) => User32.SetWindowPos(MainHWnd, User32.HWND_TOP, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
        private void SendToBackground(object? sender, HandledEventArgs e) => User32.SetWindowPos(MainHWnd, User32.HWND_BOTTOM, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
        private void Minimize(object? sender, HandledEventArgs e) => User32.ShowWindow(MainHWnd, User32.ECmdShow.SW_MINIMIZE);
        private void UnMinimize(object? sender, HandledEventArgs e) => User32.ShowWindow(MainHWnd, User32.ECmdShow.SW_RESTORE);
        #endregion Actions
    }
}
