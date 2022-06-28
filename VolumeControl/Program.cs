using AssemblyAttribute;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using VolumeControl.Core.Attributes;
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
            // vvvvvvvvvvvvvvvvvvvvvv DEBUG vvvvvvvvvvvvvvvvvvvvvv

            Core.Config cfg = new();
            cfg.Save();

            cfg.Load();

            var def = Core.Config.Default;

            return;
            // ^^^^^^^^^^^^^^^^^^^^^^ DEBUG ^^^^^^^^^^^^^^^^^^^^^^

            bool waitForMutex = args.Contains("--wait-for-mutex");

            AppDomain? appDomain = AppDomain.CurrentDomain;
            string path = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;

            // this means we're starting back up after the update util completed
            if (args.Contains("--cleanup") && System.IO.File.Exists(path + "VCUpdateUtility.exe") && MessageBox.Show($"Volume Control was updated to v{Assembly.GetExecutingAssembly().GetCustomAttribute<ExtendedVersion>()?.Version}\n\nDo you want to clean up the update utility left behind during the update process?", "Volume Control Updated", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes).Equals(MessageBoxResult.Yes))
            {
                try
                {
                    System.IO.File.Delete(path + "VCUpdateUtility.exe"); //< delete the update utility
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }

            // attempt to upgrade settings
            if (Settings.UpgradeSettings)
            {
                Log.Info(nameof(Settings.UpgradeSettings) + " was true, attempting to migrate existing settings...");
                try
                {
                    UpgradeAllSettings(appDomain.GetAssemblies().ToArray());
                    Settings.UpgradeSettings = false;
                    Log.Info("Configuration upgrade completed without error.");
                }
                catch (Exception ex)
                {
                    Log.Error($"Config migration failed because of an unhandled exception", ex);

                    if (MessageBox.Show("Your current `user.config` file is from an incompatible version of Volume Control, and cannot be migrated.\nDo you want to delete your current config?\n\nClick 'Yes' to reset your config to default.\nClick 'No' to attempt repairs manually.\nSee `volumecontrol.log` for more details.",
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
                if (waitForMutex)
                    _ = appMutex.WaitOne(); //< wait until the mutex is acquired
                else
                {
                    Log.Fatal($"Failed to acquire mutex '{appMutexIdentifier}'; another instance of Volume Control (or the update utility) is currently running!");
                    MessageBox.Show("Another instance of Volume Control is already running!");
                    return;
                }
            }

            // create the application class
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

                using var sr = Log.Endpoint.GetReader();
                if (sr is not null)
                {
                    string content = sr.ReadToEnd();
                    sr.Dispose();

                    WriteCrashDump(content);
                }
                else Log.Fatal($"Failed to create crash log dump!");

#               if DEBUG
                appMutex.ReleaseMutex();
                appMutex.Dispose();
                throw; //< rethrow in debug configuration
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
            foreach (var asm in assemblies)
            {
                if ((asm.GetCustomAttribute<AllowUpgradeConfigAttribute>() is AllowUpgradeConfigAttribute attr && attr.AllowUpgrade) || (asm.FullName?.Contains("VolumeControl.Log", StringComparison.Ordinal) ?? false))
                {
                    if (asm.GetTypes().First(t => t.BaseType?.Equals(typeof(System.Configuration.ApplicationSettingsBase)) ?? false) is Type t)
                    {
                        if (t.GetProperty("Default")?.GetValue(null) is System.Configuration.ApplicationSettingsBase cfg)
                        {
                            try
                            {
                                cfg.Upgrade();
                                Log.Info($"Successfully upgraded configuration of assembly '{asm.FullName}'");
                            } 
                            catch (Exception ex)
                            {
                                Log.Error($"An exception was thrown while attempting to update configuration of assembly '{asm.FullName}'", ex);
                            }
                            cfg.Save();
                            cfg.Reload();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Writes <paramref name="content"/> to an automatically-generated crash dump file.
        /// </summary>
        /// <param name="content">The string to write to the dump file.</param>
        private static void WriteCrashDump(string content)
        {
            int count = 0;
            foreach (string path in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "volumecontrol-crash-*.log"))
            {
                var match = Regex.Match(path, "\\d+");
                if (match.Success && int.TryParse(match.Value, out int number) && number > count)
                    count = number;
            }
            using var sw = new StreamWriter($"volumecontrol-crash-{++count}.log");

            sw.Write(content);
            sw.Flush();
            sw.Close();

            sw.Dispose();
        }
        #endregion Methods
    }
}
