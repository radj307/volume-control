using System.Collections.Generic;
using Toastify.Model;

namespace Toastify.ViewModel
{
    public interface ISettingsViewModel
    {
        #region Public Properties

        IReadOnlyList<GenericHotkeyProxy> Hotkeys { get; }

        #endregion
    }
}