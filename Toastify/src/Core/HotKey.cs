using ManagedWinapi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Serialization;
using Toastify.UI;

namespace Toastify.Core
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
            get
            {
                return this._enabled;
            }
            set
            {
                if (this._enabled != value)
                {
                    this._enabled = value;
                    this.NotifyPropertyChanged("Enabled");
                }
            }
        }

        private bool _windowsKey;

        public bool WindowsKey
        {
            get
            {
                return this._windowsKey;
            }
            set
            {
                if (this._windowsKey != value)
                {
                    this._windowsKey = value;

                    this.NotifyPropertyChanged("WindowsKey");

                    this.CheckIfValid();
                }
            }
        }


        private bool _ctrl;

        public bool Ctrl
        {
            get
            {
                return this._ctrl;
            }
            set
            {
                if (this._ctrl != value)
                {
                    this._ctrl = value;

                    this.NotifyPropertyChanged("Notify");

                    this.CheckIfValid();
                }
            }
        }

        private bool _alt;

        public bool Alt
        {
            get
            {
                return this._alt;
            }
            set
            {
                if (this._alt != value)
                {
                    this._alt = value;
                    this.NotifyPropertyChanged("Alt");
                    this.CheckIfValid();
                }
            }
        }

        private bool _shift;

        public bool Shift
        {
            get
            {
                return this._shift;
            }
            set
            {
                if (this._shift != value)
                {
                    this._shift = value;
                    this.NotifyPropertyChanged("Shift");
                    this.CheckIfValid();
                }
            }
        }


        private Key _key;

        public Key Key
        {
            get
            {
                return this._key;
            }
            set
            {
                if (this._key != value)
                {
                    this._key = value;
                    this.NotifyPropertyChanged("Key");
                    this.CheckIfValid();
                }
            }
        }

        private SpotifyAction _action;

        public SpotifyAction Action
        {
            get
            {
                return this._action;
            }
            set
            {
                if (this._action != value)
                {
                    this._action = value;
                    this.NotifyPropertyChanged("Action");
                }
            }
        }

        [XmlIgnore]
        public string HumanReadableAction
        {
            get
            {
                switch (this.Action)
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
            get
            {
                return this._isValid;
            }
            set
            {
                if (this._isValid != value)
                {
                    this._isValid = value;
                    this.NotifyPropertyChanged("IsValid");
                }
            }
        }

        private string _invalidReason;

        [XmlIgnore]
        public string InvalidReason
        {
            get
            {
                return this._invalidReason;
            }
            set
            {
                if (this._invalidReason != value)
                {
                    this._invalidReason = value;
                    this.NotifyPropertyChanged("InvalidReason");
                }
            }
        }

        private bool _active;
        private ManagedWinapi.Hotkey _globalKey;

        public Hotkey Clone()
        {
            Hotkey clone = this.MemberwiseClone() as Hotkey;

            // regardless of whether or not the original hotkey was active
            // the cloned one should not start in an active state
            if (clone != null)
                clone._active = false;

            return clone;
        }

        /// <summary>
        /// Turn this HotKey off
        /// </summary>
        public void Deactivate()
        {
            this.SetActive(false);
        }

        /// <summary>
        /// Turn this hotkey on. Does nothing if this Hotkey is not enabled
        /// </summary>
        public void Activate()
        {
            this.SetActive(true);
        }

        private void SetActive(bool value)
        {
            if (this._active != value)
            {
                this._active = value;

                this.InitGlobalKey();
            }
        }

        private void InitGlobalKey()
        {
            // If we're not enabled shut everything done asap
            if (!this.Enabled || !this._active)
            {
                if (this._globalKey != null)
                {
                    this._globalKey.Enabled = false;
                    this._globalKey = null; // may as well collect the memory
                }

                // may not be false if !Enabled
                this._active = false;

                return;
            }

            if (this._globalKey == null)
                this._globalKey = new ManagedWinapi.Hotkey();

            // make sure that we don't try to reregister the key midway updating
            // the combination
            if (this._globalKey.Enabled)
                this._globalKey.Enabled = false;

            this._globalKey.WindowsKey = this.WindowsKey;
            this._globalKey.Alt = this.Alt;
            this._globalKey.Ctrl = this.Ctrl;
            this._globalKey.Shift = this.Shift;
            this._globalKey.KeyCode = ConvertInputKeyToFormsKeys(this.Key);

            this._globalKey.HotkeyPressed += (s, e) => { Toast.ActionHookCallback(this); };

            try
            {
                this._globalKey.Enabled = true;
            }
            catch (HotkeyAlreadyInUseException)
            {
                this.IsValid = false;
                this.InvalidReason = "Hotkey is already in use by a different program";
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
            if (this.Key == Key.None)
            {
                this.IsValid = false;
                this.InvalidReason = "You must select a valid key for your hotkey combination";

                return;
            }

            this.IsValid = true;
            this.InvalidReason = "";
        }

        #region Static Functions

        private static readonly List<Hotkey> hotkeys = new List<Hotkey>();

        public static void ClearAll()
        {
            // disable will be called by the destructors, but we want to force a disable
            // now so that we don't wait for the GC to clean up the objects
            foreach (Hotkey hotkey in hotkeys)
            {
                hotkey.Deactivate();
            }

            hotkeys.Clear();
        }

        private static Keys ConvertInputKeyToFormsKeys(Key key)
        {
            if (Enum.GetNames(typeof(Keys)).Contains(key.ToString()))
                return (Keys)Enum.Parse(typeof(Keys), key.ToString());

            return Keys.None;
        }

        #endregion Static Functions

        private readonly ManagedWinapi.Hotkey key = new ManagedWinapi.Hotkey();

        public Hotkey()
        {
            hotkeys.Add(this);
        }

        ~Hotkey()
        {
            if (this.key != null)
                this.key.Enabled = false;
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
                sb.Append(this.Key);
                return sb.ToString();
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion INotifyPropertyChanged
    }
}