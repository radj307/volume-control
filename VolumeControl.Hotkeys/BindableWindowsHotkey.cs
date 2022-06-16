using HotkeyLib;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Hotkeys.Interfaces;
using VolumeControl.Log;

namespace VolumeControl.Hotkeys
{
    /// <summary>This is a wrapper object around <see cref="HotkeyLib.WindowsHotkey"/> that is designed for data binding.</summary>
    public class BindableWindowsHotkey : IKeyCombo, IDisposable, INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <inheritdoc cref="BindableWindowsHotkey"/>
        /// <param name="manager">The hotkey manager to bind to.</param>
        /// <param name="name">The name of this hotkey.</param>
        /// <param name="keys">The key &amp; modifiers of this hotkey.</param>
        /// <param name="action">The action that this hotkey is bound to.</param>
        /// <param name="registerNow">When <see langword="true"/>, the hotkey is registered in the constructor; otherwise, the hotkey starts as unregistered.</param>
        public BindableWindowsHotkey(HotkeyManager manager, string name, IKeyCombo keys, string action, bool registerNow = false)
        {
            _manager = manager;
            _name = name;
            Hotkey = new(_manager.OwnerHandle, keys);
            ActionName = action;
            if (registerNow) //< only trigger when true
                Registered = registerNow;
            this.PropertyChanged += HandlePropertyChanged;
        }

        private readonly HotkeyManager _manager;
        /// <summary>The underlying <see cref="WindowsHotkey"/> instance.<br/>This shouldn't be used when data binding.</summary>
        public WindowsHotkey Hotkey { get; private set; }
        /// <summary>
        /// Gets or sets this hotkey's name.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                NotifyPropertyChanging();
                _name = value;
                NotifyPropertyChanged();
            }
        }
        private string _name;
        /// <inheritdoc/>
        public bool Registered
        {
            get => Hotkey.Registered;
            set
            {
                NotifyPropertyChanging();
                Hotkey.Registered = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Gets or sets the name of the action associated with this hotkey.
        /// </summary>
        /// <remarks>This property automatically handles changing the <see cref="Pressed"/> event.</remarks>
        public string? ActionName
        {
            get => Action?.Name;
            set
            {
                NotifyPropertyChanging();
                if (value == null)
                    Action = null;
                else
                    Action = _manager.Actions[value];
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Gets or sets the action associated with this hotkey.
        /// </summary>
        /// <remarks>This property automatically handles changing the <see cref="Pressed"/> event.</remarks>
        public IActionBinding? Action
        {
            get => _action;
            set
            {
                NotifyPropertyChanged();
                if (_action != null)
                    Pressed -= _action.HandleKeyEvent;
                _action = value;
                FLog.Log.Debug($"Hotkey '{ID}' action was changed to '{ActionName}'");
                if (_action != null)
                    Pressed += _action.HandleKeyEvent;
                NotifyPropertyChanging();
            }
        }
        private IActionBinding? _action;

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

        #region Events
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
        /// <summary>Triggered after a property has been set.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>Triggered before a property is about to be set.</summary>
        public event PropertyChangingEventHandler? PropertyChanging;

        private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Settings.Save();
            Settings.Reload();
            FLog.Log.Debug($"{nameof(BindableWindowsHotkey)}:  Saved & Reloaded Hotkey Configuration.");
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        private void ForwardPropertyChanged(object? sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
        private void NotifyPropertyChanging([CallerMemberName] string propertyName = "") => PropertyChanging?.Invoke(this, new(propertyName));
        private void ForwardPropertyChanging(object? sender, PropertyChangingEventArgs e) => PropertyChanging?.Invoke(sender, e);
        #endregion Events

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
        /// <summary>
        /// Serializes this hotkey into a string format.<br/>
        /// This is used when saving the hotkey to the configuration file.
        /// </summary>
        /// <returns>A string containing this hotkey in serialized form.</returns>
        public string Serialize() => $"{Name}{Settings.HotkeyNameSeperatorChar}{Hotkey.ToString()}{Settings.HotkeyNameSeperatorChar}{ActionName}{Settings.HotkeyNameSeperatorChar}{Registered}";
        /// <summary>Gets all properties in a pseudo-json format for dumping them to the log.</summary>
        /// <returns>A human-readable, single-line string that includes all of this hotkey's current values.</returns>
        public string GetFullIdentifier() => $"{{ Name: '{Name}', Keys: '{Hotkey.Serialize()}', Action: '{ActionName}', Registered: '{Registered}' }}";
        /// <summary>
        /// Parse a serialized hotkey string <i>(<paramref name="hkString"/>)</i> into a usable <see cref="BindableWindowsHotkey"/>.<br/>
        /// This is used when loading hotkeys from the configuration file.
        /// </summary>
        /// <param name="hkString">A serialized hotkey string.</param>
        /// <param name="manager">The <see cref="HotkeyManager"/> instance to use when looking up action definitions.</param>
        /// <returns>A <see cref="BindableWindowsHotkey"/> class.</returns>
        public static BindableWindowsHotkey Parse(string hkString, HotkeyManager manager)
        {
            string[]? split = hkString.Split(Settings.HotkeyNameSeperatorChar, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            // Set the hotkey name:
            string name = split[0];

            // Parse the key combination:
            KeyCombo keys = split.Length > 1 ? KeyCombo.Parse(split[1]) : new KeyCombo();

            // Determine which action to bind to:
            string action = split.Length > 2 ? split[2] : string.Empty;

            // Check if the hotkey was registered:
            bool register = false;

            if (split.Length > 3)
                _ = bool.TryParse(split[3], out register);

            return new(manager, name, keys, action, register);
        }
    }
}
