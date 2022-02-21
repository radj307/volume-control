namespace UIComposites
{
    public partial class ColorScheme
    {
        public static readonly ColorScheme LightMode = new()
        {
            Default = new(Color.Black, Color.White),
            Theme = new()
            {
                new ColorBinding<Panel>(Color.Transparent, Color.WhiteSmoke),
                new ColorBinding<GroupBox>(Color.Black, Color.WhiteSmoke),
                new ColorBinding<TabControl>(Color.Black, Color.WhiteSmoke),
                new ColorBinding<Label>(Color.Black, Color.WhiteSmoke),
                new ColorBinding<CheckBox>(Color.Black, Color.WhiteSmoke),
                new ColorBinding<TabControl>(Color.Black, Color.WhiteSmoke),
                new ColorBinding<ComboBox>(Color.Black, Color.WhiteSmoke),
                new ColorBinding<NumericUpDown>(Color.Black, Color.WhiteSmoke)
            }
        };
        public static readonly ColorScheme DarkMode = new()
        {
            Default = new(Color.White, Color.DarkGray),
            Theme = new()
            {
                new ColorBinding<Panel>(Color.Transparent, Color.FromArgb(60, 60, 60)),
                new ColorBinding<GroupBox>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60)),
                new ColorBinding<TabControl>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60)),
                new ColorBinding<Label>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60)),
                new ColorBinding<CheckBox>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60)),
                new ColorBinding<TabControl>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60)),
                new ColorBinding<ComboBox>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60)),
                new ColorBinding<NumericUpDown>(Color.WhiteSmoke, Color.FromArgb(60, 60, 60))
            }
        };
    }
}
