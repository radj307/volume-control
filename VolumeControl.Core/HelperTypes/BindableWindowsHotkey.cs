using HotkeyLib;
using System.ComponentModel;
using AudioAPI.Objects;
using System.Windows.Forms;

namespace VolumeControl.Core.HelperTypes
{
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
    }

    public class HotkeyActionBindings
    {
        public HotkeyActionBindings(AudioAPI audioAPI) => Bindings = new()
        {
            { EHotkeyAction.None, null },
            { // VOLUME UP
                EHotkeyAction.VolumeUp,
                audioAPI.IncreaseVolume
            },
            { // VOLUME DOWN
                EHotkeyAction.VolumeDown,
                audioAPI.DecreaseVolume
            },
            { // TOGGLE MUTE
                EHotkeyAction.ToggleMute,
                audioAPI.ToggleMute
            },
            { // NEXT TRACK
                EHotkeyAction.NextTrack,
                audioAPI.NextTrack
            },
            { // PREVIOUS TRACK
                EHotkeyAction.PreviousTrack,
                audioAPI.PreviousTrack
            },
            { // TOGGLE PLAYBACK
                EHotkeyAction.TogglePlayback,
                audioAPI.TogglePlayback
            },
            {
                EHotkeyAction.NextTarget,
                audioAPI.NextTarget
            },
            {
                EHotkeyAction.PreviousTarget,
                audioAPI.PreviousTarget
            },
            {
                EHotkeyAction.ToggleTargetLock,
                audioAPI.ToggleTargetLock
            }
        };

        public Dictionary<EHotkeyAction, HotkeyLib.KeyEventHandler?> Bindings { get; set; }

        public HotkeyLib.KeyEventHandler? this[EHotkeyAction action]
        {
            get => Bindings[action];
            set => Bindings[action] = value;
        }
    }

    [TypeConverter(typeof(WindowsHotkeyConverter))]
    public class BindableWindowsHotkey : IKeyCombo, IDisposable
    {
        public BindableWindowsHotkey(string name, IntPtr owner, string keys)
        {
            Name = name;
            Hotkey = new(owner, new KeyCombo(keys));
        }
        public BindableWindowsHotkey(string name, IntPtr owner, IKeyCombo keys)
        {
            Name = name;
            Hotkey = new(owner, keys);
        }

        public WindowsHotkey Hotkey { get; private set; }

        public string Name { get; set; }
        public bool Registered
        {
            get => Hotkey.Registered;
            set => Hotkey.Registered = value;
        }

        #region InterfaceImplementation
        /// <inheritdoc/>
        public System.Windows.Forms.Keys Key { get => ((IKeyCombo)Hotkey).Key; set => ((IKeyCombo)Hotkey).Key = value; }
        /// <inheritdoc/>
        public Modifier Mod { get => ((IKeyCombo)Hotkey).Mod; set => ((IKeyCombo)Hotkey).Mod = value; }
        /// <inheritdoc/>
        public bool Alt { get => ((IKeyCombo)Hotkey).Alt; set => ((IKeyCombo)Hotkey).Alt = value; }
        /// <inheritdoc/>
        public bool Ctrl { get => ((IKeyCombo)Hotkey).Ctrl; set => ((IKeyCombo)Hotkey).Ctrl = value; }
        /// <inheritdoc/>
        public bool Shift { get => ((IKeyCombo)Hotkey).Shift; set => ((IKeyCombo)Hotkey).Shift = value; }
        /// <inheritdoc/>
        public bool Win { get => ((IKeyCombo)Hotkey).Win; set => ((IKeyCombo)Hotkey).Win = value; }

        /// <inheritdoc/>
        public bool Valid => ((IKeyCombo)Hotkey).Valid;

        System.Windows.Forms.Keys IKeyCombo.Key { get => ((IKeyCombo)Hotkey).Key; set => ((IKeyCombo)Hotkey).Key = value; }

        /// <inheritdoc/>
        public void Dispose() => ((IDisposable)Hotkey).Dispose();
        #endregion InterfaceImplementation

        public int ID => Hotkey.ID;

        public event HotkeyLib.KeyEventHandler Pressed
        {
            add => Hotkey.Pressed += value;
            remove => Hotkey.Pressed -= value;
        }
        public event EventHandler KeysChanged
        {
            add => Hotkey.KeysChanged += value;
            remove => Hotkey.KeysChanged -= value;
        }

        public void NotifyPressed(HandledEventArgs e) => Hotkey.NotifyPressed(e);
        public void NotifyPressed() => NotifyPressed(new());

        /// <inheritdoc/>
        public override string ToString() => $"{Name}::{base.ToString()}";

        public static BindableWindowsHotkey Parse(string hkString, HotkeyManager manager)
        {
            int div1 = hkString.IndexOf("::");
            int div2 = hkString.IndexOf("::", div1 + 2);
            if (div2 == -1)
                div2 = hkString.Length - 1;
            BindableWindowsHotkey hk = new(hkString[..div1], manager.OwnerHandle, hkString[(div1 + 2)..div2]);
            if (Enum.TryParse(typeof(EHotkeyAction), hkString[(div2 + 2)..], out object? actionObj) && actionObj is EHotkeyAction action)
                hk.Pressed += manager.ActionBindings[action];
            return hk;
        }
        public static bool TryParse(string hkString, HotkeyManager manager, out BindableWindowsHotkey? hk)
        {
            int div1 = hkString.IndexOf("::");
            int div2 = hkString.IndexOf("::", div1 + 2);
            hk = null;
            if (div1 == -1)
                return false;
            if (div2 == -1)
                div2 = hkString.Length - 1;

            hk = new(hkString[..div1], manager.OwnerHandle, new KeyCombo(hkString[(div1 + 2)..div2]));
            if (Enum.TryParse(typeof(EHotkeyAction), hkString[(div2 + 2)..], out object? actionObj) && actionObj is EHotkeyAction action)
                hk.Pressed += manager.ActionBindings[action];

            return true;
        }
    }
}
