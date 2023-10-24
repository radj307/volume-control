using System.Windows.Interop;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Core.Input.Enums;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// Represents a Win32 global hotkey.
    /// </summary>
    public interface IHotkey
    {
        #region Properties
        /// <summary>
        /// Gets or sets the friendly name of this hotkey.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Gets the ID number of this hotkey.
        /// </summary>
        int ID { get; }
        /// <summary>
        /// Gets or sets the main key for this hotkey.
        /// </summary>
        EFriendlyKey Key { get; set; }
        /// <summary>
        /// Gets or sets the modifier keys for this hotkey.
        /// </summary>
        EModifierKey Modifiers { get; set; }
        /// <summary>
        /// Gets or sets whether the Alt modifier key is enabled.
        /// </summary>
        bool Alt { get; set; }
        /// <summary>
        /// Gets or sets whether the Shift modifier key is enabled.
        /// </summary>
        bool Shift { get; set; }
        /// <summary>
        /// Gets or sets whether the Ctrl modifier key is enabled.
        /// </summary>
        bool Ctrl { get; set; }
        /// <summary>
        /// Gets or sets whether the Win modifier key is enabled.
        /// </summary>
        bool Win { get; set; }
        /// <summary>
        /// Gets or sets whether NoRepeat is enabled.
        /// </summary>
        bool NoRepeat { get; set; }
        /// <summary>
        /// Gets the windows message hook for this hotkey.
        /// </summary>
        HwndSourceHook MessageHook { get; }
        /// <summary>
        /// Gets or sets whether this hotkey is registered with the Win32 API.
        /// </summary>
        /// <returns><see langword="true"/> when the hotkey is enabled; otherwise <see langword="false"/>.</returns>
        bool IsRegistered { get; set; }
        /// <summary>
        /// Gets or sets the action that this hotkey triggers when pressed.
        /// </summary>
        HotkeyActionInstance? Action { get; set; }
        #endregion Properties

        #region Events
        /// <summary>
        /// Occurs when the hotkey combination was pressed.
        /// </summary>
        event HotkeyPressedEventHandler? Pressed;
        #endregion Events
    }
}
