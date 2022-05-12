using HotkeyLib;
using System.ComponentModel;
using System.Reflection;
using VolumeControl.Core.HelperTypes.Enum;

namespace VolumeControl.Core.HelperTypes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HandlerAttribute : Attribute
    {
        public HandlerAttribute(bool isNoneHandler = false)
        {
            if (IsNoneHandler = isNoneHandler)
                IncrementNoneCount();
        }
        public HandlerAttribute(string actionName, bool isNoneHandler = false)
        {
            ActionName = actionName;
            if (IsNoneHandler = isNoneHandler)
                IncrementNoneCount();
        }
        public bool IsNoneHandler { get; } = false;
        /// <summary>
        /// This overrides the default action name shown in the action dropdown.<br/>
        /// If this is set to null, the method name is used by default.
        /// </summary>
        public string? ActionName { get; } = null;

        private static void IncrementNoneCount()
        {
            if (++_noneCount > 1)
                throw new Exception("Multiple 'None' actions are not allowed.");
        }
        private static int _noneCount;
    }

    public class ActionBindings
    {
        /// <summary>
        /// Binding endpoint object that uses the <see cref="MethodInfo"/> object to invoke a method using reflection.
        /// </summary>
        public struct ActionBinding
        {
            /// <param name="mInfo">A MethodInfo object representing the target method.</param>
            /// <param name="handlerObj">The class instance that contains the method.</param>
            /// <param name="hAttr">The <see cref="HandlerAttribute"/> belonging to the target method.</param>
            public ActionBinding(MethodInfo mInfo, object? handlerObj, HandlerAttribute hAttr)
            {
                IsNoneHandler = hAttr.IsNoneHandler;
                Name = hAttr.ActionName ?? mInfo.Name;
                MethodInfo = mInfo;
                HandlerObject = handlerObj;
            }

            public readonly bool IsNoneHandler { get; }
            public string Name { get; }
            /// <summary>
            /// Stores reflection information about a method.<br/>
            /// This is used to determine which method to target.
            /// </summary>
            public readonly MethodInfo MethodInfo;
            /// <summary>
            /// This is the class instance whose method (determined by <see cref="MethodInfo"/>) is called in <see cref="HandleKeyEvent(object?, HandledEventArgs?)"/>.<br/>
            /// This is ignored if the method is static.
            /// </summary>
            public readonly object? HandlerObject;

            public void HandleKeyEvent(object? sender, HandledEventArgs? e) => MethodInfo.Invoke(HandlerObject, new[] { sender, e });
        }


        public ActionBindings(IntPtr mainWindowHandle, AudioAPI audioAPI)
            => Bindings = ParseActionMethods(Handlers = new(mainWindowHandle, audioAPI));

        private static List<ActionBinding> ParseActionMethods(object handlerObject)
        {
            Type type = handlerObject.GetType();

            List<ActionBinding> bindings = new();

            foreach (MethodInfo m in type.GetMethods())
            {
                if (m.GetCustomAttribute(typeof(HandlerAttribute)) is HandlerAttribute hAttr)
                {
                    bindings.Add(new ActionBinding(m, handlerObject, hAttr));
                }
            }

            return bindings;
        }

        internal ActionHandlers Handlers;
        /// <summary>
        /// This is the default handler that does nothing.
        /// </summary>
        /// <remarks>The first handler function with <see cref="HandlerAttribute.IsNoneHandler"/> set to true is always used.</remarks>
        public readonly ActionBinding None;
        public List<ActionBinding> Bindings { get; internal set; }
        public KeyEventHandler this[string actionName]
        {
            get
            {
                for (int i = 0; i < Bindings.Count; ++i)
                {
                    if (Bindings[i].Name.Equals(actionName, StringComparison.Ordinal))
                        return Bindings[i].HandleKeyEvent;
                }
                return None.HandleKeyEvent;
            }
        }

        public List<string> GetActionNames() => Bindings.Select(i => i.Name).ToList();
    }
    /// <summary>
    /// Implementation object containing all of the event handlers for <see cref="ActionBindings"/>.
    /// </summary>
    /// <remarks>Adding new action bindings is very simple:
    /// <list type="number" start="1">
    /// <item><description>Create a new event handler method within <see cref="ActionHandlers"/> with a descriptive name.</description></item>
    /// <item><description>Use the <see cref="HandlerAttribute"/> attribute to indicate that the method is an event handler.</description></item>
    /// </list></remarks>
    public class ActionHandlers
    {
        /// <summary>
        /// <see cref="ActionHandlers"/> constructor.
        /// </summary>
        /// <param name="mainWindowHandle">The handle of the main WPF window.</param>
        /// <param name="audioAPI">The <see cref="AudioAPI"/> instance to use for volume-related hotkeys.</param>
        public ActionHandlers(IntPtr mainWindowHandle, AudioAPI audioAPI)
        {
            MainHWnd = mainWindowHandle;
            AudioAPI = audioAPI;
        }

        /// <summary>
        /// The handle of the main WPF window.
        /// </summary>
        private IntPtr MainHWnd { get; }
        /// <summary>
        /// The <see cref="AudioAPI"/> instance to use for volume-related events.
        /// </summary>
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
        /// <summary>
        /// Helper function for sending virtual key presses.
        /// </summary>
        /// <inheritdoc cref="User32.KeyboardEvent(EVirtualKeyCode, byte, uint, IntPtr)"/>
        private static void SendKeyboardEvent(EVirtualKeyCode vk, byte scanCode = 0xAA, byte flags = 1) => User32.KeyboardEvent(vk, scanCode, flags, IntPtr.Zero);
        #endregion Statics
    }
}
