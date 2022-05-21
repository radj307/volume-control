using Semver;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using VolumeControl.Attributes;
using VolumeControl.Audio;
using VolumeControl.Core;
using VolumeControl.Helpers.Addon;
using VolumeControl.Helpers.Update;
using VolumeControl.Hotkeys;
using VolumeControl.Hotkeys.Addons;
using VolumeControl.Log.Enum;
using VolumeControl.Win32;
using VolumeControl.WPF;

namespace VolumeControl.Helpers
{
    public class VolumeControlSettings : INotifyPropertyChanged, INotifyCollectionChanged, IDisposable
    {
        public VolumeControlSettings()
        {
            var appDomain = AppDomain.CurrentDomain;
            ExecutablePath = Path.Combine(appDomain.RelativeSearchPath ?? appDomain.BaseDirectory, Path.ChangeExtension(appDomain.FriendlyName, ".exe"));
            _registryRunKeyHelper = new();

            _audioAPI = new();
            _hWndMixer = WindowHandleGetter.GetWindowHandle();

            var assembly = Assembly.GetAssembly(typeof(VolumeControlSettings));
            VersionNumber = assembly?.GetCustomAttribute<AssemblyAttribute.ExtendedVersion>()?.Version ?? string.Empty;
            ReleaseType = assembly?.GetCustomAttribute<ReleaseType>()?.Type ?? ERelease.NONE;

            Version = VersionNumber.GetSemVer() ?? new(0, 0, 0);
            AddonManager = new(Version);
            // load actions, including addons:
            List<object> objects = new();
            List<Type> l = AddonManager.ActionAddonTypes;
            foreach (Type type in l)
                objects.Add(Activator.CreateInstance(type)!);

            _hotkeyManager = new(new HotkeyActionManager(
                new AudioAPIActions(_audioAPI),
                new WindowsAPIActions(_hWndMixer),
                objects
            ));

            // Initialize the addon API
            API.Internal.Initializer.Initialize(_audioAPI, _hotkeyManager, _hWndMixer);

            Log.Info($"Volume Control v{VersionNumber} Initializing...");

            // v Load Settings v //
            ShowIcons = Settings.ShowIcons;
            AdvancedHotkeyMode = Settings.AdvancedHotkeys;
            RunAtStartup = Settings.RunAtStartup;
            StartMinimized = Settings.StartMinimized;
            CheckForUpdates = Settings.CheckForUpdatesOnStartup;
            NotificationEnabled = Settings.NotificationEnabled;
            NotificationTimeout = Settings.NotificationTimeoutInterval;
            // ^ Load Settings ^ //

            Log.Debug($"{nameof(VolumeControlSettings)} finished initializing settings from all assemblies.");

            _audioAPI.PropertyChanged += (s, e) => OnPropertyChanged($"AudioAPI.{e.PropertyName}");
        }
        private void SaveSettings()
        {
            // v Save Settings v //
            Settings.ShowIcons = ShowIcons;
            Settings.AdvancedHotkeys = AdvancedHotkeyMode;
            Settings.RunAtStartup = RunAtStartup;
            Settings.StartMinimized = StartMinimized;
            Settings.CheckForUpdatesOnStartup = CheckForUpdates;
            Settings.NotificationEnabled = NotificationEnabled;
            Settings.NotificationTimeoutInterval = NotificationTimeout;
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
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Fields
        #region PrivateFields
        private bool disposedValue;
        private readonly AudioAPI _audioAPI;
        private readonly HotkeyManager _hotkeyManager;
        private readonly IntPtr _hWndMixer;
        private readonly RunKeyHelper _registryRunKeyHelper;
        private const string _updateUtilityFilename = "VolumeControl.UpdateUtility.exe";
        private const string _updateUtilityResourcePath = $"VolumeControl.Resources.{_updateUtilityFilename}";
        #endregion PrivateFields
        public readonly AddonManager AddonManager;
        public readonly string ExecutablePath;
        public readonly ERelease ReleaseType;
        #endregion Fields

        #region Properties
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

        public string VersionNumber { get; private set; }
        public SemVersion Version { get; private set; }
        #endregion Statics

        #region ParentObjects
        public AudioAPI AudioAPI => _audioAPI;
        public HotkeyManager HotkeyAPI => _hotkeyManager;
        #endregion ParentObjects

        #region Settings
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
        private bool _showIcons;
        /// <summary>
        /// Gets or sets the hotkey editor mode, which can be either false (basic mode) or true (advanced mode).
        /// </summary>
        /// <remarks>Advanced mode allows the user to perform additional actions in the hotkey editor:
        /// <list type="bullet">
        /// <item><description>Create and delete hotkeys.</description></item>
        /// <item><description>Change the action bindings of hotkeys.</description></item>
        /// <item><description>Rename hotkeys.</description></item>
        /// </list></remarks>
        public bool AdvancedHotkeyMode
        {
            get => _advancedHotkeyMode;
            set
            {
                _advancedHotkeyMode = value;
                OnPropertyChanged();
            }
        }
        private bool _advancedHotkeyMode;
        public bool RunAtStartup
        {
            get => _registryRunKeyHelper.CheckRunAtStartup(Settings.RegistryStartupValueName, ExecutablePath);
            set
            {
                try
                {
                    if (value)
                    {
                        _registryRunKeyHelper.EnableRunAtStartup(Settings.RegistryStartupValueName, ExecutablePath);
                        Log.Conditional(new(EventType.DEBUG, $"Enabled Run at Startup: {{ Value: {Settings.RegistryStartupValueName}, Path: {ExecutablePath} }}"), new(EventType.INFO, "Enabled Run at Startup."));
                    }
                    else
                    {
                        _registryRunKeyHelper.DisableRunAtStartup(Settings.RegistryStartupValueName);
                        Log.Info("Disabled Run at Startup.");
                    }

                    OnPropertyChanged();
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to create run at startup registry key!", $"{{ Value: '{Settings.RegistryStartupValueName}', Path: '{ExecutablePath}' }}", ex);
                }
            }
        }
        /// <summary>
        /// Gets or sets whether the window should be minimized during startup.<br/>
        /// The window can be shown again later using the tray icon.
        /// </summary>
        public bool StartMinimized
        {
            get => _startMinimized;
            set
            {
                _startMinimized = value;
                OnPropertyChanged();
            }
        }
        private bool _startMinimized;
        /// <summary>
        /// Gets or sets whether the program should check for updates.<br/>
        /// 
        /// </summary>
        public bool CheckForUpdates
        {
            get => _checkForUpdates;
            set
            {
                if ((_checkForUpdates = value) && !_hasCheckedForUpdates)
                {
                    var updateTask = CheckForUpdatesHttps();
                    updateTask.Wait(-1); // wait asynchronously for the update check to complete
                    if (updateTask.Result)
                    { // user wants to use the auto-updater, shutdown now.
                        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                        Application.Current.Shutdown();
                    }
                }
                OnPropertyChanged();
            }
        }
        private bool _checkForUpdates, _hasCheckedForUpdates = false;
        /// <summary>
        /// Gets or sets whether notifications are enabled or not.
        /// </summary>
        public bool NotificationEnabled
        {
            get => _notificationEnabled;
            set
            {
                _notificationEnabled = value;
                OnPropertyChanged();
            }
        }
        private bool _notificationEnabled;
        public int NotificationTimeout
        {
            get => _notificationTimeout;
            set
            {
                _notificationTimeout = value;
                OnPropertyChanged();
            }
        }
        private int _notificationTimeout;
        #endregion Settings
        #endregion Properties

        #region Methods
        /// <summary>
        /// Retrieves the list of releases from the Github API, and if a newer version is found a message box is shown prompting the user to update.
        /// </summary>
        /// <returns>True when the autoupdater is ready & waiting for the program to shutdown, otherwise false.</returns>
        private async Task<bool> CheckForUpdatesHttps()
        {
            _hasCheckedForUpdates = true;
            if (await UpdateChecker.CheckForUpdates(VersionNumber, ReleaseType.Equals(ERelease.PRERELEASE)).ConfigureAwait(false) is (SemVersion, GithubReleaseHttpResponse) update)
            {
                var (newVersion, response) = update;
                switch (MessageBox.Show(
                    $"Current Version:  {VersionNumber}\n" +
                    $"Newest Version:   {newVersion}\n" +
                    "\nDo you want to update now?\n\nClick 'Yes' to update now.\nClick 'No' to update later.\nClick 'Cancel' to disable update checking."
                    , "Update Available", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No))
                {
                case MessageBoxResult.Yes: // open browser
                    string asset_url = response.assets.First(a => a.name.Equals("VolumeControl.exe", StringComparison.OrdinalIgnoreCase)).browser_download_url;
                    Log.Info($"Updating to version {newVersion}: '{asset_url}'");

                    if (SetupUpdateUtility() is string path)
                    {
                        Log.Info($"Automatic update utility was created at {path}");
                        ProcessStartInfo psi = new(path, $"--url \"{asset_url}\" --path {ExecutablePath}")
                        {
                            ErrorDialog = true,
                            UseShellExecute = true,
                        };
                        Process.Start(psi);
                        return true;
                    }
                    else
                    {
                        Log.Error("Failed to setup automatic update utility, falling back to the default web browser.");
                        UpdateChecker.OpenBrowser(response.html_url);
                    }
                    break;
                case MessageBoxResult.No: // do nothing
                    Log.Info($"Version {newVersion} is available, but was temporarily ignored.");
                    break;
                case MessageBoxResult.Cancel: // disable
                    Settings.CheckForUpdatesOnStartup = false;
                    Log.Info("Disabled automatic updates");
                    Settings.Save();
                    Settings.Reload();
                    break;
                }
            }
            return false;
        }
        /// <summary>
        /// Writes the update client from the embedded resource dictionary to the local disk.
        /// </summary>
        /// <param name="asyncTimeout">This method uses asynchronous stream operations, setting this to any value other than -1 will set a timeout in milliseconds before throwing an error and returning.</param>
        /// <returns>The absolute filepath of the updater utility's executable.</returns>
        internal string? SetupUpdateUtility(int asyncTimeout = -1)
        {
            var asm = Assembly.GetEntryAssembly();
            if (asm == null)
                return null;

            try
            {

            }
            catch (Exception ex)
            {
                Log.Error(
                    "An exception was thrown while creating the update utility!",
                   $"{{ Resource: '{_updateUtilityResourcePath}' }}",
                    ex);
            }
            string path = Path.Combine(Path.GetDirectoryName(ExecutablePath) ?? string.Empty, _updateUtilityFilename);

            if (File.Exists(path)) // if the file already exists, delete it
            {
                Log.Warning($"Deleted update utility from a previous update located at '{path}'");
                File.Delete(path);
            }

            using Stream? s = asm.GetManifestResourceStream(_updateUtilityResourcePath);

            if (s == null)
            {
                Log.Error($"Failed to get a stream containing resource '{_updateUtilityResourcePath}'!", "This indicates that something may have gone wrong during the build process!", "Please attach a copy of this log file and report this at https://github.com/radj307/volume-control/issues");
                return null;
            }
            else if (s.Length <= 0)
            {
                Log.Error("Failed to retrieve the embedded update utility resource file!", "This indicates that something may have gone wrong during the build process!", "Please attach a copy of this log file and report this at https://github.com/radj307/volume-control/issues");
                s.Close();
                s.Dispose();
                return null;
            }

            using Stream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            fs.SetLength(s.Length);
            s.CopyTo(fs);
            s.Close();
            s.Dispose();

            fs.Flush(); //< idk if this is required, but it's probably a good idea
            fs.Close();
            fs.Dispose();

            if (!File.Exists(path))
                return null;

            return path;
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
