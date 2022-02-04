using System.Diagnostics;
using System.Runtime.InteropServices;
using ToastifyAPI.Native.MMDeviceAPI;

namespace VolumeControl
{
    public static class VolumeHelper
    {
        #region Static Members

        public static void IncrementVolume(string proc_name, decimal increment)
        {
            try
            {
                decimal currentVolume = GetVolume(proc_name);
                decimal newVolume = currentVolume + increment;
                SetVolume(proc_name, newVolume >= 100.0m ? 100.0m : newVolume);
            }
            catch
            {
                Win32API.SendMediaKey(ToastifyActionEnum.VolumeUp);
            }
        }

        public static void DecrementVolume(string proc_name, decimal increment)
        {
            try
            {
                decimal currentVolume = GetVolume(proc_name);
                decimal newVolume = currentVolume - increment;
                SetVolume(proc_name, newVolume <= 0.0m ? 0.0m : newVolume);
            }
            catch
            {
                Win32API.SendMediaKey(ToastifyActionEnum.VolumeDown);
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
                Win32API.SendMediaKey(ToastifyActionEnum.Mute);
            }
        }

        internal static decimal GetVolume(string proc_name)
        {
            ISimpleAudioVolume volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            volume.GetMasterVolume(out float level);
            Marshal.ReleaseComObject(volume);
            return (decimal)level * 100m;
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

        internal static void SetVolume(string proc_name, decimal level)
        {
            ISimpleAudioVolume volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            Guid guid = Guid.Empty;
            volume.SetMasterVolume((float)(level / 100.0m), ref guid);
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