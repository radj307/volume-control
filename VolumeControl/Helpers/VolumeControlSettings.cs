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
using VolumeControl.Core.Helpers;
using VolumeControl.Helpers.Addon;
using VolumeControl.Helpers.Update;
using VolumeControl.Hotkeys;
using VolumeControl.Hotkeys.Addons;
using VolumeControl.WPF;
using static VolumeControl.Audio.AudioAPI;
using VolumeControl.TypeExtensions;
using VolumeControl.Hotkeys.Interfaces;
using System.Runtime.CompilerServices;
using System.Windows.Interop;
using VolumeControl.Properties;
using VolumeControl.Log;
using VolumeControl.Log.Enum;
using Microsoft.Win32;
using VolumeControl.Helpers.Win32;
using System.Diagnostics;

namespace VolumeControl.Helpers
{

    public abstract class VCSettings : IVCSettings
    {
        #region Constructors
        public VCSettings()
        {
            Log.Debug($"{nameof(VCSettings)} initializing...");

            // Initialize the HWndHook
            HWndHook = new(WindowHandleGetter.GetHwndSource(MainWindowHandle = WindowHandleGetter.GetWindowHandle()));
            HWndHook.AddMaximizeBugFixHandler();

            // Get the executing assembly
            var asm = Assembly.GetExecutingAssembly();

            // Get the executable path

            ExecutablePath = GetExecutablePath();
            Log.Debug($"{nameof(VCSettings)}.{nameof(ExecutablePath)} = '{ExecutablePath}'");

            // Get the current version number & release type
            CurrentVersionString = asm.GetCustomAttribute<AssemblyAttribute.ExtendedVersion>()?.Version ?? string.Empty;
            CurrentVersion = CurrentVersionString.GetSemVer() ?? new(0, 0, 0);
            ReleaseType = asm.GetCustomAttribute<ReleaseType>()?.Type ?? ERelease.NONE;
            Log.Debug($"{nameof(VCSettings)}.{nameof(CurrentVersion)} = '{CurrentVersionString}'");
            Log.Debug($"{nameof(VCSettings)}.{nameof(ReleaseType)} = '{ReleaseType}'");

            // Validate the run at startup registry value
            if (RunAtStartup)
                RunAtStartupHelper.Value = ExecutablePath;
            else // remove the registry value:
                RunAtStartupHelper.Value = null;
            // Update the RunAtStartup value without setting it twice
            ProcessRunAtStartupChangedEvents = false;
            RunAtStartup = RunAtStartupHelper.ValueEquals(ExecutablePath);
            ProcessRunAtStartupChangedEvents = true;

            // Set the notification mode
            NotificationMode = DisplayTarget.Sessions;

            Log.Debug($"{nameof(VCSettings)} initialization completed.");
        }
        #endregion Constructors

        #region Properties
        #region Statics
        private static Settings Settings => Settings.Default;
        private static LogWriter Log => FLog.Log;
        #endregion Statics
        #region ReadOnlyProperties
        /// <inheritdoc/>
        public IntPtr MainWindowHandle { get; }
        /// <inheritdoc/>
        public HWndHook HWndHook { get; }
        /// <inheritdoc/>
        public string ExecutablePath { get; }
        /// <inheritdoc/>
        public string CurrentVersionString { get; }
        /// <inheritdoc/>
        public SemVersion CurrentVersion { get; }
        /// <inheritdoc/>
        public ERelease ReleaseType { get; }
        #endregion ReadOnlyProperties
        /// <inheritdoc/>
        public bool ShowIcons
        {
            get => Settings.ShowIcons;
            set
            {
                Settings.ShowIcons = value;
                Save();
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public bool AdvancedHotkeyMode
        {
            get => Settings.AdvancedHotkeys;
            set
            {
                Settings.AdvancedHotkeys = value;
                Save();
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public bool RunAtStartup
        {
            get => Settings.RunAtStartup;
            set
            {
                Settings.RunAtStartup = value;
                Save();
                NotifyPropertyChanged();
                NotifyRunAtStartupChanged();
            }
        }
        /// <inheritdoc/>
        public bool StartMinimized
        {
            get => Settings.StartMinimized;
            set
            {
                Settings.StartMinimized = value;
                Save();
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public bool CheckForUpdates
        {
            get => Settings.CheckForUpdatesOnStartup;
            set
            {
                Settings.CheckForUpdatesOnStartup = value;
                Save();
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public bool? AllowUpdateToPreRelease
        {
            get => Settings.AllowUpdateToPreRelease.ToBoolean();
            set
            {
                Settings.AllowUpdateToPreRelease = value.ToThreeStateNumber();
                Save();
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public bool ShowUpdateMessageBox
        {
            get => Settings.ShowUpdateMessageBox;
            set
            {
                Settings.ShowUpdateMessageBox = value;
                Save();
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public bool NotificationEnabled
        {
            get => Settings.NotificationEnabled;
            set
            {
                Settings.NotificationEnabled = value;
                Save();
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public int NotificationTimeout
        {
            get => Settings.NotificationTimeoutInterval;
            set
            {
                Settings.NotificationTimeoutInterval = value;
                Save();
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public DisplayTarget NotificationMode
        {
            get;
            set;
        }
        /// <inheritdoc/>
        public bool NotificationShowsVolumeChange
        {
            get => Settings.NotificationShowsVolumeChange;
            set
            {
                Settings.NotificationShowsVolumeChange = value;
                Save();
                NotifyPropertyChanged();
            }
        }
        #endregion Properties

        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>Triggers the <see cref="PropertyChanged"/> event.</summary>
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new(propertyName));
            if (Log.FilterEventType(EventType.DEBUG)) // only perform reflection step when log message will actually be printed
                Log.Debug($"VCSettings.{propertyName} = '{typeof(VCSettings).GetProperty(propertyName)?.GetValue(this)}'");
        }
        public event EventHandler<bool>? RunAtStartupChanged;
        /// <summary>
        /// This can be used to disable internal processing of the <see cref="RunAtStartupChanged"/> event.<br/>Default is <see langword="true"/>.
        /// </summary>
        /// <remarks>Note that setting this to <see langword="false"/> will disable <b>all registry accesses</b> resulting from internal handling of the <see cref="RunAtStartupChanged"/> event, and <b><i>will</i></b> cause the <see cref="RunAtStartup"/> property to behave unexpectedly!</remarks>
        public bool ProcessRunAtStartupChangedEvents = true;
        private void NotifyRunAtStartupChanged()
        {
            if (!ProcessRunAtStartupChangedEvents)
                return;

            if (RunAtStartup)
                RunAtStartupHelper.Value = ExecutablePath;
            else // remove the registry value:
                RunAtStartupHelper.Value = null;

            NotifyPropertyChanged(nameof(RunAtStartup));

            // don't save the value of run at startup to a variable or the UI won't update properly if an error occurs
            RunAtStartupChanged?.Invoke(this, RunAtStartup);
        }
        #endregion Events

        #region Methods
        /// <inheritdoc/>
        public void Load() => Settings.Reload();
        /// <inheritdoc/>
        public void Save()
        {
            Settings.Save();
            Load();
        }
        private static string GetExecutablePath()
        {
            if (Process.GetCurrentProcess().MainModule?.FileName is string path)
                return path;
            else throw new Exception($"{nameof(VCSettings)} Error:  Retrieving the current executable path failed!");
        }
        #endregion Methods
    }


    /// <summary>Inherits from the <see cref="VCSettingsContainer"/> object.</summary>
    public class VolumeControlSettings : VCSettings, INotifyPropertyChanged, INotifyCollectionChanged, IDisposable
    {
        public VolumeControlSettings() : base()
        {
            _audioAPI = new();

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
            Actions = actionManager
                .Bindings
                .Where(a => a.Name.Length > 0)
                .OrderBy(a => a.Name[0])
                .ToList();
            // Create the hotkey manager
            _hotkeyManager = new(actionManager, HWndHook, true);

            // Initialize the addon API
            API.Internal.Initializer.Initialize(_audioAPI, _hotkeyManager, MainWindowHandle, this);

            Log.Info($"Volume Control v{CurrentVersionString}");

            Log.Debug($"{nameof(VolumeControlSettings)} finished initializing settings from all assemblies.");

            Updater = new(this);
            if (Settings.CheckForUpdatesOnStartup)
                Updater.Update();
        }
        private void SaveSettings()
        {
            this.Save();

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
        public IEnumerable<string> TargetAutoCompleteSource => _targetAutoCompleteSource ??= AudioAPI.GetSessionNames(SessionNameFormat.ProcessIdentifier | SessionNameFormat.ProcessName);
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
        private static Properties.Settings Settings => Properties.Settings.Default;
        /// <summary>Static accessor for <see cref="HotkeyManagerSettings.Default"/>.</summary>
        private static HotkeyManagerSettings HotkeySettings => HotkeyManagerSettings.Default;
        /// <summary>Static accessor for <see cref="AudioAPISettings.Default"/>.</summary>
        private static AudioAPISettings AudioSettings => AudioAPISettings.Default;
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
