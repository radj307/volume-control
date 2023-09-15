using VolumeControl.Core;
using VolumeControl.CoreAudio;
using VolumeControl.Log;
using VolumeControl.SDK.Internal;

namespace VolumeControl.SDK
{
    /// <summary>
    /// The primary interaction point for the Volume Control API.<br/>
    /// See the <see cref="VCAPI.Default"/> property for more information.<br/>
    /// </summary>
    /// <remarks>
    /// Have an idea for extending the Volume Control API? We'd love to hear it! Let us know with a <see href="https://github.com/radj307/volume-control/issues/new?assignees=&amp;labels=&amp;template=feature-request.md&amp;title=%5BREQUEST%5D">Feature Request</see>, or by submitting a <see href="https://github.com/radj307/volume-control/compare">Pull Request</see>.
    /// </remarks>
    public class VCAPI
    {
        #region Initializer
        internal VCAPI(AudioDeviceManager audioDeviceManager, AudioDeviceSelector audioDeviceSelector, AudioSessionManager audioSessionManager, AudioSessionSelector audioSessionSelector, HotkeyManager hkManager, IntPtr MainHWnd, Config settings)
        {
            AudioDeviceManager = audioDeviceManager;
            AudioDeviceSelector = audioDeviceSelector;
            AudioSessionManager = audioSessionManager;
            AudioSessionSelector = audioSessionSelector;
            HotkeyManager = hkManager;
            MainWindowHWnd = MainHWnd;
            Settings = settings;
        }
        #endregion Initializer

        #region Statics
        /// <summary>
        /// This is the default instance of VCAPI.<br/>You must use this to access the API.
        /// </summary>
        public static VCAPI Default { get; internal set; } = null!;
        /// <summary>
        /// This is the default log instance.
        /// </summary>
        public static LogWriter Log => FLog.Log;
        #endregion Statics

        #region Properties
        /// <summary>
        /// This is the global <see cref="CoreAudio.AudioDeviceManager"/> responsible for managing <see cref="AudioDevice"/> instances.
        /// </summary>
        public AudioDeviceManager AudioDeviceManager { get; }
        /// <summary>
        /// Manages the selected <see cref="AudioDevice"/> instance for the <see cref="AudioDeviceManager"/>.
        /// </summary>
        public AudioDeviceSelector AudioDeviceSelector { get; }
        /// <summary>
        /// This is the global <see cref="CoreAudio.AudioSessionManager"/> responsible for managing <see cref="AudioSession"/> instances.
        /// </summary>
        public AudioSessionManager AudioSessionManager { get; }
        /// <summary>
        /// Manages the selected <see cref="AudioSession"/> instance for the <see cref="AudioSessionManager"/>.
        /// </summary>
        public AudioSessionSelector AudioSessionSelector { get; }
        /// <summary>
        /// This is the global <see cref="Core.HotkeyManager"/> responsible for managing hotkeys.
        /// </summary>
        public HotkeyManager HotkeyManager { get; }
        /// <summary>
        /// This is the handle of the main window, for use with Windows API methods in <see cref="User32"/> or elsewhere.
        /// </summary>
        public IntPtr MainWindowHWnd { get; }
        /// <summary>
        /// This is the object that contains many of the runtime application settings.<br/>These are saved to the user configuration when the application shuts down.
        /// </summary>
        public Config Settings { get; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Display the SessionListNotification window.
        /// </summary>
        public void ShowSessionListNotification()
        {
            // don't trigger notification events if notifs are disabled-
            if (!Settings.NotificationsEnabled) return;

            VCEvents.NotifyShowSessionListNotification(this, EventArgs.Empty);
        }
        #endregion Methods
    }
}