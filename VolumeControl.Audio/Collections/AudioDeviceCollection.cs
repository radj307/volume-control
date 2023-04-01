using NAudio.CoreAudioApi;
using System.Collections;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Audio.Events;
using VolumeControl.Audio.Interfaces;
using VolumeControl.Core;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Audio.Collections
{
    /// <summary>
    /// Management container object for the <see cref="AudioDevice"/> class.
    /// </summary>
    public class AudioDeviceCollection : IList, ICollection, IEnumerable, IList<AudioDevice>, IImmutableList<AudioDevice>, ICollection<AudioDevice>, IEnumerable<AudioDevice>, IReadOnlyList<AudioDevice>, IReadOnlyCollection<AudioDevice>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable, IDeviceNotificationClient
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="AudioDeviceCollection"/> instance with the given <see cref="NAudio.CoreAudioApi.DataFlow"/>.
        /// </summary>
        /// <param name="flow">Specifies the (I/O) type of <see cref="AudioDevice"/> objects that this instance manages.</param>
        public AudioDeviceCollection(DataFlow flow)
        {
            this.DataFlow = flow;
            this.Devices = new();

            MMDeviceEnumerator = new MMDeviceEnumerator();

            ReloadDevices();

            EndpointNotificationClient = new(this);
            MMDeviceEnumerator.RegisterEndpointNotificationCallback(EndpointNotificationClient);
            EndpointNotificationClient.DefaultDeviceChanged += this.HandleDefaultDeviceChanged;
            EndpointNotificationClient.DeviceAdded += this.HandleDeviceAdded;
            EndpointNotificationClient.DeviceStateChanged += this.HandleDeviceStateChanged;
        }
        #endregion Constructors

        #region Properties
        private static Config Settings => (Config.Default as Config)!;
        private static LogWriter Log => FLog.Log;
        internal MMDeviceEnumerator MMDeviceEnumerator { get; }
        internal DeviceNotificationClient EndpointNotificationClient { get; }
        /// <summary>
        /// Gets the list of <see cref="AudioDevice"/> instances that are currently being tracked.
        /// </summary>
        public ObservableImmutableList<AudioDevice> Devices { get; }
        /// <summary>
        /// Gets the default <see cref="DataFlow.Render"/> <see cref="Role.Multimedia"/> <see cref="AudioDevice"/>.
        /// </summary>
        public AudioDevice? DefaultDevice { get; internal set; }
        /// <summary>
        /// Specifies the (I/O) type of <see cref="AudioDevice"/> objects that this instance manages.
        /// </summary>
        public DataFlow DataFlow { get; }
        #endregion Properties

        #region Events
        /// <summary>Triggered when a managed device's Enabled property was changed.</summary>
        public event EventHandler<bool>? DeviceEnabledChanged;
        private void ForwardDeviceEnabledChanged(object? sender, bool state)
        {
            if (sender is AudioDevice device)
            {
                _ = state ? Settings.EnabledDevices.AddIfUnique(device.DeviceID) : Settings.EnabledDevices.Remove(device.DeviceID);
            }
            DeviceEnabledChanged?.Invoke(sender, state);
        }
        /// <summary>Triggered when a managed device has a session added to it.</summary>
        public event EventHandler<AudioSession>? DeviceSessionCreated;
        private void ForwardSessionCreated(object? sender, AudioSession session) => DeviceSessionCreated?.Invoke(sender, session);
        /// <summary>Triggered when a managed device has a session removed from it.</summary>
        public event EventHandler<long>? DeviceSessionRemoved;
        private void ForwardSessionRemoved(object? sender, long pid) => DeviceSessionRemoved?.Invoke(sender, pid);
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void ForwardPropertyChanged(object? sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        /// <summary>
        /// Triggered when the volume or mute state of a device was changed.
        /// </summary>
        public event VolumeChangedEventHandler? DeviceVolumeChanged;
        private void ForwardDeviceVolumeChanged(object? sender, VolumeChangedEventArgs e) => DeviceVolumeChanged?.Invoke(sender, e);
        #region IDeviceNotificationClient Events
        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add => ((INotifyCollectionChanged)this.Devices).CollectionChanged += value;
            remove => ((INotifyCollectionChanged)this.Devices).CollectionChanged -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<(MMDevice, Role)>? DefaultDeviceChanged
        {
            add => this.EndpointNotificationClient.DefaultDeviceChanged += value;
            remove => this.EndpointNotificationClient.DefaultDeviceChanged -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<MMDevice>? DeviceAdded
        {
            add => this.EndpointNotificationClient.DeviceAdded += value;
            remove => this.EndpointNotificationClient.DeviceAdded -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<string>? DeviceRemoved
        {
            add => this.EndpointNotificationClient.DeviceRemoved += value;
            remove => this.EndpointNotificationClient.DeviceRemoved -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<MMDevice>? DeviceStateChanged
        {
            add => this.EndpointNotificationClient.DeviceStateChanged += value;
            remove => this.EndpointNotificationClient.DeviceStateChanged -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<(string, PropertyKey)>? PropertyValueChanged
        {
            add => this.EndpointNotificationClient.PropertyValueChanged += value;
            remove => this.EndpointNotificationClient.PropertyValueChanged -= value;
        }
        #endregion IDeviceNotificationClient Events
        #endregion Events

        #region Methods
        #region Internal/Private Methods
        #region AllDevicesEnabled
        /// <summary>
        /// Enables or disables all devices.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> when all devices are enabled, <see langword="false"/> when all devices are disabled; otherwise <see langword="null"/>.
        /// </returns>
        public bool? AllDevicesEnabled
        {
            get => _allSelected;
            set
            {
                if (value == _allSelected) return;

                _allSelected = value;

                AllDevicesEnabledChanged();
                NotifyPropertyChanged();
            }
        }

        public int Count => ((IReadOnlyCollection<AudioDevice>)this.Devices).Count;

        public bool IsReadOnly => ((ICollection<AudioDevice>)this.Devices).IsReadOnly;

        public bool IsSynchronized => ((ICollection)this.Devices).IsSynchronized;

        public object SyncRoot => ((ICollection)this.Devices).SyncRoot;

        public bool IsFixedSize => ((IList)this.Devices).IsFixedSize;

        object? IList.this[int index] { get => ((IList)this.Devices)[index]; set => ((IList)this.Devices)[index] = value; }
        AudioDevice IList<AudioDevice>.this[int index] { get => ((IList<AudioDevice>)this.Devices)[index]; set => ((IList<AudioDevice>)this.Devices)[index] = value; }

        public AudioDevice this[int index] => ((IReadOnlyList<AudioDevice>)this.Devices)[index];

        private bool? _allSelected;
        private bool _allSelectedChanging;
        private void AllDevicesEnabledChanged()
        {
            // Has this change been caused by some other change?
            // return so we don't mess things up
            if (_allSelectedChanging) return;

            try
            {
                _allSelectedChanging = true;

                if (this.AllDevicesEnabled == true)
                {
                    Devices.ForEach(dev => dev.Enabled = true);
                }
                else if (this.AllDevicesEnabled == false)
                {
                    Devices.ForEach(dev => dev.Enabled = false);
                }
            }
            finally
            {
                _allSelectedChanging = false;
            }
        }
        private void RecheckAllDevicesEnabled()
        {
            // Has this change been caused by some other change?
            // return so we don't mess things up
            if (_allSelectedChanging) return;

            try
            {
                _allSelectedChanging = true;

                if (Devices.Count > 0)
                {
                    bool prev = Devices.First().Enabled;
                    bool fullLoop = true;
                    for (int i = 1; i < Devices.Count; ++i)
                    {
                        if (Devices[i].Enabled != prev)
                        {
                            fullLoop = false;
                            this.AllDevicesEnabled = null;
                            break;
                        }
                    }
                    if (fullLoop)
                        this.AllDevicesEnabled = prev;
                }
                else
                {
                    this.AllDevicesEnabled = false;
                }
            }
            finally
            {
                _allSelectedChanging = false;
            }
        }
        #endregion AllDevicesEnabled
        internal void ConnectAudioDeviceEvents(AudioDevice device)
        {
            DisconnectAudioDeviceEvents(device); //< disconnect events first to ensure we don't add duplicate handlers

            device.StateChanged += this.HandleAudioDeviceStateChanged;
            device.Removed += this.HandleAudioDeviceRemoved;
            device.EnabledChanged += this.HandleAudioDeviceEnabledChanged;
            device.SessionCreated += this.HandleAudioDeviceSessionCreated;
            device.SessionRemoved += this.HandleAudioDeviceSessionRemoved;
            device.VolumeChanged += this.HandleAudioDeviceVolumeChanged;
        }
        internal void DisconnectAudioDeviceEvents(AudioDevice device)
        {
            device.StateChanged -= this.HandleAudioDeviceStateChanged;
            device.Removed -= this.HandleAudioDeviceRemoved;
            device.EnabledChanged -= this.HandleAudioDeviceEnabledChanged;
            device.SessionCreated -= this.HandleAudioDeviceSessionCreated;
            device.SessionRemoved -= this.HandleAudioDeviceSessionRemoved;
            device.VolumeChanged -= this.HandleAudioDeviceVolumeChanged;
        }
        internal void RemoveDevice(AudioDevice device)
        {
            DisconnectAudioDeviceEvents(device);
            Devices.Remove(device);
            Log.Debug($"Removed audio device '{device.Name}'.");
            device.Dispose();
        }
        #endregion Internal/Private Methods

        #region Public Methods
        /// <summary>
        /// Gets the <see cref="AudioDevice"/> associated with the given <paramref name="mmDevice"/>.
        /// </summary>
        /// <param name="mmDevice">An NAudio <see cref="MMDevice"/> instance.</param>
        /// <returns>The <see cref="AudioDevice"/> associated with <paramref name="mmDevice"/> if it was found in <see cref="Devices"/>; otherwise <see langword="null"/>.</returns>
        public AudioDevice? FindDeviceWithMMDevice(MMDevice mmDevice) => Devices.FirstOrDefault(device => device != null && device.DeviceID.Equals(mmDevice.ID, StringComparison.Ordinal), null);
        /// <summary>
        /// Gets the <see cref="AudioDevice"/> associated with the given <paramref name="deviceID"/>.
        /// </summary>
        /// <param name="deviceID">The <see cref="AudioDevice.DeviceID"/> to search for.</param>
        /// <param name="sCompareType">The <see cref="StringComparison"/> type to use.</param>
        /// <returns>The <see cref="AudioDevice"/> associated with <paramref name="deviceID"/> if it was found in <see cref="Devices"/>; otherwise <see langword="null"/>.</returns>
        public AudioDevice? FindDeviceWithDeviceID(string deviceID, StringComparison sCompareType = StringComparison.Ordinal) => Devices.FirstOrDefault(device => device != null && device.DeviceID.Equals(deviceID, sCompareType), null);
        /// <summary>
        /// Reloads all AudioDevices using the default <see cref="MMDeviceEnumerator"/>.
        /// </summary>
        public void ReloadDevices()
        {
            Devices.Clear();
            foreach (MMDevice mmDevice in MMDeviceEnumerator.EnumerateAudioEndPoints(this.DataFlow, DeviceState.Active))
            {
                var device = new AudioDevice(mmDevice);
                ConnectAudioDeviceEvents(device);
                Devices.Add(device);
            }
            // Set the DefaultDevice:
            if (MMDeviceEnumerator.HasDefaultAudioEndpoint(this.DataFlow, Role.Multimedia))
            {
                DefaultDevice = FindDeviceWithMMDevice(MMDeviceEnumerator.GetDefaultAudioEndpoint(this.DataFlow, Role.Multimedia));
            }
        }
        #endregion Public Methods
        #endregion Methods

        #region EventHandlers
        /// <summary>
        /// Adds/Removes audio devices from <see cref="Devices"/> when their state changes to or from <see cref="DeviceState.Active"/>.
        /// </summary>
        private void HandleAudioDeviceStateChanged(object? sender, DeviceState state)
        {
            var device = (sender as AudioDevice)!;

            Log.Debug($"Device '{device.Name}' state changed to {state:G}.");

            switch (state)
            {
            case DeviceState.Active:
                Devices.AddIfUnique(device);
                break;
            default:
                Devices.Remove(device);
                device.Dispose();
                break;
            }
        }
        /// <summary>
        /// Removes audio devices from <see cref="Devices"/> when they were removed from the system.
        /// </summary>
        private void HandleAudioDeviceRemoved(object? sender, EventArgs e)
        {
            RemoveDevice((sender as AudioDevice)!);
        }
        /// <summary>
        /// Updates the <see cref="Settings"/> configuration by adding/removing audio devices when they are enabled/disabled, respectively.
        /// </summary>
        private void HandleAudioDeviceEnabledChanged(object? sender, bool state)
        {
            var device = (sender as AudioDevice)!;

            if (state) // Device was enabled:
            {
                Settings.EnabledDevices.AddIfUnique(device.DeviceID);
                Log.Debug($"Audio device '{device.Name}' was enabled.");
            }
            else // Device was disabled:
            {
                Settings.EnabledDevices.Remove(device.DeviceID);
                Log.Debug($"Audio device '{device.Name}' was disabled.");
            }
            RecheckAllDevicesEnabled();
            DeviceEnabledChanged?.Invoke(sender, state);
        }
        /// <summary>
        /// Invokes <see cref="DeviceSessionCreated"/>.
        /// </summary>
        private void HandleAudioDeviceSessionCreated(object? sender, AudioSession session)
        {
            var device = (sender as AudioDevice)!;
            Log.Debug($"Audio session '{session.ProcessIdentifier}' created on device '{device.Name}'");
            DeviceSessionCreated?.Invoke(sender, session);
        }
        /// <summary>
        /// Invokes <see cref="DeviceSessionRemoved"/>.
        /// </summary>
        private void HandleAudioDeviceSessionRemoved(object? sender, AudioSession session)
        {
            var device = (sender as AudioDevice)!;
            Log.Debug($"Audio session '{session.ProcessIdentifier}' removed from device '{device.Name}'");
            DeviceSessionRemoved?.Invoke(sender, session.PID);
        }
        /// <summary>
        /// Invokes <see cref="DeviceVolumeChanged"/>.
        /// </summary>
        private void HandleAudioDeviceVolumeChanged(object? sender, VolumeChangedEventArgs e)
            => DeviceVolumeChanged?.Invoke(sender, e);
        private void HandleDefaultDeviceChanged(object? sender, (MMDevice mmDevice, Role role) e)
        {
            if (!e.role.Equals(Role.Multimedia) || !this.DataFlow.HasFlag(e.mmDevice.DataFlow))
            {
                Log.Debug($"Default {e.role:G} audio device was changed.");
                return;
            }

            if (FindDeviceWithMMDevice(e.mmDevice) is AudioDevice device)
            {
                DefaultDevice = device;
            }
            else
            {
                Log.Error($"Default multimedia output device was changed to an unknown {nameof(MMDevice)} instance; this likely indicates a problem! (See method {typeof(AudioDeviceCollection).FullName}.{nameof(HandleDefaultDeviceChanged)})");
            }
        }
        private void HandleDeviceAdded(object? sender, MMDevice mmDevice)
        {
            if (!this.DataFlow.HasFlag(mmDevice.DataFlow))
            {
                Log.Debug($"Ignoring new audio device '{AudioDevice.GetDeviceNameFromDeviceFriendlyName(mmDevice.DeviceFriendlyName)}' with {nameof(NAudio.CoreAudioApi.DataFlow)} '{mmDevice.DataFlow:G}'");
                return;
            }

            AudioDevice device = new(mmDevice);
            ConnectAudioDeviceEvents(device);
            Devices.Add(device);

            Log.Debug($"Added new audio device '{device.Name}' with {nameof(NAudio.CoreAudioApi.DataFlow)} '{device.MMDevice.DataFlow:G}'");
        }
        private void HandleDeviceRemoved(object? sender, string deviceId)
        {
            if (FindDeviceWithDeviceID(deviceId) is AudioDevice device)
            {
                RemoveDevice(device);
            }
        }
        private void HandleDeviceStateChanged(object? sender, MMDevice mmDevice)
        {
            if (FindDeviceWithMMDevice(mmDevice) is AudioDevice device)
            { // device already exists:
                if (!mmDevice.State.Equals(DeviceState.Active))
                {
                    RemoveDevice(device);
                }
            }
            else if (mmDevice.State.Equals(DeviceState.Active))
            { // device doesn't exist & is now active:
                var dev = new AudioDevice(mmDevice);
                ConnectAudioDeviceEvents(dev);
                Devices.Add(dev);
            }
        }

        #endregion EventHandlers

        #region InterfaceMethods
        /// <inheritdoc/>
        public void Dispose()
        {
            MMDeviceEnumerator.UnregisterEndpointNotificationCallback(EndpointNotificationClient);
            MMDeviceEnumerator.Dispose();
            Devices.ForEach(d => d.Dispose());
            Devices.Clear();
        }

        public IEnumerator<AudioDevice> GetEnumerator() => ((IEnumerable<AudioDevice>)this.Devices).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.Devices).GetEnumerator();
        public void Add(AudioDevice item) => ((ICollection<AudioDevice>)this.Devices).Add(item);
        public void Clear() => ((ICollection<AudioDevice>)this.Devices).Clear();
        public bool Contains(AudioDevice item) => ((ICollection<AudioDevice>)this.Devices).Contains(item);
        public void CopyTo(AudioDevice[] array, int arrayIndex) => ((ICollection<AudioDevice>)this.Devices).CopyTo(array, arrayIndex);
        public bool Remove(AudioDevice item) => ((ICollection<AudioDevice>)this.Devices).Remove(item);
        IImmutableList<AudioDevice> IImmutableList<AudioDevice>.Add(AudioDevice value) => ((IImmutableList<AudioDevice>)this.Devices).Add(value);
        public IImmutableList<AudioDevice> AddRange(IEnumerable<AudioDevice> items) => ((IImmutableList<AudioDevice>)this.Devices).AddRange(items);
        IImmutableList<AudioDevice> IImmutableList<AudioDevice>.Clear() => ((IImmutableList<AudioDevice>)this.Devices).Clear();
        public int IndexOf(AudioDevice item, int index, int count, IEqualityComparer<AudioDevice>? equalityComparer) => ((IImmutableList<AudioDevice>)this.Devices).IndexOf(item, index, count, equalityComparer);
        public IImmutableList<AudioDevice> Insert(int index, AudioDevice element) => ((IImmutableList<AudioDevice>)this.Devices).Insert(index, element);
        public IImmutableList<AudioDevice> InsertRange(int index, IEnumerable<AudioDevice> items) => ((IImmutableList<AudioDevice>)this.Devices).InsertRange(index, items);
        public int LastIndexOf(AudioDevice item, int index, int count, IEqualityComparer<AudioDevice>? equalityComparer) => ((IImmutableList<AudioDevice>)this.Devices).LastIndexOf(item, index, count, equalityComparer);
        public IImmutableList<AudioDevice> Remove(AudioDevice value, IEqualityComparer<AudioDevice>? equalityComparer) => ((IImmutableList<AudioDevice>)this.Devices).Remove(value, equalityComparer);
        public IImmutableList<AudioDevice> RemoveAll(Predicate<AudioDevice> match) => ((IImmutableList<AudioDevice>)this.Devices).RemoveAll(match);
        public IImmutableList<AudioDevice> RemoveAt(int index) => ((IImmutableList<AudioDevice>)this.Devices).RemoveAt(index);
        public IImmutableList<AudioDevice> RemoveRange(IEnumerable<AudioDevice> items, IEqualityComparer<AudioDevice>? equalityComparer) => ((IImmutableList<AudioDevice>)this.Devices).RemoveRange(items, equalityComparer);
        public IImmutableList<AudioDevice> RemoveRange(int index, int count) => ((IImmutableList<AudioDevice>)this.Devices).RemoveRange(index, count);
        public IImmutableList<AudioDevice> Replace(AudioDevice oldValue, AudioDevice newValue, IEqualityComparer<AudioDevice>? equalityComparer) => ((IImmutableList<AudioDevice>)this.Devices).Replace(oldValue, newValue, equalityComparer);
        public IImmutableList<AudioDevice> SetItem(int index, AudioDevice value) => ((IImmutableList<AudioDevice>)this.Devices).SetItem(index, value);
        public int IndexOf(AudioDevice item) => ((IList<AudioDevice>)this.Devices).IndexOf(item);
        void IList<AudioDevice>.Insert(int index, AudioDevice item) => ((IList<AudioDevice>)this.Devices).Insert(index, item);
        void IList<AudioDevice>.RemoveAt(int index) => ((IList<AudioDevice>)this.Devices).RemoveAt(index);
        public void CopyTo(Array array, int index) => ((ICollection)this.Devices).CopyTo(array, index);
        public int Add(object? value) => ((IList)this.Devices).Add(value);
        public bool Contains(object? value) => ((IList)this.Devices).Contains(value);
        public int IndexOf(object? value) => ((IList)this.Devices).IndexOf(value);
        public void Insert(int index, object? value) => ((IList)this.Devices).Insert(index, value);
        public void Remove(object? value) => ((IList)this.Devices).Remove(value);
        void IList.RemoveAt(int index) => ((IList)this.Devices).RemoveAt(index);
        #endregion InterfaceMethods
    }
}
