using VolumeControl.Audio;
using VolumeControl.Hotkeys;
using VolumeControl.Log;

namespace VolumeControl.API
{
    /// <summary>Struct containing various useful object references.<br/>Cannot be directly instantiated, use the <see cref="Default"/> static property.</summary>
    public struct VCAPI
    {
        public static VCAPI Default { get; internal set; }
        public static LogWriter Log => FLog.Log;

        public AudioAPI AudioAPI { get; internal set; }
        public HotkeyManager HotkeyManager { get; internal set; }
        public IntPtr MainWindowHWnd { get; internal set; }
    }
}