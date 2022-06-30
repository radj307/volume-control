using Semver;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Helpers.Update
{
    public class Updater
    {
        public Updater(VolumeControlSettings vcSettingsInstance) => VCSettings = vcSettingsInstance;

        #region Fields
        private readonly VolumeControlSettings VCSettings;
        internal const string _apiUriLatest = "https://api.github.com/repos/radj307/volume-control/releases/latest";
        internal const string _apiUserAgent = "curl/7.64.1";
        internal const string _htmlURLLatest = "https://github.com/radj307/volume-control/releases/latest";
        #endregion Fields

        #region Properties
        private static Config Settings => (Config.Default as Config)!;
        private static LogWriter Log => FLog.Log;
        private SemVersion CurrentVersion => VCSettings.CurrentVersion;
        #endregion Properties

        #region Methods
        public void CheckNow()
        {
            var latest = ReleaseInfo.Latest;

            if (latest.CompareTo(CurrentVersion) > 0)
            {
                Log.Debug($"Latest version ({latest.Version}) is newer than the current version.");
                if (Settings.ShowUpdatePrompt)
                {
                    ShowUpdatePrompt(latest);
                }
                else
                {
                    VCSettings.UpdateAvailable = true;
                    VCSettings.UpdateVersion = latest.Version.ToString();
                }
            }
        }
        #region UpdatePrompt
        private string GetPromptMessage(ReleaseInfo releaseInfo) =>
                 "A new version of Volume Control is available!\nDo you want to go to the releases page?" +
                $"Current Version:  {CurrentVersion}\n" +
                $"Latest Version:   {releaseInfo.Version}\n" +
                '\n' +
                "Click 'Yes' to go to the releases page.\n" +
                "Click 'No' if you don't want to update right now.\n" +
                "Click 'Cancel' to disable these prompts.";
        private void ShowUpdatePrompt(ReleaseInfo releaseInfo)
        {
            Log.Info($"Showing update prompt for new version: {releaseInfo.Version}");
            switch (MessageBox.Show(GetPromptMessage(releaseInfo), "Update Available", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes))
            {
            case MessageBoxResult.Yes: // open browser
                Log.Info($"User indicated they want to update to version {releaseInfo.Version}.");
                OpenBrowser(releaseInfo.URL);
                break;
            case MessageBoxResult.No:
                break;
            case MessageBoxResult.Cancel: // disable
                Settings.ShowUpdatePrompt = false;
                Log.Info("Disabled automatic updates");
                Settings.Save();
                MessageBox.Show("Update prompts will not be shown in the future.");
                break;
            }
        }
        #endregion UpdatePrompt
        #region OpenBrowser
        /// <summary>Opens the specified link in the default web browser.</summary>
        /// <param name="url">The URL of the page to open.</param>
        public static void OpenBrowser(string url)
        {
            try
            {
                var proc = Process.Start(new ProcessStartInfo(url)
                {
                    UseShellExecute = true,
                    Verb = "open"
                });
                Log.Debug($"Successfully opened default browser with process ID {proc?.Id}");
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't open '{url}' because of an exception!", ex);
                MessageBox.Show($"Couldn't open '{url}' because of an exception:\n'{ex.Message}'", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
        #endregion OpenBrowser
        #endregion Methods
    }
}
