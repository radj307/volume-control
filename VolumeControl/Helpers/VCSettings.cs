using Semver;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using VolumeControl.Attributes;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.Helpers.Win32;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;
using VolumeControl.WPF;

namespace VolumeControl.Helpers
{
    public abstract class VCSettings : INotifyPropertyChanged
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

            RunAtStartup = RunAtStartupHelper.ValueEquals(ExecutablePath);

            // Set the notification mode
            NotificationMode = DisplayTarget.Sessions;

            Log.Debug($"{nameof(VCSettings)} initialization completed.");
        }
        #endregion Constructors

        #region Properties
        #region Statics
        private static Config Settings => (Config.Default as Config)!;
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
        /// <inheritdoc cref="Config.EnableDefaultDevice"/>
        public bool EnableDefaultDevice
        {
            get => Settings.EnableDefaultDevice;
            set => Settings.EnableDefaultDevice = value;
        }
        /// <inheritdoc cref="Config.ShowIcons"/>
        public bool ShowIcons
        {
            get => Settings.ShowIcons;
            set => Settings.ShowIcons = value;
        }
        /// <inheritdoc cref="Config.AdvancedHotkeys"/>
        public bool AdvancedHotkeyMode
        {
            get => Settings.AdvancedHotkeys;
            set => Settings.AdvancedHotkeys = value;
        }
        /// <inheritdoc cref="Config.RunAtStartup"/>
        public bool RunAtStartup
        {
            get => Settings.RunAtStartup && RunAtStartupHelper.ValueEquals(ExecutablePath);
            set
            {
                bool state = Settings.RunAtStartup = value;
                if (state)
                    RunAtStartupHelper.Value = ExecutablePath;
                else // remove the registry value:
                    RunAtStartupHelper.Value = null;
            }
        }
        /// <inheritdoc/>
        public bool StartMinimized
        {
            get => Settings.StartMinimized;
            set => Settings.StartMinimized = value;
        }
        /// <inheritdoc/>
        public bool CheckForUpdates
        {
            get => Settings.CheckForUpdates;
            set => Settings.CheckForUpdates = value;
        }
        /// <inheritdoc/>
        public bool ShowUpdateMessageBox
        {
            get => Settings.ShowUpdatePrompt;
            set => Settings.ShowUpdatePrompt = value;
        }
        /// <inheritdoc/>
        public bool NotificationEnabled
        {
            get => Settings.NotificationsEnabled;
            set => Settings.NotificationsEnabled = value;
        }
        /// <inheritdoc/>
        public int NotificationTimeout
        {
            get => Settings.NotificationTimeoutMs;
            set => Settings.NotificationTimeoutMs = value;
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
            get => Settings.NotificationsOnVolumeChange;
            set => Settings.NotificationsOnVolumeChange = value;
        }
        #endregion Properties

        #region Events
#       pragma warning disable CS0067 // The event 'BindableHotkey.PropertyChanged' is never used ; This is automatically used by Fody.
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
#       pragma warning restore CS0067 // The event 'BindableHotkey.PropertyChanged' is never used ; This is automatically used by Fody.
        #endregion Events

        #region Methods
        private static string GetExecutablePath()
        {
            if (Process.GetCurrentProcess().MainModule?.FileName is string path)
                return path;
            else throw new Exception($"{nameof(VCSettings)} Error:  Retrieving the current executable path failed!");
        }
        #endregion Methods
    }
}
