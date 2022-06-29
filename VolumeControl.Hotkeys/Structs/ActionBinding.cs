using System.ComponentModel;
using System.Reflection;
using VolumeControl.Core.Keyboard.Actions;

namespace VolumeControl.Hotkeys.Structs
{
    /// <summary>
    /// Binding endpoint object that uses the <see cref="MethodInfo"/> object to invoke a method using reflection.
    /// </summary>
    public struct ActionBinding : IActionBinding
    {
        /// <param name="mInfo">A MethodInfo object representing the target method.</param>
        /// <param name="handlerObj">The class instance that contains the method.</param>
        /// <param name="actionData">The <see cref="HotkeyActionData"/> belonging to the target method.</param>
        public ActionBinding(MethodInfo mInfo, object? handlerObj, HotkeyActionData actionData)
        {
            Data = actionData;
            MethodInfo = mInfo;
            HandlerObject = handlerObj;
        }

        /// <inheritdoc/>
        public HotkeyActionData Data { get; set; }
        /// <inheritdoc/>
        public string Identifier => $"{(Data.ActionGroup is null ? "" : $"{Data.ActionGroup}:")}{Data.ActionName}";

        /// <summary>
        /// Stores reflection information about a method.<br/>
        /// This is used to determine which method to target.
        /// </summary>
        private readonly MethodInfo MethodInfo;
        /// <summary>
        /// This is the class instance whose method (determined by <see cref="MethodInfo"/>) is called in <see cref="HandleKeyEvent(object?, HandledEventArgs?)"/>.<br/>
        /// This is ignored if the method is static.
        /// </summary>
        private readonly object? HandlerObject;

        /// <inheritdoc/>
        public void HandleKeyEvent(object? sender, HandledEventArgs? e)
        {
            if (HandlerObject != null || MethodInfo.IsStatic)
                MethodInfo.Invoke(HandlerObject, new[] { sender, e });
        }
    }
}
