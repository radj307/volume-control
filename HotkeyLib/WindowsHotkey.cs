using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VolumeControl.Log;

namespace HotkeyLib
{
    [TypeConverter(typeof(WindowsHotkeyConverter))]
    public class WindowsHotkey : IKeyCombo, IDisposable
    {
        #region Constructors
        public WindowsHotkey(IntPtr owner, IKeyCombo keys)
        {
            _owner = owner;
            _combo = keys;
            KeysChanged += delegate
            {
                if (Registered)
                    Reregister();
            };
            ID = HotkeyAPI.GetID();
        }

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _combo.PropertyChanged += value;
            }

            remove
            {
                _combo.PropertyChanged -= value;
            }
        }
        #endregion Constructors

        #region Finalizers
        ~WindowsHotkey()
        {
            Dispose(true); // unregister
        }
        #endregion Finalizers

        #region Members
        private readonly IntPtr _owner;
        private readonly IKeyCombo _combo;
        private HotkeyRegistrationState _state = HotkeyRegistrationState.UNREGISTERED;
        /// <summary>
        /// This is the hotkey identifier number of this hotkey.
        /// </summary>
        /// <remarks>Each hotkey is given an identifier when constructed, it cannot be changed later.</remarks>
        public readonly int ID;
        private bool disposedValue;
        /// <summary>
        /// Triggered when the hotkey is pressed.
        /// </summary>
        public event KeyEventHandler? Pressed = null;
        /// <summary>
        /// Triggered when the hotkey's <see cref="KeyCombo"/> is changed.
        /// </summary>
        /// <remarks>This can be triggered by any of the related properties.</remarks>
        public event EventHandler? KeysChanged = null;
        #endregion Members

        #region Properties
        /// <inheritdoc/>
        public Keys Key
        {
            get => _combo.Key;
            set
            {
                _combo.Key = value;
                NotifyKeysChanged();
            }
        }
        /// <inheritdoc/>
        public Modifier Mod
        {
            get => _combo.Mod;
            set
            {
                _combo.Mod = value;
                NotifyKeysChanged();
            }
        }
        /// <inheritdoc/>
        public bool Alt
        {
            get => _combo.Alt;
            set
            {
                _combo.Alt = value;
                NotifyKeysChanged();
            }
        }
        /// <inheritdoc/>
        public bool Ctrl
        {
            get => _combo.Ctrl;
            set
            {
                _combo.Ctrl = value;
                NotifyKeysChanged();
            }
        }
        /// <inheritdoc/>
        public bool Shift
        {
            get => _combo.Shift;
            set
            {
                _combo.Shift = value;
                NotifyKeysChanged();
            }
        }
        /// <inheritdoc/>
        public bool Win
        {
            get => _combo.Win;
            set
            {
                _combo.Win = value;
                NotifyKeysChanged();
            }
        }
        /// <inheritdoc/>
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
        public void NotifyPressed(HandledEventArgs e) => Pressed?.Invoke(this, e);

        public int? Register()
        {
            if (_state == HotkeyRegistrationState.REGISTERED)
                return ID;

            if (!Valid)
            {
                FLog.Log.Warning($"Refusing to register invalid hotkey '{_combo}'");
                return ID;
            }

            //ID = HotkeyAPI.GetID();

            if (HotkeyAPI.RegisterHotkey(_owner, ID, _combo.Mod.ToWindowsModifier(), _combo.Key))
            {
                _state = HotkeyRegistrationState.REGISTERED;
                FLog.Log.Info($"Successfully registered hotkey '{_combo}' with ID '{ID}' ({_owner})");
            }
            else // an error occurred
            {
                var (code, msg) = HotkeyAPI.GetLastWin32Error();
                FLog.Log.Error(
                    $"Hotkey registration failed with code {code} ({msg})!",
                    $"Keys:       '{_combo}'",
                    $"Hotkey ID:  '{ID}'"
                );

                _state = HotkeyRegistrationState.FAILED;
                return null;
            }

            return ID;
        }
        public void Unregister()
        {
            if (_state == HotkeyRegistrationState.UNREGISTERED)
                return;
            else if (_state == HotkeyRegistrationState.FAILED)
            {
                //ID = null;
                _state = HotkeyRegistrationState.UNREGISTERED;
                FLog.Log.Info($"Successfully reset the state of hotkey '{_combo}' after a failed registration attempt.");
                return;
            }

            //if (ID != null)
            {
                if (HotkeyAPI.UnregisterHotkey(_owner, ID))
                {
                    _state = HotkeyRegistrationState.UNREGISTERED;
                    FLog.Log.Info($"Successfully unregistered hotkey '{_combo}' with ID '{ID}' ({_owner})");
                }
                else
                {
                    var (code, msg) = HotkeyAPI.GetLastWin32Error();
                    FLog.Log.Error(
                        $"Hotkey unregistration failed with code {code} ({msg})!",
                        $"Keys:       '{_combo}'",
                        $"Hotkey ID:  '{ID}'"
                    );
                    //ID = null;
                    _state = HotkeyRegistrationState.FAILED;
                }
            }

            //ID = null;
        }
        public void Reregister()
        {
            if (_state == HotkeyRegistrationState.REGISTERED)
            {
                //if (ID != null)
                {
                    if (!HotkeyAPI.UnregisterHotkey(_owner, ID))
                    {
                        var (code, msg) = HotkeyAPI.GetLastWin32Error();
                        FLog.Log.Error(
                            $"Hotkey re-registration failed with code {code} ({msg})!",
                            $"Keys:       '{_combo}'",
                            $"Hotkey ID:  '{ID}'"
                        );
                        _state = HotkeyRegistrationState.FAILED;
                        //ID = null;
                    }
                    else if (HotkeyAPI.RegisterHotkey(_owner, ID, _combo.Mod.ToWindowsModifier(), _combo.Key))
                    {
                        FLog.Log.Info($"Successfully re-registered hotkey '{_combo}'");
                        // state is still correct
                    }
                    else
                    {
                        var (code, msg) = HotkeyAPI.GetLastWin32Error();
                        FLog.Log.Error($"Hotkey re-registration failed with code {code} ({msg})!");
                        _state = HotkeyRegistrationState.FAILED;
                        //ID = null;
                    }
                }
                //else FLog.Log.Error("Cannot re-register invalid hotkey!");
            }
            else
            {
                FLog.Log.Warning("Cannot re-register hotkey that isn't already registered!");
            }
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

                //ID = null;
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
