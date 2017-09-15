using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Windows;
using Toastify.Core;
using Toastify.Model;
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
                    // Load Settings > Initialize Analytics > Update PreviousVersion
                    LoadSettings();
                    Analytics.Init();
                    Settings.Instance.PreviousVersion = VersionChecker.CurrentVersion;
                    
                    App app = new App();
                    app.InitializeComponent();

                    LastInputDebug.Start();

                    app.Run();
                }
                else
                    MessageBox.Show(Properties.Resources.INFO_TOASTIFY_ALREADY_RUNNING, "Toastify Already Running", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private static void LoadSettings()
        {
            try
            {
                Settings.Instance.Load();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception loading settings:\n" + ex);

                string msg = string.Format(Properties.Resources.ERROR_SETTINGS_UNABLE_TO_LOAD, Settings.Instance.SettingsFilePath);
                MessageBox.Show(msg, "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);

                Settings.Instance.Default(true);
            }
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class App : Application
    {
        public static string ApplicationData
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Toastify");
            }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Analytics.TrackException(e.Exception);
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            Analytics.TrackEvent(Analytics.ToastifyEventCategory.General, Analytics.ToastifyEvent.AppLaunch);
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Analytics.TrackEvent(Analytics.ToastifyEventCategory.General, Analytics.ToastifyEvent.AppTermination);
            Spotify.Instance?.Dispose();
        }
    }
}