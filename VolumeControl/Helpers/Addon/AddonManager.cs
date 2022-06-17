using Semver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using VolumeControl.Core;
using VolumeControl.Log;

namespace VolumeControl.Helpers.Addon
{
    /// <summary>
    /// Manager object for runtime DLL addons.
    /// </summary>
    public class AddonManager
    {
        #region Initializers
        public AddonManager(VolumeControlSettings settings)
        {
            _settings = settings;
            _currentVersion = _settings.CurrentVersion;
            AddonDirectories = GetAddonDirectories().Where(d => Directory.Exists(d)).ToList();
        }
        #endregion Initializers

        #region Fields
        private readonly VolumeControlSettings _settings;
        private readonly SemVersion _currentVersion;
        #endregion Fields

        #region Properties
        private static LogWriter Log => FLog.Log;
        public List<string> AddonDirectories { get; set; }
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
            Log.Debug($"Searching for addon assemblies in {AddonDirectories.Count} director{(AddonDirectories.Count == 1 ? "y" : "ies")}.");
            int asmCount = 0,
                totalCount = 0;
            foreach (string dir in AddonDirectories)
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
                    var asm = Assembly.LoadFrom(path);

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
                            else Log.Debug($"{prefix}Upgrade Error:  Failed to locate property '{propertyName}' in type '{t.FullName}'!");
                        }
                        else Log.Debug($"{prefix}Upgrade Error:  Assembly '{asm.FullName}' doesn't contain any valid configuration types derived from type '{typeof(ApplicationSettingsBase).FullName}'!");
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
            }
            Log.Debug($"Loaded {asmCount} out of {totalCount} assemblies from {AddonDirectories.Count} director{(AddonDirectories.Count == 1 ? "y" : "ies")}");
            // initialize addons
            foreach (var addon in addons)
            {
                addon.LoadFromTypes();
                Log.Debug($"Initialized {addon.Attribute.Name}");
            }
        }
        /// <summary>
        /// Searches an assembly for addon classes.
        /// </summary>
        /// <returns>The number of types loaded into all addons.</returns>
        private static int LoadAddonTypes(ref List<IBaseAddon> addons, Assembly asm, SemVersion currentVersion)
        {
            int counter = 0;
            Log.Debug($"Enumerating Classes from Assembly: {{ Name: '{asm.FullName}', Path: '{asm.Location}' }}");
            foreach (Type type in asm.GetTypes())
            {
                foreach (var addon in addons)
                {
                    if (type.GetCustomAttribute(addon.Attribute) is IBaseAddonAttribute bAttr)
                    {
                        Log.Debug($"Loading Class: {{",
                                  $"    Name: {type.FullName}",
                                  $"    Type: {addon.Attribute.Name}",
                                  $"}}"
                                  );

                        Log.Debug($"Found Addon Class: {{ Assembly: '{asm.FullName}', Name: '{type.FullName}', Type: '{addon.Attribute.Name}' }}");
                        if (bAttr.CompatibleVersions.Contains(currentVersion))
                        {
                            Log.Debug($"Successfully loaded addon class {type.FullName}");
                            addon.Types.Add(type);
                            ++counter;
                        }
                        else
                        {
#                           pragma warning disable CS0618 // Type or member is obsolete
                            Log.Debug($"Ignoring incompatible addon '{type.FullName}': ( {bAttr.CompatibleVersions.Minimum ?? "*"} <= {currentVersion} <= {bAttr.CompatibleVersions.Maximum ?? "*"} )");
#                           pragma warning restore CS0618 // Type or member is obsolete
                        }
                    }
                }
            }
            return counter;
        }
        /// <summary>
        /// Gets the root addon directory path.
        /// </summary>
        /// <returns>The absolute path to the Addons root directory.</returns>
        private static List<string> GetAddonDirectories()
        {
            // TODO: Add other directories here somehow?
            List<string> l = new();
            if (Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath) is string dir)
            {
                const string searchString = "radj307";
                int pos = dir.IndexOf(searchString);
                if (pos == -1) return l;

                l.Add(Path.Combine(dir[..(pos + searchString.Length)], "Addons"));
            }
            return l;
        }
        #endregion Methods
    }
}
