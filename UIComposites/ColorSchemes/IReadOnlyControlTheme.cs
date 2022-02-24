namespace UIComposites.ColorSchemes
{
    public interface IReadOnlyControlTheme
    {
        // Colors
        Color ForeColor { get; }
        Color BackColor { get; }
        // Appearance
        Font? Font { get; }
        Control PaintControl(Control ctrl);
        bool AppliesToControl(Control ctrl);
    }
}
