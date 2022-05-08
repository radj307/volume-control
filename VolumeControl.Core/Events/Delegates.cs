using AudioAPI.Interfaces;

namespace VolumeControl.Core.Events
{
    public delegate void DeviceSwitchEventHandler(object sender, SwitchEventArgs<IAudioDevice> e);
    public delegate void SessionSwitchEventHandler(object sender, SwitchEventArgs<IProcess> e);
    public delegate void VolumeEventHandler(object sender, VolumeEventArgs e);
}
