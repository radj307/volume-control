using System.Windows.Forms;

namespace HotkeyLib
{
    /// <summary>
    /// Static object that maintains the list of keys that aren't considered valid.<br/>
    /// This includes modifier keys and certain utility keys.
    /// </summary>
    /// <remarks>Call <see cref="GetWhitelistedKeys"/> to get the list of valid keys.</remarks>
    public static class KeysBlacklist
    {
        public static readonly List<Keys> Blacklist = new()
        {
            Keys.Alt,

            Keys.Shift,
            Keys.ShiftKey,
            Keys.LShiftKey,
            Keys.RShiftKey,

            Keys.Control,
            Keys.ControlKey,
            Keys.LControlKey,
            Keys.RControlKey,

            Keys.LWin,
            Keys.RWin,

            Keys.Modifiers,
            Keys.NoName,
        };

        public static List<Keys> GetWhitelistedKeys()
        {
            List<Keys> l = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList();
            l.RemoveAll(key => Blacklist.Contains(key));
            return l;
        }
    }
}
