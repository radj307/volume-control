using log4net;
using System;
using System.Media;
using System.Net;
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

        private static ChangelogView current;

        private readonly ChangelogViewModel viewModel;

        public ChangelogView()
        {
            this.InitializeComponent();

            this.viewModel = new ChangelogViewModel();
            this.DataContext = this.viewModel;
        }

        internal static void Launch()
        {
            if (current != null)
                return;

            if (logger.IsDebugEnabled)
                logger.Debug("Launching ChangelogViewer...");

            current = new ChangelogView();
            App.CallInSTAThreadAsync(() =>
            {
                current.DownloadChangelog();
                current.Show();
                SystemSounds.Asterisk.Play();
                Dispatch.Run();
            }, true, "Changelog Viewer");
        }

        private void DownloadChangelog()
        {
            logger.Info("Downloading latest changelog...");

            GitHubAPI gitHubAPI = new GitHubAPI(App.ProxyConfig);
            Release release = gitHubAPI.GetReleaseByTagName(App.RepoInfo, App.CurrentVersion);
            if (release.HttpStatusCode != HttpStatusCode.OK)
                release = gitHubAPI.GetLatestRelease(App.RepoInfo);

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
                                                     $"You can read the latest changes [here]({Releases.GetUrlOfLatestRelease(App.RepoInfo)}).";
                this.PanelPublished.Visibility = Visibility.Collapsed;

                logger.Warn($"Failed to download the latest changelog. StatusCode = {release.HttpStatusCode}");
            }
        }

        private void ChangelogView_OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void ChangelogView_OnClosed(object sender, EventArgs e)
        {
        }
    }
}