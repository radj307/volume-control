namespace VolumeControl.Core
{
    /// <summary>
    /// Represents an addon attribute of any type.
    /// </summary>
    public interface IBaseAddonAttribute
    {
        /// <summary>This is used when indexing this addon, as well as in the log.</summary>
        string AddonName { get; }
        /// <inheritdoc cref="CompatibleVersions"/>
        /// <remarks>This defines a range of versions that are considered compatible.</remarks>
        CompatibleVersions CompatibleVersions { get; }
    }
}