using AppConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Helpers.Controls
{
    public class NotifyIcon : IDisposable
    {
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

            _notifyIcon.MouseUp += HandleNotifyIconClicked;
        }
        ~NotifyIcon() => this.Dispose();

        public delegate bool Query();

        #region Fields
        protected System.Windows.Forms.NotifyIcon _notifyIcon;
        protected System.Windows.Forms.ContextMenuStrip _contextMenuStrip;
        #endregion Fields

        #region Events
        public event EventHandler? Clicked;
        private void HandleNotifyIconClicked(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button.HasAnyFlag(MouseButtonsFireClickedEvent))
                Clicked?.Invoke(this, EventArgs.Empty);
        }
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
        public System.Drawing.Color ForeColor
        {
            get => _contextMenuStrip.ForeColor;
            set => _contextMenuStrip.ForeColor = value;
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
