using System.ComponentModel;
using System.Reflection;

namespace VolumeControl.Core.Input.Actions
{
    /// <summary>
    /// Represents an invokable method that can be triggered via a hotkey.
    /// </summary>
    public interface IHotkeyAction
    {
        /// <summary>
        /// Gets the <see cref="System.Type"/> that implements the handler method.
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// Gets the <see cref="System.Reflection.MethodInfo"/> object for the handler method to be invoked when this action is triggered.
        /// </summary>
        MethodInfo MethodInfo { get; }
        /// <summary>
        /// The object instance that owns the method pointed to by <see cref="MethodInfo"/>.<br/>
        /// If the <see cref="MethodInfo"/> points to a <see langword="static"/> method, this is <see langword="null"/>.
        /// </summary>
        object? Instance { get; }
        /// <inheritdoc cref="HotkeyActionData"/>
        HotkeyActionData Data { get; set; }

        /// <summary>
        /// The action's unique identifier.
        /// </summary>
        string Identifier => $"{(Data.ActionGroup is null ? "" : $"{Data.ActionGroup}:")}{Data.ActionName}";
        /// <inheritdoc cref="HotkeyActionData.ActionName"/>
        string Name => Data.ActionName;
        /// <inheritdoc cref="HotkeyActionData.ActionDescription"/>
        string? Description => Data.ActionDescription;
        /// <inheritdoc cref="HotkeyActionData.ActionGroup"/>
        string? GroupName => Data.ActionGroup;
        /// <inheritdoc cref="HotkeyActionData.ActionGroupBrush"/>
        System.Windows.Media.Brush? GroupBrush => Data.ActionGroupBrush;

        /// <inheritdoc cref="HandledEventHandler"/>
        void HandleKeyEvent(object? sender, HandledEventArgs e);
    }
}
