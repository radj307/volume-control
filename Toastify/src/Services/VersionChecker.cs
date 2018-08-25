using System;
using System.ComponentModel;
using System.Threading;
using log4net;
using Toastify.Core;
using Toastify.Events;
using Toastify.Model;
using Toastify.View;
using ToastifyAPI.GitHub;

#if !DEBUG
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToastifyAPI.GitHub.Model;

#endif

namespace Toastify.Services
{
    internal class VersionChecker : IDisposable
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(VersionChecker));

        #region Singleton

        private static VersionChecker _instance;

        public static VersionChecker Instance
        {
            get { return _instance ?? (_instance = new VersionChecker()); }
        }

        #endregion Singleton

        public static string GitHubReleasesUrl { get; } = App.RepoInfo.Format("https://github.com/:owner/:repo/releases");

        private readonly GitHubAPI gitHubAPI;

        private Settings current;
        private readonly Timer checkVersionTimer;

        public event EventHandler<CheckVersionCompleteEventArgs> CheckVersionComplete;

        protected VersionChecker()
        {
            this.gitHubAPI = new GitHubAPI(App.ProxyConfig);

            this.current = Settings.Current;
            this.current.PropertyChanged += this.CurrentSettings_PropertyChanged;
            SettingsView.SettingsSaved += this.SettingsView_SettingsSaved;

#if DEBUG
            this.checkVersionTimer = null;
#else
            this.checkVersionTimer = new Timer(state => this.CheckNow(), null, CalcCheckVersionDueTime().Add(TimeSpan.FromMinutes(2.5)), new TimeSpan(-1));
#endif
        }

        public void CheckNow()
        {
#if DEBUG
            this.CheckVersionComplete?.Invoke(this, new CheckVersionCompleteEventArgs { Version = App.CurrentVersionNoRevision, IsNew = false });
#else
            Task.Run(() => this.CheckVersion());
#endif
        }

#if !DEBUG
        private void CheckVersion()
        {
            try
            {
                string latestTagName = null;
                Release latest = this.gitHubAPI.GetLatestRelease(App.RepoInfo);
                if (latest.HttpStatusCode == HttpStatusCode.OK)
                    latestTagName = latest.TagName;

                string sRemoteVersion = string.Empty;
                bool isNewVersion = false;

                if (!string.IsNullOrWhiteSpace(latestTagName))
                {
                    var match = Regex.Match(latestTagName, @"(\d+\.?)+", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (match.Success)
                    {
                        sRemoteVersion = match.Value;
                        Version localVersion = new Version(App.CurrentVersionNoRevision);
                        Version remoteVersion = new Version(sRemoteVersion);
                        isNewVersion = localVersion.CompareTo(remoteVersion) < 0;
                    }
                }

                CheckVersionCompleteEventArgs e = new CheckVersionCompleteEventArgs
                {
                    Version = sRemoteVersion,
                    IsNew = isNewVersion,
                    GitHubReleaseId = latest.Id,
                    GitHubReleaseUrl = latest.HtmlUrl,
                    GitHubReleaseDownloadUrl = latest.Assets.FirstOrDefault(asset => asset.Name == "ToastifyInstaller.exe")?.DownloadUrl
                };
                this.CheckVersionComplete?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                logger.Error("Unknown error while checking for updates.", ex);
            }
            finally
            {
                Settings.Current.LastVersionCheck = DateTime.Now;
                Settings.Current.Save();

                this.checkVersionTimer?.Change(CalcCheckVersionDueTime(), new TimeSpan(-1));
            }
        }

#endif

        private void CurrentSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.VersionCheckFrequency):
                    this.checkVersionTimer?.Change(CalcCheckVersionDueTime(), new TimeSpan(-1));
                    break;

                default:
                    // Ignore!
                    break;
            }
        }

        private void SettingsView_SettingsSaved(object sender, SettingsSavedEventArgs e)
        {
            this.current.PropertyChanged -= this.CurrentSettings_PropertyChanged;
            this.current = e.Settings;
            this.current.PropertyChanged += this.CurrentSettings_PropertyChanged;
        }

        public void Dispose()
        {
            this.current.PropertyChanged -= this.CurrentSettings_PropertyChanged;
            SettingsView.SettingsSaved -= this.SettingsView_SettingsSaved;
        }

        public static void DisposeInstance()
        {
            if (_instance != null)
            {
                _instance.Dispose();
                _instance = null;
            }
        }

        private static TimeSpan CalcCheckVersionDueTime()
        {
            DateTime last = Settings.Current.LastVersionCheck ?? default(DateTime);
            DateTime now = DateTime.Now;
            TimeSpan freq = Settings.Current.VersionCheckFrequency.Value.ToTimeSpan();

            if (last.Equals(default(DateTime)))
            {
                last = DateTime.Now;
                Settings.Current.LastVersionCheck = last;
                Settings.Current.Save();
            }

            TimeSpan t = now.Subtract(last);
            return t >= freq ? TimeSpan.Zero : freq.Subtract(t);
        }
    }
}