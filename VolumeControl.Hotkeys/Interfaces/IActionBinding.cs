using System.ComponentModel;
using System.Reflection;

namespace VolumeControl.Hotkeys.Interfaces
{
    /// <summary>
    /// Represents an action binding object, which is a container for reflective method information used to call the actual target.
    /// </summary>
    public interface IActionBinding
    {
        /// <summary>
        /// The display name of this action.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// This is the event handler for this action binding. It calls <see cref="MethodBase.Invoke(object?, object?[]?)"/> on the bound method.
        /// </summary>
        void HandleKeyEvent(object? sender, HandledEventArgs? e);
    }
}
