namespace Toastify.Core
{
    public enum SpotifyAction : long
    {
        // (Some of) these values are the lParam of a WM_APPCOMMAND message.
        // Used simulate a keyboard multimedia key press on the desktop in Spotify.SendAction(-)?
        // See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646275(v=vs.85).aspx.

        None           = 0,
        ShowToast      = 1,
        ShowSpotify    = 2,
        CopyTrackInfo  = 3,
        SettingsSaved  = 4,
        PasteTrackInfo = 5,
        ThumbsUp       = 6,           // not usable, left in for future (hopefully!)
        ThumbsDown     = 7,           // not usable, left in for future (hopefully!)
        PlayPause      = 0x000E0000L,
        Mute           = 0x00080000L,
        VolumeDown     = 0x00090000L,
        VolumeUp       = 0x000A0000L,
        PreviousTrack  = 0x000C0000L,
        NextTrack      = 0x000B0000L,
        FastForward    = 0x00310000L,
        Rewind         = 0x00320000L,
    }
}