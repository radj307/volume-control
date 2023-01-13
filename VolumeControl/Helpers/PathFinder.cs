using System.Configuration;
using System.IO;
using VolumeControl.Log;

namespace VolumeControl.Helpers
{
    /// <summary>
    /// Helper object that finds the AppData/Local/radj307 directory.
    /// </summary>
    internal static class PathFinder
    {
        private static string? _localAppData = null;
        /// <summary>
        /// The absolute filepath of the AppData/local/radj307 directory.
        /// </summary>
        public static string LocalAppData => _localAppData ??= FindLocalAppDataConfigDir();

        private static string FindLocalAppDataConfigDir()
        {
            string path = string.Empty;
            if (Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath) is string dir)
            {
                const string searchString = "VolumeControl";

                int pos = dir.IndexOf(searchString);
                if (pos != -1)
                    path = dir[..(pos + searchString.Length)];
                else
                    FLog.Log.Error($"Couldn't locate the target LocalAppData subdirectory '{searchString}' in path '{dir}'");
            }
            return path;
        }
    }
}
