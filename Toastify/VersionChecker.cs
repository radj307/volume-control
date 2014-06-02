using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace Toastify
{
    class VersionChecker
    {

        public string Version { get { return "1.6.1"; } }

        private const string CURRENT_VERSION_STRING = "Toastify 1.6";
        WebClient wc;

        public event EventHandler<CheckVersionCompleteEventArgs> CheckVersionComplete;

        public VersionChecker()
        {
            wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string version = string.Empty;
            bool newVersion = false;

            if (e.Cancelled == false && e.Error == null)
            {
                var match = Regex.Match(e.Result, "Version: (?<ver>[\\d+\\.]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (match.Success)
                    version = match.Groups["ver"].Value.Trim();

                //Error prone but hey!
                newVersion = CURRENT_VERSION_STRING != version;
            }

            if (this.CheckVersionComplete != null)
                this.CheckVersionComplete(this, new CheckVersionCompleteEventArgs { Version = version, New = newVersion });
        }

        public void BeginCheckVersion()
        {
            Thread t = new Thread(ThreadedBeginCheckVersion);
            t.IsBackground = true;
            t.Start();
        }

        private void ThreadedBeginCheckVersion()
        {
            //WebClients XXXAsync isn't as async as I wanted...
            wc.DownloadStringAsync(new Uri("http://toastify.codeplex.com/wikipage?title=Version"));
        }
    }

    class CheckVersionCompleteEventArgs : EventArgs
    {
        public string Version { get; set; }
        public bool New { get; set; }
    }
}
