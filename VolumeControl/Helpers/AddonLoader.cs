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
        #region Fields
        private static readonly Regex TranslationConfigAddonRegex = new(@"([a-z]{2}(?:-\w+){0,1}\.loc\.(?:json|yaml|yml))$", RegexOptions.Compiled);
        #endregion Fields

        #region Properties
        private static LocalizationLoader LocalizationLoader => LocalizationLoader.Instance;
        #endregion Properties

        #region Methods

        #region LoadTranslations
        private static void LoadTranslations(Assembly assembly)
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
                else
                {
                    FLog.Debug($"[AddonLoader] Loading translations from embedded resource \"{resourceName}\".");

                    // load translations
                    LocalizationHelper.LoadFromManifestResource(assembly, resourceName);
                }
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
                        var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(dllPath);

                        if (versionInfo != null)
                        {
                            if (FLog.FilterEventType(EventType.DEBUG))
                                FLog.Debug($"[AddonLoader] Found addon DLL \"{versionInfo.FileName}\":", versionInfo.ToString());

                            if (versionInfo.IsDebug)
                                FLog.Warning(
                                 $"[AddonLoader] Addon DLL was built in DEBUG configuration \"{dllPath}\"!",
                                  "              Contact the addon author");
                        }
                        else FLog.Debug($"[AddonLoader] Addon DLL \"{fileName}\" does not have any version information.");

                        // try loading the assembly
                        try
                        {
                            var asm = Assembly.LoadFrom(dllPath);
                            var assemblyName = asm.FullName ?? dllPath;
                            var exportedTypes = asm.GetExportedTypes();

                            FLog.Debug($"[AddonLoader] \"{assemblyName}\" exports {exportedTypes.Length} types.");

                            LoadTranslations(asm);

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
