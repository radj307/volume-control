namespace VolumeControl
{
    internal static class Program
    {
        // Entry point
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new VolumeControlForm());
        }
    }
}