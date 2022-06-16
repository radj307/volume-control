using AssemblyAttribute;
using System;
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
        private readonly ERelease _releaseType;
        /// <summary>
        /// Gets the 'Release' type of the given assembly attribute.
        /// </summary>
        /// <returns>The release type currently set in the assembly.</returns>
        public ERelease Type => _releaseType;
        public ReleaseType(string value) : base(value)
        {
            object? result = System.Enum.Parse(typeof(ERelease), Value, true);
            _releaseType = result != null ? (ERelease)result : ERelease.NONE;
        }
    }
}
