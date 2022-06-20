using Semver;

namespace VolumeControl.Core
{
    /// <summary>
    /// Represents a range of program versions, which allows checking whether or not a specific <see cref="SemVersion"/> is within the range.
    /// </summary>
    public struct VersionRange : IEquatable<SemVersion>
    {
        #region Properties
        /// <summary>
        /// Minimum range boundary <i>(Inclusive)</i>.<br/>
        /// When this is set to <see langword="null"/>, there is no minimum range boundary, which allows any versions released prior to <see cref="Min"/> to be considered valid.<br/>
        /// When both range boundaries are <see langword="null"/>, there are no version number restrictions.
        /// </summary>
        /// <remarks><b>Default: <see langword="null"/></b></remarks>
        public SemVersion? Min { get; set; }
        /// <summary>
        /// Maximum range boundary <i>(Inclusive)</i>.<br/>
        /// When this is set to <see langword="null"/>, there is no maximum range boundary, which allows any versions released after <see cref="Min"/> to be considered valid.<br/>
        /// When both range boundaries are <see langword="null"/>, there are no version number restrictions.
        /// </summary>
        /// <remarks><b>Default: <see langword="null"/></b></remarks>
        public SemVersion? Max { get; set; }
        /// <summary>
        /// Gets whether this <see cref="VersionRange"/> instance allows <i>any</i> version number to be considered within range or not.
        /// </summary>
        /// <returns><see langword="true"/> when both the <see cref="Min"/> &amp; <see cref="Max"/> properties are set to <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        public bool AllowsAny => Min is null && Max is null;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Checks if <paramref name="other"/> is within the range specified by this <see cref="VersionRange"/> instance.
        /// </summary>
        /// <param name="other">Version number to check.</param>
        /// <returns><see langword="true"/> when <paramref name="other"/> is within this range; otherwise <see langword="false"/></returns>
        public bool WithinRange(SemVersion other) => AllowsAny || ((Min is null || Min.CompareSortOrderTo(other) >= 0) && (Max is null || Max.CompareSortOrderTo(other) <= 0));
        /// <inheritdoc cref="WithinRange(SemVersion)"/>
        public bool Equals(SemVersion? other) => other is not null && WithinRange(other);
        #endregion Methods
    }
}