using Localization;
using Semver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows;
using VolumeControl.Core;
using VolumeControl.Log;
using VolumeControl.ViewModels;

namespace VolumeControl.Helpers.Update
{
    public class UpdateChecker
    {
        #region Constructor
        public UpdateChecker(VolumeControlVM vcSettingsInstance) => VCSettings = vcSettingsInstance;
        #endregion Constructor

        #region Fields
        private readonly VolumeControlVM VCSettings;
        internal const string _apiUriLatest = "https://api.github.com/repos/radj307/volume-control/releases/latest";
        internal const string _apiUserAgent = "curl/7.64.1";
        internal const string _htmlURLLatest = "https://github.com/radj307/volume-control/releases/latest";
        #endregion Fields

        #region Properties
        private static Config Settings => Config.Default;
        private SemVersion CurrentVersion => VCSettings.CurrentVersion;
        #endregion Properties

        #region Methods

        #region CheckForUpdateNow
        public void CheckForUpdateNow()
        {
            ReleaseInfo latest = ReleaseInfo.Latest;

            if (latest.CompareTo(this.CurrentVersion) > 0)
            {
                FLog.Debug($"Latest version ({latest.Version}) is newer than the current version.");
                if (Settings.ShowUpdatePrompt)
                {
                    this.ShowUpdatePrompt(latest);
                }
                VCSettings.UpdateAvailable = true;
                VCSettings.UpdateVersion = latest.Version.ToString();
            }
        }
        #endregion CheckForUpdateNow

        #region MakeUpdatePromptMessage
        private string MakeUpdatePromptMessage(SemVersion latestVersion)
        {
            string messageCore = Loc.Tr("VolumeControl.Dialogs.UpdatePrompt.NewVersionAvailableMessage", defaultText:
                "A new version of Volume Control is available!\n" +
                "Current Version:  ${CURRENT_VERSION}\n" +
                "Latest Version:   ${LATEST_VERSION}")
                .Replace("${CURRENT_VERSION}", this.CurrentVersion.ToString())
                .Replace("${LATEST_VERSION}", latestVersion.ToString());

            string options =
#if RELEASE_FORINSTALLER
                Loc.Tr("VolumeControl.Dialogs.UpdatePrompt.NewVersionAvailable_AutoUpdate", defaultText:
                "Click 'Yes' to install the latest release.\n" +
                "Click 'No' if you don't want to update right now.\n" +
                "Click 'Cancel' to disable these prompts.");
#else
                Loc.Tr("VolumeControl.Dialogs.UpdatePrompt.NewVersionAvailable_OpenBrowser", defaultText:
                "Click 'Yes' to go to the releases page.\n" +
                "Click 'No' if you don't want to update right now.\n" +
                "Click 'Cancel' to disable these prompts.");
#endif

            return messageCore + "\n\n" + options;
        }
        #endregion MakeUpdatePromptMessage

        #region DownloadReleaseAsset
        private static string? DownloadReleaseAsset(GithubAssetHttpResponse targetAsset, string outputDirectoryPath)
        {
            if (!Directory.Exists(outputDirectoryPath))
            {
                FLog.Error($"[{nameof(Update)}] Download failed; output directory \"{outputDirectoryPath}\" doesn't exist!");
                return null;
            }

            HttpResponseMessage? response;
            try
            {
                using var client = new HttpClient();
                using var req = new HttpRequestMessage()
                {
                    RequestUri = new(targetAsset.url),
                    Method = HttpMethod.Get
                };
                req.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/octet-stream"));
                req.Headers.Add("User-Agent", "Volume Control");
                response = client.Send(req);
            }
            catch (Exception ex)
            {
                FLog.Error($"[{nameof(Update)}] An exception occurred while downloading \"{targetAsset.name}\":", ex);
                return null;
            }
            if (!response.IsSuccessStatusCode)
            {
                FLog.Error($"[{nameof(Update)}] Download request failed with status code {response.StatusCode}!");
                return null;
            }
            var outputFilePath = Path.Combine(Path.GetFullPath(outputDirectoryPath), targetAsset.name);

            using var stream = response.Content.ReadAsStream();
            try
            {
                using var fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                stream.CopyTo(fileStream);
                fileStream.Flush();
            }
            catch (Exception ex)
            {
                FLog.Error($"[{nameof(Update)}] An exception occurred while writing to file \"{outputFilePath}\":", ex);
                return null;
            }

            return outputFilePath;
        }
        #endregion DownloadReleaseAsset

        #region ShowUpdatePrompt
        private void ShowUpdatePrompt(ReleaseInfo releaseInfo)
        {
            FLog.Info($"[{nameof(Update)}] Showing update prompt for new version: {releaseInfo.Version}");

            string msg = MakeUpdatePromptMessage(releaseInfo.Version);

            switch (MessageBox.Show(msg, "Update Available", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes))
            {
            case MessageBoxResult.Yes: // open browser
                {
                    FLog.Info($"[{nameof(Update)}] User indicated they want to update to version {releaseInfo.Version}.");
#if RELEASE_FORINSTALLER
                    const string installerAssetNamePrefix = "VolumeControl-Installer";
                    if (!releaseInfo.TryGetAsset(installerAssetNamePrefix, out var installerAsset))
                    { // failed to find a release asset with the specified name
                        FLog.Error(
                            $"[{nameof(Update)}] Failed to download installer for \"{releaseInfo.Version}\"; couldn't find a release asset beginning with \"{installerAssetNamePrefix}\"!");
                        MessageBox.Show("", "Update Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    // download the release asset
                    if (DownloadReleaseAsset(installerAsset, KnownFolders.GetPath(KnownFolder.Downloads)) is string installerPath)
                    { // start the installer
                        ShellHelper.OpenWithDefault(releaseInfo.URL); //< open the changelog
                        if (ShellHelper.OpenWithDefault(installerPath))
                        {
                            FLog.Info($"[{nameof(Update)}] Started \"{installerPath}\"; closing the application & opening \"{releaseInfo.URL}\" in the default browser.");
                            Application.Current.Shutdown(Program.EXITCODE_UPDATING);
                        }
                        else
                        {
                            MessageBox.Show(
                                Loc.Tr("VolumeControl.Dialogs.UpdatePrompt.StartFailed.Message", "Failed to start the installer, but it was downloaded successfully.\nYour system permissions may not allow running applications from your downloads folder.\n\nYou will have to start the installer manually."),
                                Loc.Tr("VolumeControl.Dialogs.UpdatePrompt.StartFailed.Caption", "Failed to Start Installer"),
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                    else if (MessageBox.Show(
                            Loc.Tr("VolumeControl.Dialogs.UpdatePrompt.DownloadFailed.Message", "An error occurred while downloading the installer, check the log for more information."),
                            Loc.Tr("VolumeControl.Dialogs.UpdatePrompt.DownloadFailed.Caption", "Download Failed"),
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Error) == MessageBoxResult.Yes)
                    {
                        ShellHelper.OpenWithDefault(releaseInfo.URL); //< open the releases page
                    }
#else
                    // open the release URL
                    OpenBrowser(releaseInfo.URL);
#endif
                    break;
                }
            case MessageBoxResult.No:
                FLog.Info($"[{nameof(Update)}] User indicated they do not want to update.");
                break;
            case MessageBoxResult.Cancel: // disable
                Settings.ShowUpdatePrompt = false;
                FLog.Info($"[{nameof(Update)}] User indicated they want to disable update prompts, and they were disabled.");
                _ = MessageBox.Show(Loc.Tr("VolumeControl.Dialogs.UpdatePrompt.DontShowInFutureMsg", "Update prompts will not be shown in the future."));
                break;
            }
        }
        #endregion ShowUpdatePrompt

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
                using var proc = Process.Start(psi);
                FLog.Debug($"Successfully opened default browser with process ID {proc?.Id}");
            }
            catch (Exception ex)
            {
                FLog.Error($"Couldn't open '{url}' because of an exception!", ex);
                _ = MessageBox.Show($"Couldn't open '{url}' because of an exception:\n'{ex.Message}'", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
        #endregion OpenBrowser

        #endregion Methods

        #region P/Invoke
        enum KnownFolder
        {
            Contacts,
            Downloads,
            Favorites,
            Links,
            SavedGames,
            SavedSearches
        }
        static class KnownFolders
        {
            private static readonly Dictionary<KnownFolder, Guid> _guids = new()
            {
                [KnownFolder.Contacts] = new("56784854-C6CB-462B-8169-88E350ACB882"),
                [KnownFolder.Downloads] = new("374DE290-123F-4565-9164-39C4925E467B"),
                [KnownFolder.Favorites] = new("1777F761-68AD-4D8A-87BD-30B759FA33DD"),
                [KnownFolder.Links] = new("BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968"),
                [KnownFolder.SavedGames] = new("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"),
                [KnownFolder.SavedSearches] = new("7D1D3A04-DEBB-4115-95CF-2F29DA2920DA")
            };

            public static string GetPath(KnownFolder knownFolder)
            {
                return SHGetKnownFolderPath(_guids[knownFolder], 0);
            }

            [DllImport("shell32",
                CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
            private static extern string SHGetKnownFolderPath(
                [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags,
                nint hToken = 0);
        }
        #endregion P/Invoke
    }
}
