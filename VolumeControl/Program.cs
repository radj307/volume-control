using System.Diagnostics;

namespace VolumeControl
{
    internal static class Program
    {
        //private static ColorSchemeList colors;
        // Entry point
        [STAThread]
        private static void Main()
        {
            if (!Properties.Settings.Default.AllowMultipleInstances)
            {
                Process thisProc = Process.GetCurrentProcess();
                foreach (Process otherInst in Process.GetProcessesByName(thisProc.ProcessName))
                    if (thisProc.Id != otherInst.Id)
                        throw new Exception("An instance of Volume Control is already running!");

            }
            //string themesDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\VolumeControl\themes";

            //ColorScheme.DarkMode.SaveToFile(themesDir + @"\DarkMode.json");
            //ColorScheme.LightMode.SaveToFile(themesDir + @"\LightMode.json");

            //colors = new(themesDir);

            // init & run application:
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApplicationConfiguration.Initialize();

            var form = new VolumeControlForm();

            Application.Run(form);
        }
    }
}