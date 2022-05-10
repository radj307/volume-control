using HotkeyLib;
using System.ComponentModel;

namespace VolumeControl.Core.HelperTypes
{
    public class BindableWindowsHotkey : IKeyCombo, IDisposable
    {
        public BindableWindowsHotkey(HotkeyManager manager, string name, IKeyCombo keys, EHotkeyAction action, bool registerNow = false)
        {
            _manager = manager;
            Name = name;
            Hotkey = new(_manager.OwnerHandle, keys);
            Action = action;
            if (registerNow) //< only trigger when true
                Registered = registerNow;
        }

        private readonly HotkeyManager _manager;
        public WindowsHotkey Hotkey { get; private set; }

        public string Name { get; set; }
        public bool Registered
        {
            get => Hotkey.Registered;
            set => Hotkey.Registered = value;
        }
        private EHotkeyAction _action = EHotkeyAction.None;
        /// <summary>
        /// Gets or sets the action associated with this hotkey.
        /// </summary>
        /// <remarks>This property automatically handles changing the <see cref="Pressed"/> event.</remarks>
        public EHotkeyAction Action
        {
            get => _action;
            set
            {
                Pressed -= _manager.ActionBindings[_action];
                _action = value;
                Pressed += _manager.ActionBindings[_action];
            }
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
        public void Dispose()
        {
            ((IDisposable)Hotkey).Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion InterfaceImplementation

        /// <inheritdoc/>
        public int ID => Hotkey.ID;

        /// <inheritdoc/>
        public event HotkeyLib.KeyEventHandler Pressed
        {
            add => Hotkey.Pressed += value;
            remove => Hotkey.Pressed -= value;
        }
        /// <inheritdoc/>
        public event EventHandler KeysChanged
        {
            add => Hotkey.KeysChanged += value;
            remove => Hotkey.KeysChanged -= value;
        }

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                ((INotifyPropertyChanged)Hotkey).PropertyChanged += value;
            }

            remove
            {
                ((INotifyPropertyChanged)Hotkey).PropertyChanged -= value;
            }
        }

        /// <summary>
        /// Triggers a <see cref="Pressed"/> event.
        /// </summary>
        /// <param name="e"><see cref="HandledEventArgs"/> to pass to the event.</param>
        public void NotifyPressed(HandledEventArgs e) => Hotkey.NotifyPressed(e);
        /// <summary>
        /// Triggers a <see cref="Pressed"/> event with the default arguments.
        /// </summary>
        public void NotifyPressed() => NotifyPressed(new());

        /// <inheritdoc/>
        public override string ToString() => Serialize();
        public string Serialize() => $"{Name}::{Hotkey.ToString()}::{Enum.GetName(typeof(EHotkeyAction), Action)}::{Registered}";

        public static BindableWindowsHotkey Parse(string hkString, HotkeyManager manager)
        {
            var split = hkString.Split("::", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            // Set the hotkey name:
            string name = split[0];

            // Parse the key combination:
            var keys = split.Length > 1 ? KeyCombo.Parse(split[1]) : new KeyCombo();

            // Determine which action to bind to:
            EHotkeyAction action = EHotkeyAction.None;
            if (split.Length > 2 && Enum.TryParse(typeof(EHotkeyAction), split[2], out object? actionObj) && actionObj is EHotkeyAction a)
                action = a;

            // Check if the hotkey was registered:
            bool register = false;
            if (split.Length > 3)
                _ = bool.TryParse(split[3], out register);

            return new(manager, name, keys, action, register);
        }
    }
}
