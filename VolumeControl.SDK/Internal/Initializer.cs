﻿using VolumeControl.Core;
using VolumeControl.Core.Input;
using VolumeControl.CoreAudio;
using VolumeControl.Log;

namespace VolumeControl.SDK.Internal
{
    /// <summary>
    /// Initializes <see cref="VCAPI.Default"/>.
    /// </summary>
    /// <remarks>
    /// Do not attempt to access this from an addon!
    /// </remarks>
    public static class Initializer
    {
        private static bool _initialized = false;

        /// <inheritdoc cref="Initializer"/>
        /// <param name="audioDeviceManager">The audio device manager object.</param>
        /// <param name="audioDeviceSelector">The audio device selection manager object.</param>
        /// <param name="audioSessionManager">The audio session manager object.</param>
        /// <param name="audioSessionMultiSelector">The audio session selection manager object.</param>
        /// <param name="mgr">The hotkey manager object.</param>
        /// <param name="mainWindowHWnd">The mixer window's handle</param>
        /// <param name="settings">The program settings container object.</param>
        /// <exception cref="InvalidOperationException">Initialize was already called previously.</exception>
        public static VCAPI Initialize(
            AudioDeviceManager audioDeviceManager,
            AudioDeviceSelector audioDeviceSelector,
            AudioSessionManager audioSessionManager,
            AudioSessionMultiSelector audioSessionMultiSelector,
            HotkeyManager mgr,
            IntPtr mainWindowHWnd,
            Config settings)
        {
            if (_initialized)
            {
                FLog.Log.Error(new Exception($"{typeof(Initializer).FullName} was already initialized! This indicates that an addon called it, which is not allowed!"));
                return VCAPI.Default;
            }
            _initialized = true;

            return VCAPI.Default = new(
                audioDeviceManager,
                audioDeviceSelector,
                audioSessionManager,
                audioSessionMultiSelector,
                mgr,
                mainWindowHWnd,
                settings);
        }
    }
}
