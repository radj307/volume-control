using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Toastify.Core;
using MouseAction = ToastifyAPI.Core.MouseAction;

namespace Toastify.Model
{
    /// <summary>
    /// Hotkey proxy used in the View layer as a view model.
    /// </summary>
    public class GenericHotkeyProxy : INotifyPropertyChanged
    {
        private readonly KeyboardHotkey keyboardHotkey;
        private readonly MouseHookHotkey mouseHookHotkey;

        private HotkeyType _hotkeyType;

        #region Properties

        public HotkeyType Type
        {
            get { return this._hotkeyType; }
            set
            {
                if (this._hotkeyType != value)
                {
                    this._hotkeyType = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Hotkey Hotkey
        {
            get
            {
                switch (this.Type)
                {
                    case HotkeyType.Keyboard:
                        return this.keyboardHotkey;

                    case HotkeyType.MouseHook:
                        return this.mouseHookHotkey;

                    default:
                        return null;
                }
            }
        }

        public bool IsValid
        {
            get { return this.Hotkey.IsValid(); }
        }

        #region Modifiers

        public bool Alt
        {
            get { return this.Hotkey.Modifiers.HasFlag(ModifierKeys.Alt); }
            set
            {
                if (value)
                    this.Hotkey.Modifiers |= ModifierKeys.Alt;
                else
                    this.Hotkey.Modifiers &= ~ModifierKeys.Alt;
            }
        }

        public bool Ctrl
        {
            get { return this.Hotkey.Modifiers.HasFlag(ModifierKeys.Control); }
            set
            {
                if (value)
                    this.Hotkey.Modifiers |= ModifierKeys.Control;
                else
                    this.Hotkey.Modifiers &= ~ModifierKeys.Control;
            }
        }

        public bool Shift
        {
            get { return this.Hotkey.Modifiers.HasFlag(ModifierKeys.Shift); }
            set
            {
                if (value)
                    this.Hotkey.Modifiers |= ModifierKeys.Shift;
                else
                    this.Hotkey.Modifiers &= ~ModifierKeys.Shift;
            }
        }

        public bool Win
        {
            get { return this.Hotkey.Modifiers.HasFlag(ModifierKeys.Windows); }
            set
            {
                if (value)
                    this.Hotkey.Modifiers |= ModifierKeys.Windows;
                else
                    this.Hotkey.Modifiers &= ~ModifierKeys.Windows;
            }
        }

        #endregion Modifiers

        #endregion Properties

        public GenericHotkeyProxy()
        {
            this.keyboardHotkey = App.Container.GetInstance<KeyboardHotkey>();
            this.mouseHookHotkey = App.Container.GetInstance<MouseHookHotkey>();
        }

        public GenericHotkeyProxy(Hotkey hotkey)
        {
            if (hotkey is KeyboardHotkey kbdHotkey)
            {
                this.Type = HotkeyType.Keyboard;
                this.keyboardHotkey = kbdHotkey;
                this.mouseHookHotkey = App.Container.GetInstance<MouseHookHotkey>();
            }
            else if (hotkey is MouseHookHotkey mhHotkey)
            {
                this.Type = HotkeyType.MouseHook;
                this.keyboardHotkey = App.Container.GetInstance<KeyboardHotkey>();
                this.mouseHookHotkey = mhHotkey;
            }
            else
            {
                this.keyboardHotkey = App.Container.GetInstance<KeyboardHotkey>();
                this.mouseHookHotkey = App.Container.GetInstance<MouseHookHotkey>();
            }
        }

        public GenericHotkeyProxy(KeyboardHotkey keyboardHotkey)
        {
            this.Type = HotkeyType.Keyboard;

            this.keyboardHotkey = keyboardHotkey;
            this.mouseHookHotkey = App.Container.GetInstance<MouseHookHotkey>();
        }

        public GenericHotkeyProxy(MouseHookHotkey mouseHookHotkey)
        {
            this.Type = HotkeyType.MouseHook;

            this.keyboardHotkey = App.Container.GetInstance<KeyboardHotkey>();
            this.mouseHookHotkey = mouseHookHotkey;
        }

        /// <summary>
        /// Set the hotkey's activator, i.e. the key or button that activates the hotkey.
        /// </summary>
        /// <param name="activator"></param>
        public void SetActivator(object activator)
        {
            switch (this.Type)
            {
                case HotkeyType.Keyboard:
                    if (this.keyboardHotkey != null && activator is Key key)
                        this.keyboardHotkey.Key = key;
                    break;

                case HotkeyType.MouseHook:
                    if (this.mouseHookHotkey != null && activator is MouseAction mouseAction)
                        this.mouseHookHotkey.MouseButton = mouseAction;
                    break;

                default:
                    // ignore
                    break;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}