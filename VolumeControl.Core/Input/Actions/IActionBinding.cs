using System.ComponentModel;
using System.Reflection;

namespace VolumeControl.Core.Input.Actions
{
    /// <summary>
    /// Represents an action binding object, which is a container for reflective method information used to call the actual target.
    /// </summary>
    public interface IActionBinding
    {
        /// <summary>
        /// <see cref="HotkeyActionData"/> associated with this action.
        /// </summary>
        HotkeyActionData Data { get; set; }
        /// <summary>
        /// The string identifier used in the save/load system.
        /// </summary>
        string Identifier { get; }
        /// <summary>
        /// This is the event handler for this action binding. It calls <see cref="MethodBase.Invoke(object?, object?[])"/> on the bound method.
        /// </summary>
        void HandleKeyEvent(object? sender, HandledEventArgs? e);
    }
}
