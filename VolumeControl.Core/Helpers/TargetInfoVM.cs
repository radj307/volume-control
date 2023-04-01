using PropertyChanged;

namespace VolumeControl.Core.Helpers
{
    [AddINotifyPropertyChangedInterface]
    public class TargetInfoVM
    {
        /// <summary>
        /// Creates a new <see cref="TargetInfoVM"/> instance with an empty <see cref="Value"/>.
        /// </summary>
        public TargetInfoVM() => _value = string.Empty;
        /// <summary>
        /// Creates a new <see cref="TargetInfoVM"/> instance with the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The name of the target process.</param>
        public TargetInfoVM(string value) => _value = value;

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
