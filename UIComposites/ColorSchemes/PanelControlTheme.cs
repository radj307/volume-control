namespace UIComposites.ColorSchemes
{
    public class PanelControlTheme : ControlTheme, IReadOnlyControlTheme, IControlTheme
    {
        public PanelControlTheme(Color? fg = null, Color? bg = null, Font? font = null, BorderStyle? borderStyle = null) : base(fg, bg, font)
        {
            _borderStyle = borderStyle;
        }

        private readonly BorderStyle? _borderStyle;

        public BorderStyle? BorderStyle
        {
            get => _borderStyle;
        }

        public new Control PaintControl(Control ctrl)
        {
            if (ctrl is Panel)
            {
                var panel = (Panel)ctrl;
                panel.ForeColor = ForeColor;
                panel.BackColor = BackColor;
                var font = Font;
                if (font != null)
                    panel.Font = font;
                var border = BorderStyle;
                if (border != null)
                    panel.BorderStyle = border.Value;
                return panel;
            }
            else throw new ArgumentException($"Cannot apply a Panel theme to a non-Panel control: \"{nameof(ctrl)}\"!");
        }

        public new bool AppliesToControl(Control ctrl)
            => ctrl is Panel;
    }
}
