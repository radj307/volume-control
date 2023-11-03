using System.ComponentModel;
using VolumeControl.Core;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Enum;

namespace VolumeControl.HotkeyActions
{
    [HotkeyActionGroup("Media", GroupColor = "#FFA54C")]
    public sealed class MediaActions
    {
        #region Action Methods
        [HotkeyAction(Description = "Switches media playback to the next track.")]
        public void NextTrack(object? sender, HandledEventArgs e) => InputSimulator.SendKey(EVirtualKeyCode.VK_MEDIA_NEXT_TRACK);
        [HotkeyAction(Description = "Switches media playback to the previous track.")]
        public void PreviousTrack(object? sender, HandledEventArgs e) => InputSimulator.SendKey(EVirtualKeyCode.VK_MEDIA_PREV_TRACK);
        [HotkeyAction(Description = "Toggles media playback.")]
        public void TogglePlayback(object? sender, HandledEventArgs e) => InputSimulator.SendKey(EVirtualKeyCode.VK_MEDIA_PLAY_PAUSE);
        [HotkeyAction(Description = "Stops media playback.")]
        public void StopPlayback(object? sender, HandledEventArgs e) => InputSimulator.SendKey(EVirtualKeyCode.VK_MEDIA_STOP);
        #endregion Action Methods
    }
}
