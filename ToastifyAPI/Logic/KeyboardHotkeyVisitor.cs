using ToastifyAPI.Logic.Interfaces;
using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Logic
{
    public class KeyboardHotkeyVisitor : IKeyboardHotkeyVisitor
    {
        /// <inheritdoc />
        public void Visit(IKeyboardHotkey hotkey)
        {
            hotkey?.PerformAction();
        }
    }
}