using Semver;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using VolumeControl.Core;
using VolumeControl.Helpers.Win32;
using VolumeControl.Log;
using VolumeControl.WPF;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Helpers
{
    public abstract class VCSettings : INotifyPropertyChanged
    {
        #region Constructors
        public VCSettings()
        {
            FLog.Debug($"{nameof(VCSettings)} initializing...");

            // Initialize the HWndHook
            this.MainWindowHandle = WindowHandleGetter.GetWindowHandle();
            this.HWndHook = new(WindowHandleGetter.GetHwndSource(this.MainWindowHandle));
            this.HWndHook.AddMaximizeBugFixHandler();

            // Get the executable path
            this.ExecutablePath = GetExecutablePath();
            FLog.Debug($"Executable location: '{this.ExecutablePath}'",
                      $"Working directory:   '{Environment.CurrentDirectory}'");

            // Get the current version number (version config property has already been updated)
            this.CurrentVersion = Settings.__VERSION__;

#       if DEBUG // Show 'DEBUG' in the version string when in Debug configuration
            this.CurrentVersionString = $"DEBUG | {this.CurrentVersion}";
#       else // Use the normal version string in Release configuration
            this.CurrentVersionString = this.CurrentVersion.ToString();
#       endif

#       if DEBUG // Debug configuration:
            FLog.Info($"Volume Control version {this.CurrentVersion} (DEBUG)");
#       elif RELEASE // Release configuration:
            Log.Info($"Volume Control version {this.CurrentVersion} (Portable)");
#       elif RELEASE_FORINSTALLER // Release-ForInstaller configuration:
            Log.Info($"Volume Control version {this.CurrentVersion} (Installed)");
#       endif
            FLog.Info(GetWindowsVersion());

            FLog.Debug($"{nameof(VCSettings)} initialization completed.");
        }
        #endregion Constructors

        #region Properties
        #region Statics
        private static Config Settings => (AppConfig.Configuration.Default as Config)!;
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
        #endregion ReadOnlyProperties
        /// <inheritdoc cref="Config.ShowIcons"/>
        public bool ShowIcons
        {
            get => Settings.ShowIcons;
            set => Settings.ShowIcons = value;
        }
        /// <inheritdoc cref="Config.DeleteHotkeyConfirmation"/>
        public bool DeleteHotkeyConfirmation
        {
            get => Settings.DeleteHotkeyConfirmation;
            set => Settings.DeleteHotkeyConfirmation = value;
        }
        /// <inheritdoc cref="Config.RunAtStartup"/>
        public bool? RunAtStartup
        {
            get => !RunAtStartupHelper.ValueEquals(this.ExecutablePath) && !RunAtStartupHelper.ValueEqualsNull() ? null : Settings.RunAtStartup;
            set => RunAtStartupHelper.Value = (Settings.RunAtStartup = value ?? false) ? this.ExecutablePath : null;
        }
        /// <inheritdoc cref="Config.StartMinimized"/>
        public bool StartMinimized
        {
            get => Settings.StartMinimized;
            set => Settings.StartMinimized = value;
        }
        /// <inheritdoc cref="Config.CheckForUpdates"/>
        public bool CheckForUpdates
        {
            get => Settings.CheckForUpdates;
            set => Settings.CheckForUpdates = value;
        }
        /// <inheritdoc cref="Config.ShowUpdatePrompt"/>
        public bool ShowUpdateMessageBox
        {
            get => Settings.ShowUpdatePrompt;
            set => Settings.ShowUpdatePrompt = value;
        }
        public ObservableImmutableList<string> CustomLocalizationDirectories
        {
            get => Settings.CustomLocalizationDirectories;
            set => Settings.CustomLocalizationDirectories = value;
        }
        public ObservableImmutableList<string> CustomAddonDirectories
        {
            get => Settings.CustomAddonDirectories;
            set => Settings.CustomAddonDirectories = value;
        }
        public int PeakMeterUpdateIntervalMs
        {
            get => Settings.PeakMeterUpdateIntervalMs;
            set => Settings.PeakMeterUpdateIntervalMs = value;
        }
        public bool ShowPeakMeters
        {
            get => Settings.ShowPeakMeters;
            set => Settings.ShowPeakMeters = value;
        }
        public bool EnableDeviceControl
        {
            get => Settings.EnableDeviceControl;
            set => Settings.EnableDeviceControl = value;
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
        public bool AllowMultipleDistinctInstances
        {
            get => Settings.AllowMultipleDistinctInstances;
            set => Settings.AllowMultipleDistinctInstances = value;
        }
        public bool KeepRelativePosition
        {
            get => Settings.KeepRelativePosition;
            set => Settings.KeepRelativePosition = value;
        }
        public int VolumeStepSize
        {
            get => Settings.VolumeStepSize;
            set => Settings.VolumeStepSize = value;
        }
        /// <inheritdoc cref="Config.SessionListNotificationConfig"/>
        public NotificationConfigSection SessionListNotificationConfig
        {
            get => Settings.SessionListNotificationConfig;
            set => Settings.SessionListNotificationConfig = value;
        }
        /// <inheritdoc cref="Config.DeviceListNotificationConfig"/>
        public NotificationConfigSection DeviceListNotificationConfig
        {
            get => Settings.DeviceListNotificationConfig;
            set => Settings.DeviceListNotificationConfig = value;
        }
        /// <inheritdoc cref="Config.NotificationMoveRequiresAlt"/>
        public bool NotificationDragRequiresAlt
        {
            get => Settings.NotificationMoveRequiresAlt;
            set => Settings.NotificationMoveRequiresAlt = value;
        }
        /// <inheritdoc cref="Config.NotificationSavePos"/>
        public bool NotificationSavesPosition
        {
            get => Settings.NotificationSavePos;
            set => Settings.NotificationSavePos = value;
        }
        /// <inheritdoc cref="Config.LockTargetSession"/>
        public bool LockTargetSession
        {
            get => Settings.LockTargetSession;
            set => Settings.LockTargetSession = value;
        }
        /// <inheritdoc cref="Config.LockTargetDevice"/>
        public bool LockTargetDevice
        {
            get => Settings.LockTargetDevice;
            set => Settings.LockTargetDevice = value;
        }
        /// <summary>
        /// This is read-only since there wouldn't be a way for volume control to find the config again after restarting
        /// </summary>
        public string ConfigLocation => System.IO.Path.Combine(Environment.CurrentDirectory, Settings.Location);
        /// <inheritdoc cref="Config.HiddenSessionProcessNames"/>
        public ObservableImmutableList<string> HiddenSessionProcessNames
        {
            get => Settings.HiddenSessionProcessNames;
            set => Settings.HiddenSessionProcessNames = value;
        }
        public bool LogEnabled
        {
            get => Settings.EnableLogging;
            set => Settings.EnableLogging = value;
        }
        public string LogFilePath
        {
            get => Settings.LogPath;
            set => Settings.LogPath = value;
        }
        #endregion Properties

        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void ForceNotifyPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Methods
        private static string GetExecutablePath() => Process.GetCurrentProcess().MainModule?.FileName is string path
                ? path
                : throw new Exception($"{nameof(VCSettings)} Error:  Retrieving the current executable path failed!");
        private static string GetWindowsVersion()
        {
            using ManagementObjectSearcher searcher = new("SELECT Caption,OSArchitecture FROM Win32_OperatingSystem");
            var info = searcher.Get();
            if (info == null) return string.Empty;

            string version = string.Empty;
            foreach (var obj in info)
            {
                version = $"{obj["Caption"]} ({obj["OSArchitecture"]})";
            }
            return version;
        }
        #endregion Methods
    }
}
