using AssemblyAttribute;
using Semver;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using VolumeControl.Attributes;
using VolumeControl.Core.Extensions;
using VolumeControl.Log;

namespace VolumeControl.Helpers.Update
{
    /// <summary>
    /// Responsible for checking for updates.
    /// </summary>
    internal static class UpdateChecker
    {
        #region Fields
        private static bool _initialized = false;
        private static bool _hasCheckedForUpdates = false;
        private static SemVersion? _currentVersion;
        private static ERelease _releaseType;
        private static string? _executablePath;
#       if DEBUG
        /// <summary>
        /// Set this to true using the Immediate Window to skip checking if the version number is actually newer.
        /// </summary>
        public static bool TEST_UPDATE = false;
#       endif

        /// <summary>
        /// The filename of the Update Utility, including extensions but not qualifiers.<br/>See <see cref="_updateUtilityResourcePath"/>
        /// </summary>
        private const string _updateUtilityFilename = "VCUpdateUtility.exe";
        /// <summary>
        /// The fully qualified name of the Update Utility as an embedded resource.
        /// </summary>
        private const string _updateUtilityResourcePath = $"VolumeControl.Resources.{_updateUtilityFilename}";
        #endregion Fields

        #region Properties
        private static Properties.Settings Settings => Properties.Settings.Default;
        private static LogWriter Log => FLog.Log;
        private static bool AllowPreReleases => _releaseType.EqualsAny(ERelease.PRERELEASE, ERelease.TESTING, ERelease.CANDIDATE);
        #endregion Properties

        #region Methods
        private static void Initialize()
        {
            if (_initialized)
                throw new Exception($"{nameof(UpdateChecker)} is already initialized!");
            var asm = Assembly.GetEntryAssembly();
            if (asm == null)
                throw new Exception($"Unable to retrieve the current assembly!");

            if (asm.GetCustomAttribute<ExtendedVersion>() is ExtendedVersion ver)
                _currentVersion = ver.Version.GetSemVer();

            if (asm.GetCustomAttribute<ReleaseType>() is ReleaseType relType)
                _releaseType = relType.Type;
            else
                _releaseType = ERelease.NORMAL;

            _executablePath = asm.Location;

            _initialized = true;
        }
        /// <summary>Sends an HTTP GET request to the default update url.</summary>
        /// <returns>The latest version applicable to the current release channel. (Normal/PreRelease)</returns>
        private static async Task<(SemVersion, GithubReleaseHttpResponse)> GetLatestVersionMetadataAsync()
        {
            if (!_initialized)
                Initialize();

            using HttpClient client = new();
            // clear the HTTP request header
            client.DefaultRequestHeaders.Accept.Clear();
            // request JSON response format
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            // use the curl util's User-Agent specifier
            client.DefaultRequestHeaders.Add("User-Agent", "curl/7.64.1");

            // create a get task that automatically deserializes the response to an array of structs
            Task<object?>? getTask = client.GetFromJsonAsync(Settings.UpdateUri, typeof(GithubReleaseHttpResponse[]));
            // wait for the task to complete
            object? result = await getTask.ConfigureAwait(false);

            client.Dispose();

            // find the newest release in the list
            (SemVersion, GithubReleaseHttpResponse) newest = (null!, new());
            if (result is GithubReleaseHttpResponse[] releases)
            {
                foreach (GithubReleaseHttpResponse rel in releases)
                {
                    if (rel.draft || (rel.prerelease && !AllowPreReleases))
                        continue;
                    else if (rel.tag_name.GetSemVer() is SemVersion relVer && (relVer.CompareByPrecedence(newest.Item1) > 0))
                        newest = (relVer, rel);
                }
            }
            return newest;
        }
        /// <summary>
        /// Retrieves the list of releases from the Github API, and if a newer version is found a message box is shown prompting the user to update.
        /// </summary>
        /// <returns>True when the autoupdater is ready & waiting for the program to shutdown, otherwise false.</returns>
        private static async Task<bool> CheckForUpdatesAsync()
        {
            if (!_initialized)
                Initialize();
            if (_hasCheckedForUpdates || _currentVersion == null)
                return false;
            _hasCheckedForUpdates = true;
            var updateTask = GetLatestVersionMetadataAsync();
            (SemVersion newVersion, GithubReleaseHttpResponse response) = await updateTask.ConfigureAwait(false);

#           if DEBUG
            if (TEST_UPDATE)
#           else
            if (newVersion > _currentVersion)
#           endif
            {
                switch (MessageBox.Show(
                    $"Current Version:  {_currentVersion}\n" +
                    $"Newest Version:   {newVersion}\n" +
                    "\nDo you want to update now?\n\nClick 'Yes' to update now.\nClick 'No' to update later.\nClick 'Cancel' to disable update checking."
                    , "Update Available", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No))
                {
                case MessageBoxResult.Yes: // open browser
                    var asset = response.assets.First(a => a.name.Equals("VolumeControl.exe", StringComparison.OrdinalIgnoreCase));

                    Log.Info($"Updating to version {newVersion}: '{asset.browser_download_url}'");

                    if (SetupUpdateUtility() is string path)
                    {
                        Log.Info($"Automatic update utility was created at {path}");
                        ProcessStartInfo psi = new(path, $"-u \"{asset.browser_download_url}\" -o {_executablePath} -s {asset.size} {Settings.UpdateUtilityExtraArguments}")
                        {
                            ErrorDialog = true,
                            UseShellExecute = true,
                        };
                        Process.Start(psi);
                        return true;
                    }
                    else
                    {
                        Log.Error("Failed to setup automatic update utility, falling back to the default web browser.");
                        OpenBrowser(response.html_url);
                    }
                    break;
                case MessageBoxResult.No: // do nothing
                    Log.Info($"Version {newVersion} is available, but was temporarily ignored.");
                    break;
                case MessageBoxResult.Cancel: // disable
                    Settings.CheckForUpdatesOnStartup = false;
                    Log.Info("Disabled automatic updates");
                    Settings.Save();
                    Settings.Reload();
                    break;
                }
            }
            return false;
        }
        /// <summary>Opens the specified link in the default web browser.</summary>
        /// <param name="url">The URL of the page to open.</param>
        public static void OpenBrowser(string url)
        {
            if (!_initialized)
                Initialize();
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
        /// <summary>
        /// Checks for updates, and automatically installs them depending on user interaction via <see cref="MessageBox"/>.
        /// </summary>
        public static bool CheckForUpdates()
        {
            Task<bool> updateTask = CheckForUpdatesAsync();
            updateTask.Wait(-1);
            return updateTask.Result;
        }
        /// <summary>
        /// Writes the update client from the embedded resource dictionary to the local disk.
        /// </summary>
        /// <returns>The absolute filepath of the updater utility's executable.</returns>
        private static string? SetupUpdateUtility()
        {
            if (!_initialized)
                Initialize();

            var asm = Assembly.GetEntryAssembly();
            if (asm == null)
            {
                return null;
            }

            string path = Path.Combine(Path.GetDirectoryName(_executablePath) ?? string.Empty, _updateUtilityFilename);

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
                return null;
            }

            return path;
        }
        #endregion Methods
    }
}
