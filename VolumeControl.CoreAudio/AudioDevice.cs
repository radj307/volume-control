﻿using CoreAudio;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.CoreAudio.Events;
using VolumeControl.CoreAudio.Helpers;
using VolumeControl.CoreAudio.Interfaces;
using VolumeControl.Log;

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

            if (MMDevice.AudioSessionManager2 is null)
                throw new NullReferenceException($"{nameof(AudioDevice)} '{Name}' has a null {nameof(MMDevice.AudioSessionManager2)} property!");
            if (MMDevice.AudioEndpointVolume is null)
                throw new NullReferenceException($"{nameof(AudioDevice)} '{Name}' has a null {nameof(MMDevice.AudioEndpointVolume)} property!");
            if (MMDevice.AudioMeterInformation is null)
                throw new NullReferenceException($"{nameof(AudioDevice)} '{Name}' has a null {nameof(MMDevice.AudioMeterInformation)} property!");

            SessionManager = new(this); //< TODO: try-catch?

            AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;

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
        private void NotifyVolumeChanged(AudioVolumeNotificationData data) => VolumeChanged?.Invoke(this, new(data));
        #endregion Events

        #region Fields
        /// <summary>
        /// Used to sequence calls to the <see cref="AudioEndpointVolume"/> property.
        /// </summary>
        private readonly object lock_AudioEndpointVolume = new();
        /// <summary>
        /// Used to prevent duplicate <see cref="PropertyChanged"/> events from being fired.
        /// </summary>
        private bool isNotifying = false;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets the underlying <see cref="MMDevice"/> for this <see cref="AudioDevice"/> instance.
        /// </summary>
        internal MMDevice MMDevice { get; }
        internal AudioEndpointVolume AudioEndpointVolume //< disposed of by MMDevice
        {
            get
            {
                // sequence calls to this device's AudioEndpointVolume instance to prevent NO_INTERFACE COM exceptions
                lock (lock_AudioEndpointVolume)
                {
                    return MMDevice.AudioEndpointVolume!;
                }
            }
        }
        internal AudioSessionManager2 AudioSessionManager //< disposed of by MMDevice
            => MMDevice.AudioSessionManager2!;
        internal AudioMeterInformation AudioMeterInformation
            => MMDevice.AudioMeterInformation!;
        /// <summary>
        /// Gets the <see cref="AudioDeviceSessionManager"/> instance that manages this <see cref="AudioDevice"/> instance's <see cref="AudioSession"/>s.
        /// </summary>
        public AudioDeviceSessionManager SessionManager { get; }
        /// <summary>
        /// Gets the DataFlow direction of this audio device.
        /// </summary>
        public DataFlow DataFlow => MMDevice.DataFlow;
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
            get => AudioEndpointVolume.MasterVolumeLevelScalar;
            set
            {
                if (value < 0.0f)
                    value = 0.0f;
                else if (value > 1.0f)
                    value = 1.0f;

                AudioEndpointVolume.MasterVolumeLevelScalar = value;
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
            get => AudioEndpointVolume.Mute;
            set
            {
                AudioEndpointVolume.Mute = value;
                NotifyPropertyChanged();
            }
        }
        #endregion IAudioControl Implementation

        #region IAudioPeakMeter Implementation
        /// <inheritdoc/>
        public float PeakMeterValue
            => AudioMeterInformation.MasterPeakValue;
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
        {
            NativeVolume = data.MasterVolume;
            Mute = data.Muted;
            NotifyVolumeChanged(data);
        }
        #endregion AudioEndpointVolume

        #endregion EventHandlers
    }
}