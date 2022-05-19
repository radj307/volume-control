using System;
using System.Reflection;
using System.Windows;
using VolumeControl.Helpers;

namespace VolumeControl
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var assembly = Assembly.GetAssembly(typeof(Mixer));
            string version = $"v{assembly?.GetCustomAttribute<Core.Attributes.ExtendedVersion>()?.Version}";

            // Tray icon
            TrayIcon = new()
            {
                Icon = VolumeControl.Properties.Resources.iconSilvered,
                Visible = true,
                Text = $"Volume Control {version}"
            };
            TrayIcon.Click += HandleTrayIconClick!;
        }

        private System.Windows.Forms.NotifyIcon TrayIcon;

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            TrayIcon.Click -= HandleTrayIconClick!;
            (FindResource("Settings") as VolumeControlSettings)?.Dispose();
            CleanupTrayIcon();
        }

        private void HandleTrayIconClick(object sender, EventArgs e)
        {
            Current.MainWindow.Show();
            Current.MainWindow.WindowState = WindowState.Normal;
        }

        public void CleanupTrayIcon()
        {
            if (TrayIcon != null)
            {
                TrayIcon.Visible = false;
                TrayIcon.Dispose();
                TrayIcon = null!;
            }
        }
    }
}
