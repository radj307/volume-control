using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;
using Toastify.Core;
using Toastify.Helpers;
using ToastifyAPI.Model.Interfaces;
using MouseAction = ToastifyAPI.Core.MouseAction;

namespace Toastify.Model
{
    /// <summary>
    ///     Proxy class for the various hotkey types. This class is used in the View layer as a view model.
    /// </summary>
    public class GenericHotkeyProxy : INotifyPropertyChanged
    {
        private readonly KeyboardHotkey keyboardHotkey;
        private readonly MouseHookHotkey mouseHookHotkey;

        private HotkeyType _hotkeyType;

        #region Non-Public Properties

        private List<Hotkey> ProxiedHotkeys
        {
            get { return new List<Hotkey> { this.keyboardHotkey, this.mouseHookHotkey }; }
        }

        #endregion

        #region Public Properties

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
            get { return this.Hotkey?.IsValid() ?? false; }
        }

        public string InvalidReason
        {
            get { return this.Hotkey?.InvalidReason; }
        }

        public bool Alt
        {
            get { return this.Hotkey.Modifiers.HasFlag(ModifierKeys.Alt); }
            set
            {
                if (value)
                    this.ProxiedHotkeys.ForEach(h => h.Modifiers |= ModifierKeys.Alt);
                else
                    this.ProxiedHotkeys.ForEach(h => h.Modifiers &= ~ModifierKeys.Alt);
            }
        }

        public bool Ctrl
        {
            get { return this.Hotkey.Modifiers.HasFlag(ModifierKeys.Control); }
            set
            {
                if (value)
                    this.ProxiedHotkeys.ForEach(h => h.Modifiers |= ModifierKeys.Control);
                else
                    this.ProxiedHotkeys.ForEach(h => h.Modifiers &= ~ModifierKeys.Control);
            }
        }

        public bool Shift
        {
            get { return this.Hotkey.Modifiers.HasFlag(ModifierKeys.Shift); }
            set
            {
                if (value)
                    this.ProxiedHotkeys.ForEach(h => h.Modifiers |= ModifierKeys.Shift);
                else
                    this.ProxiedHotkeys.ForEach(h => h.Modifiers &= ~ModifierKeys.Shift);
            }
        }

        public bool Win
        {
            get { return this.Hotkey.Modifiers.HasFlag(ModifierKeys.Windows); }
            set
            {
                if (value)
                    this.ProxiedHotkeys.ForEach(h => h.Modifiers |= ModifierKeys.Windows);
                else
                    this.ProxiedHotkeys.ForEach(h => h.Modifiers &= ~ModifierKeys.Windows);
            }
        }

        #endregion

        public GenericHotkeyProxy() : this((Hotkey)null)
        {
        }

        public GenericHotkeyProxy(IHotkey hotkey)
        {
            if (hotkey is KeyboardHotkey kbdHotkey)
            {
                this.Type = HotkeyType.Keyboard;
                this.keyboardHotkey = new KeyboardHotkey(kbdHotkey);
                this.mouseHookHotkey = new MouseHookHotkey(hotkey);
            }
            else if (hotkey is MouseHookHotkey mhHotkey)
            {
                this.Type = HotkeyType.MouseHook;
                this.mouseHookHotkey = new MouseHookHotkey(mhHotkey);
                this.keyboardHotkey = new KeyboardHotkey(hotkey);
            }
            else
            {
                this.Type = HotkeyType.Undefined;
                this.keyboardHotkey = hotkey != null ? new KeyboardHotkey(hotkey) : new KeyboardHotkey();
                this.mouseHookHotkey = hotkey != null ? new MouseHookHotkey(hotkey) : new MouseHookHotkey();
            }

            App.Container.BuildUp(this.keyboardHotkey, this.mouseHookHotkey);

            this.keyboardHotkey.PropertyChanged += this.KeyboardHotkey_PropertyChanged;
            this.mouseHookHotkey.PropertyChanged += this.MouseHookHotkey_PropertyChanged;
        }

        public GenericHotkeyProxy(KeyboardHotkey keyboardHotkey) : this((IHotkey)keyboardHotkey)
        {
        }

        public GenericHotkeyProxy(MouseHookHotkey mouseHookHotkey) : this((IHotkey)mouseHookHotkey)
        {
        }

        /// <summary>
        ///     Set the hotkey's activator, i.e. the key or button that activates the hotkey.
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

        public bool IsAlreadyInUseBy(GenericHotkeyProxy hotkeyProxy)
        {
            if (hotkeyProxy == null)
                return false;

            return this.Hotkey.Modifiers == hotkeyProxy.Hotkey.Modifiers &&
                   this.Type == hotkeyProxy.Type &&
                   (this.Type == HotkeyType.Keyboard && this.keyboardHotkey.Key == hotkeyProxy.keyboardHotkey.Key ||
                    this.Type == HotkeyType.MouseHook && this.mouseHookHotkey.MouseButton == hotkeyProxy.mouseHookHotkey.MouseButton);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void KeyboardHotkey_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
        }

        private void MouseHookHotkey_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
        }

        #endregion INotifyPropertyChanged
    }
}