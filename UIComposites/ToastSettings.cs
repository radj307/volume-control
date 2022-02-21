namespace UIComposites
{
    public partial class ToastSettings : UserControl
    {
        #region Properties

        public bool TargetListEnabled
        {
            get => cb_EnableNotification.Checked;
            set => cb_EnableNotification.Checked = value;
        }
        public int TargetListTimeout
        {
            get => num_Timeout.ValueInt32;
            set => num_Timeout.Value = Convert.ToDecimal(value);
        }
        public bool EnableDarkMode
        {
            get => cb_ToastDarkMode.Checked;
            set => cb_ToastDarkMode.Checked = value;
        }

        #endregion Properties

        public ToastSettings()
        {
            InitializeComponent();
        }

        public event EventHandler TargetListEnabledChanged
        {
            add => cb_EnableNotification.CheckedChanged += value;
            remove => cb_EnableNotification.CheckedChanged -= value;
        }
        public event EventHandler TargetListTimeoutChanged
        {
            add => num_Timeout.ValueChanged += value;
            remove => num_Timeout.ValueChanged -= value;
        }
        public event EventHandler DarkModeChanged
        {
            add => cb_ToastDarkMode.CheckedChanged += value;
            remove => cb_ToastDarkMode.CheckedChanged -= value;
        }
    }
}
