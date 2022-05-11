namespace VolumeControl.Core.HelperTypes
{
    public class HotkeyActionBindings
    {
        public HotkeyActionBindings(AudioAPI audioAPI) => Bindings = new()
        {
            { EHotkeyAction.None, null },
            { // VOLUME UP
                EHotkeyAction.VolumeUp,
                audioAPI.IncreaseVolume
            },
            { // VOLUME DOWN
                EHotkeyAction.VolumeDown,
                audioAPI.DecreaseVolume
            },
            { // TOGGLE MUTE
                EHotkeyAction.ToggleMute,
                audioAPI.ToggleMute
            },
            { // NEXT TRACK
                EHotkeyAction.NextTrack,
                audioAPI.NextTrack
            },
            { // PREVIOUS TRACK
                EHotkeyAction.PreviousTrack,
                audioAPI.PreviousTrack
            },
            { // TOGGLE PLAYBACK
                EHotkeyAction.TogglePlayback,
                audioAPI.TogglePlayback
            },
            {
                EHotkeyAction.NextTarget,
                audioAPI.NextTarget
            },
            {
                EHotkeyAction.PreviousTarget,
                audioAPI.PreviousTarget
            },
            {
                EHotkeyAction.ToggleTargetLock,
                audioAPI.ToggleTargetLock
            },
            {
                EHotkeyAction.NextDevice,
                audioAPI.NextDevice
            },
            {
                EHotkeyAction.PreviousDevice,
                audioAPI.PreviousDevice
            }
        };

        public Dictionary<EHotkeyAction, HotkeyLib.KeyEventHandler?> Bindings { get; set; }

        public HotkeyLib.KeyEventHandler? this[EHotkeyAction action]
        {
            get => Bindings[action];
            set => Bindings[action] = value;
        }
    }
}
