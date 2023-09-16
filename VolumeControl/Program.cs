using CodingSeb.Localization;
using Semver;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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
            // Load VolumeControl.json:
            Settings = new();
            Settings.Load();

            // Get program information:
            string path = GetProgramLocation();
            SemVersion version = GetProgramVersion();
            int versionCompare = version.CompareSortOrderTo(Settings.__VERSION__);
            bool doUpdate = !versionCompare.Equals(0);

            if (doUpdate)
            {
                Log.Info($"The version number in the settings file was {Settings.__VERSION__}, settings will be {(versionCompare.Equals(1) ? "upgraded" : "downgraded")} to {version}.");
            }

#if DEBUG // In debug configuration, always overwrite previous language files
            doUpdate = true;
#endif

            // Update the Settings' version number
            Settings.__VERSION__ = version;

            // Check commandline arguments:  
            bool overwriteLanguageConfigs = args.Any(arg => arg.Equals("--overwrite-language-configs", StringComparison.Ordinal));
            bool waitForMutex = args.Any(arg => arg.Equals("--wait-for-mutex", StringComparison.Ordinal));

            LocalizationHelper locale = new(doUpdate || overwriteLanguageConfigs);

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
        private static string GetProgramLocation()
        {
            // Get the app domain & program location:  
            AppDomain? appDomain = AppDomain.CurrentDomain;
            return appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
        }
        private static SemVersion GetProgramVersion()
        {
            var asm = Assembly.GetExecutingAssembly();
            string myVersion = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? string.Empty;
            return myVersion.GetSemVer() ?? new(0);
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
