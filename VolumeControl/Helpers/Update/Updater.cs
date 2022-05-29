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
using VolumeControl.Attributes;
using VolumeControl.Log;

namespace VolumeControl.Helpers.Update
{
    public class Updater
    {
        public Updater(VolumeControlSettings vcSettingsInstance)
        {
            VCSettings = vcSettingsInstance;
        }

        #region Fields
        private readonly VolumeControlSettings VCSettings;
        /// <summary>
        /// The filename of the Update Utility, including extensions but not qualifiers.<br/>See <see cref="_updateUtilityResourcePath"/>
        /// </summary>
        private const string _updateUtilityFilename = "VCUpdateUtility.exe";
        /// <summary>
        /// The fully qualified name of the Update Utility as an embedded resource.
        /// </summary>
        private const string _updateUtilityResourcePath = $"VolumeControl.Resources.{_updateUtilityFilename}";
        private const string _targetAssetName = "VolumeControl.exe";
        #endregion Fields

        #region Properties
        private static Properties.Settings Settings => Properties.Settings.Default;
        private static LogWriter Log => FLog.Log;
        private bool? IncludePreReleases
        {
            get => VCSettings.AllowUpdateToPreRelease;
            set => VCSettings.AllowUpdateToPreRelease = value;
        }
        private bool ShowPrompt => VCSettings.ShowUpdateMessageBox;
        private ERelease ReleaseType => VCSettings.ReleaseType;
        private bool CurrentVersionIsPreReleaseChannel => ReleaseType switch
        {
            ERelease.PRERELEASE or ERelease.CANDIDATE or ERelease.TESTING => true,
            _ => false
        };
        private SemVersion CurrentVersion => VCSettings.Version;
        /// <summary>
        /// Lazily-initialized & cached copy of the latest release object.
        /// </summary>
        public Release LatestRelease => _latestRelease ??= GetLatestRelease(IncludePreReleases, CurrentVersionIsPreReleaseChannel);
        private Release? _latestRelease = null;
        #endregion Properties

        #region Methods
        #region GetLatestRelease
        internal static async Task<Release> GetLatestReleaseAsync(bool? includePreReleases, bool onPreReleaseChannel)
        {
            using HttpClient client = new();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", "curl/7.64.1");

            Task<object?> getReq = client.GetFromJsonAsync(Settings.UpdateUri, typeof(GithubReleaseHttpResponse[]));
            Release? latest = null;

            if (await getReq.ConfigureAwait(false) is GithubReleaseHttpResponse[] packets)
            {
                foreach (GithubReleaseHttpResponse packet in packets)
                {
                    if (!packet.prerelease || (includePreReleases == null && onPreReleaseChannel) || (includePreReleases != null && includePreReleases.Value))
                    {
                        var rel = new Release(packet);
                        if (latest == null || rel.IsNewerThan(latest))
                            latest = rel;
                    }
                }
            }
            else throw new Exception("Failed to retrieve the latest version from the github API! (Malformed packet)");

            if (latest == null)
                throw new Exception($"Failed to retrieve the latest version from the github API! ({nameof(latest)} was null!)");

            client.Dispose();

            return latest;
        }
        internal static Release GetLatestRelease(bool? includePreReleases, bool onPreReleaseChannel) => GetLatestReleaseAsync(includePreReleases, onPreReleaseChannel).ConfigureAwait(false).GetAwaiter().GetResult();
        internal static async Task<Release> GetLatestReleaseAsync()
        {
            using HttpClient client = new();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", "curl/7.64.1");

            Task<object?> getReq = client.GetFromJsonAsync(Settings.UpdateUriLatest, typeof(GithubReleaseHttpResponse));
            Release latest;

            if (await getReq.ConfigureAwait(false) is GithubReleaseHttpResponse packet)
            {
                latest = new(packet);
            }
            else throw new Exception($"Failed to retrieve the latest version from the github API!");

            client.Dispose();

            return latest;
        }
        internal static Release GetLatestRelease() => GetLatestReleaseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        #endregion GetLatestRelease
        #region UpdatePrompt
        internal bool ShowUpdatePrompt()
        {
            switch (MessageBox.Show(
                $"Current Version:  {CurrentVersion}\n" +
                $"Newest Version:   {LatestRelease.Version}\n" +
                "\nDo you want to update now?\n\nClick 'Yes' to update now.\nClick 'No' to update later.\nClick 'Cancel' to disable update checking."
                , "Update Available", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No))
            {
            case MessageBoxResult.Yes: // open browser
                Log.Info($"User indicated they want to update to version {LatestRelease.Version}.");
                return true;
            case MessageBoxResult.No: // do nothing
                Log.Info($"Version {LatestRelease.Version} is available, but was temporarily ignored.");
                break;
            case MessageBoxResult.Cancel: // disable
                Settings.CheckForUpdatesOnStartup = false;
                Log.Info("Disabled automatic updates");
                Settings.Save();
                Settings.Reload();
                break;
            }
            return false;
        }
        #endregion UpdatePrompt
        #region AutoUpdater
        public void Update(bool force = false)
        {
            if (force || LatestRelease.IsNewerThan(CurrentVersion))
            {
                VCSettings.UpdateAvailable = true;
                VCSettings.UpdateVersion = $"Version {LatestRelease.Version} is available!";

                if (force || ShowPrompt && ShowUpdatePrompt())
                {
                    string dir = Path.GetDirectoryName(VCSettings.ExecutablePath)!;
                    if (SetupUpdateUtility(dir) is string updateUtilityPath)
                    {
                        if (Regex.Match(dir, "AppData[/\\\\]Local[/\\\\]Temp").Success)
                        {
                            throw new Exception($"Temp directory is an illegal output location! '{dir}'");
                        }
                        Log.Debug($"Successfully set up update utility in directory '{dir}'");
                        Log.Debug($"Starting update utility with commandline: ''");
                        if (StartAutomatedUpdate(LatestRelease, updateUtilityPath, VCSettings.ExecutablePath))
                        {
                            Log.Info($"Successfully prepared {_updateUtilityFilename}, shutting down...");
                            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                            Application.Current.Shutdown();
                        }
                        else Log.Error("Update failed!");
                    }
                    else
                    {
                        Log.Error($"Failed to set up the update utility in directory '{dir}'!");
                    }
                }
                else
                {
                    Log.Info($"Update Available: '{LatestRelease.Version}'");
                }
            }
            else Log.Info($"Already up-to-date. ({CurrentVersion})");
        }
        private static bool StartAutomatedUpdate(Release release, string? updateUtilityPath, string outpath)
        {
            if (updateUtilityPath == null)
                return false;
            if (release[_targetAssetName] is Release.Asset asset)
            {
                string commandline = $"-u {asset.DownloadURL} -o \"{outpath}\" -s {asset.Size} {Settings.UpdateUtilityExtraArguments}";
                Log.Debug($"Starting {updateUtilityPath} with commandline:", $"'{commandline}'");
                ProcessStartInfo psi = new(updateUtilityPath, commandline)
                {
                    ErrorDialog = true,
                    UseShellExecute = true,
                };
                Process.Start(psi);
                return true;
            }
            else
            {
                Log.Error($"Failed to find a release asset named {_targetAssetName}!", $"Opening the default web browser to '{Settings.UpdateURL}' instead.");
                OpenBrowser(Settings.UpdateURL);
            }
            return false;
        }
        /// <summary>
        /// Writes the update client from the embedded resource dictionary to the local disk.
        /// </summary>
        /// <returns>The absolute filepath of the updater utility's executable.</returns>
        private static string? SetupUpdateUtility(string directory)
        {
            var asm = Assembly.GetEntryAssembly();

            if (asm == null)
            {
                return null;
            }

            string path = Path.Combine(directory, _updateUtilityFilename);

            if (File.Exists(path)) // if the file already exists, delete it
            {
                Log.Warning($"Deleted update utility from a previous update located at '{path}'");
                File.Delete(path);
            }

            // Attempt to read the embedded update utility
            using Stream? s = asm.GetManifestResourceStream(_updateUtilityResourcePath);

            if (s == null)
            {
                Log.Error($"Failed to get a stream containing resource '{_updateUtilityResourcePath}'!", "This indicates that something may have gone wrong during the build process!", "Please attach a copy of this log file and report this at https://github.com/radj307/volume-control/issues");
                return null;
            }
            else if (s.Length <= 0)
            {
                Log.Error("Failed to retrieve the embedded update utility resource file!", "This indicates that something may have gone wrong during the build process!", "Please attach a copy of this log file and report this at https://github.com/radj307/volume-control/issues");
                s.Close();
                s.Dispose();
                return null;
            }

            using Stream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            fs.SetLength(s.Length);
            s.CopyTo(fs);
            s.Close();
            s.Dispose();

            fs.Flush(); //< idk if this is required, but it's probably a good idea
            fs.Close();
            fs.Dispose();

            if (!File.Exists(path))
            {
                Log.Error($"Failed to create the update utility at '{path}'!");
                return null;
            }

            return path;
        }
        #endregion AutoUpdater
        #region OpenBrowser
        /// <summary>Opens the specified link in the default web browser.</summary>
        /// <param name="url">The URL of the page to open.</param>
        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url)
                {
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Couldn't open '{url}' because of an exception:\n'{ex.Message}'", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
        #endregion OpenBrowser
        #endregion Methods
    }
}
