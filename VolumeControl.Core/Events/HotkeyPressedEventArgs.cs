using VolumeControl.Core.Enum;

namespace VolumeControl.Core.Events
{
    public class HotkeyPressedEventArgs : EventArgs
    {
        public HotkeyPressedEventArgs(VolumeControlSubject subject = VolumeControlSubject.NULL, VolumeControlAction action = VolumeControlAction.NULL, object? data = null)
        {
            Subject = subject;
            Action = action;
            Data = data;
        }

        public readonly VolumeControlSubject Subject;
        public readonly VolumeControlAction Action;
        public readonly object? Data;
    }
    public delegate void HotkeyPressedEventHandler(object? sender, HotkeyPressedEventArgs e);
}
