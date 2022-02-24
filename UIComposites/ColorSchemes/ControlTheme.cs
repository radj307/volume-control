namespace UIComposites.ColorSchemes
{
    public class ControlTheme : IReadOnlyControlTheme, IControlTheme
    {
        public ControlTheme(Color? fg = null, Color? bg = null, Font? font = null)
        {
            _fore = fg ?? Color.Transparent;
            _back = bg ?? Color.Transparent;
            _font = font;
        }

        private readonly Color _fore, _back;
        private readonly Font? _font;

        public Color ForeColor { get => _fore; }
        public Color BackColor { get => _back; }
        public Font? Font { get => _font; }

        public Control PaintControl(Control ctrl)
        {
            ctrl.ForeColor = ForeColor;
            ctrl.BackColor = BackColor;
            var font = Font;
            if (font != null)
                ctrl.Font = font;
            return ctrl;
        }

        public bool AppliesToControl(Control ctrl)
            => true;
    }
    public class ControlTheme<T> : ControlTheme where T : Control
    {
        public ControlTheme(Color? fg = null, Color? bg = null, Font? font = null) : base(fg, bg, font) { }

        public new bool AppliesToControl(Control ctrl)
            => ctrl is T;
    }
}
