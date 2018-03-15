using System;
using Toastify.Model;

namespace Toastify.Events
{
    public class SettingsSavedEventArgs : EventArgs
    {
        public Settings Settings { get; set; }

        public SettingsSavedEventArgs(Settings settings)
        {
            this.Settings = settings;
        }
    }
}