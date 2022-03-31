﻿using AudioAPI;
using System.ComponentModel;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Logging;

namespace VolumeControl.Core
{
    public static partial class VC_Static
    {
        #region Members
        private static bool _initialized = false, _hk_initialized = false;
        private static LogWriter _log = null!;
        private static AudioProcessAPI _api = null!;
        private static HotkeyBindingList _hotkeys = null!;
        #endregion Members

        #region Methods
        private static void SendKeyboardEvent(VirtualKeyCode vk, byte scanCode = 0xAA, byte flags = 1) => AudioAPI.WindowsAPI.User32.KeyboardEvent(vk, scanCode, flags, IntPtr.Zero);

        #region HotkeyActions
        internal static void Volume_Increase(object? sender, HandledEventArgs e)
            => VolumeHelper.IncrementVolume(API.GetSelectedProcess().PID, VolumeStep);
        internal static void Volume_Decrease(object? sender, HandledEventArgs e)
            => VolumeHelper.DecrementVolume(API.GetSelectedProcess().PID, VolumeStep);
        internal static void Volume_Toggle(object? sender, HandledEventArgs e)
            => VolumeHelper.ToggleMute(API.GetSelectedProcess().PID);

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

        public static void Initialize()
        {
            _initialized = true;
            // init log writer
#           if DEBUG
            EventType logFilter = EventType.ALL;
#           else
            EventType logFilter = EventType.ALL_EXCEPT_DEBUG;
#           endif
            string logFile = Properties.Settings.Default.log_file;
            Log = new LogWriter(logFile, logFilter);
            Log.WriteInfo(new string[] {
                $"{nameof(Log)} initialized.",
                $"Output File:  \'{logFile}\'",
                $"Type Filter:  \'{System.Enum.GetName(typeof(EventType), logFilter)}\'"
            });

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
            VolumeStep = Properties.Settings.Default.volumeStep;
            Log.WriteInfo($"{nameof(VolumeStep)} initialized to \'{VolumeStep}\'");

            Log.WriteInfo($"{nameof(VC_Static)} initialization complete.");
        }
        public static void InitializeHotkeys(Form owner)
        {
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
            Log.WriteInfo($"{nameof(Hotkeys)} successfully created.");
            Hotkeys.BindHotkeyPressedEvents(API);
            Log.WriteInfo("Finished initialization of hotkey-action bindings.");
        }
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
        #endregion Methods

        #region Properties
        public static LogWriter Log
        {
            get
            {
                if (!_initialized) throw new InvalidOperationException("Cannot retrieve uninitialized object! (This happened because the 'VC_Static.Initialize()' function wasn't called first!)");
                return _log;
            }
            internal set => _log = value;
        }
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

        public static decimal VolumeStep { get; set; }
        #endregion Properties
    }
}