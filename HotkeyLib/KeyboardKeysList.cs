using System.ComponentModel;
using System.Windows.Forms;

namespace HotkeyLib
{
    public static class KeyboardKeysList
    {
        /// <summary>
        /// Default key blacklist
        /// </summary>
        private static readonly List<Keys> DefaultKeyBlacklist = new()
        {
            Keys.KanaMode,
            Keys.KanjiMode,
            Keys.JunjaMode,
            Keys.HanjaMode,
            Keys.HangulMode,
            Keys.HanguelMode,
            Keys.FinalMode,
            Keys.Modifiers,
            Keys.Sleep,
            Keys.Control,
            Keys.Shift,
            Keys.Alt,
            Keys.LWin,
            Keys.RWin,
            Keys.ControlKey,
            Keys.ShiftKey,
            Keys.Capital,
            Keys.D0,
            Keys.D1,
            Keys.D2,
            Keys.D3,
            Keys.D4,
            Keys.D5,
            Keys.D6,
            Keys.D7,
            Keys.D8,
            Keys.D9,
            Keys.NumLock,
            Keys.Scroll,
            Keys.Exsel,
            Keys.Execute,
            Keys.Escape,
            Keys.Crsel,
            Keys.Help,
            Keys.NoName,
            Keys.ProcessKey,
            Keys.RShiftKey,
            Keys.RControlKey,
            Keys.EraseEof,
            Keys.Attn,
            Keys.LineFeed,
            Keys.LControlKey,
            Keys.LShiftKey,
            Keys.LWin,
            Keys.KeyCode,
            Keys.Pa1,
            Keys.Packet,
            Keys.PrintScreen,
            Keys.Print,
            Keys.Separator,
            Keys.Apps,
            Keys.Clear,
            Keys.LaunchMail,
            Keys.LaunchApplication1,
            Keys.LaunchApplication2,
        };
        private static readonly List<string> DefaultStringBlacklist = new()
        { // Case sensitive:
            "Oem",
            "IME",
            "Browser",
        };
        /// <summary>
        /// Create a list of keys from a string list.
        /// </summary>
        /// <param name="strlist">A list of key names, separated with a delimiter char.</param>
        /// <param name="delim">A delimiter char to split the string list.</param>
        /// <returns>List</returns>
        public static List<Keys> MakeBlacklistFrom(string strlist, char delim = ';')
        {
            string[] list = strlist.Split(delim);
            List<Keys> keylist = new();
            foreach (string str in list)
            {
                try
                {
                    keylist.Add((Keys)Enum.Parse(typeof(Keys), str));
                }
                catch { }
            }
            return keylist;
        }
        /// <summary>
        /// Creates a BindingList from a list of keys and an optional key blacklist.
        /// </summary>
        /// <param name="list">List of keys.</param>
        /// <param name="blacklist">List of keys that shouldn't be allowed.</param>
        /// <returns>BindingList</returns>
        private static BindingList<Keys> ToBindingList(List<Keys> list, List<Keys>? blacklist = null, List<string>? str_blacklist = null)
        {
            BindingList<Keys> bList = new();
            foreach (Keys key in list)
            {
                // prevent duplicates that exist for some reason (thanks C# Enum), & don't allow keys that exist on any blacklist.
                if (!bList.Contains(key) && ( ( blacklist == null || !blacklist.Contains(key) ) && ( str_blacklist == null || !str_blacklist.Any(s => key.ToString().Contains(s, StringComparison.Ordinal)) ) ))
                    bList.Add(key);
            }
            return bList;
        }
        /// <summary>
        /// A list of valid keys.
        /// </summary>
        public static readonly BindingList<Keys> KeysList = ToBindingList(Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList(), DefaultKeyBlacklist, DefaultStringBlacklist);
    }
}
