using VolumeControl.Core.Input.Actions;
using VolumeControl.Core.Input.Enums;

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
        public HotkeyWithError(EFriendlyKey key, EModifierKey modifiers, bool isRegistered) : base(key, modifiers, isRegistered)
        {
            base.Registering += new((s, e) => UnsetError());
            base.Unregistered += new((s, e) => UnsetError());
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyWithError"/> instance with the specified parameters.
        /// </summary>
        /// <inheritdoc/>
        public HotkeyWithError(string name, EFriendlyKey key, EModifierKey modifiers, bool isRegistered) : base(name, key, modifiers, isRegistered)
        {
            base.Registering += new((s, e) => UnsetError());
            base.Unregistered += new((s, e) => UnsetError());
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyWithError"/> instance with the specified parameters.
        /// </summary>
        /// <inheritdoc/>
        public HotkeyWithError(EFriendlyKey key, EModifierKey modifiers, bool isRegistered, HotkeyActionInstance actionInstance) : base(key, modifiers, isRegistered, actionInstance)
        {
            base.Registering += new((s, e) => UnsetError());
            base.Unregistered += new((s, e) => UnsetError());
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyWithError"/> instance with the specified parameters.
        /// </summary>
        /// <inheritdoc/>
        public HotkeyWithError(string name, EFriendlyKey key, EModifierKey modifiers, bool isRegistered, HotkeyActionInstance actionInstance) : base(name, key, modifiers, isRegistered, actionInstance)
        {
            base.Registering += new((s, e) => UnsetError());
            base.Unregistered += new((s, e) => UnsetError());
        }
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

        #region UnsetError
        private void UnsetError()
            => ErrorMessage = null;
        #endregion UnsetError

        #endregion Methods
    }
}
