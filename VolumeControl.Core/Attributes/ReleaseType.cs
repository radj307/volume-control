using VolumeControl.Core.Enum;

namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// Allows easier management of version self-identification for use with CI scripts.
    /// </summary>
    /// <remarks>
    /// To retrieve the release type, use the <see cref="Type"/> property.
    /// </remarks>
    public sealed class ReleaseType : AssemblyAttribute
    {
        private readonly Release _releaseType;
        /// <summary>
        /// Gets the 'Release' type of the given assembly attribute.
        /// </summary>
        /// <returns>The release type currently set in the assembly.</returns>
        public Release Type => _releaseType;
        public ReleaseType(string value) : base(value)
        {
            var result = System.Enum.Parse(typeof(Release), String, true);
            _releaseType = result != null ? (Release)result : Release.NONE;
        }
    }
}
