using Newtonsoft.Json;
using System.ComponentModel;

namespace VolumeControl.Core
{
    [JsonObject]
    public class Config : AppConfig.ConfigurationFile, INotifyPropertyChanged
    {
        private const string _filePath = "VolumeControl.json";
        public Config() : base(_filePath) { }

        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion Events

        public bool RunAtStartup { get; set; } = false;
        public bool StartMinimized { get; set; } = false;
        public bool ShowInTaskbar { get; set; } = true;
        public bool AlwaysOnTop { get; set; } = false;
        public bool AdvancedHotkeys { get; set; } = false;
        public bool ShowIcons { get; set; } = true;
        public List<string> CustomAddonDirectories { get; set; } = new();

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
    }
}
