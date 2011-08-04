using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using System.Globalization;

namespace Toastify
{
    [Serializable]
    public class SettingsXml : INotifyPropertyChanged
    {

        #region Singleton

        private static SettingsXml _theOne;

        public static SettingsXml Current
        {
            get
            {
                if (_theOne == null)
                {
                    _theOne = new SettingsXml();
                }

                return _theOne;
            }

            private set 
            {
                if (_theOne != null && _theOne != value)
                {
                    _theOne.UnloadSettings();

                    _theOne = value;

                    _theOne.ApplySettings();
                }
            }
        }

        #endregion

        private bool _GlobalHotKeys;
        private bool _DisableToast;
        private bool? _AlwaysStartSpotify;
        private int _FadeOutTime;
        private string _ToastColorTop;
        private string _ToastColorBottom;
        private string _ToastBorderColor;
        private double _ToastBorderThickness;
        //private string _ToastBorderCornerRadius;
        private double _ToastBorderCornerRadiusTopLeft;
        private double _ToastBorderCornerRadiusTopRight;
        private double _ToastBorderCornerRadiusBottomRight;
        private double _ToastBorderCornerRadiusBottomLeft;
        private double _ToastWidth;
        private double _ToastHeight;
        private double _OffsetRight;
        private double _OffsetBottom;
        private string _ClipboardTemplate;
        private List<Hotkey> _HotKeys;
        public List<PluginDetails> Plugins { get; set; }

        public bool GlobalHotKeys
        {
            get { return _GlobalHotKeys; }
            set
            {
                if (_GlobalHotKeys != value)
                {
                    _GlobalHotKeys = value;
                    NotifyPropertyChanged("GlobalHotKeys");
                }                
            }
        }

        public bool DisableToast
        {
            get { return _DisableToast; }
            set
            {
                if (_DisableToast != value)
                {
                    _DisableToast = value;

                    PropertyChanged(this, new PropertyChangedEventArgs("DisableToast"));
                }
            }
        }

        public bool? AlwaysStartSpotify
        {
            get { return _AlwaysStartSpotify; }
            set
            {
                if (_AlwaysStartSpotify != value)
                {
                    _AlwaysStartSpotify = value;

                    NotifyPropertyChanged("AlwaysStartSpotify");
                }
            }
        }

        public int FadeOutTime
        {
            get { return _FadeOutTime; }
            set
            {
                if (_FadeOutTime != value)
                {
                    _FadeOutTime = value;

                    NotifyPropertyChanged("FadeOutTime");
                }
            }
        }

        public string ToastColorTop
        {
            get { return _ToastColorTop; }
            set
            {
                if (_ToastColorTop != value)
                {
                    _ToastColorTop = value;

                    NotifyPropertyChanged("ToastColorTop");
                }
            }
        }

        public string ToastColorBottom
        {
            get { return _ToastColorBottom; }
            set
            {
                if (_ToastColorBottom != value)
                {
                    _ToastColorBottom = value;

                    NotifyPropertyChanged("ToastColorBottom");
                }
            }
        }

        public string ToastBorderColor
        {
            get { return _ToastBorderColor; }
            set
            {
                if (_ToastBorderColor != value)
                {
                    _ToastBorderColor = value;

                    NotifyPropertyChanged("ToastBorderColor");
                }
            }
        }

        public double ToastBorderThickness
        {
            get { return _ToastBorderThickness; }
            set
            {
                if (_ToastBorderThickness != value)
                {
                    _ToastBorderThickness = value;

                    NotifyPropertyChanged("ToastBorderThickness");
                }
            }
        }

        public double ToastBorderCornerRadiusTopLeft
        {
            get { return _ToastBorderCornerRadiusTopLeft; }
            set
            {
                if (_ToastBorderCornerRadiusTopLeft != value)
                {
                    _ToastBorderCornerRadiusTopLeft = value;

                    NotifyPropertyChanged("ToastBorderCornerRadiusTopLeft");
                }
            }
        }

        public double ToastBorderCornerRadiusTopRight
        {
            get { return _ToastBorderCornerRadiusTopRight; }
            set
            {
                if (_ToastBorderCornerRadiusTopRight != value)
                {
                    _ToastBorderCornerRadiusTopRight = value;

                    NotifyPropertyChanged("ToastBorderCornerRadiusTopRight");
                }
            }
        }

        public double ToastBorderCornerRadiusBottomRight
        {
            get { return _ToastBorderCornerRadiusBottomRight; }
            set
            {
                if (_ToastBorderCornerRadiusBottomRight != value)
                {
                    _ToastBorderCornerRadiusBottomRight = value;

                    NotifyPropertyChanged("ToastBorderCornerRadiusBottomRight");
                }
            }
        }

        public double ToastBorderCornerRadiusBottomLeft
        {
            get { return _ToastBorderCornerRadiusBottomLeft; }
            set
            {
                if (_ToastBorderCornerRadiusBottomLeft != value)
                {
                    _ToastBorderCornerRadiusBottomLeft = value;

                    NotifyPropertyChanged("ToastBorderCornerRadiusBottomLeft");
                }
            }
        }

        public double ToastWidth
        {
            get { return _ToastWidth; }
            set
            {
                if (_ToastWidth != value)
                {
                    _ToastWidth = value;

                    NotifyPropertyChanged("ToastWidth");
                }
            }
        }

        public double ToastHeight
        {
            get { return _ToastHeight; }
            set
            {
                if (_ToastHeight != value)
                {
                    _ToastHeight = value;

                    NotifyPropertyChanged("ToastHeight");
                }
            }
        }

        public double OffsetRight
        {
            get { return _OffsetRight; }
            set
            {
                if (_OffsetRight != value)
                {
                    _OffsetRight = value;

                    NotifyPropertyChanged("OffsetRight");
                }
            }
        }

        public double OffsetBottom
        {
            get { return _OffsetBottom; }
            set
            {
                if (_OffsetBottom != value)
                {
                    _OffsetBottom = value;

                    NotifyPropertyChanged("OffsetBottom");
                }
            }
        }

        public string ClipboardTemplate
        {
            get { return _ClipboardTemplate; }
            set
            {
                if (_ClipboardTemplate != value)
                {
                    _ClipboardTemplate = value;

                    NotifyPropertyChanged("ClipboardTemplate");
                }
            }
        }

        public List<Hotkey> HotKeys
        {
            get { return _HotKeys; }
            set
            {
                if (_HotKeys != value)
                {
                    _HotKeys = value;

                    NotifyPropertyChanged("HotKeys");
                }
            }
        }

        public void Default()
        {
            FadeOutTime = 2000;
            GlobalHotKeys = true;
            DisableToast = false;
            AlwaysStartSpotify = null;
            ToastColorTop = "#FF999999";
            ToastColorBottom = "#FF353535";
            ToastBorderColor = "#FF292929";
            ToastBorderThickness = 1.0;
            ToastWidth = 300;
            ToastHeight = 75;
            ToastBorderCornerRadiusTopLeft = 4.0;
            ToastBorderCornerRadiusTopRight = 4.0;
            ToastBorderCornerRadiusBottomRight = 4.0;
            ToastBorderCornerRadiusBottomLeft = 4.0;
            OffsetRight = 5.0;
            OffsetBottom = 5.0;
            ClipboardTemplate = "I'm currently listening to {0}";
            
            Hotkey.ClearAll();

            HotKeys = new List<Hotkey> 
            {
                new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.Up , Action= SpotifyAction.PlayPause },
                new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.Down , Action= SpotifyAction.Stop },
                new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.Left , Action= SpotifyAction.PreviousTrack },
                new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.Right , Action= SpotifyAction.NextTrack },
                new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.M , Action= SpotifyAction.Mute },
                new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.PageDown , Action= SpotifyAction.VolumeDown },
                new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.PageUp , Action= SpotifyAction.VolumeUp },
                new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.Space , Action= SpotifyAction.ShowToast },
                new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.S , Action= SpotifyAction.ShowSpotify },
                new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.C , Action= SpotifyAction.CopyTrackInfo }
            };
            Plugins = new List<PluginDetails>();
        }

        public void Save(string filename, bool replaceCurrent = false)
        {
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsXml));
                xmlSerializer.Serialize(sw, this);
            }

            if (replaceCurrent)
            {
                Current = this;
            }
        }

        public void Update()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(String.Empty));
        }

        private SettingsXml() { }

        public SettingsXml Load(string filename)
        {
            if (!System.IO.File.Exists(filename))
                throw new FileNotFoundException();

            using (StreamReader sr = new StreamReader(filename))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsXml));
                SettingsXml xml = xmlSerializer.Deserialize(sr) as SettingsXml;
                
                Current = xml;

                return Current;
            }
        }

        /// <summary>
        /// Any active settings (such as hotkeys) should be triggered here
        /// </summary>
        private void ApplySettings()
        {
            if (GlobalHotKeys)
            {
                foreach (Hotkey hotkey in HotKeys)
                {
                    hotkey.Enable();
                }
            }
        }

        /// <summary>
        /// Any active settings (such as hotkeys) should be unloaded here
        /// </summary>
        private void UnloadSettings()
        {
            if (HotKeys != null)
            {
                foreach (Hotkey hotkey in HotKeys)
                {
                    hotkey.Disable();
                }
            }
        }

        public SettingsXml Clone()
        {
            SettingsXml clone = MemberwiseClone() as SettingsXml;
            
            clone.HotKeys = new List<Hotkey>();

            foreach (Hotkey key in HotKeys)
            {
                clone.HotKeys.Add(key.Clone());
            }

            return clone;
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

    [Serializable]
    public class PluginDetails
    {
        public string FileName { get; set; }
        public string TypeName { get; set; }
        public string Settings { get; set; }
    }
}
