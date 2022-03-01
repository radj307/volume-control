using Manina.Windows.Forms;

namespace UIComposites.ColorSchemes
{
    public partial class ColorScheme
    {
        public static readonly ColorScheme LightMode = new()
        {
            Default = new(Color.Black, Color.WhiteSmoke),
            BorderHighlight = new(Color.Black, Color.FromArgb(60, 60, 60)),
            Theme = new()
            {
                new PanelControlTheme(Color.Transparent, Color.Transparent, null, BorderStyle.None),
                new ComboBoxControlTheme(Color.Black, Color.WhiteSmoke, null, FlatStyle.Standard),
                new ControlTheme<GroupBox>(Color.Black, Color.WhiteSmoke),
                new ControlTheme<Label>(Color.Black, Color.WhiteSmoke),
                new ControlTheme<CheckBox>(Color.Black, Color.WhiteSmoke),
                new ControlTheme<NumericUpDown>(Color.Black, Color.WhiteSmoke),
                new ControlTheme<UserControl>(Color.Black, Color.WhiteSmoke),
                new ControlTheme<Tab>(Color.Black, Color.WhiteSmoke)
            }
        };
        public static readonly ColorScheme DarkMode = new()
        {
            Default = new(Color.WhiteSmoke, Color.FromArgb(50, 50, 50)),
            BorderHighlight = new(Color.WhiteSmoke, Color.WhiteSmoke),
            Theme = new()
            {
                new PanelControlTheme(Color.Transparent, Color.Transparent, null, BorderStyle.None),
                new ComboBoxControlTheme(Color.White, Color.FromArgb(50, 50, 50), null, FlatStyle.Standard),
                new ControlTheme<GroupBox>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60)),
                new ControlTheme<Label>(Color.White, Color.Transparent),
                new ControlTheme<CheckBox>(Color.White, Color.FromArgb(60, 60, 60)),
                new ControlTheme<NumericUpDown>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60)),
                new ControlTheme<UserControl>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60)),
                new ControlTheme<Tab>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60))
            }
        };
    }
}
