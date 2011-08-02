using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Input;
using ManagedWinapi;

namespace Toastify
{
    public class Hotkey : INotifyPropertyChanged
    {
        private bool _ctrl;
        public bool Ctrl
        {
            get { return _ctrl; }
            set
            {
                if (_ctrl != value)
                {
                    _ctrl    = value;

                    NotifyPropertyChanged("Notify");
                }
            }
        }

        private bool _alt;
        public bool Alt
        {
            get { return _alt; }
            set
            {
                if (_alt != value)
                {
                    _alt    = value;

                    NotifyPropertyChanged("Alt");

                }
            }
        }

        private bool _shift;
        public bool Shift
        {
            get { return _shift; }
            set
            {
                if (_shift != value)
                {
                    _shift    = value;

                    NotifyPropertyChanged("Shift");
                }
            }
        }


        private Key _key;
        public Key Key
        {
            get { return _key; }
            set
            {
                if (_key != value)
                {
                    _key = value;

                    NotifyPropertyChanged("Key");
                }
            }
        }

        private SpotifyAction _action;
        public SpotifyAction Action
        {
            get { return _action; }
            set
            {
                if (_action != value)
                {
                    _action = value;

                    NotifyPropertyChanged("Action");
                }
            }
        }

        private bool _enabled = false;
        private ManagedWinapi.Hotkey _globalKey;

        public Hotkey Clone()
        {
            Hotkey clone = MemberwiseClone() as Hotkey;
            
            // regardless of whether or not the original hotkey was enabled
            // the cloned one should not be enabled
            clone._enabled = false;

            return clone;
        }

        public void Disable()
        {
            SetEnabled(false);
        }

        public void Enable()
        {
            SetEnabled(true);
        }

        private void SetEnabled(bool value)
        {
            if (_enabled != value)
            {
                _enabled = value;

                InitGlobalKey();
            }
        }
        
        private void InitGlobalKey()
        {
            if (_globalKey == null)
                _globalKey = new ManagedWinapi.Hotkey();

            _globalKey.Alt     = this.Alt;
            _globalKey.Ctrl    = this.Ctrl;
            _globalKey.Shift   = this.Shift;
            _globalKey.KeyCode = ConvertInputKeyToFormsKeys(this.Key);
            
            _globalKey.HotkeyPressed += (s, e) => { Toast.ActionHookCallback(this); };

            _globalKey.Enabled = _enabled;
        }

        #region Static Functions

        private static List<Hotkey> _hotkeys = new List<Hotkey>();

        public static void ClearAll()
        {

            if (!SettingsXml.Current.GlobalHotKeys)
            {
                // disable will be called by the destructors, but we want to force a disable
                // now so that we don't wait for the GC to clean up the objectss
                foreach (Hotkey hotkey in _hotkeys)
                {
                    hotkey.Disable();
                }
            }

            _hotkeys.Clear();
        }
        
        private static System.Windows.Forms.Keys ConvertInputKeyToFormsKeys(System.Windows.Input.Key key)
        {
            if (Enum.GetNames(typeof(System.Windows.Forms.Keys)).Contains(key.ToString()))
                return (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), key.ToString());
            else
                return Keys.None;
        }

        #endregion

        private ManagedWinapi.Hotkey key = new ManagedWinapi.Hotkey();

        public Hotkey()
        {
            _hotkeys.Add(this);
        }

        ~Hotkey()
        {
            if (key != null)
                key.Enabled = false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (this.Ctrl) sb.Append("Ctrl+");
            if (this.Alt) sb.Append("Alt+");
            if (this.Shift) sb.Append("Shift+");
            sb.Append(this.Key.ToString());
            return sb.ToString();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
