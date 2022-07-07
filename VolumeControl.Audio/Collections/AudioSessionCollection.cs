using NAudio.CoreAudioApi;
using VolumeControl.TypeExtensions;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Audio.Collections
{
    /// <summary>
    /// Management container object for the <see cref="AudioSession"/> class.
    /// </summary>
    public class AudioSessionCollection : ObservableImmutableList<AudioSession>
    {
        #region Constructor
        internal AudioSessionCollection(AudioDeviceCollection devices)
        {
            _devices = devices;
            _devices.DeviceEnabledChanged += HandleDeviceEnabledChanged;
            _devices.DeviceSessionCreated += HandleDeviceSessionCreated;
            _devices.DeviceSessionRemoved += HandleDeviceSessionRemoved;
        }
        #endregion Constructor

        #region Fields
        private readonly AudioDeviceCollection _devices;
        #endregion Fields

        #region EventHandlers
        private void HandleDeviceSessionCreated(object? sender, AudioSession session)
        {
            if (sender is AudioDevice device)
            {
                if (device.Enabled)
                    this.AddIfUnique(session);
            }
            else throw new InvalidOperationException($"{nameof(HandleDeviceSessionCreated)} received a sender of type '{sender?.GetType().FullName}'; expected '{typeof(AudioDevice).FullName}'");
        }
        private void HandleDeviceSessionRemoved(object? sender, long pid)
        {
            if (sender is AudioDevice device)
                Remove(pid);
            else throw new InvalidOperationException($"{nameof(HandleDeviceSessionRemoved)} received a sender of type '{sender?.GetType().FullName}'; expected '{typeof(AudioDevice).FullName}'");
        }
        private void HandleDeviceEnabledChanged(object? sender, bool state)
        {
            if (sender is AudioDevice device)
            {
                if (state)
                    this.AddRangeIfUnique(device.Sessions);
                else
                { // DEVICE WAS DISABLED
                    RemoveAll(s => !s.PID.Equals(0) && device.Sessions.Contains(s));
                }
            }
            else throw new InvalidOperationException($"{nameof(HandleDeviceEnabledChanged)} received a sender of type '{sender?.GetType().FullName}'; expected '{typeof(AudioDevice).FullName}'");
        }
        #endregion EventHandlers

        #region Methods
        /// <summary>Performs a selective-update of the session list.</summary>
        /// <param name="devices">Any number of <see cref="AudioDevice"/> classes.</param>
        internal void RefreshFromDevices(params AudioDevice[] devices)
        {
            List<AudioSession> l = new();
            foreach (var device in devices)
                if (device.Enabled && device.State.Equals(DeviceState.Active))
                    l.AddRangeIfUnique(device.Sessions);

            RemoveAll(s => !l.Contains(s));
            this.AddRangeIfUnique(l.AsEnumerable());
        }
        /// <inheritdoc cref="RefreshFromDevices(AudioDevice[])"/>
        /// <remarks>This method uses the internal reference to <see cref="AudioDeviceCollection"/> instead of given devices.</remarks>
        internal void RefreshFromDevices() => RefreshFromDevices(_devices.ToArray());
        /// <summary>
        /// Removes any sessions with the same process ID number as <paramref name="pid"/>.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns><see langword="true"/> when a session was successfully removed from the list; otherwise <see langword="false"/>.</returns>
        public bool Remove(long pid)
        {
            for (int i = Count - 1; i >= 0; --i)
            {
                if (this[i] is AudioSession session && session.PID.Equals(pid))
                {
                    RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        #endregion Methods
    }
}
