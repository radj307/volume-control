using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComposites
{
    public interface IColorBinding
    {
        public Color Foreground { get; set; }
        public Color Background { get; set; }

        /// <summary>
        /// Predicate function that determines whether or not this binding may apply to the given control type.
        /// </summary>
        /// <param name="type">Control type to test.</param>
        /// <returns>bool</returns>
        public bool AppliesToControl(Control type);
        /// <summary>
        /// Apply this color binding to the given control reference.
        /// Does NOT recurse.
        /// </summary>
        /// <param name="ctrl">Control Ref</param>
        public Control ApplyTo(Control ctrl)
        {
            ctrl.ForeColor = Foreground;
            ctrl.BackColor = Background;
            return ctrl;
        }
    }

    public struct DefaultColorBinding : IColorBinding
    {
        public DefaultColorBinding(Color fg, Color bg)
        {
            _foreground = fg;
            _background = bg;
        }

        private Color _foreground, _background;

        Color IColorBinding.Foreground { get => _foreground; set => _foreground = value; }
        Color IColorBinding.Background { get => _background; set => _background = value; }

        bool IColorBinding.AppliesToControl(Control type) => true;
    }

    public struct ColorBinding<T> : IColorBinding where T : Control
    {
        public ColorBinding(Color fg, Color bg)
        {
            _foreground = fg;
            _background = bg;
        }

        private Color _foreground, _background;

        Color IColorBinding.Foreground { get => _foreground; set => _foreground = value; }
        Color IColorBinding.Background { get => _background; set => _background = value; }

        bool IColorBinding.AppliesToControl(Control type) => type is T;
    }


    public class ColorScheme
    {
        public ColorScheme(DefaultColorBinding defaultColors, List<IColorBinding> colors)
        {
            Theme = colors;
            Default = defaultColors;
        }
        public ColorScheme()
        {
            Theme = new();
        }

        public List<IColorBinding> Theme;
        public DefaultColorBinding Default;

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

        public static readonly ColorScheme LightMode = new()
        {
            Default = new(Color.Black, Color.White),
            Theme = new()
            {
                new ColorBinding<Panel>(Color.Transparent, Color.Transparent),
                new ColorBinding<GroupBox>(Color.Transparent, Color.White),
                new ColorBinding<Button>(Color.Black, Color.Transparent)
            }
        };
        public static readonly ColorScheme DarkMode = new()
        {
            Default = new(Color.White, Color.DarkGray),
            Theme = new()
            {
                new ColorBinding<Panel>(Color.Transparent, Color.Transparent),
                new ColorBinding<GroupBox>(Color.Transparent, Color.SlateGray),
                new ColorBinding<Button>(Color.White, Color.DarkSlateGray)
            }
        };
    }
}
