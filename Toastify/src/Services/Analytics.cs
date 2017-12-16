using GoogleMeasurementProtocol;
using GoogleMeasurementProtocol.Parameters;
using GoogleMeasurementProtocol.Parameters.AppTracking;
using GoogleMeasurementProtocol.Parameters.ContentInformation;
using GoogleMeasurementProtocol.Parameters.EventTracking;
using GoogleMeasurementProtocol.Parameters.Exceptions;
using GoogleMeasurementProtocol.Parameters.Hit;
using GoogleMeasurementProtocol.Parameters.SystemInfo;
using GoogleMeasurementProtocol.Parameters.User;
using GoogleMeasurementProtocol.Requests;
using log4net;
using log4net.Util;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using Toastify.Core;
using Toastify.Model;

namespace Toastify.Services
{
    // ReSharper disable once PartialTypeWithSinglePart
    public static partial class Analytics
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Analytics));

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private static string TrackingId { get; set; }

        public static bool AnalyticsEnabled
        {
#if !TEST_RELEASE
            get { return requestFactory != null && Settings.Current.OptInToAnalytics; }
#else
            get { return false; }
#endif
        }

        private static GoogleAnalyticsRequestFactory requestFactory;

        internal static void Init()
        {
#if !TEST_RELEASE
            logger.DebugExt("Initializing Analytics class...");

            // ReSharper disable once InvocationIsSkipped
            SetTrackingId();

            if (string.IsNullOrWhiteSpace(TrackingId))
            {
                logger.ErrorExt("No TrackingId set.");
                return;
            }

            requestFactory = new GoogleAnalyticsRequestFactory(TrackingId);
            bool wasOnNoAnalyticsVersion = new Version(Settings.Current.PreviousVersion ?? "0.0.0") < new Version("1.9.7");
            bool appHasBeenJustUpdated = new Version(Settings.Current.PreviousVersion ?? "0.0.0") < new Version(VersionChecker.CurrentVersion);

            // Install Event
            if (Settings.Current.FirstRun || wasOnNoAnalyticsVersion)
            {
                TrackInstallEvent();
                Settings.Current.FirstRun = false;
            }

            // NOTE: The install event should always be sent
            if (!AnalyticsEnabled)
                return;

            // Collect Preferences every at every update
            if (appHasBeenJustUpdated)
                CollectPreferences();
#else
            requestFactory = null;
#endif
        }

        public static void TrackPageHit(string documentPath, string title = null, bool interactive = true)
        {
            if (!AnalyticsEnabled)
                return;

            var request = requestFactory.CreateRequest(HitTypes.PageView);

            request.Parameters.AddRange(GetCommonParameters());
            request.Parameters.Add(new DocumentHostName("github.com/aleab/toastify"));
            request.Parameters.Add(new DocumentPath(documentPath));
            if (title != null)
                request.Parameters.Add(new DocumentTitle(title));
            request.Parameters.Add(new NonInteractionHit(!interactive));

            PostRequest(request);

            logger.DebugExt($"[Analytics] PageHit: ni={!interactive}, dp=\"{documentPath}\", dt=\"{title}\"");
        }

        public static void TrackEvent(ToastifyEventCategory eventCategory, string eventAction, string eventLabel = null, int eventValue = -1)
        {
            TrackEvent(null, eventCategory, eventAction, eventLabel, eventValue);
        }

        private static void TrackEvent(IEnumerable<Parameter> extraParameters, ToastifyEventCategory eventCategory, string eventAction, string eventLabel = null, int eventValue = -1)
        {
            if (!AnalyticsEnabled)
                return;

            var request = requestFactory.CreateRequest(HitTypes.Event);

            request.Parameters.AddRange(GetCommonParameters());
            request.Parameters.Add(new EventCategory(eventCategory.ToString()));
            request.Parameters.Add(new EventAction(eventAction));
            if (eventLabel != null)
                request.Parameters.Add(new EventLabel(eventLabel));
            if (eventValue >= 0)
                request.Parameters.Add(new EventValue(eventValue));
            if (extraParameters != null)
                request.Parameters.AddRange(extraParameters);

            PostRequest(request);

            logger.DebugExt($"[Analytics] Event: ec=\"{eventCategory}\", ea=\"{eventAction}\", el=\"{eventLabel}\", ev=\"{eventValue}\"");
        }

        public static void TrackException(Exception exception, bool fatal = false)
        {
            if (!AnalyticsEnabled)
                return;

            // The exception will be truncated to 150 bytes; at some point it may be better to extract more pertinent information.
            var request = requestFactory.CreateRequest(HitTypes.Exception, GetCommonParameters());

            request.Parameters.AddRange(GetCommonParameters());
            request.Parameters.Add(new ExceptionDescription($"{exception}"));
            request.Parameters.Add(new IsExceptionFatal(fatal));

            PostRequest(request);
        }

        private static void PostRequest(IGoogleAnalyticsRequest request)
        {
            request?.Post(new ClientId(GetMachineID()));
        }

        private static IEnumerable<Parameter> GetCommonParameters()
        {
            var parameters = new List<Parameter>
            {
                new ApplicationName("Toastify"),
                new ApplicationVersion(VersionChecker.CurrentVersion)
            };
            return parameters;
        }

        private static void TrackInstallEvent()
        {
            IEnumerable<Parameter> extraParameters = new List<Parameter>
            {
                new UserLanguage(CultureInfo.CurrentUICulture.Name)
            };
            TrackEvent(extraParameters, ToastifyEventCategory.General, "Install", GetOS());
        }

        private static void TrackSettingBinaryHit(string settingName, bool track)
        {
            if (track)
                TrackPageHit($"/{VersionChecker.CurrentVersion}/Settings/{settingName}", null, false);
        }

        private static void CollectPreferences()
        {
            logger.DebugExt($"Collecting preferences...");

            // General
            TrackSettingBinaryHit(nameof(Settings.Current.LaunchOnStartup), Settings.Current.LaunchOnStartup);
            TrackSettingBinaryHit(nameof(Settings.Current.MinimizeSpotifyOnStartup), Settings.Current.MinimizeSpotifyOnStartup);
            TrackSettingBinaryHit(nameof(Settings.Current.CloseSpotifyWithToastify), Settings.Current.CloseSpotifyWithToastify);

            TrackSettingBinaryHit($"{nameof(Settings.Current.VolumeControlMode)}/{ToastifyVolumeControlMode.Spotify}", Settings.Current.VolumeControlMode == ToastifyVolumeControlMode.Spotify);
            TrackSettingBinaryHit($"{nameof(Settings.Current.VolumeControlMode)}/{ToastifyVolumeControlMode.SystemGlobal}", Settings.Current.VolumeControlMode == ToastifyVolumeControlMode.SystemGlobal);
            TrackSettingBinaryHit($"{nameof(Settings.Current.VolumeControlMode)}/{ToastifyVolumeControlMode.SystemSpotifyOnly}", Settings.Current.VolumeControlMode == ToastifyVolumeControlMode.SystemSpotifyOnly);

            TrackSettingBinaryHit(nameof(Settings.Current.SaveTrackToFile), Settings.Current.SaveTrackToFile);

            // Hotkeys
            TrackSettingBinaryHit(nameof(Settings.Current.GlobalHotKeys), Settings.Current.GlobalHotKeys);
            foreach (var hotkey in Settings.Current.HotKeys)
                TrackSettingBinaryHit($"HotKeys/{hotkey.Action}", hotkey.Enabled);

            // Toast
            TrackSettingBinaryHit(nameof(Settings.Current.DisableToast), Settings.Current.DisableToast);
            TrackSettingBinaryHit(nameof(Settings.Current.OnlyShowToastOnHotkey), Settings.Current.OnlyShowToastOnHotkey);
            TrackSettingBinaryHit(nameof(Settings.Current.DisableToastWithFullscreenVideogames), Settings.Current.DisableToastWithFullscreenVideogames);
            TrackSettingBinaryHit(nameof(Settings.Current.ShowSongProgressBar), Settings.Current.ShowSongProgressBar);

            TrackSettingBinaryHit($"{nameof(Settings.Current.ToastTitlesOrder)}/{ToastTitlesOrder.ArtistOfTrack}", Settings.Current.ToastTitlesOrder == ToastTitlesOrder.ArtistOfTrack);
            TrackSettingBinaryHit($"{nameof(Settings.Current.ToastTitlesOrder)}/{ToastTitlesOrder.TrackByArtist}", Settings.Current.ToastTitlesOrder == ToastTitlesOrder.TrackByArtist);
        }

        private static string GetOS()
        {
            return Environment.OSVersion.VersionString +
                " (" + GetFriendlyOS() + ")" +
                " (" + (Environment.Is64BitOperatingSystem ? "x64" : "x86") + ")";
        }

        private static string GetFriendlyOS()
        {
            object name = null;
            try
            {
                var managementObjects = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem")
                    .Get()
                    .OfType<ManagementObject>();
                name = (from x in managementObjects select x.GetPropertyValue("Caption")).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorExt("Error while getting FriendlyOS.", e);
            }
            return name?.ToString() ?? "Unknown";
        }

        private static string GetMachineID()
        {
            // HKLM\SOFTWARE\Microsoft\Cryptography > MachineGuid
            string machineGuid = Registry.LocalMachine
                ?.OpenSubKey("SOFTWARE")
                ?.OpenSubKey("Microsoft")
                ?.OpenSubKey("Cryptography")
                ?.GetValue("MachineGuid") as string;

            return machineGuid ?? "00000000-0000-0000-0000-000000000000";
        }

        // ReSharper disable once PartialMethodWithSinglePart
        static partial void SetTrackingId();

        public enum ToastifyEventCategory
        {
            General,
            Action
        }

        /// <summary>
        /// Poor mans enum -> expanded string.
        ///
        /// Once I've been using this for a while I may change this to a pure enum if
        /// spaces in names prove to be annoying for querying / sorting the data
        /// </summary>
        public static class ToastifyEvent
        {
            public const string Exception = "Exception";

            public const string AppLaunch = "Toastify.AppLaunched";
            public const string AppTermination = "Toastify.AppTermination";
            public const string SettingsLaunched = "Toastify.SettingsLaunched";

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