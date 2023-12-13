using CoreAudio;
using VolumeControl.CoreAudio.Helpers;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

namespace VolumeControl.CoreAudio
{
    /// <summary>
    /// Manages a list of <see cref="AudioDevice"/> instances and related events.
    /// </summary>
    /// <remarks>
    /// The <see cref="AudioDeviceManager"/> class is responsible for managing the list of active audio devices and handling all events related directly to <see cref="AudioDevice"/> instances.
    /// </remarks>
    public sealed class AudioDeviceManager : IDisposable
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="AudioDeviceManager"/> instance.
        /// </summary>
        /// <param name="deviceDataFlow">The <see cref="DataFlow"/> type of the devices that this <see cref="AudioDeviceManager"/> instance will manage. This cannot be changed later.</param>
        /// <param name="deviceEnumerator">The <see cref="MMDeviceEnumerator"/> instance to use.</param>
        public AudioDeviceManager(DataFlow deviceDataFlow, MMDeviceEnumerator deviceEnumerator)
        {
            _deviceDataFlow = deviceDataFlow;
            _devices = new();
            // initialize core audio api objects
            _eventContext = deviceEnumerator.eventContext;
            _deviceEnumerator = deviceEnumerator;
            _deviceNotificationClient = new(_deviceEnumerator);
            // connect events
            _deviceNotificationClient.DeviceStateChanged += this.DeviceNotificationClient_DeviceStateChanged;
            _deviceNotificationClient.DeviceAdded += this.DeviceNotificationClient_DeviceAdded;
            _deviceNotificationClient.DeviceRemoved += this.DeviceNotificationClient_DeviceRemoved;
            _deviceNotificationClient.DefaultDeviceChanged += this.DeviceNotificationClient_DefaultDeviceChanged;
            // initialize devices
            foreach (var mmDevice in _deviceEnumerator.EnumerateAudioEndPoints(DeviceDataFlow, DeviceState.Active))
            {
                try
                {
                    CreateAndAddDeviceIfUnique(mmDevice);
                }
                catch (Exception ex)
                {
                    FLog.Critical("An exception occurred while initializing an AudioDevice!", Log.Helpers.ObjectDebugger.ReflectionDump(mmDevice), ex);
                }
            }
        }
        /// <summary>
        /// Creates a new <see cref="AudioDeviceManager"/> instance.
        /// </summary>
        /// <param name="deviceDataFlow">The <see cref="DataFlow"/> type of the devices that this <see cref="AudioDeviceManager"/> instance will manage. This cannot be changed later.</param>
        public AudioDeviceManager(DataFlow deviceDataFlow) : this(deviceDataFlow, new MMDeviceEnumerator(new Guid())) { }
        #endregion Constructor

        #region Events
        /// <summary>
        /// Occurs when an <see cref="AudioDevice"/> was added to the <see cref="Devices"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioDevice>? DeviceAddedToList;
        private void NotifyDeviceAddedToList(AudioDevice audioDevice) => DeviceAddedToList?.Invoke(this, audioDevice);
        /// <summary>
        /// Occurs when an <see cref="AudioDevice"/> was removed from the <see cref="Devices"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioDevice>? DeviceRemovedFromList;
        private void NotifyDeviceRemovedFromList(AudioDevice audioDevice) => DeviceRemovedFromList?.Invoke(this, audioDevice);
        /// <summary>
        /// Occurs when an <see cref="AudioDevice"/> instance's state was changed.
        /// </summary>
        public event EventHandler<AudioDevice>? DeviceStateChanged;
        private void NotifyDeviceStateChanged(AudioDevice audioDevice) => DeviceStateChanged?.Invoke(this, audioDevice);
        #endregion Events

        #region Fields
        private readonly Guid _eventContext;
        private readonly MMDeviceEnumerator _deviceEnumerator;
        private readonly MMNotificationClient _deviceNotificationClient;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets the list of <see cref="AudioDevice"/> instances.
        /// </summary>
        public IReadOnlyList<AudioDevice> Devices => _devices;
        /// <summary>
        /// The underlying <see cref="List{T}"/> for the <see cref="Devices"/> property.
        /// </summary>
        private readonly List<AudioDevice> _devices;
        /// <summary>
        /// Gets the <see cref="DataFlow"/> type of the audio devices managed by this <see cref="AudioDeviceManager"/> instance.
        /// </summary>
        public DataFlow DeviceDataFlow
        {
            get => _deviceDataFlow;
            set
            {
                _deviceDataFlow = value;

                ReloadAudioDevices();
            }
        }
        private DataFlow _deviceDataFlow;
        #endregion Properties

        #region Methods
        #region Methods (FindDevice)
        /// <summary>
        /// Gets the <see cref="AudioDevice"/> instance associated with the given <paramref name="deviceID"/> string. (<see cref="MMDevice.ID"/>)
        /// </summary>
        /// <param name="deviceID">The ID of the target device.</param>
        /// <param name="comparisonType">The <see cref="StringComparison"/> type to use when comparing ID strings.</param>
        /// <returns>The <see cref="AudioDevice"/> associated with the given <paramref name="deviceID"/> if found; otherwise <see langword="null"/>.</returns>
        public AudioDevice? FindDeviceByID(string deviceID, StringComparison comparisonType = StringComparison.Ordinal)
            => Devices.FirstOrDefault(dev => dev.ID.Equals(deviceID, comparisonType));
        /// <summary>
        /// Gets the <see cref="AudioDevice"/> instance associated with the given <paramref name="mmDevice"/>.
        /// </summary>
        /// <param name="mmDevice">The <see cref="MMDevice"/> instance associated with the target device.</param>
        /// <returns>The <see cref="AudioDevice"/> associated with the given <paramref name="mmDevice"/> if found; otherwise <see langword="null"/>.</returns>
        public AudioDevice? FindDeviceByMMDevice(MMDevice mmDevice)
            => FindDeviceByID(mmDevice.ID);
        #endregion Methods (FindDevice)

        /// <summary>
        /// (Re)loads all active audio devices.
        /// </summary>
        private void ReloadAudioDevices()
        {
            if (DeviceDataFlow == DataFlow.All)
            { // load input devices
                foreach (var mmDevice in _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
                {
                    CreateAndAddDeviceIfUnique(mmDevice);
                }
            }
            else
            { // unload input devices
                for (int i = Devices.Count - 1; i >= 0; --i)
                {
                    var device = Devices[i];
                    if (device.DataFlow == DataFlow.Capture)
                        RemoveDevice(device);
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="AudioDevice"/> from the given <paramref name="mmDevice"/> and adds it to the <see cref="Devices"/> list, and triggers the <see cref="DeviceAddedToList"/> event.
        /// </summary>
        /// <param name="mmDevice">The <see cref="MMDevice"/> instance to create a new <see cref="AudioDevice"/> from.</param>
        /// <returns>The newly-created <see cref="AudioDevice"/> instance if successful; otherwise <see langword="null"/> if <paramref name="mmDevice"/> is already associated with another device.</returns>
        private AudioDevice? CreateAndAddDeviceIfUnique(MMDevice mmDevice)
        {
            if (FindDeviceByMMDevice(mmDevice) is null)
            {
                var audioDevice = new AudioDevice(mmDevice);

                _devices.Add(audioDevice);

                NotifyDeviceAddedToList(audioDevice);

                return audioDevice;
            }
            else return null;
        }
        private bool RemoveDevice(AudioDevice device)
        {
            if (_devices.Remove(device))
            {
                NotifyDeviceRemovedFromList(device);

                device.Dispose();

                return true;
            }
            else return false;
        }
        /// <summary>
        /// Gets the default device for the specified <paramref name="dataFlow"/> &amp; <paramref name="deviceRole"/>.
        /// </summary>
        /// <param name="dataFlow">The <see cref="DataFlow"/> of the target device.</param>
        /// <param name="deviceRole">The <see cref="Role"/> of the target device.</param>
        /// <returns>The default <see cref="AudioDevice"/> instance if one was found; otherwise, <see langword="null"/>.</returns>
        public AudioDevice? GetDefaultDevice(DataFlow dataFlow, Role deviceRole)
        {
            try
            {
                var defaultEndpointMMDevice = _deviceEnumerator.GetDefaultAudioEndpoint(dataFlow, deviceRole);

                return FindDeviceByID(defaultEndpointMMDevice.ID);
            }
            catch (Exception ex)
            {
                FLog.Error($"Getting default audio device with role '{deviceRole:G}' failed because an exception was thrown:", ex);
                return null;
            }
        }
        #endregion Methods

        #region Methods (_deviceNotificationClient EventHandlers)
        private void DeviceNotificationClient_DeviceStateChanged(object? sender, DeviceStateChangedEventArgs e)
        {
            if (!e.TryGetDevice(out MMDevice? mmDevice) || mmDevice is null) return;
            if (!DeviceDataFlow.HasFlag(mmDevice.DataFlow)) return;

            if (e.DeviceState == DeviceState.Active && mmDevice.State == DeviceState.Active)
            {
                if (CreateAndAddDeviceIfUnique(mmDevice) is AudioDevice newAudioDevice)
                {
                    NotifyDeviceStateChanged(newAudioDevice);
                    FLog.Debug($"{nameof(AudioDevice)} '{newAudioDevice.Name}' state changed to {mmDevice.State:G}; added it to the list.");
                }
                else
                {
                    NotifyDeviceStateChanged(FindDeviceByMMDevice(mmDevice)!);
                    FLog.Error($"{nameof(AudioDevice)} '{mmDevice.GetDeviceName()}' state changed to {mmDevice.State:G}; it is already in the list!");
                }
            }
            else // e.DeviceState != DeviceState.Active
            {
                if (FindDeviceByMMDevice(mmDevice) is AudioDevice existingAudioDevice)
                {
                    NotifyDeviceStateChanged(existingAudioDevice);

                    RemoveDevice(existingAudioDevice);

                    FLog.Debug($"{nameof(AudioDevice)} '{existingAudioDevice.Name}' state changed to {mmDevice.State:G}; removed it from the list.");
                }
                else
                {
                    FLog.Error($"{nameof(AudioDevice)} '{mmDevice.GetDeviceName()}' state changed to {mmDevice.State:G}; cannot remove it from the list because it doesn't exist!");
                }
            }
        }
        private void DeviceNotificationClient_DeviceAdded(object? sender, DeviceNotificationEventArgs e)
        {
            if (!e.TryGetDevice(out MMDevice? mmDevice) || mmDevice is null) return;
            if (!DeviceDataFlow.HasFlag(mmDevice.DataFlow)) return;

            if (mmDevice.State.Equals(DeviceState.Active))
            {
                if (CreateAndAddDeviceIfUnique(mmDevice) is AudioDevice newAudioDevice)
                    FLog.Debug($"Detected new {nameof(AudioDevice)} '{newAudioDevice.Name}'; added it to the list.");
                else
                {
                    FLog.Error($"Detected new {nameof(AudioDevice)} '{mmDevice.GetDeviceName()}'; it is already in the list!");
                }
            }
            else
            {
                FLog.Debug($"Detected new {nameof(AudioDevice)} '{mmDevice.GetDeviceName()}'; did not add it the list because its current state is {mmDevice.State:G}.");
            }
        }
        private void DeviceNotificationClient_DeviceRemoved(object? sender, DeviceNotificationEventArgs e)
        {
            if (FindDeviceByID(e.DeviceId) is AudioDevice audioDevice)
            {
                RemoveDevice(audioDevice);

                FLog.Debug($"{nameof(AudioDevice)} '{audioDevice.Name}' was removed from the system; it was removed from the list.");
            }
        }
        private void DeviceNotificationClient_DefaultDeviceChanged(object? sender, DefaultDeviceChangedEventArgs e)
        {
            Devices.ForEach(device =>
            {
                if (device.IsDefault)
                    device.IsDefault = false;
            });
            if (FindDeviceByID(e.DeviceId) is AudioDevice audioDevice)
                audioDevice.IsDefault = true;
        }
        #endregion Methods (_deviceNotificationClient EventHandlers)

        #region IDisposable Implementation
        /// <summary>
        /// Disposes of managed devices.
        /// </summary>
        ~AudioDeviceManager() => Dispose();
        /// <inheritdoc/>
        public void Dispose()
        {
            Devices.DisposeAll();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}