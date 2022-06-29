using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Forms;
using VolumeControl.Log;

namespace HotkeyLib
{
    /// <summary>
    /// Handler type for a key press event.
    /// </summary>
    /// <param name="sender">The object that sent the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void KeyEventHandler(object? sender, HandledEventArgs e);

    /// <summary>
    /// Abstract base hotkey class.
    /// </summary>
    public abstract class Hotkey : IKeyCombo, IDisposable, INotifyPropertyChanged
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="Hotkey"/> instance.
        /// </summary>
        public Hotkey()
        {
            ID = HotkeyAPI.GetID();
            PropertyChanged += (s, e) => Reregister();
        }
        #endregion Constructor

        #region Fields
        private HotkeyRegistrationState _state;
        #endregion Fields

        #region Properties
        /// <summary>
        /// The owner's handle for use with Windows API hotkey registration.
        /// </summary>
        public abstract IntPtr Owner { get; }
        /// <summary>
        /// Gets or sets whether the hotkey is currently registered (active) or not.
        /// </summary>
        public bool Registered
        {
            get => _state.Equals(HotkeyRegistrationState.REGISTERED);
            set
            {
                if (value)
                    Register();
                else
                    Unregister();
            }
        }
        /// <summary>
        /// The primary key.
        /// </summary>
        public Keys Key { get; set; } = Keys.None;
        /// <summary>
        /// Modifier keys.
        /// </summary>
        public Modifier Mod { get; set; } = Modifier.NONE;
        /// <summary>
        /// Gets or sets whether the <see cref="Mod"/> property contains the <see cref="Modifier.ALT"/> bit.
        /// </summary>
        [JsonIgnore]
        public bool Alt
        {
            get => Mod.HasFlag(Modifier.ALT);
            set
            {
                if (value)
                    Mod |= Modifier.ALT;
                else
                    Mod &= ~Modifier.ALT;
            }
        }
        /// <summary>
        /// Gets or sets whether the <see cref="Mod"/> property contains the <see cref="Modifier.SHIFT"/> bit.
        /// </summary>
        [JsonIgnore]
        public bool Shift
        {
            get => Mod.HasFlag(Modifier.SHIFT);
            set
            {
                if (value)
                    Mod |= Modifier.SHIFT;
                else
                    Mod &= ~Modifier.SHIFT;
            }
        }
        /// <summary>
        /// Gets or sets whether the <see cref="Mod"/> property contains the <see cref="Modifier.CTRL"/> bit.
        /// </summary>
        [JsonIgnore]
        public bool Ctrl
        {
            get => Mod.HasFlag(Modifier.CTRL);
            set
            {
                if (value)
                    Mod |= Modifier.CTRL;
                else
                    Mod &= ~Modifier.CTRL;
            }
        }
        /// <summary>
        /// Gets or sets whether the <see cref="Mod"/> property contains the <see cref="Modifier.WIN"/> bit.
        /// </summary>
        [JsonIgnore]
        public bool Win
        {
            get => Mod.HasFlag(Modifier.WIN);
            set
            {
                if (value)
                    Mod |= Modifier.WIN;
                else
                    Mod &= ~Modifier.WIN;
            }
        }
        /// <summary>
        /// Gets this hotkey's unique ID.
        /// </summary>
        [JsonIgnore]
        public int ID { get; }
        /// <summary>
        /// Gets whether the hotkey has a valid primary key or not.
        /// </summary>
        [JsonIgnore]
        public bool Valid => !Key.Equals(Keys.None);
        #endregion Properties

        #region Events
#       pragma warning disable CS0067
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
#       pragma warning restore CS0067
        /// <summary>
        /// Triggered when the hotkey is pressed.
        /// </summary>
        public abstract event KeyEventHandler? Pressed;
        #endregion Events

        #region Registration
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
                FLog.Log.Warning($"Refusing to register invalid hotkey {ID}");
                return false;
            }

            if (HotkeyAPI.RegisterHotkey(Owner, ID, Mod.ToWindowsModifier(), Key))
            {
                _state = HotkeyRegistrationState.REGISTERED;
                FLog.Log.Info($"Successfully registered hotkey {ID}  (Owner: {Owner})");
            }
            else // an error occurred
            {
                (int code, string msg) = HotkeyAPI.GetLastWin32Error();
                FLog.Log.Error(
                    $"Hotkey registration failed with code {code} ({msg})!",
                    $"Key:        '{Key}'",
                    $"Modifier:   '{Mod}'",
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
            {
                return false;
            }
            // If the previous registration attempt failed, reset the state
            else if (_state == HotkeyRegistrationState.FAILED)
            {
                _state = HotkeyRegistrationState.UNREGISTERED;
                FLog.Log.Info($"Successfully reset the state of hotkey {ID} after a failed registration attempt.");
            }
            // Unregister the hotkey:
            else if (HotkeyAPI.UnregisterHotkey(Owner, ID))
            {
                _state = HotkeyRegistrationState.UNREGISTERED;
                FLog.Log.Info($"Successfully unregistered hotkey {ID}  (Owner: {Owner})");
            }
            else
            {
                (int code, string msg) = HotkeyAPI.GetLastWin32Error();
                FLog.Log.Error(
                    $"Hotkey unregistration failed with code {code} ({msg})!",
                    $"Key:        '{Key}'",
                    $"Modifier:   '{Mod}'",
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
                if (!HotkeyAPI.UnregisterHotkey(Owner, ID))
                {
                    (int code, string msg) = HotkeyAPI.GetLastWin32Error();
                    FLog.Log.Error(
                        $"Hotkey re-registration failed with code {code} ({msg})!",
                        $"Key:        '{Key}'",
                        $"Modifier:   '{Mod}'",
                        $"Hotkey ID:  '{ID}'"
                    );
                    _state = HotkeyRegistrationState.FAILED;
                    return false;
                }
                if (HotkeyAPI.RegisterHotkey(Owner, ID, Mod.ToWindowsModifier(), Key))
                {
                    FLog.Log.Info($"Successfully re-registered hotkey {ID}");
                    return true;
                }
                else
                {
                    (int code, string msg) = HotkeyAPI.GetLastWin32Error();
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

        /// <summary>
        /// Unregisters the hotkey.
        /// </summary>
        public void Dispose()
        {
            Unregister();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Unregisters the hotkey.
        /// </summary>
        ~Hotkey() => Dispose();
        #endregion Registration
    }
}
