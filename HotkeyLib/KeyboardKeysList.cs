using System.ComponentModel;
using System.Windows.Forms;

namespace HotkeyLib
{
    public static class KeyboardKeysList
    {
        private static BindingList<Keys> ToBindingList(List<Keys> list)
        {
            BindingList<Keys> bList = new();
            foreach (Keys key in list)
                bList.Add(key);
            return bList;
        }
        public static readonly BindingList<Keys> KeysList = ToBindingList(Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList());
    }
}
