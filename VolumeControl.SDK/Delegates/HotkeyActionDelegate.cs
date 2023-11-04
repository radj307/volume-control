using VolumeControl.Core.Input;

namespace VolumeControl.SDK.Delegates
{
    /// <summary>
    /// Represents a method that can be used by Volume Control to generate a hotkey action.
    /// </summary>
    /// <param name="sender">The hotkey that was pressed.</param>
    /// <param name="e">The event arguments for this action, including the current values of any action settings.</param>
    public delegate void HotkeyActionDelegate(object sender, HotkeyPressedEventArgs e);
}
