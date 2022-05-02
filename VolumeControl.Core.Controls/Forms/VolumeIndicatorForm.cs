using System.ComponentModel;
using VolumeControl.Core.Audio;
using VolumeControl.Core.Controls.Enum;
using VolumeControl.Core.Controls.Extensions;
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

            InitializeComponent();
            SuspendLayout();

            if (!TopMost)
                TopMost = true;

            TimeoutInterval = Properties.Settings.Default.VolumeFormTimeoutInterval;
            Opacity = Properties.Settings.Default.VolumeFormOpacity;
            colorPanel.BackColor = Properties.Settings.Default.VolumeFormBackColor;

            VC_Static.HotkeyPressed += delegate (object? sender, HotkeyPressedEventArgs e)
            {
                if (_suspended)
                    return;
                if (e.Subject == Core.Enum.VolumeControlSubject.VOLUME)
                {
                    if (Visible)
                        tTimeout.Restart();
                    SuspendLayout();
                    Show();
                    UpdateIndicator();
                    ResumeLayout();
                }
                else if (e.Subject == Core.Enum.VolumeControlSubject.TARGET)
                {
                    UpdateIndicator();
                }
            };

            ResumeLayout();
        }

        private bool _suspended = false;
        private EVolumeIndicatorFormMode _colorMode = EVolumeIndicatorFormMode.Default;

        public bool Suspended
        {
            get => _suspended;
            set => _suspended = value;
        }
        /// <summary>
        /// This prevents the form from stealing focus when it appears, however it only works when <see cref="Form.TopMost"/> is false.
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            get => true;
        }
        /// <summary>
        /// Gets or sets the size of the form.
        /// </summary>
        /// <remarks>Overrides the <see cref="Form.Size"/> property's setter so that <see cref="Form.MinimumSize"/> and <see cref="Form.MaximumSize"/> are set as well.</remarks>
        public new Size Size
        {
            get => base.Size;
            set => base.Size = MaximumSize = MinimumSize = value;
        }
        /// <summary>
        /// Gets or sets the color of the form's border area.
        /// </summary>
        [Category("Appearance")]
        public Color BorderColor
        {
            get => colorPanel.BackColor;
            set => colorPanel.BackColor = value;
        }
        public static Color BorderColorDefault => Properties.Settings.Default.VolumeFormBackColor;
        public static Color BorderColorGlobal => Properties.Settings.Default.VolumeFormBackColorGlobal;
        public EVolumeIndicatorFormMode ColorMode
        {
            get => _colorMode;
            set => _colorMode = value;
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
        public void FadeHide(double step, int interval)
        {
            tTimeout.Stop();
            if (step > 0.0)
            {
                var prevOpacity = Opacity; //< we need to reset the opacity after fading out
                for (double i = Opacity; i > 0.0; i -= step)
                {
                    Opacity = i;
                    Thread.Sleep(interval);
                }
                Hide();
                Opacity = prevOpacity;
            }
            else Hide(); //< step is 0, this would cause an infinite loop if allowed to continue
        }
        public void FadeHide() => FadeHide(Properties.Settings.Default.VolumeFormFadeStep, Properties.Settings.Default.VolumeFormFadePeriod);
        public new void Hide()
        {
            if (tTimeout.Enabled)
                tTimeout.Stop();
            Visible = false;
            WindowState = FormWindowState.Minimized;
        }
        public new void Show()
        {
            if (!_suspended)
            {
                Visible = true;
                WindowState = FormWindowState.Normal;
                User32.ShowWindow(this.Handle, User32.ECmdShow.SW_SHOWNOACTIVATE);
                //User32.SetWindowPos(this.Handle, User32.HWND_TOP, Location.X, Location.Y, Size.Width, Size.Height,
                //    User32.EUFlags.SWP_NOACTIVATE | User32.EUFlags.SWP_NOMOVE | User32.EUFlags.SWP_NOREPOSITION
                //);
                UpdatePosition();
                tTimeout.Start();
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
            else FLog.Log.Error($"VolumeIndicatorForm.SetPosition() failed to calculate a valid origin point.");
            UpdateBounds();
        }

        public void UpdatePosition() => SetPosition();

        public void UpdateIndicator()
        {
            tbLevel.ValueChanged -= tbLevel_ValueChanged!;
            cbMuted.CheckedChanged -= cbMuted_CheckedChanged!;



            // set the volume label and the level
            labelVolume.Text = (Level = Convert.ToInt32(TargetVolume)).ToString();
            // set the muted checkbox
            Muted = TargetIsMuted;

            tbLevel.ValueChanged += tbLevel_ValueChanged!;
            cbMuted.CheckedChanged += cbMuted_CheckedChanged!;
        }

        private void cbMuted_CheckedChanged(object sender, EventArgs e)
        {
            tTimeout.Restart();
            if (VC_Static.API.GetSelectedProcess() is AudioProcess ap)
                ap.Muted = Muted;
        }
        private void tbLevel_ValueChanged(object sender, EventArgs e)
        {
            tTimeout.Restart();
            if (VC_Static.API.GetSelectedProcess() is AudioProcess ap)
                labelVolume.Text = (ap.VolumeFullRange = Convert.ToDecimal(Level)).ToString();
        }
        private void cbMuted_CheckStateChanged(object sender, EventArgs e)
        {
            tTimeout.Restart();
            cbMuted.ImageIndex = Convert.ToInt32(cbMuted.Checked);
        }

        private void tTimeout_Tick(object sender, EventArgs e) => FadeHide();

        private void VolumeIndicatorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; //< don't allow subforms to close or they'll be deleted, only close when the program exits.
            // apply property values
            Properties.Settings.Default.SetProperty("VolumeFormTimeoutInterval", TimeoutInterval);
            Properties.Settings.Default.SetProperty("VolumeDisplayCorner", (byte)DisplayCorner);
            Properties.Settings.Default.SetProperty("VolumeDisplayPadding", DisplayPadding);
            Properties.Settings.Default.SetProperty("VolumeDisplayScreen", DisplayScreen.DeviceName);
            Properties.Settings.Default.SetProperty("VolumeDisplayOffset", DisplayOffset);
            // save properties
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        private void VolumeIndicatorForm_MouseEnter(object sender, EventArgs e) => tTimeout.Stop();
        private void VolumeIndicatorForm_MouseLeave(object sender, EventArgs e) => tTimeout.Start();
    }
}
