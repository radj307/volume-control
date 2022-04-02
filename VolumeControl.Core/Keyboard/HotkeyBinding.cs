using HotkeyLib;
using VolumeControl.Core.Enum;

namespace VolumeControl.Core.Keyboard
{
    public class HotkeyBinding : WindowsHotkey
    {
        public HotkeyBinding(Control owner, string name, string keystringProperty, VolumeControlSubject subject, VolumeControlAction action, string registerProperty)
            : base(owner, (string)Properties.Settings.Default[keystringProperty])
        {
            _keystring_property = keystringProperty;
            _enabled_property = registerProperty;

            _name = name;
            Subject = subject;
            Action = action;
            Registered = Convert.ToBoolean(Properties.Settings.Default[registerProperty]);
        }

        public void Save()
        {
            string keystring = ToString()!;
            bool registered = Registered;
            Properties.Settings.Default[_keystring_property] = keystring;
            Properties.Settings.Default[_enabled_property] = registered;
            VC_Static.Log.WriteInfo($"Saved hotkey \'{Name}\':  ({keystring}, {registered})");
        }

        private readonly string _name;
        private readonly string _keystring_property, _enabled_property;
        public VolumeControlAction Action;
        public VolumeControlSubject Subject;

        public string Name
        {
            get => _name;
        }
    }
}
