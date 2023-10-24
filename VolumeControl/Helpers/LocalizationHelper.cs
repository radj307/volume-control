using CodingSeb.Localization;
using CodingSeb.Localization.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using VolumeControl.Core;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Helpers
{
    internal class LocalizationHelper
    {
        #region Constructor
        public LocalizationHelper(bool overwriteExistingConfigs, ILogWriter? log = null)
        {
            if (_initialized)
                return;

            _initialized = true;

            _ = FileLoaders.AddIfUnique(new JsonFileLoader());
            _ = FileLoaders.AddIfUnique(new YamlFileLoader());

            if (Settings.CreateDefaultTranslationFiles) //< never create default files when this (and this alone) is false!
                CreateDefaultFiles(overwriteExistingConfigs);

            ReloadLanguageConfigs(log);

            Loc.CurrentLanguage = Settings.LanguageName;

            Loc.CurrentLanguageChanged += (s, e) =>
            {
                if (!e.NewLanguageId.Equals(Settings.LanguageName, StringComparison.Ordinal))
                {
                    Settings.LanguageName = e.NewLanguageId;
                    log?.Info($"{nameof(Settings.LanguageName)} was changed to '{Settings.LanguageName}' (was '{e.OldLanguageId}')");
                }
            };
        }
        #endregion Constructor

        #region Fields
        private static readonly char[] _invalidPathChars = Path.GetInvalidPathChars();
        private static bool _initialized = false;
        #endregion Fields

        #region Properties
        private static Config Settings => (AppConfig.Configuration.Default as Config)!;
        private static LocalizationLoader Loader => LocalizationLoader.Instance;
        private static List<ILocalizationFileLoader> FileLoaders => Loader.FileLanguageLoaders;
        private static string DefaultPath { get; } = Path.Combine(PathFinder.ApplicationAppDataPath, "Localization");
        private static Loc Loc => Loc.Instance;
        private static readonly Regex LanguageConfigNameRegex = new(@"([a-z]{2}\.loc\.(?:json|yaml|yml))", RegexOptions.Compiled);
        #endregion Properties

        #region Methods
        /// <summary>
        /// Reloads all language config files from the disk, calling <see cref="LocalizationInitializer.ClearAllTranslations(bool)"/> to clear the cache, then re-enumerating the <see cref="Config.CustomLocalizationDirectories"/> list and reloading each file.
        /// </summary>
        /// <remarks>Before this method returns, it will attempt to re-select the current language config using the value of the <see cref="Config.LanguageName"/> setting.</remarks>
        public static void ReloadLanguageConfigs(ILogWriter? log)
        {
            // clear all loaded translations, and available languages
            Loader.ClearAllTranslations(true);

            // check default directory
            LoadTranslationsFromDirectory(DefaultPath, log);

            // check custom directories
            foreach (string dir in Settings.CustomLocalizationDirectories)
            {
                if (dir.Any(c => _invalidPathChars.Contains(c)))
                    log?.Error($"{nameof(Settings.CustomLocalizationDirectories)} specifies directory path with illegal characters: '{dir}'");
                else
                    LoadTranslationsFromDirectory(dir, log);
            }
        }

        private static void LoadTranslationsFromDirectory(string path, ILogWriter? log)
        {
            if (Directory.Exists(path))
            {
                foreach (string filepath in Directory.EnumerateFiles(path))
                {
                    string filename = Path.GetFileName(filepath);
                    if (!LanguageConfigNameRegex.IsMatch(filename))
                        continue;

                    try
                    {
                        Loader.AddFile(filepath);
                        log?.Debug($"Successfully loaded language config '{filepath}'");
                    }
                    catch (Exception ex)
                    {
                        log?.Error($"Failed to load language config '{filepath}' because an exception was thrown!", ex);
                    }
                }
            }
            else
            {
                log?.Error($"{nameof(LoadTranslationsFromDirectory)} cannot load directory '{path}' because it doesn't exist!");
            }
        }

        /// <summary>
        /// To add additional default localizations, put the "*.loc.json" file in VolumeControl/Localization, then mark it as an "Embedded resource":<br/><b>Solution Explorer => Properties => Build Action => Embedded resource</b>
        /// </summary>
        private static void CreateDefaultFiles(bool overwrite = false)
        {
            if (!Directory.Exists(DefaultPath))
                _ = Directory.CreateDirectory(DefaultPath);

            var asm = Assembly.GetExecutingAssembly();
            const string baseResourcePath = "VolumeControl.Localization";

            foreach (string embeddedResourceName in asm.GetManifestResourceNames())
            {
                if (!embeddedResourceName.StartsWith(baseResourcePath, StringComparison.Ordinal))
                    continue;

                if (LanguageConfigNameRegex.IsMatch(embeddedResourceName))
                {
                    string
                        embeddedResourceFilename = embeddedResourceName[(baseResourcePath.Length + 1)..],
                        filePath = Path.Combine(DefaultPath, embeddedResourceFilename);

                    if (overwrite || !File.Exists(filePath))
                    {
                        using var resourceStream = asm.GetManifestResourceStream(embeddedResourceName);

                        if (resourceStream == null || resourceStream.Length.Equals(0))
                            continue;

                        using var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

                        resourceStream.CopyTo(fileStream);
                        fileStream.Flush();
                    }
                }
            }
        }
        #endregion Methods
    }
}
