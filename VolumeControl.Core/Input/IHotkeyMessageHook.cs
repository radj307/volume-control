using System.Windows.Interop;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// Represents an object that can be added to the hotkey message hook system.
    /// </summary>
    public interface IHotkeyMessageHook
    {
        /// <summary>
        /// Gets the windows message hook method.
        /// </summary>
        HwndSourceHook MessageHook { get; }
    }
}
