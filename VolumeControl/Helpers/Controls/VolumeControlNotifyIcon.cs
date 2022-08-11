using CodingSeb.Localization;
using System;
using System.Collections.Generic;

namespace VolumeControl.Helpers.Controls
{
    public class VolumeControlNotifyIcon : NotifyIcon
    {
        public VolumeControlNotifyIcon(Query queryMainWindowVisible)
        { // CONSTRUCTOR:
            _mainWindowVisibilityChecker = queryMainWindowVisible; //< we can't call this yet

            _notifyIcon.Icon = Properties.Resources.iconSilvered;

            Items.AddRange(new List<System.Windows.Forms.ToolStripItem>()
            { // TOOLSTRIP:
                new System.Windows.Forms.ToolStripButton(GetShowText(), Properties.Resources.foreground, HandleShowHideClick),
                new System.Windows.Forms.ToolStripButton(GetBringToFrontText(), Properties.Resources.bringtofront, HandleBringToFrontClick),
                new System.Windows.Forms.ToolStripSeparator(),
                new System.Windows.Forms.ToolStripButton(GetCloseText(), Properties.Resources.X, HandleCloseClicked),
            }.ToArray());

            Loc.Instance.CurrentLanguageChanged += Handle_CurrentLanguageChanged;
            _contextMenuStrip.VisibleChanged += Handle_ContextMenuVisibilityChanged;
        }

        #region Fields
        private readonly Query _mainWindowVisibilityChecker;
        const int _idx_ShowHide = 0;
        const int _idx_BringToFront = 1;
        const int _idx_Close = 3;
        #endregion Fields

        #region TranslationGetters
        private static string GetShowText() => Loc.Tr("VolumeControl.NotifyIcon.Show", "Show");
        private static string GetHideText() => Loc.Tr("VolumeControl.NotifyIcon.Hide", "Hide");
        private static string GetShowHideText(bool isVisible) { if (isVisible) return GetHideText(); else return GetShowText(); }
        private static string GetBringToFrontText() => Loc.Tr("VolumeControl.NotifyIcon.BringToFront", "Bring to Front");
        private static string GetCloseText() => Loc.Tr("VolumeControl.NotifyIcon.Close", "Close");
        #endregion TranslationGetters

        #region Events
        private void Handle_CurrentLanguageChanged(object? sender, CurrentLanguageChangedEventArgs e)
        {
            Items[_idx_ShowHide].Text = GetShowHideText(_mainWindowVisibilityChecker());
            Items[_idx_BringToFront].Text = GetBringToFrontText();
            Items[_idx_Close].Text = GetCloseText();
        }

        private void Handle_ContextMenuVisibilityChanged(object? sender, EventArgs e)
        {
            _contextMenuStrip.SuspendLayout();
            if (_mainWindowVisibilityChecker())
            {
                Items[_idx_ShowHide].Text = GetShowHideText(true);
                Items[_idx_ShowHide].Image = Properties.Resources.background;
            }
            else
            {
                Items[_idx_ShowHide].Text = GetShowHideText(false);
                Items[_idx_ShowHide].Image = Properties.Resources.foreground;
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
        private void HandleCloseClicked(object? sender, EventArgs e) => CloseClicked?.Invoke(sender, e);

        public event EventHandler? ShowClicked;
        public event EventHandler? HideClicked;
        public event EventHandler? BringToFrontClicked;
        public event EventHandler? CloseClicked;
        #endregion Events
    }
}
