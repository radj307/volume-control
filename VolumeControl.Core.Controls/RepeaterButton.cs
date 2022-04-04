using System.ComponentModel;

namespace VolumeControl.Core.Controls
{
    public partial class RepeaterButton : UserControl
    {
        public RepeaterButton()
        {
            RepeatDelay = 400;
            RepeatInterval = 150;
            InitializeComponent();
        }

        #region Members
        private bool _repeatDelayPassed = false;
        private Keys[] _activateKeys = new[] { Keys.Enter, Keys.Space };
        private MouseButtons[] _activateButtons = new[] { MouseButtons.Left, MouseButtons.Right };
        #endregion Members

        #region Properties
        /// <summary>
        /// The text to show on the button's face.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public new string Text
        {
            get => b.Text;
            set => b.Text = value;
        }
        /// <summary>
        /// Background color of the button.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public new Color BackColor
        {
            get => b.BackColor;
            set => base.BackColor = b.BackColor = value;
        }
        /// <summary>
        /// Foreground (text) color of the button.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public new Color ForeColor
        {
            get => b.ForeColor;
            set => b.ForeColor = value;
        }
        /// <summary>
        /// Determines the appearance of the button.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public FlatStyle FlatStyle
        {
            get => b.FlatStyle;
            set => b.FlatStyle = value;
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public int FlatBorderSize
        {
            get => b.FlatAppearance.BorderSize;
            set => b.FlatAppearance.BorderSize = value;
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public Color FlatBorderColor
        {
            get => b.FlatAppearance.BorderColor;
            set => b.FlatAppearance.BorderColor = value;
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public Color FlatMouseOverBackColor
        {
            get => b.FlatAppearance.MouseOverBackColor;
            set => b.FlatAppearance.MouseOverBackColor = value;
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public Color FlatMouseDownBackColor
        {
            get => b.FlatAppearance.MouseDownBackColor;
            set => b.FlatAppearance.MouseDownBackColor = value;
        }
        /// <summary>
        /// The amount of time (in milliseconds) that the button must be pressed before repeat pressed may be triggered.
        /// </summary>
        /// <remarks>Defaults to 400ms.</remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public int RepeatDelay { get; set; }
        /// <summary>
        /// The amount of time (in milliseconds) between each repeated button press.
        /// </summary>
        /// <remarks>Defaults to 100ms.</remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public int RepeatInterval { get; set; }
        /// <summary>
        /// Gets or sets the list of keys that can trigger events.
        /// </summary>
        /// <returns>Array of valid <see cref="Keys"/> that can trigger <see cref="Pressed"/> events.</returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public Keys[] ActivateKeys
        {
            get => _activateKeys;
            set => _activateKeys = value;
        }
        /// <summary>
        /// List of mouse buttons that can trigger events.
        /// </summary>
        /// <returns>Array of valid <see cref="MouseButtons"/> that can trigger <see cref="Pressed"/> events.</returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public MouseButtons[] ActivateButtons
        {
            get => _activateButtons;
            set => _activateButtons = value;
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public bool UseVisualStyleBackColor
        {
            get => b.UseVisualStyleBackColor;
            set => b.UseVisualStyleBackColor = value;
        }
        #endregion Properties

        #region Events
        /// <summary>
        /// Triggered whenever the button is clicked, or repeatedly when held.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public event EventHandler? Pressed;
        private void NotifyPressed(EventArgs e)
            => Pressed?.Invoke(this, e);
        #endregion Events

        #region Methods
        private void StartRepeater()
            => tRepeater.Enabled = true;
        private void StopRepeater()
        {
            tRepeater.Interval = RepeatDelay;
            tRepeater.Enabled = false;
            _repeatDelayPassed = false;
        }
        private bool Filter(Keys key)
            => ActivateKeys.Contains(key);
        private bool Filter(MouseButtons button)
            => ActivateButtons.Contains(button);
        #endregion Methods

        #region ControlEventHandlers
        /// <summary>
        /// Handler for the repeater 'Tick' events.
        /// </summary>
        private void tRepeater_Tick(object sender, EventArgs e)
        {
            if (_repeatDelayPassed)
                NotifyPressed(e);
            else
            {
                _repeatDelayPassed = true;
                tRepeater.Interval = RepeatInterval;
            }
        }
        /// <summary>
        /// Mouse Button Released.
        /// </summary>
        private void b_MouseUp(object sender, MouseEventArgs e)
        {
            if (Filter(e.Button)) StopRepeater();
        }
        /// <summary>
        /// Mouse Button Pressed.
        /// </summary>
        private void b_MouseDown(object sender, MouseEventArgs e)
        {
            if (Filter(e.Button))
            {
                NotifyPressed(e);
                StartRepeater();
            }
        }
        /// <summary>
        /// Key Pressed.
        /// </summary>
        private void b_KeyUp(object sender, KeyEventArgs e)
        {
            if (Filter(e.KeyCode)) StopRepeater();
        }
        /// <summary>
        /// Key Released.
        /// </summary>
        private void b_KeyDown(object sender, KeyEventArgs e)
        {
            if (Filter(e.KeyCode))
            {
                NotifyPressed(e);
                StartRepeater();
            }
        }
        #endregion ControlEventHandlers
    }
}
