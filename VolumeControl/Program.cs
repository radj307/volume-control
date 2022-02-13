namespace VolumeControl
{
    internal static class Program
    {
        // Entry point
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new VolumeControlForm());
        }
    }
}