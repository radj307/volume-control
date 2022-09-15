using Semver;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using VolumeControl.Core;
using VolumeControl.Helpers.Win32;
using VolumeControl.Log;
using VolumeControl.WPF;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Helpers
{
    public abstract class VCSettings : INotifyPropertyChanged, INotifyCollectionChanged
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
            this.CurrentVersion = Settings.__VERSION__;
            this.CurrentVersionString = this.CurrentVersion.ToString();
            Log.Debug($"{nameof(VCSettings)}.{nameof(this.CurrentVersion)} = '{this.CurrentVersionString}'");

            this.RunAtStartup = RunAtStartupHelper.ValueEquals(this.ExecutablePath);

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
        /// <summary>
        /// This is read-only since there wouldn't be a way for volume control to find the config again after restarting
        /// </summary>
        public string ConfigLocation => System.IO.Path.Combine(Environment.CurrentDirectory, Settings.Location);
        #endregion Properties

        #region Events
#pragma warning disable CS0067 // The event 'BindableHotkey.PropertyChanged' is never used ; This is automatically used by Fody.
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        public abstract event NotifyCollectionChangedEventHandler? CollectionChanged;
#pragma warning restore CS0067 // The event 'BindableHotkey.PropertyChanged' is never used ; This is automatically used by Fody.
        #endregion Events

        #region Methods
        private static string GetExecutablePath() => Process.GetCurrentProcess().MainModule?.FileName is string path
                ? path
                : throw new Exception($"{nameof(VCSettings)} Error:  Retrieving the current executable path failed!");
        #endregion Methods
    }
}
