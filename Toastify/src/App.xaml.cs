using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using Toastify.Services;

namespace Toastify
{
    //Special entry point to allow for single instance check
    public class EntryPoint
    {
        [STAThread]
        public static void Main(string[] args)
        {
            string appSpecificGuid = "{B8F3CA50-CE27-4ffa-A812-BBE1435C9485}";
            bool exclusive;
            using (Mutex m = new Mutex(true, appSpecificGuid, out exclusive))
            {
                if (exclusive)
                {
                    App app = new App();
                    app.InitializeComponent();

                    LastInputDebug.Start();

                    app.Run();
                }
                else
                {
                    MessageBox.Show("Toastify is already running!\n\nLook for the blue icon in your system tray.", "Toastify Already Running", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Telemetry.TrackException(e.Exception);
        }
    }
}
