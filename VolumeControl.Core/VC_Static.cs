using AudioAPI;
using System.ComponentModel;
using VolumeControl.Core.Audio;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Events;
using VolumeControl.Core.Keyboard;
using VolumeControl.Log;

namespace VolumeControl.Core
{
    public static partial class VC_Static
    {
        #region Initializers
        /// <summary>
        /// Initialize all non-hotkey VC_Static members & set up the log file.
        /// </summary>
        /// <exception cref="Exception">Initialize already called.</exception>
        public static void Initialize()
        {
            if (_initialized)
                throw new Exception($"VC_Static.Initialize() was already called!");
            else if (!FLog.Initialized)
            {
                FLog.Initialize();
                Log.WriteWarning($"VC_Static.Initialize() automatically called FLog.Initialize()!");
            }
            _initialized = true;

            // init process API
            API = new AudioProcessAPI(new()
            {
                {
                    VolumeControlSubject.VOLUME,
                    new()
                    {
                        { VolumeControlAction.INCREMENT, Volume_Increase },
                        { VolumeControlAction.DECREMENT, Volume_Decrease },
                        { VolumeControlAction.TOGGLE, Volume_Toggle },
                    }
                },
                {
                    VolumeControlSubject.MEDIA,
                    new()
                    {
                        { VolumeControlAction.NEXT, Media_Next },
                        { VolumeControlAction.PREV, Media_Prev },
                        { VolumeControlAction.TOGGLE, Media_Toggle },
                    }
                },
                {
                    VolumeControlSubject.TARGET,
                    new()
                    {
                        { VolumeControlAction.NEXT, Target_Next },
                        { VolumeControlAction.PREV, Target_Prev },
                        { VolumeControlAction.TOGGLE, Target_Toggle },
                    }
                },
            });
            Log.WriteInfo($"{nameof(API)} initialized.");

            // init volume step
            VolumeStep = 0m;

            Log.WriteInfo($"{nameof(VC_Static)} initialization complete.");
        }
        /// <summary>
        /// Initialize the 'Hotkeys' list & load previous settings
        /// </summary>
        /// <param name="owner">Form that should be used as the owner control for all hotkeys.</param>
        /// <exception cref="Exception">InitializeHotkeys already called.</exception>
        public static void InitializeHotkeys(Form owner)
        {
            if (_hk_initialized)
                throw new Exception($"Hotkey initialization already completed!");
            _hk_initialized = true;
            // init hotkey manager
            Hotkeys = new HotkeyBindingList(new()
            {
                new(owner, "Volume Up", nameof(Properties.Settings.Default.hks_VolumeUp), VolumeControlSubject.VOLUME, VolumeControlAction.INCREMENT, nameof(Properties.Settings.Default.hks_VolumeUpEnabled)),
                new(owner, "Volume Down", nameof(Properties.Settings.Default.hks_VolumeDown), VolumeControlSubject.VOLUME, VolumeControlAction.DECREMENT, nameof(Properties.Settings.Default.hks_VolumeDownEnabled)),
                new(owner, "Toggle Mute", nameof(Properties.Settings.Default.hks_VolumeToggle), VolumeControlSubject.VOLUME, VolumeControlAction.TOGGLE, nameof(Properties.Settings.Default.hks_VolumeToggleEnabled)),

                new(owner, "Next Track", nameof(Properties.Settings.Default.hks_MediaNext), VolumeControlSubject.MEDIA, VolumeControlAction.INCREMENT, nameof(Properties.Settings.Default.hks_MediaNextEnabled)),
                new(owner, "Previous Track", nameof(Properties.Settings.Default.hks_MediaPrev), VolumeControlSubject.MEDIA, VolumeControlAction.DECREMENT, nameof(Properties.Settings.Default.hks_MediaPrevEnabled)),
                new(owner, "Toggle Playback", nameof(Properties.Settings.Default.hks_MediaToggle), VolumeControlSubject.MEDIA, VolumeControlAction.TOGGLE, nameof(Properties.Settings.Default.hks_MediaToggleEnabled)),

                new(owner, "Next Target", nameof(Properties.Settings.Default.hks_TargetNext), VolumeControlSubject.TARGET, VolumeControlAction.INCREMENT, nameof(Properties.Settings.Default.hks_TargetNextEnabled)),
                new(owner, "Previous Target", nameof(Properties.Settings.Default.hks_TargetPrev), VolumeControlSubject.TARGET, VolumeControlAction.DECREMENT, nameof(Properties.Settings.Default.hks_TargetPrevEnabled)),
                new(owner, "Toggle Target Lock", nameof(Properties.Settings.Default.hks_TargetToggle), VolumeControlSubject.TARGET, VolumeControlAction.TOGGLE, nameof(Properties.Settings.Default.hks_TargetToggleEnabled)),
            });
            Hotkeys.BindHotkeyPressedEvents(API);
            Log.WriteInfo("Finished initialization of hotkey-action bindings.");
        }
        #endregion Initializers

        #region Members
        private static bool _initialized = false, _hk_initialized = false;
        private static AudioProcessAPI _api = null!;
        private static HotkeyBindingList _hotkeys = null!;
        #endregion Members

        #region Events
        // TODO: Implement a NotifyHotkeyPressed event trigger for all hotkeys (currently just volume)
        public static event HotkeyPressedEventHandler? HotkeyPressed = null;
        #endregion Events

        #region Methods
        private static void NotifyHotkeyPressed(object? sender, HotkeyPressedEventArgs e) => HotkeyPressed?.Invoke(sender, e);
        private static void SendKeyboardEvent(VirtualKeyCode vk, byte scanCode = 0xAA, byte flags = 1) => AudioAPI.WindowsAPI.User32.KeyboardEvent(vk, scanCode, flags, IntPtr.Zero);

        #region HotkeyActions
        internal static void Volume_Increase(object? sender, HandledEventArgs e)
        {
            var sel = API.GetSelectedProcess();
            if (sel is AudioProcess ap)
            {
                NotifyHotkeyPressed(sender, new(VolumeControlSubject.VOLUME, VolumeControlAction.INCREMENT, ap.VolumeFullRange += VolumeStep));
                e.Handled = true;
            }
        }
        internal static void Volume_Decrease(object? sender, HandledEventArgs e)
        {
            var sel = API.GetSelectedProcess();
            if (sel is AudioProcess ap)
            {
                NotifyHotkeyPressed(sender, new(VolumeControlSubject.VOLUME, VolumeControlAction.DECREMENT, ap.VolumeFullRange -= VolumeStep));
                ap.VolumeFullRange -= VolumeStep;
                e.Handled = true;
            }
        }
        internal static void Volume_Toggle(object? sender, HandledEventArgs e)
        {
            var sel = API.GetSelectedProcess();
            if (sel is AudioProcess ap)
            {
                NotifyHotkeyPressed(sender, new(VolumeControlSubject.VOLUME, VolumeControlAction.TOGGLE, ap.ToggleMute()));
                e.Handled = true;
            }
        }

        internal static void Media_Next(object? sender, HandledEventArgs e)
            => SendKeyboardEvent(VirtualKeyCode.VK_MEDIA_NEXT_TRACK);
        internal static void Media_Prev(object? sender, HandledEventArgs e)
            => SendKeyboardEvent(VirtualKeyCode.VK_MEDIA_PREV_TRACK);
        internal static void Media_Toggle(object? sender, HandledEventArgs e)
            => SendKeyboardEvent(VirtualKeyCode.VK_MEDIA_PLAY_PAUSE);

        internal static void Target_Next(object? sender, HandledEventArgs e)
            => API.SelectNextProcess();
        internal static void Target_Prev(object? sender, HandledEventArgs e)
            => API.SelectPrevProcess();
        internal static void Target_Toggle(object? sender, HandledEventArgs e)
            => API.LockSelection = !API.LockSelection;
        #endregion HotkeyActions

        /// <summary>
        /// Saves all settings with their current values to the default windows forms settings file.
        /// </summary>
        public static void SaveSettings()
        {
            foreach (var hk in Hotkeys)
            {
                hk.Save();
            }
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            Log.WriteInfo("Saved Current Program Settings.");
        }
        /// <summary>
        /// Trigger the associated hotkey's bound action.
        /// </summary>
        /// <remarks>
        /// This is useful for programmatically 'pressing' a hotkey rather than the user actually pressing it.
        /// </remarks>
        /// <param name="subject">The <see cref="VolumeControlSubject"/> target.</param>
        /// <param name="action">The <see cref="VolumeControlAction"/> action to perform on the target.</param>
        public static void TriggerHotkey(object? sender, VolumeControlSubject subject, VolumeControlAction action)
        {
            if (!_initialized || !_hk_initialized)
                return; // not initialized yet
            var hkAction = API.Actions[subject][action];
            if (hkAction != null)
            {
                hkAction.Invoke(sender, new());
                FLog.Log.WriteDebug($"Hotkey {subject}:{action} was triggered virtually.");
            }
        }
        #endregion Methods

        #region Properties
        public static LogWriter<FileEndpoint> Log => FLog.Log;
        public static AudioProcessAPI API
        {
            get
            {
                if (!_initialized) throw new InvalidOperationException("Cannot retrieve uninitialized object! (This happened because the 'VC_Static.Initialize()' function wasn't called first!)");
                return _api;
            }
            internal set => _api = value;
        }
        public static HotkeyBindingList Hotkeys
        {
            get
            {
                if (!_hk_initialized) throw new InvalidOperationException("Cannot retrieve uninitialized object! (This happened because the 'VC_Static.InitializeHotkeys(Form)' function wasn't called first!)");
                return _hotkeys;
            }
            internal set => _hotkeys = value;
        }
        /// <summary>
        /// Gets or sets the volume step size.
        /// </summary>
        public static decimal VolumeStep { get; set; }
        /// <summary>
        /// Gets or sets the target lock state.
        /// </summary>
        public static bool TargetLocked
        {
            get => API.LockSelection;
            set => API.LockSelection = value;
        }
        #endregion Properties
    }
}
