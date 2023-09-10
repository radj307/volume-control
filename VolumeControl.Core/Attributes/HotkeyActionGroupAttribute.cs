using System.Runtime.CompilerServices;

namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// Indicates that a class contains hotkey action definitions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class HotkeyActionGroupAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="HotkeyActionGroupAttribute"/> instance.
        /// </summary>
        /// <param name="groupName">The default group name of all hotkey methods within the class that this attribute is applied to.<br/>This can be overridden on a per-method basis with the <see cref="HotkeyActionAttribute"/> attribute.<br/><br/>By default, this is automatically filled in with the name of the class it is applied to.</param>
        public HotkeyActionGroupAttribute([CallerMemberName] string groupName = "") => GroupName = groupName;

        /// <summary>
        /// Gets or sets the default sorting group of hotkey actions defined within the attached class object.
        /// </summary>
        /// <remarks>
        /// The <see cref="HotkeyActionAttribute.GroupName"/> property will override this one, if set.
        /// </remarks>
        public string? GroupName { get; set; }
        /// <summary>
        /// Gets or sets the default color of the <see cref="GroupName"/> string when it is displayed.
        /// </summary>
        /// <remarks>
        /// The <see cref="HotkeyActionAttribute.GroupColor"/> property will override this one, if set.
        /// </remarks>
        public string? GroupColor { get; set; }
    }
}
