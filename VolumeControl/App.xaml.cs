using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using VolumeControl.Controls;
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
            DispatcherUnhandledException += (s, e) =>
            {
                FLog.Error($"An unhandled exception occurred!", $"Sender: '{s}' ({s.GetType()})", e.Exception);
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
                OpenFolderAndSelectItem(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), Path.ChangeExtension(AppDomain.CurrentDomain.FriendlyName, ".exe"))));
            };
            TrayIcon.OpenAppDataClicked += (s, e) =>
            {
                Process.Start("explorer", PathFinder.ApplicationAppDataPath);
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
        private void HandleTrayIconClick(object? sender, EventArgs e)
        {
            if (this.MainWindow.IsVisible)
                this.HideMainWindow();
            else
            {
                this.ShowMainWindow();
                MainWindow.Activate();
            }
        }
        #region OpenFolderAndSelectItem
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr bindingContext, [Out] out IntPtr pidl, uint sfgaoIn, [Out] out uint psfgaoOut);
        private static void OpenFolderAndSelectItem(string filePath)
        {
            if (Path.GetDirectoryName(filePath) is not string directoryPath)
            {
                FLog.Error($"Cannot get directory from path '{filePath}'!");
                return;
            }

            SHParseDisplayName(directoryPath, IntPtr.Zero, out IntPtr nativeFolder, 0, out uint psfgaoOut);

            if (nativeFolder == IntPtr.Zero)
            {
                FLog.Error($"Cannot locate directory '{directoryPath}'!");
                return;
            }

            SHParseDisplayName(filePath, IntPtr.Zero, out IntPtr nativeFile, 0, out psfgaoOut);

            IntPtr[] fileArray;
            if (nativeFile == IntPtr.Zero)
            {
                // Open the folder without the file selected if we can't find the file
                fileArray = Array.Empty<IntPtr>();
            }
            else
            {
                fileArray = new IntPtr[] { nativeFile };
            }

            FLog.Debug($"Opening and selecting '{filePath}' in the file explorer.");

            _ = SHOpenFolderAndSelectItems(nativeFolder, (uint)fileArray.Length, fileArray, 0);

            Marshal.FreeCoTaskMem(nativeFolder);
            if (nativeFile != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(nativeFile);
            }
        }
        #endregion OpenFolderAndSelectItem
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

        #endregion EventHandlers
    }
}
