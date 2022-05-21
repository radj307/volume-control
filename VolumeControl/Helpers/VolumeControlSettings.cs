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

            AddonManager = new();
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

            var assembly = Assembly.GetAssembly(typeof(VolumeControlSettings));
            VersionNumber = assembly?.GetCustomAttribute<AssemblyAttribute.ExtendedVersion>()?.Version ?? string.Empty;
            ReleaseType = assembly?.GetCustomAttribute<ReleaseType>()?.Type ?? ERelease.NONE;

            Log.Info($"Volume Control v{VersionNumber} Initializing...");

            // v Load Settings v //
            ShowIcons = Settings.ShowIcons;
            AdvancedHotkeyMode = Settings.AdvancedHotkeys;
            RunAtStartup = Settings.RunAtStartup;
            StartMinimized = Settings.StartMinimized;
            CheckForUpdates = Settings.CheckForUpdatesOnStartup;
            // ^ Load Settings ^ //

            Log.Debug($"{nameof(VolumeControlSettings)} finished initializing settings from all assemblies.");

            _audioAPI.PropertyChanged += (s, e) => OnPropertyChanged($"AudioAPI.{e.PropertyName}");
        }
        private void SaveSettings()
        {
            // VolumeControl
            Settings.ShowIcons = ShowIcons;
            Settings.AdvancedHotkeys = AdvancedHotkeyMode;
            Settings.RunAtStartup = RunAtStartup;
            Settings.StartMinimized = StartMinimized;
            Settings.CheckForUpdatesOnStartup = CheckForUpdates;
            // save settings
            Settings.Save();
            Settings.Reload();

            // VolumeControl.Audio
            AudioSettings.Save();
            AudioSettings.Reload();

            // VolumeControl.Hotkeys
            HotkeySettings.Save();
            HotkeySettings.Reload();

            // VolumeControl.Log
            // save logsettings
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
                    updateTask.Wait(-1);
                    if (updateTask.Result)
                    {
                        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                        Application.Current.Shutdown();
                    }
                }
                OnPropertyChanged();
            }
        }
        private bool _checkForUpdates, _hasCheckedForUpdates = false;
        #endregion Settings
        #endregion Properties

        #region Methods
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
        internal string? SetupUpdateUtility(int asyncTimeout = -1)
        {
            var asm = Assembly.GetEntryAssembly();
            if (asm == null)
                return null;

            string path = Path.Combine(Path.GetDirectoryName(ExecutablePath) ?? string.Empty, "VolumeControl.UpdateUtility.exe");

            if (File.Exists(path))
                File.Delete(path);

            using Stream s = asm.GetManifestResourceStream("VolumeControl.Resources.VolumeControl.UpdateUtility.exe")!;
            using Stream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            s.CopyToAsync(fs).Wait(asyncTimeout);
            fs.FlushAsync().Wait(asyncTimeout);
            fs.Close();
            fs.Dispose();
            s.Close();
            s.Dispose();

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
