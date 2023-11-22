using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

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
        /// <remarks>
        /// <c>~/AppData/Local/radj307/VolumeControl</c>
        /// </remarks>
        public static string ApplicationAppDataPath => _localAppData ??= FindLocalAppDataConfigDir();
        private static string? _localAppData = null;
        /// <summary>
        /// The path of Volume Control's executable.
        /// </summary>
        public static string ExecutablePath => _executablePath ??= FindExecutablePath();
        private static string? _executablePath = null;
        /// <summary>
        /// The directory that Volume Control's executable is located in.
        /// </summary>
        public static string ExecutableDirectory => _executableDirectory ??= Path.GetDirectoryName(FindExecutablePath()) ?? AppDomain.CurrentDomain.BaseDirectory;
        private static string? _executableDirectory = null;
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
        private static string FindExecutablePath()
        {
            using var proc = Process.GetCurrentProcess();
            return proc.GetMainModulePath() ?? proc.GetMainModulePathWMI()!;
        }
        #endregion Functions
    }
}
