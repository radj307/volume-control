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
        /// <see langword="true"/> when the function pointed to by <see cref="MethodInfo"/> requires extra parameters.
        /// </summary>
        bool RequiresExtraParameters { get; }
        /// <summary>
        /// <see langword="true"/> when the function pointed to by <see cref="MethodInfo"/> supports receiving extra action settings via <see cref="HotkeyActionPressedEventArgs"/>; otherwise <see langword="false"/>.
        /// </summary>
        bool AcceptsExtraActionSettings { get; }
        /// <summary>
        /// Defines additional action settings that are not received via parameters, but should be considered valid nonetheless.<br/>
        /// In practice, this defines the action settings that were specified via method attributes.
        /// </summary>
        HotkeyActionSetting[]? ExtraActionSettings { get; }
        /// <summary>
        /// <see langword="true"/> when this action uses action settings from any source; otherwise <see langword="false"/>.
        /// </summary>
        public bool UsesActionSettings { get; }

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

        /// <summary>
        /// Handles key press events, and executes the action that is associated with this hotkey instance.
        /// </summary>
        /// <param name="sender">The hotkey that was pressed.</param>
        /// <param name="e">The <see cref="HotkeyActionPressedEventArgs"/> event arguments object.<br/>If the action method that you're calling uses extra non-default parameters, the <see cref="HotkeyActionPressedEventArgs.ActionSettings"/> list <b>MUST</b> be correctly filled in, otherwise the action method <b>cannot be called!</b></param>
        void HandleKeyEvent(object? sender, HotkeyActionPressedEventArgs e);
        /// <summary>
        /// Gets the list of extra parameters required to call this action's associated method.
        /// </summary>
        /// <returns>A list of <see cref="HotkeyActionSetting"/> objects with their default <see cref="HotkeyActionSetting.Value"/> property values.</returns>
        HotkeyActionSetting[] GetDefaultActionSettings();
    }
}
