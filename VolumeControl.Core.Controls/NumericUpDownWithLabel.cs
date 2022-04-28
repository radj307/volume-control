using System.ComponentModel;

namespace VolumeControl.Core.Controls
{
    [DefaultEvent("ValueChanged")]
    [DefaultProperty("Value")]
    public partial class NumericUpDownWithLabel : UserControl
    {
        public NumericUpDownWithLabel()
        {
            InitializeComponent();
        }

        private bool _allowAutoSize = false;

        #region Events
        /// <summary>
        /// Occurs when the System.Windows.Forms.NumericUpDown.Value property has been changed in some way.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public event EventHandler? ValueChanged
        {
            add => nud.ValueChanged += value;
            remove => nud.ValueChanged -= value;
        }
        #endregion Events

        #region Properties
        /// <summary>
        /// Gets or sets the value assigned to the spin box (also known as an up-down control).
        /// </summary>
        /// <returns>The numeric value of the System.Windows.Forms.NumericUpDown control.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The assigned value is less than the System.Windows.Forms.NumericUpDown.Minimum property value. -or- The assigned value is greater than the System.Windows.Forms.NumericUpDown.Maximum property value.</exception>
        public decimal Value
        {
            get => nud.Value;
            set => nud.Value = value;
        }
        /// <summary>
        /// Gets or sets the minimum allowed value for the spin box (also known as an up-down control).
        /// </summary>
        /// <returns>The minimum allowed value for the spin box. The default value is 0.</returns>
        public decimal Minimum
        {
            get => nud.Minimum;
            set => nud.Minimum = value;
        }
        /// <summary>
        /// Gets or sets the maximum value for the spin box (also known as an up-down control).
        /// </summary>
        /// <returns>The maximum value for the spin box. The default value is 100.</returns>
        public decimal Maximum
        {
            get => nud.Maximum;
            set => nud.Maximum = value;
        }
        /// <summary>
        /// Gets or sets the text associated with this control.<br/>
        /// This changes the label text.
        /// </summary>
        /// <returns>The text associated with this control.</returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        [DefaultValue("Label")]
        [Category("Appearance")]
        [Description("The text shown on the integrated label.")]
        public string LabelText
        {
            get => label.Text;
            set => label.Text = value;
        }
        /// <summary>
        /// Gets or sets the width of the numeric up down control.
        /// </summary>
        [Category("Layout")]
        public int BoxWidth
        {
            get => nudPanel.Size.Width;
            set => nudPanel.Size = new(value, nudPanel.Size.Height);
        }
        [Category("Appearance")]
        public override Color ForeColor
        {
            get => label.ForeColor;
            set => label.ForeColor = nud.ForeColor = value;
        }
        [Category("Appearance")]
        public Color BoxBackColor
        {
            get => nud.BackColor;
            set => nud.BackColor = value;
        }
        /// <summary>
        /// Gets the flow layout panel subcontrol.
        /// </summary>
        [Category("SubControls")]
        public FlowLayoutPanel FlowLayoutPanel => flp;
        /// <summary>
        /// Gets the numeric up down subcontrol.
        /// </summary>
        [Category("SubControls")]
        public NumericUpDown NumericUpDown => nud;
        /// <summary>
        /// Gets the label subcontrol.
        /// </summary>
        [Category("SubControls")]
        public Label Label => label;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Sets <see cref="Control.Size"/> to fit the subcontrol contents.
        /// </summary>
        public void SizeToFit()
        {
            if (!_allowAutoSize)
                return;

            SuspendLayout();

            int height = nud.Location.Y + nud.Size.Height + nud.Margin.Bottom;
            int width = nud.Location.X + BoxWidth + 3;

            Size = new Size(width, height);

            UpdateBounds();
            ResumeLayout();
        }
        private void SizeToFit(object sender, EventArgs e) => SizeToFit();
        #endregion Methods

        private void NumericUpDownWithLabel_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                _allowAutoSize = true;
            }
        }
    }
}
