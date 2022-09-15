using System.ComponentModel;
using VolumeControl.Hotkeys.Attributes;
using VolumeControl.Hotkeys.Enum;

namespace VolumeControl.Hotkeys.Addons
{
    [HotkeyActionGroup("Media", GroupColor = "#FFA54C")]
    public class MediaActions
    {
        #region Methods
        [HotkeyAction(Description = "Switches media playback to the next track.\nThis is for people that don't have media keys.")]
        public void NextTrack(object? sender, HandledEventArgs e) => InputSimulator.SendKey(EVirtualKeyCode.VK_MEDIA_NEXT_TRACK);
        [HotkeyAction(Description = "Switches media playback to the previous track.\nThis is for people that don't have media keys.")]
        public void PreviousTrack(object? sender, HandledEventArgs e) => InputSimulator.SendKey(EVirtualKeyCode.VK_MEDIA_PREV_TRACK);
        [HotkeyAction(Description = "Toggles media playback.\nThis is for people that don't have media keys.")]
        public void TogglePlayback(object? sender, HandledEventArgs e) => InputSimulator.SendKey(EVirtualKeyCode.VK_MEDIA_PLAY_PAUSE);
        [HotkeyAction(Description = "Stops media playback.\nThis is for people that don't have media keys.")]
        public void StopPlayback(object? sender, HandledEventArgs e) => InputSimulator.SendKey(EVirtualKeyCode.VK_MEDIA_STOP);
        #endregion Methods
    }
}
