using VolumeControl.Core.Input.Actions;
using VolumeControl.Core.Input.Enums;
using VolumeControl.Core.Input.Exceptions;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// A <see cref="Hotkey"/> that also tracks and reports registration errors.
    /// </summary>
    public class HotkeyWithError : Hotkey
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="HotkeyWithError"/> instance with the specified parameters.
        /// </summary>
        /// <inheritdoc/>
        public HotkeyWithError(EFriendlyKey key, EModifierKey modifiers, bool isRegistered) : base(key, modifiers, isRegistered) { }
        /// <summary>
        /// Creates a new <see cref="HotkeyWithError"/> instance with the specified parameters.
        /// </summary>
        /// <inheritdoc/>
        public HotkeyWithError(string name, EFriendlyKey key, EModifierKey modifiers, bool isRegistered) : base(name, key, modifiers, isRegistered) { }
        /// <summary>
        /// Creates a new <see cref="HotkeyWithError"/> instance with the specified parameters.
        /// </summary>
        /// <inheritdoc/>
        public HotkeyWithError(EFriendlyKey key, EModifierKey modifiers, bool isRegistered, HotkeyActionInstance actionInstance) : base(key, modifiers, isRegistered, actionInstance) { }
        /// <summary>
        /// Creates a new <see cref="HotkeyWithError"/> instance with the specified parameters.
        /// </summary>
        /// <inheritdoc/>
        public HotkeyWithError(string name, EFriendlyKey key, EModifierKey modifiers, bool isRegistered, HotkeyActionInstance actionInstance) : base(name, key, modifiers, isRegistered, actionInstance) { }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the last error message that occurred relating to this hotkey instance.
        /// </summary>
        public string? ErrorMessage
        {
            get => _error;
            private set
            {
                _error = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(HasError));
            }
        }
        private string? _error;
        /// <summary>
        /// Gets whether
        /// </summary>
        public bool HasError => ErrorMessage != null;
        #endregion Properties

        #region Methods

        #region Registration
        /// <inheritdoc/>
        protected override bool Register()
        {
            UnsetError();
            try
            {
                WindowsHotkeyAPI.Register(this);
                return true;
            }
            catch (HotkeyRegistrationException ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }
        /// <inheritdoc/>
        protected override bool Unregister()
        {
            UnsetError();
            try
            {
                WindowsHotkeyAPI.Unregister(this);
                return true;
            }
            catch (HotkeyRegistrationException ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }
        /// <inheritdoc/>
        protected override bool Reregister()
        {
            UnsetError();
            try
            {
                WindowsHotkeyAPI.Unregister(this);
                WindowsHotkeyAPI.Register(this);
                return true;
            }
            catch (HotkeyRegistrationException ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }
        #endregion Registration

        #region UnsetError
        private void UnsetError()
            => ErrorMessage = null;
        #endregion UnsetError

        #endregion Methods
    }
}
