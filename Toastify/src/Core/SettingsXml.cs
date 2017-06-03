using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
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
        private bool _PreventAnalytics;
        private bool _FirstRun;
        private string _PreviousOS;
        private string _PreviousVersion;
        private bool _PreventSleepWhilePlaying;
        private List<Hotkey> _HotKeys;
        public List<PluginDetails> Plugins { get; set; }

        private string _settingsFile;

        // used for both defaults and to synchronize the list of hotkeys when upgrading versions
        // so that any newly added hotkeys are magically visible to the user (without them having to
        // reset their settings)
        private readonly List<Hotkey> _defaultHotKeys = new List<Hotkey>
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
        };

        /// <summary>
        /// Returns the location of the settings file
        /// </summary>
        [XmlIgnore]
        public string SettingsFile
        {
            get
            {
                if (this._settingsFile == null)
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

                    this._settingsFile = Path.Combine(settingsPath, SETTINGS_FILE);
                }

                return this._settingsFile;
            }
        }

        // this is a dynamic setting depending on the existing registry key
        // (which allows for it to be set reliably from the installer)
        // so make sure to not include it in the XML file
        [XmlIgnore]
        public bool LaunchOnStartup
        {
            get
            {
                return this.IsSetToLaunchOnStartup();
            }
            set
            {
                this.NotifyPropertyChanged("LaunchOnStartup");
                this.SetLaunchOnStartup(value);
            }
        }

        public bool MinimizeSpotifyOnStartup
        {
            get
            {
                return this._MinimizeSpotifyOnStartup;
            }
            set
            {
                if (this._MinimizeSpotifyOnStartup != value)
                {
                    this._MinimizeSpotifyOnStartup = value;
                    this.NotifyPropertyChanged("MinimizeSpotifyOnStartup");
                }
            }
        }

        public bool CloseSpotifyWithToastify
        {
            get
            {
                return this._CloseSpotifyWithToastify;
            }
            set
            {
                if (this._CloseSpotifyWithToastify != value)
                {
                    this._CloseSpotifyWithToastify = value;
                    this.NotifyPropertyChanged("CloseSpotifyWithToastify");
                }
            }
        }

        public bool GlobalHotKeys
        {
            get
            {
                return this._GlobalHotKeys;
            }
            set
            {
                if (this._GlobalHotKeys != value)
                {
                    this._GlobalHotKeys = value;
                    this.NotifyPropertyChanged("GlobalHotKeys");
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
                return this._OnlyShowToastOnHotkey;
            }
            set
            {
                if (this._OnlyShowToastOnHotkey != value)
                {
                    this._OnlyShowToastOnHotkey = value;
                    this.NotifyPropertyChanged("OnlyShowToastOnHotkey");
                }
            }
        }

        public bool DisableToast
        {
            get
            {
                return this._DisableToast;
            }
            set
            {
                if (this._DisableToast != value)
                {
                    this._DisableToast = value;
                    this.NotifyPropertyChanged("DisableToast");
                }
            }
        }

        public bool? AlwaysStartSpotify
        {
            get
            {
                return this._AlwaysStartSpotify;
            }
            set
            {
                if (this._AlwaysStartSpotify != value)
                {
                    this._AlwaysStartSpotify = value;
                    this.NotifyPropertyChanged("AlwaysStartSpotify");
                }
            }
        }

        public bool DontPromptToStartSpotify
        {
            get
            {
                return this._DontPromptToStartSpotify;
            }
            set
            {
                if (this._DontPromptToStartSpotify != value)
                {
                    this._DontPromptToStartSpotify = value;
                    this.NotifyPropertyChanged("DontPromptToStartSpotify");
                }
            }
        }

        public bool ChangeSpotifyVolumeOnly
        {
            get
            {
                return this._ChangeSpotifyVolumeOnly;
            }
            set
            {
                if (this._ChangeSpotifyVolumeOnly != value)
                {
                    this._ChangeSpotifyVolumeOnly = value;
                    this.NotifyPropertyChanged("ChangeSpotifyVolumeOnly");
                }
            }
        }

        public int FadeOutTime
        {
            get
            {
                return this._FadeOutTime;
            }
            set
            {
                if (this._FadeOutTime != value)
                {
                    this._FadeOutTime = value;
                    this.NotifyPropertyChanged("FadeOutTime");
                }
            }
        }

        public string ToastColorTop
        {
            get
            {
                return this._ToastColorTop;
            }
            set
            {
                if (this._ToastColorTop != value)
                {
                    this._ToastColorTop = value;
                    this.NotifyPropertyChanged("ToastColorTop");
                }
            }
        }

        public string ToastColorBottom
        {
            get
            {
                return this._ToastColorBottom;
            }
            set
            {
                if (this._ToastColorBottom != value)
                {
                    this._ToastColorBottom = value;
                    this.NotifyPropertyChanged("ToastColorBottom");
                }
            }
        }

        public string ToastBorderColor
        {
            get
            {
                return this._ToastBorderColor;
            }
            set
            {
                if (this._ToastBorderColor != value)
                {
                    this._ToastBorderColor = value;
                    this.NotifyPropertyChanged("ToastBorderColor");
                }
            }
        }

        public double ToastBorderThickness
        {
            get
            {
                return this._ToastBorderThickness;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._ToastBorderThickness - value) > double.Epsilon)
                {
                    this._ToastBorderThickness = value;
                    this.NotifyPropertyChanged("ToastBorderThickness");
                }
            }
        }

        public double ToastBorderCornerRadiusTopLeft
        {
            get
            {
                return this._ToastBorderCornerRadiusTopLeft;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._ToastBorderCornerRadiusTopLeft - value) > double.Epsilon)
                {
                    this._ToastBorderCornerRadiusTopLeft = value;
                    this.NotifyPropertyChanged("ToastBorderCornerRadiusTopLeft");
                }
            }
        }

        public double ToastBorderCornerRadiusTopRight
        {
            get
            {
                return this._ToastBorderCornerRadiusTopRight;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._ToastBorderCornerRadiusTopRight - value) > double.Epsilon)
                {
                    this._ToastBorderCornerRadiusTopRight = value;
                    this.NotifyPropertyChanged("ToastBorderCornerRadiusTopRight");
                }
            }
        }

        public double ToastBorderCornerRadiusBottomRight
        {
            get
            {
                return this._ToastBorderCornerRadiusBottomRight;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._ToastBorderCornerRadiusBottomRight - value) > double.Epsilon)
                {
                    this._ToastBorderCornerRadiusBottomRight = value;
                    this.NotifyPropertyChanged("ToastBorderCornerRadiusBottomRight");
                }
            }
        }

        public double ToastBorderCornerRadiusBottomLeft
        {
            get
            {
                return this._ToastBorderCornerRadiusBottomLeft;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._ToastBorderCornerRadiusBottomLeft - value) > double.Epsilon)
                {
                    this._ToastBorderCornerRadiusBottomLeft = value;
                    this.NotifyPropertyChanged("ToastBorderCornerRadiusBottomLeft");
                }
            }
        }

        public double ToastWidth
        {
            get
            {
                return this._ToastWidth;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._ToastWidth - value) > double.Epsilon)
                {
                    this._ToastWidth = value;
                    this.NotifyPropertyChanged("ToastWidth");
                }
            }
        }

        public double ToastHeight
        {
            get
            {
                return this._ToastHeight;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be a positive number");

                if (Math.Abs(this._ToastHeight - value) > double.Epsilon)
                {
                    this._ToastHeight = value;
                    this.NotifyPropertyChanged("ToastHeight");
                }
            }
        }

        public double PositionLeft
        {
            get
            {
                return this._PositionLeft;
            }
            set
            {
                if (Math.Abs(this._PositionLeft - value) > double.Epsilon)
                {
                    this._PositionLeft = value;
                    this.NotifyPropertyChanged("PositionLeft");
                }
            }
        }

        public double PositionTop
        {
            get
            {
                return this._PositionTop;
            }
            set
            {
                if (Math.Abs(this._PositionTop - value) > double.Epsilon)
                {
                    this._PositionTop = value;
                    this.NotifyPropertyChanged("PositionTop");
                }
            }
        }

        public string ClipboardTemplate
        {
            get
            {
                return this._ClipboardTemplate;
            }
            set
            {
                if (this._ClipboardTemplate != value)
                {
                    this._ClipboardTemplate = value;
                    this.NotifyPropertyChanged("ClipboardTemplate");
                }
            }
        }

        public bool SaveTrackToFile
        {
            get
            {
                return this._SaveTrackToFile;
            }
            set
            {
                if (this._SaveTrackToFile != value)
                {
                    this._SaveTrackToFile = value;
                    this.NotifyPropertyChanged("SaveTrackToFile");
                }
            }
        }

        public string SaveTrackToFilePath
        {
            get
            {
                return this._SaveTrackToFilePath;
            }
            set
            {
                if (this._SaveTrackToFilePath != value)
                {
                    this._SaveTrackToFilePath = value;
                    this.NotifyPropertyChanged("SaveTrackToFilePath");
                }
            }
        }

        public bool PreventAnalytics
        {
            get
            {
                return this._PreventAnalytics;
            }
            set
            {
                if (this._PreventAnalytics != value)
                {
                    this._PreventAnalytics = value;
                    this.NotifyPropertyChanged("PreventAnalytics");
                }
            }
        }

        public bool FirstRun
        {
            get
            {
                return this._FirstRun;
            }
            set
            {
                if (this._FirstRun != value)
                {
                    this._FirstRun = value;
                    this.NotifyPropertyChanged("FirstRun");
                }
            }
        }

        public List<Hotkey> HotKeys
        {
            get
            {
                return this._HotKeys;
            }
            set
            {
                if (this._HotKeys != value)
                {
                    this._HotKeys = value;
                    this.NotifyPropertyChanged("HotKeys");
                }
            }
        }

        public string PreviousOS
        {
            get
            {
                return this._PreviousOS;
            }
            set
            {
                if (this._PreviousOS != value)
                {
                    this._PreviousOS = value;
                    this.NotifyPropertyChanged("PreviousOS");
                }
            }
        }

        public string PreviousVersion
        {
            get
            {
                return this._PreviousVersion;
            }
            set
            {
                if (this._PreviousVersion != value)
                {
                    this._PreviousVersion = value;
                    this.NotifyPropertyChanged("PreviousVersion");
                }
            }
        }

        public bool PreventSleepWhilePlaying
        {
            get
            {
                return this._PreventSleepWhilePlaying;
            }
            set
            {
                if (this._PreventSleepWhilePlaying != value)
                {
                    this._PreventSleepWhilePlaying = value;
                    this.NotifyPropertyChanged("PreventSleepWhilePlaying");
                }
            }
        }

        private SettingsXml()
        {
            this.Default();
        }

        public void Default(bool setHotKeys = false)
        {
            this.AlwaysStartSpotify = true;
            this.DontPromptToStartSpotify = false;
            this.CloseSpotifyWithToastify = true;
            this.ChangeSpotifyVolumeOnly = false;

            this.FadeOutTime = 4000;
            this.GlobalHotKeys = true;
            this.DisableToast = false;

            this.ToastColorTop = "#FF000000";
            this.ToastColorBottom = "#FF000000";
            this.ToastBorderColor = "#FF000000";

            this.ToastBorderThickness = 1.0;

            this.ToastWidth = 300;
            this.ToastHeight = 75;

            this.ToastBorderCornerRadiusTopLeft = 0;
            this.ToastBorderCornerRadiusTopRight = 0;
            this.ToastBorderCornerRadiusBottomRight = 0;
            this.ToastBorderCornerRadiusBottomLeft = 0;

            var position = ScreenHelper.GetDefaultToastPosition(this.ToastWidth, this.ToastHeight);

            this.PositionLeft = position.X;
            this.PositionTop = position.Y;

            this.ClipboardTemplate = "I'm currently listening to {0}";

            this.SaveTrackToFile = false;
            this.PreventAnalytics = false;
            this.PreventSleepWhilePlaying = false;

            Hotkey.ClearAll();

            // only set hotkeys when it's requested (we don't set hotkeys when
            // loading from XML since it will create duplicates)
            if (setHotKeys)
                this.HotKeys = this._defaultHotKeys;

            this.Plugins = new List<PluginDetails>();

            // there are a few settings that we don't really want to override when
            // clearing settings (in fact these are more properties that we store
            // alongside settings for convenience), so don't reset them if they have
            // values
            if (_instance != null)
            {
                this.FirstRun = _instance.FirstRun;
                this.PreviousVersion = _instance.PreviousVersion;
                this.PreviousOS = _instance.PreviousOS;
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

        public void Update()
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
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
            if (this._HotKeys == null)
                return;

            // let's collapse duplicate hotkeys
            var toKeep = new List<Hotkey>();

            foreach (Hotkey current in this._defaultHotKeys)
            {
                Hotkey keep = current;

                foreach (Hotkey compare in this._HotKeys)
                {
                    if (current.Action == compare.Action && compare.Enabled)
                        keep = compare;
                }

                toKeep.Add(keep);
            }

            // deactivate all of the old hotkeys, especially duplicate ones that are active
            foreach (Hotkey hotkey in this._HotKeys)
            {
                if (!toKeep.Contains(hotkey))
                    hotkey.Deactivate();
            }

            this._HotKeys = toKeep;

            this.Save();
        }

        /// <summary>
        /// Called when loading a settings file to iterate through new dynamic properties (such as Hotkeys)
        /// which may have changed and would otherwise be hidden from the user
        /// </summary>
        private void CheckForNewSettings()
        {
            foreach (Hotkey defaultHotkey in this._defaultHotKeys)
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
                {
                    hotkey.Activate();
                }
            }
        }

        private bool IsSetToLaunchOnStartup()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(REG_KEY_STARTUP, false);

            return key?.GetValue("Toastify", null) != null;
        }

        private void SetLaunchOnStartup(bool enabled)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(REG_KEY_STARTUP, true);

            if (enabled)
                key?.SetValue("Toastify", "\"" + System.Windows.Forms.Application.ExecutablePath + "\"");
            else
                key?.DeleteValue("Toastify", false);

            key?.Close();
        }

        /// <summary>
        /// Any active settings (such as hotkeys) should be unloaded here
        /// </summary>
        private void UnloadSettings()
        {
            if (this.HotKeys != null)
            {
                foreach (Hotkey hotkey in this.HotKeys)
                {
                    hotkey.Deactivate();
                }
            }
        }

        public SettingsXml Clone()
        {
            SettingsXml clone = this.MemberwiseClone() as SettingsXml;

            if (clone != null)
            {
                clone.HotKeys = new List<Hotkey>();

                foreach (Hotkey key in this.HotKeys)
                {
                    clone.HotKeys.Add(key.Clone());
                }
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