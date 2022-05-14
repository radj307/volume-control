using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VolumeControl.Log;

namespace HotkeyLib
{
    [TypeConverter(typeof(WindowsHotkeyConverter))]
    public class WindowsHotkey : IKeyCombo, IDisposable, INotifyPropertyChanged
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

            // bind the key combination's property changed event so it triggers our property changed event
            _combo.PropertyChanged += (s, e) => PropertyChanged?.Invoke(s, e);
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
        /// <summary>
        /// Triggered when any of the object's properties change.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged = null;
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
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Valid));
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }
        #endregion Properties

        #region Methods
        private void NotifyKeysChanged(EventArgs e)
            => KeysChanged?.Invoke(this, e);
        private void NotifyKeysChanged()
            => NotifyKeysChanged(EventArgs.Empty);
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        public void NotifyPressed(HandledEventArgs e) => Pressed?.Invoke(this, e);

        /// <summary>
        /// Registers the hotkey with the Windows API.
        /// </summary>
        /// <remarks>This function automatically writes errors to the log file.</remarks>
        /// <returns>True if successful, false if an error occurred.</returns>
        internal virtual bool Register()
        {
            if (_state == HotkeyRegistrationState.REGISTERED)
                return false;

            if (!Valid)
            {
                FLog.Log.Warning($"Refusing to register invalid hotkey '{_combo}'");
                return false;
            }

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
                return false;
            }
            return true;
        }
        /// <summary>
        /// Unregisters the hotkey with the Windows API.
        /// </summary>
        /// <remarks>This function automatically writes errors to the log file.</remarks>
        /// <returns>True if successful, false if an error occurred.</returns>
        internal virtual bool Unregister()
        {
            // if the hotkey is already registered then there is nothing to do.
            if (_state == HotkeyRegistrationState.UNREGISTERED)
                return false;
            // If the previous registration attempt failed, reset the state
            else if (_state == HotkeyRegistrationState.FAILED)
            {
                _state = HotkeyRegistrationState.UNREGISTERED;
                FLog.Log.Info($"Successfully reset the state of hotkey '{_combo}' after a failed registration attempt.");
            }
            // Unregister the hotkey:
            else if (HotkeyAPI.UnregisterHotkey(_owner, ID))
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

                _state = HotkeyRegistrationState.FAILED;
                return false;
            }
            return true;
        }
        /// <summary>
        /// Re-Registers the hotkey with the Windows API, which is useful when the key combination was changed.
        /// </summary>
        /// <remarks>This function automatically writes errors to the log file.</remarks>
        /// <returns>True if successful, false if an error occurred.</returns>
        internal virtual bool Reregister()
        {
            if (_state == HotkeyRegistrationState.REGISTERED)
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
                    return false;
                }
                if (HotkeyAPI.RegisterHotkey(_owner, ID, _combo.Mod.ToWindowsModifier(), _combo.Key))
                {
                    FLog.Log.Info($"Successfully re-registered hotkey '{_combo}'");
                    return true;
                }
                else
                {
                    var (code, msg) = HotkeyAPI.GetLastWin32Error();
                    FLog.Log.Error($"Hotkey re-registration failed with code {code} ({msg})!");
                    _state = HotkeyRegistrationState.FAILED;
                    return false;
                }
            }
            else
            {
                FLog.Log.Warning("Cannot re-register hotkey that isn't already registered!");
            }
            return false;
        }
        public new string? ToString() => Serialize();
        /// <summary>
        /// Converts this hotkey to a string with a serialized representation of the key combination.
        /// </summary>
        /// <remarks>This calls <see cref="IKeyCombo.Serialize"/> internally.</remarks>
        /// <returns><see cref="string"/> containing the serialized key representation.</returns>
        public string Serialize() => _combo.Serialize();
        /// <summary>
        /// Gets a verbose description of the hotkey for use with logging functions.
        /// </summary>
        /// <returns><see cref="string"/> containing a verbose description of this hotkey and its current values.</returns>
        public virtual string GetFullIdentifier() => $"{{ ID: '{ID}', Keys: '{Serialize()}' }}";
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
