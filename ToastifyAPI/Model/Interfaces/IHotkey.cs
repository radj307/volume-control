using System;
using System.Windows.Input;
using ToastifyAPI.Interop.Interfaces;
using ToastifyAPI.Logic.Interfaces;

namespace ToastifyAPI.Model.Interfaces
{
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