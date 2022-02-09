using System.Windows.Forms;

namespace HotkeyLib
{
    public class KeyList
    {
        public KeyList()
        {
            list = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList();
        }

        private readonly List<Keys> list;

        public List<Keys> List
        {
            get { return list; }
        }
    }
}
