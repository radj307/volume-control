using Semver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using VolumeControl.Core;
using VolumeControl.Hotkeys.Attributes;
using VolumeControl.Log;

namespace VolumeControl.Helpers.Addon
{
    /// <summary>
    /// Manager object for runtime DLL addons.
    /// </summary>
    public class AddonManager
    {
        #region Initializers
        public AddonManager(SemVersion version)
        {
            _currentVersion = version;
            _rootAddonPath = GetAddonPath();
            Log.Debug($"Checking for addon assemblies located in directory: '{_rootAddonPath}'.");
            AddonAssemblies = new();
            // load addon assemblies
            GetAddonAssemblyPaths().ForEach(p => AddonAssemblies.Add(Assembly.LoadFrom(p)));
            // log
            if (AddonAssemblies.Count == 0)
            {
                Log.Debug($"No addon assemblies were found.");
            }
            else
            {
                Log.Debug($"Found {AddonAssemblies.Count} addon assemblies.");
            }
        }
        #endregion Initializers

        #region Fields
        private readonly SemVersion _currentVersion;
        private readonly string _rootAddonPath;
        private List<Type>? _actionAddonTypes = null;
        #endregion Fields

        #region Properties
        private static LogWriter Log => FLog.Log;
        public readonly List<Assembly> AddonAssemblies;
        public List<Type> ActionAddonTypes => _actionAddonTypes ??= GetTypesWithAttribute<ActionAddonAttribute>();
        #endregion Properties

        #region Methods
        /// <summary>
        /// Gets the filepaths to all valid addon assemblies.
        /// </summary>
        /// <returns>List of the absolute filepaths of all detected addon assemblies.</returns>
        public List<string> GetAddonAssemblyPaths()
        {
            List<string> l = new();

            if (!Directory.Exists(_rootAddonPath))
            {
                return l;
            }

            foreach (string path in Directory.EnumerateFiles(_rootAddonPath, "*.dll", new EnumerationOptions()
            {
                MatchCasing = MatchCasing.CaseInsensitive,
                MatchType = MatchType.Simple,
                RecurseSubdirectories = true
            }))
            {
                l.Add(path);
            }
            return l;
        }

        private static string GetAddonPath()
        {
            if (Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath) is string dir)
            {
                const string searchString = "radj307";
                int pos = dir.IndexOf(searchString);
                if (pos == -1)
                {
                    return string.Empty;
                }

                return Path.Combine(dir[..(pos + searchString.Length)], "Addons");
            }
            return string.Empty;
        }
        private static List<Type> GetTypesWithAttribute<T>(Assembly asm, SemVersion currentVersion) where T : BaseAddonAttribute
        {
            Log.Debug($"Enumerating Classes from Addon: {{ Name: '{asm.FullName}', Path: '{asm.Location}' }}");
            List<Type> l = new();
            foreach (Type type in asm.GetTypes())
            {
                if (type.GetCustomAttribute<T>() is IBaseAddonAttribute bAttr)
                {
                    Log.Debug($"Loading Addon: {{", 
                              $"    Name: {type.FullName}",
                              $"    Type: {typeof(T).Name}",
                              $"}}"
                              );

                    Log.Debug($"Found Addon Class: {{ Assembly: '{asm.FullName}', Name: '{type.FullName}', Type: '{typeof(T).Name}' }}");
                    if (bAttr.CompatibleVersions.Contains(currentVersion))
                    {
                        Log.Debug($"Successfully loaded addon class {type.FullName}");
                        l.Add(type);
                    }
                    else
                    {
#                       pragma warning disable CS0618 // Type or member is obsolete
                        Log.Debug($"Ignoring incompatible addon '{type.FullName}': ( {bAttr.CompatibleVersions.Minimum ?? "*"} <= {currentVersion} <= {bAttr.CompatibleVersions.Maximum ?? "*"} )");
#                       pragma warning restore CS0618 // Type or member is obsolete
                    }
                }
            }
            return l;
        }
        private List<Type> GetTypesWithAttribute<T>() where T : BaseAddonAttribute
        {
            List<Type> l = new();
            foreach (Assembly asm in AddonAssemblies)
            {
                l.AddRange(GetTypesWithAttribute<T>(asm, _currentVersion));
            }

            return l;
        }
        #endregion Methods
    }
}
