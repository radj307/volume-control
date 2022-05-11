namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// Pre-release version attribute for use with CI scripts.
    /// </summary>
    /// <remarks>Use the <see cref="IsPreRelease.PreRelease"/> property to access the result as a boolean.</remarks>
    public sealed class IsPreRelease : AssemblyAttribute
    {
        private readonly bool _isPreRelease;
        /// <summary>
        /// True when this is a pre-release build.
        /// The result of expression: <code>String.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase)</code> where 'String' is the 'isPreRelease' constructor parameter.
        /// </summary>
        public bool PreRelease => _isPreRelease;
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="isPreRelease">The assembly attribute as a string.</param>
        public IsPreRelease(string isPreRelease) : base(isPreRelease)
        {
            _isPreRelease = String.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);
        }
    }
}
