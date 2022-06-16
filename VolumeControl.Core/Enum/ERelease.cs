namespace VolumeControl.Core.Enum
{
    /// <summary>
    /// Release type definitions, for use with version self-identification CI scripts using assembly attributes.
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
}
