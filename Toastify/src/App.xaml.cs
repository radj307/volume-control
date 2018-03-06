using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Xml.Serialization;
using Toastify.Core;
using Toastify.Model;
using Toastify.Services;

#if DEBUG

using log4net.Core;
using log4net.Repository.Hierarchy;
using Toastify.View;

#endif

namespace Toastify
{
    //Special entry point to allow for single instance check
    public static class EntryPoint
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(EntryPoint));

        private static string spotifyArgs = string.Empty;

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
                        ProcessCommandLineArguments(args.ToList());
                        SetupCultureInfo();
                        SetupLogger();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  -  {e}\n");
                        File.AppendAllText(Path.Combine(App.LocalApplicationData, "log.log"), $@"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  -  {e}\n");
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

        private static void ProcessCommandLineArguments(IEnumerable<string> args)
        {
            // TODO: Logger command line parameters (change minimum level, disable, change log file destination, etc.)

            foreach (string arg in args)
            {
                switch (arg)
                {
                    // Argument for Spotify, in case Toastify needs to launch it
                    default:
                        spotifyArgs += $"{arg} ";
                        break;
                }
            }
        }

        private static void SetupCultureInfo()
        {
            // Save original cultures
            App.UserCulture = Thread.CurrentThread.CurrentCulture;
            App.UserUICulture = Thread.CurrentThread.CurrentUICulture;

            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }

        private static void SetupLogger()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Toastify.log4net.config"))
            {
                XmlConfigurator.Configure(stream);
                ILoggerRepository loggerRepository = LogManager.GetRepository();

#if DEBUG
                var rootLogger = ((Hierarchy)loggerRepository).Root;
                rootLogger.Level = Level.Debug;
#endif

                // Modify RollingFileAppender's destination
                var rollingFileAppender = (RollingFileAppender)loggerRepository.GetAppenders().FirstOrDefault(appender => appender.Name == "RollingFileAppender");
                if (rollingFileAppender == null)
                    throw new ApplicationStartupException("RollingFileAppender not found", false);
                rollingFileAppender.File = Path.Combine(App.LocalApplicationData, "Toastify.log");
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
            logger.Info($"Architecture: IntPtr = {IntPtr.Size * 8}bit, Is64BitProcess = {Environment.Is64BitProcess}, Is64BitOS = {Environment.Is64BitOperatingSystem}");
            logger.Info($"Toastify version = {App.CurrentVersion}");

            App app = new App(spotifyArgs);
            app.InitializeComponent();

#if DEBUG
            DebugView.Launch();
#endif

            app.Run();
        }

        private static void LoadSettings()
        {
            try
            {
                Settings.Current.Load();
            }
            catch (InvalidOperationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
            catch (FileNotFoundException ex)
            {
                // Check if the old XML settings file is still there.

                const string oldSettingsFileName = "Toastify.xml";

                // ReSharper disable once PossibleNullReferenceException - The parent directory is created by Settings.SettingsFilePath, if needed
                string dir = new FileInfo(Settings.SettingsFilePath).Directory.FullName;
                string filePath = Path.Combine(dir, oldSettingsFileName);
                if (File.Exists(filePath))
                {
                    try
                    {
                        Settings xmlFile;
                        using (StreamReader sr = new StreamReader(filePath))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                            xmlFile = serializer.Deserialize(sr) as Settings;
                        }

                        xmlFile?.SetAsCurrentAndSave();
                        File.Delete(filePath);
                        LoadSettings();
                    }
                    catch (Exception exx)
                    {
                        logger.Error(exx.Message, exx);
                    }
                }
                else
                {
                    logger.Warn("Exception loading settings from file. Using defaults.", ex);

                    string msg = string.Format(Properties.Resources.ERROR_SETTINGS_UNABLE_TO_LOAD, Settings.SettingsFilePath);
                    MessageBox.Show(msg, "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);

                    Settings.Current.LoadSafe();
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Exception loading settings from file. Using defaults.", ex);

                string msg = string.Format(Properties.Resources.ERROR_SETTINGS_UNABLE_TO_LOAD, Settings.SettingsFilePath);
                MessageBox.Show(msg, "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);

                Settings.Current.LoadSafe();
            }
        }

        private static void StartupTask()
        {
            if (!string.IsNullOrWhiteSpace(Settings.Current.PreviousVersion))
            {
                Version previous = new Version(Settings.Current.PreviousVersion);

                if (previous < new Version("1.9.7"))
                {
                    // Re-enable Analytics by default
                    Settings.Current.OptInToAnalytics = true;
                    Settings.Current.Save();
                }
            }
        }
    }

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class App : Application
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(App));

        public static string ApplicationData { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Toastify");

        public static string LocalApplicationData { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Toastify");

        public static CultureInfo UserUICulture { get; set; }

        public static CultureInfo UserCulture { get; set; }

        public static string CurrentVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fileVersionInfo.FileVersion;
            }
        }

        public static string SpotifyParameters { get; private set; }

        public App() : this("")
        {
        }

        public App(string spotifyArgs)
        {
            SpotifyParameters = spotifyArgs.Trim();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Error("Unhandled exception.", e.Exception);
            Analytics.TrackException(e.Exception);
            MessageBox.Show($"Unhandled exception.\n{e.Exception.Message}", "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            Analytics.TrackEvent(Analytics.ToastifyEventCategory.General, Analytics.ToastifyEvent.AppLaunch);
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Analytics.TrackEvent(Analytics.ToastifyEventCategory.General, Analytics.ToastifyEvent.AppTermination);
            Spotify.DisposeInstance();
            VersionChecker.DisposeInstance();
        }
    }
}