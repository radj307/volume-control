using PropertyChanged;

namespace VolumeControl.Core.Helpers
{
    /// <summary>
    /// Viewmodel object for a single target override entry.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class TargetOverrideVM
    {
        /// <summary>
        /// Creates a new <see cref="TargetOverrideVM"/> instance with an empty <see cref="Value"/>.
        /// </summary>
        public TargetOverrideVM() => _value = string.Empty;
        /// <summary>
        /// Creates a new <see cref="TargetOverrideVM"/> instance with the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The name of the target process.</param>
        public TargetOverrideVM(string value) => _value = value;

        /// <summary>
        /// Target value.
        /// </summary>
        public string Value
        {
            get => _value;
            set => _value = value.Trim();
        }
        private string _value;
    }
}
