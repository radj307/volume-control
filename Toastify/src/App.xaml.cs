using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using Toastify.Core;
using Toastify.Services;

namespace Toastify
{
    //Special entry point to allow for single instance check
    public class EntryPoint
    {
        [STAThread]
        public static void Main(string[] args)
        {
            const string appSpecificGuid = "{B8F3CA50-CE27-4ffa-A812-BBE1435C9485}";
            using (Mutex unused = new Mutex(true, appSpecificGuid, out bool exclusive))
            {
                if (exclusive)
                {
                    App app = new App();
                    app.InitializeComponent();

                    LastInputDebug.Start();

                    app.Run();
                }
                else
                    MessageBox.Show("Toastify is already running!\n\nLook for the blue icon in your system tray.", "Toastify Already Running", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Telemetry.TrackException(e.Exception);
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Spotify.Instance?.Dispose();
        }
    }
}