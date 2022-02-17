namespace VolumeControl
{
    internal static class Program
    {
        // Entry point
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new VolumeControlForm());
        }
    }
}