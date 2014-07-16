using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Toastify
{
    class VolumeHelper
    {
        // base code from: http://stackoverflow.com/a/14322736 

        public static void IncrementVolume(string name)
        {
            var curVolume = GetApplicationVolume(name);

            if (curVolume != null && curVolume <100)
                SetApplicationVolume(name, (float)curVolume + 2);
        }

        public static void DecrementVolume(string name)
        {
            var curVolume = GetApplicationVolume(name);

            if (curVolume != null && curVolume > 0)
                SetApplicationVolume(name, (float)curVolume - 2);
        }

        public static float? GetApplicationVolume(string name)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return null;

            float level;
            volume.GetMasterVolume(out level);
            return level * 100;
        }

        public static bool? GetApplicationMute(string name)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return null;

            bool mute;
            volume.GetMute(out mute);
            return mute;
        }

        public static void SetApplicationVolume(string name, float level)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return;

            Guid guid = Guid.Empty;
            volume.SetMasterVolume(level / 100, ref guid);
        }

        internal static void ToggleApplicationMute(string name)
        {

            var muted = GetApplicationMute(name);

            if (muted == null)
                return;

            SetApplicationMute(name, !(bool)muted);
        }


        public static void SetApplicationMute(string name, bool mute)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return;

            Guid guid = Guid.Empty;
            volume.SetMute(mute, ref guid);
        }

        public static IEnumerable<string> EnumerateApplications()
        {
            // get the speakers (1st render + multimedia) device
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            IMMDevice speakers;
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

            // activate the session manager. we need the enumerator
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            object o;
            speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            IAudioSessionEnumerator sessionEnumerator;
            mgr.GetSessionEnumerator(out sessionEnumerator);
            int count;
            sessionEnumerator.GetCount(out count);

            for (int i = 0; i < count; i++)
            {
                IAudioSessionControl ctl;
                IAudioSessionControl2 ctl2;

                sessionEnumerator.GetSession(i, out ctl);

                ctl2 = ctl as IAudioSessionControl2;

                if (ctl2 != null)
                {
                    uint pid = 0;
                    string sout1 = "";
                    string sout2 = "";

                    ctl2.GetSessionIdentifier(out sout1);
                    ctl2.GetProcessId(out pid);
                    ctl2.GetSessionInstanceIdentifier(out sout2);

                }

                string dn;
                ctl.GetDisplayName(out dn);
                yield return dn;
                Marshal.ReleaseComObject(ctl);
            }
            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);
        }

        private static ISimpleAudioVolume GetVolumeObject(string name)
        {
            // get the speakers (1st render + multimedia) device
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            IMMDevice speakers;
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

            // activate the session manager. we need the enumerator
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            object o;
            speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            IAudioSessionEnumerator sessionEnumerator;
            mgr.GetSessionEnumerator(out sessionEnumerator);
            int count;
            sessionEnumerator.GetCount(out count);

            // lower case name for easier comparison with the Session ID later on
            name = name.ToLower();

            // search for an audio session with the required name
            // Note: Since GetDisplayName only returns a real name if the application bothered to call SetDisplayName
            //       (which apps like Spotify do not), we instead use the SessionID (which usually includes the exe name)
            ISimpleAudioVolume volumeControl = null;

            for (int i = 0; i < count; i++)
            {

                IAudioSessionControl ctl;
                IAudioSessionControl2 ctl2;

                sessionEnumerator.GetSession(i, out ctl);

                ctl2 = ctl as IAudioSessionControl2;

                string dn;
                ctl.GetDisplayName(out dn);

                if (ctl2 != null)
                {
                    string sessionID = "";

                    ctl2.GetSessionIdentifier(out sessionID);

                    if (sessionID.ToLower().Contains(name)) 
                    {
                        volumeControl = ctl as ISimpleAudioVolume;
                        break;
                    }
                }

                if (ctl != null)
                    Marshal.ReleaseComObject(ctl);

                if (ctl2 != null)
                    Marshal.ReleaseComObject(ctl2);
            }

            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);

            return volumeControl;
        }

        //
        // Should probably be in the central WinHelper (or a NativeMethods) but it seemed cleaner to
        // keep it all together.
        //

        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        internal class MMDeviceEnumerator
        {
        }

        internal enum EDataFlow
        {
            eRender,
            eCapture,
            eAll,
            EDataFlow_enum_count
        }

        internal enum ERole
        {
            eConsole,
            eMultimedia,
            eCommunications,
            ERole_enum_count
        }

        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IMMDeviceEnumerator
        {
            int NotImpl1();

            [PreserveSig]
            int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);

            // the rest is not implemented
        }

        [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IMMDevice
        {
            [PreserveSig]
            int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);

            // the rest is not implemented
        }

        [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioSessionManager2
        {
            int NotImpl1();
            int NotImpl2();

            [PreserveSig]
            int GetSessionEnumerator(out IAudioSessionEnumerator SessionEnum);

            // the rest is not implemented
        }

        [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioSessionEnumerator
        {
            [PreserveSig]
            int GetCount(out int SessionCount);

            [PreserveSig]
            int GetSession(int SessionCount, out IAudioSessionControl Session);
        }

        [Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioSessionControl
        {
            int NotImpl1();

            [PreserveSig]
            int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

            // the rest is not implemented
        }

        public enum AudioSessionState
        {
            AudioSessionStateInactive = 0,
            AudioSessionStateActive = 1,
            AudioSessionStateExpired = 2
        }

        public enum AudioSessionDisconnectReason
        {
            DisconnectReasonDeviceRemoval = 0,
            DisconnectReasonServerShutdown = (DisconnectReasonDeviceRemoval + 1),
            DisconnectReasonFormatChanged = (DisconnectReasonServerShutdown + 1),
            DisconnectReasonSessionLogoff = (DisconnectReasonFormatChanged + 1),
            DisconnectReasonSessionDisconnected = (DisconnectReasonSessionLogoff + 1),
            DisconnectReasonExclusiveModeOverride = (DisconnectReasonSessionDisconnected + 1)
        }

        [Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionEvents
        {
            [PreserveSig]
            int OnDisplayNameChanged([MarshalAs(UnmanagedType.LPWStr)] string NewDisplayName, Guid EventContext);
            [PreserveSig]
            int OnIconPathChanged([MarshalAs(UnmanagedType.LPWStr)] string NewIconPath, Guid EventContext);
            [PreserveSig]
            int OnSimpleVolumeChanged(float NewVolume, bool newMute, Guid EventContext);
            [PreserveSig]
            int OnChannelVolumeChanged(UInt32 ChannelCount, IntPtr NewChannelVolumeArray, UInt32 ChangedChannel, Guid EventContext);
            [PreserveSig]
            int OnGroupingParamChanged(Guid NewGroupingParam, Guid EventContext);
            [PreserveSig]
            int OnStateChanged(AudioSessionState NewState);
            [PreserveSig]
            int OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason);
        }

        [Guid("BFB7FF88-7239-4FC9-8FA2-07C950BE9C6D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionControl2
        {
            [PreserveSig]
            int GetState(out AudioSessionState state);
            [PreserveSig]
            int GetDisplayName([Out(), MarshalAs(UnmanagedType.LPWStr)] out string name);
            [PreserveSig]
            int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string value, Guid EventContext);
            [PreserveSig]
            int GetIconPath([Out(), MarshalAs(UnmanagedType.LPWStr)] out string Path);
            [PreserveSig]
            int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string Value, Guid EventContext);
            [PreserveSig]
            int GetGroupingParam(out Guid GroupingParam);
            [PreserveSig]
            int SetGroupingParam(Guid Override, Guid Eventcontext);
            [PreserveSig]
            int RegisterAudioSessionNotification(IAudioSessionEvents NewNotifications);
            [PreserveSig]
            int UnregisterAudioSessionNotification(IAudioSessionEvents NewNotifications);
            [PreserveSig]
            int GetSessionIdentifier([Out(), MarshalAs(UnmanagedType.LPWStr)] out string retVal);
            [PreserveSig]
            int GetSessionInstanceIdentifier([Out(), MarshalAs(UnmanagedType.LPWStr)] out string retVal);
            [PreserveSig]
            int GetProcessId(out UInt32 retvVal);
            [PreserveSig]
            int IsSystemSoundsSession();
            [PreserveSig]
            int SetDuckingPreference(bool optOut);
        }


        [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface ISimpleAudioVolume
        {
            [PreserveSig]
            int SetMasterVolume(float fLevel, ref Guid EventContext);

            [PreserveSig]
            int GetMasterVolume(out float pfLevel);

            [PreserveSig]
            int SetMute(bool bMute, ref Guid EventContext);

            [PreserveSig]
            int GetMute(out bool pbMute);
        }
    }
}
