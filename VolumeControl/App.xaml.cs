using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using VolumeControl.Controls;
using VolumeControl.Log;
using VolumeControl.ViewModels;

namespace VolumeControl
{
    public partial class App : Application
    {
        #region Constructors
        public App()
        {
            this.InitializeComponent();

            var assembly = Assembly.GetAssembly(typeof(Mixer));
            string version = $"v{assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}";

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
            TrayIcon.OpenLocationClicked += (s, e) =>
            {
                Process.Start(new ProcessStartInfo("explorer", $" /select, \"{Path.GetFullPath(Assembly.GetEntryAssembly()?.Location ?? AppDomain.CurrentDomain.BaseDirectory).Replace("dll", "exe")}\""));
            };
            TrayIcon.CloseClicked += (s, e) => this.Shutdown();
            TrayIcon.Visible = true;
        }
        #endregion Constructors

        #region Fields
        public readonly VolumeControlNotifyIcon TrayIcon;
        #endregion Fields

        #region Properties
        private static LogWriter Log => FLog.Log;
        #endregion Properties

        #region Methods
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

        #region EventHandlers

        #region Application
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            (this.TryFindResource("Settings") as VolumeControlVM)?.Dispose();
            // delete the tray icon
            TrayIcon.Dispose();
        }
        #endregion Application

        #endregion EventHandlers
    }
}
