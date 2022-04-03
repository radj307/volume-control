namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// Pre-release version attribute for use with CI scripts.
    /// </summary>
    public sealed class IsPreReleaseAttribute : AssemblyAttribute
    {
        /// <summary>
        /// True when this is a pre-release build.
        /// </summary>
        public bool IsPreRelease => String.Equals("true", StringComparison.OrdinalIgnoreCase);
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="isPreRelease">The assembly attribute as a string.</param>
        public IsPreReleaseAttribute(string isPreRelease) : base(isPreRelease) { }
    }
}
