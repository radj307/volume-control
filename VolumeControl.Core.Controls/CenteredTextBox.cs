using System.ComponentModel;

namespace VolumeControl.Core.Controls
{
    public partial class CenteredTextBox : UserControl
    {
        #region Constructors
        public CenteredTextBox()
        {
            EdgePadding = 3;
            InitializeComponent();
            base.ForeColor = Color.Transparent;
            ForeColor = Color.Black;
        }
        #endregion Constructors

        #region Events
        /// <summary>
        /// Occurs when the System.Windows.Forms.Control.Text property value changes.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public new event EventHandler? TextChanged
        {
            add => tb.TextChanged += value;
            remove => tb.TextChanged -= value;
        }
        #endregion Events

        #region Properties 
        /// <summary>
        /// Gets or sets the text associated with this control.
        /// </summary>
        /// <Returns>The text associated with this control.</Returns>
        public new string Text
        {
            get => tb.Text;
            set => tb.Text = value;
        }
        /// <summary>
        /// Gets the length of text in the control.
        /// </summary>
        public int TextLength => tb.TextLength;
        /// <summary>
        /// Gets or sets the text that is displayed when the control has no text and does not have the focus.
        /// </summary>
        /// <Returns>The text that is displayed when the control has no text and does not have focus.</Returns>
        public string PlaceholderText
        {
            get => tb.PlaceholderText;
            set => tb.PlaceholderText = value;
        }
        /// <summary>
        /// Get or set the foreground (text) color of this control.
        /// </summary>
        public new Color ForeColor
        {
            get => tb.ForeColor;
            set => tb.ForeColor = value;
        }
        /// <summary>
        /// Get or set the background color of this control.
        /// </summary>
        public new Color BackColor
        {
            get => tb.BackColor;
            set => base.BackColor = tb.BackColor = value;
        }
        /// <summary>
        /// Gets or sets how text is aligned in the control.
        /// </summary>
        /// <returns>One of the System.Windows.Forms.HorizontalAlignment enumeration values that specifies how text is aligned in the control. The default is (HorizontalAlignment.Left).</returns>
        /// <exception cref="InvalidEnumArgumentException">A value that is not within the range of valid values for the enumeration was assigned to the property.</exception>
        public HorizontalAlignment TextAlign
        {
            get => tb.TextAlign;
            set => tb.TextAlign = value;
        }
        /// <summary>
        /// Gets or sets the amount of empty space between the text and the left/right edges of the control.
        /// </summary>
        public int EdgePadding { get; set; }
        /// <summary>
        /// Gets or sets the starting point of text selected in the text box.
        /// </summary>
        public int SelectionStart
        {
            get => tb.SelectionStart;
            set => tb.SelectionStart = value;
        }
        /// <summary>
        /// Gets or sets the number of characters selected in the text box.
        /// </summary>
        public int SelectionLength
        {
            get => tb.SelectionLength;
            set => tb.SelectionLength = value;
        }
        /// <summary>
        /// Sets the position of the cursor within the textbox.
        /// </summary>
        /// <param name="index">The index of the character to place the cursor before.</param>
        public void SetCursorPos(int index)
        {
            // we don't have to verify that index is within bounds - out-of-bounds caret will insert text after the current text.
            SelectionStart = index;
            SelectionLength = 0;
        }
        /// <summary>
        /// Gets the position of the cursor within the textbox.
        /// </summary>
        /// <returns>The position of the cursor within the textbox.</returns>
        public int GetCursorPos() => SelectionStart;
        /// <summary>
        /// Gets the underlying textbox object.
        /// </summary>
        public TextBox TextBox => tb;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Automatically resize and center the text box within the current control size.
        /// </summary>
        private void UpdateTextBoxSizeAndLocation()
        {
            int width = Size.Width - (EdgePadding * 2);
            tb.Size = new(width, tb.Size.Height);
            tb.Location = new(EdgePadding, Size.Height / 2 - tb.Size.Height / 2);
        }
        #endregion Methods

        #region ControlEventHandlers
        private void CenteredTextBox_Resize(object sender, EventArgs e)
            => UpdateTextBoxSizeAndLocation();
        #endregion ControlEventHandlers
    }
}
