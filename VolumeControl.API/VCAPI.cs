using VolumeControl.Audio;
using VolumeControl.Hotkeys;
using VolumeControl.Log;

namespace VolumeControl.API
{
    /// <summary>Struct containing various useful object references.<br/>Cannot be directly instantiated, use the <see cref="Default"/> static property.</summary>
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
        /// This is the global <see cref="AudioAPI"/>.
        /// </summary>
        public AudioAPI AudioAPI { get; internal set; }
        /// <summary>
        /// This is the global <see cref="HotkeyManager"/>.
        /// </summary>
        public HotkeyManager HotkeyManager { get; internal set; }
        /// <summary>
        /// This is the handle of the main window, for use with Windows API methods in <see cref="User32"/> or elsewhere.
        /// </summary>
        public IntPtr MainWindowHWnd { get; internal set; }
        #endregion Properties
    }
}