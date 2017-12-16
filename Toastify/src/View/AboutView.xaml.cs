using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
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
        public AboutView()
        {
            this.InitializeComponent();

            this.DataContext = new AboutViewModel();
        }

        private void VersionChecker_CheckVersionComplete(object sender, CheckVersionCompleteEventArgs e)
        {
            string latestVersionText;

            if (string.IsNullOrEmpty(e.Version))
                latestVersionText = "Unable to check version.";
            else if (e.New)
                latestVersionText = "New version available!";
            else
                latestVersionText = "You have the latest version.";

            this.Dispatcher.Invoke(() =>
            {
                Run run = new Run(latestVersionText);
                this.LatestVersion.Inlines.Clear();
                this.LatestVersion.Inlines.Add(run);
            }, System.Windows.Threading.DispatcherPriority.Normal);
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VersionChecker.Instance.CheckVersionComplete += this.VersionChecker_CheckVersionComplete;
            VersionChecker.Instance.BeginCheckVersion();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            VersionChecker.Instance.CheckVersionComplete -= this.VersionChecker_CheckVersionComplete;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;

            this.Close();
        }
    }
}