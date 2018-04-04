using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using PowerArgs;
using SpotifyAPI;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;
using Toastify.Core;
using Toastify.Events;
using Toastify.Model;
using Toastify.Services;
using Toastify.View;
using ToastifyAPI.GitHub;

namespace Toastify
{
    //Special entry point to allow for single instance check
    public static class EntryPoint
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(EntryPoint));

        private static string previousVersion = string.Empty;

        private static MainArgs AppArgs { get; set; }

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
                        ProcessCommandLineArguments(args);
                        SetupCultureInfo();
                        SetupLogger();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  -  {e}\n");
                        File.AppendAllText(Path.Combine(App.LocalApplicationData, "log.log"), $@"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  -  {e}\n");
                    }

                    logger.Info($"Architecture: IntPtr = {IntPtr.Size * 8}bit, Is64BitProcess = {Environment.Is64BitProcess}, Is64BitOS = {Environment.Is64BitOperatingSystem}");
                    logger.Info($"Toastify version = {App.CurrentVersion}");

                    try
                    {
                        PrepareToRun();
                    }
                    catch (Exception e)
                    {
                        logger.Error("Unhandled exception while preparing to run.", e);
                        Analytics.TrackException(e);
                        MessageBox.Show($"Unhandled exception while preparing to run.\n{e.Message}", "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    RunApp();
                }
                else
                    MessageBox.Show(Properties.Resources.INFO_TOASTIFY_ALREADY_RUNNING, "Toastify Already Running", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private static void ProcessCommandLineArguments(string[] args)
        {
            try
            {
                AppArgs = args != null && args.Length > 0 ? Args.Parse<MainArgs>(args) : new MainArgs();
            }
            catch (Exception e)
            {
                logger.Warn("Invalid command-line arguments. Toastify will ignore them all.", e);
                MessageBox.Show("Invalid command-line arguments. Toastify will ignore them all.", "Invalid arguments", MessageBoxButton.OK, MessageBoxImage.Warning);
                AppArgs = new MainArgs();
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

                // Set root logger's log level
                var rootLogger = ((Hierarchy)loggerRepository).Root;
#if DEBUG || TEST_RELEASE
                rootLogger.Level = Level.Debug;
#else
                if (AppArgs.IsDebugLogEnabled)
                    rootLogger.Level = Level.Debug;
                else if (AppArgs.IsLogDisabled)
                    rootLogger.Level = Level.Off;
#endif

                // Modify RollingFileAppender's destination
                var rollingFileAppender = (RollingFileAppender)loggerRepository.GetAppenders().FirstOrDefault(appender => appender.Name == "RollingFileAppender");
                if (rollingFileAppender == null)
                    throw new ApplicationStartupException("RollingFileAppender not found");
                rollingFileAppender.File = Path.Combine(AppArgs.LogDirectory, "Toastify.log");

                // Set RollingFileAppender's minimum log level
                var filter = rollingFileAppender.FilterHead;
                while (filter != null)
                {
                    if (filter is LevelRangeFilter lrFilter)
                    {
#if DEBUG || TEST_RELEASE
                        lrFilter.LevelMin = Level.Debug;
#else
                        if (AppArgs.IsDebugLogEnabled)
                            lrFilter.LevelMin = Level.Debug;
#endif
                    }

                    filter = filter.Next;
                }

                rollingFileAppender.ActivateOptions();
            }
        }

        private static void PrepareToRun()
        {
            if (logger.IsDebugEnabled)
                logger.Debug("Preparing to launch Toastify...");

            //   Load Settings > [StartupTask] > Initialize Analytics > Update PreviousVersion to this one
            // > Initialize AutoUpdater & VersionChecker
            LoadSettings();
            StartupTask();
            Analytics.Init();

            previousVersion = Settings.Current.PreviousVersion;
            Settings.Current.PreviousVersion = App.CurrentVersionNoRevision;
            Settings.Current.Save();

            InitUpdater();
        }

        private static void RunApp()
        {
            App app = new App(AppArgs.SpotifyArgs);
            app.InitializeComponent();

#if DEBUG
            DebugView.Launch();
#endif

            Spotify.Instance.Connected -= Spotify_Connected;
            Spotify.Instance.Connected += Spotify_Connected;

            app.Run();
        }

        private static void LoadSettings()
        {
            try
            {
                Settings.Current.Load();

                if (logger.IsDebugEnabled)
                    logger.Debug("Settings loaded!");
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
                    if (logger.IsDebugEnabled)
                        logger.Debug("Migrating from old XML settings to new JSON config file.");

                    try
                    {
                        Settings xmlFile;
                        using (StreamReader sr = new StreamReader(filePath))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                            xmlFile = serializer.Deserialize(sr) as Settings;
                        }

                        xmlFile?.SetAsCurrentAndSave();
                        File.Copy(filePath, $"{filePath}.bak", true);
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
                    logger.Warn("Neither a JSON config file nor an old XML settings file exist! Toastify will use default values.", ex);
                    Settings.Current.LoadSafe();
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Error while loading settings. Toastify will reset to default values.", ex);

                string msg = string.Format(Properties.Resources.ERROR_SETTINGS_UNABLE_TO_LOAD, Settings.SettingsFilePath);
                MessageBox.Show(msg, "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);

                File.Copy(Settings.SettingsFilePath, $"{Settings.SettingsFilePath}.corrupted", true);
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

        private static void InitUpdater()
        {
            // Just getting the instances to initialize the singletons
            // ReSharper disable UnusedVariable
            var ignore1 = VersionChecker.Instance;
            var ignore2 = AutoUpdater.Instance;
            // ReSharper restore UnusedVariable

            AutoUpdater.Instance.UpdateReady += AutoUpdater_UpdateReady;
        }

        private static void Spotify_Connected(object sender, SpotifyStateEventArgs spotifyStateEventArgs)
        {
            // Show the changelog if necessary
            if (!string.IsNullOrWhiteSpace(previousVersion))
            {
                Version previous = new Version(previousVersion);
                if (previous < new Version(App.CurrentVersionNoRevision))
                    ChangelogView.Launch();
            }
        }

        private static void AutoUpdater_UpdateReady(object sender, UpdateReadyEventArgs e)
        {
            logger.Info($"New update is ready to be installed ({e.Version})");

            MessageBoxResult choice = MessageBoxResult.Yes;
            App.CallInSTAThread(() =>
            {
                choice = CustomMessageBox.ShowYesNoCancel(
                    "A new update is ready to be installed!",
                   $"Toastify {e.Version}",
                    "Install",      // Yes
                    "Open folder",  // No
                    "Go to GitHub", // Cancel
                    MessageBoxImage.Information);
            }, true);

            logger.Info($"Use choice = {(choice == MessageBoxResult.Cancel ? "Go to GitHub" : choice == MessageBoxResult.No ? "Open folder" : "Install")}");

            try
            {
                if (choice == MessageBoxResult.Cancel)
                {
                    ProcessStartInfo psi = new ProcessStartInfo(e.GitHubReleaseUrl) { UseShellExecute = true };
                    Process.Start(psi);
                }
                else if (choice == MessageBoxResult.No)
                {
                    FileInfo fileInfo = new FileInfo(e.InstallerPath);
                    ProcessStartInfo psi = new ProcessStartInfo(fileInfo.Directory?.FullName)
                    {
                        UseShellExecute = true,
                        Verb = "open"
                    };
                    Process.Start(psi);
                }
                else
                {
                    ProcessStartInfo psi = new ProcessStartInfo(e.InstallerPath)
                    {
                        UseShellExecute = true,
                        Verb = "runas"
                    };
                    Process.Start(psi);
                    App.Terminate();
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Unknown error in {nameof(AutoUpdater_UpdateReady)}", ex);
            }
        }

        [TabCompletion]
        internal class MainArgs
        {
            // [ArgShortcut("--debug"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
            [ArgShortcut("--debug")]
            [ArgDescription("Enables Debug level logging.")]
            [ArgCantBeCombinedWith("IsLogDisabled")]
            public bool IsDebugLogEnabled { get; set; }

            // [ArgShortcut("--disable-log"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
            [ArgShortcut("--disable-log")]
            [ArgDescription("Disables logging entirely.")]
            [ArgCantBeCombinedWith("IsDebugLogEnabled")]
            public bool IsLogDisabled { get; set; } = false;

            [ArgShortcut("--log-dir"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
            [ArgDescription("Destination directory of log files.")]
            [ArgExistingDirectory]
            public string LogDirectory { get; set; } = App.LocalApplicationData;

            [ArgShortcut("--spotify-args"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
            [ArgDescription("Spotify command-line arguments.")]
            public string SpotifyArgs { get; set; } = string.Empty;
        }
    }

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class App : Application
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(App));

        private static readonly ProxyConfig noProxy = new ProxyConfig();

        // ReSharper disable once InconsistentNaming
        private static ProxyConfigAdapter _proxyConfig;

        public static string ApplicationData { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Toastify");

        public static string LocalApplicationData { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Toastify");

        public static CultureInfo UserUICulture { get; set; }

        public static CultureInfo UserCulture { get; set; }

        public static string CurrentVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return assembly.GetName().Version.ToString();
            }
        }

        public static string CurrentVersionNoRevision
        {
            get
            {
                string version = CurrentVersion;
                if (version != null)
                {
                    Regex regex = new Regex(@"([0-9]+\.[0-9]+\.[0-9]+)(?:\.[0-9]+)*");
                    Match match = regex.Match(version);
                    if (match.Success)
                        version = match.Groups[1].Value;
                }

                return version;
            }
        }

        public static string SpotifyParameters { get; private set; }

        /// <summary>
        /// The currently used proxy settings.
        /// </summary>
        public static ProxyConfigAdapter ProxyConfig
        {
            get
            {
                if (_proxyConfig == null)
                    _proxyConfig = new ProxyConfigAdapter(Settings.Current.ProxyConfig.ProxyConfig);

                if (!Settings.Current.UseProxy)
                    _proxyConfig.Set(noProxy);
                return _proxyConfig;
            }
            set { _proxyConfig.Set(value.ProxyConfig ?? noProxy); }
        }

        public static RepoInfo RepoInfo { get; } = new RepoInfo("toastify", "aleab");

        public App() : this("")
        {
        }

        public App(string spotifyArgs)
        {
            SpotifyParameters = spotifyArgs.Trim();
        }

        public static void ShowConfigProxyDialog()
        {
            CallInSTAThread(() =>
            {
                ConfigProxyDialog proxyDialog = new ConfigProxyDialog();
                proxyDialog.ShowDialog();
            }, true, "Config Proxy");
        }

        public static void CallInSTAThread(Action action, bool background)
        {
            Thread t = CallInSTAThreadAsync(action, background);
            t?.Join();
        }

        public static void CallInSTAThread(Action action, bool background, string threadName)
        {
            Thread t = CallInSTAThreadAsync(action, background, threadName);
            t?.Join();
        }

        public static Thread CallInSTAThreadAsync(Action action, bool background)
        {
            return CallInSTAThreadAsync(action, background, null);
        }

        public static Thread CallInSTAThreadAsync(Action action, bool background, string threadName)
        {
            if (action == null)
                return null;

            Thread t = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    logger.Error($"Unknown error in {nameof(CallInSTAThreadAsync)}", ex);
                }
            })
            {
                IsBackground = background
            };
            if (!string.IsNullOrWhiteSpace(threadName))
                t.Name = threadName;
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            return t;
        }

        public static void Terminate()
        {
            Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() => Current.Shutdown()));
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
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

            logger.Info($"Toastify terminated with exit code {e.ApplicationExitCode}.");
        }
    }
}