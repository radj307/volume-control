using Newtonsoft.Json;
using Semver;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Helpers;
using VolumeControl.Core.Input;
using VolumeControl.Log;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Core
{
    /// <summary>
    /// Contains the application configuration and logic to read from and write to JSON files.
    /// </summary>
    [JsonObject]
    public class Config : AppConfig.ConfigurationFile
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="Config"/> instance.
        /// </summary>
        /// <remarks>The first time this is called, the <see cref="AppConfig.Configuration.Default"/> property is set to that instance; all subsequent calls do not update this property.</remarks>
        public Config(string filePath) : base(filePath)
        {
            // Forward PropertyChanged events for all properties that implement INotifyPropertyChanged
            foreach (var propertyInfo in typeof(Config).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly))
            {
                var property = propertyInfo.GetValue(this);
                if (property is INotifyPropertyChanged propertyWithPropertyChangedEvents)
                { // property implements INotifyPropertyChanged
                    propertyWithPropertyChangedEvents.PropertyChanged += this.PropertyWithPropertyChangedEvents_PropertyChanged;
                }
                else if (property is INotifyCollectionChanged propertyWithCollectionChangedEvents)
                {
                    propertyWithCollectionChangedEvents.CollectionChanged += this.PropertyWithCollectionChangedEvents_CollectionChanged;
                }
            }
        }
        #endregion Constructor

        #region Fields
        private static bool _autoSaveEnabled;
        #endregion Fields

        #region Methods
        /// <summary>
        /// Resumes automatic saving of the config to disk whenever a <see cref="PropertyChanged"/> event is triggered.
        /// </summary>
        public void ResumeAutoSave()
        {
            _autoSaveEnabled = true;
            PropertyChanged += this.HandlePropertyChanged;
        }
        /// <summary>
        /// Pauses automatic saving of the config to disk whenever a <see cref="PropertyChanged"/> event is triggered.
        /// </summary>
        public void PauseAutoSave()
        {
            _autoSaveEnabled = false;
            PropertyChanged -= this.HandlePropertyChanged;
        }
        #endregion Methods

        #region EventHandlers
        private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Log.Debug($"Config property '{e.PropertyName}' was modified.");
            this.Save();
        }
        private void PropertyWithPropertyChangedEvents_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!_autoSaveEnabled) return;
            
            this.Save();
        }
        private void PropertyWithCollectionChangedEvents_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_autoSaveEnabled) return;

            this.Save();
        }
        #endregion EventHandlers

        #region Statics
        private static LogWriter Log => FLog.Log;
        #endregion Statics

        #region Main
        /// <summary>
        /// Gets or sets the name of the current localization language.
        /// </summary>
        /// <remarks><b>Default: "English (US/CA)"</b></remarks>
        public string LanguageName { get; set; } = "English (US/CA)";
        /// <summary>
        /// Gets or sets whether the program should create the default translation files if they don't already exist.
        /// </summary>
        /// <remarks><b>Default: <see langword="true"/></b></remarks>
        public bool CreateDefaultTranslationFiles { get; set; } = true;
        /// <summary>
        /// Gets or sets a list of additional directories to load localization packages from.
        /// </summary>
        /// <remarks><b>Default: {}</b></remarks>
        public ObservableImmutableList<string> CustomLocalizationDirectories { get; set; } = new();
        /// <summary>
        /// Gets or sets whether multiple distinct instances of Volume Control are allowed to run concurrently.<br/>
        /// In this case, <i>distinct</i> means that each instance is using a different config file.<br/><br/>
        /// This works by creating a MD5 hash of <see cref="AppConfig.ConfigurationFile.Location"/>, and appending it to the instance's mutex ID, seperated with a colon (:).<br/>
        /// The initialization process of acquiring a mutex lock is still performed, which will always prevent Volume Control from running alongside other instances using the same config as itself, regardless of this setting.
        /// </summary>
        /// <remarks><b>Default: <see langword="false"/></b></remarks>
        public bool AllowMultipleDistinctInstances { get; set; } = false;
        /// <summary>
        /// Gets or sets whether the application should run when Windows starts.<br/>
        /// Creates/deletes a registry value in <b>HKEY_CURRENT_USER => SOFTWARE\Microsoft\Windows\CurrentVersion\Run</b>
        /// </summary>
        /// <remarks><b>Default: <see langword="false"/></b></remarks>
        public bool RunAtStartup { get; set; } = false;
        /// <summary>
        /// Gets or sets whether the window should be minimized during startup.<br/>
        /// The window can be shown again later using the tray icon.
        /// </summary>
        /// <remarks><b>Default: <see langword="false"/></b></remarks>
        public bool StartMinimized { get; set; } = false;
        /// <summary>
        /// Gets or sets whether the taskbar icon is visible when the window isn't minimized.
        /// </summary>
        /// <remarks><b>Default: <see langword="true"/></b></remarks>
        public bool ShowInTaskbar { get; set; } = true;
        /// <summary>
        /// Gets or sets whether volume control should always appear on top of other windows when the window isn't minimized.
        /// </summary>
        /// <remarks><b>Default: <see langword="false"/></b></remarks>
        public bool AlwaysOnTop { get; set; } = false;
        /// <summary>
        /// Gets or sets whether confirmation is required to delete a hotkey.
        /// </summary>
        public bool DeleteHotkeyConfirmation { get; set; } = true;
        /// <summary>
        /// Gets or sets whether or not device/session icons are shown in the UI.
        /// </summary>
        /// <remarks><b>Default: <see langword="true"/></b></remarks>
        public bool ShowIcons { get; set; } = true;
        /// <summary>
        /// List of directories that should be checked for addons in addition to the default one.
        /// </summary>
        /// <remarks><b>Default: {}</b></remarks>
        public ObservableImmutableList<string> CustomAddonDirectories { get; set; } = new();
        /// <summary>
        /// Gets or sets whether the main window allows transparency effects.<br/>
        /// <list type="bullet">
        /// <item>When <see langword="true"/>, window background transparency is allowed.</item>
        /// <item>When <see langword="false"/>, window background transparency isn't allowed;<br/>only controls <i>within</i> the window may use transparent background effects.<br/>(See <see cref="System.Windows.Controls.Control.Background"/>)</item>
        /// </list>
        /// </summary>
        /// <remarks><b>Default: <see langword="true"/></b></remarks>
        public bool AllowTransparency { get; set; } = true;
        /// <summary>
        /// Gets or sets whether the main window keeps its position relative to the nearest screen corner when its size changes.
        /// </summary>
        /// <returns><see langword="true"/> when the main window </returns>
        public bool KeepRelativePosition { get; set; } = false;
        #endregion Main

        #region Notifications
        /// <summary>
        /// The brush to use for the background of the list notification when locked.
        /// </summary>
        [JsonIgnore]
        public static readonly Brush NotificationLockedBrush = new LinearGradientBrush(new GradientStopCollection()
        {
            new GradientStop(Color.FromRgb(0xA3, 0x28, 0x28), 0.2),
            new GradientStop(Color.FromRgb(0xA8, 0x14, 0x14), 0.8)
        }, new Point(0, 0), new Point(1, 1));
        /// <summary>
        /// The brush to use for the background of the list notification when unlocked.
        /// </summary>
        [JsonIgnore]
        public static readonly Brush NotificationUnlockedBrush = new LinearGradientBrush(new GradientStopCollection()
        {
            new GradientStop(Color.FromRgb(0x49, 0x6D, 0x49), 0.2),
            new GradientStop(Color.FromRgb(0x40, 0x70, 0x40), 0.8)
        }, new Point(0, 0), new Point(1, 1));
        /// <summary>
        /// The default background brush used for the ListNotification
        /// </summary>
        [JsonIgnore]
        public static readonly Brush NotificationDefaultBrush = new SolidColorBrush(Color.FromRgb(0x30, 0x30, 0x30));
        /// <summary>
        /// Gets or sets the configuration for the SessionListNotification window.
        /// </summary>
        public NotificationConfigSection SessionListNotificationConfig { get; set; } = new();
        /// <summary>
        /// Gets or sets the configuration for the DeviceListNotification window.
        /// </summary>
        public NotificationConfigSection DeviceListNotificationConfig { get; set; } = new();
        /// <summary>
        /// Gets or sets whether the notification can be dragged without holding down the ALT key.
        /// </summary>
        public bool NotificationMoveRequiresAlt { get; set; } = false;
        /// <summary>
        /// Gets or sets whether the notification window position is restored when the program starts.
        /// </summary>
        public bool NotificationSavePos { get; set; } = true;
        /// <summary>
        /// Gets or sets whether extra mouse controls are enabled in the notification window.<br/>
        /// Extra mouse controls include Right-Click to deselect &amp; Middle-Click to toggle selection lock.
        /// </summary>
        public bool NotificationExtraMouseControlsEnabled { get; set; } = true;
        #endregion Notifications

        #region Updates
        /// <summary>
        /// Gets or sets whether the program should check for updates on startup.
        /// </summary>
        public bool CheckForUpdates { get; set; } = true;
        /// <summary>
        /// Gets or sets whether a message box prompt is shown informing the user of an available update.
        /// </summary>
        /// <remarks>When this is disabled, updates are notified through the caption bar.</remarks>
        public bool ShowUpdatePrompt { get; set; } = true;
        #endregion Updates

        #region Hotkeys
        /// <summary>
        /// The current hotkey configuration, or <see langword="null"/> if the default configuration should be used instead.
        /// </summary>
        public BindableHotkeyJsonWrapper[] Hotkeys { get; set; } = null!;
        /// <summary>
        /// Default hotkey configuration.
        /// </summary>
        [JsonIgnore]
        public static readonly BindableHotkeyJsonWrapper[] Hotkeys_Default = new BindableHotkeyJsonWrapper[]
        {
            new ()
            {
                Name = "Volume Up",
                Key = EFriendlyKey.VolumeUp,
                Modifier = Modifier.None,
                ActionIdentifier = "Session:Volume Up",
                Registered = true,
            },
            new()
            {
                Name = "Volume Down",
                Key = EFriendlyKey.VolumeDown,
                Modifier = Modifier.None,
                ActionIdentifier = "Session:Volume Down",
                Registered = true,
            },
            new()
            {
                Name = "Toggle Mute",
                Key = EFriendlyKey.VolumeMute,
                Modifier = Modifier.None,
                ActionIdentifier = "Session:Toggle Mute",
                Registered = true,
            },
            new()
            {
                Name = "Next Session",
                Key = EFriendlyKey.E,
                Modifier = Modifier.Alt | Modifier.Shift | Modifier.Ctrl,
                ActionIdentifier = "Session:Select Next",
                Registered = true,
            },
            new()
            {
                Name = "Previous Session",
                Key = EFriendlyKey.Q,
                Modifier = Modifier.Alt | Modifier.Shift | Modifier.Ctrl,
                ActionIdentifier = "Session:Select Previous",
                Registered = true,
            },
            new()
            {
                Name = "Un/Lock Session",
                Key = EFriendlyKey.S,
                Modifier = Modifier.Alt | Modifier.Shift | Modifier.Ctrl,
                ActionIdentifier = "Session:Toggle Lock",
                Registered = true,
            }
        };
        #endregion Hotkeys

        #region Audio
        /// <summary>
        /// Gets or sets the last target session.
        /// </summary>
        public TargetInfo TargetSession { get; set; } = TargetInfo.Empty;
        /// <summary>
        /// Gets or sets whether the target session is locked.
        /// </summary>
        public bool LockTargetSession { get; set; } = false;
        /// <summary>
        /// Gets or sets the target device ID.
        /// </summary>
        public string TargetDeviceID { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets whether the target device is locked.
        /// </summary>
        public bool LockTargetDevice { get; set; } = false;
        /// <summary>
        /// Gets or sets the amount of volume added or subtracted from the current volume level when volume change hotkeys are pressed.
        /// </summary>
        public int VolumeStepSize { get; set; } = 2;
        /// <summary>
        /// Gets or sets the interval (in milliseconds) between updating the audio peak meters.
        /// </summary>
        public int PeakMeterUpdateIntervalMs { get; set; } = 100;
        /// <summary>
        /// Gets or sets whether or not peak meters are shown in the mixer.
        /// </summary>
        public bool ShowPeakMeters { get; set; } = true;
        /// <summary>
        /// The minimum boundary shown on peak meters.
        /// </summary>
        public const double PeakMeterMinValue = 0.0;
        /// <summary>
        /// The maximum boundary shown on peak meters.
        /// </summary>
        public const double PeakMeterMaxValue = 1.0;
        /// <summary>
        /// Gets or sets whether volume &amp; mute controls are visible in the Audio Devices list.
        /// </summary>
        public bool EnableDeviceControl { get; set; } = false;
        /// <summary>
        /// Gets or sets the list of (process names of) hidden audio sessions.
        /// </summary>
        public ObservableImmutableList<string> HiddenSessionProcessNames { get; set; } = new() { };
        #endregion Audio

        #region Log
        /// <summary>
        /// Gets or sets whether the log is enabled or not.<br/>
        /// See <see cref="Log.SettingsInterface.EnableLogging"/>
        /// </summary>
        /// <remarks><b>Default: <see langword="true"/></b></remarks>
        public bool EnableLogging { get; set; } = true;
        /// <summary>
        /// Gets or sets the location of the log file.<br/>
        /// See <see cref="Log.SettingsInterface.LogPath"/>
        /// </summary>
        /// <remarks><b>Default: "VolumeControl.log"</b></remarks>
        public string LogPath { get; set; } = "VolumeControl.log";
        /// <summary>
        /// Gets or sets the <see cref="Log.Enum.EventType"/> filter used for messages.<br/>
        /// See <see cref="Log.SettingsInterface.LogFilter"/>
        /// </summary>
        /// <remarks><b>Default: <see cref="Log.Enum.EventType.ALL_EXCEPT_DEBUG"/></b></remarks>
        public VolumeControl.Log.Enum.EventType LogFilter { get; set; } = VolumeControl.Log.Enum.EventType.ALL_EXCEPT_DEBUG;
        /// <summary>
        /// Gets or sets whether the log is cleared when the program starts.<br/>
        /// See <see cref="Log.SettingsInterface.LogClearOnInitialize"/>
        /// </summary>
        /// <remarks><b>Default: <see langword="true"/></b></remarks>
        public bool LogClearOnInitialize { get; set; } = true;
        /// <summary>
        /// Gets or sets the format string used for timestamps in the log.<br/>
        /// See <see cref="Log.SettingsInterface.LogTimestampFormat"/>
        /// </summary>
        /// <remarks><b>Default: "HH:mm:ss:fff"</b></remarks>
        public string LogTimestampFormat { get; set; } = "HH:mm:ss:fff";
        /// <summary>
        /// 
        /// See <see cref="Log.SettingsInterface.LogEnableStackTrace"/>
        /// </summary>
        /// <remarks><b>Default: <see langword="true"/></b></remarks>
        public bool LogEnableStackTrace { get; set; } = true;
        /// <summary>
        /// 
        /// See <see cref="Log.SettingsInterface.LogEnableStackTraceLineCount"/>
        /// </summary>
        /// <remarks><b>Default: <see langword="true"/></b></remarks>
        public bool LogEnableStackTraceLineCount { get; set; } = true;
        #endregion Log

        #region Misc
        /// <summary>
        /// Gets or sets the version number identifier of the last Volume Control instance to access this config file.
        /// </summary>
        /// <remarks><b>Default: 0.0.0</b></remarks>
        public SemVersion __VERSION__ { get; set; } = new(0);
        #endregion Misc
    }
}
