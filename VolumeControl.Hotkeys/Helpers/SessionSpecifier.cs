using VolumeControl.Core.Helpers;
using VolumeControl.Core.Input.Actions;
using VolumeControl.SDK;

namespace VolumeControl.Hotkeys.Helpers
{
    /// <summary>
    /// Used to specify <see cref="Audio.AudioSession"/> targets for hotkey actions.
    /// </summary>
    public class SessionSpecifier : ActionTargetSpecifier
    {
        public override void AddNewTarget()
        {
            if (VCAPI.Default.AudioAPI.SelectedSession?.ProcessName is string processName && !Targets.Any(t => t.Value.Equals(processName, StringComparison.OrdinalIgnoreCase)))
            {
                Targets.Add(new TargetInfoVM()
                {
                    Value = processName
                });
            }
            else Targets.Add(new());
        }
    }
}
