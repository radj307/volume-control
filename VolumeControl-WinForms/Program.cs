using MovablePython;

namespace VolumeControl_WinForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var form = new Form1();

            const string name = "Deezer";
            const float incr = 5f;

            Hotkey hk_up = new(Keys.VolumeUp, false, true, false, false);
            hk_up.Pressed += delegate { VolumeControl.VolumeHelper.IncrementVolume(name, incr); };

            Hotkey hk_down = new(Keys.VolumeDown, false, true, false, false);
            hk_down.Pressed += delegate { VolumeControl.VolumeHelper.DecrementVolume(name, incr); };

            hk_up.Register(form);
            hk_down.Register(form);

            Application.Run(form);
        }
    }
}