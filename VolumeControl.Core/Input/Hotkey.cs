using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using VolumeControl.Log;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// A Hotkey that may be registered with the windows API.
    /// </summary>
    public class Hotkey : IHotkey, INotifyPropertyChanged, IDisposable
    {
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
        private bool _registered;

        /// <summary>
        /// Gets a boolean that indicates whether the last attempt to register this hotkey failed or not.
        /// </summary>
        public bool HasError { get; internal set; }
        /// <summary>
        /// When <see cref="HasError"/> is <see langword="true"/>, this retrieves the error message string associated with the error that occurred; when <see cref="HasError"/> is <see langword="false"/>, this is <see langword="null"/>.
        /// </summary>
        public string? ErrorMessage { get; internal set; }

        internal void SetError(string message)
        {
            ErrorMessage = message;
            NotifyPropertyChanged(nameof(ErrorMessage));

            if (!HasError)
            {
                HasError = true;
                NotifyPropertyChanged(nameof(HasError));
            }
        }
        internal void UnsetError()
        {
            ErrorMessage = null;
            NotifyPropertyChanged(nameof(ErrorMessage));

            if (HasError)
            {
                HasError = false;
                NotifyPropertyChanged(nameof(HasError));
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
        public event HandledEventHandler? Pressed;

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
                        Pressed?.Invoke(this, new());
                    }
                    catch (Exception ex)
                    {
                        FLog.Log.Error($"The action handler for Hotkey {this.ID} ({this.Modifier:G}+{this.Key:G}) threw an exception!", ex);
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
        /// Finalizer
        /// </summary>
        ~Hotkey() => this.Dispose();
        /// <inheritdoc/>
        public void Dispose()
        {
            _ = WindowsHotkeyAPI.Unregister(this);
            GC.SuppressFinalize(this);
        }
    }
}
