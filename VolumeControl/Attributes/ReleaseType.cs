using AssemblyAttribute;

namespace VolumeControl.Attributes
{
    /// <summary>
    /// Release types, for use with version self-identification CI scripts with the <see cref="ReleaseType"/> assembly attribute.
    /// </summary>
    public enum ERelease : byte
    {
        /// <summary>
        /// Null type / Error Type.
        /// </summary>
        NONE,
        /// <summary>
        /// Testing build, not meant for release.
        /// </summary>
        TESTING,
        /// <summary>
        /// Pre-release (unfinished) version.
        /// </summary>
        PRERELEASE,
        /// <summary>
        /// Release candidate version.
        /// These are almost ready to be released, but haven't been extensively tested yet.
        /// </summary>
        CANDIDATE,
        /// <summary>
        /// A normal release.
        /// This does not include revisions!
        /// </summary>
        NORMAL,
        /// <summary>
        /// This is a slight modification to an existing release that doesn't qualify as a whole new version.
        /// </summary>
        REVISION,
    }

    /// <summary>
    /// Allows easier management of version self-identification for use with CI scripts.
    /// </summary>
    /// <remarks>
    /// To retrieve the release type, use the <see cref="Type"/> property.
    /// </remarks>
    public sealed class ReleaseType : BaseAssemblyAttribute
    {
        private readonly ERelease _releaseType;
        /// <summary>
        /// Gets the 'Release' type of the given assembly attribute.
        /// </summary>
        /// <returns>The release type currently set in the assembly.</returns>
        public ERelease Type => _releaseType;
        public ReleaseType(string value) : base(value)
        {
            var result = System.Enum.Parse(typeof(ERelease), Value, true);
            _releaseType = result != null ? (ERelease)result : ERelease.NONE;
        }
    }
}
