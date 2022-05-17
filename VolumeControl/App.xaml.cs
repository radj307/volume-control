using System.Windows;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.Properties;

namespace VolumeControl
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            (FindResource("Settings") as VolumeControlSettings)?.Dispose();
        }
    }
}
