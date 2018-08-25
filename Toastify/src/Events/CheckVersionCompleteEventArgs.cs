using System;

namespace Toastify.Events
{
    public class CheckVersionCompleteEventArgs : EventArgs
    {
        #region Public Properties

        public string Version { get; set; }

        public bool IsNew { get; set; }

        public int GitHubReleaseId { get; set; } = -1;

        public string GitHubReleaseUrl { get; set; }

        public string GitHubReleaseDownloadUrl { get; set; }

        #endregion
    }
}