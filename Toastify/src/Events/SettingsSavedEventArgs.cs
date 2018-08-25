using System;
using System.Collections.Generic;
using Toastify.Model;

namespace Toastify.Events
{
    public class SettingsSavedEventArgs : EventArgs
    {
        #region Public Properties

        public Settings Settings { get; set; }
        public IReadOnlyList<GenericHotkeyProxy> PreviewHotkeys { get; set; }

        #endregion

        public SettingsSavedEventArgs(Settings settings, IReadOnlyList<GenericHotkeyProxy> previewHotkeys)
        {
            this.Settings = settings;
            this.PreviewHotkeys = previewHotkeys;
        }
    }
}