namespace UIComposites
{
    public partial class NumberUpDownWithLabel : UserControl
    {
        #region Properties

        public decimal Value
        {
            get => numUpDown.Value;
            set => numUpDown.Value = value;
        }
        public decimal MinValue
        {
            get => numUpDown.Minimum;
            set => numUpDown.Minimum = value;
        }
        public decimal MaxValue
        {
            get => numUpDown.Maximum;
            set => numUpDown.Maximum = value;
        }
        public decimal Increment
        {
            get => numUpDown.Increment;
            set => numUpDown.Increment = value;
        }
        public string LabelText
        {
            get => label.Text;
            set => label.Text = value;
        }
        public int ValueInt32 => Convert.ToInt32(Value);
        public long ValueInt64 => Convert.ToInt64(Value);
        public int NumberUpDownWidth
        {
            get => numUpDown.Width;
            set => numUpDown.Width = value;
        }

        #endregion Properties

        public NumberUpDownWithLabel()
        {
            InitializeComponent();
        }


        public event EventHandler ValueChanged
        {
            add => numUpDown.ValueChanged += value;
            remove => numUpDown.ValueChanged -= value;
        }
    }
}
