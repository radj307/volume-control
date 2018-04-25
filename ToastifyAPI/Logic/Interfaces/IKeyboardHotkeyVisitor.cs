using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Logic.Interfaces
{
    public interface IKeyboardHotkeyVisitor : IHotkeyVisitor
    {
        void Visit(IKeyboardHotkey hotkey);
    }
}