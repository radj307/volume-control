using System;
using System.Drawing;
using System.Windows.Forms;

namespace Toastify.Common
{
    public class SystemTray : IDisposable
    {
        private readonly NotifyIcon sysTrayIcon;

        private int timeOut;
        private bool isTimeOut;
        private int _animationStepMilliseconds = 175;
        private int animationStep;
        private readonly Timer animationTimer;
        private Icon[] animationIcons;

        #region Events

        public event MouseEventHandler MouseMove;

        public event MouseEventHandler MouseUp;

        public event MouseEventHandler MouseDown;

        public event MouseEventHandler MouseClick;

        public event MouseEventHandler MouseDoubleClick;

        public event EventHandler Click;

        public event EventHandler DoubleClick;

        public event EventHandler BalloonTipShown;

        public event EventHandler BalloonTipClosed;

        public event EventHandler BalloonTipClicked;

        #endregion Events

        #region Properties

        public Icon Icon
        {
            get { return this.sysTrayIcon.Icon; }
            set { this.sysTrayIcon.Icon = value; }
        }

        public bool Visible
        {
            get { return this.sysTrayIcon.Visible; }
            set { this.sysTrayIcon.Visible = value; }
        }

        public string Text
        {
            get { return this.sysTrayIcon.Text; }
            set { this.sysTrayIcon.Text = value; }
        }

        public bool Animate { get; set; }

        public int AnimationStepMilliseconds
        {
            get
            {
                return this._animationStepMilliseconds;
            }
            set
            {
                this._animationStepMilliseconds = value;
                if (this.animationTimer != null)
                    this.animationTimer.Interval = value;
            }
        }

        public string BalloonTipText
        {
            get { return this.sysTrayIcon.BalloonTipText; }
            set { this.sysTrayIcon.BalloonTipText = value; }
        }

        public string BalloonTipTitle
        {
            get { return this.sysTrayIcon.BalloonTipTitle; }
            set { this.sysTrayIcon.BalloonTipTitle = value; }
        }

        public ToolTipIcon BalloonTipIcon
        {
            get { return this.sysTrayIcon.BalloonTipIcon; }
            set { this.sysTrayIcon.BalloonTipIcon = value; }
        }

        public ContextMenu ContextMenu
        {
            get { return this.sysTrayIcon.ContextMenu; }
            set { this.sysTrayIcon.ContextMenu = value; }
        }

        public ContextMenuStrip ContextMenuStrip
        {
            get { return this.sysTrayIcon.ContextMenuStrip; }
            set { this.sysTrayIcon.ContextMenuStrip = value; }
        }

        #endregion Properties

        public SystemTray(string tooltipText = null, Icon icon = null, bool animate = false)
        {
            this.sysTrayIcon = new NotifyIcon();

            this.animationTimer = new Timer
            {
                Interval = this.AnimationStepMilliseconds
            };
            this.sysTrayIcon.Visible = true;
            this.sysTrayIcon.Text = tooltipText;
            this.sysTrayIcon.Icon = icon;
            this.Animate = animate;

            this.sysTrayIcon.MouseMove += this.SysTrayIcon_MouseMove;
            this.sysTrayIcon.MouseUp += this.SysTrayIcon_MouseUp;
            this.sysTrayIcon.MouseDown += this.SysTrayIcon_MouseDown;
            this.sysTrayIcon.MouseClick += this.SysTrayIcon_MouseClick;
            this.sysTrayIcon.MouseDoubleClick += this.SysTrayIcon_MouseDoubleClick;
            this.sysTrayIcon.Click += this.SysTrayIcon_Click;
            this.sysTrayIcon.DoubleClick += this.SysTrayIcon_DoubleClick;
            this.sysTrayIcon.BalloonTipShown += this.SysTrayIcon_BalloonTipShown;
            this.sysTrayIcon.BalloonTipClosed += this.SysTrayIcon_BalloonTipClosed;
            this.sysTrayIcon.BalloonTipClicked += this.SysTrayIcon_BalloonTipClicked;

            this.animationTimer.Tick += this.PerformAnimationStep;
        }

        public void Dispose()
        {
            this.sysTrayIcon.Dispose();
            this.animationTimer.Dispose();
        }

        public void SetContextMenuOrStrip(object value)
        {
            if (value.GetType() == typeof(ContextMenu))
                this.sysTrayIcon.ContextMenu = (ContextMenu)value;
            if (value.GetType() == typeof(ContextMenuStrip))
                this.sysTrayIcon.ContextMenuStrip = (ContextMenuStrip)value;
        }

        public void StartAnimation()
        {
            if (this.Animate)
                this.animationTimer.Start();
        }

        public void StartAnimation(int timeOut)
        {
            if (this.Animate)
            {
                this.isTimeOut = true;
                this.timeOut = timeOut;
                this.animationTimer.Start();
            }
        }

        public void StopAnimation()
        {
            if (this.Animate)
                this.animationTimer.Stop();
        }

        public void ShowBaloonTrayTip(int timeout)
        {
            this.sysTrayIcon.ShowBalloonTip(timeout);
        }

        public void ShowBaloonTrayTip(int timeOut, string tipTitle, string tipText, ToolTipIcon tipIcon)
        {
            this.sysTrayIcon.ShowBalloonTip(timeOut, tipTitle, tipText, tipIcon);
        }

        public void SetIconRange(object[] icons)
        {
            Type tp = icons[0].GetType();
            if (tp == typeof(string))
            {
                this.animationIcons = new Icon[icons.Length];
                for (int i = 0; i < icons.Length; ++i)
                    this.animationIcons[i] = new Icon(icons[i].ToString());
            }
            if (tp == typeof(Icon))
            {
                this.animationIcons = new Icon[icons.Length];
                for (int i = 0; i < icons.Length; ++i)
                    this.animationIcons[i] = (Icon)icons[i];
            }
            if (icons.Length > 0)
                this.Icon = this.animationIcons[0];
        }

        private void PerformAnimationStep(object sender, EventArgs e)
        {
            this.sysTrayIcon.Icon = this.animationIcons[this.animationStep++];
            this.timeOut -= this.AnimationStepMilliseconds;

            if (this.animationStep >= this.animationIcons.Length)
                this.animationStep = 0;
            if (this.isTimeOut && this.timeOut < this.AnimationStepMilliseconds)
            {
                this.animationTimer.Stop();
                this.isTimeOut = false;
            }
        }

        #region Event Handlers

        private void SysTrayIcon_MouseMove(object sender, MouseEventArgs e)
        {
            this.MouseMove?.Invoke(sender, e);
        }

        private void SysTrayIcon_MouseUp(object sender, MouseEventArgs e)
        {
            this.MouseUp?.Invoke(sender, e);
        }

        private void SysTrayIcon_MouseDown(object sender, MouseEventArgs e)
        {
            this.MouseDown?.Invoke(sender, e);
        }

        private void SysTrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            this.MouseClick?.Invoke(sender, e);
        }

        private void SysTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.MouseDoubleClick?.Invoke(sender, e);
        }

        private void SysTrayIcon_Click(object sender, EventArgs e)
        {
            this.Click?.Invoke(sender, e);
        }

        private void SysTrayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.DoubleClick?.Invoke(sender, e);
        }

        private void SysTrayIcon_BalloonTipShown(object sender, EventArgs e)
        {
            this.BalloonTipShown?.Invoke(sender, e);
        }

        private void SysTrayIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            this.BalloonTipClosed?.Invoke(sender, e);
        }

        private void SysTrayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            this.BalloonTipClicked?.Invoke(sender, e);
        }

        #endregion Event Handlers
    }
}