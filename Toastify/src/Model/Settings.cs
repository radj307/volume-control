using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Toastify.Common;
using Toastify.Core;
using Toastify.Helpers;

namespace Toastify.Model
{
    [Serializable]
    [XmlRoot("SettingsXml")]
    public class Settings : ObservableObject
    {
        #region Singleton

        private static Settings _instance;

        public static Settings Instance
        {
            get
            {
                return _instance ?? (_instance = new Settings());
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
        private const string SETTINGS_FILENAME = "Toastify.xml";

        private XmlSerializer _xmlSerializer;
        private string _settingsFile;

        [XmlIgnore]
        public XmlSerializer XmlSerializer
        {
            get
            {
                if (this._xmlSerializer == null)
                {
                    this._xmlSerializer = new XmlSerializer(typeof(Settings));
                    this._xmlSerializer.UnknownAttribute += this.XmlSerializer_UnknownAttribute;
                    this._xmlSerializer.UnknownElement += this.XmlSerializer_UnknownElement;
                    this._xmlSerializer.UnknownNode += this.XmlSerializer_UnknownNode;
                }
                return this._xmlSerializer;
            }
        }

        /// <summary>
        /// Returns the location of the settings file
        /// </summary>
        [XmlIgnore]
        public string SettingsFilePath
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
                            Debug.WriteLine("Exception creating user settings directory (" + settingsPath + "). Exception: " + ex);

                            // No messagebox as this should not happen (and there will be a MessageBox later on when
                            // settings fail to load)
                        }
                    }

                    this._settingsFile = Path.Combine(settingsPath, SETTINGS_FILENAME);
                }

                return this._settingsFile;
            }
        }

        #region Settings

        #region Private fields

        private bool _minimizeSpotifyOnStartup;
        private bool _closeSpotifyWithToastify;
        private ToastifyVolumeControlMode _volumeControlMode;
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
        private bool _disableToastWithFullscreenVideogames;
        private bool _showSongProgressBar;
        private int _fadeOutTime;
        private ToastTitlesOrder _toastTitlesOrder;
        private string _toastColorTop;
        private string _toastColorBottom;
        private double _toastColorTopOffset;
        private double _toastColorBottomOffset;
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
        private string _toastTitle1Color;
        private string _toastTitle2Color;
        private double _toastTitle1FontSize;
        private double _toastTitle2FontSize;
        private bool _toastTitle1DropShadow;
        private double _toastTitle1ShadowDepth;
        private double _toastTitle1ShadowBlur;
        private bool _toastTitle2DropShadow;
        private double _toastTitle2ShadowDepth;
        private double _toastTitle2ShadowBlur;
        private string _songProgressBarBackgroundColor;
        private string _songProgressBarForegroundColor;

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

                this.NotifyPropertyChanged();
            }
        }

        public bool MinimizeSpotifyOnStartup
        {
            get { return this._minimizeSpotifyOnStartup; }
            set { this.RaiseAndSetIfChanged(ref this._minimizeSpotifyOnStartup, value); }
        }

        public bool CloseSpotifyWithToastify
        {
            get { return this._closeSpotifyWithToastify; }
            set { this.RaiseAndSetIfChanged(ref this._closeSpotifyWithToastify, value); }
        }

        public ToastifyVolumeControlMode VolumeControlMode
        {
            get { return this._volumeControlMode; }
            set { this.RaiseAndSetIfChanged(ref this._volumeControlMode, value); }
        }

        [Obsolete("UseSpotifyVolumeControl is obsolete and will be removed in the future. Use VolumeControlMode instead.")]
        public bool UseSpotifyVolumeControl
        {
            get { return this._useSpotifyVolumeControl; }
            set { this.RaiseAndSetIfChanged(ref this._useSpotifyVolumeControl, value); }
        }

        public float WindowsVolumeMixerIncrement
        {
            get { return this._windowsVolumeMixerIncrement; }
            set { this.RaiseAndSetIfChanged(ref this._windowsVolumeMixerIncrement, value); }
        }

        public string ClipboardTemplate
        {
            get { return this._clipboardTemplate; }
            set { this.RaiseAndSetIfChanged(ref this._clipboardTemplate, value); }
        }

        public bool SaveTrackToFile
        {
            get { return this._saveTrackToFile; }
            set { this.RaiseAndSetIfChanged(ref this._saveTrackToFile, value); }
        }

        public string SaveTrackToFilePath
        {
            get { return this._saveTrackToFilePath; }
            set { this.RaiseAndSetIfChanged(ref this._saveTrackToFilePath, value); }
        }

        public bool PreventSleepWhilePlaying
        {
            get { return this._preventSleepWhilePlaying; }
            set { this.RaiseAndSetIfChanged(ref this._preventSleepWhilePlaying, value); }
        }

        public bool OptInToAnalytics
        {
            get { return this._optInToAnalytics; }
            set { this.RaiseAndSetIfChanged(ref this._optInToAnalytics, value); }
        }

        #endregion [General]

        #region [Hotkeys]

        public bool GlobalHotKeys
        {
            get { return this._globalHotKeys; }
            set { this.RaiseAndSetIfChanged(ref this._globalHotKeys, value); }
        }

        public List<Hotkey> HotKeys
        {
            get { return this._hotKeys; }
            set { this.RaiseAndSetIfChanged(ref this._hotKeys, value); }
        }

        #endregion [Hotkeys]

        #region [Toast]

        public bool DisableToast
        {
            get { return this._disableToast; }
            set { this.RaiseAndSetIfChanged(ref this._disableToast, value); }
        }

        /// <summary>
        /// Only show the toast when the `<see cref="SpotifyAction.ShowToast"/>` hotkey is pressed.
        /// </summary>
        public bool OnlyShowToastOnHotkey
        {
            get { return this._onlyShowToastOnHotkey; }
            set { this.RaiseAndSetIfChanged(ref this._onlyShowToastOnHotkey, value); }
        }

        public bool DisableToastWithFullscreenVideogames
        {
            get { return this._disableToastWithFullscreenVideogames; }
            set { this.RaiseAndSetIfChanged(ref this._disableToastWithFullscreenVideogames, value); }
        }

        public bool ShowSongProgressBar
        {
            get { return this._showSongProgressBar; }
            set { this.RaiseAndSetIfChanged(ref this._showSongProgressBar, value); }
        }

        public int FadeOutTime
        {
            get { return this._fadeOutTime; }
            set { this.RaiseAndSetIfChanged(ref this._fadeOutTime, value); }
        }

        public ToastTitlesOrder ToastTitlesOrder
        {
            get { return this._toastTitlesOrder; }
            set { this.RaiseAndSetIfChanged(ref this._toastTitlesOrder, value); }
        }

        public string ToastColorTop
        {
            get { return this._toastColorTop; }
            set { this.RaiseAndSetIfChanged(ref this._toastColorTop, value); }
        }

        public string ToastColorBottom
        {
            get { return this._toastColorBottom; }
            set { this.RaiseAndSetIfChanged(ref this._toastColorBottom, value); }
        }

        public double ToastColorTopOffset
        {
            get { return this._toastColorTopOffset; }
            set { this.RaiseAndSetIfChanged(ref this._toastColorTopOffset, value); }
        }

        public double ToastColorBottomOffset
        {
            get { return this._toastColorBottomOffset; }
            set { this.RaiseAndSetIfChanged(ref this._toastColorBottomOffset, value); }
        }

        public string ToastBorderColor
        {
            get { return this._toastBorderColor; }
            set { this.RaiseAndSetIfChanged(ref this._toastBorderColor, value); }
        }

        public double ToastBorderThickness
        {
            get { return this._toastBorderThickness; }
            set { this.RaiseAndSetIfChangedWithConstraint(ref this._toastBorderThickness, value, () => value >= 0.0); }
        }

        public double ToastBorderCornerRadiusTopLeft
        {
            get { return this._toastBorderCornerRadiusTopLeft; }
            set { this.RaiseAndSetIfChangedWithConstraint(ref this._toastBorderCornerRadiusTopLeft, value, () => value >= 0.0); }
        }

        public double ToastBorderCornerRadiusTopRight
        {
            get { return this._toastBorderCornerRadiusTopRight; }
            set { this.RaiseAndSetIfChangedWithConstraint(ref this._toastBorderCornerRadiusTopRight, value, () => value >= 0.0); }
        }

        public double ToastBorderCornerRadiusBottomLeft
        {
            get { return this._toastBorderCornerRadiusBottomLeft; }
            set { this.RaiseAndSetIfChangedWithConstraint(ref this._toastBorderCornerRadiusBottomLeft, value, () => value >= 0.0); }
        }

        public double ToastBorderCornerRadiusBottomRight
        {
            get { return this._toastBorderCornerRadiusBottomRight; }
            set { this.RaiseAndSetIfChangedWithConstraint(ref this._toastBorderCornerRadiusBottomRight, value, () => value >= 0.0); }
        }

        public double ToastWidth
        {
            get { return this._toastWidth; }
            set { this.RaiseAndSetIfChangedWithConstraint(ref this._toastWidth, value, () => value >= 0.0); }
        }

        public double ToastHeight
        {
            get { return this._toastHeight; }
            set { this.RaiseAndSetIfChangedWithConstraint(ref this._toastHeight, value, () => value >= 0.0); }
        }

        public double PositionLeft
        {
            get { return this._positionLeft; }
            set { this.RaiseAndSetIfChanged(ref this._positionLeft, value); }
        }

        public double PositionTop
        {
            get { return this._positionTop; }
            set { this.RaiseAndSetIfChanged(ref this._positionTop, value); }
        }

        public string ToastTitle1Color
        {
            get { return this._toastTitle1Color; }
            set { this.RaiseAndSetIfChanged(ref this._toastTitle1Color, value); }
        }

        public string ToastTitle2Color
        {
            get { return this._toastTitle2Color; }
            set { this.RaiseAndSetIfChanged(ref this._toastTitle2Color, value); }
        }

        public double ToastTitle1FontSize
        {
            get { return this._toastTitle1FontSize; }
            set { this.RaiseAndSetIfChanged(ref this._toastTitle1FontSize, value); }
        }

        public double ToastTitle2FontSize
        {
            get { return this._toastTitle2FontSize; }
            set { this.RaiseAndSetIfChanged(ref this._toastTitle2FontSize, value); }
        }

        public bool ToastTitle1DropShadow
        {
            get { return this._toastTitle1DropShadow; }
            set { this.RaiseAndSetIfChanged(ref this._toastTitle1DropShadow, value); }
        }

        public double ToastTitle1ShadowDepth
        {
            get { return this._toastTitle1ShadowDepth; }
            set { this.RaiseAndSetIfChanged(ref this._toastTitle1ShadowDepth, value); }
        }

        public double ToastTitle1ShadowBlur
        {
            get { return this._toastTitle1ShadowBlur; }
            set { this.RaiseAndSetIfChanged(ref this._toastTitle1ShadowBlur, value); }
        }

        public bool ToastTitle2DropShadow
        {
            get { return this._toastTitle2DropShadow; }
            set { this.RaiseAndSetIfChanged(ref this._toastTitle2DropShadow, value); }
        }

        public double ToastTitle2ShadowDepth
        {
            get { return this._toastTitle2ShadowDepth; }
            set { this.RaiseAndSetIfChanged(ref this._toastTitle2ShadowDepth, value); }
        }

        public double ToastTitle2ShadowBlur
        {
            get { return this._toastTitle2ShadowBlur; }
            set { this.RaiseAndSetIfChanged(ref this._toastTitle2ShadowBlur, value); }
        }

        public string SongProgressBarBackgroundColor
        {
            get { return this._songProgressBarBackgroundColor; }
            set { this.RaiseAndSetIfChanged(ref this._songProgressBarBackgroundColor, value); }
        }

        public string SongProgressBarForegroundColor
        {
            get { return this._songProgressBarForegroundColor; }
            set { this.RaiseAndSetIfChanged(ref this._songProgressBarForegroundColor, value); }
        }

        #endregion [Toast]

        #region (hidden)

        public bool FirstRun { get; set; }

        public string PreviousVersion { get; set; }

        public int StartupWaitTimeout { get; set; }

        public int SpotifyConnectionAttempts { get; set; }

        public WindowPosition SettingsWindowLastLocation { get; set; }

        public List<PluginDetails> Plugins { get; set; }

        #endregion (hidden)

        #endregion Settings

        private Settings()
        {
            this.Default();
        }

        public void Default(bool setHotKeys = false)
        {
            // [General]
            this.MinimizeSpotifyOnStartup = false;
            this.CloseSpotifyWithToastify = true;
            this.VolumeControlMode = ToastifyVolumeControlMode.SystemSpotifyOnly;
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
            this.DisableToastWithFullscreenVideogames = true;
            this.ShowSongProgressBar = true;
            this.FadeOutTime = 4000;
            this.ToastTitlesOrder = ToastTitlesOrder.TrackByArtist;

            this.ToastColorTop = "#FF000000";
            this.ToastColorBottom = "#FF000000";
            this.ToastColorTopOffset = 0.0;
            this.ToastColorBottomOffset = 1.0;
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

            this.ToastTitle1Color = "#FFFFFFFF";
            this.ToastTitle2Color = "#FFF0F0F0";
            this.ToastTitle1FontSize = 16;
            this.ToastTitle2FontSize = 12;
            this.ToastTitle1DropShadow = true;
            this.ToastTitle1ShadowDepth = 3;
            this.ToastTitle1ShadowBlur = 2;
            this.ToastTitle2DropShadow = false;
            this.ToastTitle2ShadowDepth = 2;
            this.ToastTitle2ShadowBlur = 2;

            this.SongProgressBarBackgroundColor = "FF333333";
            this.SongProgressBarForegroundColor = "FFA0A0A0";

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
            using (StreamWriter sw = new StreamWriter(this.SettingsFilePath, false))
            {
                this.XmlSerializer.Serialize(sw, this);
            }

            if (replaceCurrent)
                Instance = this;
        }

        public Settings Load()
        {
            if (!File.Exists(this.SettingsFilePath))
            {
                Instance.Default(true);
                Instance.Save();
            }
            else
            {
                using (StreamReader sr = new StreamReader(this.SettingsFilePath))
                {
                    Settings file = this.XmlSerializer.Deserialize(sr) as Settings;

                    file?.CheckForNewSettings();

                    Instance = file;
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

        public Settings Clone()
        {
            Settings clone = this.MemberwiseClone() as Settings;

            if (clone != null)
            {
                clone.HotKeys = new List<Hotkey>();

                foreach (Hotkey key in this.HotKeys)
                    clone.HotKeys.Add(key.Clone());
            }

            return clone;
        }

        #region XmlSerializer event handlers

        private void XmlSerializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            Debug.WriteLine($"XmlSerializer_UnknownAttribute: {e.Attr.LocalName} = \"{e.Attr.Value}\"");
        }

        private void XmlSerializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            switch (e.Element.LocalName)
            {
                case "UseSpotifyVolumeControl":
                    bool value;
                    bool.TryParse(e.Element.InnerText, out value);
                    this.VolumeControlMode = value ? ToastifyVolumeControlMode.Spotify : ToastifyVolumeControlMode.SystemSpotifyOnly;
                    break;
            }
        }

        private void XmlSerializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            if (e.NodeType == XmlNodeType.Attribute || e.NodeType == XmlNodeType.Element)
                return;

            Debug.WriteLine($"XmlSerializer_UnknownNode: {e.LocalName} = \"{e.Text}\"");
        }

        #endregion XmlSerializer event handlers
    }

    [Serializable]
    public class PluginDetails
    {
        public string FileName { get; set; }
        public string TypeName { get; set; }
        public string Settings { get; set; }
    }
}