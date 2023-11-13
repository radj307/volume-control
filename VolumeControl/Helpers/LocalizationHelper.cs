using Localization;
using Localization.Events;
using Localization.Json;
using Localization.Yaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VolumeControl.Core;
using VolumeControl.Log;
using VolumeControl.Log.Interfaces;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Helpers
{
    public static class LocalizationHelper
    {
        #region Fields
        private const string _baseResourcePath = $"{nameof(VolumeControl)}.Localization.";
        private static string _defaultTranslationConfigDirectory = Path.Combine(PathFinder.ApplicationAppDataPath, "Localization");
        private static readonly Regex LanguageConfigNameRegex = new(@"([a-z]{2}\.loc\.(?:json|yaml|yml))$", RegexOptions.Compiled);
        #endregion Fields

        #region Properties
        private static Config Settings => Config.Default;
        private static ILogWriter? LogWriter { get; set; }
        private static JsonTranslationLoader JsonLoader { get; set; } = null!;
        private static YamlTranslationLoader YamlLoader { get; set; } = null!;
        private static List<(Assembly Assembly, string ManifestResourceName)> AddonManifestResourceConfigs { get; } = new();
        public static bool IsInitialized { get; private set; }
        #endregion Properties

        #region Methods

        #region Initialize
        public static void Initialize(bool overwriteExistingTranslationConfigs, bool logMissingTranslations, ILogWriter? log)
        {
            if (IsInitialized)
                throw new InvalidOperationException($"[{nameof(LocalizationHelper)}] has already been initialized!");
            IsInitialized = true;

            LogWriter = log;

            // create file loaders
            if (Loc.Instance.TranslationLoaders.Count == 0)
            {
                JsonLoader = Loc.Instance.AddTranslationLoader<JsonTranslationLoader>();
                YamlLoader = Loc.Instance.AddTranslationLoader<YamlTranslationLoader>();
            }

            // set the default translation config directory
            if (Settings.CreateTranslationConfigsInLocalDirectory)
                _defaultTranslationConfigDirectory = Path.Combine(PathFinder.ExecutableDirectory, "Localization");

            // create the default translation configs
            if (Settings.CreateTranslationConfigs)
            {
                WriteEmbeddedTranslationConfigs(overwriteExistingTranslationConfigs);
            }

            // use blank string instead of key when no default text was provided
            Loc.Instance.UseKeyAsFallback = false;

#if !DEBUG
            if (Settings.LogMissingTranslations)
#endif
            {
                Loc.Instance.MissingTranslationStringRequested += Instance_MissingTranslationStringRequested;
            }

            // load the default translation configs
            ReloadTranslations(keepCurrentLanguage: false);

            // set the current language
            Loc.Instance.CurrentLanguageName = Settings.LanguageName;
            Loc.Instance.CurrentLanguageChanged += LocInstance_CurrentLanguageChanged;
        }

        private static void Instance_MissingTranslationStringRequested(object sender, MissingTranslationStringRequestedEventArgs e)
        {
            FLog.Warning($"[{nameof(LocalizationHelper)}] Language \"{e.LanguageName}\" is missing translation \"{e.StringPath}\"");
        }
#endregion Initialize

#region (Private) WriteStreamToFileAsync
        private static async Task WriteStreamToFileAsync(Stream stream, string filePath)
        {
            if (stream.Length == 0)
            {
                await stream.DisposeAsync();
                return;
            }
            try
            {
                using var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);

                await stream.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }
            catch (Exception ex)
            {
                LogWriter?.Error($"[{nameof(LocalizationHelper)}] Failed to write resource stream to file \"{filePath}\" due to an exception:", ex);
            }
            await stream.DisposeAsync();
        }
#endregion (Private) WriteStreamToFileAsync

#region (Private) WriteEmbeddedTranslationConfigs
        private static void WriteEmbeddedTranslationConfigs(bool overwriteExisting)
        {
            // create directory if it doesn't exist
            Directory.CreateDirectory(_defaultTranslationConfigDirectory);

            var assembly = Assembly.GetExecutingAssembly();
            var writeTasks = new List<Task>();

            foreach (var resourcePath in assembly.GetManifestResourceNames())
            {
                if (!resourcePath.StartsWith(_baseResourcePath, StringComparison.Ordinal)) continue;
                else if (!LanguageConfigNameRegex.IsMatch(resourcePath))
                {
                    LogWriter?.Error($"[{nameof(LocalizationHelper)}] Translation config \"{resourcePath}\" does not have a valid name, and was skipped.");
                    continue;
                }

                string resourceName = resourcePath[_baseResourcePath.Length..];
                string outputFilePath = Path.Combine(_defaultTranslationConfigDirectory, resourceName);

                bool fileAlreadyExists = File.Exists(outputFilePath);
                if (overwriteExisting || !fileAlreadyExists)
                {
                    // WriteStreamToFileAsync disposes of the stream for us:
                    writeTasks.Add(WriteStreamToFileAsync(assembly.GetManifestResourceStream(resourcePath)!, outputFilePath));

                    LogWriter?.Debug($"[{nameof(LocalizationHelper)}] Writing \"{resourceName}\" to \"{outputFilePath}\"");
                }
            }

            Task.WhenAll(writeTasks).GetAwaiter().GetResult();
        }
#endregion (Private) WriteEmbeddedTranslationConfigs

#region LoadFromDirectory
        /// <summary>
        /// Loads all translation configs in the specified <paramref name="directoryPath"/>.
        /// </summary>
        /// <remarks>
        /// If the specified <paramref name="directoryPath"/> doesn't exist, the method returns immediately.
        /// </remarks>
        /// <param name="directoryPath">The directory path to load translation configs from.</param>
        public static void LoadFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                LogWriter?.Error($"[{nameof(LocalizationHelper)}] Directory \"{directoryPath}\" doesn't exist!");
                return;
            }
            else
            {
                LogWriter?.Trace($"[{nameof(LocalizationHelper)}] Searching for translation configs in directory \"{directoryPath}\"");
            }

            int successfullyLoadedConfigsCount = 0;

            foreach (var filePath in Directory.EnumerateFiles(directoryPath))
            {
                if (!LanguageConfigNameRegex.IsMatch(filePath))
                {
                    LogWriter?.Warning($"[{nameof(LocalizationHelper)}] Skipped loading \"{filePath}\" because its name is invalid!");
                    continue;
                }

                try
                {
                    Loc.Instance.LoadFromFile(filePath);
                    ++successfullyLoadedConfigsCount;
                    LogWriter?.Trace($"[{nameof(LocalizationHelper)}] Successfully loaded language config \"{filePath}\"");
                }
                catch (Exception ex)
                {
                    LogWriter?.Error($"[{nameof(LocalizationHelper)}] Failed to load language config \"{filePath}\" due to an exception:", ex);
                }
            }

            LogWriter?.Trace($"[{nameof(LocalizationHelper)}] Loaded {successfullyLoadedConfigsCount} translation config{(successfullyLoadedConfigsCount != 1 ? "s" : "")} from directory \"{directoryPath}\".");
        }
#endregion LoadFromDirectory

#region (Private) GetStreamContents
        private static string? GetStreamContents(Stream? stream)
        {
            if (stream == null) return null;

            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
#endregion (Private) GetStreamContents

#region LoadFromManifestResource
        public static void LoadFromManifestResource(Assembly addonAssembly, string resourceName)
        {
            if (JsonLoader.CanLoadFile(resourceName))
                Loc.Instance.LoadFromString(JsonLoader, GetStreamContents(addonAssembly.GetManifestResourceStream(resourceName)));
            else if (YamlLoader.CanLoadFile(resourceName))
                Loc.Instance.LoadFromString(YamlLoader, GetStreamContents(addonAssembly.GetManifestResourceStream(resourceName)));
            AddonManifestResourceConfigs.AddIfUnique((addonAssembly, resourceName));
        }
#endregion LoadFromManifestResource

#region ReloadTranslations
        public static void ReloadTranslations(bool keepCurrentLanguage = true)
        {
            var currentLanguage = Loc.Instance.CurrentLanguageName;

            LogWriter?.Trace($"[{nameof(LocalizationHelper)}] Reloading all translation configs.");
            Loc.Instance.ClearLanguages();

            LoadFromDirectory(_defaultTranslationConfigDirectory);

            foreach (var directoryPath in Settings.CustomLocalizationDirectories)
            {
                LoadFromDirectory(directoryPath);
            }

            foreach (var (addonAssembly, resourceName) in AddonManifestResourceConfigs)
            {
                using var stream = addonAssembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;
                using var reader = new StreamReader(stream);
                Loc.Instance.LoadFromString(JsonLoader, reader.ReadToEnd());
            }

            if (keepCurrentLanguage)
                Loc.Instance.CurrentLanguageName = currentLanguage;
        }
#endregion ReloadTranslations

#endregion Methods

#region EventHandlers

#region Loc.Instance
        private static void LocInstance_CurrentLanguageChanged(object? sender, CurrentLanguageChangedEventArgs e)
        { // update the language in the settings
            if (!e.NewLanguageName.Equals(Settings.LanguageName, StringComparison.Ordinal))
            {
                var previousLanguageName = Settings.LanguageName;
                Settings.LanguageName = e.NewLanguageName;
                LogWriter?.Info($"[{nameof(LocalizationHelper)}] Language changed to \"{e.NewLanguageName}\" (was \"{e.OldLanguageName}\")");
            }
        }
#endregion Loc.Instance

#endregion EventHandlers
    }
}
