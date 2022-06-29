using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Input;
using VolumeControl.TypeExtensions;

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
        public Hotkey() => ID = WindowsHotkeyAPI.NextID;
        /// <summary>
        /// Creates a new <see cref="Hotkey"/> instance with the specified parameters.
        /// </summary>
        /// <param name="key">The primary keyboard <see cref="Key"/> associated with this hotkey.</param>
        /// <param name="modifiers">Modifier key(s) required by this hotkey.</param>
        /// <param name="registered">Whether this hotkey should be registered during construction.</param>
        public Hotkey(Key key, Modifier modifiers, bool registered = false)
        {
            ID = WindowsHotkeyAPI.NextID;
            Key = key;
            Modifier = modifiers;
            Registered = registered;

            PropertyChanged += HandlePropertyChanged;
        }

        /// <inheritdoc/>
        public int ID { get; }
        /// <inheritdoc/>
        public Key Key { get; set; }
        /// <inheritdoc/>
        public Modifier Modifier { get; set; }
        /// <inheritdoc/>
        public bool Registered
        {
            get => _registered;
            set
            {
                if (_registered = value)
                { // true // register:
                    WindowsHotkeyAPI.Register(this);
                }
                else
                { // false // unregister:
                    WindowsHotkeyAPI.Unregister(this);
                }
            }
        }
        private bool _registered;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
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
                if (wParam.ToInt32().Equals(ID))
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

        private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is string name && name.EqualsAny(nameof(Key), nameof(Modifier)))
                WindowsHotkeyAPI.Reregister(this);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~Hotkey() => Dispose();
        /// <inheritdoc/>
        public void Dispose()
        {
            WindowsHotkeyAPI.Unregister(this);
            GC.SuppressFinalize(this);
        }
    }
}
