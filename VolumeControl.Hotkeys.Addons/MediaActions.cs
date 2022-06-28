using System.ComponentModel;
using VolumeControl.Hotkeys.Attributes;
using VolumeControl.Hotkeys.Enum;

namespace VolumeControl.Hotkeys.Addons
{
    [ActionAddon(nameof(MediaActions))]
    public class MediaActions
    {
        #region Fields
        private const string GroupColor = "#FFA54C";
        private const string GroupName = "Media";
        #endregion Fields

        #region Methods
        [HotkeyAction(GroupColor = GroupColor, GroupName = GroupName, Description = "Switches media playback to the next track.\nThis is for people that don't have media keys.")]
        public void NextTrack(object? sender, HandledEventArgs e) => User32.KeyboardEvent(EVirtualKeyCode.VK_MEDIA_NEXT_TRACK);
        [HotkeyAction(GroupColor = GroupColor, GroupName = GroupName, Description = "Switches media playback to the previous track.\nThis is for people that don't have media keys.")]
        public void PreviousTrack(object? sender, HandledEventArgs e) => User32.KeyboardEvent(EVirtualKeyCode.VK_MEDIA_PREV_TRACK);
        [HotkeyAction(GroupColor = GroupColor, GroupName = GroupName, Description = "Toggles media playback.\nThis is for people that don't have media keys.")]
        public void TogglePlayback(object? sender, HandledEventArgs e) => User32.KeyboardEvent(EVirtualKeyCode.VK_MEDIA_PLAY_PAUSE);
        [HotkeyAction(GroupColor = GroupColor, GroupName = GroupName, Description = "Stops media playback.\nThis is for people that don't have media keys.")]
        public void StopPlayback(object? sender, HandledEventArgs e) => User32.KeyboardEvent(EVirtualKeyCode.VK_MEDIA_STOP);
        #endregion Methods
    }
}
