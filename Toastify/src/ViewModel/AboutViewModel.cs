using System;
using Toastify.Common;
using Toastify.Services;

namespace Toastify.ViewModel
{
    public class AboutViewModel : ObservableObject
    {
        private readonly Uri homepageUri;

        private string _updateUrl;

        public string ToastifyVersion { get { return $"v{App.CurrentVersionNoRevision}"; } }

        public string HomepageUrl { get; } = App.RepoInfo.Format("https://github.com/:owner/:repo");

        public string HomepageUrlNoScheme { get { return $"{this.homepageUri.Host}{this.homepageUri.PathAndQuery}"; } }

        public string UpdateUrl
        {
            get { return string.IsNullOrWhiteSpace(this._updateUrl) ? VersionChecker.GitHubReleasesUrl : this._updateUrl; }
            set { this.RaiseAndSetIfChanged(ref this._updateUrl, value); }
        }

        public AboutViewModel()
        {
            this.homepageUri = new Uri(this.HomepageUrl);
        }
    }
}