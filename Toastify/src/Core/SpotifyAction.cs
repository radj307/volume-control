namespace Toastify.Core
{
    public enum SpotifyAction : long
    {
        None = 0,
        ShowToast = 1,
        ShowSpotify = 2,
        CopyTrackInfo = 3,
        SettingsSaved = 4,
        PasteTrackInfo = 5,
        ThumbsUp = 6,   // not usable, left in for future (hopefully!)
        ThumbsDown = 7, // not usable, left in for future (hopefully!)
        PlayPause = 917504,
        Mute = 524288,
        VolumeDown = 589824,
        VolumeUp = 655360,
        Stop = 851968,
        PreviousTrack = 786432,
        NextTrack = 720896,
        FastForward = 49 << 16,
        Rewind = 50 << 16,
    }
}