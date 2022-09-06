using System;
using System.Reflection;
using System.Windows;
using VolumeControl.Controls;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.WPF;

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
            DispatcherUnhandledException += (s, e) =>
            {
                Log.Error($"An unhandled exception occurred!", $"Sender: '{s}' ({s.GetType()})", e.Exception);
                e.Handled = true;
            };

            // setup the tray icon
            TrayIcon = new(() => this.MainWindow.Visibility == Visibility.Visible)
            {
                Tooltip = $"Volume Control {version}"
            };
            TrayIcon.DoubleClick += this.HandleTrayIconClick;
            TrayIcon.ShowClicked += this.HandleTrayIconClick;
            TrayIcon.HideClicked += (s, e) => this.HideMainWindow();
            TrayIcon.BringToFrontClicked += (s, e) => this.ActivateMainWindow();
            TrayIcon.CloseClicked += (s, e) => this.Shutdown();
            TrayIcon.Visible = true;

            // Initialize the list notification window so it can appear
            _ = WindowHandleGetter.GetWindowHandle((this.FindResource("Notification") as ListNotification)!);
        }
        #endregion Constructors

        #region Fields
        public readonly VolumeControlNotifyIcon TrayIcon;
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
        private void HandleTrayIconClick(object? sender, EventArgs e)
        {
            if (this.MainWindow.IsVisible)
                this.HideMainWindow();
            else
                this.ShowMainWindow();
        }
        #endregion Methods
    }
}
