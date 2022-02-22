//using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace UIComposites
{
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
        protected ColorBindingBase()
        {
            _fg = Color.Transparent;
            _fgEnabled = false;
            _bg = Color.Transparent;
            _bgEnabled = false;
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
}
