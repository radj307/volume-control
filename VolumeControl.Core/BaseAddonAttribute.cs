namespace VolumeControl.Core
{
    /// <summary>Abstract base class that all other addon attributes inherit from.<br/><b>Unless you're implementing a new addon type, this isn't the attribute you're looking for.</b></summary>
    /// <remarks>An addon type for the Volume Control program.</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public abstract class BaseAddonAttribute : Attribute, IBaseAddonAttribute
    {
        /// <inheritdoc cref="BaseAddonAttribute"/>
        /// <param name="addonName">The string to set the <see cref="AddonName"/> property to.</param>
        /// <param name="compatibleVersions">A <see cref="VersionRange"/> object that specifies which versions of Volume Control this addon supports.</param>
        protected BaseAddonAttribute(string addonName, VersionRange compatibleVersions)
        {
            this.AddonName = addonName;
            this.CompatibleVersions = compatibleVersions;
        }
        /// <inheritdoc cref="BaseAddonAttribute"/>
        /// <param name="addonName">The string to set the <see cref="AddonName"/> property to.</param>
        protected BaseAddonAttribute(string addonName)
        {
            this.AddonName = addonName;
            this.CompatibleVersions = new()
            {
                Min = null,
                Max = null
            };
        }
        /// <inheritdoc/>
        public string AddonName { get; }
        /// <inheritdoc/> 
        public VersionRange CompatibleVersions { get; }
    }
}