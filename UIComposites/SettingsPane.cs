namespace UIComposites
{
    public partial class SettingsPane : UserControl
    {
        #region Properties

        public bool AlwaysOnTop
        {
            get => cb_AlwaysOnTop.Checked;
            set => cb_AlwaysOnTop.Checked = value;
        }
        public bool RunAtStartup
        {
            get => cb_RunAtStartup.Checked;
            set => cb_RunAtStartup.Checked = value;
        }
        public bool MinimizeOnStartup
        {
            get => cb_MinimizeOnStartup.Checked;
            set => cb_MinimizeOnStartup.Checked = value;
        }
        public bool ShowInTaskbar
        {
            get => cb_ShowInTaskbar.Checked;
            set => cb_ShowInTaskbar.Checked = value;
        }
        public bool EnableDarkMode
        {
            get => cb_DarkMode.Checked;
            set => cb_DarkMode.Checked = value;
        }
        public decimal VolumeStep
        {
            get => num_VolumeStep.Value;
            set => num_VolumeStep.Value = value;
        }

        #endregion Properties

        public SettingsPane()
        {
            InitializeComponent();
        }

        public event EventHandler AlwaysOnTopChanged
        {
            add => cb_AlwaysOnTop.CheckedChanged += value;
            remove => cb_AlwaysOnTop.CheckedChanged -= value;
        }
        public event EventHandler RunAtStartupChanged
        {
            add => cb_RunAtStartup.CheckedChanged += value;
            remove => cb_RunAtStartup.CheckedChanged -= value;
        }
        public event EventHandler MinimizeOnStartupChanged
        {
            add => cb_MinimizeOnStartup.CheckedChanged += value;
            remove => cb_MinimizeOnStartup.CheckedChanged -= value;
        }
        public event EventHandler ShowInTaskbarChanged
        {
            add => cb_ShowInTaskbar.CheckedChanged += value;
            remove => cb_ShowInTaskbar.CheckedChanged -= value;
        }
        public event EventHandler DarkModeChanged
        {
            add => cb_DarkMode.CheckedChanged += value;
            remove => cb_DarkMode.CheckedChanged -= value;
        }
        public event EventHandler VolumeStepChanged
        {
            add => num_VolumeStep.ValueChanged += value;
            remove => num_VolumeStep.ValueChanged -= value;
        }
    }
}
