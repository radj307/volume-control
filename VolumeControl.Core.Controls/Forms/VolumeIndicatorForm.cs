using VolumeControl.Core.Audio;
using VolumeControl.Core.Controls.Enum;
using VolumeControl.Core.Events;
using VolumeControl.Log;

namespace VolumeControl.Core.Controls.Forms
{
    public partial class VolumeIndicatorForm : Form
    {
        public VolumeIndicatorForm()
        {
            // load local settings
            DisplayCorner = (Corner)Properties.Settings.Default.VolumeDisplayCorner;
            DisplayPadding = Properties.Settings.Default.VolumeDisplayPadding;
            DisplayScreen = Screen.AllScreens.FirstOrDefault(scr => scr.DeviceName.Equals(Properties.Settings.Default.VolumeDisplayScreen, StringComparison.OrdinalIgnoreCase), Screen.PrimaryScreen);
            DisplayOffset = Properties.Settings.Default.VolumeDisplayOffset;
            TopMost = Properties.Settings.Default.VolumeFormTopMost;

            InitializeComponent();
            SuspendLayout();

            Opacity = Properties.Settings.Default.VolumeFormOpacity;

            int width = Size.Width - Size.Width / 3;

            HandleSize = new(width, width);

            // call ShowWindow once here so it is initialized correctly for subsequent calls.
            User32.ShowWindow(Handle, User32.SW_HIDE);

            VC_Static.API.SelectedProcessChanged += delegate (object sender, TargetEventArgs e)
            {
                if (!_suspended)
                {
                    if (Enabled = e.Selected is AudioProcess)
                    {
                        SuspendLayout();
                        UpdateIndicator();
                        ResumeLayout();
                    }
                }
            };
            VC_Static.HotkeyPressed += delegate (object? sender, HotkeyPressedEventArgs e)
            {
                if (Enabled && !_suspended && e.Subject == Core.Enum.VolumeControlSubject.VOLUME)
                {
                    SuspendLayout();
                    Show();
                    UpdateIndicator();
                    ResumeLayout();
                }
            };

            ResumeLayout();
        }

        private bool _suspended = false;

        /// <summary>
        /// This prevents the form from stealing focus when it appears, however it only works when <see cref="Form.TopMost"/> is false.
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            get => true;
        }

        public int TimeoutInterval
        {
            get => tTimeout.Interval;
            set => tTimeout.Interval = value;
        }
        public Size DisplayPadding { get; set; }
        public Screen DisplayScreen { get; set; }
        public Corner DisplayCorner { get; set; }
        public Size DisplayOffset { get; set; }
        public Size HandleSize { get; set; }
        public new Font Font
        {
            get => base.Font;
            set => base.Font = value;
        }

        private static bool TargetIsMuted => (VC_Static.API.GetSelectedProcess() is AudioProcess ap && ap.Muted);
        private static decimal TargetVolume => VC_Static.API.GetSelectedProcess() is AudioProcess ap ? ap.VolumeFullRange : 0m;
        /// <summary>
        /// Gets or sets the muted checkbox.
        /// </summary>
        private bool Muted
        {
            get => cbMuted.Checked;
            set => cbMuted.Checked = value;
        }
        /// <summary>
        /// Gets or sets the level of the indicator (0-100)
        /// </summary>
        private int Level
        {
            get => tbLevel.Value;
            set => tbLevel.Value = value;
        }

        /// <summary>
        /// Prevent the toast form from becoming visible.
        /// </summary>
        public void SuspendNotifications()
            => _suspended = true;
        /// <summary>
        /// Allow the toast form to be visible again.
        /// </summary>
        public void ResumeNotifications()
            => _suspended = false;

        public new void Hide()
        {
            base.Hide();
            WindowState = FormWindowState.Minimized;
        }
        public new void Show()
        {
            if (!_suspended)
            {
                tTimeout.Enabled = false;
                if (Enabled)
                {
                    WindowState = FormWindowState.Normal;
                    // use ShowWindow instead of Form.Show() to prevent stealing focus when TopMost is true.
                    User32.ShowWindow(Handle, User32.SW_SHOWNOACTIVATE);
                    UpdatePosition();
                    tTimeout.Enabled = true;
                }
                else Hide();
            }
        }

        /// <summary>
        /// Automatically sets the position of this form to be in the bottom-right corner of the screen.
        /// </summary>
        private void SetPosition()
        {
            Size wsz = DisplayScreen.WorkingArea.Size;
            Point? pos = DisplayCorner.GetPosition(wsz, Size, DisplayPadding, DisplayOffset);
            if (pos != null)
                Location = pos.Value;
            else FLog.Log.Error($"ToastForm.SetPosition() failed to calculate a valid origin point.");
            UpdateBounds();
        }

        public void UpdatePosition() => SetPosition();

        public void UpdateIndicator()
        {
            UpdatePosition();
            tbLevel.ValueChanged -= tbLevel_ValueChanged!;
            cbMuted.CheckedChanged -= cbMuted_CheckedChanged!;

            Level = Convert.ToInt32(TargetVolume);
            Muted = TargetIsMuted;

            tbLevel.ValueChanged += tbLevel_ValueChanged!;
            cbMuted.CheckedChanged += cbMuted_CheckedChanged!;
        }

        private void tTimeout_Tick(object sender, EventArgs e)
        {
            tTimeout.Enabled = false;
            Hide();
        }

        private void cbPaint(object sender, PaintEventArgs e)
        {
            if (sender is not CheckBox cb) return;

            Graphics g = e.Graphics;
            var rect = e.ClipRectangle;

            CheckBoxRenderer.DrawParentBackground(g, rect, cb);

            int boxSize = 13;

            var box = new Rectangle(rect.Location.X, rect.Location.Y + (rect.Height / 2 - boxSize / 2), boxSize - 1, boxSize - 1);
            int textStart = box.X + box.Width + 4;

            var b = new SolidBrush(Color.FromArgb(200, 200, 200));

            g.DrawRectangle(new Pen(b, 1f), box);
            if (cb.Checked)
            {
                g.FillRectangle(b, new Rectangle(box.X + 3, box.Y + 3, box.Width - 5, box.Height - 5));
            }

            g.DrawString(cb.Text, cb.Font, new SolidBrush(cb.ForeColor), new Point(textStart, rect.Location.Y + 1));
        }

        private void cbMuted_CheckedChanged(object sender, EventArgs e)
        {
            if (VC_Static.API.GetSelectedProcess() is AudioProcess ap)
                ap.Muted = Muted;
        }

        private void tbLevel_ValueChanged(object sender, EventArgs e)
        {
            if (VC_Static.API.GetSelectedProcess() is AudioProcess ap)
                ap.VolumeFullRange = Convert.ToDecimal(Level);
        }
    }
}
