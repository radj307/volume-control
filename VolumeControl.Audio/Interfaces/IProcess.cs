namespace VolumeControl.Audio.Interfaces
{
    /// <summary>
    /// Represents a process running on the local system.
    /// </summary>
    public interface IProcess
    {
        string ProcessName { get; }
        long PID { get; }
        string ProcessIdentifier { get; }
    }
}
