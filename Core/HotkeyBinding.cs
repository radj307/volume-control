using Core.Enum;
using HotkeyLib;

namespace Core
{
    public class HotkeyBinding : WindowsHotkey
    {
        public HotkeyBinding(Control owner, string keystring, VolumeControlSubject subject, VolumeControlAction action) : base(owner, keystring)
        {
            Subject = subject;
            Action = action;
        }

        public VolumeControlAction Action;
        public VolumeControlSubject Subject;
    }
}
