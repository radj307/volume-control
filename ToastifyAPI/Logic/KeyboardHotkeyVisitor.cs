using ToastifyAPI.Logic.Interfaces;
using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Logic
{
    /// <summary>
    ///     Implements the Visitor pattern for the <see cref="IKeyboardHotkey" /> class.
    /// </summary>
    public class KeyboardHotkeyVisitor : IKeyboardHotkeyVisitor
    {
        /// <inheritdoc />
        public void Visit(IKeyboardHotkey hotkey)
        {
            hotkey?.PerformAction();
        }
    }
}