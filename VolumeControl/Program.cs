// sample program
using System.Diagnostics;
using System.Runtime.InteropServices;
using VolumeControl;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"{VolumeHelper.GetVolume("Deezer")}");
        VolumeHelper.SetVolume("Deezer", 80);
    }
}
