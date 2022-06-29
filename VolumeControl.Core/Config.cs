using Newtonsoft.Json;
using System.Windows.Input;
using VolumeControl.Core.Keyboard;

namespace VolumeControl.Core
{
    /// <summary>
    /// Acts as a temporary wrapper for the <see cref="BindableHotkey"/> class so that the JSON parser can read and write it without attempting to register hotkeys before the API is initialized.
    /// </summary>
    [JsonObject]
    public class BindableHotkeyJsonWrapper
    {
        /// <summary>
        /// The hotkey's name
        /// </summary>
        [JsonProperty]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Whether the hotkey is registered or not.
        /// </summary>
        [JsonProperty]
        public bool Registered { get; set; } = false;
        /// <summary>
        /// The primary key
        /// </summary>
        [JsonProperty]
        public Key Key { get; set; } = Key.None;
        /// <summary>
        /// The modifier keys
        /// </summary>
        [JsonProperty]
        public Modifier Modifier { get; set; } = Modifier.None;
        /// <summary>
        /// The name of the action
        /// </summary>
        [JsonProperty]
        public string? ActionIdentifier { get; set; } = null;
    }
    /// <summary>
    /// Contains the application configuration and logic to read from and write to JSON files.
    /// </summary>
    [JsonObject]
    public class Config : AppConfig.ConfigurationFile
    {
        private const string _filePath = "VolumeControl.json";
        public Config() : base(_filePath) { }

        #region Main
        public bool RunAtStartup { get; set; } = false;
        public bool StartMinimized { get; set; } = false;
        public bool ShowInTaskbar { get; set; } = true;
        public bool AlwaysOnTop { get; set; } = false;
        public bool AdvancedHotkeys { get; set; } = false;
        public bool ShowIcons { get; set; } = true;
        public List<string> CustomAddonDirectories { get; set; } = new();
        #endregion Main

        #region Notifications
        public bool NotificationsEnabled { get; set; } = true;
        public bool NotificationsOnVolumeChange { get; set; } = true;
        public int NotificationTimeoutMs { get; set; } = 3000;
        #endregion Notifications

        #region Updates
        public bool CheckForUpdates { get; set; } = true;
        public bool? AllowUpdateToPreRelease { get; set; } = null;
        public bool ShowUpdateMessageBox { get; set; } = false;
        #endregion Updates

        #region Hotkeys
        public BindableHotkeyJsonWrapper[] Hotkeys { get; set; } = null;
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
        public string Target { get; set; } = string.Empty;
        public bool LockTargetSession { get; set; } = false;
        public int VolumeStepSize { get; set; } = 2;
        public List<string> EnabledDevices { get; set; } = new() { string.Empty };
        #endregion Audio
    }
}
