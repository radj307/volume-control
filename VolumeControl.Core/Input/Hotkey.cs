using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Interop;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Core.Input.Enums;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// A hotkey that executes an action when pressed.
    /// </summary>
    public class Hotkey : IHotkey, INotifyPropertyChanged, IDisposable
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="Hotkey"/> instance with the specified parameters.
        /// </summary>
        /// <param name="name">The name of the hotkey.</param>
        /// <param name="key">The primary key of the hotkey combination.</param>
        /// <param name="modifiers">The modifier keys of the hotkey combination.</param>
        /// <param name="isRegistered">The registration state of the hotkey.</param>
        /// <param name="actionInstance">The action to trigger when the hotkey is pressed.</param>
        public Hotkey(string name, EFriendlyKey key, EModifierKey modifiers, bool isRegistered, HotkeyActionInstance actionInstance)
        {
            ID = WindowsHotkeyAPI.NextID; //< get assigned a unique ID
            Name = name;
            Key = key;
            Modifiers = modifiers;
            Action = actionInstance;
            IsRegistered = isRegistered;
        }
        /// <inheritdoc cref="Hotkey(string, EFriendlyKey, EModifierKey, bool, HotkeyActionInstance)"/>
        public Hotkey(string name, EFriendlyKey key, EModifierKey modifiers, bool isRegistered) : this(name, key, modifiers, isRegistered, null!) { }
        /// <inheritdoc cref="Hotkey(string, EFriendlyKey, EModifierKey, bool, HotkeyActionInstance)"/>
        public Hotkey(EFriendlyKey key, EModifierKey modifiers, bool isRegistered, HotkeyActionInstance actionInstance) : this(string.Empty, key, modifiers, isRegistered, actionInstance) { }
        /// <inheritdoc cref="Hotkey(string, EFriendlyKey, EModifierKey, bool, HotkeyActionInstance)"/>
        public Hotkey(EFriendlyKey key, EModifierKey modifiers, bool isRegistered) : this(key, modifiers, isRegistered, null!) { }
        #endregion Constructor

        #region Properties
        /// <inheritdoc/>
        public ushort ID { get; private set; }
        /// <inheritdoc/>
        public string Name { get; set; }
        /// <inheritdoc/>
        public EFriendlyKey Key
        {
            get => _key;
            set
            {
                if (value == _key) return;

                _key = value;
                Reregister(); //< reregister the hotkey
                NotifyPropertyChanged();
            }
        }
        private EFriendlyKey _key;
        /// <inheritdoc/>
        public EModifierKey Modifiers
        {
            get => _modifiers;
            set
            {
                if (value == _modifiers) return;

                var changedFlags = _modifiers ^ value; //< XOR
                _modifiers = value;
                Reregister(); //< reregister the hotkey
                NotifyPropertyChanged();

                // notify changed flags
                if (changedFlags.HasFlag(EModifierKey.Alt))
                    NotifyPropertyChanged(nameof(Alt));
                if (changedFlags.HasFlag(EModifierKey.Shift))
                    NotifyPropertyChanged(nameof(Shift));
                if (changedFlags.HasFlag(EModifierKey.Ctrl))
                    NotifyPropertyChanged(nameof(Ctrl));
                if (changedFlags.HasFlag(EModifierKey.Super))
                    NotifyPropertyChanged(nameof(Win));
                if (changedFlags.HasFlag(EModifierKey.NoRepeat))
                    NotifyPropertyChanged(nameof(NoRepeat));
            }
        }
        private EModifierKey _modifiers;
        /// <inheritdoc/>
        public bool Alt
        {
            get => Modifiers.HasFlag(EModifierKey.Alt);
            set => Modifiers = Modifiers.SetFlagState(EModifierKey.Alt, value);
        }
        /// <inheritdoc/>
        public bool Shift
        {
            get => Modifiers.HasFlag(EModifierKey.Shift);
            set => Modifiers = Modifiers.SetFlagState(EModifierKey.Shift, value);
        }
        /// <inheritdoc/>
        public bool Ctrl
        {
            get => Modifiers.HasFlag(EModifierKey.Ctrl);
            set => Modifiers = Modifiers.SetFlagState(EModifierKey.Ctrl, value);
        }
        /// <inheritdoc/>
        public bool Win
        {
            get => Modifiers.HasFlag(EModifierKey.Super);
            set => Modifiers = Modifiers.SetFlagState(EModifierKey.Super, value);
        }
        /// <inheritdoc/>
        public bool NoRepeat
        {
            get => Modifiers.HasFlag(EModifierKey.NoRepeat);
            set => Modifiers = Modifiers.SetFlagState(EModifierKey.NoRepeat, value);
        }
        /// <inheritdoc/>
        public HwndSourceHook MessageHook => MessageHookImpl;
        /// <inheritdoc/>
        public bool IsRegistered
        {
            get => _isRegistered;
            set
            {
                if (value == _isRegistered) return; //< don't re-register if nothing changed

                if (value)
                { // register
                    if (Register())
                        _isRegistered = true;
                }
                else
                { // unregister
                    Unregister();
                    _isRegistered = false;
                }

                NotifyPropertyChanged();
            }
        }
        private bool _isRegistered;
        /// <inheritdoc/>
        public HotkeyActionInstance? Action
        {
            get => _action;
            set
            {
                _action = value;
                NotifyPropertyChanged();
            }
        }
        private HotkeyActionInstance? _action;
        #endregion Properties

        #region Events
        /// <summary>
        /// Occurs when the hotkey was pressed, after the Action has been triggered.
        /// </summary>
        public event HotkeyPressedEventHandler? Pressed;
        internal void NotifyPressed(object? sender, HotkeyPressedEventArgs e) => Pressed?.Invoke(sender, e);
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Triggers the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that was changed.</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        /// <inheritdoc/>
        public event HotkeyRegisteringEventHandler? Registering;
        /// <returns><see cref="HandledEventArgs.Handled"/></returns>
        private HotkeyRegisteringEventArgs NotifyRegistering()
        {
            var args = new HotkeyRegisteringEventArgs();
            Registering?.Invoke(this, args);
            return args;
        }
        /// <inheritdoc/>
        public event EventHandler? Registered;
        internal void NotifyRegistered() => Registered?.Invoke(this, EventArgs.Empty);
        /// <inheritdoc/>
        public event HotkeyRegisteringEventHandler? Unregistering;
        private HotkeyRegisteringEventArgs NotifyUnregistering()
        {
            var args = new HotkeyRegisteringEventArgs();
            Unregistering?.Invoke(this, args);
            return args;
        }
        /// <inheritdoc/>
        public event EventHandler? Unregistered;
        internal void NotifyUnregistered() => Unregistered?.Invoke(this, EventArgs.Empty);
        #endregion Events

        #region Methods

        #region Registration
        /// <summary>
        /// Registers the hotkey with the Windows API.
        /// </summary>
        /// <remarks>
        /// This can be overridden in derived classes to change how the hotkey is registered.
        /// This method is only called after the Registering event returns without being set as handled.
        /// </remarks>
        /// <returns><see langword="true"/> when successful; otherwise <see langword="false"/>.</returns>
        protected virtual bool RegisterHotkey() => WindowsHotkeyAPI.TryRegister(this);
        /// <summary>
        /// Unregisters the hotkey with the Windows API.
        /// </summary>
        /// <remarks>
        /// This can be overridden in derived classes to change how the hotkey is unregistered.
        /// </remarks>
        /// <returns><see langword="true"/> when successful; otherwise <see langword="false"/>.</returns>
        protected virtual bool UnregisterHotkey() => WindowsHotkeyAPI.TryUnregister(this);
        /// <summary>
        /// Registers the hotkey with the Win32 API.
        /// </summary>
        /// <returns><see langword="true"/> when successful and not interrupted; otherwise <see langword="false"/>.</returns>
        private bool Register()
        {
            if (IsRegistered) return false;

            var interrupt = NotifyRegistering();
            if (interrupt.Handled)
            {
                if (interrupt.RegistrationSuccessStateWhenHandled)
                {
                    NotifyRegistered();
                    return true;
                }
                else return false;
            }

            if (RegisterHotkey())
            {
                NotifyRegistered();
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Unregisters the hotkey with the Win32 API.
        /// </summary>
        /// <returns><see langword="true"/> when successful and not interrupted; otherwise <see langword="false"/>.</returns>
        private bool Unregister()
        {
            if (!IsRegistered) return false;

            var interrupt = NotifyUnregistering();
            if (interrupt.Handled)
            {
                if (interrupt.RegistrationSuccessStateWhenHandled)
                {
                    NotifyUnregistered();
                    return true;
                }
                else return false;
            }

            if (UnregisterHotkey())
            {
                NotifyUnregistered();
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Re-registers the hotkey with the Win32 API.
        /// </summary>
        /// <returns><see langword="true"/> when successful; otherwise <see langword="false"/>.</returns>
        private bool Reregister()
        {
            if (Unregister()) //< no need to check if we're registered since this does it for us
            {
                _isRegistered = false;
                if (Register())
                {
                    _isRegistered = true;
                    NotifyPropertyChanged(nameof(IsRegistered));
                    return true;
                }
                // else Register() failed & IsRegistered is false; notify of the change
                NotifyPropertyChanged(nameof(IsRegistered));
            }
            return false;
        }
        #endregion Registration

        #region MessageHookImpl
        IntPtr MessageHookImpl(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
            case WindowsHotkeyAPI.WM_HOTKEY:
                if (Convert.ToUInt16(wParam.ToInt32()).Equals(this.ID))
                {
                    if (Action != null)
                    {
                        HotkeyPressedEventArgs eventArgs = new(Action.ActionSettings);
                        Action.Invoke(this, eventArgs);
                        NotifyPressed(this, eventArgs);
                    }
                    handled = true;
                }
                break;
            default:
                break;
            }
            return IntPtr.Zero;
        }
        #endregion MessageHookImpl

        #region GetNewID
        /// <summary>
        /// Gets a new ID from the <see cref="WindowsHotkeyAPI"/>.
        /// </summary>
        /// <remarks>
        /// This method automatically unregisters &amp; reregisters the hotkey when called.
        /// </remarks>
        public void GetNewID()
        {
            var registered = IsRegistered;
            if (registered) Unregister();

            ID = WindowsHotkeyAPI.NextID;

            if (registered) Register();
        }
        #endregion GetNewID

        #endregion Methods

        #region IDisposable Implementation
        /// <summary>
        /// Cleans up the <see cref="Hotkey"/> instance by unregistering it, if necessary.
        /// </summary>
        ~Hotkey() => Dispose();
        /// <inheritdoc/>
        public void Dispose()
        {
            _isRegistered = !Unregister();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}
