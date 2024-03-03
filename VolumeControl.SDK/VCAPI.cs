using VolumeControl.Core;
using VolumeControl.Core.Input;
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
        internal VCAPI(AudioDeviceManager audioDeviceManager, AudioDeviceSelector audioDeviceSelector, AudioSessionManager audioSessionManager, AudioSessionMultiSelector audioSessionMultiSelector, HotkeyManager hkManager, IntPtr MainHWnd, Config settings)
        {
            AudioDeviceManager = audioDeviceManager;
            AudioDeviceSelector = audioDeviceSelector;
            AudioSessionManager = audioSessionManager;
            AudioSessionMultiSelector = audioSessionMultiSelector;
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
        public static AsyncLogWriter Log => FLog.Log;
        #endregion Statics

        #region Properties
        /// <summary>
        /// Gets the audio device manager object.
        /// </summary>
        public AudioDeviceManager AudioDeviceManager { get; }
        /// <summary>
        /// Gets the audio device selection manager object.
        /// </summary>
        public AudioDeviceSelector AudioDeviceSelector { get; }
        /// <summary>
        /// Gets the audio session manager object.
        /// </summary>
        public AudioSessionManager AudioSessionManager { get; }
        /// <summary>
        /// Gets the audio session selection manager object.
        /// </summary>
        public AudioSessionMultiSelector AudioSessionMultiSelector { get; }
        /// <summary>
        /// Gets the hotkey manager object.
        /// </summary>
        public HotkeyManager HotkeyManager { get; }
        /// <summary>
        /// Gets the handle of the main window.
        /// </summary>
        /// <remarks>
        /// This is the handle of the main window, for use with Windows API methods in <see cref="User32"/> or elsewhere.
        /// </remarks>
        public IntPtr MainWindowHWnd { get; }
        /// <summary>
        /// Gets the application settings object.
        /// </summary>
        /// <remarks>
        /// This is the object that contains many of the runtime application settings.<br/>These are saved to the user configuration when the application shuts down.
        /// </remarks>
        public Config Settings { get; }
        #endregion Properties

        #region Methods

        #region Show...
        /// <summary>
        /// Display the SessionListNotification window.
        /// </summary>
        public void ShowSessionListNotification()
        {
            // don't trigger notification events if notifs are disabled-
            if (!Settings.SessionListNotificationConfig.Enabled) return;
            if (AudioSessionMultiSelector.NotificationIsEmpty()) return;

            AudioSessionMultiSelector.NotifyActiveSessionChanged(null);
            VCEvents.NotifyShowSessionListNotification(this, EventArgs.Empty);
        }
        /// <summary>
        /// Display the SessionListNotification window.
        /// </summary>
        public void ShowSessionListNotification(List<AudioSession> activeSessions)
        {
            // don't trigger notification events if notifs are disabled-
            if (!Settings.SessionListNotificationConfig.Enabled) return;

            AudioSessionMultiSelector.NotifyActiveSessionChanged(null);
            if (activeSessions != null)
            {
                foreach (var session in activeSessions)
                {
                    AudioSessionMultiSelector.NotifyActiveSessionChanged(session);
                }
            }
            VCEvents.NotifyShowSessionListNotification(this, EventArgs.Empty);
        }
        /// <summary>
        /// Display the SessionListNotification window.
        /// </summary>
        public void ShowSessionListNotification(AudioSession activeSession)
        {
            // don't trigger notification events if notifs are disabled-
            if (!Settings.SessionListNotificationConfig.Enabled) return;

            AudioSessionMultiSelector.NotifyActiveSessionChanged(null);
            AudioSessionMultiSelector.NotifyActiveSessionChanged(activeSession);
            VCEvents.NotifyShowSessionListNotification(this, EventArgs.Empty);
        }
        /// <summary>
        /// Display the DeviceListNotification window.
        /// </summary>
        public void ShowDeviceListNotification()
        {
            if (!Settings.DeviceListNotificationConfig.Enabled) return;

            VCEvents.NotifyShowDeviceListNotification(this, EventArgs.Empty);
        }
        /// <summary>
        /// Display the Mixer window.
        /// </summary>
        public void ShowMixer()
        {
            VCEvents.NotifyShowMixer(this, EventArgs.Empty);
        }
        #endregion Show...

        #endregion Methods
    }
}
