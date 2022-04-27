using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using VolumeControl.Core;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Controls;
using VolumeControl.Core.Controls.Forms;
using VolumeControl.Log;

namespace VolumeControl
{
    internal static class Program
    {
        /// <summary>
        /// Attempts to upgrade the settings from previous versions.
        /// </summary>
        /// <remarks>This works for all assemblies, and should only be called once.</remarks>
        static void UpgradeAllSettings()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
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

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // This is the equivalent of calling ApplicationConfiguration.Initialize()
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetHighDpiMode(HighDpiMode.SystemAware);

                if (Properties.Settings.Default.UpgradeSettings) //< this is only true on the first run of each new version
                {
                    try
                    {
                        UpgradeAllSettings();
                        Properties.Settings.Default.UpgradeSettings = false; //< set to false now that we have successfully upgraded the settings
                        Properties.Settings.Default.Save();
                        Properties.Settings.Default.Reload();
                    }
                    catch (Exception ex)
                    {
                        if (MessageBox.Show(
                            $"An exception was thrown while upgrading settings from a previous version.\n  Exception Message: '{ex.Message}'\n\nDo you want to use the new version's settings instead?\n\n  Click 'OK' to use the new settings.\n  Click 'Cancel' if you want to attempt a manual upgrade.",
                            "Settings Upgrade Failed!",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Error
                        ) == DialogResult.OK)
                        {
                            Properties.Settings.Default.Reload();
                            Properties.Settings.Default.Save();
                        }
                        else
                        {
                            Properties.Settings.Default.UpgradeSettings = true; //< ensure this is triggered again next time, or settings will be overwritten.
                            return;
                        }
                    }
                }


                try // validate settings
                {
                    // provoke an exception if the settings file is invalid
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.Reload();
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex) // settings are corrupted/invalid
                {
                    string? path = (ex.InnerException as ConfigurationErrorsException)?.Filename;
                    if (MessageBox.Show(
                        $"{Application.ProductName} failed to load the 'user.config' file located at:\n  '{path}'\nThis could be caused by an incompatibility during a version upgrade, or due to invalid changes to the file.\n\nClick 'Yes' to reset all settings to their default values.\n(This will delete all of your custom settings!)\n\nClick 'No' if you want to attempt to repair it manually.",
                        "Corrupted 'user.config' File!",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error
                    ) == DialogResult.Yes)
                    {
                        if (path != null)
                        {
                            File.Delete(path);
                            Properties.Settings.Default.Reload();
                            Properties.Settings.Default.Save();
                        }
                    }
                    else Process.GetCurrentProcess().Kill(); //< kill self
                }

                FLog.Initialize();

#               if DEBUG // force enable log when running in DEBUG configuration
                FLog.EnableLog = true;
#               endif

                VC_Static.Initialize(); // Initialize global statics

#               if DEBUG
                FLog.Log.Debug("Binary was built in 'DEBUG' configuration.");
                if (!Properties.Settings.Default.AllowMultipleInstances)
                {
                    var thisProc = Process.GetCurrentProcess();
                    foreach (var proc in Process.GetProcessesByName(thisProc.ProcessName))
                    {
                        if (proc.Id != thisProc.Id)
                        {
                            FLog.Log.Warning($"Killing other instance of Volume Control with PID '{proc.Id}'...");
                            proc.Kill();
                        }
                    }
                }
#               else
                FLog.Log.Debug("Binary was built in 'RELEASE' configuration.");

                if (!Properties.Settings.Default.AllowMultipleInstances)
                {
                    Process thisProc = Process.GetCurrentProcess();
                    if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
                    {
                        FLog.Log.Fatal("Another instance of Volume Control is already running!");
                        return;
                    }
                }
#               endif

                var hkeditForm = new HotkeyEditorForm(); // create the hotkey editor form
                var toastForm = new ToastForm(); // create the toast notification form
                var indicatorForm = new VolumeIndicatorForm(); // create the volume indicator form

                if (toastForm.DisplayCorner == indicatorForm.DisplayCorner) // increase the amount of padding for the toastform so it doesn't overlap
                    toastForm.DisplayPadding = new(toastForm.DisplayPadding.Width + indicatorForm.Size.Width + indicatorForm.DisplayPadding.Width, toastForm.DisplayPadding.Height);//.Width += ;

                var vcForm = new Form(hkeditForm, toastForm, indicatorForm); // create the main form

                toastForm.Owner = vcForm;

                VC_Static.Log.Info("Initialization completed, starting the application...");

                Application.Run(vcForm);

                VC_Static.Log.Info("Application exited normally.");
            }
            catch (Exception ex)
            {
                if (FLog.Initialized)
                    FLog.Log.FatalException(ex);
                else throw;
            }
        }
    }
}