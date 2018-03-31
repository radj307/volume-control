using log4net;
using System;
using System.IO;
using System.Net;
using Toastify.Core;
using Toastify.Events;
using Toastify.Model;

namespace Toastify.Services
{
    /// <summary>
    /// Toastify auto-updater component
    /// </summary>
    /// <remarks>
    /// The installation part of the update is handled in <see cref="EntryPoint.AutoUpdater_UpdateReady"/>.
    /// </remarks>
    internal class AutoUpdater
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(AutoUpdater));

        #region Singleton

        private static AutoUpdater _instance;

        public static AutoUpdater Instance
        {
            get { return _instance ?? (_instance = new AutoUpdater()); }
        }

        #endregion Singleton

        public static string UpdateDownloadPath { get; } = Path.Combine(App.LocalApplicationData, "upd");

        public event EventHandler<CheckVersionCompleteEventArgs> AutoUpdateFailed;

        public event EventHandler<UpdateReadyEventArgs> UpdateReady;

        protected AutoUpdater()
        {
            VersionChecker.Instance.CheckVersionComplete += this.VersionChecker_CheckVersionComplete;

            this.VersionChecker_CheckVersionComplete(this, new CheckVersionCompleteEventArgs
            {
                GitHubReleaseDownloadUrl = "https://github.com/aleab/toastify/releases/download/v1.10.5/ToastifyInstaller.exe",
                Version = "v1.10.5",
                GitHubReleaseUrl = "https://github.com/aleab/toastify/releases/tag/v1.10.5",
                IsNew = true
            });
        }

        private static bool ShouldDownload(UpdateDeliveryMode updateDeliveryMode)
        {
            return updateDeliveryMode == UpdateDeliveryMode.AutoDownload;
        }

        private async void VersionChecker_CheckVersionComplete(object sender, CheckVersionCompleteEventArgs e)
        {
            if (!ShouldDownload(Settings.Current.UpdateDeliveryMode) || !e.IsNew)
                return;

            if (string.IsNullOrWhiteSpace(e.GitHubReleaseDownloadUrl))
                this.AutoUpdateFailed?.Invoke(this, e);
            else
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Proxy = App.ProxyConfig.CreateWebProxy();
                    try
                    {
                        if (Directory.Exists(UpdateDownloadPath))
                            Directory.Delete(UpdateDownloadPath, true);
                        Directory.CreateDirectory(UpdateDownloadPath);
                        string filePath = Path.Combine(UpdateDownloadPath, "ToastifyInstaller.exe");
                        await webClient.DownloadFileTaskAsync(e.GitHubReleaseDownloadUrl, filePath).ConfigureAwait(false);

                        if (File.Exists(filePath))
                        {
                            UpdateReadyEventArgs updateReadyEventArgs = new UpdateReadyEventArgs
                            {
                                Version = e.Version,
                                InstallerPath = filePath,
                                GitHubReleaseUrl = e.GitHubReleaseUrl
                            };
                            this.UpdateReady?.Invoke(this, updateReadyEventArgs);
                        }
                        else
                            this.AutoUpdateFailed?.Invoke(this, e);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Unknown error while downloading new update.", ex);
                        this.AutoUpdateFailed?.Invoke(this, e);
                    }
                }
            }
        }
    }
}