using NAudio.CoreAudioApi;
using ObservableImmutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Audio.Events;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;
using static System.Windows.Forms.AxHost;

namespace VolumeControl.Audio
{
    public class AudioDeviceCollection : ObservableImmutableList<AudioDevice>, IDisposable
    {
        public AudioDeviceCollection(DataFlow flow)
        {
            DataFlow = flow;
            using var enumerator = new MMDeviceEnumerator();

            Reload(enumerator);
            if (enumerator.HasDefaultAudioEndpoint(DataFlow, Role.Multimedia))
                Default = CreateDeviceFromMMDevice(enumerator.GetDefaultAudioEndpoint(DataFlow, Role.Multimedia));

            // set up notification client; this includes default device handling
            DeviceNotificationClient = new(this);
            enumerator.RegisterEndpointNotificationCallback(DeviceNotificationClient);
            DeviceNotificationClient.GlobalDeviceAdded += HandleDeviceAdded;
            enumerator.Dispose();
        }

        #region Fields
        private bool disposedValue;
        #endregion Fields

        #region Events
        public event EventHandler<bool>? DeviceEnabledChanged;
        private void ForwardDeviceEnabledChanged(object? sender, bool state)
        {
            if (sender is AudioDevice device)
            {
                Settings.EnabledDevices.AddIfUnique(device.DeviceID);
                Settings.Save();
                Settings.Reload();
            }
            DeviceEnabledChanged?.Invoke(sender, state);
        }
        public event EventHandler<AudioSession>? DeviceSessionCreated;
        private void ForwardSessionCreated(object? sender, AudioSession session) => DeviceSessionCreated?.Invoke(sender, session);
        public event EventHandler<long>? DeviceSessionRemoved;
        private void ForwardSessionRemoved(object? sender, long pid) => DeviceSessionRemoved?.Invoke(sender, pid);
        #endregion Events

        #region Properties
        private static LogWriter Log => FLog.Log;
        private static AudioAPISettings Settings => AudioAPISettings.Default;
        private DeviceNotificationClient DeviceNotificationClient { get; }
        /// <summary>Whether the devices in this list are input or output devices. (or both)</summary>
        public DataFlow DataFlow { get; }
        /// <summary>This is the current default multimedia device.</summary>
        public AudioDevice? Default { get; internal set; }
        #endregion Properties

        #region HandleDeviceEvents
        private void HandleDeviceAdded(object? sender, AudioDevice device)
        {
            if (AudioAPISettings.Default.EnabledDevices.Contains(device.DeviceID))
            {
                device.Enabled = true;
            }
        }
        private void HandleDeviceStateChanged(object? sender, DeviceState state)
        {
            if (sender is AudioDevice device)
            {
                Log.Debug($"Device '{device.Name}' state changed to {state:G}");
                switch (state)
                {
                case DeviceState.Active:
                    this.AddIfUnique(device);
                    break;
                default:
                    Remove(device);
                    device.Dispose();
                    device = null!;
                    break;
                }
            }
            else throw new InvalidOperationException($"{nameof(HandleDeviceStateChanged)} received invalid type {sender?.GetType().FullName}; expected {typeof(AudioDevice).FullName}");
        }
        private void HandleDeviceRemoved(object? sender, EventArgs e)
        {
            if (sender is AudioDevice device)
            {
                Remove(device);
                device.Dispose();
                device = null!;
            }
            else throw new InvalidOperationException($"{nameof(HandleDeviceRemoved)} received invalid type {sender?.GetType().FullName}; expected {typeof(AudioDevice).FullName}");
        }
        #endregion HandleDeviceEvents

        #region Methods
        public AudioDevice[] FindDevice(Predicate<AudioDevice> predicate) => this.Where(dev => predicate(dev)).ToArray();
        internal AudioDevice? FindDeviceWithMMDevice(MMDevice mmDevice) => this.FirstOrDefault(dev => dev != null && dev.DeviceID.Equals(mmDevice.ID, StringComparison.Ordinal), null);
        /// <summary>Creates a new audio device instance and connects its events to this object's handlers.</summary>
        /// <param name="mmDevice">The MMDevice instance to use when creating the device.</param>
        internal AudioDevice CreateDeviceFromMMDevice(MMDevice mmDevice)
        {
            if (FindDeviceWithMMDevice(mmDevice) is AudioDevice existing)
                return existing;
            AudioDevice dev = new(mmDevice);
            dev.StateChanged += HandleDeviceStateChanged;
            dev.Removed += HandleDeviceRemoved;
            dev.EnabledChanged += ForwardDeviceEnabledChanged;
            dev.SessionCreated += ForwardSessionCreated;
            dev.SessionRemoved += (s, e) => ForwardSessionRemoved(s, e.PID);
            Add(dev);
            return dev;
        }
        /// <summary>Clears the list of devices &amp; reloads them from the Windows API.</summary>
        internal void Reload(MMDeviceEnumerator enumerator)
        {
            Clear();
            foreach (MMDevice mmDevice in enumerator.EnumerateAudioEndPoints(DataFlow, DeviceState.Active))
                CreateDeviceFromMMDevice(mmDevice);
        }
        /// <summary>Clears the list of devices &amp; reloads them from the Windows API.</summary>
        internal void Reload()
        {
            using var enumerator = new MMDeviceEnumerator();
            Reload(enumerator);
            enumerator.Dispose();
        }
        #endregion Methods


        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.ForEach(d => d.Dispose());
                }
                disposedValue = true;
            }
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class AudioSessionCollection : ObservableImmutableList<AudioSession>
    {
        public AudioSessionCollection(AudioDeviceCollection devices)
        {
            _devices = devices;
            _devices.DeviceEnabledChanged += HandleDeviceEnabledChanged;
            _devices.DeviceSessionCreated += HandleDeviceSessionCreated;
            _devices.DeviceSessionRemoved += HandleDeviceSessionRemoved;
        }

        private readonly AudioDeviceCollection _devices;
        internal void RefreshFromDevices() => RefreshFromDevices(_devices.ToArray());
        #region EventHandlers
        private void HandleDeviceSessionCreated(object? sender, AudioSession session)
        {
            if (sender is AudioDevice device)
            {
                if (device.Enabled)
                {
                    this.AddIfUnique(session);
                }
            }
            else throw new InvalidOperationException($"{nameof(HandleDeviceSessionCreated)} received a sender of type '{sender?.GetType().FullName}'; expected '{typeof(AudioDevice).FullName}'");
        }
        private void HandleDeviceSessionRemoved(object? sender, long pid)
        {
            if (sender is AudioDevice device)
            {
                Remove(pid);
            }
            else throw new InvalidOperationException($"{nameof(HandleDeviceSessionRemoved)} received a sender of type '{sender?.GetType().FullName}'; expected '{typeof(AudioDevice).FullName}'");
        }
        private void HandleDeviceEnabledChanged(object? sender, bool state)
        {
            if (sender is AudioDevice device)
            {
                if (state)
                { // DEVICE WAS ENABLED
                    this.AddRangeIfUnique(device.Sessions);

                }
                else
                { // DEVICE WAS DISABLED
                    RemoveAll(s => !s.PID.Equals(0) && device.Sessions.Contains(s));
                }
            }
            else throw new InvalidOperationException($"{nameof(HandleDeviceEnabledChanged)} received a sender of type '{sender?.GetType().FullName}'; expected '{typeof(AudioDevice).FullName}'");
        }
        #endregion EventHandlers

        #region Methods
        internal void RefreshFromDevices(params AudioDevice[] devices)
        {
            List<AudioSession> l = new();
            foreach (var device in devices)
                if (device.Enabled && device.State.Equals(DeviceState.Active))
                    l.AddRangeIfUnique(device.Sessions);

            RemoveAll(s => !l.Contains(s));
            this.AddRangeIfUnique(l.AsEnumerable());
        }
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
