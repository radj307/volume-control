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
            this.HWndHook = new(WindowHandleGetter.GetHwndSource(this.MainWindowHandle = WindowHandleGetter.GetWindowHandle()));
            this.HWndHook.AddMaximizeBugFixHandler();

            // Get the executing assembly
            var asm = Assembly.GetExecutingAssembly();

            // Get the executable path

            this.ExecutablePath = GetExecutablePath();
            Log.Debug($"{nameof(VCSettings)}.{nameof(this.ExecutablePath)} = '{this.ExecutablePath}'");

            // Get the current version number & release type
            this.CurrentVersionString = asm.GetCustomAttribute<AssemblyAttribute.ExtendedVersion>()?.Version ?? string.Empty;
            this.CurrentVersion = this.CurrentVersionString.GetSemVer() ?? new(0, 0, 0);
            this.ReleaseType = asm.GetCustomAttribute<ReleaseType>()?.Type ?? ERelease.NONE;
            Log.Debug($"{nameof(VCSettings)}.{nameof(this.CurrentVersion)} = '{this.CurrentVersionString}'");
            Log.Debug($"{nameof(VCSettings)}.{nameof(this.ReleaseType)} = '{this.ReleaseType}'");

            this.RunAtStartup = RunAtStartupHelper.ValueEquals(this.ExecutablePath);

            // Set the notification mode
            this.NotificationMode = DisplayTarget.Sessions;

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
            get => Settings.RunAtStartup && RunAtStartupHelper.ValueEquals(this.ExecutablePath);
            set
            {
                bool state = Settings.RunAtStartup = value;
                RunAtStartupHelper.Value = state ? this.ExecutablePath : null;
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
            get => Settings.NotificationMode;
            set => Settings.NotificationMode = value;
        }
        /// <inheritdoc/>
        public bool NotificationShowsVolumeChange
        {
            get => Settings.NotificationsOnVolumeChange;
            set => Settings.NotificationsOnVolumeChange = value;
        }
        public bool AlwaysOnTop
        {
            get => Settings.AlwaysOnTop;
            set => Settings.AlwaysOnTop = value;
        }
        public bool ShowInTaskbar
        {
            get => Settings.ShowInTaskbar;
            set => Settings.ShowInTaskbar = value;
        }
        #endregion Properties

        #region Events
#       pragma warning disable CS0067 // The event 'BindableHotkey.PropertyChanged' is never used ; This is automatically used by Fody.
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
#       pragma warning restore CS0067 // The event 'BindableHotkey.PropertyChanged' is never used ; This is automatically used by Fody.
        #endregion Events

        #region Methods
        private static string GetExecutablePath() => Process.GetCurrentProcess().MainModule?.FileName is string path
                ? path
                : throw new Exception($"{nameof(VCSettings)} Error:  Retrieving the current executable path failed!");
        #endregion Methods
    }
}
