namespace VolumeControl.Core
{
    /// <summary>
    /// Represents the attribute(s) associated with a <see cref="IBaseAddon"/>.<br/>
    /// All valid addon attributes implement this interface.
    /// </summary>
    /// <remarks><b>Note that this is used by Volume Control internally, and should not be used directly from within addons.</b></remarks>
    public interface IBaseAddonAttribute
    {
        /// <summary>This is used when indexing this addon, as well as in the log.</summary>
        string AddonName { get; }
        /// <inheritdoc cref="VersionRange"/>
        /// <remarks>This defines a range of versions that are considered compatible.</remarks>
        VersionRange CompatibleVersions { get; }
    }
}