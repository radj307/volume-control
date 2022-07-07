using Semver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using VolumeControl.Core;
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
            this.AddonDirectories = GetAddonDirectories().Where(d => Directory.Exists(d)).ToList();
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
                Log.Debug($"Searching for addon assemblies in {this.AddonDirectories.Count} director{(this.AddonDirectories.Count == 1 ? "y" : "ies")}.");
                int asmCount = 0,
                    totalCount = 0;
                foreach (string dir in this.AddonDirectories)
                {
                    if (!Directory.Exists(dir))
                    {
                        Log.Warning($"Addon directory '{dir}' doesn't exist, skipping...");
                        continue;
                    }

                    Log.Debug($"Searching for Addon Assemblies in '{dir}'");

                    foreach (string path in Directory.EnumerateFiles(dir, "*.dll", DirectoryEnumerationOptions))
                    {
                        string prefix = $"  [{++totalCount}] ";
                        try
                        {
                            // load the addon assembly
                            var asm = Assembly.LoadFrom(path);

                            // attempt configuration upgrade if that attribute was specified
                            if (asm.GetCustomAttribute<Core.Attributes.AllowUpgradeConfigAttribute>() is Core.Attributes.AllowUpgradeConfigAttribute attr && attr.AllowUpgrade)
                            {
                                Log.Debug($"{prefix}Found {nameof(Core.Attributes.AllowUpgradeConfigAttribute)}, searching for valid exported configurations...");

                                if (asm.GetTypes().First(t => t.BaseType?.Equals(typeof(ApplicationSettingsBase)) ?? false) is Type t)
                                {
                                    Log.Debug($"{prefix}Found valid configuration type '{t.FullName}'");
                                    const string propertyName = "Default";
                                    if (t.GetProperty(propertyName)?.GetValue(null) is ApplicationSettingsBase cfg)
                                    {
                                        try
                                        {
                                            cfg.Upgrade();
                                            Log.Debug($"{prefix}Successfully performed configuration upgrade for addon '{asm.FullName}'");
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Debug($"{prefix}Upgrade Error:  An exception was thrown!", ex);
                                        }
                                        cfg.Save();
                                        cfg.Reload();
                                    }
                                    else
                                    {
                                        Log.Debug($"{prefix}Upgrade Error:  Failed to locate property '{propertyName}' in type '{t.FullName}'!");
                                    }
                                }
                                else
                                {
                                    Log.Debug($"{prefix}Upgrade Error:  Assembly '{asm.FullName}' doesn't contain any valid configuration types derived from type '{typeof(ApplicationSettingsBase).FullName}'!");
                                }
                            }

                            int typeCount = LoadAddonTypes(ref addons, asm, _currentVersion);

                            if (typeCount == 0)
                            {
                                Log.Debug($"{prefix}No classes found in assembly '{asm.FullName}'");
                                continue;
                            }
                            Log.Debug($"{prefix}Loaded {typeCount} classes from assembly '{asm.FullName}'.");
                            prefix = new(' ', prefix.Length);

                            ++asmCount;
                            Log.Debug($"{prefix}Cached addon '{asm.FullName}'");
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"{prefix}An exception occurred while loading assembly from file '{path}'", ex);
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
            // Check default appdata directory:
            if (Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath) is string dir)
            {
                const string searchString = "radj307";

                int pos = dir.IndexOf(searchString);
                if (pos != -1)
                {
                    string path = Path.Combine(dir[..(pos + searchString.Length)], "Addons");
                    if (Directory.Exists(path))
                        l.AddIfUnique(path);
                }
            }
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
