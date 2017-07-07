using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Toastify.Common;
using Toastify.Helpers;

namespace Toastify.Core
{
    [Serializable]
    public class SettingsXml : INotifyPropertyChanged
    {
        #region Singleton

        private static SettingsXml _instance;

        public static SettingsXml Instance
        {
            get
            {
                return _instance ?? (_instance = new SettingsXml());
            }
            private set
            {
                if (_instance != null)
                {
                    _instance.UnloadSettings();
                    _instance = value;
                    _instance.ApplySettings();
                }
            }
        }

        #endregion Singleton

        private const string REG_KEY_STARTUP = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string SETTINGS_FILE = "Toastify.xml";

        private string _settingsFile;

        /// <summary>
        /// Returns the location of the settings file
        /// </summary>
        [XmlIgnore]
        public string SettingsFile
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._settingsFile))
                {
                    string settingsPath = App.ApplicationData;

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

                    this._settingsFile = Path.Combine(settingsPath, SETTINGS_FILE);
                }

                return this._settingsFile;
            }
        }

        #region Settings

        #region Private fields

        private bool _minimizeSpotifyOnStartup;
        private bool _closeSpotifyWithToastify;
        private bool _useSpotifyVolumeControl;
        private float _windowsVolumeMixerIncrement;
        private string _clipboardTemplate;
        private bool _saveTrackToFile;
        private string _saveTrackToFilePath;
        private bool _preventSleepWhilePlaying;
        private bool _optInToAnalytics;

        private bool _globalHotKeys;
        private List<Hotkey> _hotKeys;

        private readonly List<Hotkey> defaultHotKeys = new List<Hotkey>
        {
            new Hotkey { Ctrl = true, Alt = true, Key = System.Windows.Input.Key.Up      , Action = SpotifyAction.PlayPause      },
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
        };

        private bool _disableToast;
        private bool _onlyShowToastOnHotkey;
        private int _fadeOutTime;
        private string _toastColorTop;
        private string _toastColorBottom;
        private string _toastBorderColor;
        private double _toastBorderThickness;
        private double _toastBorderCornerRadiusTopLeft;
        private double _toastBorderCornerRadiusTopRight;
        private double _toastBorderCornerRadiusBottomLeft;
        private double _toastBorderCornerRadiusBottomRight;
        private double _toastWidth;
        private double _toastHeight;
        private double _positionLeft;
        private double _positionTop;

        private bool _firstRun;
        private string _previousVersion;
        private int _startupWaitTimeout;
        private int _spotifyConnectionAttempts;

        #endregion Private fields

        #region [General]

        // this is a dynamic setting depending on the existing registry key
        // (which allows for it to be set reliably from the installer) so make sure to not include it in the XML file
        [XmlIgnore]
        public bool LaunchOnStartup
        {
            get
            {
                bool launchOnStartup;
                using (var key = Registry.CurrentUser.OpenSubKey(REG_KEY_STARTUP, false))
                {
                    launchOnStartup = key?.GetValue("Toastify", null) != null;
                }
                return launchOnStartup;
            }
            set
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REG_KEY_STARTUP, true))
                {
                    if (value)
                        key?.SetValue("Toastify", $"\"{System.Windows.Forms.Application.ExecutablePath}\"");
                    else
                        key?.DeleteValue("Toastify", false);
                }

                this.NotifyPropertyChanged("LaunchOnStartup");
            }
        }

        public bool MinimizeSpotifyOnStartup
        {
            get
            {
                return this._minimizeSpotifyOnStartup;
            }
            set
            {
                if (this._minimizeSpotifyOnStartup != value)
                {
                    this._minimizeSpotifyOnStartup = value;
                    this.NotifyPropertyChanged("MinimizeSpotifyOnStartup");
                }
            }
        }

        public bool CloseSpotifyWithToastify
        {
            get
            {
                return this._closeSpotifyWithToastify;
            }
            set
            {
                if (this._closeSpotifyWithToastify != value)
                {
                    this._closeSpotifyWithToastify = value;
                    this.NotifyPropertyChanged("CloseSpotifyWithToastify");
                }
            }
        }

        public bool UseSpotifyVolumeControl
        {
            get
            {
                return this._useSpotifyVolumeControl;
            }
            set
            {
                if (this._useSpotifyVolumeControl != value)
                {
                    this._useSpotifyVolumeControl = value;
                    this.NotifyPropertyChanged("UseSpotifyVolumeControl");
                }
            }
        }

        public float WindowsVolumeMixerIncrement
        {
            get
            {
                return this._windowsVolumeMixerIncrement;
            }
            set
            {
                if (Math.Abs(this._windowsVolumeMixerIncrement - value) > 0.000001f)
                {
                    this._windowsVolumeMixerIncrement = value;
                    this.NotifyPropertyChanged("WindowsVolumeMixerIncrement");
                }
            }
        }

        public string ClipboardTemplate
        {
            get
            {
                return this._clipboardTemplate;
            }
            set
            {
                if (this._clipboardTemplate != value)
                {
                    this._clipboardTemplate = value;
                    this.NotifyPropertyChanged("ClipboardTemplate");
                }
            }
        }

        public bool SaveTrackToFile
        {
            get
            {
                return this._saveTrackToFile;
            }
            set
            {
                if (this._saveTrackToFile != value)
                {
                    this._saveTrackToFile = value;
                    this.NotifyPropertyChanged("SaveTrackToFile");
                }
            }
        }

        public string SaveTrackToFilePath
        {
            get
            {
                return this._saveTrackToFilePath;
            }
            set
            {
                if (this._saveTrackToFilePath != value)
                {
                    this._saveTrackToFilePath = value;
                    this.NotifyPropertyChanged("SaveTrackToFilePath");
                }
            }
        }

        public bool PreventSleepWhilePlaying
        {
            get
            {
                return this._preventSleepWhilePlaying;
            }
            set
            {
                if (this._preventSleepWhilePlaying != value)
                {
                    this._preventSleepWhilePlaying = value;
                    this.NotifyPropertyChanged("PreventSleepWhilePlaying");
                }
            }
        }

        public bool OptInToAnalytics
        {
            get
            {
                return this._optInToAnalytics;
            }
            set
            {
                if (this._optInToAnalytics != value)
                {
                    this._optInToAnalytics = value;
                    this.NotifyPropertyChanged("OptInToAnalytics");
                }
            }
        }

        #endregion [General]

        #region [Hotkeys]

        public bool GlobalHotKeys
        {
            get
            {
                return this._globalHotKeys;
            }
            set
            {
                if (this._globalHotKeys != value)
                {
                    this._globalHotKeys = value;
                    this.NotifyPropertyChanged("GlobalHotKeys");
                }
            }
        }

        public List<Hotkey> HotKeys
        {
            get
            {
                return this._hotKeys;
            }
            set
            {
                if (this._hotKeys != value)
                {
                    this._hotKeys = value;
                    this.NotifyPropertyChanged("HotKeys");
                }
            }
        }

        #endregion [Hotkeys]

        #region [Toast]

        public bool DisableToast
        {
            get
            {
                return this._disableToast;
            }
            set
            {
                if (this._disableToast != value)
                {
                    this._disableToast = value;
                    this.NotifyPropertyChanged("DisableToast");
                }
            }
        }

        /// <summary>
        /// Only show the toast when the `<see cref="SpotifyAction.ShowToast"/>` hotkey is pressed.
        /// </summary>
        public bool OnlyShowToastOnHotkey
        {
            get
            {
                return this._onlyShowToastOnHotkey;
            }
            set
            {
                if (this._onlyShowToastOnHotkey != value)
                {
                    this._onlyShowToastOnHotkey = value;
                    this.NotifyPropertyChanged("OnlyShowToastOnHotkey");
                }
            }
        }

        public int FadeOutTime
        {
            get
            {
                return this._fadeOutTime;
            }
            set
            {
                if (this._fadeOutTime != value)
                {
                    this._fadeOutTime = value;
                    this.NotifyPropertyChanged("FadeOutTime");
                }
            }
        }

        public string ToastColorTop
        {
            get
            {
                return this._toastColorTop;
            }
            set
            {
                if (this._toastColorTop != value)
                {
                    this._toastColorTop = value;
                    this.NotifyPropertyChanged("ToastColorTop");
                }
            }
        }

        public string ToastColorBottom
        {
            get
            {
                return this._toastColorBottom;
            }
            set
            {
                if (this._toastColorBottom != value)
                {
                    this._toastColorBottom = value;
                    this.NotifyPropertyChanged("ToastColorBottom");
                }
            }
        }

        public string ToastBorderColor
        {
            get
            {
                return this._toastBorderColor;
            }
            set
            {
                if (this._toastBorderColor != value)
                {
                    this._toastBorderColor = value;
                    this.NotifyPropertyChanged("ToastBorderColor");
                }
            }
        }

        public double ToastBorderThickness
        {
            get
            {
                return this._toastBorderThickness;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._toastBorderThickness - value) > double.Epsilon)
                {
                    this._toastBorderThickness = value;
                    this.NotifyPropertyChanged("ToastBorderThickness");
                }
            }
        }

        public double ToastBorderCornerRadiusTopLeft
        {
            get
            {
                return this._toastBorderCornerRadiusTopLeft;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._toastBorderCornerRadiusTopLeft - value) > double.Epsilon)
                {
                    this._toastBorderCornerRadiusTopLeft = value;
                    this.NotifyPropertyChanged("ToastBorderCornerRadiusTopLeft");
                }
            }
        }

        public double ToastBorderCornerRadiusTopRight
        {
            get
            {
                return this._toastBorderCornerRadiusTopRight;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._toastBorderCornerRadiusTopRight - value) > double.Epsilon)
                {
                    this._toastBorderCornerRadiusTopRight = value;
                    this.NotifyPropertyChanged("ToastBorderCornerRadiusTopRight");
                }
            }
        }

        public double ToastBorderCornerRadiusBottomLeft
        {
            get
            {
                return this._toastBorderCornerRadiusBottomLeft;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._toastBorderCornerRadiusBottomLeft - value) > double.Epsilon)
                {
                    this._toastBorderCornerRadiusBottomLeft = value;
                    this.NotifyPropertyChanged("ToastBorderCornerRadiusBottomLeft");
                }
            }
        }

        public double ToastBorderCornerRadiusBottomRight
        {
            get
            {
                return this._toastBorderCornerRadiusBottomRight;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._toastBorderCornerRadiusBottomRight - value) > double.Epsilon)
                {
                    this._toastBorderCornerRadiusBottomRight = value;
                    this.NotifyPropertyChanged("ToastBorderCornerRadiusBottomRight");
                }
            }
        }

        public double ToastWidth
        {
            get
            {
                return this._toastWidth;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._toastWidth - value) > double.Epsilon)
                {
                    this._toastWidth = value;
                    this.NotifyPropertyChanged("ToastWidth");
                }
            }
        }

        public double ToastHeight
        {
            get
            {
                return this._toastHeight;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._toastHeight - value) > double.Epsilon)
                {
                    this._toastHeight = value;
                    this.NotifyPropertyChanged("ToastHeight");
                }
            }
        }

        public double PositionLeft
        {
            get
            {
                return this._positionLeft;
            }
            set
            {
                if (Math.Abs(this._positionLeft - value) > double.Epsilon)
                {
                    this._positionLeft = value;
                    this.NotifyPropertyChanged("PositionLeft");
                }
            }
        }

        public double PositionTop
        {
            get
            {
                return this._positionTop;
            }
            set
            {
                if (Math.Abs(this._positionTop - value) > double.Epsilon)
                {
                    this._positionTop = value;
                    this.NotifyPropertyChanged("PositionTop");
                }
            }
        }

        #endregion [Toast]

        #region (hidden)

        public bool FirstRun
        {
            get
            {
                return this._firstRun;
            }
            set
            {
                if (this._firstRun != value)
                {
                    this._firstRun = value;
                    this.NotifyPropertyChanged("FirstRun");
                }
            }
        }

        public string PreviousVersion
        {
            get
            {
                return this._previousVersion;
            }
            set
            {
                if (this._previousVersion != value)
                {
                    this._previousVersion = value;
                    this.NotifyPropertyChanged("PreviousVersion");
                }
            }
        }

        public int StartupWaitTimeout
        {
            get
            {
                return this._startupWaitTimeout;
            }
            set
            {
                if (this._startupWaitTimeout != value)
                {
                    this._startupWaitTimeout = value;
                    this.NotifyPropertyChanged("StartupWaitTimeout");
                }
            }
        }

        public int SpotifyConnectionAttempts
        {
            get
            {
                return this._spotifyConnectionAttempts;
            }
            set
            {
                if (this._spotifyConnectionAttempts != value)
                {
                    this._spotifyConnectionAttempts = value;
                    this.NotifyPropertyChanged("SpotifyConnectionAttempts");
                }
            }
        }

        public WindowPosition SettingsWindowLastLocation { get; set; }

        public List<PluginDetails> Plugins { get; set; }

        #endregion (hidden)

        #endregion Settings

        private SettingsXml()
        {
            this.Default();
        }

        public void Default(bool setHotKeys = false)
        {
            // [General]
            this.MinimizeSpotifyOnStartup = false;
            this.CloseSpotifyWithToastify = true;
            this.UseSpotifyVolumeControl = false;
            this.WindowsVolumeMixerIncrement = 2.0f;

            this.ClipboardTemplate = "I'm currently listening to {0}";
            this.SaveTrackToFile = false;
            this.SaveTrackToFilePath = Path.Combine(App.ApplicationData, "current_song.txt");

            this.PreventSleepWhilePlaying = false;
            this.OptInToAnalytics = false;

            // [Hotkeys]
            Hotkey.ClearAll();
            this.GlobalHotKeys = true;

            // Only set hotkeys when it's requested (we don't set hotkeys when loading from XML since it will create duplicates)
            if (setHotKeys)
                this.HotKeys = this.defaultHotKeys;

            // [Toast]
            this.DisableToast = false;
            this.OnlyShowToastOnHotkey = true;
            this.FadeOutTime = 4000;

            this.ToastColorTop = "#FF000000";
            this.ToastColorBottom = "#FF000000";
            this.ToastBorderColor = "#FF000000";
            this.ToastBorderThickness = 1.0;

            this.ToastBorderCornerRadiusTopLeft = 0;
            this.ToastBorderCornerRadiusTopRight = 0;
            this.ToastBorderCornerRadiusBottomRight = 0;
            this.ToastBorderCornerRadiusBottomLeft = 0;

            this.ToastWidth = 300;
            this.ToastHeight = 75;

            var position = ScreenHelper.GetDefaultToastPosition(this.ToastWidth, this.ToastHeight);

            this.PositionLeft = position.X;
            this.PositionTop = position.Y;

            // (hidden)
            this.StartupWaitTimeout = 20000;
            this.SpotifyConnectionAttempts = 5;
            this.Plugins = new List<PluginDetails>();

            // There are a few settings that we don't really want to override when
            // clearing settings (in fact these are more properties that we store
            // alongside settings for convenience), so don't reset them if they have values
            if (_instance != null)
            {
                this.FirstRun = _instance.FirstRun;
                this.PreviousVersion = _instance.PreviousVersion;
            }
        }

        public void Save(bool replaceCurrent = false)
        {
            using (StreamWriter sw = new StreamWriter(this.SettingsFile, false))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsXml));
                xmlSerializer.Serialize(sw, this);
            }

            if (replaceCurrent)
                Instance = this;
        }

        public SettingsXml Load()
        {
            if (!File.Exists(this.SettingsFile))
            {
                Instance.Default(true);
                Instance.Save();
            }
            else
            {
                using (StreamReader sr = new StreamReader(this.SettingsFile))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsXml));
                    SettingsXml xml = xmlSerializer.Deserialize(sr) as SettingsXml;

                    xml?.CheckForNewSettings();

                    Instance = xml;
                }
            }

            Instance?.SanitizeSettingsFile();

            return Instance;
        }

        /// <summary>
        /// Helpful place to fix common issues with settings files
        /// </summary>
        private void SanitizeSettingsFile()
        {
            if (this._hotKeys == null)
                return;

            // Let's collapse duplicate hotkeys
            var toKeep = new List<Hotkey>();

            foreach (Hotkey current in this.defaultHotKeys)
            {
                Hotkey keep = current;

                foreach (Hotkey compare in this._hotKeys)
                {
                    if (current.Action == compare.Action && compare.Enabled)
                        keep = compare;
                }

                toKeep.Add(keep);
            }

            // Deactivate all of the old hotkeys, especially duplicate ones that are active
            foreach (Hotkey hotkey in this._hotKeys)
            {
                if (!toKeep.Contains(hotkey))
                    hotkey.Deactivate();
            }

            this._hotKeys = toKeep;

            // Validate WindowsVolumeMixerIncrement: must be positive!
            this.WindowsVolumeMixerIncrement = Math.Abs(this.WindowsVolumeMixerIncrement);

            // Validate StartupWaitTimeout: it cannot be negative!
            this.StartupWaitTimeout = Math.Abs(this.StartupWaitTimeout);

            // Validate SpotifyConnectionAttempts: it cannot be non-positive!
            this.SpotifyConnectionAttempts = Math.Abs(this.SpotifyConnectionAttempts);
            this.SpotifyConnectionAttempts += this.SpotifyConnectionAttempts == 0 ? 1 : 0;

            this.Save();
        }

        /// <summary>
        /// Called when loading a settings file to iterate through new dynamic properties (such as Hotkeys)
        /// which may have changed and would otherwise be hidden from the user
        /// </summary>
        private void CheckForNewSettings()
        {
            foreach (Hotkey defaultHotkey in this.defaultHotKeys)
            {
                bool found = this.HotKeys.Any(hotkey => hotkey.Action == defaultHotkey.Action);

                if (!found)
                    this.HotKeys.Add(defaultHotkey.Clone());
            }
        }

        /// <summary>
        /// Any active settings (such as hotkeys) should be triggered here
        /// </summary>
        private void ApplySettings()
        {
            if (this.GlobalHotKeys)
            {
                foreach (Hotkey hotkey in this.HotKeys)
                    hotkey.Activate();
            }
        }

        /// <summary>
        /// Any active settings (such as hotkeys) should be unloaded here
        /// </summary>
        private void UnloadSettings()
        {
            if (this.HotKeys != null)
            {
                foreach (Hotkey hotkey in this.HotKeys)
                    hotkey.Deactivate();
            }
        }

        public SettingsXml Clone()
        {
            SettingsXml clone = this.MemberwiseClone() as SettingsXml;

            if (clone != null)
            {
                clone.HotKeys = new List<Hotkey>();

                foreach (Hotkey key in this.HotKeys)
                    clone.HotKeys.Add(key.Clone());
            }

            return clone;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion INotifyPropertyChanged
    }

    [Serializable]
    public class PluginDetails
    {
        public string FileName { get; set; }
        public string TypeName { get; set; }
        public string Settings { get; set; }
    }
}