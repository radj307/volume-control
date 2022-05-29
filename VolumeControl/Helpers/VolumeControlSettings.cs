using Semver;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using VolumeControl.Attributes;
using VolumeControl.Audio;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Extensions;
using VolumeControl.Core.Helpers;
using VolumeControl.Helpers.Addon;
using VolumeControl.Helpers.Update;
using VolumeControl.Hotkeys;
using VolumeControl.Hotkeys.Addons;
using VolumeControl.WPF;
using static VolumeControl.Audio.AudioAPI;

namespace VolumeControl.Helpers
{
    /// <summary>Inherits from the <see cref="VCSettingsContainer"/> object.</summary>
    public class VolumeControlSettings : VCSettingsContainer, INotifyPropertyChanged, INotifyPropertyChanging, INotifyCollectionChanged, IDisposable
    {
        public VolumeControlSettings() : base(GetExecutablePath())
        {
            _audioAPI = new();
            _hWndMixer = WindowHandleGetter.GetWindowHandle();

            var assembly = Assembly.GetAssembly(typeof(VolumeControlSettings));
            VersionNumber = assembly?.GetCustomAttribute<AssemblyAttribute.ExtendedVersion>()?.Version ?? string.Empty;
            ReleaseType = assembly?.GetCustomAttribute<ReleaseType>()?.Type ?? ERelease.NONE;

            Version = VersionNumber.GetSemVer() ?? new(0, 0, 0);
            AddonManager = new(this);

            HotkeyActionManager actionManager = new();
            // Add premade actions
            actionManager.Types.Add(typeof(AudioAPIActions));
            actionManager.Types.Add(typeof(WindowsAPIActions));

            List<IBaseAddon> addons = new()
            {
                actionManager
            };
            // Load addons
            AddonManager.LoadAddons(ref addons);
            // Retrieve a list of all loaded action names
            ActionNames = actionManager
                .GetActionNames()
                .Where(s => s.Length > 0)
                .OrderBy(s => s[0])
                .ToList();
            // Create the hotkey manager
            _hotkeyManager = new(actionManager);
            _hotkeyManager.LoadHotkeys();

            // Initialize the addon API
            API.Internal.Initializer.Initialize(_audioAPI, _hotkeyManager, _hWndMixer, this);

            Log.Info($"Volume Control v{VersionNumber} Initializing...");

            // v Load Settings v //
            ShowIcons = Settings.ShowIcons;
            AdvancedHotkeyMode = Settings.AdvancedHotkeys;
            RunAtStartup = Settings.RunAtStartup;
            StartMinimized = Settings.StartMinimized;
            CheckForUpdates = Settings.CheckForUpdatesOnStartup;
            AllowUpdateToPreRelease = Settings.AllowUpdateToPreRelease.ToBoolean();
            ShowUpdateMessageBox = Settings.ShowUpdateMessageBox;
            NotificationEnabled = Settings.NotificationEnabled;
            NotificationTimeout = Settings.NotificationTimeoutInterval;
            NotificationShowsVolumeChange = Settings.NotificationShowsVolumeChange;
            // ^ Load Settings ^ //

            Log.Debug($"{nameof(VolumeControlSettings)} finished initializing settings from all assemblies.");

            _audioAPI.PropertyChanged += Handle_AudioAPI_PropertyChanged;

            Updater = new(this);
#if DEBUG
            Updater.Update(true);
#else
            if (Settings.CheckForUpdatesOnStartup)
            {
                Updater.Update();
            }
#endif
        }
        private void SaveSettings()
        {
            // v Save Settings v //
            Settings.ShowIcons = ShowIcons;
            Settings.AdvancedHotkeys = AdvancedHotkeyMode;
            Settings.RunAtStartup = RunAtStartup;
            Settings.StartMinimized = StartMinimized;
            Settings.CheckForUpdatesOnStartup = CheckForUpdates;
            Settings.AllowUpdateToPreRelease = AllowUpdateToPreRelease.ToThreeStateNumber();
            Settings.ShowUpdateMessageBox = ShowUpdateMessageBox;
            Settings.NotificationEnabled = NotificationEnabled;
            Settings.NotificationTimeoutInterval = NotificationTimeout;
            Settings.NotificationShowsVolumeChange = NotificationShowsVolumeChange;
            // ^ Save Settings ^ //

            // VolumeControl
            Settings.Save();
            Settings.Reload();

            // VolumeControl.Audio
            AudioSettings.Save();
            AudioSettings.Reload();

            // VolumeControl.Hotkeys
            HotkeySettings.Save();
            HotkeySettings.Reload();

            // VolumeControl.Log
            LogSettings.Save();
            LogSettings.Reload();

            Log.Debug($"{nameof(VolumeControlSettings)} finished saving settings from all assemblies.");
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
        private readonly IntPtr _hWndMixer;
        private IEnumerable<string>? _targetAutoCompleteSource;
#endregion PrivateFields
        public readonly AddonManager AddonManager;
        public readonly Updater Updater;
        public readonly ERelease ReleaseType;
#endregion Fields

#region Properties
#region Other
        /// <summary>
        /// This is used by the target box's autocomplete feature, and is automatically invalidated & refreshed each time the sessions list changes.
        /// </summary>
        public IEnumerable<string> TargetAutoCompleteSource => _targetAutoCompleteSource ??= AudioAPI.GetSessionNames(SessionNameFormat.ProcessIdentifier | SessionNameFormat.ProcessName);
        public IEnumerable<string> ActionNames
        {
            get => _actionNames;
            internal set
            {
                NotifyPropertyChanging();
                _actionNames = value;
                NotifyPropertyChanged();
            }
        }
        private IEnumerable<string> _actionNames = null!;
        public IEnumerable<string> NotificationModes
        {
            get => _notificationModes;
            set
            {
                NotifyPropertyChanging();
                _notificationModes = value;
                NotifyPropertyChanged();
            }
        }
        private IEnumerable<string> _notificationModes = Enum.GetNames(typeof(DisplayTarget));
#endregion Other

#region Statics
#region PrivateStatics
        /// <summary>Static accessor for <see cref="Properties.Settings.Default"/>.</summary>
        private static Properties.Settings Settings => Properties.Settings.Default;
        /// <summary>Static accessor for <see cref="HotkeyManagerSettings.Default"/>.</summary>
        private static HotkeyManagerSettings HotkeySettings => HotkeyManagerSettings.Default;
        /// <summary>Static accessor for <see cref="AudioAPISettings.Default"/>.</summary>
        private static AudioAPISettings AudioSettings => AudioAPISettings.Default;
        /// <summary>Static accessor for <see cref="Log.Properties.Settings.Default"/>.</summary>
        private static Log.Properties.Settings LogSettings => global::VolumeControl.Log.Properties.Settings.Default;
        private static Log.LogWriter Log => global::VolumeControl.Log.FLog.Log;
#endregion PrivateStatics
        public bool UpdateAvailable
        {
            get => _updateAvailable;
            internal set
            {
                NotifyPropertyChanging();
                _updateAvailable = value;
                NotifyPropertyChanged();
            }
        }
        private bool _updateAvailable = false;
        public string UpdateVersion
        {
            get => _updateVersion;
            internal set
            {
                NotifyPropertyChanging();
                _updateVersion = value;
                NotifyPropertyChanged();
            }
        }
        private string _updateVersion = string.Empty;
        public string VersionNumber { get; private set; }
        public SemVersion Version { get; private set; }
#endregion Statics

#region ParentObjects
        public AudioAPI AudioAPI => _audioAPI;
        public HotkeyManager HotkeyAPI => _hotkeyManager;
#endregion ParentObjects
#endregion Properties

#region EventHandlers
        protected virtual void Handle_AudioAPI_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null)
            {
                return;
            }

            if (e.PropertyName.Equals("Sessions"))
            { // reset autocomplete suggestions
                _targetAutoCompleteSource = null;
                NotifyPropertyChanged(nameof(TargetAutoCompleteSource));
            }

            NotifyPropertyChanged($"AudioAPI.{e.PropertyName}");
        }
#endregion EventHandlers

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
                    AudioAPI.Dispose();
                    HotkeyAPI.Dispose();
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
