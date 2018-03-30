using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using log4net;
using Toastify.Core;
using Toastify.Events;
using Toastify.Model;
using Toastify.View;
using ToastifyAPI.GitHub;
using ToastifyAPI.GitHub.Model;

#if !DEBUG

using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        #region Static properties

        public static string GitHubReleasesUrl { get; } = App.RepoInfo.Format("https://github.com/:owner/:repo/releases");

        public static TimeSpan VersionCheckInterval { get { return Settings.Current.VersionCheckFrequency.Value.ToTimeSpan(); } }

        #endregion Static properties

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
            this.checkVersionTimer = new Timer(state => this.CheckNow(), null, CalcCheckVersionDueTime(), VersionCheckInterval);
#endif
        }

        public void CheckNow()
        {
#if DEBUG
            this.CheckVersionComplete?.Invoke(this, new CheckVersionCompleteEventArgs { Version = App.CurrentVersionNoRevision, New = false });
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

                this.CheckVersionComplete?.Invoke(this, new CheckVersionCompleteEventArgs { Version = sRemoteVersion, New = isNewVersion });
            }
            catch (Exception ex)
            {
                logger.Error("Unknown error while checking for updates.", ex);
            }
        }

#endif

        private void CurrentSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.VersionCheckFrequency):
                    this.checkVersionTimer?.Change(CalcCheckVersionDueTime(), VersionCheckInterval);
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
            DateTime last = Settings.Current.LastVersionCheck;
            DateTime now = DateTime.Now;
            TimeSpan freq = VersionCheckInterval;

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