using System;
using System.Reflection;
using System.Windows;
using VolumeControl.Helpers;
using VolumeControl.Log;

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

            // Tray icon
            TrayIcon = new($"Volume Control {version}", (s, e) => MainWindow.Visibility == Visibility.Visible);
            TrayIcon.Clicked += HandleTrayIconClick;
            TrayIcon.BringToFrontClicked += HandleTrayIconClick;
            TrayIcon.SendToBackClicked += (s, e) => HideMainWindow();
            TrayIcon.CloseButtonClicked += (s, e) => HandleApplicationExit();

            DispatcherUnhandledException += (s, e) => Log.Error($"An unhandled exception occurred!", $"  Sender: '{s}' ({s.GetType()})", e.Exception);
        }
        #endregion Constructors

        #region Fields
        public readonly NotifyIcon TrayIcon;
        #endregion Fields

        #region Properties
        private static LogWriter Log => FLog.Log;
        #endregion Properties

        #region Methods
        private void HandleApplicationExit()
        {
            TrayIcon.Clicked -= HandleTrayIconClick!;
            (FindResource("Settings") as VolumeControlSettings)?.Dispose();
            TrayIcon.Dispose();
        }
        private void Application_Exit(object sender, ExitEventArgs e) => HandleApplicationExit();

        private void HideMainWindow() => MainWindow.Hide();
        private void ShowMainWindow()
        {
            MainWindow.Show();
            MainWindow.WindowState = WindowState.Normal;
        }
        private void HandleTrayIconClick(object? sender, EventArgs e) => ShowMainWindow();
        #endregion Methods
    }
}
