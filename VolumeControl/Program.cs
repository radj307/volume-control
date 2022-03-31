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
            // TODO: Disallow multiple instances

            ApplicationConfiguration.Initialize();

            VC_Static.Initialize(); // Initialize global statics

            var form = new Form(); // create a form

            VC_Static.InitializeHotkeys(form); // Initialize hotkeys

            form.hkedit.SetDataSource(VC_Static.Hotkeys);

            Application.Run(form);
        }
    }
}