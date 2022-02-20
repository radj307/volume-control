using System.Windows.Forms;

namespace HotkeyLib
{
    public static class ValidKeysList
    {
        public static readonly KeyList KeysList = new(delegate (Keys key)
        {
            return !KeyFilters.ExcludeKeys.Any(k => k.Equals(key));
        });
    }
}