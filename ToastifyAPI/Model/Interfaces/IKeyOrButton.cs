using System;
using System.Windows.Input;
using MouseAction = ToastifyAPI.Core.MouseAction;

namespace ToastifyAPI.Model.Interfaces
{
    public interface IKeyOrButton : ICloneable
    {
        bool IsKey { get; }

        Key? Key { get; }

        MouseAction? MouseButton { get; }
    }
}