using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace UIComposites
{
    public interface IColorBinding
    {
        public Color Foreground { get; set; }
        public Color Background { get; set; }
        public bool EnableForeground { get; set; }
        public bool EnableBackground { get; set; }

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

    public class ColorBindingBase : IColorBinding
    {
        protected ColorBindingBase((Color, bool) fg, (Color, bool) bg)
        {
            _fg = fg.Item1;
            _fgEnabled = fg.Item2;
            _bg = bg.Item1;
            _bgEnabled = bg.Item2;
        }
        protected ColorBindingBase(Color? fg = null, Color? bg = null)
        {
            _fg = fg ?? Color.Transparent;
            _fgEnabled = fg != null;
            _bg = bg ?? Color.Transparent;
            _bgEnabled = bg != null;
        }

        protected Color _fg, _bg;
        protected bool _fgEnabled, _bgEnabled;

        Color IColorBinding.Foreground { get => _fg; set => _fg = value; }
        Color IColorBinding.Background { get => _bg; set => _bg = value; }
        bool IColorBinding.EnableForeground { get => _fgEnabled; set => _fgEnabled = value; }
        bool IColorBinding.EnableBackground { get => _bgEnabled; set => _bgEnabled = value; }

        public Color GetForeColor() => _fg;
        public bool GetForeColorEnabled() => _fgEnabled;
        public Color GetBackColor() => _bg;
        public bool GetBackColorEnabled() => _bgEnabled;

        virtual public bool AppliesToControl(Control type)
        {
            throw new NotImplementedException("Cannot instantiate ColorBindingBase object.");
        }

        public Form ApplyToSingle(Form frm)
        {
            frm.ForeColor = _fg;
            frm.BackColor = _bg;
            return frm;
        }
    }

    public class DefaultColorBinding : ColorBindingBase
    {
        public DefaultColorBinding(Color? fg = null, Color? bg = null, List<Type>? exclude = null) : base(fg, bg)
        {
            excludeControls = exclude ?? new();
        }

        private readonly List<Type> excludeControls;

        public override bool AppliesToControl(Control type)
        {
            foreach (Type t in excludeControls)
            {
                if (type.GetType() == t)
                    return false;
            }
            return true;
        }
    }

    public class ColorBinding<T> : ColorBindingBase where T : Control
    {
        public ColorBinding(Color? fg, Color? bg) : base(fg, bg) {}

        public override bool AppliesToControl(Control type)
            => type is T;
    }


    public partial class ColorScheme
    {
        public ColorScheme(DefaultColorBinding defaultColors, List<IColorBinding> colors)
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

        public List<IColorBinding> Theme;
        public DefaultColorBinding Default;
        public DefaultColorBinding BorderHighlight;

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
    }
}
