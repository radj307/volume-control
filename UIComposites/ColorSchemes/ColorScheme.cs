using System.Text.Json;

namespace UIComposites.ColorSchemes
{
    public partial class ColorScheme : IColorSchemeFile
    {
        public ColorScheme(ControlTheme<Control> defaultColors, List<IControlTheme> colors)
        {
            Theme = colors;
            Default = defaultColors;
            BorderHighlight = new();
        }
        public ColorScheme()
        {
            Theme = new();
            Default = new();
            BorderHighlight = new();
        }

        public ControlTheme Default;
        public ControlTheme BorderHighlight;
        public List<IControlTheme> Theme;

        public IControlTheme Primary { get => Default; set => Default = (ControlTheme<Control>)value; }
        public IControlTheme Secondary { get => BorderHighlight; set => BorderHighlight = (ControlTheme<Control>)value; }
        public List<IControlTheme> Components { get => Theme; set => Theme = value; }

        private IControlTheme GetApplicableColor(Control type)
        {
            foreach (IControlTheme bind in Theme)
            {
                if (bind.AppliesToControl(type))
                {
                    return bind;
                }
            }
            return Default;
        }

        public void ApplyTo(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                GetApplicableColor(ctrl).PaintControl(ctrl);
                // recurse:
                ApplyTo(ctrl.Controls);
            }
        }

        public static ColorScheme? LoadFromFile(string path)
        {
            return JSON.Read<ColorScheme>(path, new JsonSerializerOptions()
            {
                Converters =
                {
                    new InterfaceConverterFactory(typeof(ControlTheme<Control>), typeof(IControlTheme)),
                                 new IListInterfaceConverterFactory(typeof(IControlTheme))
                }
            });
        }
        public void SaveToFile(string path) => JSON.Write<ColorScheme>(path, this);
        public static void SaveToFile(ColorScheme scheme, string path) => JSON.Write<ColorScheme>(path, scheme);
    }
}
