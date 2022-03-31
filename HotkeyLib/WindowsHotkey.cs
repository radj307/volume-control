using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HotkeyLib
{
    [TypeConverter(typeof(WindowsHotkeyConverter))]
    public class WindowsHotkey : IMessageFilter, IKeyCombo
    {
        #region Constructors
        public WindowsHotkey(Control owner, string keystr)
        {
            _owner = owner; _combo = new(keystr);
            Application.AddMessageFilter(this);
        }
        public WindowsHotkey(Control owner, Keys key, Modifier mods)
        {
            _owner = owner; _combo = new(key, mods);
            Application.AddMessageFilter(this);
        }
        public WindowsHotkey(Control owner)
        {
            _owner = owner; _combo = new();
            Application.AddMessageFilter(this);
        }
        #endregion Constructors

        #region Destructors
        ~WindowsHotkey()
        {
            Unregister();
        }
        #endregion Destructors

        #region Members
        private Control _owner;
        private KeyCombo _combo;
        private HotkeyRegistrationState _state = HotkeyRegistrationState.UNREGISTERED;
        private int? _id = null;

        public event KeyEventHandler? Pressed = null;
        #endregion Members

        #region Methods
        /// <summary>
        /// Set the owner control handle to a new handle, and return the previous one.
        /// </summary>
        /// <param name="newHwnd">Handle of the new owner object.</param>
        /// <returns>The previous owner's handle.</returns>
        public Control SwapOwner(Control newHwnd)
        {
            Control prev = _owner;
            _owner = newHwnd;
            return prev;
        }
        public int? Register(Control? owner = null)
        {
            if (_state == HotkeyRegistrationState.REGISTERED)
                return _id;

            if (_owner.IsDisposed)
            {
                if (owner != null)
                    _owner = owner;
                else throw new Exception("Cannot register a hotkey for a window that has been disposed of!");
            }

            _id = HotkeyAPI.GetID();

            if (HotkeyAPI.RegisterHotKey(_owner.Handle, _id.Value, _combo.Mod.ToWindowsModifier(), _combo.Key) != 0)
                _state = HotkeyRegistrationState.REGISTERED;
            else // an error occurred
            {
                int lastError = Marshal.GetLastWin32Error(), copyID = _id.Value;

                _state = HotkeyRegistrationState.FAILED;
                _id = null;

                // throw an exception if the error wasn't because the hotkey was already registered
                if (lastError != HotkeyAPI.ERROR_HOTKEY_ALREADY_REGISTERED)
                    throw new Win32Exception($"Failed to register hotkey ({_combo}) with ID {copyID}:  {lastError}");
            }

            return _id;
        }
        public void Unregister()
        {
            if (_state == HotkeyRegistrationState.UNREGISTERED)
                return;
            else if (_state == HotkeyRegistrationState.FAILED)
            {
                _id = null;
                _state = HotkeyRegistrationState.UNREGISTERED;
                return;
            }

            if (!_owner.IsDisposed && _id != null)
            {
                if (HotkeyAPI.UnregisterHotKey(_owner.Handle, _id.Value) != 0)
                    _state = HotkeyRegistrationState.UNREGISTERED;
                else
                {
                    int copyID = _id.Value;
                    _id = null;
                    throw new Win32Exception($"Failed to unregister hotkey ({_combo}) with ID {copyID}!");
                }
            }

            _id = null;
        }
        public void Reregister()
        {
            if (_state == HotkeyRegistrationState.REGISTERED)
            {
                Unregister();
                if (!_owner.IsDisposed)
                    Register();
            }
        }
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg != HotkeyAPI.WM_HOTKEY)
                return false;
            else if (_state == HotkeyRegistrationState.REGISTERED && m.WParam.ToInt32() == _id)
                return OnPressed();
            else return false;
        }
        private bool OnPressed()
        {
            HandledEventArgs args = new(false);
            Pressed?.Invoke(this, args);
            return args.Handled;
        }
        /// <summary>
        /// Converts this hotkey's key combination into a readable/writable string, formatted as "<KEY>[+<MOD>...]"
        /// </summary>
        /// <returns>A valid hotkey string representation.</returns>
        public new string? ToString()
        {
            return _combo.ToString();
        }
        #endregion Methods

        #region Properties
        public Keys Key
        {
            get => _combo.Key;
            set
            {
                bool registered = _state == HotkeyRegistrationState.REGISTERED;
                if (registered)
                    Unregister();
                _combo.Key = value;
                if (registered)
                    Register();
            }
        }
        public Modifier Mod
        {
            get => _combo.Mod;
            set
            {
                bool registered = _state == HotkeyRegistrationState.REGISTERED;
                if (registered)
                    Unregister();
                _combo.Mod = value;
                if (registered)
                    Register();
            }
        }
        public bool Alt
        {
            get => _combo.Alt;
            set
            {
                bool registered = _state == HotkeyRegistrationState.REGISTERED;
                if (registered)
                    Unregister();
                _combo.Alt = value;
                if (registered)
                    Register();
            }
        }
        public bool Ctrl
        {
            get => _combo.Ctrl;
            set
            {
                bool registered = _state == HotkeyRegistrationState.REGISTERED;
                if (registered)
                    Unregister();
                _combo.Ctrl = value;
                if (registered)
                    Register();
            }
        }
        public bool Shift
        {
            get => _combo.Shift;
            set
            {
                bool registered = _state == HotkeyRegistrationState.REGISTERED;
                if (registered)
                    Unregister();
                _combo.Shift = value;
                if (registered)
                    Register();
            }
        }
        public bool Win
        {
            get => _combo.Win;
            set
            {
                bool registered = _state == HotkeyRegistrationState.REGISTERED;
                if (registered)
                    Unregister();
                _combo.Win = value;
                if (registered)
                    Register();
            }
        }
        public bool Valid => _combo.Valid;
        public bool Registered
        {
            get => _state == HotkeyRegistrationState.REGISTERED;
            set
            {
                if (value)
                    Register();
                else Unregister();
            }
        }
        #endregion Properties
    }
}
