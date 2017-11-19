using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using Toastify.Core;
using Toastify.Helpers;
using Toastify.Model;
using Toastify.Services;
using Toastify.View;

namespace Toastify
{
    //Special entry point to allow for single instance check
    public class EntryPoint
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(EntryPoint));

        [STAThread]
        public static void Main(string[] args)
        {
            const string appSpecificGuid = "{B8F3CA50-CE27-4ffa-A812-BBE1435C9485}";
            using (Mutex unused = new Mutex(true, appSpecificGuid, out bool exclusive))
            {
                if (exclusive)
                {
                    try
                    {
                        SetupLogger();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  -  {e.ToStringInvariantCulture()}\n");
                        File.AppendAllText(Path.Combine(App.ApplicationData, "log.log"), $@"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  -  {e.ToStringInvariantCulture()}\n");
                    }

                    try
                    {
                        PrepareToRun();
                        RunApp();
                    }
                    catch (Exception e)
                    {
                        logger.Error("Unhandled top-level exception.", e);
                        Analytics.TrackException(e);
                        MessageBox.Show($"Unhandled top-level exception.\n{e.Message}", "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                    MessageBox.Show(Properties.Resources.INFO_TOASTIFY_ALREADY_RUNNING, "Toastify Already Running", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private static void SetupLogger()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Toastify.log4net.config"))
            {
                XmlConfigurator.Configure(stream);
                ILoggerRepository loggerRepository = LogManager.GetRepository();

                // Modify RollingFileAppender's destination
                var rollingFileAppender = (RollingFileAppender)loggerRepository.GetAppenders().FirstOrDefault(appender => appender.Name == "RollingFileAppender");
                if (rollingFileAppender == null)
                    throw new Exception("RollingFileAppender not found");
                rollingFileAppender.File = Path.Combine(App.ApplicationData, "Toastify.log");
                rollingFileAppender.ActivateOptions();
            }
        }

        private static void PrepareToRun()
        {
            // Load Settings > [StartupTask] > Initialize Analytics > Update PreviousVersion to this one
            LoadSettings();
            StartupTask();
            Analytics.Init();
            Settings.Current.PreviousVersion = VersionChecker.CurrentVersion;
            Settings.Current.Save();
        }

        private static void RunApp()
        {
            App app = new App();
            app.InitializeComponent();

#if DEBUG
            LastInputDebug.Start();

            Thread th = new Thread(() =>
            {
                DebugView debugView = new DebugView();
                debugView.Show();
                System.Windows.Threading.Dispatcher.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.IsBackground = true;
            th.Start();
#endif

            app.Run();
        }

        private static void LoadSettings()
        {
            try
            {
                Settings.Current.Load();
            }
            catch (Exception ex)
            {
                logger.Warn("Exception loading settings from file. Using defaults.", ex);

                string msg = string.Format(Properties.Resources.ERROR_SETTINGS_UNABLE_TO_LOAD, Settings.Current.SettingsFilePath);
                MessageBox.Show(msg, "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);

                Settings.Current.Default(true);
            }
        }

        private static void StartupTask()
        {
            if (!string.IsNullOrWhiteSpace(Settings.Current.PreviousVersion))
            {
                Version previous = new Version(Settings.Current.PreviousVersion);
                Version current = new Version(VersionChecker.CurrentVersion);

                if (previous < new Version("1.9.7"))
                {
                    // Re-enable Analytics by default
                    Settings.Current.OptInToAnalytics = true;
                    Settings.Current.Save();
                }
            }
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class App : Application
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(App));

        public static string ApplicationData
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Toastify"); }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Error("Unhandled Dispatcher exception.", e.Exception);
            Analytics.TrackException(e.Exception);
            MessageBox.Show($"Unhandled Dispatcher exception.\n{e.Exception.Message}", "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
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