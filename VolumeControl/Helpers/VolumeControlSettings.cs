using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using VolumeControl.Audio;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Keyboard.Actions;
using VolumeControl.Helpers.Addon;
using VolumeControl.Helpers.Update;
using VolumeControl.Hotkeys;
using VolumeControl.Hotkeys.Addons;
using VolumeControl.Hotkeys.Interfaces;
using VolumeControl.Log;
using VolumeControl.Properties;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Helpers
{
    /// <summary>Inherits from the <see cref="VCSettingsContainer"/> object.</summary>
    public class VolumeControlSettings : VCSettings, INotifyPropertyChanged, INotifyCollectionChanged, IDisposable
    {
        public VolumeControlSettings() : base()
        {
            // create the updater & check for updates if enabled
            Updater = new(this);
            if (Settings.CheckForUpdates) Updater.CheckNow();

            _audioAPI = new();

            AddonManager = new(this);

            // Hotkey Action Addons:
            HotkeyActionManager actionManager = new();
            // Add built-in action container types
            actionManager.Types.Add(typeof(AudioDeviceActions));
            actionManager.Types.Add(typeof(AudioSessionActions));
            actionManager.Types.Add(typeof(ApplicationActions));
            actionManager.Types.Add(typeof(MediaActions));

            // Create the hotkey manager
            _hotkeyManager = new(actionManager);

            // Initialize the addon API
            API.Internal.Initializer.Initialize(_audioAPI, _hotkeyManager, MainWindowHandle, (Config.Default as Config)!);

            // Create a list of all addon manager types
            List<IBaseAddon> addons = new()
            {
                actionManager
            };

            // Load addon types into the addon list
            AddonManager.LoadAddons(ref addons);

            // Retrieve a list of all loaded action names
            const char sort_last = (char)('z' + 1);
            Actions = actionManager
                .Bindings
                .OrderBy(a => a.Data.ActionName.AtIndexOrDefault(0, sort_last))                                           //< sort entries alphabetically
                .OrderBy(a => a.Data.ActionGroup is null ? sort_last : a.Data.ActionGroup.AtIndexOrDefault(0, sort_last)) //< sort entries by group; null groups are always last.
                .ToList();

            // load saved hotkeys
            //  We need to have accessed the Settings property at least once by the time we reach this point
            _hotkeyManager.LoadHotkeys();

            Log.Info($"Volume Control v{CurrentVersionString}");
            Log.Debug($"{nameof(VolumeControlSettings)} finished initializing settings from all assemblies.");
        }
        private void SaveSettings()
        {
            // VolumeControl.Log
            LogSettings.Save();
            LogSettings.Reload();
        }

        #region Events
        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add => ((INotifyCollectionChanged)_hotkeyManager).CollectionChanged += value;
            remove => ((INotifyCollectionChanged)_hotkeyManager).CollectionChanged -= value;
        }
        #endregion Events

        #region Fields
        #region PrivateFields
        private bool disposedValue;
        private readonly AudioAPI _audioAPI;
        private readonly HotkeyManager _hotkeyManager;
        private IEnumerable<string>? _targetAutoCompleteSource;
        #endregion PrivateFields
        public readonly AddonManager AddonManager;
        public readonly Updater Updater;
        #endregion Fields

        #region Properties
        #region Other
        /// <summary>
        /// This is used by the target box's autocomplete feature, and is automatically invalidated & refreshed each time the sessions list changes.
        /// </summary>
        public IEnumerable<string> TargetAutoCompleteSource => _targetAutoCompleteSource ??= AudioAPI.GetSessionNames(AudioAPI.SessionNameFormat.ProcessIdentifier | AudioAPI.SessionNameFormat.ProcessName);
        public IEnumerable<IActionBinding> Actions
        {
            get => _actions;
            internal set
            {
                _actions = value;
                NotifyPropertyChanged();
            }
        }
        private IEnumerable<IActionBinding> _actions = null!;
        public IEnumerable<string> NotificationModes
        {
            get => _notificationModes;
            set
            {
                _notificationModes = value;
                NotifyPropertyChanged();
            }
        }
        private IEnumerable<string> _notificationModes = Enum.GetNames(typeof(DisplayTarget));
        #endregion Other

        #region Statics
        #region PrivateStatics
        /// <summary>Static accessor for <see cref="Settings.Default"/>.</summary>
        private static Config Settings => (Config.Default as Config)!;
        /// <summary>Static accessor for <see cref="Log.Properties.Settings.Default"/>.</summary>
        private static Log.Properties.Settings LogSettings => VolumeControl.Log.Properties.Settings.Default;
        private static Log.LogWriter Log => FLog.Log;
        #endregion PrivateStatics
        /// <summary>
        /// True when there is a newer version of volume control available.
        /// </summary>
        public bool UpdateAvailable
        {
            get => _updateAvailable;
            internal set
            {
                _updateAvailable = value;
                NotifyPropertyChanged();
            }
        }
        private bool _updateAvailable = false;
        /// <summary>
        /// The version number of the new version, as a string.
        /// </summary>
        public string UpdateVersion
        {
            get => _updateVersion;
            internal set
            {
                _updateVersion = value;
                NotifyPropertyChanged();
            }
        }
        private string _updateVersion = string.Empty;
        #endregion Statics

        #region ParentObjects
        public AudioAPI AudioAPI => _audioAPI;
        public HotkeyManager HotkeyAPI => _hotkeyManager;
        #endregion ParentObjects
        #endregion Properties

        #region Methods
        public static string GetExecutablePath()
        {
            AppDomain? appDomain = AppDomain.CurrentDomain;
            return Path.Combine(appDomain.RelativeSearchPath ?? appDomain.BaseDirectory, Path.ChangeExtension(appDomain.FriendlyName, ".exe"));
        }
        /// <summary>Displays a message box prompting the user for confirmation, and if confirmation is given, resets all hotkeys to their default settings using <see cref="HotkeyManager.ResetHotkeys"/>.</summary>
        public void ResetHotkeySettings()
        {
            if (MessageBox.Show("Are you sure you want to reset your hotkeys to their default values?\n\nThis cannot be undone!", "Reset Hotkeys?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                HotkeyAPI.ResetHotkeys();

                Log.Info("Hotkey definitions were reset to default.");
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    SaveSettings();
                    // Dispose of objects
                    AudioAPI?.Dispose();
                    HotkeyAPI?.Dispose();
                }

                disposedValue = true;
            }
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        ~VolumeControlSettings()
        {
            Dispose(disposing: true);
        }
        #endregion Methods
    }
}
