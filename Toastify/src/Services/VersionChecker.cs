using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Toastify.Events;

namespace Toastify.Services
{
    internal class VersionChecker
    {
        private static string _version;

        public static string CurrentVersion
        {
            get
            {
                if (_version == null)
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    if (assembly.Location != null)
                    {
                        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                        _version = fileVersionInfo.FileVersion;
                    }

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

        private readonly WebClient wc;

        public event EventHandler<CheckVersionCompleteEventArgs> CheckVersionComplete;

        public VersionChecker()
        {
            this.wc = new WebClient();
            this.wc.DownloadStringCompleted += this.WebClient_DownloadStringCompleted;
        }

        public void BeginCheckVersion()
        {
            Thread t = new Thread(this.ThreadedBeginCheckVersion) { IsBackground = true };
            t.Start();
        }

        private void ThreadedBeginCheckVersion()
        {
            //WebClients XXXAsync isn't as async as I wanted...
            this.wc.DownloadStringAsync(new Uri(VersionUrl));
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
    }
}