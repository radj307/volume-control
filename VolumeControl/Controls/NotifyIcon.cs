using System;
using System.Collections.Generic;
using System.Windows.Media;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Controls
{
    public class NotifyIcon : IDisposable
    {
        #region Constructor
        public NotifyIcon()
        {
            _contextMenuStrip = new()
            {
                AutoClose = true,
                AllowTransparency = true,
                ShowCheckMargin = false,
                ShowImageMargin = false,
                ShowItemToolTips = false,
                BackColor = System.Drawing.Color.FromArgb(48, 48, 48),
                ForeColor = System.Drawing.Color.WhiteSmoke,
                DropShadowEnabled = true,
            };

            _notifyIcon = new()
            {
                ContextMenuStrip = _contextMenuStrip,
            };

            _notifyIcon.MouseClick += this.ForwardClick;
            _notifyIcon.MouseDoubleClick += this.ForwardDoubleClick;
            _notifyIcon.BalloonTipShown += this.ForwardBalloonTipOpened;
            _notifyIcon.BalloonTipClosed += this.ForwardBalloonTipClosed;

            _notifyIcon.MouseClick += this.ForwardClick;
        }
        #endregion Constructor

        #region Finalizer
        ~NotifyIcon() => this.Dispose();
        #endregion Finalizer

        #region Delegates
        public delegate bool Query();
        #endregion Delegates

        #region Fields
        protected System.Windows.Forms.NotifyIcon _notifyIcon;
        protected System.Windows.Forms.ContextMenuStrip _contextMenuStrip;
        #endregion Fields

        #region Events
        /// <inheritdoc cref="System.Windows.Forms.NotifyIcon.DoubleClick"/>
        public event EventHandler? DoubleClick;
        private void ForwardDoubleClick(object? s, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button.HasAnyFlag(this.MouseButtonsFireClickedEvent))
                DoubleClick?.Invoke(s, e);
        }
        /// <inheritdoc cref="System.Windows.Forms.NotifyIcon.Click"/>
        public event EventHandler? Click;
        private void ForwardClick(object? s, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button.HasAnyFlag(this.MouseButtonsFireClickedEvent))
                Click?.Invoke(s, e);
        }
        /// <inheritdoc cref="System.Windows.Forms.NotifyIcon.BalloonTipShown"/>
        public event EventHandler? BalloonTipOpened;
        private void ForwardBalloonTipOpened(object? s, EventArgs e) => BalloonTipOpened?.Invoke(s, e);
        /// <inheritdoc cref="System.Windows.Forms.NotifyIcon.BalloonTipClosed"/>
        public event EventHandler? BalloonTipClosed;
        private void ForwardBalloonTipClosed(object? s, EventArgs e) => BalloonTipClosed?.Invoke(s, e);
        #endregion Events

        #region Properties
        public List<System.Windows.Forms.MouseButtons> MouseButtonsFireClickedEvent { get; set; } = new()
        {
            System.Windows.Forms.MouseButtons.Left
        };
        public System.Windows.Forms.ToolStripItemCollection Items => _contextMenuStrip.Items;
        public bool Visible
        {
            get => _notifyIcon.Visible;
            set => _notifyIcon.Visible = value;
        }
        public string Tooltip
        {
            get => _notifyIcon.Text;
            set => _notifyIcon.Text = value;
        }
        public bool ShowCheckMargin
        {
            get => _contextMenuStrip.ShowCheckMargin;
            set => _contextMenuStrip.ShowCheckMargin = value;
        }
        public bool ShowImageMargin
        {
            get => _contextMenuStrip.ShowImageMargin;
            set => _contextMenuStrip.ShowImageMargin = value;
        }
        public bool ShowItemToolTips
        {
            get => _contextMenuStrip.ShowItemToolTips;
            set => _contextMenuStrip.ShowItemToolTips = value;
        }
        public System.Drawing.Color BackColor
        {
            get => _contextMenuStrip.BackColor;
            set => _contextMenuStrip.BackColor = value;
        }
        public Color Background
        {
            get => BackColor.ToWpfColor();
            set => BackColor = value.ToFormsColor();
        }
        public System.Drawing.Color ForeColor
        {
            get => _contextMenuStrip.ForeColor;
            set => _contextMenuStrip.ForeColor = value;
        }
        public Color Foreground
        {
            get => ForeColor.ToWpfColor();
            set => ForeColor = value.ToFormsColor();
        }
        public bool DropShadowEnabled
        {
            get => _contextMenuStrip.DropShadowEnabled;
            set => _contextMenuStrip.DropShadowEnabled = value;
        }
        public System.Drawing.Icon Icon
        {
            get => _notifyIcon.Icon;
            set => _notifyIcon.Icon = value;
        }
        #endregion Properties

        #region Methods
        /// <inheritdoc/>
        public void Dispose()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.ContextMenuStrip = null;
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null!;
            }
            if (_contextMenuStrip != null)
            {
                _contextMenuStrip.Visible = false;
                _contextMenuStrip.Items.Clear();
                _contextMenuStrip.Dispose();
                _contextMenuStrip = null!;
            }
            GC.SuppressFinalize(this);
        }
        #endregion Methods
    }
}
