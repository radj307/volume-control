using CodingSeb.Localization;
using Semver;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using VolumeControl.Core;
using VolumeControl.Core.Input;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

namespace VolumeControl
{
    public class DebugProfiler
    {
        #region Fields
        private readonly Stopwatch _stopwatch = new();
        private readonly Random _random = Random.Shared;
        #endregion Fields

        #region Methods

        #region Profile
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the elapsed time.
        /// </summary>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <returns>A <see cref="TimeSpan"/> containing the elapsed time.</returns>
        public TimeSpan Profile(Action action)
        {
            _stopwatch.Reset();

            _stopwatch.Start();
            action.Invoke();
            _stopwatch.Stop();

            return _stopwatch.Elapsed;
        }
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the average elapsed time.
        /// </summary>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <returns>The average amount of elapsed time, as a <see cref="TimeSpan"/> instance.</returns>
        public TimeSpan Profile(Action action, int count)
        {
            TimeSpan[] t = new TimeSpan[count];

            for (int i = 0; i < count; ++i)
            {
                t[i] = Profile(action);
            }

            return new TimeSpan((long)Math.Round(t.Select(ts => ts.Ticks).Average(), 0));
        }
        #endregion Profile

        #region Next
        /// <summary>
        /// Inclusive version of the <see cref="Random.Next"/> method.
        /// </summary>
        /// <param name="min">The minimum value that can be returned.</param>
        /// <param name="max">The maximum value that can be returned.</param>
        /// <returns>A random value from <paramref name="min"/> up to and including <paramref name="max"/>.</returns>
        public int Next(int min, int max)
            => _random.Next(min, max + 1);
        /// <summary>
        /// Inclusive version of the <see cref="Random.Next"/> method.
        /// </summary>
        /// <param name="max">The maximum value that can be returned.</param>
        /// <returns>A random value from 0 up to and including <paramref name="max"/>.</returns>
        public int Next(int max)
            => _random.Next(max + 1);
        #endregion Next

        #region NextBool
        public bool NextBool()
            => Next(0, 1) == 0;
        #endregion NextBool

        #endregion Methods

        public void TestGeneric<T1, T2>()
        {

        }
    }

    internal static class Program
    {
        #region Statics
        private static LogWriter Log => FLog.Log;
        #endregion Statics

        #region Fields
        private const string appMutexIdentifier = "VolumeControlSingleInstance";
        private static Mutex appMutex = null!;
        private static Config Settings = null!;
        #endregion Fields

        #region Methods
        /// <summary>
        /// Program entry point
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            // make sure the application's working directory isn't System32 (this occurs when run at startup is enabled and the program was started via its registry key)
            bool changedWorkingDirectory = false;
            if (Environment.CurrentDirectory.Equals(Environment.SystemDirectory, StringComparison.OrdinalIgnoreCase))
            { // working directory is System32, change that to the executable directory to prevent problems
                Environment.CurrentDirectory = Path.GetDirectoryName(GetExecutablePath()) ?? AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory; ;
                changedWorkingDirectory = true;
            }

#if RELEASE_FORINSTALLER // use the application's AppData directory for the config file
            Settings = new(Path.Combine(PathFinder.ApplicationAppDataPath, "VolumeControl.json"))
            {
                LogPath = Path.Combine(PathFinder.ApplicationAppDataPath, "VolumeControl.log"),
            };
            Settings.Load();
#else
            Settings = new("VolumeControl.json");
            Settings.Load();
#endif

            // Get program information:
            SemVersion version = GetProgramVersion();
            int versionCompare = version.CompareSortOrderTo(Settings.__VERSION__);
            bool doUpdate = !versionCompare.Equals(0);

            // Check commandline arguments:  
            bool overwriteLanguageConfigs = args.Any(arg => arg.Equals("--overwrite-language-configs", StringComparison.Ordinal));
            bool waitForMutex = args.Any(arg => arg.Equals("--wait-for-mutex", StringComparison.Ordinal));
#if DEBUG // DEBUG
            overwriteLanguageConfigs = true; //< always overwrite language configs in DEBUG configuration
#endif // DEBUG

            LocalizationHelper locale = new(overwriteDefaultLangConfigs: doUpdate || overwriteLanguageConfigs);

            // Multi instance gate
            string mutexId = appMutexIdentifier;

            if (Settings.AllowMultipleDistinctInstances) // append the hash of the config's path to the mutex ID:
                mutexId += ':' + HashFilePath(Settings.Location);

            // Acquire mutex lock
            bool isNewInstance = false;
            appMutex = new Mutex(true, mutexId, out isNewInstance);

            if (!isNewInstance) // Check if we acquired the mutex lock
            {
                if (waitForMutex)
                {
                    _ = appMutex.WaitOne(); //< wait until the mutex is acquired
                }
                else
                {
                    Log.Fatal($"Failed to acquire a mutex lock using identifier '{mutexId}'; ", Settings.AllowMultipleDistinctInstances ? $"another instance of Volume Control is using the config located at '{Settings.Location}'" : "another instance of Volume Control is already running!");
                    MessageBox.Show(Loc.Tr($"VolumeControl.Dialogs.AnotherInstanceIsRunning.{(Settings.AllowMultipleDistinctInstances ? "MultiInstance" : "SingleInstance")}", "Another instance of Volume Control is already running!").Replace("${PATH}", Settings.Location));
                    return;
                }
            }

            if (changedWorkingDirectory)
            { // write a log message about changing the working directory
                if (Settings.RunAtStartup)
                {
                    Log.Info($"Changed application working directory to '{Environment.CurrentDirectory}' (Was '{Environment.SystemDirectory}').");
                }
                else
                { // warn the user
                    Log.Warning($"Changed application working directory to '{Environment.CurrentDirectory}' (Was '{Environment.SystemDirectory}').",
                        "Run at Startup is disabled. The application's working directory should never be set to the system directory under any other circumstances!");
                }
            }

            // Update the Settings' version number
            if (doUpdate)
            {
                Log.Info($"The version number in the settings file was {Settings.__VERSION__}, settings will be {(versionCompare.Equals(1) ? "upgraded" : "downgraded")} to {version}.");
            }
#if DEBUG // In debug configuration, always overwrite previous language files
            doUpdate = true;
#endif
            Settings.__VERSION__ = version;

            // create the application class
            var app = new App();
            try
            {
                int rc = app.Run();
                Log.Info($"App exited with code {rc}");
            }
            catch (Exception ex)
            {
                Log.Fatal("App exited because of an unhandled exception!", ex);
                try
                {
                    // cleanup the notify icon if the program crashes (if it shuts down normally, it is correctly disposed of)
                    app.TrayIcon.Dispose();

                    using TextReader? sr = Log.Endpoint.GetReader();
                    if (sr is not null)
                    {
                        string content = sr.ReadToEnd();
                        sr.Dispose();

                        WriteCrashDump(content);
                    }
                    else
                    {
                        Log.Fatal($"Failed to create crash log dump!");
                    }
                }
                catch (Exception inner_ex)
                {
                    Log.Fatal("Another exception occurred while writing a crash dump!", inner_ex);
                }

#if DEBUG
                Log.Dispose();
                appMutex.ReleaseMutex();
                appMutex.Dispose();
                throw; //< rethrow in debug configuration
#endif
            }

            GC.WaitForPendingFinalizers();

            Log.Dispose();
            appMutex.ReleaseMutex();
            appMutex.Dispose();
        }

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
            string myVersion = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? string.Empty;
            return myVersion.GetSemVer() ?? new(0);
        }
        private static string GetExecutablePath() => System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName is string path
                ? path
                : throw new Exception("Failed to get the location of the current process' executable!");
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

            string headerText = $"/// VOLUME CONTROL CRASH REPORT /// {Timestamp.Now(VolumeControl.Log.Enum.EventType.FATAL)} ///";

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
        #endregion Methods
    }
}
