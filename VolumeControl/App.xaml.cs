using System;
using System.Reflection;
using System.Windows;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.Helpers.Controls;

namespace VolumeControl
{
    public partial class App : Application
    {
        #region Constructors
        public App()
        {
            InitializeComponent();

            var assembly = Assembly.GetAssembly(typeof(Mixer));
            string version = $"v{assembly?.GetCustomAttribute<AssemblyAttribute.ExtendedVersion>()?.Version}";

            // Add a log handler to the dispatcher's unhandled exception event
            DispatcherUnhandledException += (s, e) => Log.Error($"An unhandled exception occurred!", $"  Sender: '{s}' ({s.GetType()})", e.Exception);

            // Tray icon
            TrayIcon = new($"Volume Control {version}", (s, e) => MainWindow.Visibility == Visibility.Visible);
            TrayIcon.Clicked += HandleTrayIconClick;
            TrayIcon.ShowClicked += HandleTrayIconClick;
            TrayIcon.HideClicked += (s, e) => HideMainWindow();
            TrayIcon.BringToFrontClicked += (s, e) => ActivateMainWindow();
            TrayIcon.CloseClicked += (s, e) => Shutdown();
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
            (TryFindResource("Settings") as VolumeControlSettings)?.Dispose();
            // delete the tray icon
            TrayIcon.Dispose();
        }
        private void HideMainWindow() => MainWindow.Hide();
        private void ShowMainWindow()
        {
            MainWindow.Show();
            MainWindow.WindowState = WindowState.Normal;
        }
        private void ActivateMainWindow() => MainWindow.Activate();
        private void HandleTrayIconClick(object? sender, EventArgs e) => ShowMainWindow();
        #endregion Methods
    }
}
