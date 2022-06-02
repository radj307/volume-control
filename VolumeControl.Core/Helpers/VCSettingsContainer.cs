using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Win32;
using VolumeControl.Log;
using VolumeControl.Log.Enum;
using VolumeControl.WPF;

namespace VolumeControl.Core.Helpers
{
    /// <summary>
    /// Responsible for containing the runtime state settings.
    /// </summary>
    public class VCSettingsContainer : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Constructors
        /// <inheritdoc cref="VCSettingsContainer"/>
        public VCSettingsContainer(string executablePath)
        {
            _registryRunKeyHelper = new();
            ExecutablePath = executablePath;
            Hook = new();
        }
        #endregion Constructors

        #region Helpers
        private readonly RunKeyHelper _registryRunKeyHelper;
        private const string _registryVCRunAtStartupValueName = "VolumeControl";
        /// <summary>The location of the executable in the local filesystem.</summary>
        public readonly string ExecutablePath;
        private static LogWriter Log => FLog.Log;
        #endregion Helpers

        #region Fields
        /// <summary>
        /// The window message hook handler object.
        /// </summary>
        public readonly HWndHook Hook;
        #endregion Fields

        #region Settings
        /// <summary>
        /// Gets or sets a boolean that determines whether or not device/session icons are shown in the UI.
        /// </summary>
        public bool ShowIcons
        {
            get => _showIcons;
            set
            {
                NotifyPropertyChanging();
                _showIcons = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanging();
                _advancedHotkeyMode = value;
                NotifyPropertyChanged();
            }
        }
        private bool _advancedHotkeyMode;
        /// <summary>
        /// Gets or sets whether the application should run when Windows starts.<br/>
        /// Creates/deletes a registry value in <b>HKEY_CURRENT_USER => SOFTWARE\Microsoft\Windows\CurrentVersion\Run</b>
        /// </summary>
        public bool RunAtStartup
        {
            get => _registryRunKeyHelper.CheckRunAtStartup(_registryVCRunAtStartupValueName, ExecutablePath);
            set
            {
                try
                {
                    NotifyPropertyChanging();

                    if (value)
                    {
                        _registryRunKeyHelper.EnableRunAtStartup(_registryVCRunAtStartupValueName, ExecutablePath);
                        Log.Conditional(new(EventType.DEBUG, $"Enabled Run at Startup: {{ Value: {_registryVCRunAtStartupValueName}, Path: {ExecutablePath} }}"), new(EventType.INFO, "Enabled Run at Startup."));
                    }
                    else
                    {
                        _registryRunKeyHelper.DisableRunAtStartup(_registryVCRunAtStartupValueName);
                        Log.Info("Disabled Run at Startup.");
                    }

                    NotifyPropertyChanged();
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to create run at startup registry key!", $"{{ Value: '{_registryVCRunAtStartupValueName}', Path: '{ExecutablePath}' }}", ex);
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
                NotifyPropertyChanging();
                _startMinimized = value;
                NotifyPropertyChanged();
            }
        }
        private bool _startMinimized;
        /// <summary>
        /// Gets or sets whether the program should check for updates on startup.
        /// </summary>
        public bool CheckForUpdates
        {
            get => _checkForUpdates;
            set
            {
                NotifyPropertyChanging();
                _checkForUpdates = value;
                NotifyPropertyChanged();
            }
        }
        private bool _checkForUpdates;
        /// <summary>
        /// Gets or sets whether the program should include pre-release versions when checking for updates.
        /// </summary>
        public bool? AllowUpdateToPreRelease
        {
            get => _allowUpdateToPreRelease;
            set
            {
                NotifyPropertyChanging();
                _allowUpdateToPreRelease = value;
                NotifyPropertyChanged();
            }
        }
        private bool? _allowUpdateToPreRelease;
        /// <summary>
        /// Gets or sets whether a message box prompt is shown informing the user of an available update.
        /// </summary>
        /// <remarks>When this is disabled, updates are notified through the caption bar.</remarks>
        public bool ShowUpdateMessageBox
        {
            get => _showUpdateMessageBox;
            set
            {
                NotifyPropertyChanging();
                _showUpdateMessageBox = value;
                NotifyPropertyChanged();
            }
        }
        private bool _showUpdateMessageBox;
        /// <summary>
        /// Gets or sets whether notifications are enabled or not.
        /// </summary>
        public bool NotificationEnabled
        {
            get => _notificationEnabled;
            set
            {
                NotifyPropertyChanging();
                _notificationEnabled = value;
                NotifyPropertyChanged();
            }
        }
        private bool _notificationEnabled;
        /// <summary>
        /// The amount of time, in milliseconds, that the notification window stays visible for before disappearing.
        /// </summary>
        public int NotificationTimeout
        {
            get => _notificationTimeout;
            set
            {
                NotifyPropertyChanging();
                _notificationTimeout = value;
                NotifyPropertyChanged();
            }
        }
        private int _notificationTimeout;
        /// <summary>
        /// This partially controls what the notification window actually displays.
        /// </summary>
        public DisplayTarget NotificationMode
        {
            get => _notificationMode;
            set
            {
                NotifyPropertyChanging();
                _notificationMode = value;
                NotifyPropertyChanged();
            }
        }
        private DisplayTarget _notificationMode = DisplayTarget.Sessions;
        /// <summary>Gets or sets whether or not the list notification window appears for volume change events.</summary>
        public bool NotificationShowsVolumeChange
        {
            get => _notificationShowsVolumeChange;
            set
            {
                NotifyPropertyChanging();
                _notificationShowsVolumeChange = value;
                NotifyPropertyChanged();
            }
        }
        private bool _notificationShowsVolumeChange;
        #endregion Settings

        #region Events
        /// <summary>Triggered when a property was set.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>Triggered when a property will be set.</summary>
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <summary>Triggers the <see cref="PropertyChanged"/> event.</summary>
        /// <param name="propertyName">The name of the property that was changed; or leave blank to automatically fill this in using the <see cref="CallerMemberNameAttribute"/> attribute.</param>
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new(propertyName));
            Log.Debug($"{GetType().FullName}::{propertyName} was changed:", GetType().GetProperty(propertyName)?.GetValue(this));
        }
        /// <summary>Triggers the <see cref="PropertyChanging"/> event.</summary>
        /// <param name="propertyName">The name of the property that is changing; or leave blank to automatically fill this in using the <see cref="CallerMemberNameAttribute"/> attribute.</param>
        protected virtual void NotifyPropertyChanging([CallerMemberName] string propertyName = "") => PropertyChanging?.Invoke(this, new(propertyName));
        #endregion Events
    }
}
