using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VolumeControl.Controls;
using VolumeControl.Core;
using VolumeControl.Helpers;
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
            DispatcherUnhandledException += this.App_DispatcherUnhandledException;

            // setup the tray icon
            TrayIcon = new(() => this.MainWindow.Visibility == Visibility.Visible)
            {
                Tooltip = $"Volume Control {version}",
                Background = (System.Windows.Media.Color)FindResource("ContextMenuBackgroundColor"),
            };
            TrayIcon.DoubleClick += (s, e) =>
            {
                if (this.MainWindow.IsVisible)
                    this.HideMainWindow();
                else
                {
                    this.ShowMainWindow();
                    MainWindow.Activate();
                }
            };
            TrayIcon.ShowClicked += (s, e) => this.ShowMainWindow();
            TrayIcon.HideClicked += (s, e) => this.HideMainWindow();
            TrayIcon.BringToFrontClicked += (s, e) => this.ActivateMainWindow();

            TrayIcon.OpenConfigClicked += (s, e) =>
            {
                ShellHelper.OpenWithDefault(Config.Default.Location);
            };
            TrayIcon.OpenLogClicked += (s, e) =>
            {
                ShellHelper.OpenWithDefault(Config.Default.LogPath);
            };

            TrayIcon.OpenLocationClicked += (s, e) =>
            {
                ShellHelper.OpenFolderAndSelectItem(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), Path.ChangeExtension(AppDomain.CurrentDomain.FriendlyName, ".exe"))));
            };
            TrayIcon.OpenAppDataClicked += (s, e) =>
            {
                ShellHelper.OpenWith("explorer", PathFinder.ApplicationAppDataPath);
            };
            TrayIcon.CloseClicked += (s, e) => this.Shutdown();
            TrayIcon.Visible = true;
        }
        #endregion Constructors

        #region Fields
        public readonly VolumeControlNotifyIcon TrayIcon;
        #endregion Fields

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

        #region App
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            FLog.Critical(
                $"An unhandled exception occurred in the application dispatcher!",
                $"Sender: \"{sender}\" ({sender.GetType()})",
                $"Thread: \"{e.Dispatcher.Thread.Name}\"",
                e.Exception);
#if DEBUG
            throw new Exception("Unhandled dispatcher exception!", e.Exception);
#endif
        }
        #endregion App

        #region PART_TextBox
        /// <summary>
        /// Prevents non-numeric or control keys from being received as input by the textbox.<br/>
        /// This is used within NumericUpDownStyle
        /// </summary>
        private void PART_TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
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
        #endregion PART_TextBox

        #region TextBox
        private void TextBox_Loaded_AttachEscapeBehavior(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;

            WPF.Behaviors.EscapeRemovesFocusBehavior escapeRemovesFocusBehavior = new();
            escapeRemovesFocusBehavior.Attach(textBox);
            Microsoft.Xaml.Behaviors.Interaction.GetBehaviors(textBox).Add(escapeRemovesFocusBehavior);
        }
        #endregion TextBox

        #region ScrollViewer
        private void ScrollViewer_Loaded_AttachHorizontalScrollBehavior(object sender, RoutedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;

            WPF.Behaviors.ScrollViewerHorizontalScrollBehavior behavior = new();
            behavior.Attach(scrollViewer);
            Microsoft.Xaml.Behaviors.Interaction.GetBehaviors(scrollViewer).Add(behavior);
        }
        #endregion ScrollViewer

        #endregion EventHandlers
    }
}
