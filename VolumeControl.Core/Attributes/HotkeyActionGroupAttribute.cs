using System.Runtime.CompilerServices;
using VolumeControl.Core;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// <b>(Optional)</b> Marks the attached class type as a Hotkey Actions addon type.
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
        /// A default group name to apply to all methods found within the class that this attribute is applied to.
        /// </summary>
        public string? GroupName { get; set; }
        /// <summary>
        /// A default group color to apply to all methods found within the class that this attribute is applied to.
        /// </summary>
        public string? GroupColor { get; set; }
    }
}
