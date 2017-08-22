using System;
using Toastify.Common;
using Toastify.Services;

namespace Toastify.ViewModel
{
    public class AboutViewModel : ObservableObject
    {
        public const string HOMEPAGE_URL = "http://github.com/aleab/toastify";

        private readonly Uri homepageUri = new Uri(HOMEPAGE_URL);

        public string ToastifyVersion { get { return VersionChecker.CurrentVersion; } }

        public string HomepageUrl { get { return HOMEPAGE_URL; } }

        public string HomepageUrlNoScheme { get { return $"{this.homepageUri.Host}{this.homepageUri.PathAndQuery}"; } }

        public string UpdateUrl { get { return VersionChecker.UpdateUrl; } }
    }
}