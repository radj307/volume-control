using HotkeyLib;
using System.ComponentModel;

namespace VolumeControl.Hotkeys
{
    public class BindableWindowsHotkey : IKeyCombo, IDisposable
    {
        public BindableWindowsHotkey(HotkeyManager manager, string name, IKeyCombo keys, string action, bool registerNow = false)
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

        private string _actionName = string.Empty;

        /// <summary>
        /// Gets or sets the action associated with this hotkey.
        /// </summary>
        /// <remarks>This property automatically handles changing the <see cref="Pressed"/> event.</remarks>
        public string Action
        {
            get => _actionName;
            set
            {
                if (_actionName.Length > 0)
                    Pressed -= _manager.Actions[_actionName];
                Pressed += _manager.Actions[_actionName = value];
            }
        }

        #region InterfaceImplementation
        /// <inheritdoc/>
        public System.Windows.Forms.Keys Key { get => Hotkey.Key; set => Hotkey.Key = value; }
        /// <inheritdoc/>
        public Modifier Mod { get => Hotkey.Mod; set => Hotkey.Mod = value; }
        /// <inheritdoc/>
        public bool Alt { get => Hotkey.Alt; set => Hotkey.Alt = value; }
        /// <inheritdoc/>
        public bool Ctrl { get => Hotkey.Ctrl; set => Hotkey.Ctrl = value; }
        /// <inheritdoc/>
        public bool Shift { get => Hotkey.Shift; set => Hotkey.Shift = value; }
        /// <inheritdoc/>
        public bool Win { get => Hotkey.Win; set => Hotkey.Win = value; }

        /// <inheritdoc/>
        public bool Valid => Hotkey.Valid;

        /// <inheritdoc/>
        System.Windows.Forms.Keys IKeyCombo.Key { get => Hotkey.Key; set => Hotkey.Key = value; }

        /// <inheritdoc/>
        public void Dispose()
        {
            Hotkey.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion InterfaceImplementation

        /// <inheritdoc/>
        public int ID => Hotkey.ID;
        private static HotkeyManagerSettings Settings => HotkeyManagerSettings.Default;

        /// <inheritdoc/>
        public event KeyEventHandler Pressed
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
            add => Hotkey.PropertyChanged += value;
            remove => Hotkey.PropertyChanged -= value;
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
        public string Serialize() => $"{Name}{Settings.HotkeyNameSeperatorChar}{Hotkey.ToString()}{Settings.HotkeyNameSeperatorChar}{Action}{Settings.HotkeyNameSeperatorChar}{Registered}";
        public string GetFullIdentifier() => $"{{ Name: '{Name}', Keys: '{Hotkey.Serialize()}', Action: '{Action}', Registered: '{Registered}' }}";

        public static BindableWindowsHotkey Parse(string hkString, HotkeyManager manager)
        {
            var split = hkString.Split(Settings.HotkeyNameSeperatorChar, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            // Set the hotkey name:
            string name = split[0];

            // Parse the key combination:
            var keys = split.Length > 1 ? KeyCombo.Parse(split[1]) : new KeyCombo();

            // Determine which action to bind to:
            string action = split.Length > 2 ? split[2] : string.Empty;
            //EHotkeyAction action = EHotkeyAction.None;
            //if (split.Length > 2 && Enum.TryParse(typeof(EHotkeyAction), split[2], out object? actionObj) && actionObj is EHotkeyAction a)
            //    action = a;

            // Check if the hotkey was registered:
            bool register = false;
            if (split.Length > 3)
                _ = bool.TryParse(split[3], out register);

            return new(manager, name, keys, action, register);
        }
    }
}
