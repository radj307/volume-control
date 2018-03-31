using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using Toastify.Events;
using Toastify.Services;
using Toastify.ViewModel;

namespace Toastify.View
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class AboutView : Window
    {
        private readonly AboutViewModel viewModel;

        public AboutView()
        {
            this.InitializeComponent();

            this.viewModel = new AboutViewModel();
            this.DataContext = this.viewModel;
        }

        private void VersionChecker_CheckVersionComplete(object sender, CheckVersionCompleteEventArgs e)
        {
            string latestVersionText;

            if (string.IsNullOrEmpty(e.Version))
                latestVersionText = "Unable to check for updates";
            else if (e.IsNew)
                latestVersionText = "New version available!";
            else
                latestVersionText = "You have the latest version";

            this.viewModel.UpdateUrl = e.GitHubReleaseUrl;

            this.Dispatcher.Invoke(() =>
            {
                Run run = new Run(latestVersionText);
                this.LatestVersion.Inlines.Clear();
                this.LatestVersion.Inlines.Add(run);
            }, DispatcherPriority.Render);
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VersionChecker.Instance.CheckVersionComplete += this.VersionChecker_CheckVersionComplete;
            VersionChecker.Instance.CheckNow();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            VersionChecker.Instance.CheckVersionComplete -= this.VersionChecker_CheckVersionComplete;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true };
            Process.Start(psi);
            e.Handled = true;

            this.Close();
        }
    }
}