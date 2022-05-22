using Semver;

namespace VolumeControl.Core
{
    /// <summary>
    /// Represents an addon attribute of any type.
    /// </summary>
    public interface IBaseAddonAttribute
    {
        string AddonName { get; }
        SemVersion? MinimumVersion { get; set; }
        SemVersion? MaximumVersion { get; set; }
        public bool CanLoadAddon(SemVersion version) => ((MinimumVersion == null || version >= MinimumVersion) && (MaximumVersion == null || version <= MaximumVersion));
    }
}