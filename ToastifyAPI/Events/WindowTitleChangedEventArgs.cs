using System;

namespace ToastifyAPI.Events
{
    public class WindowTitleChangedEventArgs : EventArgs
    {
        #region Public Properties

        public string OldTitle { get; }
        public string NewTitle { get; }

        #endregion

        public WindowTitleChangedEventArgs(string oldTitle, string newTitle)
        {
            this.OldTitle = oldTitle;
            this.NewTitle = newTitle;
        }
    }
}