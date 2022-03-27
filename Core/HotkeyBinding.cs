using HotkeyLib;
using System.ComponentModel;

namespace Core
{
    public class HotkeyBinding
    {

        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="ownerControl">The owner control of this hotkey.</param>
        /// <param name="enabled">When true, the hotkey will start out registered. (enabled)</param>
        /// <param name="key">Keyboard Key</param>
        /// <param name="shift">When true, the hotkey requires the shift modifier.</param>
        /// <param name="ctrl">When true, the hotkey requires the ctrl modifier.</param>
        /// <param name="alt">When true, the hotkey requires the alt modifier.</param>
        /// <param name="win">When true, the hotkey requires the super modifier.</param>
        /// <param name="pressed">An optional action to trigger when the hotkey is pressed.</param>
        public HotkeyBinding(Control ownerControl,
                             bool enabled = false,
                             Keys key = Keys.None,
                             bool shift = false,
                             bool ctrl = false,
                             bool alt = false,
                             bool win = false,
                             HandledEventHandler? pressed = null)
        {
            owner = ownerControl;
            Hotkey = new(key, shift, ctrl, alt, win, pressed);
            Registered = enabled;
        }
        /// <summary>
        /// String Constructor.
        /// Parses the given string into a usable Hotkey.
        /// </summary>
        /// <param name="ownerControl">The owner control of this hotkey.</param>
        /// <param name="enabled">When true, the hotkey will start out registered. (enabled)</param>
        /// <param name="hkstring">A valid string representing a hotkey. If the string is invalid, an exception may be thrown.</param>
        /// <param name="pressed">An optional action to trigger when the hotkey is pressed.</param>
        public HotkeyBinding(Control ownerControl, bool enabled, string hkstring, HandledEventHandler? pressed = null)
        {
            owner = ownerControl;
            Hotkey = new(hkstring, pressed);
            Registered = enabled;
        }

        private readonly Control owner;
        public readonly Hotkey Hotkey;

        public event HandledEventHandler Pressed
        {
            add => Hotkey.Pressed += value;
            remove => Hotkey.Pressed -= value;
        }

        public bool Registered
        {
            get => Hotkey.Registered;
            set
            {
                if (value)
                    Hotkey.TryRegister(owner);
                else if (Hotkey.Registered)
                    Hotkey.TryUnregister();
            }
        }
    }
}
