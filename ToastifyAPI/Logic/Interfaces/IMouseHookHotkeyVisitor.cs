using JetBrains.Annotations;
using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Logic.Interfaces
{
    public interface IMouseHookHotkeyVisitor : IHotkeyVisitor
    {
        void Visit(IMouseHookHotkey hotkey);

        bool IsRegistered([NotNull] IMouseHookHotkey hotkey);
        void RegisterHook([NotNull] IMouseHookHotkey hotkey);
        void UnregisterHook([NotNull] IMouseHookHotkey hotkey);
        void UnregisterAll();
    }
}