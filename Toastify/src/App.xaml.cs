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
                        logger.Error("Uncaught top-level exception.", e);
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
            Settings.Instance.PreviousVersion = VersionChecker.CurrentVersion;
            Settings.Instance.Save();
        }

        private static void RunApp()
        {
            App app = new App();
            app.InitializeComponent();
            LastInputDebug.Start();
            app.Run();
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

        private static void StartupTask()
        {
            if (!string.IsNullOrWhiteSpace(Settings.Instance.PreviousVersion))
            {
                Version previous = new Version(Settings.Instance.PreviousVersion);
                Version current = new Version(VersionChecker.CurrentVersion);

                if (previous < new Version("1.9.7"))
                {
                    // Re-enable Analytics by default
                    Settings.Instance.OptInToAnalytics = true;
                    Settings.Instance.Save();
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
        public static string ApplicationData
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Toastify"); }
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