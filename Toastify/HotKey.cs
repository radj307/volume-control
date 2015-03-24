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
using System.Xml.Serialization;

namespace Toastify
{
    public class Hotkey : INotifyPropertyChanged
    {
        private bool _enabled;
        /// <summary>
        /// Specifies whether or not the hotkey is enabled or disabled from a user's
        /// perspective. Does not actually enable or disable the hotkey, use Activate()
        /// and Deactivate().
        /// 
        /// Why do we have these two schemes? We need a way to be able to deactivate a
        /// Hotkey (for example when unloading settings) without changing the Enabled
        /// property (which only indicates the user's preference)
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;

                    NotifyPropertyChanged("Enabled");
                }
            }
        }

        private bool _windowsKey;
        public bool WindowsKey
        {
            get { return _windowsKey; }
            set
            {
                if (_windowsKey != value)
                {
                    _windowsKey = value;

                    NotifyPropertyChanged("WindowsKey");

                    CheckIfValid();
                }
            }
        }


        private bool _ctrl;
        public bool Ctrl
        {
            get { return _ctrl; }
            set
            {
                if (_ctrl != value)
                {
                    _ctrl = value;

                    NotifyPropertyChanged("Notify");

                    CheckIfValid();
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
                    _alt = value;

                    NotifyPropertyChanged("Alt");

                    CheckIfValid();
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
                    _shift = value;

                    NotifyPropertyChanged("Shift");

                    CheckIfValid();
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

                    CheckIfValid();
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

        [XmlIgnore]
        public string HumanReadableAction
        {
            get
            {
                switch (Action)
                {
                    case SpotifyAction.CopyTrackInfo:
                        return "Copy Track Name";
                 
                    case SpotifyAction.PasteTrackInfo:
                        return "Paste Track Name";
       
                    case SpotifyAction.Mute:
                        return "Mute";

                    case SpotifyAction.NextTrack:
                        return "Next Track";

                    case SpotifyAction.None:
                        return "None";

                    case SpotifyAction.PlayPause:
                        return "Play / Pause";

                    case SpotifyAction.PreviousTrack:
                        return "Previous Track";

                    case SpotifyAction.SettingsSaved:
                        return "Settings Saved";

                    case SpotifyAction.ShowSpotify:
                        return "Show / Hide Spotify";

                    case SpotifyAction.ShowToast:
                        return "Show Toast";

                    case SpotifyAction.Stop:
                        return "Stop";

                    case SpotifyAction.VolumeDown:
                        return "Volume Down";

                    case SpotifyAction.VolumeUp:
                        return "Volume Up";

                    case SpotifyAction.FastForward:
                        return "Fast Forward";

                    case SpotifyAction.Rewind:
                        return "Rewind";

                    case SpotifyAction.ThumbsUp:
                        return "Thumbs Up";

                    case SpotifyAction.ThumbsDown:
                        return "Thunbs Down";
                }

                return "No Action";
            }
        }

        private bool _isValid;
        [XmlIgnore]
        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;

                    NotifyPropertyChanged("IsValid");
                }
            }

        }

        private string _invalidReason;
        [XmlIgnore]
        public string InvalidReason
        {
            get { return _invalidReason; }
            set
            {
                if (_invalidReason != value)
                {
                    _invalidReason = value;

                    NotifyPropertyChanged("InvalidReason");
                }
            }
        }

        private bool _active = false;
        private ManagedWinapi.Hotkey _globalKey;

        public Hotkey Clone()
        {
            Hotkey clone = MemberwiseClone() as Hotkey;
            
            // regardless of whether or not the original hotkey was active
            // the cloned one should not start in an active state
            clone._active = false;

            return clone;
        }

        /// <summary>
        /// Turn this HotKey off
        /// </summary>
        public void Deactivate()
        {
            SetActive(false);
        }

        /// <summary>
        /// Turn this hotkey on. Does nothing if this Hotkey is not enabled
        /// </summary>
        public void Activate()
        {
            SetActive(true);
        }

        private void SetActive(bool value)
        {
            if (_active != value)
            {
                _active = value;

                InitGlobalKey();
            }
        }
        
        private void InitGlobalKey()
        {
            // If we're not enabled shut everything done asap
            if (!Enabled || !_active)
            {
                if (_globalKey != null)
                {
                    _globalKey.Enabled = false;
                    _globalKey = null; // may as well collect the memory
                }

                // may not be false if !Enabled
                _active = false;

                return;
            }

            if (_globalKey == null)
                _globalKey = new ManagedWinapi.Hotkey();
            
            // make sure that we don't try to reregister the key midway updating 
            // the combination
            if (_globalKey.Enabled)
                _globalKey.Enabled = false;

            _globalKey.WindowsKey = this.WindowsKey;
            _globalKey.Alt        = this.Alt;
            _globalKey.Ctrl       = this.Ctrl;
            _globalKey.Shift      = this.Shift;
            _globalKey.KeyCode    = ConvertInputKeyToFormsKeys(this.Key);
            
            _globalKey.HotkeyPressed += (s, e) => { Toast.ActionHookCallback(this); };

            try
            {
                _globalKey.Enabled = true;
            }
            catch (HotkeyAlreadyInUseException)
            {
                IsValid = false;
                InvalidReason = "Hotkey is already in use by a different program";
            }
        }

        /// <summary>
        /// Validity rules are:
        /// 
        /// 1. Ctrl or Alt must be selected
        /// 2. a key must be specified
        /// </summary>
        private void CheckIfValid()
        {
            if (Key == Key.None)
            {
                IsValid = false;
                InvalidReason = "You must select a valid key for your hotkey combination";

                return;
            }

            IsValid = true;
            InvalidReason = "";
        }

        #region Static Functions

        private static List<Hotkey> _hotkeys = new List<Hotkey>();

        public static void ClearAll()
        {
            // disable will be called by the destructors, but we want to force a disable
            // now so that we don't wait for the GC to clean up the objects
            foreach (Hotkey hotkey in _hotkeys)
            {
                hotkey.Deactivate();
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

        [XmlIgnore]
        public string HumanReadableKey
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (this.WindowsKey) sb.Append("Win+");
                if (this.Ctrl) sb.Append("Ctrl+");
                if (this.Alt) sb.Append("Alt+");
                if (this.Shift) sb.Append("Shift+");
                sb.Append(this.Key.ToString());
                return sb.ToString();
            }
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
