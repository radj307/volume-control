namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// Indicates that the associated class contains hotkey action methods marked with <see cref="HotkeyActionAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class HotkeyActionGroupAttribute : Attribute
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="HotkeyActionGroupAttribute"/> instance with a group name specified by <paramref name="groupName"/>.
        /// </summary>
        /// <param name="groupName">The name of this hotkey action group.</param>
        public HotkeyActionGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets or sets the name of this hotkey action group.
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// Gets or sets the color of this hotkey action group, as a hexadecimal color code.
        /// </summary>
        public string? GroupColor { get; set; }
        #endregion Properties
    }
}
