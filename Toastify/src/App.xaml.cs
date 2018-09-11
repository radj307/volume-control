using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using JetBrains.Annotations;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerArgs;
using SpotifyAPI;
using Toastify.Core;
using Toastify.Core.Broadcaster;
using Toastify.Events;
using Toastify.Model;
using Toastify.Services;
using Toastify.View;
using ToastifyAPI.Core;
using ToastifyAPI.GitHub;
using ToastifyAPI.Interop;
using ToastifyAPI.Interop.Interfaces;
using ToastifyAPI.Logic;
using ToastifyAPI.Logic.Interfaces;
using MouseAction = ToastifyAPI.Core.MouseAction;
using Resources = Toastify.Properties.Resources;

namespace Toastify
{
    //Special entry point to allow for single instance check
    public static class EntryPoint
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(EntryPoint));

        #region Static Fields and Properties

        private static string previousVersion = string.Empty;

        private static MainArgs AppArgs { get; set; }

        #endregion

        #region Static Members

        [STAThread]
        public static void Main(string[] args)
        {
            const string appSpecificGuid = "{B8F3CA50-CE27-4ffa-A812-BBE1435C9485}";
            using (var unused = new Mutex(true, appSpecificGuid, out bool exclusive))
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
                        App.EmergencyLog(e);
                        Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  -  {e}{Environment.NewLine}");
                    }

                    logger.Info($"Architecture: IntPtr = {IntPtr.Size * 8}bit, Is64BitProcess = {Environment.Is64BitProcess}, Is64BitOS = {Environment.Is64BitOperatingSystem}");
                    logger.Info($"Operating System: Version = {ToastifyAPI.Helpers.System.GetOSVersion()}, Friendly Name = \"{ToastifyAPI.Helpers.System.GetFriendlyOSVersion()}\"");
                    logger.Info($"CLR: {Environment.Version}");
                    logger.Info($"Toastify version = {App.CurrentVersion}");

                    try
                    {
                        PrepareToRun();
                    }
                    catch (Exception e)
                    {
                        logger.Error("Unhandled exception while preparing to run.", e);
                        Analytics.TrackException(e);
                        MessageBox.Show($"Unhandled exception while preparing to run.{Environment.NewLine}{e.Message}", "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    RunApp();
                }
                else
                    MessageBox.Show(Resources.INFO_TOASTIFY_ALREADY_RUNNING, "Toastify Already Running", MessageBoxButton.OK, MessageBoxImage.Information);
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
                App.EmergencyLog(e);
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
            // Configure log4net
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Toastify.log4net.config"))
            {
                ILoggerRepository loggerRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                XmlConfigurator.Configure(loggerRepository, stream);

                // Set root logger's log level
                Logger rootLogger = ((Hierarchy)loggerRepository).Root;
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
                IFilter filter = rollingFileAppender.FilterHead;
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

            // > [StartupTaskPerSettings] > Load Settings > [StartupTask]
            // > Initialize Analytics > Update PreviousVersion to this one
            // > Initialize AutoUpdater & VersionChecker
            StartupTaskPreSettings();
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
            var app = new App(AppArgs.SpotifyArgs);
            app.InitializeComponent();

#if DEBUG
            DebugView.Launch();
#endif

            Spotify.Instance.Connected -= Spotify_Connected;
            Spotify.Instance.Connected += Spotify_Connected;

            app.Run();
        }

        private static void StartupTaskPreSettings()
        {
            if (logger.IsDebugEnabled)
                logger.Debug($"[{nameof(StartupTaskPreSettings)}]");

            // Convert the settings file
            if (File.Exists(Settings.SettingsFilePath))
            {
                JObject jSettings;

                using (var sr = new StreamReader(Settings.SettingsFilePath))
                {
                    using (var jsonReader = new JsonTextReader(sr))
                    {
                        jSettings = JObject.Load(jsonReader);
                    }
                }

                JToken jPreviousV = jSettings["PreviousVersion"];
                var previousV = new Version(jPreviousV?.Value<string>() ?? "0");

                // [IDO] Hotkeys
                if (previousV < new Version("1.10.10"))
                {
                    // pre-1.10.10 hotkeys must be converted to the current format
                    logger.Info("Converting old hotkeys to the current format...");

                    JToken jHotkeys = jSettings["HotKeys"];
                    if (jHotkeys?.HasValues == true)
                    {
                        var convertedHotkeys = new JArray();

                        foreach (JToken jHotkey in jHotkeys.Children())
                        {
                            JToken jKeyOrButton = jHotkey["KeyOrButton"];
                            JToken jKey = jKeyOrButton?["Key"];

                            ModifierKeys modifiers = (jHotkey["Alt"]?.Value<bool>() == true ? ModifierKeys.Alt : ModifierKeys.None) |
                                                     (jHotkey["Ctrl"]?.Value<bool>() == true ? ModifierKeys.Control : ModifierKeys.None) |
                                                     (jHotkey["Shift"]?.Value<bool>() == true ? ModifierKeys.Shift : ModifierKeys.None) |
                                                     (jHotkey["WindowsKey"]?.Value<bool>() == true ? ModifierKeys.Windows : ModifierKeys.None);

                            ToastifyActionEnum toastifyActionEnum = Enum.TryParse(jHotkey["Action"]?.Value<string>(), out ToastifyActionEnum tae) ? tae : ToastifyActionEnum.None;
                            ToastifyAction action = App.Container.Resolve<IToastifyActionRegistry>().GetAction(toastifyActionEnum);
                            bool enabled = jHotkey["Enabled"]?.Value<bool>() ?? false;

                            Hotkey h = jKeyOrButton?["IsKey"]?.Value<bool>() ?? jKey != null
                                ? new KeyboardHotkey
                                {
                                    Modifiers = modifiers,
                                    Key = Enum.TryParse(jKey?.Value<string>(), out Key k) ? k : (Key?)null,
                                    Action = action,
                                    Enabled = enabled
                                } as Hotkey
                                : new MouseHookHotkey
                                {
                                    Modifiers = modifiers,
                                    MouseButton = Enum.TryParse(jKeyOrButton?["MouseButton"]?.Value<string>(), out MouseAction ma) ? ma : (MouseAction?)null,
                                    Action = action,
                                    Enabled = enabled
                                };

                            JObject jH = JObject.FromObject(h, Settings.JsonSerializer);
                            convertedHotkeys.Add(jH);
                        }

                        jHotkeys.Replace(convertedHotkeys);
                    }
                }

                // Write the modified JObject back to the settings file
                string json = JsonConvert.SerializeObject(jSettings, Formatting.Indented, Settings.JsonSerializerSettings);
                File.WriteAllText(Settings.SettingsFilePath, json, Encoding.UTF8);
            }
        }

        private static void LoadSettings()
        {
            try
            {
                if (logger.IsDebugEnabled)
                    logger.Debug($"[{nameof(LoadSettings)}]");

                Settings.Current.Load();

                EnsureHotkeysRegisteredCorrectly();

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
                logger.Warn("Config file not found! Toastify will use default values.", ex);
                Settings.Current.LoadSafe();
            }
            catch (Exception ex)
            {
                logger.Warn("Error while loading settings. Toastify will reset to default values.", ex);

                string msg = string.Format(Resources.ERROR_SETTINGS_UNABLE_TO_LOAD, Settings.SettingsFilePath);
                MessageBox.Show(msg, "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);

                File.Copy(Settings.SettingsFilePath, $"{Settings.SettingsFilePath}.corrupted", true);
                Settings.Current.LoadSafe();
            }
        }

        private static void EnsureHotkeysRegisteredCorrectly()
        {
            // If Toastify is launched at system startup, then hotkeys might fail to register correctly

            bool ShouldTryAgain(bool log = true)
            {
                List<Hotkey> enabledHotkeys = Settings.Current.HotKeys.Where(h => h.Enabled).ToList();
                int enabledCount = enabledHotkeys.Count;
                int invalidCount = enabledHotkeys.Count(h => !h.IsValid());

                if (log && logger.IsDebugEnabled)
                    logger.Debug($"Enabled hotkeys: {enabledCount}; Invalid hotkeys: {invalidCount}");

                return invalidCount >= enabledCount / 2;
            }

            if (ShouldTryAgain(false))
            {
                var thread = new Thread(() =>
                {
                    logger.Info("Hotkeys have not been registered correctly. Trying again...");

                    // Let's try again
                    int @try = 0;

                    const int maxAttempts = 5;
                    while (ShouldTryAgain() && @try < maxAttempts)
                    {
                        Thread.Sleep(2000);
                        Settings.Current.ReActivateHotkeys();

                        @try++;
                    }

                    logger.Info(ShouldTryAgain(false) ? "Hotkeys have not been registered correctly" : "Hotkeys have been registered correctly");
                })
                {
                    Name = $"Toastify_{nameof(EnsureHotkeysRegisteredCorrectly)}",
                    IsBackground = true
                };

                thread.Start();
            }
        }

        private static void StartupTask()
        {
            if (logger.IsDebugEnabled)
                logger.Debug($"[{nameof(StartupTask)}]");

            if (!string.IsNullOrWhiteSpace(Settings.Current.PreviousVersion))
            {
                var previous = new Version(Settings.Current.PreviousVersion);

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
            if (logger.IsDebugEnabled)
                logger.Debug($"[{nameof(InitUpdater)}]");

            // Just getting the instances to initialize the singletons
            // ReSharper disable UnusedVariable
            VersionChecker ignore1 = VersionChecker.Instance;
            AutoUpdater ignore2 = AutoUpdater.Instance;
            // ReSharper restore UnusedVariable

            AutoUpdater.Instance.UpdateReady += AutoUpdater_UpdateReady;
        }

        private static void Spotify_Connected(object sender, SpotifyStateEventArgs spotifyStateEventArgs)
        {
            // Show the changelog if necessary
            if (!string.IsNullOrWhiteSpace(previousVersion))
            {
                var previous = new Version(previousVersion);
                if (previous < new Version(App.CurrentVersionNoRevision))
                    ChangelogView.Launch();
            }
        }

        private static void AutoUpdater_UpdateReady(object sender, UpdateReadyEventArgs e)
        {
            logger.Info($"New update is ready to be installed ({e.Version})");

            var choice = MessageBoxResult.Yes;
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
                    var psi = new ProcessStartInfo(e.GitHubReleaseUrl) { UseShellExecute = true };
                    Process.Start(psi);
                }
                else if (choice == MessageBoxResult.No)
                {
                    var fileInfo = new FileInfo(e.InstallerPath);
                    var psi = new ProcessStartInfo(fileInfo.Directory?.FullName)
                    {
                        UseShellExecute = true,
                        Verb = "open"
                    };
                    Process.Start(psi);
                }
                else
                {
                    var psi = new ProcessStartInfo(e.InstallerPath)
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

        #endregion

        [TabCompletion]
        internal class MainArgs
        {
            #region Public Properties

            [ArgShortcut("--debug"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
            [ArgDescription("Enables Debug level logging.")]
            [ArgCantBeCombinedWith("-disable-log")]
            public bool IsDebugLogEnabled { get; set; }

            [ArgShortcut("--disable-log"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
            [ArgDescription("Disables logging entirely.")]
            [ArgCantBeCombinedWith("-debug")]
            public bool IsLogDisabled { get; set; } = false;

            [ArgShortcut("--log-dir"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
            [ArgDescription("Destination directory of log files.")]
            [ArgExistingDirectory]
            public string LogDirectory { get; set; } = App.LocalApplicationData;

            [ArgShortcut("--spotify-args"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
            [ArgDescription("Spotify command-line arguments.")]
            public string SpotifyArgs { get; set; } = string.Empty;

            #endregion
        }
    }

    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class App : Application
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(App));

        #region Static Fields and Properties

        private static readonly ProxyConfig noProxy = new ProxyConfig();
        private static readonly string _applicationData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Toastify");
        private static readonly string _localApplicationData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Toastify");

        private static ProxyConfigAdapter _proxyConfig;

        public static WindsorContainer Container { get; private set; }

        public static string ApplicationData
        {
            get
            {
                if (!Directory.Exists(_applicationData))
                    Directory.CreateDirectory(_applicationData);
                return _applicationData;
            }
        }

        public static string LocalApplicationData
        {
            get
            {
                if (!Directory.Exists(_localApplicationData))
                    Directory.CreateDirectory(_localApplicationData);
                return _localApplicationData;
            }
        }

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
                    var regex = new Regex(@"([0-9]+\.[0-9]+\.[0-9]+)(?:\.[0-9]+)*");
                    Match match = regex.Match(version);
                    if (match.Success)
                        version = match.Groups[1].Value;
                }

                return version;
            }
        }

        public static string SpotifyParameters { get; private set; }

        /// <summary>
        ///     The currently used proxy settings.
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
            set
            {
                if (_proxyConfig == null)
                    _proxyConfig = new ProxyConfigAdapter(noProxy);
                _proxyConfig.Set(value?.ProxyConfig ?? noProxy);
            }
        }

        public static RepoInfo RepoInfo { get; } = new RepoInfo("toastify", "aleab");

        #endregion

        public App() : this("")
        {
        }

        public App(string spotifyArgs)
        {
            SpotifyParameters = spotifyArgs.Trim();
        }

        #region Static Members

        public static void EmergencyLog(string logMessage)
        {
            if (string.IsNullOrWhiteSpace(logMessage))
                return;

            try
            {
                File.AppendAllText(Path.Combine(LocalApplicationData, "log.log"), $@"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  -  {logMessage}{Environment.NewLine}");
            }
            catch
            {
                // ignored
            }
        }

        public static void EmergencyLog(Exception exception)
        {
            if (string.IsNullOrWhiteSpace(exception?.Message))
                return;
            EmergencyLog(exception.ToString());
        }

        public static void ShowConfigProxyDialog()
        {
            CallInSTAThread(() =>
            {
                var proxyDialog = new ConfigProxyDialog();
                proxyDialog.ShowDialog();
            }, true, "Config Proxy");
        }

        public static void Terminate()
        {
            Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() => Current.Shutdown()));
        }

        #endregion

        #region Static setup

        static App()
        {
            SetupContainer();
        }

        private static void SetupContainer()
        {
            Container = new WindsorContainer();

            // ReSharper disable once RedundantExplicitParamsArrayCreation
            Container.Register(new IRegistration[]
            {
                Component.For<IKeyboard>().Instance(InputDevices.PrimaryKeyboard),
                Component.For<IMouse>().Instance(InputDevices.PrimaryMouse),
                Component.For<IInputDevices>().ImplementedBy<InputDevices>(),

                Component.For<IToastifyActionRegistry>().ImplementedBy<ToastifyActionRegistry>(),

                Component.For<IKeyboardHotkeyVisitor>().ImplementedBy<KeyboardHotkeyVisitor>(),
                Component.For<IMouseHookHotkeyVisitor>().ImplementedBy<MouseHookHotkeyVisitor>(),

                Component.For<IToastifyBroadcaster>().ImplementedBy<ToastifyBroadcaster>()
            });
        }

        #endregion Static setup

        #region CallInSTAThread

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

        [CanBeNull]
        public static Thread CallInSTAThreadAsync([CanBeNull] Action action, bool background)
        {
            return CallInSTAThreadAsync(action, background, null);
        }

        [CanBeNull]
        public static Thread CallInSTAThreadAsync([CanBeNull] Action action, bool background, [CanBeNull] string threadName)
        {
            if (action == null)
                return null;

            var t = new Thread(() =>
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

        #endregion CallInSTAThread

        #region Event handlers

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Error("Unhandled exception.", e.Exception);
            Analytics.TrackException(e.Exception);
            MessageBox.Show($"Unhandled exception.{Environment.NewLine}{e.Exception.Message}", "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
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

        #endregion Event handlers
    }
}