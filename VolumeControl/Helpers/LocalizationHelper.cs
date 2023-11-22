using Localization;
using Localization.Json;
using Localization.Yaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using VolumeControl.Core;
using VolumeControl.Log;
using VolumeControl.Log.Interfaces;

namespace VolumeControl.Helpers
{
    public static class LocalizationHelper
    {
        #region Fields
        private const string _baseResourcePath = $"{nameof(VolumeControl)}.Localization.";
        private static readonly Regex LanguageConfigNameRegex = new(@"([a-z]{2}\.loc\.(?:json|yaml|yml))$", RegexOptions.Compiled);
        #endregion Fields

        #region Properties
        private static Config Settings => Config.Default;
        private static ILogWriter? LogWriter { get; set; }
        private static List<(Assembly Assembly, string ManifestResourceName, ITranslationLoader Loader)> AddonManifestResourceConfigs { get; } = new();
        public static bool IsInitialized { get; private set; }
        #endregion Properties

        #region Methods

        #region Initialize
        public static void Initialize(bool logMissingTranslations, ILogWriter? log)
        {
            if (IsInitialized)
                throw new InvalidOperationException($"[{nameof(LocalizationHelper)}] has already been initialized!");
            IsInitialized = true;

            LogWriter = log;

            // create file loaders
            Loc.Instance.AddTranslationLoader<JsonSingleTranslationLoader>();
            Loc.Instance.AddTranslationLoader<JsonTranslationLoader>();
            Loc.Instance.AddTranslationLoader<YamlSingleTranslationLoader>();
            Loc.Instance.AddTranslationLoader<YamlTranslationLoader>();

            // use blank string instead of key when no default text was provided
            Loc.Instance.UseKeyAsFallback = false;

#if !DEBUG
            if (Settings.LogMissingTranslations)
#endif
            {
                Loc.Instance.MissingTranslationStringRequested += Instance_MissingTranslationStringRequested;
            }

            // print log messages when loading new languages
            Loc.Instance.LanguageAdded += Instance_LanguageAdded;

            // load the default translation configs
            ReloadTranslations(clearCurrentLanguage: true);

            // set the current language
            Loc.Instance.CurrentLanguageName = Settings.LanguageName;
            Loc.Instance.CurrentLanguageChanged += LocInstance_CurrentLanguageChanged;
            // set the fallback language to english
            Loc.Instance.FallbackLanguageName = Loc.Instance.AvailableLanguageNames.FirstOrDefault(langName => langName.StartsWith("English", StringComparison.OrdinalIgnoreCase));
        }
        #endregion Initialize

        #region LoadFromManifestResource
        public static void LoadFromManifestResource(Assembly addonAssembly, string resourceName, ITranslationLoader? translationLoader = null)
        {
            string? serial = null;
            using (var stream = addonAssembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) return;
                using var reader = new StreamReader(stream);
                serial = reader.ReadToEnd();
            }
            if (serial == null) return;

            try
            {
                try
                {
                    if (translationLoader != null && Loc.Instance.LoadFromString(translationLoader, serial))
                    {
                        AddonManifestResourceConfigs.Add((addonAssembly, resourceName, translationLoader));
                        return;
                    }
                }
                catch { }

                foreach (var loader in Loc.Instance.TranslationLoaders)
                {
                    try
                    {
                        if (Loc.Instance.LoadFromString(loader, serial))
                        {
                            AddonManifestResourceConfigs.Add((addonAssembly, resourceName, loader));
                            return;
                        }
                    }
                    catch { }
                }
            }
            finally
            {
                LogWriter?.Trace($"[{nameof(LocalizationHelper)}] Assembly \"{addonAssembly.FullName}\" adds translation config \"{resourceName}\".");
            }
        }
        #endregion LoadFromManifestResource

        #region ReloadTranslations
        public static void ReloadTranslations(bool clearCurrentLanguage = false)
        {
            LogWriter?.Trace($"[{nameof(LocalizationHelper)}] Reloading translation configs.");
            Loc.Instance.ClearLanguages(clearCurrentLanguage);

            // load the built-in translation configs
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (LanguageConfigNameRegex.IsMatch(resourceName))
                {
                    string? serial = null;
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream == null) continue;
                        using var reader = new StreamReader(stream);
                        serial = reader.ReadToEnd();
                    }

                    Loc.Instance.LoadFromString(serial);
                }
            }

            foreach (var directoryPath in Settings.LocalizationDirectories)
            {
                Loc.Instance.LoadFromDirectory(directoryPath);
            }

            foreach (var (addonAssembly, resourceName, loader) in AddonManifestResourceConfigs)
            {
                using var stream = addonAssembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;
                using var reader = new StreamReader(stream);
                Loc.Instance.LoadFromString(loader, reader.ReadToEnd());
            }
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
        private static void Instance_MissingTranslationStringRequested(object sender, MissingTranslationRequestedEventArgs e)
        {
            string? keyString = e.Key;
            if (e.Keys?.Count() > 0)
                keyString = '"' + string.Join("\", \"", e.Keys) + '"';
            if (keyString != null)
                LogWriter?.Warning($"[{nameof(LocalizationHelper)}] \"{e.LanguageName}\" is missing a translation for key \"{keyString}\"");
        }
        private static void Instance_LanguageAdded(object? sender, LanguageEventArgs e)
        {
            LogWriter?.Debug($"[{nameof(LocalizationHelper)}] Loaded language \"{e.LanguageName}\" with {e.Translations.Count} translations.");
        }
        #endregion Loc.Instance

        #endregion EventHandlers
    }
}
