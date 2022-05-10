using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace HotkeyLib
{
    /// <summary>
    /// Represents a combination of one keyboard key and any number of modifier keys.
    /// </summary>
    public struct KeyCombo : IKeyCombo
    {
        #region Constructors
        /// <summary>
        /// String Constructor.
        /// </summary>
        /// <param name="keystr">A string in the format "<KEY>[+<MOD>...]"</param>
        public KeyCombo(string keystr)
        {
            int div = keystr.IndexOf('+');
            // set key
            if (Enum.TryParse(typeof(Keys), div == -1 ? keystr : keystr[(div + 1)..], out object? key) && key != null)
                _key = (Keys)key;
            else _key = Keys.None;

            // set modifiers
            _mod = Modifier.NONE;
            if (div != -1)
            {
                string mod = keystr[..div];
                if (mod.Contains("Alt", StringComparison.OrdinalIgnoreCase))
                    _mod.Set(Modifier.ALT);
                if (mod.Contains("Ctrl", StringComparison.OrdinalIgnoreCase))
                    _mod.Set(Modifier.CTRL);
                if (mod.Contains("Shift", StringComparison.OrdinalIgnoreCase))
                    _mod.Set(Modifier.SHIFT);
                if (mod.Contains("Win", StringComparison.OrdinalIgnoreCase))
                    _mod.Set(Modifier.WIN);
            }
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">A Non-Modifier Keyboard Key.</param>
        /// <param name="mods">A bitfield where the first 4 bits represent the various modifier keys.</param>
        public KeyCombo(Keys key, Modifier mods)
        {
            _key = key;
            _mod = mods;
        }
        /// <summary>
        /// Default Constructor.
        /// 
        /// Key = Keys.None;
        /// Modifier = Modifier.NONE;
        /// </summary>
        public KeyCombo() : this(Keys.None, Modifier.NONE) { }
        #endregion Constructors

        #region Members
        private Keys _key;
        private Modifier _mod;

        public event PropertyChangedEventHandler? PropertyChanged = null;
        #endregion Members

        #region Properties
        /// <summary>
        /// The primary key on the Keyboard.
        /// </summary>
        public Keys Key
        {
            get => _key;
            set
            {
                _key = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Valid));
            }
        }
        /// <summary>
        /// The modifier key bitfield.
        /// </summary>
        public Modifier Mod
        {
            get => _mod;
            set
            {
                _mod = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Get or set whether the Alt modifier key is enabled.
        /// This corresponds to the first bit in the modifier key bitfield.
        /// Both the left and right modifier keys apply here, there is no method available to limit the key combination to only the left or right modifier key.
        /// </summary>
        public bool Alt
        {
            get => _mod.Contains(Modifier.ALT);
            set
            {
                _mod.Apply(Modifier.ALT, value);
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Get or set whether the Ctrl modifier key is enabled.
        /// This corresponds to the second bit in the modifier key bitfield.
        /// Both the left and right modifier keys apply here, there is no method available to limit the key combination to only the left or right modifier key.
        /// </summary>
        public bool Ctrl
        {
            get => _mod.Contains(Modifier.CTRL);
            set
            {
                _mod.Apply(Modifier.CTRL, value);
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Get or set whether the Shift modifier key is enabled.
        /// This corresponds to the third bit in the modifier key bitfield.
        /// Both the left and right modifier keys apply here, there is no method available to limit the key combination to only the left or right modifier key.
        /// </summary>
        public bool Shift
        {
            get => _mod.Contains(Modifier.SHIFT);
            set
            {
                _mod.Apply(Modifier.SHIFT, value);
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Get or set whether the "Windows Key" (super) modifier key is enabled.
        /// This corresponds to the fourth bit in the modifier key bitfield.
        /// Both the left and right modifier keys apply here, there is no method available to limit the key combination to only the left or right modifier key.
        /// </summary>
        public bool Win
        {
            get => _mod.Contains(Modifier.WIN);
            set
            {
                _mod.Apply(Modifier.WIN, value);
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public bool Valid
        {
            get => !_key.Equals(Keys.None);
        }
        #endregion Properties

        #region Methods
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        /// <summary>
        /// Converts this key combination into a readable/writable string, formatted as "<KEY>[+<MOD>...]"
        /// </summary>
        /// <returns>A valid hotkey string representation.</returns>
        public override string? ToString() => $"{(_mod.Empty() ? "" : $"{_mod.Stringify()}+")}{Enum.GetName(typeof(Keys), _key)}";

        /// <summary>
        /// Parse the given string into a <see cref="KeyCombo"/>.
        /// </summary>
        /// <param name="keystr">Input String</param>
        /// <param name="sCompareType">String Comparison Type</param>
        /// <returns><see cref="KeyCombo"/></returns>
        public static KeyCombo Parse(string keystr, StringComparison sCompareType = StringComparison.OrdinalIgnoreCase)
        {
            Keys key = Keys.None;
            Modifier mod = Modifier.NONE;

            foreach (string s in keystr.Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                if (s.Equals("Alt", sCompareType))
                    mod |= Modifier.ALT;
                else if (s.Equals("Ctrl", sCompareType))
                    mod |= Modifier.CTRL;
                else if (s.Equals("Shift", sCompareType))
                    mod |= Modifier.SHIFT;
                else if (s.Equals("Win", sCompareType))
                    mod |= Modifier.WIN;
                else if (Enum.TryParse(typeof(Keys), s, out object? keyObj) && keyObj is Keys k)
                    key = k;
            }

            return new(key, mod);
        }
        #endregion Methods
    }

    /// <summary>
    /// Handler type for a key press event.
    /// </summary>
    /// <param name="sender">The object that sent the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void KeyEventHandler(object? sender, HandledEventArgs e);
}
