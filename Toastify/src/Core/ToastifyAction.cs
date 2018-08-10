using Toastify.Common;

namespace Toastify.Core
{
    // TODO: Apply the 'Polymorphism' pattern.
    //       When related alternatives or behaviours vary by type, assign responsibilities for the
    //       behaviour, using polymorphic operations, to the types for which the behaviour varies.
    public enum ToastifyAction : long
    {
        // (Some of) these values are the lParam of a WM_APPCOMMAND message.
        // Used simulate a keyboard multimedia key press on the desktop in Spotify.SendAction(-)?
        // See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646275(v=vs.85).aspx.

        [EnumReadableName("None")]
        None = 0,

        [EnumReadableName("Show Toast")]
        ShowToast = 1,

        [EnumReadableName("Show / Hide Spotify")]
        ShowSpotify = 2,

        [EnumReadableName("Copy Track Name")]
        CopyTrackInfo = 3,

        [EnumReadableName("Settings Saved")]
        SettingsSaved = 4,

        [EnumReadableName("Paste Track Name")]
        PasteTrackInfo = 5,

        [EnumReadableName("Thumbs Up")]
        ThumbsUp = 6,

        [EnumReadableName("Thumbs Down")]
        ThumbsDown = 7,

        [EnumReadableName("Stop")]
        Stop = 0x000D0000L,

        [EnumReadableName("Play / Pause")]
        PlayPause = 0x000E0000L,

        [EnumReadableName("Mute")]
        Mute = 0x00080000L,

        [EnumReadableName("Volume Down")]
        VolumeDown = 0x00090000L,

        [EnumReadableName("Volume Up")]
        VolumeUp = 0x000A0000L,

        [EnumReadableName("Previous Track")]
        PreviousTrack = 0x000C0000L,

        [EnumReadableName("Next Track")]
        NextTrack = 0x000B0000L,

        [EnumReadableName("Fast Forward")]
        FastForward = 0x00310000L,

        [EnumReadableName("Rewind")]
        Rewind = 0x00320000L,

        [EnumReadableName("Exit")]
        Exit = 0xFFFFFFFFL,

#if DEBUG

        [EnumReadableName("Show DebugView")]
        ShowDebugView = -8,

#endif
    }
}