using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeControl.Helpers.Update
{    //    /// <summary>
    //    /// Responsible for checking for updates.
    //    /// </summary>
    //    internal static class UpdateChecker
    //    {
    //        #region Fields
    //        private static bool _initialized = false;
    //        private static SemVersion? _currentVersion;
    //        private static ERelease _releaseType;
    //        private static string? _executablePath;
    //        private const string _targetAssetName = "VolumeControl.exe";
    //        private Release? _newest;
    //#       if DEBUG
    //        /// <summary>
    //        /// Set this to true using the Immediate Window to skip checking if the version number is actually newer.
    //        /// </summary>
    //        public static bool TEST_UPDATE = false;
    //#       endif

    //        /// <summary>
    //        /// The filename of the Update Utility, including extensions but not qualifiers.<br/>See <see cref="_updateUtilityResourcePath"/>
    //        /// </summary>
    //        private const string _updateUtilityFilename = "VCUpdateUtility.exe";
    //        /// <summary>
    //        /// The fully qualified name of the Update Utility as an embedded resource.
    //        /// </summary>
    //        private const string _updateUtilityResourcePath = $"VolumeControl.Resources.{_updateUtilityFilename}";
    //        #endregion Fields

    //        #region Properties
    //        private static Properties.Settings Settings => Properties.Settings.Default;
    //        private static LogWriter Log => FLog.Log;
    //        private static bool ImplicitIncludePreRelease => _releaseType.EqualsAny(ERelease.PRERELEASE, ERelease.TESTING, ERelease.CANDIDATE);
    //        #endregion Properties

    //        #region Methods
    //        private static void Initialize()
    //        {
    //            if (_initialized)
    //                throw new Exception($"{nameof(UpdateChecker)} is already initialized!");
    //            var asm = Assembly.GetEntryAssembly();
    //            if (asm == null)
    //                throw new Exception($"Unable to retrieve the current assembly!");

    //            if (asm.GetCustomAttribute<ExtendedVersion>() is ExtendedVersion ver)
    //                _currentVersion = ver.Version.GetSemVer();

    //            if (asm.GetCustomAttribute<ReleaseType>() is ReleaseType relType)
    //                _releaseType = relType.Type;
    //            else
    //                _releaseType = ERelease.NORMAL;

    //            _executablePath = asm.Location;

    //            _initialized = true;
    //        }
    //        /// <summary>Gets the metadata of the latest release from the github api.</summary>
    //        /// <param name="includePreReleases">Possible values:<list type="table">
    //        /// <item><term><see langword="true"/></term><description> Pre-Release versions are included.</description></item>
    //        /// <item><term><see langword="false"/></term><description> Pre-Release versions are not included.</description></item>
    //        /// <item><term><see langword="null"/></term><description> Pre-Release versions are included only when the current version is a pre-release or release candidate.</description></item>
    //        /// </list></param>
    //        public static async Task<Release> GetLatestVersionMetadataAsync(bool? includePreReleases = null)
    //        {
    //            using HttpClient client = new();
    //            // clear the HTTP request header
    //            client.DefaultRequestHeaders.Accept.Clear();
    //            // request JSON response format
    //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
    //            // use the curl util's User-Agent specifier
    //            client.DefaultRequestHeaders.Add("User-Agent", "curl/7.64.1");

    //            // create a get task that automatically deserializes the response to an array of structs
    //            Task<object?>? getTask = client.GetFromJsonAsync(Settings.UpdateUri, typeof(GithubReleaseHttpResponse[]));
    //            // wait for the task to complete
    //            object? result = await getTask.ConfigureAwait(false);

    //            client.Dispose();

    //            // find the newest release in the list
    //            Release newest = null!;
    //            if (result is GithubReleaseHttpResponse[] releases)
    //            {
    //                foreach (GithubReleaseHttpResponse packet in releases.Where(r => !r.draft && (!r.prerelease || (includePreReleases == null && ImplicitIncludePreRelease) || (includePreReleases != null && includePreReleases.Value))))
    //                {
    //                    var rel = new Release(packet);
    //                    if (newest == null || rel.IsNewerThan(newest))
    //                        newest = rel;
    //                }
    //            }
    //            return newest;
    //        }
    //        /// <summary>Gets the metadata of the latest release from the github api.</summary>
    //        /// <param name="includePreReleases">Possible values:<list type="table">
    //        /// <item><term><see langword="true"/></term><description> Pre-Release versions are included.</description></item>
    //        /// <item><term><see langword="false"/></term><description> Pre-Release versions are not included.</description></item>
    //        /// <item><term><see langword="null"/></term><description> Pre-Release versions are included only when the current version is a pre-release or release candidate.</description></item>
    //        /// </list></param>
    //        public static Release GetLatestVersionMetadata(bool? includePreRelease = null)
    //        {
    //            var task = GetLatestVersionMetadataAsync(includePreRelease);
    //            task.Wait(-1);
    //            return task.Result;
    //        }
    //        /// <summary>
    //        /// Retrieves the list of releases from the Github API, and if a newer version is found a message box is shown prompting the user to update.
    //        /// </summary>
    //        /// <param name="includePreReleases">Possible values:<list type="table">
    //        /// <item><term><see langword="true"/></term><description> Pre-Release versions are included.</description></item>
    //        /// <item><term><see langword="false"/></term><description> Pre-Release versions are not included.</description></item>
    //        /// <item><term><see langword="null"/></term><description> Pre-Release versions are included only when the current version is a pre-release or release candidate.</description></item>
    //        /// </list></param>
    //        /// <param name="showPrompt">When true, a message box is shown prompting the user to update.</param>
    //        /// <returns>True when the autoupdater is ready & waiting for the program to shutdown, otherwise false.</returns>
    //        private static async Task<bool> CheckForUpdatesAsync(bool? includePreReleases, bool showPrompt, bool forceUpdate = false)
    //        {
    //            if (_currentVersion == null)
    //                return false;
    //            var updateTask = GetLatestVersionMetadataAsync(includePreReleases);

    //            Release? release = await updateTask.ConfigureAwait(false);

    //            if (forceUpdate || release.IsNewerThan(_currentVersion))
    //            {
    //                VolumeControlSettings.UpdateAvailable = true;
    //                VolumeControlSettings.UpdateVersion = $"Update Available: {release.Version}";

    //                if (showPrompt)
    //                {
    //                    switch (MessageBox.Show(
    //                        $"Current Version:  {_currentVersion}\n" +
    //                        $"Newest Version:   {release.Version}\n" +
    //                        "\nDo you want to update now?\n\nClick 'Yes' to update now.\nClick 'No' to update later.\nClick 'Cancel' to disable update checking."
    //                        , "Update Available", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No))
    //                    {
    //                    case MessageBoxResult.Yes: // open browser
    //                        StartAutomatedUpdate(release);
    //                        break;
    //                    case MessageBoxResult.No: // do nothing
    //                        Log.Info($"Version {release.Version} is available, but was temporarily ignored.");
    //                        break;
    //                    case MessageBoxResult.Cancel: // disable
    //                        Settings.CheckForUpdatesOnStartup = false;
    //                        Log.Info("Disabled automatic updates");
    //                        Settings.Save();
    //                        Settings.Reload();
    //                        break;
    //                    }
    //                }
    //            }
    //            return false;
    //        }
    //        public static void StartAutomatedUpdate(Release release)
    //        {
    //            if (release[_targetAssetName] is Release.Asset asset)
    //            {
    //                Log.Info($"Setting up {_updateUtilityFilename}.");

    //                if (SetupUpdateUtility() is string path)
    //                {
    //                    Log.Info($"{_updateUtilityFilename} was created at {path}");
    //                    ProcessStartInfo psi = new(path, $"-u {asset.DownloadURL} -o \"{_executablePath}\" -s {asset.Size} {Settings.UpdateUtilityExtraArguments}")
    //                    {
    //                        ErrorDialog = true,
    //                        UseShellExecute = true,
    //                    };
    //                    Process.Start(psi);
    //                    return true;
    //                }
    //                else
    //                {
    //                    Log.Error("Failed to setup automatic update utility, falling back to the default web browser.");
    //                    OpenBrowser(release.URL);
    //                }
    //            }
    //            else
    //            {
    //                Log.Error($"Failed to find a release asset named {_targetAssetName}!", $"Opening the default web browser to '{Settings.UpdateURL}' instead.");
    //                OpenBrowser(Settings.UpdateURL);
    //            }
    //        }
    //        public static void StartAutomatedUpdate() => StartAutomatedUpdate(_newest ??= GetLatestVersionMetadata());
    //        /// <summary>Opens the specified link in the default web browser.</summary>
    //        /// <param name="url">The URL of the page to open.</param>
    //        public static void OpenBrowser(string url)
    //        {
    //            if (!_initialized)
    //                Initialize();
    //            try
    //            {
    //                Process.Start(new ProcessStartInfo(url)
    //                {
    //                    UseShellExecute = true,
    //                    Verb = "open"
    //                });
    //            }
    //            catch (Exception ex)
    //            {
    //                MessageBox.Show($"Couldn't open '{url}' because of an exception:\n'{ex.Message}'", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
    //            }
    //        }
    //        /// <summary>
    //        /// Checks for updates, and automatically installs them depending on user interaction via <see cref="MessageBox"/>.
    //        /// </summary>
    //        /// <param name="includePreReleases">Possible values:<list type="table">
    //        /// <item><term><see langword="true"/></term><description> Pre-Release versions are included.</description></item>
    //        /// <item><term><see langword="false"/></term><description> Pre-Release versions are not included.</description></item>
    //        /// <item><term><see langword="null"/></term><description> Pre-Release versions are included only when the current version is a pre-release or release candidate.</description></item>
    //        /// </list></param>
    //        /// <param name="forceUpdate">When true, the update prompt is shown regardless of whether the newest version on github is actually newer than the current one.</param>
    //        public static bool CheckForUpdates(bool? includePreReleases = null, bool forceUpdate = false)
    //        {
    //            if (!_initialized)
    //                Initialize();
    //            Task<bool> updateTask = CheckForUpdatesAsync(includePreReleases, forceUpdate);
    //            updateTask.Wait(-1);
    //            return updateTask.Result;
    //        }
    //        /// <summary>
    //        /// Writes the update client from the embedded resource dictionary to the local disk.
    //        /// </summary>
    //        /// <returns>The absolute filepath of the updater utility's executable.</returns>
    //        private static string? SetupUpdateUtility()
    //        {
    //            var asm = Assembly.GetEntryAssembly();
    //            if (asm == null)
    //            {
    //                return null;
    //            }

    //            string path = Path.Combine(Path.GetDirectoryName(_executablePath) ?? string.Empty, _updateUtilityFilename);

    //            if (File.Exists(path)) // if the file already exists, delete it
    //            {
    //                Log.Warning($"Deleted update utility from a previous update located at '{path}'");
    //                File.Delete(path);
    //            }

    //            // Attempt to read the embedded update utility
    //            using Stream? s = asm.GetManifestResourceStream(_updateUtilityResourcePath);

    //            if (s == null)
    //            {
    //                Log.Error($"Failed to get a stream containing resource '{_updateUtilityResourcePath}'!", "This indicates that something may have gone wrong during the build process!", "Please attach a copy of this log file and report this at https://github.com/radj307/volume-control/issues");
    //                return null;
    //            }
    //            else if (s.Length <= 0)
    //            {
    //                Log.Error("Failed to retrieve the embedded update utility resource file!", "This indicates that something may have gone wrong during the build process!", "Please attach a copy of this log file and report this at https://github.com/radj307/volume-control/issues");
    //                s.Close();
    //                s.Dispose();
    //                return null;
    //            }

    //            using Stream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
    //            fs.SetLength(s.Length);
    //            s.CopyTo(fs);
    //            s.Close();
    //            s.Dispose();

    //            fs.Flush(); //< idk if this is required, but it's probably a good idea
    //            fs.Close();
    //            fs.Dispose();

    //            if (!File.Exists(path))
    //            {
    //                return null;
    //            }

    //            return path;
    //        }
    //        #endregion Methods
    //    }
}
