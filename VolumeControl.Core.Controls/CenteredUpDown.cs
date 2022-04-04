using System.ComponentModel;

namespace VolumeControl.Core.Controls
{
    public partial class CenteredUpDown : UserControl
    {
        #region Constructor
        public CenteredUpDown()
        {
            InitializeComponent();
            // Fix winforms designer sucking major ass:
            // Force centered textbox to behave
            tb.BorderStyle = BorderStyle.None;
            // Force split container to behave (winforms designer refuses to act like an adult)
            splitContainer.Panel2MinSize = 21;
            splitContainer.SplitterDistance = 99999;
            splitContainer.SplitterWidth = 1;
            splitContainer.Panel1.Cursor = Cursors.Default;
            splitContainer.Panel2.Cursor = Cursors.Default;
            splitContainer.Cursor = Cursors.Default;
            // Add up/down arrows to the buttons
            //bUp.Text = "\x1e";
            //bDown.Text = "\x1f";
        }
        #endregion Constructor

        #region Members
        private FlatStyle _buttonStyle;
        #endregion Members

        #region Events
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public event EventHandler? UpPressed
        {
            add => bUp.Pressed += value;
            remove => bUp.Pressed -= value;
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public event EventHandler? DownPressed
        {
            add => bDown.Pressed += value;
            remove => bDown.Pressed -= value;
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public new event EventHandler? TextChanged
        {
            add => tb.TextChanged += value;
            remove => tb.TextChanged -= value;
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public new event KeyPressEventHandler? KeyPress
        {
            add => tb.KeyPress += value;
            remove => tb.KeyPress -= value;
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public new event KeyEventHandler? KeyUp
        {
            add => tb.KeyUp += value;
            remove => tb.KeyUp -= value;
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public new event KeyEventHandler? KeyDown
        {
            add => tb.KeyDown += value;
            remove => tb.KeyDown -= value;
        }
        #endregion Events

        #region Properties
        public new string Text
        {
            get => tb.Text;
            set => tb.Text = value;
        }
        public string PlaceholderText
        {
            get => tb.PlaceholderText;
            set => tb.PlaceholderText = value;
        }
        public Color BackColorTextBox
        {
            get => tb.BackColor;
            set => tb.BackColor = value;
        }
        public new Color BackColor
        {
            get => tb.BackColor;
            set => tb.BackColor = value;
        }
        public Color BackColorSplitter
        {
            get => splitContainer.BackColor;
            set => splitContainer.BackColor = value;
        }
        public FlatStyle FlatStyle
        {
            get => _buttonStyle;
            set => bUp.FlatStyle = bDown.FlatStyle = _buttonStyle = value;
        }
        public int FlatBorderSizeUp
        {
            get => bUp.FlatBorderSize;
            set => bUp.FlatBorderSize = value;
        }
        public Color FlatBorderColorUp
        {
            get => bUp.FlatBorderColor;
            set => bUp.FlatBorderColor = value;
        }
        public Color FlatMouseOverBackColorUp
        {
            get => bUp.FlatMouseOverBackColor;
            set => bUp.FlatMouseOverBackColor = value;
        }
        public Color FlatMouseDownBackColorUp
        {
            get => bUp.FlatMouseDownBackColor;
            set => bUp.FlatMouseDownBackColor = value;
        }
        public int FlatBorderSizeDown
        {
            get => bDown.FlatBorderSize;
            set => bDown.FlatBorderSize = value;
        }
        public Color FlatBorderColorDown
        {
            get => bDown.FlatBorderColor;
            set => bDown.FlatBorderColor = value;
        }
        public Color FlatMouseOverBackColorDown
        {
            get => bDown.FlatMouseOverBackColor;
            set => bDown.FlatMouseOverBackColor = value;
        }
        public Color FlatMouseDownBackColorDown
        {
            get => bDown.FlatMouseDownBackColor;
            set => bDown.FlatMouseDownBackColor = value;
        }
        #endregion Properties
    }
}
