using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using VolumeControl.Core.HelperTypes;
using VolumeControl.Core.HelperTypes.Lists;
using VolumeControl.Log;

namespace VolumeControl.Core
{
    public class HotkeyManager : ISupportInitialize
    {
        #region Statics
        public static readonly IntPtr DefaultOwner = Process.GetCurrentProcess().MainWindowHandle;
        #endregion Statics

        public HotkeyManager() => Hotkeys = new();
        ~HotkeyManager()
        {
            // Save Hotkeys To Settings
            StringCollection list = new();
            foreach (var hk in Hotkeys)
                list.Add(hk.ToString());
            Settings.Hotkeys = list;
            // Save Settings
            Settings.Save();
            Settings.Reload();
        }

        private static CoreSettings Settings => CoreSettings.Default;
        private static LogWriter Log => FLog.Log;
        private AudioAPI _audioAPI = null!;
        public AudioAPI AudioAPI
        {
            get => _audioAPI;
            set
            {
                if (_audioAPI == null)
                    _audioAPI = value;
            }
        }
        private HotkeyActionBindings _actionBindings = null!;
        public HotkeyActionBindings ActionBindings
        {
            get => _actionBindings ??= new(AudioAPI);
            set => _actionBindings = value;
        }

        public ObservableList<BindableWindowsHotkey> Hotkeys;

        public void BeginInit() {}
        public void EndInit()
        {
            var list = Settings.Hotkeys ?? Settings.Hotkeys_Default;

            // Load Hotkeys From Settings
            for (int i = 0, end = list.Count; i < end; ++i)
            {
                if (list[i] is not string s || s.Length < 2) //< 2 is the minimum valid length "::" (no name, null keys)
                {
                    Log.Error($"Hotkeys[{i}] wasn't a valid hotkey string!");
                    continue;
                }
                if (!BindableWindowsHotkey.TryParse(s, ActionBindings, out BindableWindowsHotkey? hk) || hk is null)
                {
                    Log.Warning($"Hotkeys[{i}] ('{s}') couldn't be parsed into a valid hotkey!");
                    continue;
                }
                Log.Info($"Hotkeys[{i}] ('{s}') was successfully parsed '{hk.Name}'");

                Hotkeys.Add(hk);
            }
        }
    }
}
