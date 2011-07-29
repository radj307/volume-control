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
        public event PropertyChangedEventHandler PropertyChanged;


        private bool _GlobalHotKeys;
        private bool _DisableToast;
        private bool? _AlwaysStartSpotify;
        private int _FadeOutTime;
        private string _ToastColorTop;
        private string _ToastColorBottom;
        private string _ToastBorderColor;
        private double _ToastBorderThickness;
        //private string _ToastBorderCornerRadious;
        private double _ToastBorderCornerRadiousTopLeft;
        private double _ToastBorderCornerRadiousTopRight;
        private double _ToastBorderCornerRadiousBottomRight;
        private double _ToastBorderCornerRadiousBottomLeft;
        private double _ToastWidth;
        private double _ToastHeight;
        private double _OffsetRight;
        private double _OffsetBottom;
        private string _ClipboardTemplate;
        private List<Hotkey> _HotKeys;
        public List<PluginDetails> Plugins { get; set; }

        public bool GlobalHotKeys
        {
            get
            {
                return _GlobalHotKeys;
            }
            set
            {
                _GlobalHotKeys = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("GlobalHotKeys"));
            }
        }

        public bool DisableToast
        {
            get
            {
                return _DisableToast;
            }
            set
            {
                _DisableToast = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DisableToast"));
            }
        }

        public bool? AlwaysStartSpotify
        {
            get
            {
                return _AlwaysStartSpotify;
            }
            set
            {
                _AlwaysStartSpotify = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("AlwaysStartSpotify"));
            }
        }

        public int FadeOutTime
        {
            get
            {
                return _FadeOutTime;
            }
            set
            {
                _FadeOutTime = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FadeOutTime"));
            }
        }

        public string ToastColorTop
        {
            get
            {
                return _ToastColorTop;
            }
            set
            {
                _ToastColorTop = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ToastColorTop"));
            }
        }

        public string ToastColorBottom
        {
            get
            {
                return _ToastColorBottom;
            }
            set
            {
                _ToastColorBottom = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ToastColorBottom"));
            }
        }

        public string ToastBorderColor
        {
            get
            {
                return _ToastBorderColor;
            }
            set
            {
                _ToastBorderColor = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ToastBorderColor"));
            }
        }

        public double ToastBorderThickness
        {
            get
            {
                return _ToastBorderThickness;
            }
            set
            {
                _ToastBorderThickness = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ToastBorderThickness"));
            }
        }

        public double ToastBorderCornerRadiousTopLeft
        {
            get
            {
                return _ToastBorderCornerRadiousTopLeft;
            }
            set
            {
                _ToastBorderCornerRadiousTopLeft = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ToastBorderCornerRadiousTopLeft"));
            }
        }

        public double ToastBorderCornerRadiousTopRight
        {
            get
            {
                return _ToastBorderCornerRadiousTopRight;
            }
            set
            {
                _ToastBorderCornerRadiousTopRight = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ToastBorderCornerRadiousTopRight"));
            }
        }

        public double ToastBorderCornerRadiousBottomRight
        {
            get
            {
                return _ToastBorderCornerRadiousBottomRight;
            }
            set
            {
                _ToastBorderCornerRadiousBottomRight = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ToastBorderCornerRadiousBottomRight"));
            }
        }

        public double ToastBorderCornerRadiousBottomLeft
        {
            get
            {
                return _ToastBorderCornerRadiousBottomLeft;
            }
            set
            {
                _ToastBorderCornerRadiousBottomLeft = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ToastBorderCornerRadiousBottomLeft"));
            }
        }

        public double ToastWidth
        {
            get
            {
                return _ToastWidth;
            }
            set
            {
                _ToastWidth = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ToastWidth"));
            }
        }

        public double ToastHeight
        {
            get
            {
                return _ToastHeight;
            }
            set
            {
                _ToastHeight = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ToastHeight"));
            }
        }

        public double OffsetRight
        {
            get
            {
                return _OffsetRight;
            }
            set
            {
                _OffsetRight = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("OffsetRight"));
            }
        }

        public double OffsetBottom
        {
            get
            {
                return _OffsetBottom;
            }
            set
            {
                _OffsetBottom = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("OffsetBottom"));
            }
        }

        public string ClipboardTemplate
        {
            get
            {
                return _ClipboardTemplate;
            }
            set
            {
                _ClipboardTemplate = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ClipboardTemplate"));
            }
        }

        public List<Hotkey> HotKeys
        {
            get
            {
                return _HotKeys;
            }
            set
            {
                _HotKeys = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("HotKeys"));
            }
        }

        public void Defaul()
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
            ToastBorderCornerRadiousTopLeft = 4.0;
            ToastBorderCornerRadiousTopRight = 4.0;
            ToastBorderCornerRadiousBottomRight = 4.0;
            ToastBorderCornerRadiousBottomLeft = 4.0;
            OffsetRight = 5.0;
            OffsetBottom = 5.0;
            ClipboardTemplate = "I'm currently listening to {0}";
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

        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsXml));
                xmlSerializer.Serialize(sw, this);
            }
        }

        public void Update()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(String.Empty));
        }

        public static SettingsXml Open(string filename)
        {
            if (!System.IO.File.Exists(filename))
                throw new FileNotFoundException();

            using (StreamReader sr = new StreamReader(filename))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsXml));
                SettingsXml xml = xmlSerializer.Deserialize(sr) as SettingsXml;
                return xml;
            }
        }
    }

    [Serializable]
    public class PluginDetails
    {
        public string FileName { get; set; }
        public string TypeName { get; set; }
        public string Settings { get; set; }
    }
}
