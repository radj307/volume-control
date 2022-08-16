using System.Windows.Media;

namespace VolumeControl.Core.Input.Actions
{
    /// <summary>
    /// Runtime container for hotkey action metadata.<br/>
    /// </summary>
    public struct HotkeyActionData
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="HotkeyActionData"/> instance.
        /// </summary>
        public HotkeyActionData(string actionName, string? actionDescription, string? actionGroup, Brush? actionGroupBrush = null)
        {
            this.ActionName = actionName;
            this.ActionDescription = actionDescription;
            this.ActionGroup = actionGroup;
            this.ActionGroupBrush = actionGroupBrush;
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
