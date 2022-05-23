namespace VolumeControl.Core
{
    /// <summary>
    /// <see langword="abstract"/> base class for an object that manages an addon.
    /// </summary>
    public abstract class BaseAddon : IBaseAddon
    {
        /// <param name="attrType">The attribute type used by this addon.</param>
        protected BaseAddon(Type attrType) => Attribute = attrType;
        /// <inheritdoc/>
        public Type Attribute { get; set; }
        /// <inheritdoc/>
        public List<Type> Types { get; } = new();
        /// <inheritdoc/>
        public abstract void LoadFromTypes();
    }
}
