namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// Used to access the current git tag in combination with the 'SetVersion.ps1' CI script.
    /// See the documentation for 'AssemblyAttribute' for details on how to implement this attribute.
    /// </summary>
    public sealed class ExtendedVersionAttribute : AssemblyAttribute
    {
        /// <summary>
        /// The current value of the ExtendedVersion Assembly property.
        /// </summary>
        public string ExtendedVersion => String;
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="extver">The extended version string.</param>
        public ExtendedVersionAttribute(string extver) : base(extver) { }
    }
}
