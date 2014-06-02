using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;

namespace Toastify
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        VersionChecker versionChecker;

        public About()
        {
            InitializeComponent();

            versionChecker = new VersionChecker();
            versionChecker.CheckVersionComplete += new EventHandler<CheckVersionCompleteEventArgs>(versionChecker_CheckVersionComplete);

            this.DataContext = versionChecker;
        }

        void versionChecker_CheckVersionComplete(object sender, CheckVersionCompleteEventArgs e)
        {
            string latestVersionText = "";

            if (string.IsNullOrEmpty(e.Version))
                latestVersionText = "Unable to check version.";
            else if (e.New)
                latestVersionText = "New version available!";
            else
                latestVersionText = "You have the latest version.";

            this.Dispatcher.Invoke(new Action(() => 
            {
                Run run = new Run(latestVersionText);
                LatestVersion.Inlines.Clear();
                LatestVersion.Inlines.Add(run);
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        private void CodeplexLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://toastify.codeplex.com");
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            versionChecker.BeginCheckVersion();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;

            this.Close();
        }
    }
}
