using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using VolumeControl.Core;
using VolumeControl.Core.Input;
using VolumeControl.Log;
using VolumeControl.ViewModels;

namespace VolumeControl.Helpers
{
    public class AddonLoader
    {
        #region Fields
        private static readonly Regex TranslationConfigAddonRegex = new(@"([a-z]{2}(?:-\w+){0,1}\.loc\.(?:json|yaml|yml))$", RegexOptions.Compiled);
        #endregion Fields

        #region Properties
        private static Config Settings => Config.Default;
        #endregion Properties

        #region Methods

        #region LoadTranslations
        private static void LoadTranslations(Assembly assembly)
        {
            // get the names of all embedded resources in the assembly
            var manifestResourceNames = assembly.GetManifestResourceNames();
            bool writeDebugLogMessages = FLog.FilterEventType(EventType.DEBUG);
            if (writeDebugLogMessages)
            {
                if (manifestResourceNames.Length == 0)
                {
                    FLog.Debug($"[{nameof(AddonLoader)}] Assembly \"{assembly.FullName}\" does not contain any embedded resources.");
                    return;
                }
                else
                {
                    FLog.Debug($"[{nameof(AddonLoader)}] Assembly \"{assembly.FullName}\" contains {manifestResourceNames.Length} embedded resources. Preparing to search them for translation configs.");
                }
            }

            int totalCount = 0;
            int loadedCount = 0;

            // enumerate embedded resources
            foreach (var resourceName in manifestResourceNames)
            {
                // filter out invalid names
                if (!TranslationConfigAddonRegex.IsMatch(resourceName)) continue;
                else ++totalCount; // this is a translation config.

                if (writeDebugLogMessages)
                    FLog.Debug($"[{nameof(AddonLoader)}] Loading translation config from embedded resource \"{resourceName}\".");
                try
                {
                    // load translation config resource
                    LocalizationHelper.LoadFromManifestResource(assembly, resourceName);
                    ++loadedCount;
                }
                catch (Exception ex)
                {
                    FLog.Error($"[{nameof(AddonLoader)}] An exception occurred while loading embedded resource \"{resourceName}\":", ex);
                }
            }


            // log the outcome
            if (!writeDebugLogMessages) return;
            else if (manifestResourceNames.Length == 0 && totalCount == 0)
                FLog.Debug($"[{nameof(AddonLoader)}] Assembly \"{assembly.FullName}\" does not contain any translation configs.");
            else
                FLog.Debug($"[{nameof(AddonLoader)}] Loaded {loadedCount}/{totalCount} translation config{(totalCount == 1 ? "" : "s")} from assembly \"{assembly.FullName}\"");
        }
        #endregion LoadTranslations

        #region (Private) LoadAddonsFromDirectory
        private static void LoadAddonsFromDirectory(string directoryPath, bool recursive, VolumeControlVM inst, TemplateProviderManager providerManager)
        {
            bool showTraceMessages = FLog.FilterEventType(EventType.TRACE);
            if (showTraceMessages)
                FLog.Trace($"[{nameof(AddonLoader)}] Searching for addon DLLs in directory \"{directoryPath}\".");
            int totalCount = 0;
            int loadedCount = 0;
            foreach (string dllPath in Directory.EnumerateFiles(directoryPath, "*.dll", new EnumerationOptions() { MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = recursive, }))
            {
                ++totalCount;
                // get file version info
                var dllName = Path.GetFileName(dllPath);
                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(dllPath);

                // report that we found an addon DLL
                var versionInfoNumber = versionInfo.ProductVersion ?? versionInfo.FileVersion;
                if (versionInfoNumber != null)
                {
                    FLog.Info($"[{nameof(AddonLoader)}] Found addon \"{versionInfo.ProductName ?? dllName}\" version {versionInfoNumber}");
                }
                else
                {
                    FLog.Warning($"[{nameof(AddonLoader)}] Found addon \"{dllName}\", but it doesn't have any version information!");
                }

                // ensure the DLL was not compiled in DEBUG configuration
                if (versionInfo.IsDebug)
                {
#if DEBUG           // we're in DEBUG configuration; warn & continue
                    FLog.Warning($"[{nameof(AddonLoader)}] Addon DLL \"{dllName}\" was built in DEBUG configuration! (Volume Control will not be able to load this in RELEASE configuration!)");
#else               // we're in RELEASE configuration; error & skip
                    FLog.Error($"[{nameof(AddonLoader)}] Addon DLL \"{dllName}\" was built in DEBUG configuration and cannot be loaded! Report this issue to the addon author.");
                    continue;
#endif
                }

                // try loading the assembly
                try
                {
                    var asm = Assembly.LoadFrom(dllPath);
                    var assemblyName = asm.FullName ?? dllPath;
                    var exportedTypes = asm.GetExportedTypes();

                    FLog.Debug($"[{nameof(AddonLoader)}] Found {exportedTypes.Length} public type{(exportedTypes.Length == 1 ? "" : "s")} in assembly \"{assemblyName}\".");

                    // load translations from addon assembly
                    LoadTranslations(asm);

                    // load providers from addon assembly
                    HotkeyActionAddonLoader.LoadProviders(ref providerManager, exportedTypes);

                    // load actions from addon assembly
                    inst.HotkeyAPI.HotkeyManager.HotkeyActionManager.AddActionDefinitions(HotkeyActionAddonLoader.LoadActions(providerManager, exportedTypes));
                    ++loadedCount;
                }
                catch (Exception ex)
                {
                    FLog.Critical($"[{nameof(AddonLoader)}] Failed to load addon DLL \"{dllPath}\" due to an exception!", ex);
                }
            }
            if (!showTraceMessages) return;
            if (totalCount == 0)
                FLog.Trace($"[{nameof(AddonLoader)}] No addon DLLs found in directory \"{directoryPath}\".");
            else
                FLog.Trace($"[{nameof(AddonLoader)}] Loaded {loadedCount}/{totalCount} addon DLLs from directory \"{directoryPath}\".");
        }
        #endregion (Private) LoadAddonsFromDirectory

        #region LoadAddons
        /// <summary>
        /// Loads addons from the default and custom addon directories.
        /// </summary>
        /// <remarks>
        /// Addon components are loaded in the following order:
        /// <list type="number">
        /// <item>Translation Configs</item>
        /// <item>DataTemplate Providers</item>
        /// <item>Actions</item>
        /// </list>
        /// The built-in translations/providers/actions are loaded first, then addons in the default location, then addons in custom directories.
        /// </remarks>
        /// <param name="inst">The main window's view model instance.</param>
        public void LoadAddons(VolumeControlVM inst)
        {
            // create a template provider manager
            var templateProviderManager = new TemplateProviderManager();

            // load default addin translations
            var hotkeyActionsAssembly = Assembly.Load($"{nameof(VolumeControl)}.{nameof(HotkeyActions)}");
            LoadTranslations(hotkeyActionsAssembly);

            // load default template providers
            HotkeyActionAddonLoader.LoadProviders(ref templateProviderManager,
                GetDataTemplateProviderTypes(Assembly.Load($"{nameof(VolumeControl)}.{nameof(SDK)}")));

            // load default actions
            inst.HotkeyAPI.HotkeyManager.HotkeyActionManager.AddActionDefinitions(HotkeyActionAddonLoader.LoadActions(templateProviderManager,
                GetActionGroupTypes(hotkeyActionsAssembly)));
            hotkeyActionsAssembly = null;

            // create the default addons directory if it doesn't exist
            string defaultAddonDirectory = Path.Combine(PathFinder.ApplicationAppDataPath, "Addons");
            if (!Directory.Exists(defaultAddonDirectory))
            {
                try
                {
                    Directory.CreateDirectory(defaultAddonDirectory);
                }
                catch { }
            }

            // load addons from default directory
            if (Directory.Exists(defaultAddonDirectory))
                LoadAddonsFromDirectory(defaultAddonDirectory, true, inst, templateProviderManager);

            // load addons from custom directories
            foreach (var directoryPath in Settings.CustomAddonDirectories)
            {
                if (Directory.Exists(directoryPath))
                {
                    LoadAddonsFromDirectory(directoryPath, false, inst, templateProviderManager);
                }
                else
                {
                    FLog.Trace($"[{nameof(AddonLoader)}] Addon directory \"{directoryPath}\" doesn't exist.");
                }
            }
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
