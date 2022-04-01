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
            if (!Properties.Settings.Default.AllowMultipleInstances)
            {
                Process thisProc = Process.GetCurrentProcess();
                foreach (Process otherInst in Process.GetProcessesByName(thisProc.ProcessName))
                {
                    if (thisProc.Id != otherInst.Id)
                    {
#                   if DEBUG
                        otherInst.Kill(true);
#                   else // RELEASE:
                        throw new Exception("An instance of Volume Control is already running!");
#                   endif
                    }
                }
            }

            ApplicationConfiguration.Initialize();

            VC_Static.Initialize(); // Initialize global statics

            var form = new Form(); // create a form

            VC_Static.InitializeHotkeys(form); // Initialize hotkeys
            form.hkedit.SetDataSource(VC_Static.Hotkeys); // set data source

            Application.Run(form);
        }
    }
}