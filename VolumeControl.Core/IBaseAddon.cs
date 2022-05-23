namespace VolumeControl.Core
{
    /// <summary>Represents an addon object.<br/><br/>You can't directly implement this interface; inherit from the <see langword="abstract"/> <see langword="class"/> <see cref="BaseAddon"/> instead.</summary>
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
