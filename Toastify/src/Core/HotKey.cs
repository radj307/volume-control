using ManagedWinapi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Toastify.UI;

namespace Toastify.Core
{
    [Serializable]
    [XmlRoot("Hotkey")]
    public class Hotkey : INotifyPropertyChanged, IXmlSerializable
    {
        private static readonly List<Hotkey> hotkeys = new List<Hotkey>();

        #region Private fields

        private SpotifyAction _action;
        private bool _enabled;
        private bool _shift;
        private bool _ctrl;
        private bool _alt;
        private bool _windowsKey;
        private Key _key;

        private bool _isValid;
        private string _invalidReason;

        private bool _active;
        private ManagedWinapi.Hotkey _globalKey;
        private readonly ManagedWinapi.Hotkey key = new ManagedWinapi.Hotkey();

        #endregion Private fields

        #region Public properties

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
                    this.NotifyPropertyChanged("Action", false);
                }
            }
        }

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
                    this.NotifyPropertyChanged("Enabled", false);
                }
            }
        }

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
                }
            }
        }

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
                    this.NotifyPropertyChanged("Ctrl");
                }
            }
        }

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
                }
            }
        }

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
                }
            }
        }

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
                }
            }
        }

        [XmlIgnore]
        public string HumanReadableKey
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (this.Shift) sb.Append("Shift+");
                if (this.Ctrl) sb.Append("Ctrl+");
                if (this.Alt) sb.Append("Alt+");
                if (this.WindowsKey) sb.Append("Win+");
                sb.Append(this.Key);
                return sb.ToString();
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
                    this.NotifyPropertyChanged("IsValid", false);
                }
            }
        }

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
                    this.NotifyPropertyChanged("InvalidReason", false);
                }
            }
        }

        [XmlIgnore]
        public bool Active
        {
            get
            {
                return this._active;
            }
            private set
            {
                if (this._active != value)
                {
                    this._active = value;
                    this.InitGlobalKey();
                }
            }
        }

        #endregion Public properties

        public event PropertyChangedEventHandler PropertyChanged;

        public Hotkey()
        {
            hotkeys.Add(this);
        }

        ~Hotkey()
        {
            if (this.key != null)
                this.key.Enabled = false;
        }

        public Hotkey Clone()
        {
            Hotkey clone = this.MemberwiseClone() as Hotkey;

            // Regardless of whether or not the original hotkey was active,
            // the cloned one should not start in an active state.
            if (clone != null)
                clone._active = false;

            return clone;
        }

        /// <summary>
        /// Turn this HotKey off.
        /// </summary>
        public void Deactivate()
        {
            this.Active = false;
        }

        /// <summary>
        /// Turn this hotkey on. Does nothing if this Hotkey is not enabled
        /// </summary>
        public void Activate()
        {
            this.Active = true;
        }

        private void InitGlobalKey()
        {
            // If we're not enabled shut everything down asap
            if (!this.Enabled || !this.Active)
            {
                if (this._globalKey != null)
                {
                    this._globalKey.Enabled = false;
                    this._globalKey = null; // may as well collect the memory
                }

                // May not be false if !Enabled
                this._active = false;

                return;
            }

            if (this._globalKey == null)
                this._globalKey = new ManagedWinapi.Hotkey();

            // Make sure that we don't try to reregister the key midway updating the combination.
            if (this._globalKey.Enabled)
                this._globalKey.Enabled = false;

            this._globalKey.Shift = this.Shift;
            this._globalKey.Ctrl = this.Ctrl;
            this._globalKey.Alt = this.Alt;
            this._globalKey.WindowsKey = this.WindowsKey;
            this._globalKey.KeyCode = ConvertInputKeyToFormsKeys(this.Key);

            // Un-subscribe from the event first, in case it was already subscribed.
            this._globalKey.HotkeyPressed -= this.GlobalKey_HotkeyPressed;
            this._globalKey.HotkeyPressed += this.GlobalKey_HotkeyPressed;

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
            }
            else if (this.Shift == false && this.Ctrl == false && this.Alt == false)
            {
                this.IsValid = false;
                this.InvalidReason = "At least one modifier key must be selected";
            }
            else
            {
                this.IsValid = true;
                this.InvalidReason = "";
            }
        }

        private void GlobalKey_HotkeyPressed(object sender, EventArgs e)
        {
            Toast.HotkeyActionCallback(this);
        }

        #region INotifyPropertyChanged

        private void NotifyPropertyChanged(string info, bool checkIfValid = true)
        {
            if (checkIfValid)
                this.CheckIfValid();
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion INotifyPropertyChanged

        #region IXmlSerializable

        private enum Attribute
        {
            Null = 0,
            Enabled,
            Shift,
            Ctrl,
            Alt,
            WindowsKey,
            Key,
            Action
        }

        private static readonly Dictionary<Attribute, List<string>> nodeNames = new Dictionary<Attribute, List<string>>
        {
            { Attribute.Enabled,    new List<string> { "Enabled" } },
            { Attribute.Shift,      new List<string> { "Shift" } },
            { Attribute.Ctrl,       new List<string> { "Ctrl" } },
            { Attribute.Alt,        new List<string> { "Alt" } },
            { Attribute.WindowsKey, new List<string> { "WindowsKey", "Win", "WinKey" } },
            { Attribute.Key,        new List<string> { "Key" } },
            { Attribute.Action,     new List<string> { "Action" } }
        };

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement && !reader.HasAttributes)
                return;

            try
            {
                Dictionary<Attribute, bool> deserializedFlags = new Dictionary<Attribute, bool>();

                // First, try to read the attributes of the 'Hotkey' node.
                if (reader.MoveToFirstAttribute())
                {
                    do
                    {
                        string name = reader.LocalName;
                        string value = reader.Value;
                        this.ParseNode(name, value, deserializedFlags);
                    } while (reader.MoveToNextAttribute());

                    reader.MoveToElement();
                }

                // Then, try to read the child nodes (for compatibility with the old settings file).
                if (!reader.IsEmptyElement)
                {
                    reader.Read();
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        string name = reader.LocalName;
                        string value = reader.ReadElementContentAsString();
                        this.ParseNode(name, value, deserializedFlags);
                    }
                }
            }
            finally
            {
                if (!reader.IsEmptyElement)
                    reader.ReadEndElement();
                else
                    reader.Skip();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString(nodeNames[Attribute.Action].First(), this.Action.ToString());
            writer.WriteAttributeString(nodeNames[Attribute.Enabled].First(), this.Enabled.ToString());
            writer.WriteAttributeString(nodeNames[Attribute.Shift].First(), this.Shift.ToString());
            writer.WriteAttributeString(nodeNames[Attribute.Ctrl].First(), this.Ctrl.ToString());
            writer.WriteAttributeString(nodeNames[Attribute.Alt].First(), this.Alt.ToString());
            writer.WriteAttributeString(nodeNames[Attribute.WindowsKey].First(), this.WindowsKey.ToString());
            writer.WriteAttributeString(nodeNames[Attribute.Key].First(), this.Key.ToString());
        }

        private void ParseNode(string localName, string value, IDictionary<Attribute, bool> deserializedFlags)
        {
            Attribute hotkeyAttribute = nodeNames
                .FirstOrDefault(kvp => kvp.Value.Contains(localName, StringComparer.InvariantCultureIgnoreCase))
                .Key;

            // The current node name does not match any of the known names.
            if (hotkeyAttribute == Attribute.Null)
                return;

            // The current node is a duplicate.
            if (deserializedFlags.ContainsKey(hotkeyAttribute) && deserializedFlags[hotkeyAttribute])
                return;

            // Parse the value.
            this.ParseNodeValue(hotkeyAttribute, value, deserializedFlags);
        }

        private void ParseNodeValue(Attribute attribute, string value, IDictionary<Attribute, bool> deserializedFlags)
        {
            if (deserializedFlags.ContainsKey(attribute))
                deserializedFlags[attribute] = true;

            switch (attribute)
            {
                case Attribute.Enabled:
                    this.Enabled = bool.Parse(value);
                    break;

                case Attribute.Shift:
                    this.Shift = bool.Parse(value);
                    break;

                case Attribute.Ctrl:
                    this.Ctrl = bool.Parse(value);
                    break;

                case Attribute.Alt:
                    this.Alt = bool.Parse(value);
                    break;

                case Attribute.WindowsKey:
                    this.WindowsKey = bool.Parse(value);
                    break;

                case Attribute.Key:
                    this.Key = (Key)Enum.Parse(typeof(Key), value);
                    break;

                case Attribute.Action:
                    SpotifyAction action;
                    if (!Enum.TryParse(value, true, out action))
                        action = SpotifyAction.None;
                    this.Action = action;
                    break;

                default:
                    return;
            }
        }

        #endregion IXmlSerializable

        #region Static functions

        public static void ClearAll()
        {
            // Disable will be called by the destructors, but we want to force a disable
            // now so that we don't wait for the GC to clean up the objects
            foreach (Hotkey hotkey in hotkeys)
                hotkey.Deactivate();
            hotkeys.Clear();
        }

        private static Keys ConvertInputKeyToFormsKeys(Key key)
        {
            if (Enum.GetNames(typeof(Keys)).Contains(key.ToString()))
                return (Keys)Enum.Parse(typeof(Keys), key.ToString());

            return Keys.None;
        }

        #endregion Static functions
    }
}