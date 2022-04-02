using System.Diagnostics;
using VolumeControl.Core;

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
            Form? form = null;
            try // initialize
            {
                ApplicationConfiguration.Initialize();

                VC_Static.Initialize(); // Initialize global statics

#               if DEBUG
                VC_Static.Log.WriteDebug("This binary was built in Debug configuration.");
#               else
                VC_Static.Log.WriteDebug("This binary was built in Release configuration.");
#               endif

                if (!Properties.Settings.Default.AllowMultipleInstances)
                {
                    Process thisProc = Process.GetCurrentProcess(); // check for other processes named the same thing.
                    VC_Static.Log.WriteDebug($"This Volume Control instance '{thisProc.ProcessName}' has PID '{thisProc.Id}'");
                    foreach (Process otherInst in Process.GetProcessesByName(thisProc.ProcessName))
                    {
                        if (thisProc.Id != otherInst.Id) // process has the same name but different IDs, this is (likely) another instance.
                        {
                            VC_Static.Log.WriteFatal($"Found another instance of Volume Control with PID '{otherInst.Id}'!");
#                           if DEBUG // DEBUGGING MODE:
                            otherInst.Kill(true); // kill the other instance and start debugging anyway.
                            VC_Static.Log.WriteDebug($"Killed another instance of Volume Control with PID '{otherInst.Id}' because we're running in DEBUG mode.");
#                           else // RELEASE:
                            throw new Exception("An instance of Volume Control is already running!"); // In release mode, throw an exception and exit.
#                           endif
                        }
                    }
                }

                form = new Form(); // create the main form

                VC_Static.InitializeHotkeys(form); // Initialize hotkeys
                form.hkedit.SetDataSource(VC_Static.Hotkeys); // set data source

                VC_Static.Log.WriteInfo("Initialization completed, starting the application...");

            }
            catch (Exception ex)
            {
                VC_Static.Log.WriteExceptionFatal(ex);
                return;
            }
            try // run application
            {
                Application.Run(form);
                VC_Static.Log.WriteInfo("Application exited normally.");
            }
            catch (Exception ex)
            {
                VC_Static.Log.WriteExceptionFatal(ex);
                return;
            }
        }
    }
}