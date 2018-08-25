using System;

namespace Toastify.Events
{
    public class UpdateReadyEventArgs : EventArgs
    {
        #region Public Properties

        public string Version { get; set; }

        public string InstallerPath { get; set; }

        public string GitHubReleaseUrl { get; set; }

        #endregion
    }
}