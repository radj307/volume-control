using System.Windows.Forms;

namespace HotkeyLib
{
    public class KeyList : FilteredEnumList<Keys>
    {
        public KeyList() : base(Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList()) { }
        public KeyList(List<Keys> l) : base(l) { }
        public KeyList(Func<Keys, bool> predicate) : base(Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList(), predicate) { }
    }
}