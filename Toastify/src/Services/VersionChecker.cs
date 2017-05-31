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

        public static string Version
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

        public string UpdateUrl { get { return "https://toastify.codeplex.com/releases/view/24273"; } }

        public string VersionUrl { get { return "http://toastify.codeplex.com/wikipage?title=Version"; } }

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
            this.wc.DownloadStringAsync(new Uri(this.VersionUrl));
        }

        private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string version = string.Empty;
            bool newVersion = false;

            if (!e.Cancelled && e.Error == null)
            {
                var match = Regex.Match(e.Result, "Version: (?<ver>[\\d+\\.]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (match.Success)
                {
                    version = match.Groups["ver"].Value.Trim();
                    newVersion = Version != version;
                }
            }

            this.CheckVersionComplete?.Invoke(this, new CheckVersionCompleteEventArgs { Version = version, New = newVersion });
        }
    }
}