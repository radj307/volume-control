//using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace UIComposites
{
    public class ColorBinding<T> : ColorBindingBase where T : Control
    {
        public ColorBinding(Color? fg = null, Color? bg = null) : base(fg, bg) {}
        public ColorBinding() {}

        public override bool AppliesToControl(Control type)
            => type is T;
    }
}
