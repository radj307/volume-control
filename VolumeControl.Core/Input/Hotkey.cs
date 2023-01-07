using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Log;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// A Hotkey that may be registered with the windows API.
    /// </summary>
    public class Hotkey : IHotkey, INotifyPropertyChanged, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Hotkey"/> instance.
        /// </summary>
        public Hotkey() => this.ID = WindowsHotkeyAPI.NextID;
        /// <summary>
        /// Creates a new <see cref="Hotkey"/> instance with the specified parameters.
        /// </summary>
        /// <param name="key">The primary keyboard <see cref="Key"/> associated with this hotkey.</param>
        /// <param name="modifiers">Modifier key(s) required by this hotkey.</param>
        /// <param name="registered">Whether this hotkey should be registered during construction.</param>
        public Hotkey(Key key, Modifier modifiers, bool registered = false)
        {
            this.ID = WindowsHotkeyAPI.NextID;
            this.Key = key;
            this.Modifier = modifiers;
            this.Registered = registered;
        }
        #endregion Constructors

        #region Properties
        /// <inheritdoc/>
        public int ID { get; }
        /// <inheritdoc/>
        public Key Key
        {
            get => _key;
            set
            {
                _key = value;
                _ = WindowsHotkeyAPI.Reregister(this);
            }
        }
        private Key _key;
        /// <inheritdoc/>
        public Modifier Modifier
        {
            get => _modifier;
            set
            {
                _modifier = value;
                _ = WindowsHotkeyAPI.Reregister(this);
            }
        }
        private Modifier _modifier;
        /// <inheritdoc/>
        public bool Registered
        {
            get => _registered;
            set => _ = (_registered = value) ? WindowsHotkeyAPI.Register(this) : WindowsHotkeyAPI.Unregister(this);
        }
        private bool _registered = false;
        /// <summary>
        /// Gets a boolean that indicates whether the last attempt to register this hotkey failed or not.
        /// </summary>
        public bool HasError { get; internal set; } = false;
        /// <summary>
        /// When <see cref="HasError"/> is <see langword="true"/>, this retrieves the error message string associated with the error that occurred; when <see cref="HasError"/> is <see langword="false"/>, this is <see langword="null"/>.
        /// </summary>
        public string? ErrorMessage { get; internal set; } = null;
        /// <inheritdoc/>
        public ObservableImmutableList<HotkeyActionSetting>? ActionSettings { get; set; }
        #endregion Properties

        #region Methods
        internal void SetError(string message)
        {
            this.ErrorMessage = message;
            this.NotifyPropertyChanged(nameof(this.ErrorMessage));

            if (!this.HasError)
            {
                this.HasError = true;
                this.NotifyPropertyChanged(nameof(this.HasError));
            }
        }
        internal void UnsetError()
        {
            this.ErrorMessage = null;
            this.NotifyPropertyChanged(nameof(this.ErrorMessage));

            if (this.HasError)
            {
                this.HasError = false;
                this.NotifyPropertyChanged(nameof(this.HasError));
            }
        }
#       pragma warning disable CS0067
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
#       pragma warning restore CS0067
        /// <summary>
        /// Triggered when the hotkey is pressed.
        /// </summary>
        public event HotkeyActionPressedEventHandler? Pressed;
#       pragma warning disable IDE0060 // Remove unused parameter
        internal IntPtr MessageHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
            case WindowsHotkeyAPI.WM_HOTKEY:
                if (wParam.ToInt32().Equals(this.ID))
                {
                    handled = true;
                    try
                    {
                        Pressed?.Invoke(this, new(ActionSettings));
                    }
                    catch (Exception ex)
                    {
                        FLog.Log.Error($"The action handler for Hotkey {this.ID} ({this.GetStringRepresentation()}) threw an exception!", ex);
                    }
                }
                break;
            default:
                break;
            }
            return IntPtr.Zero;
        }
#       pragma warning restore IDE0060 // Remove unused parameter
        /// <summary>
        /// Gets the string representation of this hotkey's key combination.
        /// </summary>
        /// <returns>A string representing the key combination of this hotkey.</returns>
        public string GetStringRepresentation()
        {
            var modStr = this.Modifier.GetStringRepresentation();
            return $"{modStr}{(modStr.Length.Equals(0) ? "" : "-")}{this.Key:G}";
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~Hotkey() => this.Dispose();
        /// <inheritdoc/>
        public void Dispose()
        {
            _ = WindowsHotkeyAPI.Unregister(this);
            GC.SuppressFinalize(this);
        }
        #endregion Methods
    }
}
