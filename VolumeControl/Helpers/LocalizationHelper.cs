using System;
using System.IO;
using VolumeControl.Core;
using VolumeControl.Log;
using CodingSeb.Localization.Loaders;
using System.Reflection;
using CodingSeb.Localization;

namespace VolumeControl.Helpers
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

            Reload();

            Loc.CurrentLanguageChanged += HandleCurrentLanguageChanged;
        }

        private static void Reload()
        {
            Loader.ClearAllTranslations(false);
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

        /// <summary>
        /// To add additional default localizations, put the "*.loc.json" file in VolumeControl/Localization, then mark it as an "Embedded resource":<br/><b>Solution Explorer => Properties => Build Action => Embedded resource</b>
        /// </summary>
        public static void CreateDefaultFiles(bool overwrite = false)
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
                    if ((overwrite || !File.Exists(filepath)) && !filename.StartsWith("TestLang"))
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
            if (overwrite)
                Reload();
        }
    }
}
