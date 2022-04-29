using VolumeControl.Core.Audio;
using VolumeControl.Core.Controls.Enum;
using VolumeControl.Core.Controls.Extensions;
using VolumeControl.Core.Events;
using VolumeControl.Core.Extensions;
using VolumeControl.Log;

namespace VolumeControl.Core.Controls
{
    public partial class ToastForm : Form
    {
        #region Initializers
        public ToastForm()
        {
            SuspendNotifications();

            // load local settings
            DisplayCorner = (Corner)Properties.Settings.Default.ToastDisplayCorner;
            DisplayPadding = Properties.Settings.Default.ToastDisplayPadding;
            DisplayScreen = Screen.AllScreens.FirstOrDefault(scr => scr.DeviceName.Equals(Properties.Settings.Default.ToastDisplayScreen, StringComparison.OrdinalIgnoreCase), Screen.PrimaryScreen);
            DisplayOffset = Properties.Settings.Default.ToastDisplayOffset;
            IndicatorWidth = Properties.Settings.Default.IndicatorWidth;

            InitializeComponent();
            SuspendLayout();

            Opacity = Properties.Settings.Default.ToastOpacity;

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
                    listBox.Refresh();
                    ResumeLayout();
                }
            };
            VC_Static.API.ProcessListUpdated += delegate
            {
                SuspendLayout();
                listBox.DataSource = null;
                listBox.Items.Clear();
                listBox.DataSource = bsAudioProcessAPI;
                ResumeLayout();
            };
            VC_Static.HotkeyPressed += delegate (object? sender, HotkeyPressedEventArgs e)
            {
                if (Enabled && e.Subject == Core.Enum.VolumeControlSubject.TARGET)
                {
                    SuspendLayout();
                    tTimeout.Restart();
                    UpdateSelection();
                    UpdateLockSelection();
                    if (ShowIndicator)
                        listBox.Refresh();
                    ResumeLayout();
                }
            };

            UpdateSelection();
            UpdateLockSelection();

            Font = Properties.Settings.Default.ToastFont;

            ResumeLayout();
        }
        #endregion Initializers

        #region Members
        private bool _allowAutoSize = false;
        private bool _suspended = true;
        #endregion Members

        #region Properties
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
        public new Font Font
        {
            get => listBox.Font;
            set => listBox.Font = value;
        }
        private static bool TargetIsLocked => VC_Static.API.LockSelection;
        private static bool TargetIsMuted => (VC_Static.API.GetSelectedProcess() is AudioProcess ap && ap.Muted);
        private static decimal TargetVolume => VC_Static.API.GetSelectedProcess() is AudioProcess ap ? ap.VolumeFullRange : 0m;
        /// <summary>
        /// Color of the volume level indicator.
        /// </summary>
        private static Color ColorLevel => Properties.Settings.Default.ColorLevel;
        /// <summary>
        /// Color when the target is locked and isn't muted
        /// </summary>
        private static Color ColorLockUnmuted => Properties.Settings.Default.ColorLock;
        /// <summary>
        /// Color when the target is locked and is muted.
        /// </summary>
        private static Color ColorLockMuted => Properties.Settings.Default.ColorLockMuted;
        /// <summary>
        /// Color when the target is locked, auto-selected based on whether the target is muted.
        /// </summary>
        private static Color ColorLock => TargetIsMuted ? ColorLockMuted : ColorLockUnmuted;
        /// <summary>
        /// Color when the target is unlocked and isn't muted.
        /// </summary>
        private static Color ColorUnlockUnmuted => Properties.Settings.Default.ColorUnlock;
        /// <summary>
        /// Color when the target is unlocked and is muted.
        /// </summary>
        private static Color ColorUnlockMuted => Properties.Settings.Default.ColorUnlockMuted;
        /// <summary>
        /// Color when the target is unlocked, auto-selected based on whether the target is muted.
        /// </summary>
        private static Color ColorUnlock => TargetIsMuted ? ColorUnlockMuted : ColorUnlockUnmuted;
        /// <summary>
        /// Auto-selects the color based on whether it is locked/unlocked and muted/unmuted.
        /// </summary>
        private static Color Color => TargetIsLocked ? ColorLock : ColorUnlock;

        private static bool ShowIndicator => Properties.Settings.Default.ShowIndicator;
        public int IndicatorWidth { get; set; }
        private int LevelIndicatorHeight => Convert.ToInt32(TargetVolume.Scale(0m, 100m, Convert.ToDecimal(IndicatorWidth), Convert.ToDecimal(listBox.Size.Height)));
        #endregion Properties

        #region Methods
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

        public void SuspendSizeToFit()
            => _allowAutoSize = false;

        public void ResumeSizeToFit(bool trigger = false)
        {
            _allowAutoSize = true;
            if (trigger)
                SizeToFit();
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
            tTimeout.Enabled = false;
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
        public void FadeHide() => FadeHide(Properties.Settings.Default.ToastFadeStep, Properties.Settings.Default.ToastFadePeriod);
        public new void Hide()
        {
            if (tTimeout.Enabled)
                tTimeout.Stop();
            Visible = false;
            WindowState = FormWindowState.Minimized;
        }
        public new void Show()
        {
            tTimeout.Restart();
            if (!_suspended)
            {
                User32.ShowWindow(this.Handle, User32.ECmdShow.SW_SHOWNOACTIVATE);
                //User32.SetWindowPos(this.Handle, User32.HWND_TOP, Location.X, Location.Y, Size.Width, Size.Height,
                //    User32.EUFlags.SWP_NOACTIVATE | User32.EUFlags.SWP_NOMOVE | User32.EUFlags.SWP_NOREPOSITION
                //);
                Visible = true;
                WindowState = FormWindowState.Normal;
                UpdatePosition();
                tTimeout.Start();
            }
        }

        private void SizeToFit()
        {
            if (!_allowAutoSize)
                return;

            // calculate height
            int height = colorPanel.Padding.Top + colorPanel.Padding.Bottom + listPanelInner.Padding.Top + listPanelInner.Padding.Bottom + listPanelOuter.Padding.Top + listPanelOuter.Padding.Bottom;

            if (FormBorderStyle != FormBorderStyle.None)
                height += 40;

            int itemHeight = listBox.ItemHeight;
            // add the height of all the items that can fit in 3/4 of the screen's height
            height += itemHeight * (listBox.Items.Count % ((Screen.PrimaryScreen.WorkingArea.Height - (Screen.PrimaryScreen.WorkingArea.Height / 4)) / itemHeight));

            // calculate width
            int width = colorPanel.Padding.Left + colorPanel.Padding.Right + listPanelInner.Padding.Left + listPanelInner.Padding.Right + listPanelOuter.Padding.Left + listPanelOuter.Padding.Right;

            float longest = 0f;
            Graphics graphics = CreateGraphics(); //< used to get the size of a string in pixels
            var font = new Font(listBox.Font.FontFamily, listBox.Font.Size, FontStyle.Bold);
            foreach (AudioProcess proc in listBox.Items)
            {
                float sz = graphics.MeasureString(proc.ProcessName, font).Width;
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
            Size wsz = DisplayScreen.WorkingArea.Size;
            Point? pos = DisplayCorner.GetPosition(wsz, Size, DisplayPadding, DisplayOffset);
            if (pos != null)
                Location = pos.Value;
            else FLog.Log.Error($"ToastForm.SetPosition() failed to calculate a valid origin point.");
            UpdateBounds();
        }

        public void UpdatePosition()
        {
            SuspendLayout();
            if (!_allowAutoSize)
                ResumeSizeToFit();
            SizeToFit();
            SetPosition();
            ResumeLayout();
        }

        /// <summary>
        /// Updates the selected item to reflect the current target selection.
        /// </summary>
        public void UpdateSelection()
        {
            var sel = VC_Static.API.GetSelectedProcess();
            if (sel is AudioProcess ap)
            {
                int i = listBox.Items.IndexOf(ap);
                if (i != -1)
                {
                    listBox.SelectedIndex = i;
                    return;
                }
            }
            listBox.SelectedIndex = -1;
            listBox.ClearSelected();
        }
        /// <summary>
        /// Updates the background color to indicate whether the current target is locked.
        /// </summary>
        public void UpdateLockSelection()
            => colorPanel.BackColor = Color;
        #endregion Methods

        #region ControlEventHandlers
        private void tTimeout_Tick(object sender, EventArgs e)
        {
            tTimeout.Enabled = false;
            FadeHide();
        }
        private void listBox_AddedRemoved(object sender, ControlEventArgs e)
            => UpdatePosition();
        private void ToastForm_Load(object sender, EventArgs e)
            => ResumeSizeToFit(true);

        private void listBox_MouseClick(object sender, MouseEventArgs e) => VC_Static.API.TrySetSelectedProcess((listBox.SelectedItem as IAudioProcess)?.ProcessName, false);

        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || listBox.Items[e.Index] is not AudioProcess item)
            {
                FLog.Log.Error($"ToastForm.listBox_DrawItem() skipped drawing item at index '{e.Index}' because it isn't an AudioProcess!");
                return;
            }

            e.DrawBackground();
            Graphics g = e.Graphics;
            bool isSelected = (e.State & DrawItemState.Selected) != 0;
            g.FillRectangle(new SolidBrush(isSelected ? Color : e.BackColor), e.Bounds);

            if (ShowIndicator)
            {
                int indicatorHeight = listBox.Size.Height - LevelIndicatorHeight;
                if (indicatorHeight >= e.Bounds.Y && indicatorHeight <= e.Bounds.Y + e.Bounds.Height)
                {
                    listBox.CreateGraphics().DrawLine(new Pen(new SolidBrush(ColorLevel), IndicatorWidth), new Point(e.Bounds.X, indicatorHeight), new Point(e.Bounds.X + e.Bounds.Width, indicatorHeight));
                }
            }

            var font = e.Font ?? listBox.Font;
            if (isSelected)
            {
                g.DrawString(item.ProcessName, new Font(font.FontFamily, font.Size, FontStyle.Bold), new SolidBrush(Color.Black), e.Bounds, StringFormat.GenericDefault);
            }
            else
                g.DrawString(item.ProcessName, font, new SolidBrush(e.ForeColor), e.Bounds, StringFormat.GenericDefault);
        }
        private void ToastForm_MouseEnter(object sender, EventArgs e) => tTimeout.Stop();
        private void ToastForm_MouseLeave(object sender, EventArgs e) => tTimeout.Start();
        private void ToastForm_Shown(object sender, EventArgs e)
        {
            User32.ShowWindow(this.Handle, User32.ECmdShow.SW_SHOWNOACTIVATE);
        }
        #endregion ControlEventHandlers
    }
}
