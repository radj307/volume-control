using System.Net.NetworkInformation;
using VolumeControl.Core.Input;
using VolumeControl.Core.Interfaces;
using VolumeControl.Hotkeys.Interfaces;

namespace VolumeControl.Hotkeys
{
    /// <summary>
    /// Extends the <see cref="BindableHotkey"/> and <see cref="BindableHotkeyJsonWrapper"/> objects with methods for converting between them.
    /// </summary>
    public static class BindableHotkeyConverterExtensions
    {
        /// <summary>
        /// Converts from a <see cref="BindableHotkeyJsonWrapper"/> to a <see cref="BindableHotkey"/> object for deserialization.
        /// </summary>
        /// <param name="jsonWrapper">The bindable hotkey JSON wrapper object to convert.</param>
        /// <param name="actionManager">The hotkey actions manager to use when parsing the action identifier.</param>
        /// <returns><see cref="BindableHotkey"/></returns>
        public static BindableHotkey ToBindableHotkey(this BindableHotkeyJsonWrapper jsonWrapper, IHotkeyActionManager actionManager) => new()
        {
            Name = jsonWrapper.Name,
            Registered = jsonWrapper.Registered,
            Key = jsonWrapper.Key,
            Modifier = jsonWrapper.Modifier,
            Action = jsonWrapper.ActionIdentifier is null ? null : actionManager[jsonWrapper.ActionIdentifier],
            ActionSettings = jsonWrapper.ActionSettings is null ? new() : new(jsonWrapper.ActionSettings),
        };
        /// <summary>
        /// Converts from a <see cref="IBindableHotkey"/> to a <see cref="BindableHotkeyJsonWrapper"/> object for improved serialization.
        /// </summary>
        /// <param name="bindableHotkey">The bindable hotkey to convert.</param>
        /// <returns><see cref="BindableHotkeyJsonWrapper"/></returns>
        public static BindableHotkeyJsonWrapper ToBindableHotkeyJsonWrapper(this IBindableHotkey bindableHotkey) => new()
        {
            Name = bindableHotkey.Name,
            Registered = bindableHotkey.Registered,
            Key = bindableHotkey.Key,
            Modifier = bindableHotkey.Modifier,
            ActionIdentifier = bindableHotkey.Action?.Identifier,
            ActionSettings = bindableHotkey.ActionSettings?.ToList(),
        };
    }
}
