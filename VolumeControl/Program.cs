// sample program
using VolumeControl;

class Program
{
    static void Main(string[] args)
    {
        const string name = "Deezer";
        for (float i = 100f; i >= 0f; --i)
        {
            VolumeHelper.SetVolume(name, i);
            Console.WriteLine($"{VolumeHelper.GetVolume(name)}");
            System.Threading.Thread.Sleep(5);
        }
        for (float i = 0f; i <= 100f; ++i)
        {
            VolumeHelper.SetVolume(name, i);
            Console.WriteLine($"{VolumeHelper.GetVolume(name)}");
            System.Threading.Thread.Sleep(5);
        }
    }
}
