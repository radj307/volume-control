namespace VolumeControl.Core
{
    /// <summary>
    /// Represents any type of Volume Control addon definition object.<br/>
    /// This interface is implemented by the <see cref="BaseAddon"/> <see langword="abstract"/> class.
    /// </summary>
    /// <remarks><b>Note that this is used by Volume Control internally, and should not be used directly from within addons.</b></remarks>
    public interface IBaseAddon
    {
        /// <summary>The attribute type used by this addon.</summary>
        Type Attribute { get; set; }
        /// <summary>A list of every type belonging to this addon manager instance.</summary>
        List<Type> Types { get; }
        /// <summary>
        /// Performs initialization using the <see cref="Types"/> property.
        /// </summary>
        /// <remarks>This is called by the addon manager once all types have been loaded.</remarks>
        void LoadFromTypes();
    }
}
