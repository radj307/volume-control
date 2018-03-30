using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using Toastify.Events;
using ToastifyAPI.Helpers;

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

        #endregion Static properties

        private Thread checkVersionThread;

        public event EventHandler<CheckVersionCompleteEventArgs> CheckVersionComplete;

        protected VersionChecker()
        {
        }

        public void BeginCheckVersion()
        {
#if DEBUG
            this.checkVersionThread = null;
            this.CheckVersionComplete?.Invoke(this, new CheckVersionCompleteEventArgs { Version = CurrentVersion, New = false });
#else
            this.checkVersionThread = new Thread(this.ThreadedCheckVersion)
            {
                Name = "Toastify VersionChecker",
                IsBackground = true
            };
            this.checkVersionThread.Start();
#endif
        }

        private async void ThreadedCheckVersion()
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

        public void Dispose()
        {
            this.checkVersionThread?.Abort();
        }

        public static void DisposeInstance()
        {
            if (_instance != null)
            {
                _instance.Dispose();
                _instance = null;
            }
        }
    }
}