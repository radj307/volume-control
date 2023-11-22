using CoreAudio;
using CoreAudio.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.CoreAudio.Events;
using VolumeControl.CoreAudio.Helpers;
using VolumeControl.CoreAudio.Interfaces;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

namespace VolumeControl.CoreAudio
{
    /// <summary>
    /// An audio endpoint device.
    /// </summary>
    public sealed class AudioDevice : IAudioControl, IReadOnlyAudioControl, IAudioPeakMeter, INotifyPropertyChanged, IDisposable
    {
        #region Constructor
        internal AudioDevice(MMDevice mmDevice)
        {
            MMDevice = mmDevice;

            FullName = mmDevice.DeviceFriendlyName;
            Name = mmDevice.GetDeviceName();
            ID = mmDevice.ID;
            DataFlow = MMDevice.DataFlow;

            if (MMDevice.AudioSessionManager2 is null)
                throw new NullReferenceException($"{nameof(AudioDevice)} '{Name}' has a null {nameof(MMDevice.AudioSessionManager2)} property!");
            if (MMDevice.AudioEndpointVolume is null)
                throw new NullReferenceException($"{nameof(AudioDevice)} '{Name}' has a null {nameof(MMDevice.AudioEndpointVolume)} property!");
            if (MMDevice.AudioMeterInformation is null)
                throw new NullReferenceException($"{nameof(AudioDevice)} '{Name}' has a null {nameof(MMDevice.AudioMeterInformation)} property!");

            MMDevice.AudioSessionManager2.OnSessionCreated += this.AudioSessionManager2_OnSessionCreated;

            SessionManager = new(this);

            MMDevice.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;

            if (FLog.FilterEventType(EventType.TRACE))
                FLog.Trace($"[{nameof(AudioDevice)}] Successfully created {nameof(AudioDevice)} instance \"{FullName}\".");
        }
        #endregion Constructor

        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        /// <summary>
        /// Occurs when this <see cref="AudioDevice"/> instance's volume or mute state was changed.
        /// </summary>
        public event VolumeChangedEventHandler? VolumeChanged;
        private void NotifyVolumeChanged(float nativeVolume, bool mute) => VolumeChanged?.Invoke(this, new(nativeVolume, mute));
        internal event EventHandler<IAudioSessionControl2>? SessionCreated;
        private void NotifySessionCreated(IAudioSessionControl2 audioSessionControl2) => SessionCreated?.Invoke(this, audioSessionControl2);
        #endregion Events

        #region Fields
        /// <summary>
        /// Used to prevent duplicate <see cref="PropertyChanged"/> events from being fired.
        /// </summary>
        private bool isNotifying = false;
        private readonly object _audioEndpointVolumeLock = new();
        private readonly object _audioSessionManagerLock = new();
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets the underlying <see cref="MMDevice"/> for this <see cref="AudioDevice"/> instance.
        /// </summary>
        private MMDevice MMDevice { get; }
        private AudioMeterInformation AudioMeterInformation => MMDevice.AudioMeterInformation!;
        /// <summary>
        /// Gets the <see cref="AudioDeviceSessionManager"/> instance that manages this <see cref="AudioDevice"/> instance's <see cref="AudioSession"/>s.
        /// </summary>
        public AudioDeviceSessionManager SessionManager { get; }
        /// <summary>
        /// Gets the DataFlow direction of this audio device.
        /// </summary>
        public DataFlow DataFlow { get; } //< this needs to be cached to avoid exceptions
        /// <summary>
        /// Gets the full name of this <see cref="AudioDevice"/> instance, including both the device name and the interface name.
        /// </summary>
        public string FullName { get; }
        /// <summary>
        /// Gets the name of this <see cref="AudioDevice"/> instance as shown in the Windows sound settings window.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the ID string of this <see cref="AudioDevice"/> instance.
        /// </summary>
        public string ID { get; }
        /// <summary>
        /// Gets whether this is the default <see cref="AudioDevice"/> or not.
        /// </summary>
        /// <returns><see langword="true"/> when this is the default device; otherwise <see langword="false"/>.</returns>
        public bool IsDefault
        {
            get => _isDefault;
            internal set
            {
                _isDefault = value;
                NotifyPropertyChanged();
            }
        }
        private bool _isDefault = false;
        /// <summary>
        /// Gets the path to the device's icon file.
        /// </summary>
        public string IconPath => MMDevice.IconPath;
        #endregion Properties

        #region IAudioControl Implementation
        /// <inheritdoc/>
        public float NativeVolume
        {
            get
            {
                lock (_audioEndpointVolumeLock)
                {
                    return MMDevice.AudioEndpointVolume!.MasterVolumeLevelScalar;
                }
            }
            set
            {
                value = value.Bound(0f, 1f);
                if (value == NativeVolume) return;

                SetVolume(value);
                if (isNotifying) return; //< don't duplicate propertychanged notifications
                isNotifying = true;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Volume));
                isNotifying = false;
            }
        }
        /// <inheritdoc/>
        public int Volume
        {
            get => VolumeLevelConverter.FromNativeVolume(NativeVolume);
            set
            {
                if (value == Volume) return;

                NativeVolume = VolumeLevelConverter.ToNativeVolume(value);
                if (isNotifying) return; //< don't duplicate propertychanged notifications
                isNotifying = true;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(NativeVolume));
                isNotifying = false;
            }
        }
        /// <inheritdoc/>
        public bool Mute
        {
            get
            {
                lock (_audioEndpointVolumeLock)
                {
                    return MMDevice.AudioEndpointVolume!.Mute;
                }
            }
            set
            {
                if (value == Mute) return;

                SetMuteState(value);
                NotifyPropertyChanged();
            }
        }
        #endregion IAudioControl Implementation

        #region IAudioPeakMeter Implementation
        /// <inheritdoc/>
        public float PeakMeterValue => AudioMeterInformation.MasterPeakValue;
        #endregion IAudioPeakMeter Implementation

        #region IDisposable Implementation
        /// <summary>
        /// Disposes of the audio device.
        /// </summary>
        ~AudioDevice() => Dispose();
        /// <inheritdoc/>
        public void Dispose()
        {
            if (FLog.Log.FilterEventType(EventType.TRACE))
                FLog.Log.Trace($"Disposing of {nameof(AudioDevice)} instance \"{FullName}\"");
            ((IDisposable)this.MMDevice).Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation

        #region Methods

        #region (Private) Set Volume/Mute/Both
        private void SetVolume(float nativeVolume)
        {
            lock (_audioEndpointVolumeLock)
            {
                MMDevice.AudioEndpointVolume!.MasterVolumeLevelScalar = nativeVolume;
            }
            NotifyVolumeChanged(nativeVolume, Mute);
        }
        private void SetMuteState(bool muteState)
        {
            lock (_audioEndpointVolumeLock)
            {
                MMDevice.AudioEndpointVolume!.Mute = muteState;
            }
            NotifyVolumeChanged(NativeVolume, muteState);
        }
        private void SetVolumeAndMuteState(float nativeVolume, bool muteState)
        {
            lock (_audioEndpointVolumeLock)
            {
                MMDevice.AudioEndpointVolume!.MasterVolumeLevel = nativeVolume;
                MMDevice.AudioEndpointVolume!.Mute = muteState;
            }
            NotifyVolumeChanged(nativeVolume, muteState);
        }
        #endregion (Private) Set Volume/Mute/Both

        #region (Internal) RefreshSessions
        internal void RefreshSessions()
        {
            lock (_audioSessionManagerLock)
            {
                MMDevice.AudioSessionManager2!.RefreshSessions();
            }
        }
        #endregion (Internal) RefreshSessions

        #region (Internal) GetSessionControl
        internal AudioSessionControl2? GetSessionControl(string sessionInstanceIdentifier)
        {
            lock (_audioSessionManagerLock)
            {
                return MMDevice.AudioSessionManager2!.Sessions?.FirstOrDefault(sessionControl => sessionControl.SessionInstanceIdentifier.Equals(sessionInstanceIdentifier, StringComparison.Ordinal));
            }
        }
        #endregion (Internal) GetSessionControl

        #region (Internal) GetAllSessionControls
        internal SessionCollection? GetAllSessionControls()
        {
            lock (_audioSessionManagerLock)
            {
                return MMDevice.AudioSessionManager2!.Sessions;
            }
        }
        #endregion (Internal) GetAllSessionControls

        #region ToString
        /// <summary>
        /// Gets the FullName of this <see cref="AudioDevice"/> instance.
        /// </summary>
        /// <returns>The FullName <see cref="string"/> of this <see cref="AudioDevice"/> instance.</returns>
        public override string ToString() => FullName;
        #endregion ToString

        #endregion Methods

        #region EventHandlers

        #region AudioEndpointVolume
        /// <summary>
        /// Triggers the <see cref="VolumeChanged"/> event.
        /// </summary>
        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
            => SetVolumeAndMuteState(data.MasterVolume, data.Muted);
        #endregion AudioEndpointVolume

        #region AudioSessionManager2
        private void AudioSessionManager2_OnSessionCreated(object sender, IAudioSessionControl2 newSession)
        {
            NotifySessionCreated(newSession);
        }
        #endregion AudioSessionManager2

        #endregion EventHandlers
    }
}