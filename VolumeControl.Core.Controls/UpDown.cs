using System.ComponentModel;

namespace VolumeControl.Core.Controls
{
    public class UpDown : UpDownBase
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public event EventHandler? UpPressed;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public event EventHandler? DownPressed;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public event EventHandler? UpdateText;

        private void NotifyUpPressed(EventArgs e)
            => UpPressed?.Invoke(this, e);

        private void NotifyDownPressed(EventArgs e)
            => DownPressed?.Invoke(this, e);

        private void NotifyUpdateText(EventArgs e)
            => UpdateText?.Invoke(this, e);

        public override void UpButton()
            => NotifyUpPressed(EventArgs.Empty);
        public override void DownButton()
            => NotifyDownPressed(EventArgs.Empty);
        protected override void UpdateEditText()
            => NotifyUpdateText(EventArgs.Empty);
    }
}
