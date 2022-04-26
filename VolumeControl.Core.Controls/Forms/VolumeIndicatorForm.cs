using VolumeControl.Core.Audio;
using VolumeControl.Core.Controls.Enum;
using VolumeControl.Core.Events;
using VolumeControl.Core.Extensions;
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

            // call ShowWindow once here so it is initialized correctly for subsequent calls.
            User32.ShowWindow(Handle, User32.ECmdShow.SW_HIDE);

            VC_Static.API.SelectedProcessChanged += delegate (object sender, TargetEventArgs e)
            {
                if (Enabled = e.Selected is AudioProcess)
                {
                    SuspendLayout();
                    UpdateIndicator();
                    ResumeLayout();
                }
            };
            VC_Static.HotkeyPressed += delegate (object? sender, HotkeyPressedEventArgs e)
            {
                if (e.Subject == Core.Enum.VolumeControlSubject.VOLUME)
                {
                    if (Visible)
                        tTimeout.Restart();
                    SuspendLayout();
                    Show();
                    UpdateIndicator();
                    ResumeLayout();
                }
            };

            ResumeLayout();
        }

        /// <summary>
        /// This prevents the form from stealing focus when it appears, however it only works when <see cref="Form.TopMost"/> is false.
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            get => true;
        }

        //public int TimeoutInterval
        //{
        //    get => tTimeout.Interval;
        //    set => tTimeout.Interval = value;
        //}
        public Size DisplayPadding { get; set; }
        public Screen DisplayScreen { get; set; }
        public Corner DisplayCorner { get; set; }
        public Size DisplayOffset { get; set; }

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
        /// Hides the window by fading it out over time.<br/>
        /// This function handles the entire form hiding process:
        /// <list type="bullet">
        /// <item><description>Stops <see cref="tTimeout"/>.</description></item>
        /// <item><description>Calls <see cref="Hide"/>.</description></item>
        /// <item><description>Resets the form opacity back to its value prior to calling <see cref="FadeHide(double, int)"/>.</description></item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// The total amount of time in milliseconds that it will take for the form to be completely hidden can be calculated as follows: <code>(<see cref="Form.Opacity"/> / <paramref name="step"/>) * <paramref name="interval"/></code>
        /// </remarks>
        /// <param name="step">The amount of opacity that is subtracted from the current opacity every interval.</param>
        /// <param name="interval">The amount of time in milliseconds to wait before lowering the current opacity again.</param>
        public void FadeHide(double step, int interval = 15)
        {
            tTimeout.Stop();
            var prevOpacity = Opacity; //< we need to reset the opacity after fading out
            for (double i = Opacity; i > 0.0; i -= step)
            {
                Opacity = i;
                Thread.Sleep(interval);
            }
            Hide();
            Opacity = prevOpacity;
        }
        public void FadeHide() => FadeHide(0.1, 15);
        public new void Hide()
        {
            Visible = false;
            WindowState = FormWindowState.Minimized;
        }
        public new void Show()
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
            // use ShowWindow instead of Form.Show() to prevent stealing focus when TopMost is true.
            User32.ShowWindow(Handle, User32.ECmdShow.SW_SHOWNOACTIVATE);
            UpdatePosition();
            tTimeout.Start();
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
            else FLog.Log.Error($"VolumeIndicatorForm.SetPosition() failed to calculate a valid origin point.");
            UpdateBounds();
        }

        public void UpdatePosition() => SetPosition();

        public void UpdateIndicator()
        {
            UpdatePosition();
            tbLevel.ValueChanged -= tbLevel_ValueChanged!;
            cbMuted.CheckedChanged -= cbMuted_CheckedChanged!;

            labelVolume.Text = (Level = Convert.ToInt32(TargetVolume)).ToString();
            Muted = TargetIsMuted;

            tbLevel.ValueChanged += tbLevel_ValueChanged!;
            cbMuted.CheckedChanged += cbMuted_CheckedChanged!;
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

        private void cbMuted_CheckStateChanged(object sender, EventArgs e)
        {
            cbMuted.ImageIndex = Convert.ToInt32(cbMuted.Checked);
        }

        private void tTimeout_Tick(object sender, EventArgs e) => FadeHide();
    }
}
