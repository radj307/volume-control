namespace ToastifyAPI.Model.Interfaces
{
    public interface ICurrentlyPlayingObject
    {
        int ProgressMs { get; }
        bool IsPlaying { get; }
        ISong Track { get; }
        string Type { get; }
    }
}