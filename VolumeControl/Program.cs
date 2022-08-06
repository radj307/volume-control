using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using VolumeControl.Core;
using VolumeControl.Helpers;
using VolumeControl.Log;

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
            Settings = new();
            Settings.Load();

            // Object that manages localization:
            LocalizationHelper locale = new();

            bool waitForMutex = args.Any(arg => arg.Equals("--wait-for-mutex", StringComparison.Ordinal));

            AppDomain? appDomain = AppDomain.CurrentDomain;
            string path = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;

            // Multi instance gate
            bool isNewInstance = false;
            appMutex = new(true, appMutexIdentifier, out isNewInstance);

            if (!isNewInstance)
            {
                if (waitForMutex)
                {
                    _ = appMutex.WaitOne(); //< wait until the mutex is acquired
                }
                else
                {
                    Log.Fatal($"Failed to acquire mutex '{appMutexIdentifier}'; another instance of Volume Control (or the update utility) is currently running!");
                    MessageBox.Show("Another instance of Volume Control is already running!");
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

#if DEBUG
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

            sw.Write(content);
            sw.Flush();
            sw.Close();

            sw.Dispose();
        }
        #endregion Methods
    }
}
