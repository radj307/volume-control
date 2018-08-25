using System;
using Toastify.Model;
using Toastify.ViewModel;

namespace Toastify.Events
{
    public class SettingsViewLaunchedEventArgs : EventArgs
    {
        #region Public Properties

        public Settings Settings { get; set; }
        public ISettingsViewModel SettingsViewModel { get; set; }

        #endregion

        public SettingsViewLaunchedEventArgs(Settings settings, ISettingsViewModel settingsViewModel)
        {
            this.Settings = settings;
            this.SettingsViewModel = settingsViewModel;
        }
    }
}