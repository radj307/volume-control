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
        public BindableHotkey(Hotkey hk) => this.Hotkey = hk;
        /// <summary>
        /// Creates a new <see cref="BindableHotkey"/> instance.
        /// </summary>
        public BindableHotkey() => this.Hotkey = new();
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="Hotkey"/> instance associated with this <see cref="BindableHotkey"/>.
        /// </summary>
        public Hotkey Hotkey { get; set; }

        /// <inheritdoc/>
        public int ID => this.Hotkey.ID;
        /// <inheritdoc/>
        public Key Key
        {
            get => this.Hotkey.Key;
            set => this.Hotkey.Key = value;
        }
        /// <inheritdoc/>
        public Modifier Modifier
        {
            get => this.Hotkey.Modifier;
            set => this.Hotkey.Modifier = value;
        }
        /// <inheritdoc/>
        public bool Registered
        {
            get => this.Hotkey.Registered;
            set => this.Hotkey.Registered = value;
        }

        /// <inheritdoc/>
        public bool Alt
        {
            get => this.Modifier.HasFlag(Modifier.Alt);
            set => this.Modifier = this.Modifier.Set(Modifier.Alt, value);
        }
        /// <inheritdoc/>
        public bool Ctrl
        {
            get => this.Modifier.HasFlag(Modifier.Ctrl);
            set => this.Modifier = this.Modifier.Set(Modifier.Ctrl, value);
        }
        /// <inheritdoc/>
        public bool Shift
        {
            get => this.Modifier.HasFlag(Modifier.Shift);
            set => this.Modifier = this.Modifier.Set(Modifier.Shift, value);
        }
        /// <inheritdoc/>
        public bool Win
        {
            get => this.Modifier.HasFlag(Modifier.Super);
            set => this.Modifier = this.Modifier.Set(Modifier.Super, value);
        }
        /// <summary>
        /// Gets whether <see cref="Key"/> is set to <see cref="Key.None"/> or not.
        /// </summary>
        public bool Valid => !this.Key.Equals(Key.None);
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
            add => this.Hotkey.Pressed += value;
            remove => this.Hotkey.Pressed -= value;
        }

#       pragma warning disable CS0067 // The event 'BindableHotkey.PropertyChanged' is never used ; This is automatically used by Fody.
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
#       pragma warning restore CS0067 // The event 'BindableHotkey.PropertyChanged' is never used ; This is automatically used by Fody.

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Hotkey.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion Events
    }
}
