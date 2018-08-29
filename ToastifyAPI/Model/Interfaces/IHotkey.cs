using System;
using System.Windows.Input;
using ToastifyAPI.Interop.Interfaces;
using ToastifyAPI.Logic.Interfaces;

namespace ToastifyAPI.Model.Interfaces
{
    /// <summary>
    ///     Defines a hotkey that can perform an action and provides methods to check its internal validity, to check if the required keyboard modifiers are
    ///     pressed and to activate or deactivate the hotkey itself.
    /// </summary>
    public interface IHotkey : IActionable, ICloneable, IDisposable
    {
        #region Public Properties

        IInputDevices InputDevices { get; set; }

        bool Enabled { get; set; }
        bool Active { get; }
        ModifierKeys Modifiers { get; set; }

        #endregion

        bool IsValid();

        bool AreModifiersPressed();

        void Activate();

        void Deactivate();

        void Dispatch(IHotkeyVisitor visitor);
    }
}