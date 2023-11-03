using CodingSeb.Localization;
using CodingSeb.Localization.Loaders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VolumeControl.Core;
using VolumeControl.Log.Interfaces;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Helpers
{
    public static class LocalizationHelper
    {
        #region (class) StreamLoader
        class StreamLoader
        {
            #region Fields
            private readonly JsonFileLoader JsonFileLoader = (JsonFileLoader?)LocalizationLoader.Instance.FileLanguageLoaders.FirstOrDefault(loader =>
                loader.GetType().IsAssignableTo(typeof(JsonFileLoader))) ?? new JsonFileLoader();
            #endregion Fields

            #region Methods

            #region LoadFromStream
            /// <summary>
            /// Loads all translations defined in Json format from the specified <paramref name="stream"/>.
            /// </summary>
            /// <remarks>
            /// The caller is responsible for disposing of the <paramref name="stream"/>.
            /// </remarks>
            /// <param name="stream">The stream to load translations from.</param>
            /// <param name="loader">The loader to use for loading translations from the string.</param>
            /// <param name="sourceFileName">Optional source file name.</param>
            public void LoadFromStream(Stream stream, LocalizationLoader loader, string resourceName)
            {
                using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, true, leaveOpen: true);
                string content = reader.ReadToEnd();
                LoadFromString(content, loader, resourceName);
            }
            #endregion LoadFromStream

            #region LoadFromString
            /// <summary>
            /// Load all translations defined in Json format from the specified <paramref name="jsonString"/>.
            /// </summary>
            /// <param name="jsonString">String to load serialized Json format translations from.</param>
            /// <param name="loader">The loader to use for loading translations from the string.</param>
            /// <param name="sourceFileName">Optional source file name.</param>
            public void LoadFromString(string jsonString, LocalizationLoader loader, string sourceFileName = "")
            {
                JObject root = (JObject)JsonConvert.DeserializeObject(jsonString)!;

                root.Properties().ToList()
                    .ForEach(property => ParseSubElement(property, new Stack<string>(), loader, sourceFileName));
            }
            #endregion LoadFromString

            #region ParseSubElement
            private void ParseSubElement(JProperty property, Stack<string> textId, LocalizationLoader loader, string source)
            {
                switch (property.Value.Type)
                {
                case JTokenType.Object:
                    textId.Push(property.Name);
                    ((JObject)property.Value).Properties().ToList()
                        .ForEach(subProperty => ParseSubElement(subProperty, textId, loader, source));
                    textId.Pop();
                    break;
                case JTokenType.String:

                    if (JsonFileLoader.LangIdDecoding == JsonFileLoaderLangIdDecoding.InFileNameBeforeExtension)
                    {
                        textId.Push(property.Name);
                        loader.AddTranslation(
                            JsonFileLoader.LabelPathRootPrefix + string.Join(JsonFileLoader.LabelPathSeparator, textId.Reverse()) + JsonFileLoader.LabelPathSuffix,
                            Path.GetExtension(Regex.Replace(source, @"\.loc\.json", "")).Replace(".", ""),
                            property.Value.ToString(),
                            source);
                        textId.Pop();
                    }
                    else if (JsonFileLoader.LangIdDecoding == JsonFileLoaderLangIdDecoding.DirectoryName)
                    {
                        textId.Push(property.Name);
                        loader.AddTranslation(JsonFileLoader.LabelPathRootPrefix + string.Join(JsonFileLoader.LabelPathSeparator, textId.Reverse()) + JsonFileLoader.LabelPathSuffix,
                            Path.GetDirectoryName(source),
                            property.Value.ToString(),
                            source);
                        textId.Pop();
                    }
                    else
                    {
                        loader.AddTranslation(JsonFileLoader.LabelPathRootPrefix + string.Join(JsonFileLoader.LabelPathSeparator, textId.Reverse()) + JsonFileLoader.LabelPathSuffix, property.Name, property.Value.ToString(), source);
                    }
                    break;
                default:
                    throw new FormatException($"Invalid format in Json language file for property [{property.Name}]");
                }
            }
            #endregion ParseSubElement

            #endregion Methods
        }
        #endregion (class) StreamLoader

        #region Fields
        private const string _baseResourcePath = $"{nameof(VolumeControl)}.Localization.";
        private static string _defaultTranslationConfigDirectory = Path.Combine(PathFinder.ApplicationAppDataPath, "Localization");
        private static readonly Regex LanguageConfigNameRegex = new(@"([a-z]{2}\.loc\.(?:json|yaml|yml))$", RegexOptions.Compiled);
        #endregion Fields

        #region Properties
        private static Config Settings => Config.Default;
        private static ILogWriter? LogWriter { get; set; }
        private static StreamLoader StreamLoaderInstance => _streamLoaderInstance ??= new();
        private static StreamLoader? _streamLoaderInstance = null;
        private static List<(Assembly Assembly, string ManifestResourceName)> AddonManifestResourceConfigs { get; } = new();
        public static bool IsInitialized { get; private set; }
        #endregion Properties

        #region Methods

        #region Initialize
        public static void Initialize(bool overwriteExistingTranslationConfigs, bool logMissingTranslations, ILogWriter? log)
        {
            if (IsInitialized)
                throw new InvalidOperationException($"{nameof(LocalizationHelper)} has already been initialized!");
            IsInitialized = true;

            LogWriter = log;

            // create file loaders
            if (LocalizationLoader.Instance.FileLanguageLoaders.Count == 0)
            {
                LocalizationLoader.Instance.FileLanguageLoaders.Add(new JsonFileLoader());
                LocalizationLoader.Instance.FileLanguageLoaders.Add(new YamlFileLoader());
            }

            // set the default translation config directory
            if (Settings.CreateTranslationConfigsInLocalDirectory)
                _defaultTranslationConfigDirectory = Path.Combine(PathFinder.ExecutableDirectory, "Localization");

            // create the default translation configs
            if (Settings.CreateTranslationConfigs)
            {
                WriteEmbeddedTranslationConfigs(overwriteExistingTranslationConfigs);
            }

            // load the default translation configs
            ReloadTranslations(keepCurrentLanguage: false);

            // setup logging for missing translation keys
            //  (Missing translation event is broken)
            //if (logMissingTranslations || Settings.LogMissingTranslations)
            //{
            //    Loc.MissingTranslationFound += Loc_MissingTranslationFound;
            //    Loc.LogOutMissingTranslations = true;
            //}

            // set the current language
            Loc.Instance.CurrentLanguage = Settings.LanguageName;
            Loc.Instance.CurrentLanguageChanged += LocInstance_CurrentLanguageChanged;
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

                    LogWriter?.Debug($"{nameof(LocalizationHelper)} Writing \"{resourceName}\" to \"{outputFilePath}\"");
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
                    LocalizationLoader.Instance.AddFile(filePath);
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

        #region LoadFromManifestResource
        public static void LoadFromManifestResource(Assembly addonAssembly, string resourceName)
        {
            StreamLoaderInstance.LoadFromStream(addonAssembly.GetManifestResourceStream(resourceName)!, LocalizationLoader.Instance, resourceName);

            AddonManifestResourceConfigs.AddIfUnique((addonAssembly, resourceName));
        }
        #endregion LoadFromManifestResource

        #region ReloadTranslations
        public static void ReloadTranslations(bool keepCurrentLanguage = true)
        {
            var currentLanguage = Loc.Instance.CurrentLanguage;

            LogWriter?.Trace($"[{nameof(LocalizationHelper)}] Reloading all translation configs.");
            LocalizationLoader.Instance.ClearAllTranslations(true);

            LoadFromDirectory(_defaultTranslationConfigDirectory);

            foreach (var directoryPath in Settings.CustomLocalizationDirectories)
            {
                LoadFromDirectory(directoryPath);
            }

            foreach (var (addonAssembly, resourceName) in AddonManifestResourceConfigs)
            {
                StreamLoaderInstance.LoadFromStream(addonAssembly.GetManifestResourceStream(resourceName)!, LocalizationLoader.Instance, resourceName);
            }

            if (keepCurrentLanguage)
                Loc.Instance.CurrentLanguage = currentLanguage;
        }
        #endregion ReloadTranslations

        #endregion Methods

        #region EventHandlers

        #region Loc.Instance
        private static void LocInstance_CurrentLanguageChanged(object? sender, CurrentLanguageChangedEventArgs e)
        { // update the language in the settings
            if (!e.NewLanguageId.Equals(Settings.LanguageName, StringComparison.Ordinal))
            {
                var previousLanguageName = Settings.LanguageName;
                Settings.LanguageName = e.NewLanguageId;
                LogWriter?.Info($"[{nameof(LocalizationHelper)}] Language changed to \"{e.NewLanguageId}\" (was \"{previousLanguageName}\")");
            }
        }
        #endregion Loc.Instance

        #region Loc
        //private static void Loc_MissingTranslationFound(object? sender, LocalizationMissingTranslationEventArgs e)
        //{
        //    string languageName = e.LanguageId ?? Loc.Instance.CurrentLanguage;
        //    LogWriter?.Error($"[{nameof(LocalizationHelper)}] Language \"{languageName}\" is missing translation \"{e.TextId}\"");
        //}
        #endregion Loc

        #endregion EventHandlers
    }
}
