using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using VolumeControl.Hotkeys.Attributes;

namespace VolumeControl.Helpers.Addon
{
    /// <summary>
    /// Manager object for runtime DLL addons.
    /// </summary>
    public class AddonManager
    {
        #region Initializers
        public AddonManager()
        {
            _rootAddonPath = GetAddonPath();
            AddonAssemblies = new();
            // load addon assemblies
            GetAddonAssemblyPaths().ForEach(p => AddonAssemblies.Add(Assembly.LoadFrom(p)));
        }
        #endregion Initializers

        #region Fields
        private readonly string _rootAddonPath;
        private List<Type>? _actionAddonTypes = null;
        #endregion Fields

        #region Properties
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
                return l;

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
                const string searchString = "VolumeControl";
                int pos = dir.IndexOf(searchString);
                if (pos == -1)
                    return string.Empty;
                return Path.Combine(dir[..(pos + searchString.Length)], "Addons");
            }
            return string.Empty;
        }
        private static List<Type> GetTypesWithAttribute<T>(Assembly asm) where T : Attribute
        {
            List<Type> l = new();
            foreach (Type type in asm.GetTypes())
                if (type.GetCustomAttribute<T>() != null)
                    l.Add(type);
            return l;
        }
        private List<Type> GetTypesWithAttribute<T>() where T : Attribute
        {
            List<Type> l = new();
            foreach (Assembly asm in AddonAssemblies)
                l.AddRange(GetTypesWithAttribute<T>(asm));
            return l;
        }
        #endregion Methods
    }
}
