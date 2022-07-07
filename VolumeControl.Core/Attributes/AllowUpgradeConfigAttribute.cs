using AssemblyAttribute;

namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// This attribute can be attached to assemblies to indicate that any contained configuration files should be upgraded between versions when possible.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public sealed class AllowUpgradeConfigAttribute : BaseAssemblyAttribute
    {
        /// <summary>
        /// Creates a new <see cref="AllowUpgradeConfigAttribute"/> instance.
        /// </summary>
        /// <param name="value">This is passed automatically from the .csproj file.</param>
        public AllowUpgradeConfigAttribute(string value = "") : base(value) { }
        /// <summary>
        /// When <see langword="true"/>, the assembly that this attribute is attached to allows upgrading configurations between assembly versions.
        /// </summary>
        public bool AllowUpgrade => _allowUpgrade ??= this.Value.Equals("true", StringComparison.OrdinalIgnoreCase) || this.Value.Equals("1", StringComparison.OrdinalIgnoreCase);
        private bool? _allowUpgrade = null;
    }
}
