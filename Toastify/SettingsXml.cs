using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Win32;

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
                if (_theOne != null)
                {
                    _theOne.UnloadSettings();

                    _theOne = value;

                    _theOne.ApplySettings();
                }
            }
        }

        #endregion

        private const string REG_KEY_STARTUP = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string SETTINGS_FILE   =  "Toastify.xml";

        private bool _CloseSpotifyWithToastify;
        private bool _MinimizeSpotifyOnStartup;
        private bool _GlobalHotKeys;
        private bool _DisableToast;
        private bool _OnlyShowToastOnHotkey;
        private bool? _AlwaysStartSpotify;
        private bool _DontPromptToStartSpotify;
        private bool _ChangeSpotifyVolumeOnly;
        private int _FadeOutTime;
        private string _ToastColorTop;
        private string _ToastColorBottom;
        private string _ToastBorderColor;
        private double _ToastBorderThickness;
        private double _ToastBorderCornerRadiusTopLeft;
        private double _ToastBorderCornerRadiusTopRight;
        private double _ToastBorderCornerRadiusBottomRight;
        private double _ToastBorderCornerRadiusBottomLeft;
        private double _PositionLeft;
        private double _PositionTop;
        private double _ToastWidth;
        private double _ToastHeight;
        private string _ClipboardTemplate;
        private bool _SaveTrackToFile;
        private string _SaveTrackToFilePath;
        private List<Hotkey> _HotKeys;
        public List<PluginDetails> Plugins { get; set; }

        private string _settingsFile;

        // used for both defaults and to synchronize the list of hotkeys when upgrading versions
        // so that any newly added hotkeys are magically visible to the user (without them having to
        // reset their settings)
        private List<Hotkey> _defaultHotKeys = new List<Hotkey> 
            {
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.Up      , Action = SpotifyAction.PlayPause      },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.Down    , Action = SpotifyAction.Stop           },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.Left    , Action = SpotifyAction.PreviousTrack  },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.Right   , Action = SpotifyAction.NextTrack      },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.M       , Action = SpotifyAction.Mute           },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.PageDown, Action = SpotifyAction.VolumeDown     },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.PageUp  , Action = SpotifyAction.VolumeUp       },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.Space   , Action = SpotifyAction.ShowToast      },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.S       , Action = SpotifyAction.ShowSpotify    },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.C       , Action = SpotifyAction.CopyTrackInfo  },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.V       , Action = SpotifyAction.PasteTrackInfo },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.OemPlus , Action = SpotifyAction.FastForward    },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.OemMinus, Action = SpotifyAction.Rewind         },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.Add     , Action = SpotifyAction.ThumbsUp       },
                new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.Subtract, Action = SpotifyAction.ThumbsDown     },
            };

        /// <summary>
        /// Returns the location of the settings file
        /// </summary>
        [XmlIgnore]
        public string SettingsFile
        {
            get
            {
                if (_settingsFile == null)
                {
                    string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Toastify");

                    if (!Directory.Exists(settingsPath))
                    {
                        try
                        {
                            Directory.CreateDirectory(settingsPath);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Exception creating user settings directory (" + settingsPath + "). Exception: " + ex);

                            // No messagebox as this should not happen (and there will be a MessageBox later on when 
                            // settings fail to load)
                        }
                    }

                    _settingsFile = Path.Combine(settingsPath, SETTINGS_FILE);
                }

                return _settingsFile;
            }
        }

        // this is a dynamic setting depending on the existing registry key
        // (which allows for it to be set reliably from the installer)
        // so make sure to not include it in the XML file
        [XmlIgnore]
        public bool LaunchOnStartup
        {
            get { return IsSetToLaunchOnStartup(); }
            set
            {
                NotifyPropertyChanged("LaunchOnStartup");

                SetLaunchOnStartup(value);
            }
        }

        public bool MinimizeSpotifyOnStartup
        {
            get { return _MinimizeSpotifyOnStartup; }
            set
            {
                if (_MinimizeSpotifyOnStartup != value)
                {
                    _MinimizeSpotifyOnStartup = value;
                    NotifyPropertyChanged("MinimizeSpotifyOnStartup");
                }
            }
        }

        public bool CloseSpotifyWithToastify
        {
            get { return _CloseSpotifyWithToastify; }
            set
            {
                if (_CloseSpotifyWithToastify != value)
                {
                    _CloseSpotifyWithToastify = value;
                    NotifyPropertyChanged("CloseSpotifyWithToastify");
                }
            }
        }

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

        public bool OnlyShowToastOnHotkey
        {
            get { return _OnlyShowToastOnHotkey; }
            set
            {
                if (_OnlyShowToastOnHotkey != value)
                {
                    _OnlyShowToastOnHotkey = value;

                    NotifyPropertyChanged("OnlyShowToastOnHotkey");
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

                    NotifyPropertyChanged("DisableToast");
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

        public bool DontPromptToStartSpotify 
        { 
            get { return _DontPromptToStartSpotify; }
            set
            {
                if (_DontPromptToStartSpotify != value)
                {
                    _DontPromptToStartSpotify = value;

                    NotifyPropertyChanged("DontPromptToStartSpotify");
                }
            }
        }

        public bool ChangeSpotifyVolumeOnly
        {
            get { return _ChangeSpotifyVolumeOnly; }
            set
            {
                if (_ChangeSpotifyVolumeOnly != value)
                {
                    _ChangeSpotifyVolumeOnly = value;

                    NotifyPropertyChanged("ChangeSpotifyVolumeOnly");
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
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

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
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

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
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

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
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

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
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

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
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

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
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (_ToastHeight != value)
                {
                    _ToastHeight = value;

                    NotifyPropertyChanged("ToastHeight");
                }
            }
        }

        public double PositionLeft
        {
            get { return _PositionLeft; }
            set
            {
                if (_PositionLeft != value)
                {
                    _PositionLeft = value;

                    NotifyPropertyChanged("PositionLeft");
                }
            }
        }

        public double PositionTop
        {
            get { return _PositionTop; }
            set
            {
                if (_PositionTop != value)
                {
                    _PositionTop = value;

                    NotifyPropertyChanged("PositionTop");
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

        public bool SaveTrackToFile
        {
            get { return _SaveTrackToFile; }
            set
            {
                if (_SaveTrackToFile != value)
                {
                    _SaveTrackToFile = value;

                    NotifyPropertyChanged("SaveTrackToFile");
                }
            }
        }

        public string SaveTrackToFilePath
        {
            get { return _SaveTrackToFilePath; }
            set
            {
                if (_SaveTrackToFilePath != value)
                {
                    _SaveTrackToFilePath = value;

                    NotifyPropertyChanged("SaveTrackToFilePath");
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

        public void Default(bool setHotKeys = false)
        {
            AlwaysStartSpotify       = true;
            DontPromptToStartSpotify = false;
            CloseSpotifyWithToastify = true;
            ChangeSpotifyVolumeOnly  = false;

            FadeOutTime   = 4000;
            GlobalHotKeys = true;
            DisableToast  = false;

            ToastColorTop    = "#FF000000";
            ToastColorBottom = "#FF000000";
            ToastBorderColor = "#FF000000";
            
            ToastBorderThickness = 1.0;

            ToastWidth  = 300;
            ToastHeight = 75;

            ToastBorderCornerRadiusTopLeft     = 0;
            ToastBorderCornerRadiusTopRight    = 0;
            ToastBorderCornerRadiusBottomRight = 0;
            ToastBorderCornerRadiusBottomLeft  = 0;

            var position = ScreenHelper.GetDefaultToastPosition(ToastWidth, ToastHeight);

            PositionLeft = position.X;
            PositionTop  = position.Y;

            ClipboardTemplate = "I'm currently listening to {0}";

            SaveTrackToFile = false;

            Hotkey.ClearAll();

            // only set hotkeys when it's requested (we don't set hotkeys when
            // loading from XML since it will create duplicates)
            if (setHotKeys)
                HotKeys = _defaultHotKeys;

            Plugins = new List<PluginDetails>();
        }

        public void Save(bool replaceCurrent = false)
        {
            using (StreamWriter sw = new StreamWriter(SettingsFile, false))
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

        private SettingsXml() 
        {
            Default();
        }

        public SettingsXml Load()
        {
            if (!System.IO.File.Exists(SettingsFile))
            {
                SettingsXml.Current.Default(setHotKeys: true);
                SettingsXml.Current.Save();
            }
            else
            {
                using (StreamReader sr = new StreamReader(SettingsFile))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsXml));
                    SettingsXml xml = xmlSerializer.Deserialize(sr) as SettingsXml;

                    xml.CheckForNewSettings();

                    Current = xml;
                }
            }

            Current.SanitizeSettingsFile();

            return Current;
        }

        /// <summary>
        /// Helpful place to fix common issues with settings files
        /// </summary>
        private void SanitizeSettingsFile()
        {
            if (this._HotKeys == null)
                return;

            // let's collapse duplicate hotkeys
            var toKeep = new List<Hotkey>();

            for (int i = 0; i < _defaultHotKeys.Count; i++)
            {
                var current = _defaultHotKeys[i];
                var keep = current;

                for (int j = 0; j < _HotKeys.Count; j++)
                {
                    var compare = _HotKeys[j];

                    if (current.Action == compare.Action && compare.Enabled)
                    {
                        keep = compare;
                    }
                }

                toKeep.Add(keep);
            }

            // deactivate all of the old hotkeys, especially duplicate ones that are active
            foreach (var hotkey in _HotKeys)
            {
                if (!toKeep.Contains(hotkey))
                    hotkey.Deactivate();
            }

            _HotKeys = toKeep;

            Save();
        }

        /// <summary>
        /// Called when loading a settings file to iterate through new dynamic properties (such as Hotkeys)
        /// which may have changed and would otherwise be hidden from the user
        /// </summary>
        private void CheckForNewSettings()
        {
            foreach (var defaultHotkey in _defaultHotKeys)
            {
                bool found = false;
                foreach (var hotkey in HotKeys)
                {
                    if (hotkey.Action == defaultHotkey.Action)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    HotKeys.Add(defaultHotkey.Clone());
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
                    hotkey.Activate();
                }
            }
        }

        private bool IsSetToLaunchOnStartup()
        {
            RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REG_KEY_STARTUP, false);

            return (key.GetValue("Toastify", null) != null);
        }

        private void SetLaunchOnStartup(bool enabled)
        {
            RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REG_KEY_STARTUP, true);

            if (enabled)
            {
                key.SetValue("Toastify", "\"" + System.Windows.Forms.Application.ExecutablePath + "\"");
            }
            else
            {
                key.DeleteValue("Toastify", false);
            }

            key.Close();
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
                    hotkey.Deactivate();
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
