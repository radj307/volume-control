using CodingSeb.Localization;
using Semver;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using VolumeControl.Core;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.Log.Enum;
using VolumeControl.TypeExtensions;

namespace VolumeControl
{
#if DEBUG
    /// <summary>
    /// Helper class for comparing the approximate speeds of code snippets.
    /// </summary>
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
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <returns>A <see cref="TimeSpan"/> containing the elapsed time.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public TimeSpan Profile(Action action, Action? preAction = null, Action? postAction = null)
        {
            _stopwatch.Reset();

            preAction?.Invoke();

            _stopwatch.Start();
            action.Invoke();
            _stopwatch.Stop();

            postAction?.Invoke();

            return _stopwatch.Elapsed;
        }
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the average elapsed time.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <returns>The average amount of elapsed time, as a <see cref="TimeSpan"/> instance.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public TimeSpan Profile(int count, Action action, Action? preAction = null, Action? postAction = null)
        {
            return new TimeSpan((long)Math.Round(ProfileAll(count, action, preAction, postAction).Select(ts => ts.Ticks).Average(), 0));
        }
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the average elapsed time.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <returns>The average amount of elapsed time, as a <see cref="TimeSpan"/> instance.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public TimeSpan Profile(int count, Action action, Action<int>? preAction, Action<int>? postAction)
        {
            return new TimeSpan((long)Math.Round(ProfileAll(count, action, preAction, postAction).Select(ts => ts.Ticks).Average(), 0));
        }
        #endregion Profile

        #region ProfileAll
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the elapsed times.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <returns>An array of <see cref="TimeSpan"/> instances containing the elapsed time of each run.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public TimeSpan[] ProfileAll(int count, Action action, Action? preAction = null, Action? postAction = null)
        {
            TimeSpan[] t = new TimeSpan[count];

            for (int i = 0; i < count; ++i)
            {
                t[i] = Profile(action, preAction, postAction);
            }

            return t;
        }
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the elapsed times.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <returns>An array of <see cref="TimeSpan"/> instances containing the elapsed time of each run.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public TimeSpan[] ProfileAll(int count, Action action, Action<int>? preAction, Action<int>? postAction)
        {
            TimeSpan[] t = new TimeSpan[count];

            for (int i = 0; i < count; ++i)
            {
                preAction?.Invoke(i);

                t[i] = Profile(action);

                postAction?.Invoke(i);
            }

            return t;
        }
        #endregion ProfileAll

        #region ProfileTicks
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the average elapsed time in ticks.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <returns>The average elapsed time, measured in ticks.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public double ProfileTicks(int count, Action action, Action? preAction = null, Action? postAction = null)
        {
            return ProfileAll(count, action, preAction, postAction).Select(ts => ts.Ticks).Average();
        }
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the average elapsed time in ticks.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <returns>The average elapsed time, measured in ticks.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public double ProfileTicks(int count, Action action, Action<int>? preAction, Action<int>? postAction)
        {
            return ProfileAll(count, action, preAction, postAction).Select(ts => ts.Ticks).Average();
        }
        #endregion ProfileTicks

        #region ProfileMicroseconds
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the average elapsed time in microseconds.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <returns>The average elapsed time, measured in microseconds.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public double ProfileMicroseconds(int count, Action action, Action? preAction = null, Action? postAction = null)
        {
            double avgTicks = ProfileTicks(count, action, preAction, postAction);
            return (avgTicks / Stopwatch.Frequency) * 1000000;
        }
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the average elapsed time in microseconds.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <returns>The average elapsed time, measured in microseconds.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public double ProfileMicroseconds(int count, Action action, Action<int>? preAction, Action<int>? postAction)
        {
            double avgTicks = ProfileTicks(count, action, preAction, postAction);
            return (avgTicks / Stopwatch.Frequency) * 1000000;
        }
        #endregion ProfileMicroseconds

        #region IndexOfFastestValue
        /// <summary>
        /// Finds the smallest value &amp; its index in the specified <paramref name="values"/> array.
        /// </summary>
        /// <typeparam name="T">The comparable type of the specified <paramref name="values"/>.</typeparam>
        /// <param name="values">Any number of values.</param>
        /// <returns>The fastest value and its index in <paramref name="values"/>.</returns>
        public static (T Value, int Index) Smallest<T>(params T[] values) where T : IComparable<T>
        {
            T smallestValue = default!;
            int smallestIndex = -1;

            for (int i = 0, i_max = values.Length; i < i_max; ++i)
            {
                T value = values[i];
                if (smallestIndex == -1 || value.CompareTo(smallestValue) < 0)
                {
                    smallestValue = value;
                    smallestIndex = i;
                }
            }

            return (smallestValue, smallestIndex);
        }
        /// <summary>
        /// Gets the index of the smallest value in the specified <paramref name="values"/> array.
        /// </summary>
        /// <typeparam name="T">The comparable type of the specified <paramref name="values"/>.</typeparam>
        /// <param name="values">Any number of values.</param>
        /// <returns>The index of the fastest value in <paramref name="values"/>.</returns>
        public static int IndexOfSmallest<T>(params T[] values) where T : IComparable<T>
            => Smallest(values).Index;
        /// <inheritdoc cref="Smallest{T}(T[])"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public (T Value, int Index) Fastest<T>(params T[] values) where T : IComparable<T> => Smallest(values);
        /// <inheritdoc cref="IndexOfSmallest{T}(T[])"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public int IndexOfFastest<T>(params T[] values) where T : IComparable<T> => IndexOfSmallest(values);
        #endregion IndexOfFastestValue

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
    }
#endif // DEBUG

    internal static class Program
    {
        #region Fields
        private const string appMutexIdentifier = "VolumeControlSingleInstance";
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
                return Main_Impl(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }
        /// <summary>
        /// Program entry point
        /// </summary>
        public static int Main_Impl(string[] args)
        {
            // make sure the application's working directory isn't System32 (this occurs when run at startup is enabled and the program was started via its registry key)
            bool changedWorkingDirectory = false;
            if (Environment.CurrentDirectory.Equals(Environment.SystemDirectory, StringComparison.OrdinalIgnoreCase))
            { // working directory is System32, change that to the executable directory to prevent problems
                var proc = Process.GetCurrentProcess();
                var exeDir = Path.GetDirectoryName(proc.MainModule?.FileName) ?? AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
                proc.Dispose();
                Environment.CurrentDirectory = exeDir;
                changedWorkingDirectory = true;
            }

            // Initialize the config
#if RELEASE_FORINSTALLER // use the application's AppData directory for the config file
            Settings = new(Path.Combine(PathFinder.ApplicationAppDataPath, "VolumeControl.json"))
            {
                LogPath = Path.Combine(PathFinder.ApplicationAppDataPath, "VolumeControl.log"),
            };
#else
            Settings = new("VolumeControl.json");
#endif
            Settings.Load();

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
                    _ = appMutex.WaitOne(); //< wait until the mutex is acquired
                }
                else
                {
                    LocalizationHelper localeHelper = new(false); //< initialize without logging
                    MessageBox.Show(Loc.Tr($"VolumeControl.Dialogs.AnotherInstanceIsRunning.{(Settings.AllowMultipleDistinctInstances ? "MultiInstance" : "SingleInstance")}", "Another instance of Volume Control is already running!").Replace("${PATH}", Settings.Location));
                    return 2;
                }
            }

            // Initialize the log now that we aren't potentially overwriting another instance's log file
            FLog.Initialize();
#if DEBUG
            // show all log message types in debug mode
            FLog.Log.EventTypeFilter = EventType.DEBUG | EventType.INFO | EventType.WARN | EventType.ERROR | EventType.FATAL | EventType.TRACE;
#endif

            // Get program information:
            SemVersion version = GetProgramVersion();
            int versionCompare = version.CompareSortOrderTo(Settings.__VERSION__);
            bool versionChanged = !versionCompare.Equals(0);

            bool overwriteLanguageConfigs = args.Any(arg => arg.Equals("--overwrite-language-configs", StringComparison.Ordinal));
#if DEBUG
            overwriteLanguageConfigs = true; //< always overwrite language configs in DEBUG configuration
#endif

            // Initialize locale helper
            //  overwrite language configs when the version number changed or when specified on the commandline
            LocalizationHelper locale = new(overwriteExistingConfigs: versionChanged || overwriteLanguageConfigs, FLog.Log);

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

        #region Private
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
        #endregion Private

        #endregion Methods
    }
}
