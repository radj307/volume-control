using System;
using Toastify.Model;

namespace Toastify.Events
{
    public class CurrentSettingsChangedEventArgs : EventArgs
    {
        #region Public Properties

        public Settings PreviousSettings { get; }
        public Settings CurrentSettings { get; }

        #endregion

        public CurrentSettingsChangedEventArgs(Settings previousSettings, Settings currentSettings)
        {
            this.PreviousSettings = previousSettings;
            this.CurrentSettings = currentSettings;
        }
    }
}