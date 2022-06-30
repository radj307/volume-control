using Newtonsoft.Json;
using System.Windows.Input;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Keyboard;

namespace VolumeControl.Core
{
    /// <summary>
    /// Contains the application configuration and logic to read from and write to JSON files.
    /// </summary>
    [JsonObject]
    public class Config : AppConfig.ConfigurationFile
    {
        /// <summary>
        /// Creates a new <see cref="Config"/> instance.
        /// </summary>
        /// <remarks>The first time this is called, the <see cref="AppConfig.Configuration.Default"/> property is set to that instance; all subsequent calls do not update this property.</remarks>
        public Config() : base(_filePath) { }

        // Default filepath used for the config file:
        private const string _filePath = "VolumeControl.json";

        #region Main
        /// <summary>
        /// Gets or sets whether the application should run when Windows starts.<br/>
        /// Creates/deletes a registry value in <b>HKEY_CURRENT_USER => SOFTWARE\Microsoft\Windows\CurrentVersion\Run</b>
        /// </summary>
        public bool RunAtStartup { get; set; } = false;
        /// <summary>
        /// Gets or sets whether the window should be minimized during startup.<br/>
        /// The window can be shown again later using the tray icon.
        /// </summary>
        public bool StartMinimized { get; set; } = false;
        /// <summary>
        /// Gets or sets whether the taskbar icon is visible when the window isn't minimized.
        /// </summary>
        public bool ShowInTaskbar { get; set; } = true;
        /// <summary>
        /// Gets or sets whether volume control should always appear on top of other windows when the window isn't minimized.
        /// </summary>
        public bool AlwaysOnTop { get; set; } = false;
        /// <summary>
        /// Gets or sets the hotkey editor mode, which can be either false (basic mode) or true (advanced mode).
        /// </summary>
        /// <remarks>Advanced mode allows the user to perform additional actions in the hotkey editor:
        /// <list type="bullet">
        /// <item><description>Create and delete hotkeys.</description></item>
        /// <item><description>Change the action bindings of hotkeys.</description></item>
        /// <item><description>Rename hotkeys.</description></item>
        /// </list></remarks>
        public bool AdvancedHotkeys { get; set; } = false;
        /// <summary>
        /// Gets or sets whether or not device/session icons are shown in the UI.
        /// </summary>
        public bool ShowIcons { get; set; } = true;
        /// <summary>
        /// List of directories that should be checked for addons in addition to the default one.
        /// </summary>
        public List<string> CustomAddonDirectories { get; set; } = new();
        #endregion Main

        #region Notifications
        /// <summary>
        /// Gets or sets whether notifications are enabled or not.
        /// </summary>
        public bool NotificationsEnabled { get; set; } = true;
        /// <summary>
        /// Gets or sets whether or not the list notification window appears for volume change events.
        /// </summary>
        public bool NotificationsOnVolumeChange { get; set; } = true;
        /// <summary>
        /// The amount of time, in milliseconds, that the notification window stays visible for before disappearing.
        /// </summary>
        public int NotificationTimeoutMs { get; set; } = 3000;
        /// <summary>
        /// This partially controls what the notification window actually displays.
        /// </summary>
        public DisplayTarget NotificationMode { get; set; }
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
                Key = Key.VolumeUp,
                Modifier = Modifier.None,
                ActionIdentifier = "Session:Volume Up",
            },
            new()
            {
                Name = "Volume Down",
                Key = Key.VolumeDown,
                Modifier = Modifier.None,
                ActionIdentifier = "Session:Volume Down",
            },
            new()
            {
                Name = "Volume Up",
                Key = Key.VolumeMute,
                Modifier = Modifier.None,
                ActionIdentifier = "Session:Toggle Mute",
            },
            new()
            {
                Name = "Next Session",
                Key = Key.D,
                Modifier = Modifier.Alt | Modifier.Shift | Modifier.Ctrl,
                ActionIdentifier = "Session:Select Next",
            },
            new()
            {
                Name = "Previous Session",
                Key = Key.A,
                Modifier = Modifier.Alt | Modifier.Shift | Modifier.Ctrl,
                ActionIdentifier = "Session:Select Previous",
            },
            new()
            {
                Name = "Un/Lock Session",
                Key = Key.S,
                Modifier = Modifier.Alt | Modifier.Shift | Modifier.Ctrl,
                ActionIdentifier = "Session:Toggle Lock",
            }
        };
        #endregion Hotkeys

        #region Audio
        /// <summary>
        /// Gets or sets the last session target name.
        /// </summary>
        public string Target { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets whether the target session is locked.
        /// </summary>
        public bool LockTargetSession { get; set; } = false;
        /// <summary>
        /// Gets or sets the amount of volume added or subtracted from the current volume level when volume change hotkeys are pressed.
        /// </summary>
        public int VolumeStepSize { get; set; } = 2;
        /// <summary>
        /// List of enabled audio device IDs.
        /// </summary>
        public List<string> EnabledDevices { get; set; } = new() { string.Empty };
        #endregion Audio
    }
}
