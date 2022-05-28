using System;
using VolumeControl.Properties;

namespace VolumeControl.Helpers.Controls
{
    public class NotifyIcon : IDisposable
    {
        #region Constructors
        public NotifyIcon(string text, QueryMainWindowVisibleEventHandler queryVisibility)
        {
            _queryMainWindowVisibleHandler = queryVisibility;

            _contextMenu = new()
            {
                AutoClose = true,
                AllowTransparency = true,
                ShowCheckMargin = false,
                ShowImageMargin = false,
                ShowItemToolTips = false,
                BackColor = System.Drawing.Color.FromArgb(48, 48, 48),
                ForeColor = System.Drawing.Color.WhiteSmoke
            };

            _contextMenu.Items.Add(new System.Windows.Forms.ToolStripButton("Temp", Resources.reload)); //< this is a placeholder
            _contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            _contextMenu.Items.Add(new System.Windows.Forms.ToolStripButton("Close", Resources.X, HandleCloseButtonClick));

            _notifyIcon = new()
            {
                Visible = true,
                Icon = Resources.iconSilvered,
                Text = text,
                ContextMenuStrip = _contextMenu,
            };

            _notifyIcon.MouseUp += HandleClicked;

            _contextMenu.VisibleChanged += (s, e) =>
            {
                if (_queryMainWindowVisibleHandler(this, EventArgs.Empty))
                { // window is visible
                    _contextMenu.SuspendLayout();
                    DynamicButton.Text = "Hide";
                    DynamicButton.Image = Resources.background;
                    DynamicButton.Click -= HandleBringToFrontClicked;
                    DynamicButton.Click += HandleSendToBackClicked;
                    _contextMenu.Refresh();
                    _contextMenu.ResumeLayout();
                }
                else
                { // window is hidden
                    _contextMenu.SuspendLayout();
                    DynamicButton.Text = "Show";
                    DynamicButton.Image = Resources.foreground;
                    DynamicButton.Click -= HandleSendToBackClicked;
                    DynamicButton.Click += HandleBringToFrontClicked;
                    _contextMenu.Refresh();
                    _contextMenu.ResumeLayout();
                }
            };
        }
        #endregion Constructors

        #region Finalizer
        ~NotifyIcon() { Dispose(); }
        #endregion Finalizer

        #region Delegates
        public delegate bool QueryMainWindowVisibleEventHandler(object? sender, EventArgs e);
        #endregion Delegates

        #region Fields
        private readonly QueryMainWindowVisibleEventHandler _queryMainWindowVisibleHandler;

        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private System.Windows.Forms.ContextMenuStrip _contextMenu;
        #endregion Fields

        #region Events
        public event EventHandler? Clicked;
        private void HandleClicked(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Left))
                Clicked?.Invoke(sender, e);
        }
        /// <summary>Triggered when the close button is clicked.</summary>
        public event EventHandler? CloseButtonClicked;
        private void HandleCloseButtonClick(object? sender, EventArgs e) => CloseButtonClicked?.Invoke(sender, e);
        /// <summary>Triggered when the bring to front button is clicked.</summary>
        public event EventHandler? BringToFrontClicked;
        private void HandleBringToFrontClicked(object? sender, EventArgs e) => BringToFrontClicked?.Invoke(sender, e);
        /// <summary>Triggered when the send to back button is clicked.</summary>
        public event EventHandler? SendToBackClicked;
        private void HandleSendToBackClicked(object? sender, EventArgs e) => SendToBackClicked?.Invoke(sender, e);
        #endregion Events

        #region Properties
        /// <inheritdoc cref="System.Windows.Forms.NotifyIcon.Text"/>
        public string Text
        {
            get => _notifyIcon.Text;
            set => _notifyIcon.Text = value;
        }
        /// <inheritdoc cref="System.Windows.Forms.NotifyIcon.Icon"/>
        public System.Drawing.Icon Icon
        {
            get => _notifyIcon.Icon;
            set => _notifyIcon.Icon = value;
        }
        private System.Windows.Forms.ToolStripItem DynamicButton => _contextMenu.Items[0];
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
            if (_contextMenu != null)
            {
                _contextMenu.Visible = false;
                _contextMenu.Items.Clear();
                _contextMenu.Dispose();
                _contextMenu = null!;
            }
            GC.SuppressFinalize(this);
        }
        #endregion Methods
    }
}
