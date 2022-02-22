using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace VolumeControl
{
    internal static class Program
    {
        // Entry point
        [STAThread]
        private static void Main()
        {
            Process thisProc = Process.GetCurrentProcess();
            foreach (Process otherInst in Process.GetProcessesByName(thisProc.ProcessName))
                if (thisProc.Id != otherInst.Id)
                    throw new Exception("An instance of Volume Control is already running!");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApplicationConfiguration.Initialize();

            var form = new VolumeControlForm();

            Application.Run(form);
        }
    }
}