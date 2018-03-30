using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using Toastify.Core;
using Toastify.Events;
using Toastify.Model;
using Toastify.View;

#if !DEBUG

using System.Net.Http;
using System.Threading.Tasks;
using ToastifyAPI.Helpers;

#endif

namespace Toastify.Services
{
    internal class VersionChecker : IDisposable
    {
        #region Singleton

        private static VersionChecker _instance;

        public static VersionChecker Instance
        {
            get { return _instance ?? (_instance = new VersionChecker()); }
        }

        #endregion Singleton

        #region Static properties

        private static string _version;

        public static string CurrentVersion
        {
            get
            {
                if (_version == null)
                {
                    _version = App.CurrentVersion;

                    if (_version != null)
                    {
                        Regex regex = new Regex(@"([0-9]+\.[0-9]+\.[0-9]+)(?:\.[0-9]+)*");
                        Match match = regex.Match(_version);
                        if (match.Success)
                            _version = match.Groups[1].Value;
                    }
                }

                return _version;
            }
        }

        public static string UpdateUrl { get { return "https://github.com/aleab/toastify/releases"; } }

        public static string VersionUrl { get { return "https://raw.githubusercontent.com/aleab/toastify/master/Toastify/version"; } }

        public static TimeSpan VersionCheckInterval { get { return Settings.Current.VersionCheckFrequency.Value.ToTimeSpan(); } }

        #endregion Static properties

        private Settings current;

        private readonly Timer checkVersionTimer;

        public event EventHandler<CheckVersionCompleteEventArgs> CheckVersionComplete;

        protected VersionChecker()
        {
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
            this.CheckVersionComplete?.Invoke(this, new CheckVersionCompleteEventArgs { Version = CurrentVersion, New = false });
#else
            Task.Run(async () => await this.ThreadedCheckVersion());
#endif
        }

#if !DEBUG

        private async Task ThreadedCheckVersion()
        {
            string downloadedString = null;
            try
            {
                HttpClientHandler handler = Net.CreateHttpClientHandler(App.ProxyConfig);
                using (HttpClient http = new HttpClient(handler))
                {
                    downloadedString = await http.GetStringAsync(new Uri(VersionUrl)).ConfigureAwait(false);
                }
            }
            finally
            {
                string sRemote = string.Empty;
                bool isNewVersion = false;

                if (!string.IsNullOrWhiteSpace(downloadedString))
                {
                    var match = Regex.Match(downloadedString, @"(\d+\.?)+", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (match.Success)
                    {
                        sRemote = match.Value;
                        Version vLocal = new Version(CurrentVersion);
                        Version vRemote = new Version(sRemote);
                        isNewVersion = vLocal.CompareTo(vRemote) < 0;
                    }
                }

                this.CheckVersionComplete?.Invoke(this, new CheckVersionCompleteEventArgs { Version = sRemote, New = isNewVersion });
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