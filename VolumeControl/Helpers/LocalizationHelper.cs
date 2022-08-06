using CodingSeb.Localization;
using CodingSeb.Localization.Loaders;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using VolumeControl.Core;
using VolumeControl.Log;

namespace VolumeControl.Helpers
{
    internal class LocalizationHelper
    {
        #region Constructor
        public LocalizationHelper(bool overwriteDefaultLangConfigs = false)
        {
            LocalizationLoader.Instance.FileLanguageLoaders.Add(new JsonFileLoader());
            LocalizationLoader.Instance.FileLanguageLoaders.Add(new YamlFileLoader());

            if (Settings.CreateDefaultTranslationFiles)
                CreateDefaultFiles(overwriteDefaultLangConfigs);

            ReloadLanguageConfigs();

            Loc.CurrentLanguage = Settings.LanguageName;

            Loc.CurrentLanguageChanged += (s, e) =>
            {
                if (e.OldLanguageId != e.NewLanguageId)
                    Settings.LanguageName = e.NewLanguageId;
            };
        }
        #endregion Constructor

        #region Fields
        private static readonly char[] _invalidPathChars = Path.GetInvalidPathChars();
        #endregion Fields

        #region Properties
        private static LogWriter Log => FLog.Log;
        private static Config Settings => (Config.Default as Config)!;
        private static LocalizationLoader Loader => LocalizationLoader.Instance;
        private static string DefaultPath { get; } = Path.Combine(PathFinder.LocalAppData, "Localization");
        private static Loc Loc => Loc.Instance;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Reloads all language config files from the disk, calling <see cref="LocalizationLoader.ClearAllTranslations(bool)"/> to clear the cache, then re-enumerating the <see cref="Config.CustomLocalizationDirectories"/> list and reloading each file.
        /// </summary>
        /// <remarks>Before this method returns, it will attempt to re-select the current language config using the value of the <see cref="Config.LanguageName"/> setting.</remarks>
        public static void ReloadLanguageConfigs()
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
            const string resourcePath = "VolumeControl.Localization"; //< specifies the directory/namespace where localization files are located (relative to the solution dir)

            foreach (string embeddedResourceName in asm.GetManifestResourceNames())
            {
                if (embeddedResourceName.StartsWith(resourcePath, StringComparison.Ordinal) && (embeddedResourceName.EndsWith(".loc.json", StringComparison.Ordinal) || embeddedResourceName.EndsWith(".loc.yaml", StringComparison.Ordinal)))
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
        }
        #endregion Methods
    }
}
