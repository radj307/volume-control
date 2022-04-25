using System.Runtime.InteropServices;
using VolumeControl.Core.Audio;
using VolumeControl.Core.Controls.Enum;
using VolumeControl.Core.Events;
using VolumeControl.Log;

namespace VolumeControl.Core.Controls
{
    public partial class ToastForm : Form
    {
        #region Initializers
        public ToastForm()
        {
            Visible = false;
            SuspendNotifications();

            // load local settings
            DisplayCorner = (Corner)Properties.Settings.Default.DisplayCorner;
            DisplayPadding = Properties.Settings.Default.DisplayPadding;
            DisplayScreen = Screen.AllScreens.FirstOrDefault(scr => scr.DeviceName == Properties.Settings.Default.DisplayScreen, Screen.PrimaryScreen);
            DisplayOffset = Properties.Settings.Default.DisplayOffset;
            IndicatorWidth = Properties.Settings.Default.IndicatorWidth;
            TopMost = Properties.Settings.Default.ToastFormTopMost;

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
            VC_Static.HotkeyPressed += delegate(object? sender, HotkeyPressedEventArgs e)
            {
                if (Enabled && e.Subject == Core.Enum.VolumeControlSubject.TARGET)
                {
                    SuspendLayout();
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

        #region Finalizers
        private void ToastForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // update local settings
            Properties.Settings.Default.DisplayCorner = (byte)DisplayCorner;
            Properties.Settings.Default.DisplayPadding = DisplayPadding;
            Properties.Settings.Default.DisplayScreen = DisplayScreen.DeviceName;
            Properties.Settings.Default.DisplayOffset = DisplayOffset;
            Properties.Settings.Default.IndicatorWidth = IndicatorWidth;
            // save settings
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            e.Cancel = true; // don't delete the form, just hide it
            Hide();
        }
        #endregion Finalizers

        #region Members
        private bool _allowAutoSize = false;
        private bool _suspended = true;
        #endregion Members

        #region Properties
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        private const int WS_EX_TOPMOST = 0x00000008;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= WS_EX_TOPMOST;
                return createParams;
            }
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
                    base.Show();
                    UpdatePosition();
                    tTimeout.Enabled = true;
                }
                else Hide();
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
            else FLog.Log.WriteError($"ToastForm.SetPosition() failed to calculate a valid origin point.");
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
            Hide();
        }
        private void listBox_AddedRemoved(object sender, ControlEventArgs e)
            => UpdatePosition();
        private void ToastForm_Load(object sender, EventArgs e)
        {
            ResumeSizeToFit();
        }

        private void listBox_MouseClick(object sender, MouseEventArgs e) => VC_Static.API.TrySetSelectedProcess((listBox.SelectedItem as IAudioProcess)?.ProcessName, false);

        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || listBox.Items[e.Index] is not AudioProcess item)
            {
                FLog.Log.WriteError($"ToastForm.listBox_DrawItem() skipped drawing item at index '{e.Index}' because it isn't an AudioProcess!");
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
        #endregion ControlEventHandlers
    }
    public static class DecimalExtensions
    {
        public static decimal Scale(this decimal n, decimal nMin, decimal nMax, decimal outMin, decimal outMax)
        {
            return nMin + (n - nMin) * (outMax - outMin) / (nMax - nMin);
        }
    }
}
