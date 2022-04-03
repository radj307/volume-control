using System.Diagnostics;
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
            FLog.Initialize();
            try
            {
                ApplicationConfiguration.Initialize();

                VC_Static.Initialize(); // Initialize global statics

#               if DEBUG
                FLog.Log.WriteDebug("This binary was built in Debug configuration.");
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
                VC_Static.Log.WriteDebug("This binary was built in Release configuration.");

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