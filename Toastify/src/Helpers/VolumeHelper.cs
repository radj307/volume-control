using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Toastify.Core;
using ToastifyAPI.Helpers;
using ToastifyAPI.Native.MMDeviceAPI;
using Spotify = ToastifyAPI.Spotify;

namespace Toastify.Helpers
{
    public static class VolumeHelper
    {
        #region Static Members

        public static void IncrementVolume(float increment)
        {
            try
            {
                float currentVolume = GetSpotifyVolume();
                float newVolume = currentVolume + increment;
                SetSpotifyVolume(newVolume >= 100.0f ? 100.0f : newVolume);
            }
            catch
            {
                Win32API.SendMediaKey(ToastifyActionEnum.VolumeUp);
            }
        }

        public static void DecrementVolume(float increment)
        {
            try
            {
                float currentVolume = GetSpotifyVolume();
                float newVolume = currentVolume - increment;
                SetSpotifyVolume(newVolume <= 0.0f ? 0.0f : newVolume);
            }
            catch
            {
                Win32API.SendMediaKey(ToastifyActionEnum.VolumeDown);
            }
        }

        public static void ToggleMute()
        {
            try
            {
                if (!ToastifyAPI.Helpers.System.IsOSCompatible(6, 1))
                    throw new NotSupportedException("This feature is only available on Windows 7 or newer");

                MuteSpotify(!IsSpotifyMuted());
            }
            catch
            {
                Win32API.SendMediaKey(ToastifyActionEnum.Mute);
            }
        }

        internal static float GetSpotifyVolume()
        {
            ISimpleAudioVolume volume = GetSpotifyVolumeObject();
            if (volume == null)
                throw new COMException("Volume object creation failed");

            volume.GetMasterVolume(out float level);
            Marshal.ReleaseComObject(volume);
            return level * 100;
        }

        internal static bool IsSpotifyMuted()
        {
            ISimpleAudioVolume volume = GetSpotifyVolumeObject();
            if (volume == null)
                throw new COMException("Volume object creation failed");

            volume.GetMute(out bool mute);
            Marshal.ReleaseComObject(volume);
            return mute;
        }

        internal static void SetSpotifyVolume(float level)
        {
            ISimpleAudioVolume volume = GetSpotifyVolumeObject();
            if (volume == null)
                throw new COMException("Volume object creation failed");

            Guid guid = Guid.Empty;
            volume.SetMasterVolume(level / 100, ref guid);
            Marshal.ReleaseComObject(volume);
        }

        internal static void MuteSpotify(bool mute)
        {
            ISimpleAudioVolume volume = GetSpotifyVolumeObject();
            if (volume == null)
                throw new COMException("Volume object creation failed");

            Guid guid = Guid.Empty;
            volume.SetMute(mute, ref guid);
            Marshal.ReleaseComObject(volume);
        }

        private static ISimpleAudioVolume GetSpotifyVolumeObject()
        {
            var audioVolumeObjects = from p in Process.GetProcessesByName(Spotify.ProcessName)
                                     let vol = Volume.GetVolumeObject(p.Id)
                                     where vol != null
                                     select vol;
            return audioVolumeObjects.FirstOrDefault();
        }

        #endregion
    }
}