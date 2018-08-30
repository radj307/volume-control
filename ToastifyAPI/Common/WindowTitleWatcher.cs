using System;
using System.Timers;
using ToastifyAPI.Events;
using NativeWindows = ToastifyAPI.Native.Windows;

namespace ToastifyAPI.Common
{
    public class WindowTitleWatcher : IDisposable
    {
        private readonly object currentTitleLock = new object();
        private readonly IntPtr hWnd;

        private int _checkInterval = 100;
        private Timer checkTimer;

        #region Public Properties

        public string CurrentTitle { get; private set; }

        public int CheckInterval
        {
            get { return this._checkInterval; }
            set
            {
                if (this._checkInterval == value)
                    return;

                bool attached = this.checkTimer?.Enabled == true;
                if (attached)
                    this.DetachTimer();

                this._checkInterval = value;

                if (attached)
                    this.AttachTimer();
            }
        }

        #endregion

        #region Events

        private event EventHandler<WindowTitleChangedEventArgs> _titleChanged;

        public event EventHandler<WindowTitleChangedEventArgs> TitleChanged
        {
            add
            {
                this._titleChanged += value;
                if (this.checkTimer == null)
                    this.AttachTimer();
            }
            remove
            {
                this._titleChanged -= value;
                if (this._titleChanged == null)
                    this.DetachTimer();
            }
        }

        #endregion

        public WindowTitleWatcher(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                throw new ArgumentException($"{nameof(hWnd)} is null", nameof(hWnd));

            this.hWnd = hWnd;
            this.CurrentTitle = NativeWindows.GetWindowTitle(this.hWnd);
        }

        private void AttachTimer()
        {
            this.DetachTimer();
            this.checkTimer = new Timer
            {
                AutoReset = true,
                Interval = this._checkInterval,
                Enabled = true
            };
            this.checkTimer.Elapsed += this.CheckTimer_Elapsed;
        }

        private void DetachTimer()
        {
            if (this.checkTimer != null)
            {
                this.checkTimer.Enabled = false;
                this.checkTimer.Dispose();
                this.checkTimer = null;
            }
        }

        private void CheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (this.currentTitleLock)
            {
                string oldTitle = this.CurrentTitle;
                string newTitle = NativeWindows.GetWindowTitle(this.hWnd);
                if (!string.Equals(oldTitle, newTitle, StringComparison.CurrentCulture))
                {
                    this.CurrentTitle = newTitle;
                    this.OnTitleChanged(oldTitle, newTitle);
                }
            }
        }

        private void OnTitleChanged(string oldTitle, string newTitle)
        {
            this._titleChanged?.Invoke(this, new WindowTitleChangedEventArgs(oldTitle, newTitle));
        }

        public void Dispose()
        {
            this.DetachTimer();
        }
    }
}