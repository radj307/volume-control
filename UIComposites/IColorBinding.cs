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
}
