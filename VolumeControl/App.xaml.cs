using System;
using System.Configuration;
using System.Reflection;
using System.Windows;
using VolumeControl.Log;
using VolumeControl.Properties;

namespace VolumeControl
{
    public partial class App : Application
    {
        #region Properties
        private static Settings Settings => Settings.Default;
        private static LogWriter Log => FLog.Log;
        #endregion Properties

        #region ApplicationEventHandlers
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Add a logging event handler
            Settings.Default.SettingsSaving += (s, e) =>
            {
                Log.Debug("Saved current settings to the config file.");
            };
            // attempt to upgrade settings
            if (Settings.UpgradeSettings)
            {
                Log.Info($"{nameof(Settings.UpgradeSettings)} was true, attempting to migrate existing settings...");
                try
                {
                    UpgradeAllSettings();
                    Settings.UpgradeSettings = false;
                    Settings.Save();
                    Settings.Reload();
                    Log.Info("User settings were upgraded successfully; or there were no settings to upgrade.");
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex);
                    if (MessageBox.Show(
                               $"Your current `user.config` file is from an incompatible version of Volume Control, and cannot be migrated.\nDo you want to delete your current config?\n\nClick 'Yes' to reset your config to default.\nClick 'No' to attempt repairs manually.\nSee `volumecontrol.log` for more details.",
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
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Save();
            Settings.Reload();
        }
        #endregion ApplicationEventHandlers

        #region Methods
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
