using Semver;
using System.ComponentModel;
using VolumeControl.Core.Enum;
using VolumeControl.WPF;

namespace VolumeControl.Core.Helpers
{
    /// <summary>
    /// Represents the public settings used by Volume Control.<br/>
    /// These are also used as binding sources for the GUI.
    /// </summary>
    public interface IVCSettings : INotifyPropertyChanged
    {
        #region Properties
        #region ReadOnlyProperties
        /// <summary>
        /// The main volume control window's handle.
        /// </summary>
        public IntPtr MainWindowHandle { get; }
        /// <summary>
        /// The windows message hook helper object associated with this Volume Control instance.<br/>
        /// This is used as the owner handle for hotkeys, as well as for handling windows messages.
        /// </summary>
        HWndHook HWndHook { get; }
        /// <summary>
        /// The filepath of `VolumeControl.exe` in the local filesystem.
        /// </summary>
        string ExecutablePath { get; }
        /// <summary>
        /// The current volume control version as a <see langword="string"/>.
        /// </summary>
        string CurrentVersionString { get; }
        /// <summary>
        /// The current volume control version as a <see cref="SemVersion"/>.
        /// </summary>
        SemVersion CurrentVersion { get; }
        /// <summary>
        /// The type of release that the current version represents.<br/>See <see cref="ERelease"/> for more information.
        /// </summary>
        ERelease ReleaseType { get; }
        #endregion ReadOnlyProperties
        /// <summary>
        /// Gets or sets a boolean that determines whether or not device/session icons are shown in the UI.
        /// </summary>
        bool ShowIcons { get; set; }
        /// <summary>
        /// Gets or sets the hotkey editor mode, which can be either false (basic mode) or true (advanced mode).
        /// </summary>
        /// <remarks>Advanced mode allows the user to perform additional actions in the hotkey editor:
        /// <list type="bullet">
        /// <item><description>Create and delete hotkeys.</description></item>
        /// <item><description>Change the action bindings of hotkeys.</description></item>
        /// <item><description>Rename hotkeys.</description></item>
        /// </list></remarks>
        bool AdvancedHotkeyMode { get; set; }
        /// <summary>
        /// Gets or sets whether the application should run when Windows starts.<br/>
        /// Creates/deletes a registry value in <b>HKEY_CURRENT_USER => SOFTWARE\Microsoft\Windows\CurrentVersion\Run</b>
        /// </summary>
        bool RunAtStartup { get; set; }
        /// <summary>
        /// Gets or sets whether the window should be minimized during startup.<br/>
        /// The window can be shown again later using the tray icon.
        /// </summary>
        bool StartMinimized { get; set; }
        /// <summary>
        /// Gets or sets whether the program should check for updates on startup.
        /// </summary>
        bool CheckForUpdates { get; set; }
        /// <summary>
        /// Gets or sets whether the program should include pre-release versions when checking for updates.
        /// </summary>
        bool? AllowUpdateToPreRelease { get; set; }
        /// <summary>
        /// Gets or sets whether a message box prompt is shown informing the user of an available update.
        /// </summary>
        /// <remarks>When this is disabled, updates are notified through the caption bar.</remarks>
        bool ShowUpdateMessageBox { get; set; }
        /// <summary>
        /// Gets or sets whether notifications are enabled or not.
        /// </summary>
        bool NotificationEnabled { get; set; }
        /// <summary>
        /// The amount of time, in milliseconds, that the notification window stays visible for before disappearing.
        /// </summary>
        int NotificationTimeout { get; set; }
        /// <summary>
        /// This partially controls what the notification window actually displays.
        /// </summary>
        DisplayTarget NotificationMode { get; set; }
        /// <summary>
        /// Gets or sets whether or not the list notification window appears for volume change events.
        /// </summary>
        bool NotificationShowsVolumeChange { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Saves the current setting values.
        /// </summary>
        void Save();
        /// <summary>
        /// Loads the last-saved setting values, overwriting all current values.
        /// </summary>
        void Load();
        #endregion Methods
    }
}
