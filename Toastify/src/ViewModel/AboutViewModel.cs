using System;
using Toastify.Common;
using Toastify.Services;

namespace Toastify.ViewModel
{
    public class AboutViewModel : ObservableObject
    {
        private readonly Uri homepageUri;

        public string ToastifyVersion { get { return App.CurrentVersionNoRevision; } }

        public string HomepageUrl { get; } = App.RepoInfo.Format("https://github.com/:owner/:repo");

        public string HomepageUrlNoScheme { get { return $"{this.homepageUri.Host}{this.homepageUri.PathAndQuery}"; } }

        public string UpdateUrl { get { return VersionChecker.GitHubReleasesUrl; } }

        public AboutViewModel()
        {
            this.homepageUri = new Uri(this.HomepageUrl);
        }
    }
}