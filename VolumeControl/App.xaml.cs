using System;
using System.Reflection;
using System.Windows;
using VolumeControl.Helpers;
using VolumeControl.Helpers.Controls;
using VolumeControl.Log;

namespace VolumeControl
{
    public partial class App : Application
    {
        #region Constructors
        public App()
        {
            this.InitializeComponent();

            var assembly = Assembly.GetAssembly(typeof(Mixer));
            string version = $"v{assembly?.GetCustomAttribute<AssemblyAttribute.ExtendedVersion>()?.Version}";

            // Add a log handler to the dispatcher's unhandled exception event
            DispatcherUnhandledException += (s, e) => Log.Error($"An unhandled exception occurred!", $"  Sender: '{s}' ({s.GetType()})", e.Exception);

            // Tray icon
            TrayIcon = new($"Volume Control {version}", (s, e) => this.MainWindow.Visibility == Visibility.Visible);
            TrayIcon.Clicked += this.HandleTrayIconClick;
            TrayIcon.ShowClicked += this.HandleTrayIconClick;
            TrayIcon.HideClicked += (s, e) => this.HideMainWindow();
            TrayIcon.BringToFrontClicked += (s, e) => this.ActivateMainWindow();
            TrayIcon.CloseClicked += (s, e) => this.Shutdown();
        }
        #endregion Constructors

        #region Fields
        public readonly NotifyIcon TrayIcon;
        #endregion Fields

        #region Properties
        private static LogWriter Log => FLog.Log;
        #endregion Properties

        #region Methods
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            (this.TryFindResource("Settings") as VolumeControlSettings)?.Dispose();
            // delete the tray icon
            TrayIcon.Dispose();
        }
        private void HideMainWindow() => this.MainWindow.Hide();
        private void ShowMainWindow()
        {
            this.MainWindow.Show();
            this.MainWindow.WindowState = WindowState.Normal;
        }
        private void ActivateMainWindow()
        {
            this.MainWindow.Show();
            _ = this.MainWindow.Activate();
        }
        private void HandleTrayIconClick(object? sender, EventArgs e) => this.ShowMainWindow();
        #endregion Methods
    }
}
