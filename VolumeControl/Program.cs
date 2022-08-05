using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using VolumeControl.Core;
using VolumeControl.Log;
using CodingSeb.Localization.Loaders;
using VolumeControl.Log.Properties;
using VolumeControl.Properties;
using System.Resources;
using System.Reflection;
using CodingSeb.Localization;

namespace VolumeControl
{
    internal class LocalizationHelper
    {
        private static LogWriter Log => FLog.Log;
        private static Config Settings => (Config.Default as Config)!;
        private static LocalizationLoader Loader => LocalizationLoader.Instance;
        private static string DefaultPath { get; } = Path.Combine(Helpers.PathFinder.LocalAppData, "Localization");
        private static Loc Loc => Loc.Instance;

        public LocalizationHelper()
        {
            LocalizationLoader.Instance.FileLanguageLoaders.Add(new JsonFileLoader());
            if (Settings.CreateDefaultTranslationFiles)
                CreateDefaultFiles();
            LoadTranslationsFromDirectory(DefaultPath);
            // add custom directories
            foreach (string dir in Settings.CustomLocalizationDirectories)
            {
                LoadTranslationsFromDirectory(dir);
            }
            if (Loc.AvailableLanguages.Contains(Settings.LanguageIdentifier))
            {
                Loc.CurrentLanguage = Settings.LanguageIdentifier;
            }
            else
            {
                Log.Error($"Cannot find translation package for {nameof(Settings.LanguageIdentifier)}: '{Settings.LanguageIdentifier}'");
            }

            Loc.CurrentLanguageChanged += HandleCurrentLanguageChanged;
        }

        private static void HandleCurrentLanguageChanged(object? sender, CurrentLanguageChangedEventArgs e) => Settings.LanguageIdentifier = e.NewLanguageId;

        private static void LoadTranslationsFromDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Loader.AddDirectory(path);
                }
                catch (Exception ex)
                {
                    Log.Error($"{nameof(LoadTranslationsFromDirectory)} failed for directory '{path}' because of an exception!", ex);
                }
            }
            else
            {
                Log.Error($"{nameof(LoadTranslationsFromDirectory)} cannot load directory '{path}' because it doesn't exist!");
            }
        }

        private static void CreateFile(string filepath, byte[] data)
        {
            using var sw = new StreamWriter(File.Open(filepath, FileMode.Create, FileAccess.Write, FileShare.None));
            sw.Write(System.Text.Encoding.UTF8.GetString(data));
            sw.Flush();
            sw.Close();
        }

        private static void CreateDefaultFiles()
        {
            if (!Directory.Exists(DefaultPath))
                Directory.CreateDirectory(DefaultPath);

            var asm = Assembly.GetExecutingAssembly();
            string resourcePath = "VolumeControl.Localization"; //< the directory where the localization resources are stored, relative to the solution dir
            foreach (string embeddedResourceName in asm.GetManifestResourceNames())
            {
                if (embeddedResourceName.StartsWith(resourcePath, StringComparison.Ordinal) && embeddedResourceName.EndsWith(".loc.json", StringComparison.Ordinal))
                {
                    string
                        filename = embeddedResourceName[(resourcePath.Length + 1)..],
                        filepath = Path.Combine(DefaultPath, filename);
#if !DEBUG
                    if (!File.Exists(filepath))
#endif
                    {
                        using (var stream = asm.GetManifestResourceStream(embeddedResourceName))
                        using (var sw = new StreamWriter(File.Open(filepath, FileMode.Create, FileAccess.Write, FileShare.None)))
                        {
                            stream?.CopyTo(sw.BaseStream);
                            sw.Flush();
                            sw.Close();
                        };
                    }
                }
            }
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
            Settings = new();
            Settings.Load();

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
