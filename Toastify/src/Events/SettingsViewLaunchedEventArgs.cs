using System;
using Toastify.Model;

namespace Toastify.Events
{
    public class SettingsViewLaunchedEventArgs : EventArgs
    {
        public Settings Settings { get; set; }

        public SettingsViewLaunchedEventArgs(Settings settings)
        {
            this.Settings = settings;
        }
    }
}