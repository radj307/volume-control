using System.ComponentModel;
using VolumeControl.API;
using VolumeControl.Audio;
using VolumeControl.Hotkeys.Attributes;

namespace VolumeControl.Hotkeys.Addons
{
    /// <summary>
    /// Contains hotkey action handlers that interact with AudioSessions in the AudioAPI object.
    /// </summary>
    [ActionAddon(nameof(AudioSessionActions))]
    public sealed class AudioSessionActions
    {
        private static AudioAPI AudioAPI => VCAPI.Default.AudioAPI;
        private const string GroupColor = "#99FF99";
        private const string GroupName = "Session";

        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Increases the volume of the selected session by the value of VolumeStep.")]
        public void VolumeUp(object? sender, HandledEventArgs e) => AudioAPI.IncrementSessionVolume();
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Decreases the volume of the selected session by the value of VolumeStep.")]
        public void VolumeDown(object? sender, HandledEventArgs e) => AudioAPI.DecrementSessionVolume();
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Mutes the selected session.")]
        public void Mute(object? sender, HandledEventArgs e) => AudioAPI.SetSessionMute(true);
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Unmutes the selected session.")]
        public void Unmute(object? sender, HandledEventArgs e) => AudioAPI.SetSessionMute(false);
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Toggles the selected session's mute state.")]
        public void ToggleMute(object? sender, HandledEventArgs e) => AudioAPI.ToggleSessionMute();
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Selects the next session in the list.")]
        public void SelectNext(object? sender, HandledEventArgs e) => AudioAPI.SelectNextSession();
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Selects the previous session in the list.")]
        public void SelectPrevious(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousSession();
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Locks the selected session, preventing it from being changed.")]
        public void Lock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = true;
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Unlocks the selected session, allowing it to be changed.")]
        public void Unlock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = false;
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Toggles whether the selected session can be changed or not.")]
        public void ToggleLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = !AudioAPI.LockSelectedSession;
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Changes the selected session to null.")]
        public void Deselect(object? sender, HandledEventArgs e) => AudioAPI.DeselectSession();
    }
}
