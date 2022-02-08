using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotkeyLib
{
    public class KeyList
    {
        public static readonly List<Keys> list = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList();
    }
}
