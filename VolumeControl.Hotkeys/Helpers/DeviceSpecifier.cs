using VolumeControl.Core.Input.Actions;
using VolumeControl.SDK;

namespace VolumeControl.Hotkeys.Helpers
{
    /// <summary>
    /// Used to specify <see cref="Audio.AudioDevice"/> targets for hotkey actions.
    /// </summary>
    public class DeviceSpecifier : ActionTargetSpecifier
    {
        public override void AddNewTarget()
        {
            // TODO: Reimplement
            //if (VCAPI.Default.AudioDeviceManager.DefaultDevice?.DeviceID is string deviceID && !Targets.Any(t => t.Value.Equals(deviceID, StringComparison.OrdinalIgnoreCase)))
            //{
            //    Targets.Add(new()
            //    {
            //        Value = deviceID
            //    });
            //}
            //else Targets.Add(new());
        }
    }
}
