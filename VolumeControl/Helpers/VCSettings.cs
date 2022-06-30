using Semver;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using VolumeControl.Attributes;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.Helpers.Win32;
using VolumeControl.Log;
using VolumeControl.Log.Enum;
using VolumeControl.Properties;
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
        /// <inheritdoc/>
        public bool ShowIcons
        {
            get => Settings.ShowIcons;
            set => Settings.ShowIcons = value;
        }
        /// <inheritdoc/>
        public bool AdvancedHotkeyMode
        {
            get => Settings.AdvancedHotkeys;
            set => Settings.AdvancedHotkeys = value;
        }
        /// <inheritdoc/>
        public bool RunAtStartup
        {
            get => Settings.RunAtStartup;
            set => Settings.RunAtStartup = value;
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
        private static string GetExecutablePath()
        {
            if (Process.GetCurrentProcess().MainModule?.FileName is string path)
                return path;
            else throw new Exception($"{nameof(VCSettings)} Error:  Retrieving the current executable path failed!");
        }
        #endregion Methods
    }
}
