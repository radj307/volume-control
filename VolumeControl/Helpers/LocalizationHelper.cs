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
using VolumeControl.Log.Interfaces;
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
        private static Config Settings => Config.Default;
        private static LocalizationLoader Loader => LocalizationLoader.Instance;
        private static string DefaultPath { get; } = Path.Combine(PathFinder.ApplicationAppDataPath, "Localization");
        private static Loc Loc => Loc.Instance;
        private static readonly Regex LanguageConfigNameRegex = new(@"([a-z]{2}\.loc\.(?:json|yaml|yml))$", RegexOptions.Compiled);
        #endregion Properties

        #region Methods
        /// <summary>
        /// Reloads all language config files from the disk, calling <see cref="LocalizationLoader.ClearAllTranslations(bool)"/> to clear the cache, then re-enumerating the <see cref="Config.CustomLocalizationDirectories"/> list and reloading each file.
        /// </summary>
        /// <remarks>Before this method returns, it will attempt to re-select the current language config using the value of the <see cref="Config.LanguageName"/> setting.</remarks>
        public static void ReloadLanguageConfigs(ILogWriter? log)
        {
            // clear all loaded translations, and available languages
            Loader.ClearAllTranslations(true);

            // check default directory
            LoadTranslationsFromDirectory(DefaultPath, log);

            // check custom directories
            foreach (string dirPath in Settings.CustomLocalizationDirectories)
            {
                if (dirPath.Any(c => _invalidPathChars.Contains(c)))
                    log?.Error($"{nameof(Settings.CustomLocalizationDirectories)} specifies directory path with illegal characters: '{dirPath}'");
                else
                    LoadTranslationsFromDirectory(dirPath, log);
            }
        }

        private static void LoadTranslationsFromDirectory(string path, ILogWriter? log)
        {
            bool showTraceLogMessages = log?.FilterEventType(EventType.TRACE) ?? false;
            if (Directory.Exists(path))
            {
                if (showTraceLogMessages)
                    log?.Trace($"[{nameof(LocalizationHelper)}] Searching for language config files in \"{path}\"");
                foreach (string filepath in Directory.EnumerateFiles(path))
                {
                    string filename = Path.GetFileName(filepath);
                    if (!LanguageConfigNameRegex.IsMatch(filename))
                    {
                        if (showTraceLogMessages)
                            log?.Trace($"[{nameof(LocalizationHelper)}] Skipped loading \"{filepath}\" because its name is invalid!");
                        continue;
                    }

                    try
                    {
                        Loader.AddFile(filepath);

                        if (showTraceLogMessages)
                            log?.Trace($"[{nameof(LocalizationHelper)}] Successfully loaded language config \"{filepath}\"");
                    }
                    catch (Exception ex)
                    {
                        log?.Error($"[{nameof(LocalizationHelper)}] Failed to load language config \"{filepath}\" due to an exception:", ex);
                    }
                }
            }
            else
            {
                log?.Error($"[{nameof(LocalizationHelper)}] Directory \"{path}\" doesn't exist!");
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

                    var fileExists = File.Exists(filePath);
                    if (overwrite || !fileExists)
                    {
                        using var resourceStream = asm.GetManifestResourceStream(embeddedResourceName);

                        if (resourceStream == null || resourceStream.Length.Equals(0))
                            continue;

                        using var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

                        resourceStream.CopyTo(fileStream);
                        fileStream.Flush();

                        FLog.Info($"[LocalizationHelper] {(fileExists ? "Overwrote" : "Created")} translation config at \"{filePath}\" with data from manifest resource \"{embeddedResourceName}\".");
                    }
                }
                else FLog.Error($"[LocalizationHelper] Manifest resource \"{embeddedResourceName}\" does not have a valid name for a translation config.");
            }
        }
        #endregion Methods
    }
}
