using HotkeyLib;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using VolumeControl.Core;

namespace VolumeControl.Helpers
{
    public class VolumeControlSettings : INotifyPropertyChanged
    {
        public VolumeControlSettings()
        {
            _audioAPI = new();
            _hotkeyManager = new()
            {
                AudioAPI = _audioAPI
            };

            ShowIcons = Settings.ShowIcons;

            Log.Debug($"{nameof(VolumeControlSettings)} finished initializing settings from all assemblies.");

            _hotkeyManager.EndInit();
        }
        ~VolumeControlSettings()
        {
            // VolumeControl
            Settings.ShowIcons = ShowIcons;
            // save settings
            Settings.Save();
            Settings.Reload();

            // VolumeControl.Core
            // save coresettings
            CoreSettings.Save();
            CoreSettings.Reload();

            // VolumeControl.Log
            // save logsettings
            LogSettings.Save();
            LogSettings.Reload();

            Log.Debug($"{nameof(VolumeControlSettings)} finished saving settings from all assemblies.");
        }

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Fields
        private bool _showIcons;
        private AudioAPI _audioAPI;
        private HotkeyManager _hotkeyManager;
        #endregion Fields

        #region Properties
        #region Statics
        /// <summary>Static accessor for <see cref="Properties.Settings.Default"/>.</summary>
        private static Properties.Settings Settings => Properties.Settings.Default;
        /// <summary>Static accessor for <see cref="Core.CoreSettings.Default"/>.</summary>
        private static Core.CoreSettings CoreSettings => CoreSettings.Default;
        /// <summary>Static accessor for <see cref="Log.Properties.Settings.Default"/>.</summary>
        private static Log.Properties.Settings LogSettings => global::VolumeControl.Log.Properties.Settings.Default;
        private static Log.LogWriter Log => global::VolumeControl.Log.FLog.Log;
        #endregion Statics

        #region ParentObjects
        public AudioAPI AudioAPI => _audioAPI;
        public HotkeyManager HotkeyAPI => _hotkeyManager;
        #endregion ParentObjects

        /// <summary>
        /// Gets or sets a boolean that determines whether or not device/session icons are shown in the UI.
        /// </summary>
        public bool ShowIcons
        {
            get => _showIcons;
            set
            {
                _showIcons = value;
                OnPropertyChanged();
            }
        }
        // TODO: Advanced Hotkeys
        // TODO: Run At Startup
        // TODO: Start Minimized
        // TODO: Enable Notifications
        // TODO: Notification Timeout
        #endregion Properties

        #region Methods
        #region SettingsManipulation
        public void ResetHotkeySettings()
        {
            if (MessageBox.Show("Are you sure you want to reset your hotkeys to their default values?\n\nThis cannot be undone!", "Reset Hotkeys?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                HotkeyAPI.ResetHotkeys();

                Log.Info("Hotkey definitions were reset to default.");
            }
        }
        #endregion SettingsManipulation
        #endregion Methods
    }
}
