using CodingSeb.Localization.Loaders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using VolumeControl.Core.Input;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;
using VolumeControl.ViewModels;

namespace VolumeControl.Helpers
{
    public class AddonLoader
    {
        #region Constructor
        public AddonLoader()
        {
            ManifestResourceLoader = new((JsonFileLoader?)LocalizationLoader.FileLanguageLoaders.FirstOrDefault(loader =>
                loader.GetType().IsAssignableTo(typeof(JsonFileLoader))) ?? new JsonFileLoader());
        }
        #endregion Constructor

        #region (class) ManifestResourceLocalizationLoader
        class ManifestResourceLocalizationLoader
        {
            #region Constructor
            public ManifestResourceLocalizationLoader(JsonFileLoader jsonFileLoader)
            {
                JsonFileLoader = jsonFileLoader;
            }
            #endregion Constructor

            #region Fields
            private readonly JsonFileLoader JsonFileLoader;
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
        #endregion (class) ManifestResourceLocalizationLoader

        #region Fields
        private static readonly Regex TranslationConfigAddonRegex = new(@"([a-z]{2}(?:-\w+){0,1}\.loc\.(?:json|yaml|yml))$", RegexOptions.Compiled);
        private readonly ManifestResourceLocalizationLoader ManifestResourceLoader;
        #endregion Fields

        #region Properties
        private static LocalizationLoader LocalizationLoader => LocalizationLoader.Instance;
        #endregion Properties

        #region Methods

        #region LoadTranslations
        private void LoadTranslations(Assembly assembly)
        {
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                // filter out invalid names
                var match = TranslationConfigAddonRegex.Match(resourceName);
                if (!match.Success)
                {
                    FLog.Debug($"[AddonLoader] Embedded resource \"{resourceName}\" does not have a valid name for a translation config, so it was not loaded.");
                    continue;
                }

                // load translations
                ManifestResourceLoader.LoadFromStream(assembly.GetManifestResourceStream(resourceName)!, LocalizationLoader, resourceName);
            }
        }
        #endregion LoadTranslations

        #region LoadAddons
        public void LoadAddons(VolumeControlVM inst)
        {
            // create a template provider manager
            var templateProviderManager = new TemplateProviderManager();

            var hotkeyActionsAssembly = Assembly.Load($"{nameof(VolumeControl)}.{nameof(HotkeyActions)}");

            // load default addin translations
            LoadTranslations(hotkeyActionsAssembly);

            // load default template providers
            HotkeyActionAddonLoader.LoadProviders(ref templateProviderManager,
                GetDataTemplateProviderTypes(Assembly.Load($"{nameof(VolumeControl)}.{nameof(SDK)}")));

            // load default actions
            inst.HotkeyAPI.HotkeyManager.HotkeyActionManager.AddActionDefinitions(HotkeyActionAddonLoader.LoadActions(templateProviderManager,
                GetActionGroupTypes(hotkeyActionsAssembly)));

            // load custom addons
            inst.AddonDirectories.ForEach(dir =>
            {
                if (Directory.Exists(dir))
                {
                    FLog.Trace($"[AddonLoader] Searching for DLLs in directory \"{dir}\".");
                    foreach (string dllPath in Directory.EnumerateFiles(dir, "*.dll", new EnumerationOptions() { MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = false, }))
                    {
                        // print version info
                        var fileName = Path.GetFileName(dllPath);
                        FLog.Debug($"[AddonLoader] Found addon DLL \"{fileName}\"");

                        var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(dllPath);
                        if (versionInfo == null)
                        {
                            FLog.Debug($"[AddonLoader] Addon DLL \"{fileName}\" does not have any version information.");
                        }
                        else
                        {
                            var authorName = versionInfo.CompanyName;

                            if (versionInfo.IsDebug)
                            {
                                FLog.Warning(
                                    $"[AddonLoader] Addon DLL was built in DEBUG configuration \"{dllPath}\"!",
                                    $"              Contact {versionInfo.CompanyName ?? "the addon author"}");
                            }
                            if (FLog.FilterEventType(EventType.DEBUG))
                            {

                                FLog.Debug($"[AddonLoader] Found addon DLL \"{versionInfo.FileName}\":", versionInfo.ToString());
                            }
                        }

                        // try loading the assembly
                        try
                        {
                            var asm = Assembly.LoadFrom(dllPath);
                            var assemblyName = asm.FullName ?? dllPath;
                            var exportedTypes = asm.GetExportedTypes();
                            asm = null;

                            FLog.Debug($"[AddonLoader] \"{assemblyName}\" exports {exportedTypes.Length} types.");

                            // load providers from addon assembly
                            HotkeyActionAddonLoader.LoadProviders(ref templateProviderManager, exportedTypes);

                            // load actions from addon assembly
                            inst.HotkeyAPI.HotkeyManager.HotkeyActionManager.AddActionDefinitions(HotkeyActionAddonLoader.LoadActions(templateProviderManager, exportedTypes));
                        }
                        catch (Exception ex)
                        {
                            FLog.Critical($"[AddonLoader] Failed to load addon DLL \"{dllPath}\" due to an exception!", ex);
                        }
                    }
                }
                else
                {
                    FLog.Trace($"[AddonLoader] Addon directory \"{dir}\" doesn't exist.");
                }
            });
        }
        #endregion LoadAddons

        #region GetActionGroupTypes
        /// <summary>
        /// Gets the hotkey action types from the specified <paramref name="assembly"/>.
        /// </summary>
        private static Type[] GetActionGroupTypes(Assembly assembly)
            => assembly.GetExportedTypes().Where(type => type.GetCustomAttribute<Core.Attributes.HotkeyActionGroupAttribute>() != null).ToArray();
        #endregion GetActionGroupTypes

        #region GetDataTemplateProviderTypes
        /// <summary>
        /// Gets the data template provider types from the specified <paramref name="assembly"/>.
        /// </summary>
        private static Type[] GetDataTemplateProviderTypes(Assembly assembly)
            => assembly.GetExportedTypes().Where(type => type.GetCustomAttribute<Core.Attributes.DataTemplateProviderAttribute>() != null).ToArray();
        #endregion GetDataTemplateProviderTypes

        #endregion Methods
    }
}
