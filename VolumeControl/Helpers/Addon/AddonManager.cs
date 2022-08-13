using Semver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using VolumeControl.Core;
using VolumeControl.Core.Attributes;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Helpers.Addon
{
    /// <summary>
    /// Manager object for runtime DLL addons.
    /// </summary>
    /// <remarks>This is responsible for handling all addon types.</remarks>
    public class AddonManager
    {
        #region Initializers
        public AddonManager(VolumeControlSettings settings)
        {
            _settings = settings;
            _currentVersion = _settings.CurrentVersion;
            this.AddonDirectories = GetAddonDirectories().ToList();
        }
        #endregion Initializers

        #region Fields
        private readonly VolumeControlSettings _settings;
        private readonly SemVersion _currentVersion;
        #endregion Fields

        #region Properties
        private static Config Settings => (Config.Default as Config)!;
        private static LogWriter Log => FLog.Log;
        /// <summary>
        /// List of directories to search for valid addon assemblies.
        /// </summary>
        public List<string> AddonDirectories { get; set; }
        /// <summary>
        /// This is used by all addon manager instances when enumerating through the files in an addon directory.<br/>
        /// <b>Note that sub-directory recursion is enabled.</b>
        /// </summary>
        public static EnumerationOptions DirectoryEnumerationOptions { get; set; } = new()
        {
            MatchCasing = MatchCasing.CaseInsensitive,
            MatchType = MatchType.Simple,
            RecurseSubdirectories = true
        };
        #endregion Properties

        #region Methods
        /// <summary>
        /// Loads assemblies into the given addon types.
        /// </summary>
        /// <param name="addons">Any enumerable type containing <see cref="BaseAddon"/>-derived types.</param>
        public void LoadAddons(ref List<IBaseAddon> addons)
        {
            if (this.AddonDirectories.Count > 0)
            {
                Log.Debug($"Scanning {this.AddonDirectories.Count} location{(this.AddonDirectories.Count.Equals(1) ? "" : "s")} for addons.");
                int asmCount = 0,
                    totalCount = 0;
                foreach (string dir in this.AddonDirectories)
                {
                    if (!Directory.Exists(dir))
                    {
                        Log.Warning($"Skipped missing directory '{dir}'");
                        continue;
                    }

                    foreach (string path in Directory.EnumerateFiles(dir, "*.dll", DirectoryEnumerationOptions))
                    {
                        try
                        {
                            // load the addon assembly
                            var asm = Assembly.LoadFrom(path);

                            // attempt configuration upgrade if the AllowUpgradeConfigAttribute assembly attribute is present
                            if (asm.GetCustomAttribute<AllowUpgradeConfigAttribute>() is AllowUpgradeConfigAttribute attr && attr.AllowUpgrade)
                            {
                                if (asm.GetTypes().First(t => t.BaseType?.Equals(typeof(ApplicationSettingsBase)) ?? false) is Type t)
                                {
                                    const string appSettingsPropertyName = "Default";
                                    if (t.GetProperty(appSettingsPropertyName)?.GetValue(null) is ApplicationSettingsBase cfg)
                                    {
                                        try
                                        {
                                            cfg.Upgrade();
                                            Log.Debug($"[ADDON]\tUpgraded app configuration in assembly {asm.FullName}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Error($"[ADDON]\tFailed to upgrade app configuration in assembly {asm.FullName} due to an exception!", ex);
                                        }
                                        cfg.Save();
                                        cfg.Reload();
                                    }
                                    else
                                    {
                                        Log.Error($"[ADDON]\tAssembly {asm.FullName} has attribute {nameof(AllowUpgradeConfigAttribute)}, but does not contain any app configurations!");
                                    }
                                }
                            }

                            int typeCount = LoadAddonTypes(ref addons, asm, _currentVersion);

                            if (typeCount == 0)
                            {
                                Log.Debug($"[ADDON]\tSkipped assembly {asm.FullName}; no valid types were found.");
                                continue;
                            }

                            ++asmCount;
                            Log.Debug($"[ADDON]\tLoaded assembly {asm.FullName}");
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Failed to load assembly from '{path}' due to an exception!", ex);
                        }
                    }
                }

                Log.Debug($"Loaded {asmCount}/{totalCount} assemblies from {this.AddonDirectories.Count} director{(this.AddonDirectories.Count == 1 ? "y" : "ies")}");
            }
            else
            {
                Log.Debug($"Skipped loading addons because no search directories were located.");
            }
            // initialize addons
            foreach (IBaseAddon? addon in addons)
            {
                addon.LoadFromTypes();
                Log.Debug($"Initialized {addon.Attribute.Name}");
            }
        }
        /// <summary>
        /// Searches an assembly for addon classes and uses them to populate <paramref name="addons"/>.
        /// </summary>
        /// <returns>The total number of types loaded from <paramref name="asm"/>.</returns>
        private static int LoadAddonTypes(ref List<IBaseAddon> addons, Assembly asm, SemVersion currentVersion)
        {
            int counter = 0;
            Log.Debug($"Loading types from assembly '{asm.FullName}' into {addons.Count} loaded modules.");
            // iterate through the types in the assembly
            foreach (Type type in asm.GetTypes())
            {
                // iterate through addons in the list for each type
                foreach (IBaseAddon? addon in addons)
                {
                    // check for valid addon attributes
                    if (type.GetCustomAttribute(addon.Attribute) is IBaseAddonAttribute bAttr)
                    {
                        Log.Debug($"Loading Class: {{",
                                  $"    Name: {type.FullName}",
                                  $"    Type: {addon.Attribute.Name}",
                                  $"}}"
                                  );

                        Log.Debug($"Found Addon Class: {{ Assembly: '{asm.FullName}', Name: '{type.FullName}', Type: '{addon.Attribute.Name}' }}");
                        // check if the version number is marked as compatible
                        if (bAttr.CompatibleVersions.WithinRange(currentVersion))
                        {
                            addon.Types.Add(type);
                            ++counter;
                            Log.Debug($"Successfully loaded addon class '{type.FullName}'");
                        }
                        else
                        {
#                           pragma warning disable CS0618 // Type or member is obsolete
                            Log.Debug($"Ignoring incompatible addon '{type.FullName}': ( {bAttr.CompatibleVersions.Min ?? "*"} <= {currentVersion} <= {bAttr.CompatibleVersions.Max ?? "*"} )");
#                           pragma warning restore CS0618 // Type or member is obsolete
                        }
                    }
                }
            }
            return counter;
        }
        /// <summary>
        /// Gets the list of addon directory paths to search.
        /// </summary>
        /// <returns>The list of extant directories to search for addons within.</returns>
        private static List<string> GetAddonDirectories()
        {
            List<string> l = new();
            // check default path:
            string defaultPath = Path.Combine(PathFinder.LocalAppData, "Addons");
            if (Directory.Exists(defaultPath))
                l.AddIfUnique(defaultPath);
            // check custom directories:
            if (Settings.CustomAddonDirectories is not null)
            {
                foreach (string? path in Settings.CustomAddonDirectories)
                {
                    if (path is null) continue;
                    if (Directory.Exists(path))
                    {
                        l.AddIfUnique(path);
                        Log.Debug($"Successfully added custom addon search directory '{path}'");
                    }
                    else
                    {
                        Log.Debug($"'{nameof(Settings.CustomAddonDirectories)}' contains an item that wasn't found: '{path}'!");
                    }
                }
            }
            else
            {
                Log.Debug($"{nameof(Settings.CustomAddonDirectories)} is null.");
            }

            return l;
        }
        #endregion Methods
    }
}
