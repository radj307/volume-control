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
#               else
                VC_Static.Log.WriteDebug("This binary was built in Release configuration.");
#               endif

                if (!Properties.Settings.Default.AllowMultipleInstances)
                {
                    VC_Static.Log.WriteInfo("Multiple instances are disabled, checking for other instances...");
                    Process thisProc = Process.GetCurrentProcess(); // check for other processes named the same thing.
                    VC_Static.Log.WriteInfo(new string[] {
                        "Retrieved information about this Volume Control instance from Windows.",
                        $"Process Name:  '{thisProc.ProcessName}'",
                        $"Process ID:    '{thisProc.Id}'"
                    });
                    var similarProcs = Process.GetProcessesByName(thisProc.ProcessName);
                    if (similarProcs.Length > 1)
                    {
                        VC_Static.Log.WriteInfo(similarProcs.Select(p => $"ID: {p.Id}").ToArray());
                        VC_Static.Log.WriteInfo(new string[] { "Found at least one other running Volume Control instance!" });
                        ;
                        VC_Static.Log.WriteInfo();
                        foreach (Process otherInst in similarProcs)
                        {
                            if (thisProc.Id != otherInst.Id) // process has the same name but different IDs, this is (likely) another instance.
                            {
                                VC_Static.Log.WriteFatal($"Found another instance of Volume Control with PID '{otherInst.Id}'!");
#                               if DEBUG // DEBUGGING MODE:
                                otherInst.Kill(true); // kill the other instance and start debugging anyway.
                                VC_Static.Log.WriteDebug($"Killed another instance of Volume Control with PID '{otherInst.Id}' because we're running in DEBUG mode.");
#                               else // RELEASE:
                                throw new Exception("An instance of Volume Control is already running!"); // In release mode, throw an exception and exit.
#                               endif
                            }
                        }
                    }
                }

                var form = new Form(); // create the main form

                VC_Static.InitializeHotkeys(form); // Initialize hotkeys
                form.hkedit.SetDataSource(VC_Static.Hotkeys); // set data source

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