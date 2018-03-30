using log4net;
using System;
using System.Media;
using System.Net;
using System.Threading;
using System.Windows;
using Toastify.ViewModel;
using ToastifyAPI.GitHub;
using ToastifyAPI.GitHub.Model;
using Dispatch = System.Windows.Threading.Dispatcher;

namespace Toastify.View
{
    public partial class ChangelogView : Window
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ChangelogView));

        private static Thread changelogViewerThread;

        private readonly ChangelogViewModel viewModel;

        public ChangelogView()
        {
            this.InitializeComponent();

            this.viewModel = new ChangelogViewModel();
            this.DataContext = this.viewModel;
        }

        internal static void Launch()
        {
            if (changelogViewerThread != null)
                return;

            if (logger.IsDebugEnabled)
                logger.Debug("Launching ChangelogViewer...");

            changelogViewerThread = new Thread(() =>
            {
                ChangelogView view = new ChangelogView();
                view.DownloadChangelog();
                view.Show();
                SystemSounds.Asterisk.Play();
                Dispatch.Run();
            })
            {
                Name = "Changelog Viewer",
                IsBackground = true
            };
            changelogViewerThread.SetApartmentState(ApartmentState.STA);
            changelogViewerThread.Start();
        }

        private void DownloadChangelog()
        {
            logger.Info("Downloading latest changelog...");

            GitHubAPI gitHubAPI = new GitHubAPI(App.ProxyConfig)
            {
                Owner = "aleab",
                Repository = "toastify"
            };
            Release release = gitHubAPI.GetReleaseByTagName(App.CurrentVersion);
            if (release.HttpStatusCode != HttpStatusCode.OK)
                release = gitHubAPI.GetLatestRelease();

            if (release.HttpStatusCode == HttpStatusCode.OK)
            {
                this.viewModel.ReleaseBodyMarkdown = $"## {release.Name}\n" + 
                                                     $"{gitHubAPI.GitHubify(release.Body)}";

                if (release.PublishedAt != null)
                    this.TextBlockPublishedDate.Text = release.PublishedAt?.ToString(App.UserCulture);
                else
                    this.PanelPublished.Visibility = Visibility.Collapsed;

                logger.Info("Changelog downloaded");
            }
            else
            {
                this.viewModel.ReleaseBodyMarkdown = "### Failed loading the changelog!\n" +
                                                     $"You can read the latest changes [here]({gitHubAPI.GetUrlOfLatestRelease()}).";
                this.PanelPublished.Visibility = Visibility.Collapsed;

                logger.Warn($"Failed to download the latest changelog. StatusCode = {release.HttpStatusCode}");
            }
        }

        private void ChangelogView_OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void ChangelogView_OnClosed(object sender, EventArgs e)
        {
            changelogViewerThread = null;
        }
    }
}