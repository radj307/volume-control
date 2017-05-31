using Garlic;
using System;
using System.Linq;
using System.Management;
using Toastify.Core;

namespace Toastify.Services
{
    public static class Telemetry
    {
        private static AnalyticsSession _session;
        private static IAnalyticsPageViewRequest _client;

        static Telemetry()
        {
            Init();
        }

        private static void Init()
        {
            _session = new AnalyticsSession("http://toastify.nachmore.com/app", "UA-61123985-2");

            var settings = SettingsXml.Instance;

            // abort asap if we are surpressing analytics
            if (settings.PreventAnalytics)
                return;

            _session.SetCustomVariable(1, "OS Version", GetOS());

            _client = _session.CreatePageViewRequest("/", "Global");

            if (SettingsXml.Instance.FirstRun)
            {
                TrackEvent(TelemetryCategory.General, "Install", GetOS());

                SettingsXml.Instance.FirstRun = false;
            }
        }

        public static void TrackEvent(TelemetryCategory category, string action, object label = null, int value = 0)
        {
            _client?.SendEvent(category.ToString(), action, label?.ToString(), value.ToString());
        }

        internal static void TrackException(Exception exception)
        {
            // The exception will be truncated to 500bytes (GA limit for Labels), at some point it may be better to extract more pertinant information
            TrackEvent(TelemetryCategory.General, TelemetryEvent.Exception, exception.ToString());
        }

        internal static void TrackEvent(TelemetryCategory general, object settingsLaunched)
        {
            throw new NotImplementedException();
        }

        public static string GetOS()
        {
            return Environment.OSVersion.VersionString +
                " (" + GetFriendlyOS() + ")" +
                " (" + (Environment.Is64BitOperatingSystem ? "x64" : "x86") + ")";
        }

        private static string GetFriendlyOS()
        {
            var name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().OfType<ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault();
            return name?.ToString() ?? "Unknown";
        }

        /// <summary>
        /// Poor mans enum -> expanded string.
        ///
        /// Once I've been using this for a while I may change this to a pure enum if
        /// spaces in names prove to be annoying for querying / sorting the data
        /// </summary>
        public static class TelemetryEvent
        {
            public const string Exception = "Toastify.General.Exception";

            public const string AppLaunch = "Toastify.General.AppLaunched";
            public const string AppUpgraded = "Toastify.General.AppUpgraded";

            public const string SettingsLaunched = "Toastify.General.SettingsLaunched";

            public static class SpotifyWebService
            {
                public const string NetworkError = "Toastify.SpotifyWebService.NetworkError";
                public const string ResponseError = "Toastify.SpotifyWebService.ResponseError";
            }

            public static class Action
            {
                public const string Mute = "Toastify.Action.Mute";
                public const string VolumeDown = "Toastify.Action.VolumeDown";
                public const string VolumeUp = "Toasitfy.Action.VolumeUp";
                public const string ShowToast = "Toastify.Action.ShowToast";
                public const string ShowSpotify = "Toastify.Action.ShowSpotify";
                public const string CopyTrackInfo = "Toastify.Action.CopyTrackInfo";
                public const string PasteTrackInfo = "Toastify.Action.PasteTrackInfo";
                public const string FastForward = "Toastify.Action.FastForward";
                public const string Rewind = "Toastify.Action.Rewind";
                public const string Default = "Toastify.Action.";
            }
        }
    }
}