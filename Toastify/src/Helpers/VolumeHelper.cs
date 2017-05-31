using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Toastify.Helpers
{
    internal class VolumeHelper
    {
        // base code from: http://stackoverflow.com/a/14322736

        private const float VOLUME_DELTA = 5.0f;

        public static void IncrementVolume(string name)
        {
            float? curVolume = GetApplicationVolume(name);

            if (curVolume != null && curVolume < 100.0f)
                SetApplicationVolume(name, curVolume.Value + VOLUME_DELTA);
        }

        public static void DecrementVolume(string name)
        {
            var curVolume = GetApplicationVolume(name);

            if (curVolume != null && curVolume > 0.0f)
                SetApplicationVolume(name, curVolume.Value - VOLUME_DELTA);
        }

        public static float? GetApplicationVolume(string name)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return null;

            volume.GetMasterVolume(out float level);
            return level * 100.0f;
        }

        public static bool? GetApplicationMute(string name)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return null;

            volume.GetMute(out bool mute);
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
            bool? muted = GetApplicationMute(name);
            if (muted == null)
                return;

            SetApplicationMute(name, !muted.Value);
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
            MMDeviceEnumerator deviceEnumerator_ = new MMDeviceEnumerator();
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)deviceEnumerator_;
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.ERender, ERole.EMultimedia, out IMMDevice speakers);

            // activate the session manager. we need the enumerator
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            mgr.GetSessionEnumerator(out IAudioSessionEnumerator sessionEnumerator);
            sessionEnumerator.GetCount(out int count);

            for (int i = 0; i < count; i++)
            {
                sessionEnumerator.GetSession(i, out IAudioSessionControl ctl);

                if (ctl is IAudioSessionControl2 ctl2)
                {
                    ctl2.GetSessionIdentifier(out string sout1);
                    ctl2.GetProcessId(out uint pid);
                    ctl2.GetSessionInstanceIdentifier(out string sout2);
                }

                ctl.GetDisplayName(out string dn);
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
            MMDeviceEnumerator deviceEnumerator_ = new MMDeviceEnumerator();
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)deviceEnumerator_;
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.ERender, ERole.EMultimedia, out IMMDevice speakers);

            // activate the session manager. we need the enumerator
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            mgr.GetSessionEnumerator(out IAudioSessionEnumerator sessionEnumerator);
            sessionEnumerator.GetCount(out int count);

            // lower case name for easier comparison with the Session ID later on
            name = name.ToLower();

            // search for an audio session with the required name
            // Note: Since GetDisplayName only returns a real name if the application bothered to call SetDisplayName
            //       (which apps like Spotify do not), we instead use the SessionID (which usually includes the exe name)
            ISimpleAudioVolume volumeControl = null;

            for (int i = 0; i < count; i++)
            {
                sessionEnumerator.GetSession(i, out IAudioSessionControl ctl);
                IAudioSessionControl2 ctl2 = ctl as IAudioSessionControl2;

                ctl.GetDisplayName(out string dn);

                if (ctl2 != null)
                {
                    ctl2.GetSessionIdentifier(out string sessionID);

                    if (sessionID.ToLower().Contains(name))
                    {
                        volumeControl = ctl as ISimpleAudioVolume;
                        break;
                    }
                }

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
            ERender,
            ECapture,
            EAll,
            EDataFlowEnumCount
        }

        internal enum ERole
        {
            EConsole,
            EMultimedia,
            ECommunications,
            ERoleEnumCount
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
            int GetSessionEnumerator(out IAudioSessionEnumerator sessionEnum);

            // the rest is not implemented
        }

        [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioSessionEnumerator
        {
            [PreserveSig]
            int GetCount(out int sessionCount);

            [PreserveSig]
            int GetSession(int sessionCount, out IAudioSessionControl session);
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

        [Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionEvents
        {
            [PreserveSig]
            int OnDisplayNameChanged([MarshalAs(UnmanagedType.LPWStr)] string newDisplayName, Guid eventContext);

            [PreserveSig]
            int OnIconPathChanged([MarshalAs(UnmanagedType.LPWStr)] string newIconPath, Guid eventContext);

            [PreserveSig]
            int OnSimpleVolumeChanged(float newVolume, bool newMute, Guid eventContext);

            [PreserveSig]
            int OnChannelVolumeChanged(UInt32 channelCount, IntPtr newChannelVolumeArray, UInt32 changedChannel, Guid eventContext);

            [PreserveSig]
            int OnGroupingParamChanged(Guid newGroupingParam, Guid eventContext);

            [PreserveSig]
            int OnStateChanged(AudioSessionState newState);

            [PreserveSig]
            int OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason);
        }

        [Guid("BFB7FF88-7239-4FC9-8FA2-07C950BE9C6D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionControl2
        {
            [PreserveSig]
            int GetState(out AudioSessionState state);

            [PreserveSig]
            int GetDisplayName([Out(), MarshalAs(UnmanagedType.LPWStr)] out string name);

            [PreserveSig]
            int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string value, Guid eventContext);

            [PreserveSig]
            int GetIconPath([Out(), MarshalAs(UnmanagedType.LPWStr)] out string path);

            [PreserveSig]
            int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string value, Guid eventContext);

            [PreserveSig]
            int GetGroupingParam(out Guid groupingParam);

            [PreserveSig]
            int SetGroupingParam(Guid Override, Guid eventcontext);

            [PreserveSig]
            int RegisterAudioSessionNotification(IAudioSessionEvents newNotifications);

            [PreserveSig]
            int UnregisterAudioSessionNotification(IAudioSessionEvents newNotifications);

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
            int SetMasterVolume(float fLevel, ref Guid eventContext);

            [PreserveSig]
            int GetMasterVolume(out float pfLevel);

            [PreserveSig]
            int SetMute(bool bMute, ref Guid eventContext);

            [PreserveSig]
            int GetMute(out bool pbMute);
        }
    }
}