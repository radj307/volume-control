using System.Configuration;
using System.Diagnostics;
using VolumeControl.Core;
using VolumeControl.Log;
using VolumeControl.Core.Controls.Forms;

namespace VolumeControl
{
    internal class VCApplicationContext : ApplicationContext
    {
        /// <summary>
        /// Handles <see cref="System.Windows.Forms.Form.FormClosed"/> events.<br/>
        /// This kills the associated thread and decrements the <see cref="openForms"/> counter.
        /// </summary>
        /// <remarks>These events are only triggered once the <see cref="System.Windows.Forms.Form.FormClosing"/> event has returned <b>without setting Cancel to true.</b></remarks>
        /// <param name="sender">The form that sent the event.</param>
        /// <param name="e">The event arguments associated with this event.</param>
        private void HandleFormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is Form form)
            { // hide the tray icon
                VC_Static.Log.Info("Closing the main form.");
                if (form.TrayIcon.Container != null)
                {
                    VC_Static.Log.Debug($"The main form's tray icon contains {form.TrayIcon.Container.Components.Count} components to be disposed of.");
                    foreach (object? item in form.TrayIcon.Container.Components)
                    {
                        if (item == null || item.Equals(form.TrayIcon))
                            continue;
                        if (item is IDisposable disposableItem)
                            disposableItem.Dispose();
                        else
                            VC_Static.Log.FollowupIf(Log.Enum.EventType.DEBUG, $"- Found Non-Disposable Object Type '{item.GetType().FullName}'!");
                    }
                }
                form.TrayIcon.Visible = false;
                form.TrayIcon.Icon = null;
                form.TrayIcon.Dispose();
                VC_Static.Log.Debug("Finished cleaning up the tray icon.");
            }
            VC_Static.Log.Info($"Closing '{sender.GetType().FullName}', the number of open forms is now {openForms - 1}");
            if (Interlocked.Decrement(ref openForms) == 0)
                ExitThread();
        }

        public VCApplicationContext()
        {
            // Initialize forms
            hkeditForm = new HotkeyEditorForm();
            toastForm = new ToastForm();
            indicatorForm = new VolumeIndicatorForm();
            vcForm = new Form(hkeditForm, toastForm, indicatorForm);

            // if the toastform & indicator are shown in the same corner, increase the amount of padding for the toastform so it doesn't overlap
            if (toastForm.DisplayCorner == indicatorForm.DisplayCorner)
                toastForm.DisplayPadding = new(toastForm.DisplayPadding.Width + indicatorForm.Size.Width + indicatorForm.DisplayPadding.Width, toastForm.DisplayPadding.Height);//.Width += ;

            // Initialize form counter
            openForms = 4;

            // Set owners
            hkeditForm.Owner = vcForm;
            toastForm.Owner = vcForm;
            indicatorForm.Owner = vcForm;

            // Set event callbacks
            hkeditForm.FormClosed += HandleFormClosed!;
            toastForm.FormClosed += HandleFormClosed!;
            indicatorForm.FormClosed += HandleFormClosed!;
            vcForm.FormClosed += HandleFormClosed!;

            if (!Properties.Settings.Default.StartMinimized)
            {
                vcForm.Show();
            }
        }

        private readonly HotkeyEditorForm hkeditForm;
        private readonly ToastForm toastForm;
        private readonly VolumeIndicatorForm indicatorForm;
        private readonly Form vcForm;
        private int openForms;
    }

    internal static class Program
    {
        /// <summary>
        /// Attempts to upgrade the settings from previous versions.
        /// </summary>
        /// <remarks>This works for all assemblies, and should only be called once.</remarks>
        private static void UpgradeAllSettings()
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
        private static void Main()
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
#else
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
#endif

                VC_Static.Log.Info("Core systems initialization complete. Initializing Forms...");

                VCApplicationContext appContext = new();

                VC_Static.Log.Info("Form initialization complete. Starting application...");

                Application.Run(appContext);

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