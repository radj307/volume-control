using System.Diagnostics;
using System.Runtime.InteropServices;
using ToastifyAPI.Native.MMDeviceAPI;

namespace VolumeControl
{
    public static class VolumeHelper
    {
        #region Static Members

        public static void IncrementVolume(string proc_name, float increment)
        {
            try
            {
                float currentVolume = GetVolume(proc_name);
                float newVolume = currentVolume + increment;
                SetVolume(proc_name, newVolume >= 100.0f ? 100.0f : newVolume);
            }
            catch
            {
                Keyboard.SendMediaKey(ToastifyActionEnum.VolumeUp);
            }
        }

        public static void DecrementVolume(string proc_name, float increment)
        {
            try
            {
                float currentVolume = GetVolume(proc_name);
                float newVolume = currentVolume - increment;
                SetVolume(proc_name, newVolume <= 0.0f ? 0.0f : newVolume);
            }
            catch
            {
                Keyboard.SendMediaKey(ToastifyActionEnum.VolumeDown);
            }
        }

        public static void ToggleMute(string proc_name)
        {
            try
            {
                if (!ToastifyAPI.Helpers.System.IsOSCompatible(6, 1))
                    throw new NotSupportedException("This feature is only available on Windows 7 or newer");

                Mute(proc_name, !IsMuted(proc_name));
            }
            catch
            {
                Keyboard.SendMediaKey(ToastifyActionEnum.Mute);
            }
        }

        internal static float GetVolume(string proc_name)
        {
            ISimpleAudioVolume volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            volume.GetMasterVolume(out float level);
            Marshal.ReleaseComObject(volume);
            return level * 100;
        }

        internal static bool IsMuted(string proc_name)
        {
            ISimpleAudioVolume volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            volume.GetMute(out bool mute);
            Marshal.ReleaseComObject(volume);
            return mute;
        }

        internal static void SetVolume(string proc_name, float level)
        {
            ISimpleAudioVolume volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            Guid guid = Guid.Empty;
            volume.SetMasterVolume(level / 100, ref guid);
            Marshal.ReleaseComObject(volume);
        }

        internal static void Mute(string proc_name, bool mute)
        {
            ISimpleAudioVolume volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            Guid guid = Guid.Empty;
            volume.SetMute(mute, ref guid);
            Marshal.ReleaseComObject(volume);
        }

        private static ISimpleAudioVolume GetVolumeObject(string proc_name)
        {
            var audioVolumeObjects = from p in Process.GetProcessesByName(proc_name)
                                     let vol = Volume.GetVolumeObject(p.Id)
                                     where vol != null
                                     select vol;
            return audioVolumeObjects.FirstOrDefault();
        }

        #endregion
    }
}