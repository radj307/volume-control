using AudioAPI.WindowsAPI;
using AudioAPI.WindowsAPI.Audio;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AudioAPI
{
    public static class VolumeHelper
    {
        #region StaticMembers
        #region ByProcessName
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
                // send media volume up keypress globally
                User32.KeyboardEvent(VirtualKeyCode.VK_VOLUME_UP, 0, 1, IntPtr.Zero);
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
                User32.KeyboardEvent(VirtualKeyCode.VK_VOLUME_DOWN, 0, 1, IntPtr.Zero);
            }
        }

        public static void ToggleMute(string proc_name)
        {
            try
            {
                if (!System.VersionGreaterThan(6, 1))
                    throw new NotSupportedException($"This application requires Windows 7 or newer!");

                SetMute(proc_name, !IsMuted(proc_name));
            }
            catch
            {
                User32.KeyboardEvent(VirtualKeyCode.VK_VOLUME_MUTE, 0, 1, IntPtr.Zero);
            }
        }

        public static decimal GetVolume(string proc_name)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException($"Volume object creation failed for target \"{proc_name}\"");

            volume.GetMasterVolume(out float level);
            Marshal.ReleaseComObject(volume);
            return (decimal)level * 100m;
        }

        public static bool TryGetVolume(string proc_name, out decimal volume)
        {
            volume = 0m;
            ISimpleAudioVolume? volObj = GetVolumeObject(proc_name);
            if (volObj == null)
                return false;
            volObj.GetMasterVolume(out float level);
            Marshal.ReleaseComObject(volObj);
            volume = Convert.ToDecimal(level);
            return true;
        }

        public static bool IsMuted(string proc_name)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            volume.GetMute(out bool mute);
            Marshal.ReleaseComObject(volume);
            return mute;
        }

        public static bool TryIsMuted(string proc_name, out bool muted)
        {
            muted = false;
            ISimpleAudioVolume? volObj = GetVolumeObject(proc_name);
            if (volObj == null)
                return false;
            volObj.GetMute(out muted);
            Marshal.ReleaseComObject(volObj);
            return true;
        }

        public static void SetVolume(string proc_name, decimal level)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            Guid guid = Guid.Empty;
            volume.SetMasterVolume((float)( level / 100.0m ), ref guid);
            Marshal.ReleaseComObject(volume);
        }

        public static void SetMute(string proc_name, bool mute)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            Guid guid = Guid.Empty;
            volume.SetMute(mute, ref guid);
            Marshal.ReleaseComObject(volume);
        }

        public static ISimpleAudioVolume? GetVolumeObject(string proc_name)
        {
            var audioVolumeObjects = from p in Process.GetProcessesByName(proc_name)
                                     let vol = Volume.GetVolumeObject(p.Id)
                                     where vol != null
                                     select vol;
            return audioVolumeObjects.FirstOrDefault();
        }
        #endregion ByProcessName
        #region ByProcessID
        public static void IncrementVolume(int proc_id, decimal increment)
        {
            try
            {
                decimal currentVolume = GetVolume(proc_id);
                decimal newVolume = currentVolume + increment;
                SetVolume(proc_id, newVolume >= 100.0m ? 100.0m : newVolume);
            }
            catch
            {
                // send media volume up keypress globally
                User32.KeyboardEvent(VirtualKeyCode.VK_VOLUME_UP, 0, 1, IntPtr.Zero);
            }
        }

        public static void DecrementVolume(int proc_id, decimal increment)
        {
            try
            {
                decimal currentVolume = GetVolume(proc_id);
                decimal newVolume = currentVolume - increment;
                SetVolume(proc_id, newVolume <= 0.0m ? 0.0m : newVolume);
            }
            catch
            {
                User32.KeyboardEvent(VirtualKeyCode.VK_VOLUME_DOWN, 0, 1, IntPtr.Zero);
            }
        }

        public static void ToggleMute(int proc_id)
        {
            try
            {
                if (!System.VersionGreaterThan(6, 1))
                    throw new NotSupportedException("This feature is only available on Windows 7 or newer");

                SetMute(proc_id, !IsMuted(proc_id));
            }
            catch
            {
                User32.KeyboardEvent(VirtualKeyCode.VK_VOLUME_MUTE, 0, 1, IntPtr.Zero);
            }
        }

        public static decimal GetVolume(int proc_id)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_id);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            volume.GetMasterVolume(out float level);
            Marshal.ReleaseComObject(volume);
            return (decimal)level * 100m;
        }

        public static bool IsMuted(int proc_id)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_id);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            volume.GetMute(out bool mute);
            Marshal.ReleaseComObject(volume);
            return mute;
        }

        public static void SetVolume(int proc_id, decimal level)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_id);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            Guid guid = Guid.Empty;
            volume.SetMasterVolume((float)( level / 100.0m ), ref guid);
            Marshal.ReleaseComObject(volume);
        }

        public static void SetMute(int proc_id, bool mute)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_id);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            Guid guid = Guid.Empty;
            volume.SetMute(mute, ref guid);
            Marshal.ReleaseComObject(volume);
        }

        public static ISimpleAudioVolume? GetVolumeObject(int proc_id) => Volume.GetVolumeObject(proc_id);

        #endregion ByProcessID
        #endregion StaticMembers
    }
}