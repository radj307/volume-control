using log4net;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
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
                        {
                            Directory.Delete(UpdateDownloadPath);
                            Directory.CreateDirectory(UpdateDownloadPath);
                        }
                        string filePath = Path.Combine(UpdateDownloadPath, "ToastifyInstaller.exe");
                        await webClient.DownloadFileTaskAsync(UpdateDownloadPath, filePath).ConfigureAwait(false);

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