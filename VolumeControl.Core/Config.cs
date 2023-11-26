using Newtonsoft.Json;
using Semver;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Input.Enums;
using VolumeControl.Core.Input.Json;
using VolumeControl.Core.Structs;
using VolumeControl.Log;
using VolumeControl.WPF.Collections;
using VolumeControl.WPF.Extensions;

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
        /// <remarks>The first time this is called, the <see cref="AppConfig.Configuration.DefaultInstance"/> property is set to that instance; all subsequent calls do not update this property.</remarks>
        public Config(string filePath) : base(filePath)
        {
            PropertyChanged += UpdateFLogState;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// The default <see cref="Config"/> instance.
        /// </summary>
        [JsonIgnore]
        public static Config Default => (Config)DefaultInstance;
        #endregion Properties

        #region Fields
        private static bool _autoSaveEnabled;
        #endregion Fields

        #region Methods
        /// <summary>
        /// Resumes automatic saving of the config to disk whenever a <see cref="PropertyChanged"/> event is triggered.
        /// </summary>
        public void ResumeAutoSave()
        {
            if (FLog.FilterEventType(EventType.TRACE))
                FLog.Trace($"Enabled {nameof(Config)} autosave.");

            _autoSaveEnabled = true;
            PropertyChanged += this.HandlePropertyChanged;
        }
        /// <summary>
        /// Pauses automatic saving of the config to disk whenever a <see cref="PropertyChanged"/> event is triggered.
        /// </summary>
        public void PauseAutoSave()
        {
            if (FLog.FilterEventType(EventType.TRACE))
                FLog.Trace($"Disabled {nameof(Config)} autosave.");

            _autoSaveEnabled = false;
            PropertyChanged -= this.HandlePropertyChanged;
        }
        /// <summary>
        /// Attaches autosave event handlers to properties that have notifications
        /// </summary>
        public void AttachReflectivePropertyChangedHandlers()
        {
            // Forward PropertyChanged events for all properties that implement INotifyPropertyChanged
            foreach (var propertyInfo in GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly))
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
        #endregion Methods

        #region Method Overrides
        /// <inheritdoc/>
        public void Save(Formatting formatting = Formatting.Indented, [CallerMemberName] string callerName = "")
        {
            if (FLog.FilterEventType(EventType.TRACE))
                FLog.Trace($"Saved {nameof(Config)} (Caller: \"{callerName}\")");
            base.Save(formatting);
        }
        public override bool Load()
        {
            if (FLog.IsInitialized && FLog.FilterEventType(EventType.TRACE))
                FLog.Trace($"Loaded {nameof(Config)} from {Location}.");
            return base.Load();
        }
        #endregion Method Overrides

        #region EventHandlers
        private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (FLog.FilterEventType(EventType.TRACE))
                FLog.Trace($"Config property '{e.PropertyName}' was modified.");
            Save();
        }
        private void UpdateFLogState(object? sender, PropertyChangedEventArgs e)
        {
            if (!FLog.IsInitialized) return;

            // update the log's properties
            if (e.PropertyName != null)
            {
                if (e.PropertyName.Equals(nameof(EnableLogging), StringComparison.Ordinal))
                {
                    FLog.Log.EndpointEnabled = EnableLogging;
                }
                else if (e.PropertyName.Equals(nameof(LogFilter), StringComparison.Ordinal))
                {
                    FLog.Log.EventTypeFilter = LogFilter;
                }
                else if (e.PropertyName.Equals(nameof(LogPath), StringComparison.Ordinal))
                {
                    FLog.ChangeLogPath(LogPath);
                }
            }
        }
        private void PropertyWithPropertyChangedEvents_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!_autoSaveEnabled) return;

            if (FLog.FilterEventType(EventType.TRACE))
                FLog.Trace($"Config sub-property \"{e.PropertyName}\" was modified.");

            Save();
        }
        private void PropertyWithCollectionChangedEvents_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_autoSaveEnabled) return;

            Save();
        }
        #endregion EventHandlers

        #region Main
        /// <summary>
        /// Gets or sets the name of the current localization language.
        /// </summary>
        /// <remarks><b>Default: "English (US/CA)"</b></remarks>
        public string LanguageName { get; set; } = "English (US/CA)";
        /// <summary>
        /// Gets or sets whether missing translation keys cause a log message to be written.
        /// </summary>
        public bool LogMissingTranslations { get; set; } = true;
        /// <summary>
        /// Gets or sets a list of directories to load localization packages from.
        /// </summary>
        /// <remarks><b>Default: {}</b></remarks>
        public ObservableImmutableList<string> LocalizationDirectories { get; set; } = new();
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
        /// <summary>
        /// Gets or sets whether the main window should have its last known position restored when the application starts.
        /// </summary>
        public bool RestoreMainWindowPosition { get; set; } = false;
        /// <summary>
        /// Gets or sets the origin corner to use when restoring the main window's position.
        /// </summary>
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public EScreenCorner MainWindowOriginCorner { get; set; } = EScreenCorner.TopLeft;
        /// <summary>
        /// Gets or sets the position to restore the main window to when the application starts.
        /// </summary>
        public Point? MainWindowPosition { get; set; } = null;
        #endregion Main

        #region Notifications
        /// <summary>
        /// Gets or sets the configuration for the SessionListNotification window.
        /// </summary>
        public NotificationConfigSection SessionListNotificationConfig { get; set; } = new();
        /// <summary>
        /// Gets or sets the configuration for the DeviceListNotification window.
        /// </summary>
        public NotificationConfigSection DeviceListNotificationConfig { get; set; } = new()
        {
            LockedColor = Color.FromRgb(0xA7, 0x33, 0x74),
            UnlockedColor = Color.FromRgb(0x49, 0x80, 0x49),
        };
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
        public JsonHotkey[] Hotkeys { get; set; } = null!;
        /// <summary>
        /// Default hotkey configuration.
        /// </summary>
        [JsonIgnore]
        public static readonly JsonHotkey[] Hotkeys_Default = new JsonHotkey[]
        { // don't rename hotkeys without also renaming the localization paths. See HotkeyManagerVM.GetTranslatedDefaultHotkeys for more info.
            new ()
            {
                Name = "Volume Up",
                Key = EFriendlyKey.VolumeUp,
                Modifiers = EModifierKey.None,
                ActionIdentifier = "Session:Volume Up",
                IsRegistered = true,
            },
            new()
            {
                Name = "Volume Down",
                Key = EFriendlyKey.VolumeDown,
                Modifiers = EModifierKey.None,
                ActionIdentifier = "Session:Volume Down",
                IsRegistered = true,
            },
            new()
            {
                Name = "Toggle Mute",
                Key = EFriendlyKey.VolumeMute,
                Modifiers = EModifierKey.None,
                ActionIdentifier = "Session:Toggle Mute",
                IsRegistered = true,
            },
            new()
            {
                Name = "Next Session",
                Key = EFriendlyKey.E,
                Modifiers = EModifierKey.Alt | EModifierKey.Shift | EModifierKey.Ctrl,
                ActionIdentifier = "Session:Select Next",
                IsRegistered = true,
            },
            new()
            {
                Name = "Previous Session",
                Key = EFriendlyKey.Q,
                Modifiers = EModifierKey.Alt | EModifierKey.Shift | EModifierKey.Ctrl,
                ActionIdentifier = "Session:Select Previous",
                IsRegistered = true,
            },
            new()
            {
                Name = "Un/Lock Session",
                Key = EFriendlyKey.S,
                Modifiers = EModifierKey.Alt | EModifierKey.Shift | EModifierKey.Ctrl,
                ActionIdentifier = "Session:Toggle Lock",
                IsRegistered = true,
            },
            new()
            {
                Name = "Toggle Selected",
                Key = EFriendlyKey.W,
                Modifiers = EModifierKey.Alt | EModifierKey.Shift | EModifierKey.Ctrl,
                ActionIdentifier = "Session:Toggle Selected",
                IsRegistered = true,
            }
        };
        #endregion Hotkeys

        #region Audio
        /// <summary>
        /// Gets or sets the last target session.
        /// </summary>
        public TargetInfo TargetSession { get; set; } = TargetInfo.Empty;
        /// <summary>
        /// Gets or sets the list of selected sessions.
        /// </summary>
        public ObservableImmutableList<TargetInfo> SelectedSessions { get; set; } = new();
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
        /// Gets or sets whether volume &amp; mute controls are visible in the Audio Devices list.
        /// </summary>
        public bool EnableDeviceControl { get; set; } = false;
        /// <summary>
        /// Gets or sets the list of (process names of) hidden audio sessions.
        /// </summary>
        public ObservableImmutableList<string> HiddenSessionProcessNames { get; set; } = new();
        /// <summary>
        /// Gets or sets a value that indicates whether input devices are loaded by the AudioDeviceManager.
        /// </summary>
        public bool EnableInputDevices { get; set; } = false;
        #endregion Audio

        #region Log
        /// <summary>
        /// Gets or sets whether the log is enabled or not.
        /// </summary>
        /// <remarks><b>Default: <see langword="true"/></b></remarks>
        public bool EnableLogging { get; set; } = true;
        /// <summary>
        /// Gets or sets the location of the log file.
        /// </summary>
        /// <remarks><b>Default: "VolumeControl.log"</b></remarks>
        public string LogPath { get; set; } = DefaultLogPath;
        /// <summary>
        /// The default log file path.
        /// </summary>
        public const string DefaultLogPath = "VolumeControl.log";
        /// <summary>
        /// Gets or sets the <see cref="Log.EventType"/> filter used for messages.
        /// </summary>
        public EventType LogFilter { get; set; } = EventType.INFO | EventType.WARN | EventType.ERROR | EventType.FATAL | EventType.CRITICAL;
        /// <summary>
        /// Gets or sets whether the log is cleared when the program starts.
        /// </summary>
        /// <remarks><b>Default: <see langword="true"/></b></remarks>
        public bool LogClearOnInitialize { get; set; } = true;
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
