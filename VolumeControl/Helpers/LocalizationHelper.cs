using System;
using System.IO;
using VolumeControl.Core;
using VolumeControl.Log;
using CodingSeb.Localization.Loaders;
using System.Reflection;
using CodingSeb.Localization;
using System.Linq;

namespace VolumeControl.Helpers
{
    internal class LocalizationHelper
    {
        #region Constructor
        public LocalizationHelper()
        {
            LocalizationLoader.Instance.FileLanguageLoaders.Add(new JsonFileLoader());
            LocalizationLoader.Instance.FileLanguageLoaders.Add(new YamlFileLoader());

            if (Settings.CreateDefaultTranslationFiles)
                CreateDefaultFiles();

            Reload();

            Loc.CurrentLanguageChanged += HandleCurrentLanguageChanged;
        }
        #endregion Constructor

        #region Fields
        private static readonly char[] _invalidPathChars = Path.GetInvalidPathChars();
        #endregion Fields

        #region Properties
        private static LogWriter Log => FLog.Log;
        private static Config Settings => (Config.Default as Config)!;
        private static LocalizationLoader Loader => LocalizationLoader.Instance;
        private static string DefaultPath { get; } = Path.Combine(Helpers.PathFinder.LocalAppData, "Localization");
        private static Loc Loc => Loc.Instance;
        #endregion Properties

        #region EventHandlers
        private static void HandleCurrentLanguageChanged(object? sender, CurrentLanguageChangedEventArgs e) => Settings.LanguageName = e.NewLanguageId;
        #endregion EventHandlers

        #region Methods
        /// <summary>
        /// Reloads all language config files from the disk, calling <see cref="LocalizationLoader.ClearAllTranslations(bool)"/> to clear the cache, then re-enumerating the <see cref="Config.CustomLocalizationDirectories"/> list and reloading each file.
        /// </summary>
        /// <remarks>Before this method returns, it will attempt to re-select the current language config using the value of the <see cref="Config.LanguageName"/> setting.</remarks>
        public static void Reload()
        {
            // clear all loaded translations, and available languages
            Loader.ClearAllTranslations(true);

            // check default directory
            LoadTranslationsFromDirectory(DefaultPath);

            // check custom directories
            foreach (string dir in Settings.CustomLocalizationDirectories)
            {
                if (dir.Any(c => _invalidPathChars.Contains(c)))
                    Log.Error($"{nameof(Settings.CustomLocalizationDirectories)} specifies directory path with illegal characters: '{dir}'");
                else
                    LoadTranslationsFromDirectory(dir);
            }

            // attempt to re-select the current language
            if (Loc.AvailableLanguages.Contains(Settings.LanguageName))
            {
                Loc.CurrentLanguage = Settings.LanguageName;
            }
            else
            {
                Log.Error($"Cannot find translation package for {nameof(Settings.LanguageName)}: '{Settings.LanguageName}'!");
            }
        }

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

        /// <summary>
        /// To add additional default localizations, put the "*.loc.json" file in VolumeControl/Localization, then mark it as an "Embedded resource":<br/><b>Solution Explorer => Properties => Build Action => Embedded resource</b>
        /// </summary>
        private static void CreateDefaultFiles(bool overwrite = false)
        {
            if (!Directory.Exists(DefaultPath))
                Directory.CreateDirectory(DefaultPath);

            var asm = Assembly.GetExecutingAssembly();
            const string resourcePath = "VolumeControl.Localization"; //< the directory where the localization resources are stored, relative to the solution dir
            foreach (string embeddedResourceName in asm.GetManifestResourceNames())
            {
                if (embeddedResourceName.StartsWith(resourcePath, StringComparison.Ordinal) && embeddedResourceName.EndsWith(".loc.json", StringComparison.Ordinal))
                {
                    string
                        filename = embeddedResourceName[(resourcePath.Length + 1)..],
                        filepath = Path.Combine(DefaultPath, filename);

                    if ((overwrite || !File.Exists(filepath)))
                    {
                        using var stream = asm.GetManifestResourceStream(embeddedResourceName);

                        if (stream is null)
                            continue;

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
        #endregion Methods
    }
}
