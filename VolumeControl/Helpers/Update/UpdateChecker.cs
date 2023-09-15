using CodingSeb.Localization;
using Semver;
using System;
using System.Diagnostics;
using System.Windows;
using VolumeControl.Core;
using VolumeControl.Log;
using VolumeControl.ViewModels;

namespace VolumeControl.Helpers.Update
{
    public class UpdateChecker
    {
        public UpdateChecker(VolumeControlVM vcSettingsInstance) => VCSettings = vcSettingsInstance;

        #region Fields
        private readonly VolumeControlVM VCSettings;
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
        public void CheckForUpdateNow()
        {
            ReleaseInfo latest = ReleaseInfo.Latest;

            if (latest.CompareTo(this.CurrentVersion) > 0)
            {
                Log.Debug($"Latest version ({latest.Version}) is newer than the current version.");
                if (Settings.ShowUpdatePrompt)
                {
                    this.ShowUpdatePrompt(latest);
                }
                VCSettings.UpdateAvailable = true;
                VCSettings.UpdateVersion = latest.Version.ToString();
            }
        }
        #region UpdatePrompt
        private string GetPromptMessage(ReleaseInfo releaseInfo)
        {
            string msg = Loc.Tr("VolumeControl.Dialogs.UpdatePrompt.NewVersionAvailableFormatMsg", "A new version of Volume Control is available!\nDo you want to go to the releases page?\nCurrent Version:  ${CURRENT_VERSION}\nLatest Version:   ${LATEST_VERSION}\n\nClick 'Yes' to go to the releases page.\nClick 'No' if you don't want to update right now.\nClick 'Cancel' to disable these prompts.");
            msg = msg.Replace("${CURRENT_VERSION}", this.CurrentVersion.ToString());
            msg = msg.Replace("${LATEST_VERSION}", releaseInfo.Version.ToString());
            return msg;
        }
        private void ShowUpdatePrompt(ReleaseInfo releaseInfo)
        {
            Log.Info($"Showing update prompt for new version: {releaseInfo.Version}");
            switch (MessageBox.Show(this.GetPromptMessage(releaseInfo), "Update Available", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes))
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
                _ = MessageBox.Show(Loc.Tr("VolumeControl.Dialogs.UpdatePrompt.DontShowInFutureMsg", "Update prompts will not be shown in the future."));
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
                var psi = new ProcessStartInfo(url)
                {
                    Verb = "open",
                    UseShellExecute = true
                };
                var proc = Process.Start(psi);
                Log.Debug($"Successfully opened default browser with process ID {proc?.Id}");
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't open '{url}' because of an exception!", ex);
                _ = MessageBox.Show($"Couldn't open '{url}' because of an exception:\n'{ex.Message}'", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
        #endregion OpenBrowser
        #endregion Methods
    }
}
