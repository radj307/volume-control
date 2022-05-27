using VolumeControl.Audio;
using VolumeControl.Core.Helpers;
using VolumeControl.Hotkeys;
using VolumeControl.Log;

namespace VolumeControl.API
{
    /// <summary>Struct containing various useful object references.<br/>Cannot be directly instantiated, use the <see cref="Default"/> static property.</summary>
    /// <remarks>Have an idea for extending the Volume Control API?  <see href="https://github.com/radj307/volume-control/issues/new?assignees=&amp;labels=&amp;template=feature-request.md&amp;title=%5BREQUEST%5D">Request a Feature</see> or <see href="https://github.com/radj307/volume-control/compare">Submit a Pull Request</see></remarks>
    public struct VCAPI
    {
        #region Statics
        /// <summary>
        /// This is the default instance of VCAPI.<br/>You must use this to access the API.
        /// </summary>
        public static VCAPI Default { get; internal set; }
        /// <summary>
        /// This is the default log instance.
        /// </summary>
        public static LogWriter Log => FLog.Log;
        #endregion Statics

        #region Properties
        /// <summary>
        /// This is the global <see cref="Audio.AudioAPI"/>.
        /// </summary>
        public AudioAPI AudioAPI { get; internal set; }
        /// <summary>
        /// This is the global <see cref="Hotkeys.HotkeyManager"/>.
        /// </summary>
        public HotkeyManager HotkeyManager { get; internal set; }
        /// <summary>
        /// This is the handle of the main window, for use with Windows API methods in <see cref="User32"/> or elsewhere.
        /// </summary>
        public IntPtr MainWindowHWnd { get; internal set; }
        /// <summary>
        /// This is the object that contains many of the runtime application settings.<br/>These are saved to the user configuration when the application shuts down.
        /// </summary>
        public VCSettingsContainer Settings { get; internal set; }
        #endregion Properties
    }
}