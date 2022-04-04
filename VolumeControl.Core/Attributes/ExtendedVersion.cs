namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// Used to access the current git tag in combination with the 'SetVersion.ps1' CI script.
    /// See the documentation for 'AssemblyAttribute' for details on how to implement this attribute.
    /// </summary>
    public sealed class ExtendedVersion : AssemblyAttribute
    {
        /// <summary>
        /// The current value of the ExtendedVersion Assembly property.
        /// </summary>
        public string Version => String;
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="extver">The extended version string.</param>
        public ExtendedVersion(string extver) : base(extver) { }
    }
}
