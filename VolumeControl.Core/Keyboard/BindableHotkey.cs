using System.ComponentModel;
using System.Windows.Input;
using VolumeControl.Core.Keyboard.Actions;

namespace VolumeControl.Core.Keyboard
{
    /// <summary>
    /// A hotkey view model instance.
    /// </summary>
    public class BindableHotkey : IBindableHotkey, INotifyPropertyChanged
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="BindableHotkey"/> instance using <paramref name="hk"/>.
        /// </summary>
        /// <param name="hk">A <see cref="Hotkey"/> instance to use.</param>
        public BindableHotkey(Hotkey hk) => Hotkey = hk;
        /// <summary>
        /// Creates a new <see cref="BindableHotkey"/> instance.
        /// </summary>
        public BindableHotkey() => Hotkey = new();
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="Hotkey"/> instance associated with this <see cref="BindableHotkey"/>.
        /// </summary>
        public Hotkey Hotkey { get; set; }

        /// <inheritdoc/>
        public int ID => Hotkey.ID;
        /// <inheritdoc/>
        public Key Key
        {
            get => Hotkey.Key;
            set => Hotkey.Key = value;
        }
        /// <inheritdoc/>
        public Modifier Modifier
        {
            get => Hotkey.Modifier;
            set => Hotkey.Modifier = value;
        }
        /// <inheritdoc/>
        public bool Registered
        {
            get => Hotkey.Registered;
            set => Hotkey.Registered = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Modifier.Alt"/> bit in the <see cref="Modifier"/> property.
        /// </summary>
        public bool Alt
        {
            get => Modifier.HasFlag(Modifier.Alt);
            set => Modifier = Modifier.Set(Modifier.Alt, value);
        }
        /// <summary>
        /// Gets or sets the <see cref="Modifier.Ctrl"/> bit in the <see cref="Modifier"/> property.
        /// </summary>
        public bool Ctrl
        {
            get => Modifier.HasFlag(Modifier.Ctrl);
            set => Modifier = Modifier.Set(Modifier.Ctrl, value);
        }
        /// <summary>
        /// Gets or sets the <see cref="Modifier.Shift"/> bit in the <see cref="Modifier"/> property.
        /// </summary>
        public bool Shift
        {
            get => Modifier.HasFlag(Modifier.Shift);
            set => Modifier = Modifier.Set(Modifier.Shift, value);
        }
        /// <summary>
        /// Gets or sets the <see cref="Modifier.Super"/> bit in the <see cref="Modifier"/> property.
        /// </summary>
        public bool Win
        {
            get => Modifier.HasFlag(Modifier.Super);
            set => Modifier = Modifier.Set(Modifier.Super, value);
        }
        /// <summary>
        /// Gets whether <see cref="Key"/> is set to <see cref="Key.None"/> or not.
        /// </summary>
        public bool Valid => !Key.Equals(Key.None);
        /// <summary>
        /// Gets or sets the <see cref="IActionBinding"/> associated with this hotkey instance.
        /// </summary>
        public IActionBinding? Action
        {
            get => _actionBinding;
            set
            {
                if (_actionBinding is not null)
                    Pressed -= _actionBinding.HandleKeyEvent;
                _actionBinding = value;
                if (_actionBinding is not null)
                    Pressed += _actionBinding.HandleKeyEvent;
            }
        }
        /// <inheritdoc/>
        private IActionBinding? _actionBinding;
        /// <inheritdoc/>
        public string Name { get; set; } = string.Empty;
        #endregion Properties

        #region Events
        /// <inheritdoc/>
        public event HandledEventHandler? Pressed
        {
            add => Hotkey.Pressed += value;
            remove => Hotkey.Pressed -= value;
        }
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        public void Dispose()
        {
            Hotkey.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion Events
    }
}
