namespace VolumeControl.Core.Controls
{
    public partial class ToastForm : Form
    {
        #region Initializers
        public ToastForm()
        {
            // load local settings
            DisplayCorner = (Corner)Properties.Settings.Default.DisplayCorner;
            DisplayPadding = Properties.Settings.Default.DisplayPadding;
            DisplayScreen = Screen.AllScreens.FirstOrDefault(scr => scr.DeviceName == Properties.Settings.Default.DisplayScreen, Screen.PrimaryScreen);
            LockedColor = Properties.Settings.Default.LockedColor;
            UnlockedColor = Properties.Settings.Default.UnlockedColor;

            InitializeComponent();

            SuspendLayout();

            // set data source
            bsAudioProcessAPI.DataSource = VC_Static.API;

            VC_Static.API.SelectedProcessChanged += delegate
            {
                if (Enabled)
                {
                    SuspendLayout();
                    Show();
                    UpdateSelection();
                    ResumeLayout();
                }
            };
            VC_Static.API.LockSelectionChanged += delegate
            {
                if (Enabled)
                {
                    SuspendLayout();
                    Show();
                    UpdateLockSelection();
                    ResumeLayout();
                }
            };

            UpdateSelection();
            UpdateLockSelection();

            ResumeLayout();
        }
        #endregion Initializers

        #region Finalizers
        private void ToastForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // update local settings
            Properties.Settings.Default.DisplayCorner = (byte)DisplayCorner;
            Properties.Settings.Default.DisplayPadding = DisplayPadding;
            Properties.Settings.Default.DisplayScreen = DisplayScreen.DeviceName;
            Properties.Settings.Default.LockedColor = LockedColor;
            Properties.Settings.Default.UnlockedColor = UnlockedColor;
            // save settings
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            e.Cancel = true; // don't delete the form, just hide it
            Hide();
        }
        #endregion Finalizers

        #region Properties
        public int TimeoutInterval
        {
            get => tTimeout.Interval;
            set => tTimeout.Interval = value;
        }
        public Size DisplayPadding { get; set; }
        public Screen DisplayScreen { get; set; }
        public Corner DisplayCorner { get; set; }
        public Color LockedColor { get; set; }
        public Color UnlockedColor { get; set; }
        #endregion Properties

        #region Methods
        public new void Hide()
        {
            base.Hide();
            WindowState = FormWindowState.Minimized;
        }
        public new void Show()
        {
            tTimeout.Enabled = false;
            if (Enabled)
            {
                WindowState = FormWindowState.Normal;
                base.Show();
                UpdatePosition();
                tTimeout.Enabled = true;
            }
            else Hide();
        }

        private void SizeToFit()
        {
            // calculate height
            int height = listPanelInner.Padding.Top + listPanelInner.Padding.Bottom + listPanelOuter.Padding.Top + listPanelOuter.Padding.Bottom;

            if (FormBorderStyle != FormBorderStyle.None)
                height += 40;

            int itemHeight = listBox.ItemHeight;
            // add the height of all the items that can fit in 3/4 of the screen's height
            height += itemHeight * (listBox.Items.Count % ((Screen.PrimaryScreen.WorkingArea.Height - (Screen.PrimaryScreen.WorkingArea.Height / 4)) / itemHeight));

            // calculate width
            int width = listPanelInner.Padding.Left + listPanelInner.Padding.Right + listPanelOuter.Padding.Left + listPanelOuter.Padding.Right;

            float longest = 0f;
            Graphics graphics = CreateGraphics(); //< used to get the size of a string in pixels
            foreach (AudioAPI.AudioProcess proc in listBox.Items)
            {
                float sz = graphics.MeasureString(proc.ProcessName, listBox.Font).Width;
                if (sz > longest)
                    longest = sz;
            }
            width += Convert.ToInt32(Math.Ceiling(longest));

            // apply size
            Size = new(width, height);
            UpdateBounds();
        }
        /// <summary>
        /// Automatically sets the position of this form to be in the bottom-right corner of the screen.
        /// </summary>
        private void SetPosition()
        {
            Point? pos = DisplayCorner.GetPosition(DisplayScreen.WorkingArea.Size, Size, DisplayPadding);
            if (pos == null) // fallback to default
            {
                Size wsz = Screen.PrimaryScreen.WorkingArea.Size;
                pos = new(wsz.Width - Size.Width - DisplayPadding.Width, wsz.Height - Size.Height - DisplayPadding.Height);
            }
            Location = pos.Value;
            UpdateBounds();
        }

        public void UpdatePosition()
        {
            SuspendLayout();
            SizeToFit();
            SetPosition();
            ResumeLayout();
        }

        /// <summary>
        /// Updates the selected item to reflect the current target selection.
        /// </summary>
        public void UpdateSelection()
            => listBox.SelectedIndex = listBox.Items.IndexOf(VC_Static.API.GetSelectedProcess());
        /// <summary>
        /// Updates the background color to indicate whether the current target is locked.
        /// </summary>
        public void UpdateLockSelection()
            => BackColor = VC_Static.API.LockSelection ? LockedColor : UnlockedColor;
        #endregion Methods

        #region ControlEventHandlers
        private void tTimeout_Tick(object sender, EventArgs e)
        {
            tTimeout.Enabled = false;
            Hide();
        }
        private void listBox_AddedRemoved(object sender, ControlEventArgs e)
            => UpdatePosition();
        #endregion ControlEventHandlers
    }
}
