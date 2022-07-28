using NAudio.CoreAudioApi;
using VolumeControl.Audio.Events;
using VolumeControl.Core;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Audio.Collections
{
    /// <summary>
    /// Management container object for the <see cref="AudioDevice"/> class.
    /// </summary>
    public class AudioDeviceCollection : ObservableImmutableList<AudioDevice>, IDisposable
    {
        #region Constructor
        internal AudioDeviceCollection(DataFlow flow)
        {
            this.DataFlow = flow;
            using var enumerator = new MMDeviceEnumerator();

            this.Reload(enumerator);
            if (enumerator.HasDefaultAudioEndpoint(this.DataFlow, Role.Multimedia))
                this.Default = this.CreateDeviceFromMMDevice(enumerator.GetDefaultAudioEndpoint(this.DataFlow, Role.Multimedia));

            // set up notification client; this includes default device handling
            this.DeviceNotificationClient = new(this);
            _ = enumerator.RegisterEndpointNotificationCallback(this.DeviceNotificationClient);
            this.DeviceNotificationClient.GlobalDeviceAdded += this.HandleDeviceAdded;
            enumerator.Dispose();
        }
        #endregion Constructor

        #region Fields
        private bool disposedValue;
        #endregion Fields

        #region Events
        /// <summary>Triggered when a managed device's Enabled property was changed.</summary>
        public event EventHandler<bool>? DeviceEnabledChanged;
        private void ForwardDeviceEnabledChanged(object? sender, bool state)
        {
            if (sender is AudioDevice device)
            {
                if (state)
                    Settings.EnabledDevices.AddIfUnique(device.DeviceID);
                else
                    _ = Settings.EnabledDevices.Remove(device.DeviceID);
            }
            DeviceEnabledChanged?.Invoke(sender, state);
        }
        /// <summary>Triggered when a managed device has a session added to it.</summary>
        public event EventHandler<AudioSession>? DeviceSessionCreated;
        private void ForwardSessionCreated(object? sender, AudioSession session) => DeviceSessionCreated?.Invoke(sender, session);
        /// <summary>Triggered when a managed device has a session removed from it.</summary>
        public event EventHandler<long>? DeviceSessionRemoved;
        private void ForwardSessionRemoved(object? sender, long pid) => DeviceSessionRemoved?.Invoke(sender, pid);
        #endregion Events

        #region Properties
        private static LogWriter Log => FLog.Log;
        private static Config Settings => (Config.Default as Config)!;
        private DeviceNotificationClient DeviceNotificationClient { get; }
        /// <summary>Whether the devices in this list are input or output devices. (or both)</summary>
        public DataFlow DataFlow { get; }
        /// <summary>This is the current default multimedia device.</summary>
        public AudioDevice? Default { get; internal set; }
        #endregion Properties

        #region HandleDeviceEventsga
        private void HandleDeviceAdded(object? sender, AudioDevice device)
        {
            if (Settings.EnabledDevices.Contains(device.DeviceID))
                device.Enabled = true;
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
                    _ = this.Remove(device);
                    device.Dispose();
                    device = null!;
                    break;
                }
            }
            else
            {
                Log.Error($"{nameof(HandleDeviceStateChanged)} received invalid type {sender?.GetType().FullName}; expected {typeof(AudioDevice).FullName}");
                Reload();
            }
        }
        private void HandleDeviceRemoved(object? sender, EventArgs e)
        {
            if (sender is AudioDevice device)
            {
                _ = this.Remove(device);
                device.Dispose();
                device = null!;
            }
            else
            {
                Log.Error($"{nameof(HandleDeviceRemoved)} received invalid type {sender?.GetType().FullName}; expected {typeof(AudioDevice).FullName}");
                Reload();
            }
        }
        #endregion HandleDeviceEvents

        #region Methods
        /// <summary>
        /// Finds a device using the <paramref name="predicate"/> function.
        /// </summary>
        /// <param name="predicate">A predicate function that accepts <see cref="AudioDevice"/> class types.</param>
        /// <returns>Devices that <paramref name="predicate"/> returned <see langword="true"/> for.</returns>
        public AudioDevice[] FindDevice(Predicate<AudioDevice> predicate) => this.Where(dev => predicate(dev)).ToArray();
        /// <summary>Finds the <see cref="AudioDevice"/> using the given <paramref name="mmDevice"/> object.</summary>
        /// <param name="mmDevice">A <see cref="MMDevice"/> object to find.</param>
        /// <returns>The audio device using <paramref name="mmDevice"/>.</returns>
        internal AudioDevice? FindDeviceWithMMDevice(MMDevice mmDevice) => this.FirstOrDefault(dev => dev != null && dev.DeviceID.Equals(mmDevice.ID, StringComparison.Ordinal), null);
        /// <summary>Creates a new audio device instance and connects its events to this object's handlers.</summary>
        /// <param name="mmDevice">The MMDevice instance to use when creating the device.</param>
        internal AudioDevice CreateDeviceFromMMDevice(MMDevice mmDevice)
        {
            if (this.FindDeviceWithMMDevice(mmDevice) is AudioDevice existing)
                return existing;
            AudioDevice dev = new(mmDevice);
            dev.StateChanged += this.HandleDeviceStateChanged;
            dev.Removed += this.HandleDeviceRemoved;
            dev.EnabledChanged += this.ForwardDeviceEnabledChanged;
            dev.SessionCreated += this.ForwardSessionCreated;
            dev.SessionRemoved += (s, e) => this.ForwardSessionRemoved(s, e.PID);
            _ = this.Add(dev);
            return dev;
        }
        /// <summary>Clears the list of devices &amp; reloads them from the Windows API.</summary>
        internal void Reload(MMDeviceEnumerator enumerator)
        {
            _ = this.Clear();
            foreach (MMDevice mmDevice in enumerator.EnumerateAudioEndPoints(this.DataFlow, DeviceState.Active))
                _ = this.CreateDeviceFromMMDevice(mmDevice);
        }
        /// <summary>Clears the list of devices &amp; reloads them from the Windows API.</summary>
        internal void Reload()
        {
            using var enumerator = new MMDeviceEnumerator();
            this.Reload(enumerator);
            enumerator.Dispose();
        }
        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    this.ForEach(d => d.Dispose());
                disposedValue = true;
            }
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Methods
    }
}
