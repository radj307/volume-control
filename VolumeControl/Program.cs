using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using VolumeControl.Core;
using VolumeControl.Log;

namespace VolumeControl
{
    internal static class Program
    {
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

                try // validate settings
                {
                    // provoke an exception if the settings file is invalid
                    Properties.Settings.Default.Reload();
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex) // settings are corrupted/invalid
                {
                    string? path = (ex.InnerException as ConfigurationErrorsException)?.Filename;
                    if (MessageBox.Show(
                        $"{Application.ProductName} failed to load the 'user.config' file located at:\n  '{path}'\n\nClick 'Yes' to reset all settings to their default values.\n(This will delete all of your custom settings!)\n\nClick 'No' if you want to attempt to repair it manually.",
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
                FLog.Log.WriteDebug("Binary was built in 'DEBUG' configuration.");
                if (!Properties.Settings.Default.AllowMultipleInstances)
                {
                    var thisProc = Process.GetCurrentProcess();
                    foreach (var proc in Process.GetProcessesByName(thisProc.ProcessName))
                    {
                        if (proc.Id != thisProc.Id)
                        {
                            FLog.Log.WriteWarning($"Killing other instance of Volume Control with PID '{proc.Id}'... (This only occurs in DEBUG configuration.)");
                            proc.Kill();
                        }
                    }
                }
#               else
                FLog.Log.WriteDebug("Binary was built in 'RELEASE' configuration.");

                if (!Properties.Settings.Default.AllowMultipleInstances)
                {
                    Process thisProc = Process.GetCurrentProcess();
                    if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
                    {
                        FLog.Log.WriteFatal("Another instance of Volume Control is already running!");
                        return;
                    }
                }
#               endif

                var form = new Form(); // create the main form

                VC_Static.Log.WriteInfo("Initialization completed, starting the application...");

                Application.Run(form);

                VC_Static.Log.WriteInfo("Application exited normally.");
            }
            catch (Exception ex)
            {
                FLog.Log.WriteExceptionFatal("The program performed a controlled crash because of an unhandled exception!", ex);
            }
        }
    }
}