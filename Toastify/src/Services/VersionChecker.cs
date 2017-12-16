using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Toastify.Events;

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
                    var assembly = Assembly.GetExecutingAssembly();
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                    _version = fileVersionInfo.FileVersion;

                    if (_version != null)
                    {
                        var thirdDot = _version.LastIndexOf('.');
                        _version = _version.Substring(0, thirdDot);
                    }
                }

                return _version;
            }
        }

        public static string UpdateUrl { get { return "https://github.com/aleab/toastify/releases"; } }

        public static string VersionUrl { get { return "https://raw.githubusercontent.com/aleab/toastify/master/Toastify/version"; } }

        #endregion Static properties

        private readonly WebClient webClient;

        private Thread checkVersionThread;

        public event EventHandler<CheckVersionCompleteEventArgs> CheckVersionComplete;

        protected VersionChecker()
        {
            this.webClient = new WebClient();
            this.webClient.DownloadStringCompleted += this.WebClient_DownloadStringCompleted;
        }

        public void BeginCheckVersion()
        {
#if DEBUG || TEST_RELEASE
            this.checkVersionThread = null;
            this.CheckVersionComplete?.Invoke(this, new CheckVersionCompleteEventArgs { Version = CurrentVersion, New = false });
#else
            this.checkVersionThread = new Thread(this.ThreadedBeginCheckVersion) { IsBackground = true };
            this.checkVersionThread.Start();
#endif
        }

        private void ThreadedBeginCheckVersion()
        {
            //WebClients XXXAsync isn't as async as I wanted...
            this.webClient.DownloadStringAsync(new Uri(VersionUrl));
        }

        private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string version = string.Empty;
            bool newVersion = false;

            if (!e.Cancelled && e.Error == null)
            {
                var match = Regex.Match(e.Result, "(\\d+\\.?)+", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (match.Success)
                {
                    version = match.Value;
                    Version localV = new Version(CurrentVersion);
                    Version remoteV = new Version(version);
                    newVersion = localV.CompareTo(remoteV) < 0;
                }
            }

            this.CheckVersionComplete?.Invoke(this, new CheckVersionCompleteEventArgs { Version = version, New = newVersion });
        }

        public void Dispose()
        {
            if (this.webClient != null)
            {
                this.webClient.DownloadStringCompleted -= this.WebClient_DownloadStringCompleted;
                this.webClient.CancelAsync();
                this.webClient.Dispose();
            }

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