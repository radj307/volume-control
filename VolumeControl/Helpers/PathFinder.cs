using System.Configuration;
using System.IO;
using VolumeControl.Log;

namespace VolumeControl.Helpers
{
    /// <summary>
    /// Helper object that finds the application's local appdata subdirectory.
    /// </summary>
    internal static class PathFinder
    {
        #region Properties
        /// <summary>
        /// The absolute filepath of Volume Control's local appdata subdirectory.
        /// </summary>
        public static string ApplicationAppDataPath => _localAppData ??= FindLocalAppDataConfigDir();
        private static string? _localAppData = null;
        #endregion Properties

        #region Functions
        private static string FindLocalAppDataConfigDir()
        {
            string path = string.Empty;
            if (Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath) is string dir)
            {
                const string searchString = "VolumeControl";

                int pos = dir.IndexOf(searchString);
                if (pos != -1)
                {
                    path = dir[..(pos + searchString.Length)];
                }
                else
                {
                    FLog.Error($"Couldn't locate the target LocalAppData subdirectory '{searchString}' in path '{dir}'");
                }
            }
            return path;
        }
        #endregion Functions
    }
}
