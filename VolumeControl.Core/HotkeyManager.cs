using System.Collections.Specialized;
using System.ComponentModel;
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
        public HotkeyManager() => Hotkeys = new();
        ~HotkeyManager()
        {
            HwndSource.RemoveHook(HwndHook);

            // Save Hotkeys To Settings
            StringCollection list = new();
            foreach (var hk in Hotkeys)
                list.Add(hk.ToString());
            Settings.Hotkeys = list;
            Log.Debug($"Successfully saved {nameof(Settings.Hotkeys)}: '{Settings.Hotkeys}'");
            // Save Settings
            Settings.Save();
            Settings.Reload();
        }

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

        public ObservableList<BindableWindowsHotkey> Hotkeys;

        /// <inheritdoc/>
        public void BeginInit() { }
        /// <inheritdoc/>
        public void EndInit()
        {
            var whg = new WindowHandleGetter();
            HwndSource = whg.GetHwndSource(OwnerHandle = whg.GetWindowHandle());
            HwndSource.AddHook(HwndHook);

            var list = Settings.Hotkeys ??= Settings.Hotkeys_Default;

            // Load Hotkeys From Settings
            for (int i = 0, end = list.Count; i < end; ++i)
            {
                if (list[i] is not string s || s.Length < 2) //< 2 is the minimum valid length "::" (no name, null keys)
                {
                    Log.Error($"Hotkeys[{i}] wasn't a valid hotkey string!");
                    continue;
                }
                if (!BindableWindowsHotkey.TryParse(s, this, out BindableWindowsHotkey? hk) || hk is null)
                {
                    Log.Warning($"Hotkeys[{i}] ('{s}') couldn't be parsed into a valid hotkey!");
                    continue;
                }
                Log.Info($"Hotkeys[{i}] ('{s}') was successfully parsed '{hk.Name}'");

                Hotkeys.Add(hk);
            }
        }

        private IntPtr HwndHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
            case (int)HotkeyLib.HotkeyAPI.WM_HOTKEY:
                if (Hotkeys.FirstOrDefault(h => h is not null && h.ID.Equals(wParam.ToInt32()), null) is BindableWindowsHotkey hk)
                {
                    hk.NotifyPressed();
                    handled = true;
                }
                break;
            }
            return IntPtr.Zero;
        }
    }
}
