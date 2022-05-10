using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using VolumeControl.Core.HelperTypes;
using VolumeControl.Core.HelperTypes.Lists;
using VolumeControl.Log;
using VolumeControl.WPF;

namespace VolumeControl.Core
{
    public class HotkeyManager : ISupportInitialize
    {
        public IntPtr OwnerHandle;
        private HwndSource HwndSource = null!;
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
        public bool ShowActionBindings { get; set; }
        public Visibility ActionBindingsVisibility => ShowActionBindings ? Visibility.Visible : Visibility.Hidden;

        public ObservableList<BindableWindowsHotkey> Hotkeys { get; } = new();

        /// <inheritdoc/>
        public void BeginInit() { }
        /// <inheritdoc/>
        public void EndInit()
        {
            HwndSource = WindowHandleGetter.GetHwndSource(OwnerHandle = WindowHandleGetter.GetWindowHandle());
            HwndSource.AddHook(HwndHook);
            Log.Debug("HotkeyManager HwndHook was added, ready to receive 'WM_HOTKEY' messages.");

            var list = Settings.Hotkeys ??= Settings.Hotkeys_Default;

            // Load Hotkeys From Settings
            for (int i = 0, end = list.Count; i < end; ++i)
            {
                if (list[i] is not string s || s.Length < 2) //< 2 is the minimum valid length "::" (no name, null keys)
                {
                    Log.Error($"Hotkeys[{i}] wasn't a valid hotkey string!");
                    continue;
                }
                
                var hk = BindableWindowsHotkey.Parse(s, this);
                Hotkeys.Add(hk);

                Log.Debug($"Hotkeys[{i}] ('{s}') was successfully parsed: {{ Name: '{hk.Name}', KeyCombo: '{hk.Hotkey.ToString()}', Action: '{ActionBindings[hk.Action]?.Method.Name}', Registered: '{hk.Registered}' }}");
            }
        }

        private IntPtr HwndHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
            case (int)HotkeyLib.HotkeyAPI.WM_HOTKEY:
                if (Hotkeys.FirstOrDefault(h => h is not null && h.ID.Equals(wParam.ToInt32()), null) is BindableWindowsHotkey hk)
                {
                    Log.Info($"Hotkey {hk.ID} ({hk.Name}) Pressed.",
                        hk.Action.Equals(EHotkeyAction.None) ? " ()" : "");
                    hk.NotifyPressed();
                    handled = true;
                }
                break;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Saves all hotkeys to the settings file.
        /// </summary>
        public void SaveHotkeys()
        {
            // Save Hotkeys To Settings
            StringCollection list = new();
            foreach (var hk in Hotkeys)
                list.Add(hk.Serialize());
            Settings.Hotkeys = list;
            Log.Debug($"Successfully saved {nameof(Settings.Hotkeys)}: '{Settings.Hotkeys}'");
            // Save Settings
            Settings.Save();
            Settings.Reload();
        }
        public void RemoveHook()
        {
            HwndSource.RemoveHook(HwndHook);
            HwndSource.Dispose();
            Log.Debug("HotkeyManager HwndHook was removed, 'WM_HOTKEY' messages will no longer be received.");
        }
        ~HotkeyManager()
        { // this doesn't get called reliably:
            SaveHotkeys();
            RemoveHook();
        } // use other methods
    }
}
