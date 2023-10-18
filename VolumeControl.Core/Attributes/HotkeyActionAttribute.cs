using System.Runtime.CompilerServices;

namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// Indicates that this is a hotkey action method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class HotkeyActionAttribute : Attribute
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="HotkeyActionAttribute"/> instance with a name specified by <paramref name="methodName"/>.
        /// </summary>
        /// <param name="methodName">The name of this hotkey action. If you do not provide one, the name of the method will be used automatically.</param>
        public HotkeyActionAttribute([CallerMemberName] string methodName = "")
        {
            Name = methodName;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets or sets the name of this hotkey action.
        /// </summary>
        /// <remarks>
        /// This defaults to the name of the method that this attribute is attached to, with spaces inserted between each word.
        /// To use this exact string without changing it, set UseExactName to <see langword="true"/>.
        /// </remarks>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets whether the Name property is modified by inserting spaces between unseparated words.
        /// </summary>
        public bool UseExactName { get; set; }
        /// <summary>
        /// Gets or sets the description string of this hotkey action.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Gets or sets the group name of this hotkey action, overriding whatever is specified by the action group.
        /// </summary>
        public string? GroupNameOverride { get; set; }
        /// <summary>
        /// Gets or sets the group color of this hotkey action, overriding whatever is specified by the action group.
        /// </summary>
        public string? GroupColorOverride { get; set; }
        #endregion Properties
    }
}
