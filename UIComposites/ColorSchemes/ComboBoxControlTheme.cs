namespace UIComposites.ColorSchemes
{
    public class ComboBoxControlTheme : ControlTheme, IReadOnlyControlTheme, IControlTheme
    {
        public ComboBoxControlTheme(Color? fg = null, Color? bg = null, Font? font = null, FlatStyle? flatStyle = null) : base(fg, bg, font)
        {
            _flatStyle = flatStyle;
        }

        private readonly FlatStyle? _flatStyle;

        public FlatStyle? FlatStyle { get => _flatStyle; }

        public Control PaintControl(Control ctrl)
        {
            if (ctrl is ComboBox)
            {
                var cmb = (ComboBox)ctrl;
                cmb.ForeColor = ForeColor;
                cmb.BackColor = BackColor;
                var font = Font;
                if (font != null)
                    cmb.Font = font;
                var flat = FlatStyle;
                if (flat != null)
                    cmb.FlatStyle = flat.Value;
                return cmb;
            }
            else throw new ArgumentException($"Cannot apply a ComboBox theme to a non-ComboBox control: \"{nameof(ctrl)}\"!");
        }

        public new bool AppliesToControl(Control ctrl)
                => ctrl is ComboBox;
    }
}
