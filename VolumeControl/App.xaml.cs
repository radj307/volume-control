using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
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

        private void PART_TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
            #region NumberKeys
            case Key.D0:
            case Key.D1:
            case Key.D2:
            case Key.D3:
            case Key.D4:
            case Key.D5:
            case Key.D6:
            case Key.D7:
            case Key.D8:
            case Key.D9:
            #endregion NumberKeys
            case Key.NumPad0:
            case Key.NumPad1:
            case Key.NumPad2:
            case Key.NumPad3:
            case Key.NumPad4:
            case Key.NumPad5:
            case Key.NumPad6:
            case Key.NumPad7:
            case Key.NumPad8:
            case Key.NumPad9:
            case Key.Back:
            case Key.Left:
            case Key.Right:
            case Key.Delete:
            case Key.Tab:
                break;
            default:
                e.Handled = true;
                break;
            }
        }
    }
}
