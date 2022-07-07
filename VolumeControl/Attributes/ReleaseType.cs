using AssemblyAttribute;
using VolumeControl.Core.Enum;

namespace VolumeControl.Attributes
{
    /// <summary>
    /// Allows easier management of version self-identification for use with CI scripts.
    /// </summary>
    /// <remarks>
    /// To retrieve the release type, use the <see cref="Type"/> property.
    /// </remarks>
    public sealed class ReleaseType : BaseAssemblyAttribute
    {
        /// <summary>
        /// Gets the 'Release' type of the given assembly attribute.
        /// </summary>
        /// <returns>The release type currently set in the assembly.</returns>
        public ERelease Type { get; }
        public ReleaseType(string value) : base(value)
        {
            object? result = System.Enum.Parse(typeof(ERelease), this.Value, true);
            this.Type = result != null ? (ERelease)result : ERelease.NONE;
        }
    }
}
