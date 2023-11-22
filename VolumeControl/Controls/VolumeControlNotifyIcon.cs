using Localization;
using System;

namespace VolumeControl.Controls
{
    public class VolumeControlNotifyIcon : NotifyIcon
    {
        public VolumeControlNotifyIcon(Query queryMainWindowVisible)
        { // CONSTRUCTOR:
            _mainWindowVisibilityChecker = queryMainWindowVisible; //< we can't call this yet

            _notifyIcon.Icon = Properties.Resources.iconSilvered;

            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            { // TOOLSTRIP:
                new System.Windows.Forms.ToolStripButton(GetShowText(), Properties.Resources.foreground, this.HandleShowHideClick),
                new System.Windows.Forms.ToolStripButton(GetBringToFrontText(), Properties.Resources.bringtofront, this.HandleBringToFrontClick),
                new System.Windows.Forms.ToolStripSeparator(),

                new System.Windows.Forms.ToolStripButton("Open Config", Properties.Resources.codefile, this.HandleOpenConfigClicked),
                new System.Windows.Forms.ToolStripButton("Open Log", Properties.Resources.textfile, this.HandleOpenLogClicked),

                new System.Windows.Forms.ToolStripSeparator(),
                new System.Windows.Forms.ToolStripButton(GetOpenAppDataText(), Properties.Resources.file_inverted, this.HandleOpenAppDataClick),
                new System.Windows.Forms.ToolStripButton(GetOpenLocationText(), Properties.Resources.file, this.HandleOpenLocationClick),
                new System.Windows.Forms.ToolStripButton(GetCloseText(), Properties.Resources.X, this.HandleCloseClicked),
            });

            Loc.Instance.CurrentLanguageChanged += this.Handle_CurrentLanguageChanged;
            _contextMenuStrip.VisibleChanged += this.Handle_ContextMenuVisibilityChanged;
        }

        #region Fields
        private readonly Query _mainWindowVisibilityChecker;
        private const int _idx_ShowHide = 0;
        private const int _idx_BringToFront = 1;
        private const int _idx_Close = 3;
        #endregion Fields

        #region TranslationGetters
        private static string GetShowText() => Loc.Tr("VolumeControl.NotifyIcon.Show", "Show");
        private static string GetHideText() => Loc.Tr("VolumeControl.NotifyIcon.Hide", "Hide");
        private static string GetShowHideText(bool isVisible) => isVisible ? GetHideText() : GetShowText();
        private static string GetBringToFrontText() => Loc.Tr("VolumeControl.NotifyIcon.BringToFront", "Bring to Front");
        private static string GetOpenLocationText() => Loc.Tr("VolumeControl.NotifyIcon.OpenLocation", "Open Location");
        private static string GetOpenAppDataText() => Loc.Tr("VolumeControl.NotifyIcon.OpenAppData", "Open AppData");
        private static string GetCloseText() => Loc.Tr("VolumeControl.NotifyIcon.Close", "Close");
        #endregion TranslationGetters

        #region Events
        private void Handle_CurrentLanguageChanged(object? sender, CurrentLanguageChangedEventArgs e)
        {
            this.Items[_idx_ShowHide].Text = GetShowHideText(_mainWindowVisibilityChecker());
            this.Items[_idx_BringToFront].Text = GetBringToFrontText();
            this.Items[_idx_Close].Text = GetCloseText();
        }

        private void Handle_ContextMenuVisibilityChanged(object? sender, EventArgs e)
        {
            _contextMenuStrip.SuspendLayout();
            if (_mainWindowVisibilityChecker())
            {
                this.Items[_idx_ShowHide].Text = GetShowHideText(true);
                this.Items[_idx_ShowHide].Image = Properties.Resources.background;
            }
            else
            {
                this.Items[_idx_ShowHide].Text = GetShowHideText(false);
                this.Items[_idx_ShowHide].Image = Properties.Resources.foreground;
            }
            _contextMenuStrip.ResumeLayout();
            _contextMenuStrip.Refresh();
        }
        private void HandleShowHideClick(object? sender, EventArgs e)
        {
            if (_mainWindowVisibilityChecker())
            {
                HideClicked?.Invoke(sender, e);
            }
            else
            {
                ShowClicked?.Invoke(sender, e);
            }
        }
        private void HandleBringToFrontClick(object? sender, EventArgs e) => BringToFrontClicked?.Invoke(sender, e);
        private void HandleOpenLocationClick(object? sender, EventArgs e) => OpenLocationClicked?.Invoke(sender, e);
        private void HandleOpenAppDataClick(object? sender, EventArgs e) => OpenAppDataClicked?.Invoke(sender, e);
        private void HandleCloseClicked(object? sender, EventArgs e) => CloseClicked?.Invoke(sender, e);
        private void HandleOpenConfigClicked(object? sender, EventArgs e) => OpenConfigClicked?.Invoke(sender, e);
        private void HandleOpenLogClicked(object? sender, EventArgs e) => OpenLogClicked?.Invoke(sender, e);

        public event EventHandler? ShowClicked;
        public event EventHandler? HideClicked;
        public event EventHandler? BringToFrontClicked;

        public event EventHandler? OpenConfigClicked;
        public event EventHandler? OpenLogClicked;

        public event EventHandler? OpenLocationClicked;
        public event EventHandler? OpenAppDataClicked;
        public event EventHandler? CloseClicked;
        #endregion Events
    }
}
