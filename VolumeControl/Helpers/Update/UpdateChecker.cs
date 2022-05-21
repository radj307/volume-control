using Semver;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using VolumeControl.Core.Extensions;

namespace VolumeControl.Helpers.Update
{
    internal static class UpdateChecker
    {
        private static Properties.Settings Settings => Properties.Settings.Default;

        /// <summary>Sends an HTTP GET request to the default update url.</summary>
        /// <param name="currentVersionString">The current version number as a string.</param>
        /// <param name="allowPreReleases">Whether or not to allow returning PreRelease versions.</param>
        /// <returns>The latest version applicable to the current release channel. (Normal/PreRelease)</returns>
        public static async Task<(SemVersion, GithubReleaseHttpResponse)?> CheckForUpdates(string currentVersionString, bool allowPreReleases)
        {
            SemVersion? currentVersion = currentVersionString.GetSemVer();
            if (!Settings.CheckForUpdatesOnStartup || currentVersion == null)
            {
                return null;
            }

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
            (SemVersion, GithubReleaseHttpResponse)? newest = null;
            if (result is GithubReleaseHttpResponse[] releases)
            {
                foreach (GithubReleaseHttpResponse rel in releases)
                {
                    if (rel.draft || (rel.prerelease && !allowPreReleases))
                    {
                        continue;
                    }
                    else if (rel.tag_name.GetSemVer() is SemVersion relVer && (!newest.HasValue || relVer.CompareByPrecedence(newest.Value.Item1) > 0))
                    {
                        newest = (relVer, rel);
                    }
                }
            }
            if (newest is (SemVersion, GithubReleaseHttpResponse) newestReleasePair && newestReleasePair.Item1.CompareByPrecedence(currentVersion) > 0)
            {
                return newest;
            }

            return null;
        }
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
    }
}
