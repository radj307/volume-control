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
        /// <summary>The list of <see cref="Keys"/> that should never be allowed to be used for hotkeys.<br/>This list is comprised of keys that don't actually function at all, so don't go fucking with this or you may break something.</summary>
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
            Keys.Cancel,
            Keys.LineFeed,
            Keys.Clear,
            Keys.Sleep,
            Keys.Apps,
            Keys.Help,
            Keys.Execute,
            Keys.Select,
            Keys.SelectMedia,
            Keys.LMenu,
            Keys.RMenu,
            Keys.ProcessKey,
            Keys.Packet,
            Keys.Attn,
            Keys.Crsel,
            Keys.Exsel,
            Keys.EraseEof,
            Keys.Pa1,
            Keys.KeyCode,
            Keys.LButton,
            Keys.RButton,
            Keys.MButton,
            Keys.XButton1,
            Keys.XButton2,
            Keys.Menu,
            Keys.Play,
            Keys.Zoom,
            Keys.LaunchApplication1,
            Keys.LaunchApplication2,
            Keys.LaunchMail,
            Keys.Scroll,
            Keys.FinalMode,
            Keys.HanguelMode,
            Keys.HangulMode,
            Keys.HanjaMode,
            Keys.IMEModeChange,
            Keys.JunjaMode,
            Keys.KanaMode,
            Keys.KanjiMode,
            Keys.IMEAccept,
            Keys.IMEAceept,
            Keys.IMEConvert,
            Keys.IMENonconvert,
            Keys.Oem1,
            Keys.Oem102,
            Keys.Oem2,
            Keys.Oem3,
            Keys.OemPeriod,
            Keys.Oem6,
            Keys.Oem7,
            Keys.Oem6,
            Keys.Oem4,
            Keys.Oem5,
            Keys.Oem8,
            Keys.OemBackslash,
            Keys.OemClear,
            Keys.OemCloseBrackets,
            Keys.Oemcomma,
            Keys.OemMinus,
            Keys.OemOpenBrackets,
            Keys.OemPeriod,
            Keys.OemPipe,
            Keys.Oemplus,
            Keys.Oemplus,
            Keys.OemQuestion,
            Keys.OemQuotes,
            Keys.OemSemicolon,
            Keys.Oemtilde,
            Keys.Print,
            Keys.BrowserStop,
            Keys.BrowserSearch,
            Keys.BrowserRefresh,
            Keys.BrowserBack,
            Keys.BrowserFavorites,
            Keys.BrowserForward,
            Keys.BrowserHome,
            Keys.Back,
            Keys.Capital,
            Keys.NumLock,
            Keys.CapsLock,
            Keys.Separator,
        };

        /// <summary>
        /// Gets the list of keys that aren't present on the blacklist.
        /// </summary>
        /// <returns>List of <see cref="Keys"/> that are <b>not blacklisted.</b><br/>That is; the list of <b>valid</b> keys.</returns>
        public static List<Keys> GetWhitelistedKeys()
        {
            List<Keys> l = new();
            foreach (Keys key in Enum.GetValues(typeof(Keys)).Cast<Keys>())
                if (!l.Contains(key) && !Blacklist.Contains(key))
                    l.Add(key);
            return l;
        }
    }
}
