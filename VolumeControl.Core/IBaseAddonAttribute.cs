using Semver;

namespace VolumeControl.Core
{
    /// <summary>
    /// Represents an addon attribute of any type.
    /// </summary>
    public interface IBaseAddonAttribute : IComparable<SemVersion>
    {
        string AddonName { get; }
        SemVersion? MinimumVersion { get; set; }
        SemVersion? MaximumVersion { get; set; }

        /// <summary>Check if this addon can be loaded by checking its version requirements.</summary>
        /// <param name="attr">Any type derived from <see cref="BaseAddonAttribute"/></param>
        /// <param name="version">The current Volume Control version number.</param>
        /// <returns>True when the addon should be loaded, otherwise false.</returns>
        public bool CanLoadAddon(SemVersion version) => CompareTo(version) == 0;
    }
}