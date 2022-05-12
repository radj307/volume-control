using HotkeyLib;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Interop;
using VolumeControl.Core.HelperTypes;
using VolumeControl.Core.HelperTypes.Lists;
using VolumeControl.Log;
using VolumeControl.WPF;

namespace VolumeControl.Core
{
    public class HotkeyManager : ISupportInitialize
    {
        /// <inheritdoc/>
        public void BeginInit()
        {
        }
        /// <inheritdoc/>
        /// <remarks>The <see cref="HotkeyManager"/> object uses this instead of constructors so that it can correctly load hotkeys with the action binding context needed.</remarks>
        public void EndInit()
        {
            HwndSource = WindowHandleGetter.GetHwndSource(OwnerHandle = WindowHandleGetter.GetWindowHandle());
            HwndSource.AddHook(HwndHook);
            Log.Debug("HotkeyManager HwndHook was added, ready to receive 'WM_HOTKEY' messages.");

            _actionBindings = new(OwnerHandle, AudioAPI);
            _initialized = true;

            // set the settings hotkeys to default if they're null
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

                Log.Debug($"Hotkeys[{i}] ('{s}') was successfully parsed:", hk.GetFullIdentifier());
            }

            ActionNames = _actionBindings.GetActionNames();
        }

        #region Fields
        public IntPtr OwnerHandle;
        private HwndSource HwndSource = null!;
        private AudioAPI _audioAPI = null!;
        private bool _initialized = false;
        //private HotkeyActionBindings _actionBindings = null!;
        private HelperTypes.Experimental.ActionBindings _actionBindings = null!;
        #endregion Fields

        #region Properties
        private static CoreSettings Settings => CoreSettings.Default;
        private static LogWriter Log => FLog.Log;
        public AudioAPI AudioAPI
        {
            get => _audioAPI;
            set
            {
                if (_audioAPI == null)
                    _audioAPI = value;
            }
        }
        //public HotkeyActionBindings ActionBindings
        //{
        //    get => _actionBindings ??= new(OwnerHandle, AudioAPI);
        //    set => _actionBindings = value;
        //}
        public HelperTypes.Experimental.ActionBindings ActionBindings
        {
            get => _initialized ? _actionBindings ??= new(OwnerHandle, AudioAPI) : null!;
            set => _actionBindings = value;
        }
        public ObservableList<BindableWindowsHotkey> Hotkeys { get; } = new();
        public List<string> ActionNames { get; set; } = null!;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Create a new hotkey and add it to <see cref="Hotkeys"/>.
        /// </summary>
        /// <param name="name">The name of the new hotkey.</param>
        /// <param name="keys">The key combination of the new hotkey.</param>
        /// <param name="action">The associated action of the new hotkey.</param>
        /// <param name="registerNow">When true, the new hotkey is registered immediately after construction.</param>
        public void AddHotkey(string name, IKeyCombo keys, string action, bool registerNow = false)
        {
            var hk = new BindableWindowsHotkey(this, name, keys, action, registerNow);
            Hotkeys.Add(hk);
            Log.Info($"Created a new hotkey entry:", hk.GetFullIdentifier());
        }
        /// <summary>
        /// Create a new blank hotkey and add it to <see cref="Hotkeys"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="BindableWindowsHotkey.Name"/> = <see cref="string.Empty"/>
        /// </remarks>
        public void AddHotkey() => AddHotkey(string.Empty, new KeyCombo(), "None", false);
        /// <summary>
        /// Remove the specified hotkey from <see cref="Hotkeys"/>.
        /// </summary>
        /// <param name="hk">The <see cref="BindableWindowsHotkey"/> object to remove.<br/>If this is null, nothing happens.</param>
        public void DelHotkey(BindableWindowsHotkey? hk)
        {
            if (hk == null)
                return;
            Hotkeys.Remove(hk);
            Log.Info($"Deleted hotkey {hk.ID} '{hk.Name}'");
        }
        /// <summary>
        /// Remove the specified hotkey from <see cref="Hotkeys"/>.
        /// </summary>
        /// <param name="id">The ID number of the hotkey to delete.</param>
        public void DelHotkey(int id)
        {
            for (int i = Hotkeys.Count - 1; i >= 0; --i)
                if (Hotkeys[i].ID.Equals(id))
                    Hotkeys.RemoveAt(i);
        }
        public BindableWindowsHotkey? GetHotkey(int id) => Hotkeys.FirstOrDefault(hk => hk is not null && hk.ID.Equals(id), null);
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
        /// <summary>
        /// Handles window messages, and triggers <see cref="WindowsHotkey.Pressed"/> events.
        /// </summary>
        private IntPtr HwndHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
            case (int)HotkeyAPI.WM_HOTKEY:
                int pressedID = wParam.ToInt32();
                if (GetHotkey(pressedID) is BindableWindowsHotkey hk)
                {
                    hk.NotifyPressed(); //< trigger the associated hotkey's Pressed event
                    handled = true;
                }
                break;
            }
            return IntPtr.Zero;
        }
        /// <summary>
        /// Removes the message hook from the application's handle.
        /// </summary>
        public void RemoveHook()
        {
            HwndSource.RemoveHook(HwndHook);
            HwndSource.Dispose();
            Log.Debug("HotkeyManager HwndHook was removed, 'WM_HOTKEY' messages will no longer be received.");
        }
        #endregion Methods
    }
}
