using System.ComponentModel;
using System.Windows.Input;

namespace VolumeControl.Core.Keyboard
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

#       pragma warning disable CS0067
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
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
                    Pressed?.Invoke(this, new());
                    handled = true;
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
