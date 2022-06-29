using System.ComponentModel;
using System.Windows.Forms;

namespace HotkeyLib
{
    /// <summary>
    /// Represents a combination of one keyboard key and any number of modifier keys.
    /// </summary>
    public interface IKeyCombo : INotifyPropertyChanged
    {
        #region Properties
        /// <summary>
        /// The primary key on the Keyboard.
        /// </summary>
        Keys Key { get; set; }
        /// <summary>
        /// The modifier key bitfield.
        /// </summary>
        Modifier Mod { get; set; }
        /// <summary>
        /// Get or set whether the Alt modifier key is enabled.
        /// This corresponds to the first bit in the modifier key bitfield.
        /// Both the left and right modifier keys apply here, there is no method available to limit the key combination to only the left or right modifier key.
        /// </summary>
        bool Alt { get; set; }
        /// <summary>
        /// Get or set whether the Ctrl modifier key is enabled.
        /// This corresponds to the second bit in the modifier key bitfield.
        /// Both the left and right modifier keys apply here, there is no method available to limit the key combination to only the left or right modifier key.
        /// </summary>
        bool Ctrl { get; set; }
        /// <summary>
        /// Get or set whether the Shift modifier key is enabled.
        /// This corresponds to the third bit in the modifier key bitfield.
        /// Both the left and right modifier keys apply here, there is no method available to limit the key combination to only the left or right modifier key.
        /// </summary>
        bool Shift { get; set; }
        /// <summary>
        /// Get or set whether the "Windows Key" (super) modifier key is enabled.
        /// This corresponds to the fourth bit in the modifier key bitfield.
        /// Both the left and right modifier keys apply here, there is no method available to limit the key combination to only the left or right modifier key.
        /// </summary>
        bool Win { get; set; }
        /// <summary>
        /// Checks if this KeyCombo instance contains a valid keyboard key combination.
        /// </summary>
        /// <returns><list type="table">
        /// <item><term>true</term><description>This KeyCombo instance is valid, and may be registered with the WIN32 API.</description></item>
        /// <item><term>false</term><description>This KeyCombo instance is invalid and cannot be registered.</description></item>
        /// </list></returns>
        bool Valid { get; }
        #endregion Properties
    }
}
