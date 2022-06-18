using System.Windows.Media;
using VolumeControl.Hotkeys.Attributes;

namespace VolumeControl.Hotkeys.Structs
{
    /// <summary>
    /// Runtime container for hotkey action metadata.<br/>
    /// Instances of this struct cannot be constructed externally; they must be retrieved through the <see cref="HotkeyActionAttribute.GetActionData"/> method.
    /// </summary>
    public struct HotkeyActionData
    {
        #region Constructor
        internal HotkeyActionData(string actionName, string? actionDescription, string? actionGroup, Brush? actionGroupBrush = null)
        {
            ActionName = actionName;
            ActionDescription = actionDescription;
            ActionGroup = actionGroup;
            ActionGroupBrush = actionGroupBrush;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// The name of the hotkey action.
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// The description tooltip of the hotkey action.
        /// </summary>
        public string? ActionDescription { get; set; }
        /// <summary>
        /// The group that this action belongs to.
        /// </summary>
        public string? ActionGroup { get; set; }
        /// <summary>
        /// The color to use when printing the group name.
        /// </summary>
        public Brush? ActionGroupBrush { get; set; }
        #endregion Properties
    }
}
