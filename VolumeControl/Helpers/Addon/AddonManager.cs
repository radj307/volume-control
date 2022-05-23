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
            _currentVersion = _settings.Version;
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
            if (AddonDirectories.Count == 0) return;
            Log.Debug($"Searching for addon assemblies in {AddonDirectories.Count} director{(AddonDirectories.Count == 1 ? "y" : "ies")}.");
            int asmCount = 0,
                totalCount = 0;
            foreach (string dir in AddonDirectories)
            {
                if (!Directory.Exists(dir))
                {
                    Log.Warning($"Addon directory '{dir}' doesn't exist.");
                    continue;
                }

                Log.Debug($"Searching for Addon Assemblies in '{dir}'");

                foreach (string path in Directory.EnumerateFiles(dir, "*.dll", DirectoryEnumerationOptions))
                {
                    string prefix = $"  [{++totalCount}] ";
                    var asm = Assembly.LoadFrom(path);

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
