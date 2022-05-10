using HotkeyLib;
using System.Windows.Forms;
using System.ComponentModel;
using AudioAPI.Objects;

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
    public class BindableWindowsHotkey : IMessageFilter, IKeyCombo, IDisposable
    {
        public BindableWindowsHotkey(string name, string keys)
        {
            Name = name;
            Hotkey = new(HotkeyManager.DefaultOwner, new KeyCombo(keys));
            Application.AddMessageFilter(this);
        }
        public BindableWindowsHotkey(string name, IKeyCombo keys)
        {
            Name = name;
            Hotkey = new(HotkeyManager.DefaultOwner, keys);
            Application.AddMessageFilter(this);
        }

        public WindowsHotkey Hotkey { get; private set; }

        public string Name { get; set; }
        public bool Registered
        {
            get => Hotkey.Registered;
            set => Hotkey.Registered = value;
        }

        #region InterfaceImplementation
        public Keys Key { get => ((IKeyCombo)Hotkey).Key; set => ((IKeyCombo)Hotkey).Key = value; }
        public Modifier Mod { get => ((IKeyCombo)Hotkey).Mod; set => ((IKeyCombo)Hotkey).Mod = value; }
        public bool Alt { get => ((IKeyCombo)Hotkey).Alt; set => ((IKeyCombo)Hotkey).Alt = value; }
        public bool Ctrl { get => ((IKeyCombo)Hotkey).Ctrl; set => ((IKeyCombo)Hotkey).Ctrl = value; }
        public bool Shift { get => ((IKeyCombo)Hotkey).Shift; set => ((IKeyCombo)Hotkey).Shift = value; }
        public bool Win { get => ((IKeyCombo)Hotkey).Win; set => ((IKeyCombo)Hotkey).Win = value; }

        public bool Valid => ((IKeyCombo)Hotkey).Valid;

        public void Dispose() => ((IDisposable)Hotkey).Dispose();
        public bool PreFilterMessage(ref Message m) => ((IMessageFilter)Hotkey).PreFilterMessage(ref m);
        #endregion InterfaceImplementation

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

        public override string ToString() => $"{Name}::{base.ToString()}";

        public static BindableWindowsHotkey Parse(string hkString, HotkeyActionBindings actions)
        {
            int div1 = hkString.IndexOf("::");
            int div2 = hkString.IndexOf("::", div1 + 2);
            if (div2 == -1)
                div2 = hkString.Length - 1;
            BindableWindowsHotkey hk = new(hkString[..div1], hkString[(div1 + 2)..div2]);
            if (Enum.TryParse(typeof(EHotkeyAction), hkString[(div2 + 2)..], out object? actionObj) && actionObj is EHotkeyAction action)
                hk.Pressed += actions[action];
            return hk;
        }
        public static bool TryParse(string hkString, HotkeyActionBindings actions, out BindableWindowsHotkey? hk)
        {
            int div1 = hkString.IndexOf("::");
            int div2 = hkString.IndexOf("::", div1 + 2);
            hk = null;
            if (div1 == -1)
                return false;
            if (div2 == -1)
                div2 = hkString.Length - 1;

            hk = new(hkString[..div1], new KeyCombo(hkString[(div1 + 2)..div2]));
            if (Enum.TryParse(typeof(EHotkeyAction), hkString[(div2 + 2)..], out object? actionObj) && actionObj is EHotkeyAction action)
                hk.Pressed += actions[action];

            return true;
        }
    }
}
