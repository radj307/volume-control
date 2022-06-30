using VolumeControl.Audio;
using VolumeControl.Core;
using VolumeControl.Hotkeys;

namespace VolumeControl.API.Internal
{
    /// <summary>Initializes <see cref="VCAPI.Default"/>.</summary>
    public static class Initializer
    {
        private static bool _initialized = false;

        /// <inheritdoc cref="Initializer"/>
        /// <param name="audioAPI">The audio API object.</param>
        /// <param name="mgr">The hotkey manager object.</param>
        /// <param name="mainWindowHWnd">The mixer window's handle</param>
        /// <param name="settings">The program settings container object.</param>
        /// <exception cref="InvalidOperationException">Initialize was already called previously.</exception>
        public static void Initialize(AudioAPI audioAPI, HotkeyManager mgr, IntPtr mainWindowHWnd, Config settings)
        {
            if (_initialized)
                throw new InvalidOperationException("Cannot re-initialize VolumeControl.API!");
            _initialized = true;

            VCAPI.Default = new()
            {
                AudioAPI = audioAPI,
                HotkeyManager = mgr,
                MainWindowHWnd = mainWindowHWnd,
                Settings = settings
            };
        }
    }
}
