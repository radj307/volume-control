using AssemblyAttribute;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using VolumeControl.Helpers.Update;
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
            AppDomain? appDomain = AppDomain.CurrentDomain;
            string path = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;

            // this means we're starting back up after the update util completed
            if (args.Contains("--cleanup") && System.IO.File.Exists(path + "VCUpdateUtility.exe") && MessageBox.Show($"Volume Control was updated to v{Assembly.GetExecutingAssembly().GetCustomAttribute<ExtendedVersion>()?.Version}\n\nDo you want to clean up the update utility left behind during the update process?", "Volume Control Updated", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes).Equals(MessageBoxResult.Yes))
                System.IO.File.Delete(path + "VCUpdateUtility.exe"); //< delete the update utility
            else if (args.Contains("--update"))
                
//                UpdateChecker.CheckForUpdates(true);

            // attempt to upgrade settings
            if (Settings.UpgradeSettings)
            {
                Log.Info($"{nameof(Settings.UpgradeSettings)} was true, attempting to migrate existing settings...");
                try
                {
                    UpgradeAllSettings(AppDomain.CurrentDomain.GetAssemblies().Where(asm => asm.FullName?.Contains("VolumeControl", StringComparison.Ordinal) ?? false).ToArray());
                    Settings.UpgradeSettings = false;
                    Log.Info("User settings were upgraded successfully; or there were no settings to upgrade.");
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
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

            if (!isNewInstance)
            {
                Log.Fatal($"Failed to acquire mutex '{appMutexIdentifier}'; another instance of Volume Control (or the update utility) is currently running!");
                MessageBox.Show("Another instance of Volume Control is already running!");
                return;
            }
            //if (Settings.CheckForUpdatesOnStartup && UpdateChecker.CheckForUpdates())
            //{
            //    return;
            //}

            var app = new App();
            try
            {
                int rc = app.Run();
                Log.Info($"App exited with code {rc}");
            }
            catch (Exception ex)
            {
                Log.Fatal("App exited because of an unhandled exception!", ex);
                app.TrayIcon.Dispose();
#               if DEBUG
                throw; //< rethrow exception
#               endif
            }

            GC.WaitForPendingFinalizers();

            Log.Dispose();
            appMutex.ReleaseMutex();
            appMutex.Dispose();
        }

        /// <summary>
        /// Attempts to upgrade the settings from previous versions.
        /// </summary>
        /// <remarks>This works for all assemblies, and should only be called once.</remarks>
        private static void UpgradeAllSettings(params Assembly[] assemblies)
        {
            foreach (Assembly? assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetProperty("Default")?.GetValue(null, null) is global::System.Configuration.ApplicationSettingsBase cfg)
                    {
                        cfg.Upgrade();
                        cfg.Reload();
                        cfg.Save();
                    }
                }
            }
        }
        #endregion Methods
    }
}
