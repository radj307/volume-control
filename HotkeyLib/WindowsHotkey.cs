using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VolumeControl.Log;

namespace HotkeyLib
{
    [TypeConverter(typeof(WindowsHotkeyConverter))]
    public class WindowsHotkey : IMessageFilter, IKeyCombo, IDisposable
    {
        #region Constructors
        public WindowsHotkey(Control owner, string keystr)
        {
            _owner = owner;
            _combo = new(keystr);
            KeysChanged += delegate
            {
                if (Registered)
                    Reregister();
            };
            Application.AddMessageFilter(this);
        }
        public WindowsHotkey(Control owner, Keys key, Modifier mods)
        {
            _owner = owner;
            _combo = new(key, mods);
            KeysChanged += delegate
            {
                if (Registered)
                    Reregister();
            };
            Application.AddMessageFilter(this);
        }
        public WindowsHotkey(Control owner)
        {
            _owner = owner;
            _combo = new();
            KeysChanged += delegate
            {
                if (Registered)
                    Reregister();
            };
            Application.AddMessageFilter(this);
        }
        #endregion Constructors

        #region Finalizers
        ~WindowsHotkey()
        {
            Dispose(true); // unregister
        }
        #endregion Finalizers

        #region Members
        private Control _owner;
        private KeyCombo _combo;
        private HotkeyRegistrationState _state = HotkeyRegistrationState.UNREGISTERED;
        private int? _id = null;
        private bool disposedValue;
        public event KeyEventHandler? Pressed = null;
        public event EventHandler? KeysChanged = null;
        #endregion Members

        #region Properties
        public Keys Key
        {
            get => _combo.Key;
            set
            {
                _combo.Key = value;
                NotifyKeysChanged();
            }
        }
        public Modifier Mod
        {
            get => _combo.Mod;
            set
            {
                _combo.Mod = value;
                NotifyKeysChanged();
            }
        }
        public bool Alt
        {
            get => _combo.Alt;
            set
            {
                _combo.Alt = value;
                NotifyKeysChanged();
            }
        }
        public bool Ctrl
        {
            get => _combo.Ctrl;
            set
            {
                _combo.Ctrl = value;
                NotifyKeysChanged();
            }
        }
        public bool Shift
        {
            get => _combo.Shift;
            set
            {
                _combo.Shift = value;
                NotifyKeysChanged();
            }
        }
        public bool Win
        {
            get => _combo.Win;
            set
            {
                _combo.Win = value;
                NotifyKeysChanged();
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

        #region Methods
        private void NotifyKeysChanged(EventArgs e)
            => KeysChanged?.Invoke(this, e);
        private void NotifyKeysChanged()
            => NotifyKeysChanged(EventArgs.Empty);

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
            else if (!Valid)
            {
                FLog.Log.Warning($"Refusing to register invalid hotkey '{_combo}'");
                return _id;
            }

            _id = HotkeyAPI.GetID();

            if (HotkeyAPI.RegisterHotkey(_owner.Handle, _id.Value, _combo.Mod.ToWindowsModifier(), _combo.Key))
            {
                _state = HotkeyRegistrationState.REGISTERED;
                FLog.Log.Info($"Successfully registered hotkey '{_combo}' with ID '{_id}'");
            }
            else // an error occurred
            {
                int lastError = HotkeyAPI.GetLastWin32Error();

                FLog.Log.Error(
                    $"Hotkey registration failed with code {lastError}! {(lastError == HotkeyAPI.ERROR_HOTKEY_ALREADY_REGISTERED ? "(Hotkey Already Registered)" : "")}",
                    $"Keys:       '{_combo}'",
                    $"Hotkey ID:  '{_id}'"
                );

                _state = HotkeyRegistrationState.FAILED;
                _id = null;
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
                FLog.Log.Info($"Successfully reset the state of hotkey '{_combo}' after a failed registration attempt.");
                return;
            }

            if (!_owner.IsDisposed && _id != null)
            {
                if (HotkeyAPI.UnregisterHotkey(_owner.Handle, _id.Value))
                {
                    _state = HotkeyRegistrationState.UNREGISTERED;
                    FLog.Log.Info($"Successfully unregistered hotkey '{_combo}' with ID '{_id}'");
                }
                else
                {
                    FLog.Log.Error(
                        $"Hotkey unregistration failed with code {HotkeyAPI.GetLastWin32Error()}!",
                        $"Keys:       '{_combo}'",
                        $"Hotkey ID:  '{_id}'"
                    );
                    _id = null;
                    _state = HotkeyRegistrationState.FAILED;
                }
            }

            _id = null;
        }
        public void Reregister()
        {
            if (_state == HotkeyRegistrationState.REGISTERED)
            {
                if (!_owner.IsDisposed && _id != null)
                {
                    if (!HotkeyAPI.UnregisterHotkey(_owner.Handle, _id.Value))
                    {
                        FLog.Log.Error(
                            $"Hotkey re-registration failed with code {HotkeyAPI.GetLastWin32Error()}!",
                            $"Keys:       '{_combo}'",
                            $"Hotkey ID:  '{_id}'"
                        );
                        _state = HotkeyRegistrationState.FAILED;
                        _id = null;
                    }
                    else if (HotkeyAPI.RegisterHotkey(_owner.Handle, _id.Value, _combo.Mod.ToWindowsModifier(), _combo.Key))
                    {
                        FLog.Log.Info($"Successfully re-registered hotkey '{_combo}'");
                        // state is still correct
                    }
                    else
                    {
                        var lastError = HotkeyAPI.GetLastWin32Error();
                        FLog.Log.Error($"Hotkey re-registration failed with code {lastError}! {(lastError == HotkeyAPI.GetLastWin32Error() ? "(Hotkey Already Registered)" : "")}");
                        _state = HotkeyRegistrationState.FAILED;
                        _id = null;
                    }
                }
                else FLog.Log.Error("Cannot re-register invalid hotkey!");
            }
            else
            {
                FLog.Log.Warning("Cannot re-register hotkey that isn't already registered!");
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
            FLog.Log.Debug($"Hotkey '{_combo}' pressed.");
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
        /// <summary>
        /// Dispose of this object.
        /// </summary>
        /// <param name="disposing">When true, any hotkeys are unregistered before disposal.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Unregister();
                }

                _owner = null!;
                _id = null;
                disposedValue = true;
            }
        }
        /// <summary>
        /// Dispose of this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Methods
    }
}
