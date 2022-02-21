using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manina.Windows.Forms;

namespace UIComposites
{
    public partial class ColorScheme
    {
        public static readonly ColorScheme LightMode = new()
        {
            Default = new(Color.Black, Color.WhiteSmoke),
            Theme = new()
            {
                new ColorBinding<Panel>(Color.Transparent, Color.WhiteSmoke),
                new ColorBinding<GroupBox>(Color.Black, Color.WhiteSmoke),
                new ColorBinding<Label>(Color.Black, Color.WhiteSmoke),
                new ColorBinding<CheckBox>(Color.Black, Color.WhiteSmoke),
                new ColorBinding<ComboBox>(Color.Black, Color.WhiteSmoke),
                new ColorBinding<NumericUpDown>(Color.Black, Color.WhiteSmoke),
                new ColorBinding<UserControl>(Color.Black, Color.WhiteSmoke),
                new ColorBinding<Tab>(Color.Black, Color.WhiteSmoke)
            }
        };
        public static readonly ColorScheme DarkMode = new()
        {
            Default = new(Color.WhiteSmoke, Color.FromArgb(75, 75, 75)),
            Theme = new()
            {
                new ColorBinding<Panel>(Color.Transparent, Color.FromArgb(60, 60, 60)),
                new ColorBinding<GroupBox>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60)),
                new ColorBinding<Label>(Color.WhiteSmoke, Color.Transparent),
                new ColorBinding<CheckBox>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60)),
                new ColorBinding<ComboBox>(Color.WhiteSmoke, Color.FromArgb(75, 75, 75)),
                new ColorBinding<NumericUpDown>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60)),
                new ColorBinding<UserControl>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60)),
                new ColorBinding<Tab>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60))
            }
        };
    }
}
