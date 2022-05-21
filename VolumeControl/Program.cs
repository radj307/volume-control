using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using VolumeControl.Helpers;
using VolumeControl.Log;

namespace VolumeControl
{
    internal static class Program
    {
        #region Statics
        [DllImport("Kernel32")]
        private static extern void AllocConsole();
        [DllImport("Kernel32")]
        private static extern void FreeConsole();
        private static Properties.Settings Settings => Properties.Settings.Default;
        private static LogWriter Log => FLog.Log;
        #endregion Statics

        #region Fields
        private const string appMutexIdentifier = "VolumeControlSingleInstance";
        private static Mutex appMutex = null!;
        #endregion Fields

        #region Methods
        /// <summary>
        /// Program entry point
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Contains("--autoupdated"))
            { // delete updater file
                var appDomain = AppDomain.CurrentDomain;
                string path = System.IO.Path.Combine(appDomain.RelativeSearchPath ?? appDomain.BaseDirectory, "VolumeControl.UpdateUtility.exe");
                System.IO.File.Delete(path);
            }

            // attempt to upgrade settings
            if (Settings.UpgradeSettings)
            {
                Log.Info($"{nameof(Settings.UpgradeSettings)} was true, attempting to migrate existing settings...");
                try
                {
                    UpgradeAllSettings();
                    Settings.UpgradeSettings = false;
                    Log.Info("User settings were upgraded successfully; or there were no settings to upgrade.");
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex);
                    if (MessageBox.Show($"Your current `user.config` file is from an incompatible version of Volume Control, and cannot be migrated.\nDo you want to delete your current config?\n\nClick 'Yes' to reset your config to default.\nClick 'No' to attempt repairs manually.\nSee `volumecontrol.log` for more details.",
                                        "Invalid User Configuration File",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Error,
                                        MessageBoxResult.No,
                                        MessageBoxOptions.None) == MessageBoxResult.Yes)
                    {
                        Settings.Reload();
                        Settings.Save();
                    }
                    else
                    {
                        Settings.UpgradeSettings = true; //< ensure this is triggered again next time, or settings will be overwritten.
                        return;
                    }
                }
            }

            // Multi instance gate
            bool isNewInstance = false;
            appMutex = new(true, appMutexIdentifier, out isNewInstance);

            if (!Settings.AllowMultipleInstances && !isNewInstance)
            {
                MessageBox.Show("Another instance of Volume Control is already running!");
                return;
            }

            var app = new App();
            try
            {
                int rc = app.Run();
                Log.Info($"App exited with code {rc}");
            }
            catch (Exception ex)
            {
                Log.FatalException(ex, "App exited because of an unhandled exception!");
                app.CleanupTrayIcon();
            }

            GC.WaitForPendingFinalizers();

            appMutex.ReleaseMutex();
            appMutex.Dispose();
        }

        /// <summary>
        /// Attempts to upgrade the settings from previous versions.
        /// </summary>
        /// <remarks>This works for all assemblies, and should only be called once.</remarks>
        private static void UpgradeAllSettings()
        {
            foreach (Assembly? assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type? type in assembly.GetTypes())
                {
                    if (type.Name == "Settings" && typeof(SettingsBase).IsAssignableFrom(type))
                    {
                        var cfg = (ApplicationSettingsBase?)type.GetProperty("Default")?.GetValue(null, null);
                        if (cfg != null)
                        {
                            cfg.Upgrade();
                            cfg.Reload();
                            cfg.Save();
                        }
                    }
                }
            }
        }
        #endregion Methods
    }
}
