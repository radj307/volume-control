using VolumeControl.Core.Helpers;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Core
{
    /// <summary>
    /// Specifies the target(s) of a hotkey action.
    /// </summary>
    public class ActionTargetSpecifier
    {
        /// <summary>
        /// List of targets.
        /// </summary>
        public ObservableImmutableList<TargetOverrideVM> Targets { get; } = new();
    }
}
