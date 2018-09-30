namespace ToastifyAPI.Core
{
    public interface ISpotifyWebAPI
    {
        object GetCurrentTrack();
        object GetUserPrivateProfile();
    }
}