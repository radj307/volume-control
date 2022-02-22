using System.Text.Json;

namespace UIComposites
{
    public partial class ColorScheme : IColorSchemeFile
    {
        public ColorScheme(ColorBinding<Control> defaultColors, List<IColorBinding> colors)
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

        public ColorBinding<Control> Default;
        public ColorBinding<Control> BorderHighlight;
        public List<IColorBinding> Theme;

        public IColorBinding Primary { get => this.Default; set => this.Default = (ColorBinding<Control>)value; }
        public IColorBinding Secondary { get => this.BorderHighlight; set => this.BorderHighlight = (ColorBinding<Control>)value; }
        public List<IColorBinding> Components { get => this.Theme; set => this.Theme = value; }

        private IColorBinding GetApplicableColor(Control type)
        {
            foreach (IColorBinding bind in Theme)
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
                GetApplicableColor(ctrl).ApplyTo(ctrl);
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
                    new InterfaceConverterFactory(typeof(ColorBinding<Control>), typeof(IColorBinding)),
                                 new IListInterfaceConverterFactory(typeof(IColorBinding))
                }
            });
        }
        public void SaveToFile(string path) => JSON.Write<ColorScheme>(path, this);
        public static void SaveToFile(ColorScheme scheme, string path) => JSON.Write<ColorScheme>(path, scheme);
    }
}
