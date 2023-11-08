using CodingSeb.Localization;
using Semver;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using VolumeControl.Core;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

namespace VolumeControl
{
    internal static class Program
    {
        #region Fields
        private const string appMutexIdentifier = "VolumeControlSingleInstance"; //< Don't change this without also changing the installer script (vcsetup.iss)
        private static Mutex appMutex = null!;
        private static Config Settings = null!;
        #endregion Fields

        #region Methods

        #region Main
        [STAThread]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int Main(string[] args)
        {
            try
            {
                Console.Out.WriteLine("Application is starting.");
                Console.Out.WriteLine($"Working directory: \"{Environment.CurrentDirectory}\"");

                return Main_Impl(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
#if DEBUG
                throw; //< rethrow in DEBUG configuration
#else
                return 1; //< otherwise return error
#endif
            }
        }
        private static int Main_Impl(string[] args)
        {
            var exeDir = PathFinder.ExecutableDirectory;
            // make sure the application's working directory isn't System32 (this occurs when run at startup is enabled and the program was started via its registry key)
            bool changedWorkingDirectory = false;
            if (Environment.CurrentDirectory.Equals(Environment.SystemDirectory, StringComparison.OrdinalIgnoreCase))
            { // working directory is System32, change that to the executable directory to prevent problems
                Environment.CurrentDirectory = exeDir;
                changedWorkingDirectory = true;
            }

            // Initialize the config
#if RELEASE_FORINSTALLER // use the application's AppData directory for the config file
            Settings = new(Path.Combine(PathFinder.ApplicationAppDataPath, "VolumeControl.json"));

            if (Settings.LogPath == Config.DefaultLogPath && !CanWriteToDirectory(exeDir))
                Settings.LogPath = Path.Combine(PathFinder.ApplicationAppDataPath, "VolumeControl.log");
#else
            Settings = new("VolumeControl.json");
#endif
            Settings.Load();
            Settings.AttachReflectivePropertyChangedHandlers();

            // Multi instance gate
            string appMutexName = appMutexIdentifier;
            if (Settings.AllowMultipleDistinctInstances)
            { // multi-instance mode is enabled; hash the config filepath and append it to the mutex name:
                appMutexName += ':' + HashFilePath(Settings.Location);
            }

            // Create the app mutex
            appMutex = new Mutex(true, appMutexName, out bool isNewInstance);
            if (!isNewInstance)
            { // mutex not acquired, a conflicting instance is running:
                if (args.Any(arg => arg.Equals("--wait-for-mutex", StringComparison.Ordinal)))
                {
                    Console.Out.WriteLine($"Waiting for mutex \"{appMutexName}\" to become available...");
                    _ = appMutex.WaitOne(); //< wait until the mutex is acquired
                    Console.Out.WriteLine($"Acquired mutex \"{appMutexName}\"");
                }
                else
                {
                    Console.Error.WriteLine($"Failed to acquire mutex \"{appMutexName}\" because another instance of Volume Control is using it!");
                    LocalizationHelper.Initialize(false, false, null);
                    MessageBox.Show(Loc.Tr($"VolumeControl.Dialogs.AnotherInstanceIsRunning.{(Settings.AllowMultipleDistinctInstances ? "MultiInstance" : "SingleInstance")}", "Another instance of Volume Control is already running!").Replace("${PATH}", Settings.Location));
                    return 2;
                }
            }
            else Console.Out.WriteLine($"Acquired mutex \"{appMutexName}\".");

            // Initialize the log now that we aren't potentially overwriting another instance's log file
            FLog.Initialize(Settings.LogPath, Settings.EnableLogging, Settings.LogFilter, Settings.LogClearOnInitialize);
#if DEBUG
            FLog.Log.IsAsyncEnabled = false;
            // show all log message types in debug mode
            FLog.Log.EventTypeFilter = EventType.DEBUG | EventType.INFO | EventType.WARN | EventType.ERROR | EventType.FATAL | EventType.TRACE;
            // open the log file in debug mode
            ShellHelper.OpenWithDefault(Settings.LogPath);
#endif

            // write commandline arguments to the log if they were specified
            if (args.Length > 0)
            {
                var msg = new LogMessage(EventType.INFO, $"Commandline arguments were included:");
                for (int i = 0, i_max = args.Length; i < i_max; ++i)
                {
                    msg.Add($" [{i}] \"{args[i]}\"");
                }
                FLog.LogMessage(msg);
            }

            // Get program information:
            SemVersion version = GetProgramVersion();
            int versionCompare = version.CompareSortOrderTo(Settings.__VERSION__);
            bool versionChanged = !versionCompare.Equals(0);

            bool overwriteLanguageConfigs = args.Any(arg => arg.Equals("--overwrite-language-configs", StringComparison.Ordinal));
            bool logMissingTranslations = Settings.LogMissingTranslations;
#if DEBUG
            overwriteLanguageConfigs = true; //< always overwrite language configs in DEBUG configuration
            logMissingTranslations = true; //< always log missing translations in DEBUG configuration
#endif

            // Initialize locale helper
            //  overwrite language configs when the version number changed or when specified on the commandline
            LocalizationHelper.Initialize(versionChanged || overwriteLanguageConfigs, logMissingTranslations, FLog.Log);

            if (changedWorkingDirectory)
            { // write a log message about changing the working directory
                if (Settings.RunAtStartup)
                {
                    FLog.Info($"Changed application working directory to '{Environment.CurrentDirectory}' (Was '{Environment.SystemDirectory}').");
                }
                else
                { // warn the user
                    FLog.Warning($"Changed application working directory to '{Environment.CurrentDirectory}' (Was '{Environment.SystemDirectory}').",
                        "Run at Startup is disabled. The application's working directory should never be set to the system directory under any other circumstances!");
                }
            }

            // Update the version number in the config
            if (versionChanged)
            {
                FLog.Info($"Config version is {Settings.__VERSION__}, settings will be migrated to version {version}.");
            }
#if DEBUG
            versionChanged = true;
#endif
            // update the version number in the config
            Settings.__VERSION__ = version;

            // create the application class
            var app = new App();
            int rc = -1;
            try
            {
                rc = app.Run();
                FLog.Info($"App exited with code {rc}");
            }
            catch (Exception ex)
            {
                // cleanup the notify icon if the program crashes (if it shuts down normally, it is correctly disposed of)
                app.TrayIcon.Dispose(); //< do this before literally ANYTHING else

                FLog.Fatal("App exited because of an unhandled exception:", ex);

                WriteCrashDump(File.ReadAllText(Settings.LogPath));

#if DEBUG
                throw; //< rethrow in debug configuration
#endif
            }

            GC.WaitForPendingFinalizers();

            Settings.Save();
            FLog.Log.Flush(); //< wait for log messages to finish being written
            FLog.Log.Dispose();
            appMutex.ReleaseMutex();
            appMutex.Dispose();

            return rc;
        }
        #endregion Main

        #region (Private)
        private static string HashFilePath(string path)
        {
            var md5 = MD5.Create();

            byte[] pathBytes = Encoding.UTF8.GetBytes(path.ToLower());
            _ = md5.TransformFinalBlock(Encoding.UTF8.GetBytes(path.ToLower()), 0, pathBytes.Length);

            return md5.Hash is not null ? BitConverter.ToString(md5.Hash) : path;
        }
        private static SemVersion GetProgramVersion()
        {
            var asm = Assembly.GetExecutingAssembly();

            var versionAttribute = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            if (versionAttribute == null)
            {
                throw new InvalidOperationException($"Failed to retrieve {nameof(AssemblyInformationalVersionAttribute)} from assembly {asm.FullName}!"); ;
            }

            var versionString = versionAttribute.InformationalVersion;
            var version = versionString.GetSemVer();

            if (version == null)
            {
                throw new InvalidOperationException($"Failed to parse a version number from version string '{versionString}' retrieved from assembly {asm.FullName}!");
            }

            return version;
        }
        /// <summary>
        /// Writes <paramref name="content"/> to an automatically-generated crash dump file.
        /// </summary>
        /// <param name="content">The string to write to the dump file.</param>
        private static void WriteCrashDump(string content)
        {
            int count = 0;
            foreach (string path in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "volumecontrol-crash-*.log"))
            {
                Match? match = Regex.Match(path, "\\d+");
                if (match.Success && int.TryParse(match.Value, out int number) && number > count)
                    count = number;
            }
            using var sw = new StreamWriter($"volumecontrol-crash-{++count}.log");

            string headerText = $"/// VOLUME CONTROL CRASH REPORT /// {DateTime.Now.ToString(AsyncLogWriter.DateTimeFormatString)} [{nameof(EventType.FATAL)}] ///";

            int len = 120;
            if (headerText.Length > 120)
                len = headerText.Length;

            sw.WriteLine(new string('/', len));
            sw.WriteLine();
            int half = (len / 2) - (headerText.Length / 2);
            if (half > 0) sw.Write(new string(' ', half));
            sw.WriteLine(headerText);
            sw.WriteLine();
            sw.WriteLine(new string('/', len));
            sw.WriteLine();

            sw.Write(content);

            sw.Flush();
            sw.Close();

            sw.Dispose();
        }
        private static bool CanWriteToDirectory(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            const FileSystemRights writeDataPermission = FileSystemRights.WriteData;

            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                bool identityHasGroups = identity.Groups != null;

                DirectoryInfo di = new DirectoryInfo(path);
                DirectorySecurity acl = di.GetAccessControl(AccessControlSections.All);
                AuthorizationRuleCollection rules = acl.GetAccessRules(true, true, typeof(NTAccount));

                //Go through the rules returned from the DirectorySecurity
                foreach (AuthorizationRule rule in rules)
                {
                    var ruleIdentityRef = rule.IdentityReference;
                    if (ruleIdentityRef == identity.Owner || (identityHasGroups && identity.Groups!.Contains(ruleIdentityRef)))
                    {
                        var accessRule = (FileSystemAccessRule)rule;

                        if (accessRule.AccessControlType == AccessControlType.Allow
                            && accessRule.FileSystemRights.HasFlag(writeDataPermission))
                        {
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }
        #endregion (Private)

        #endregion Methods
    }
}
