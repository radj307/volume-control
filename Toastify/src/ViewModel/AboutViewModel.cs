using System;
using Toastify.Common;
using Toastify.Services;

namespace Toastify.ViewModel
{
    public class AboutViewModel : ObservableObject
    {
        private readonly Uri homepageUri;

        public string ToastifyVersion { get { return VersionChecker.CurrentVersion; } }

        public string HomepageUrl { get; } = "http://github.com/aleab/toastify";

        public string HomepageUrlNoScheme { get { return $"{this.homepageUri.Host}{this.homepageUri.PathAndQuery}"; } }

        public string UpdateUrl { get { return VersionChecker.UpdateUrl; } }

        public AboutViewModel()
        {
            this.homepageUri = new Uri(this.HomepageUrl);
        }
    }
}