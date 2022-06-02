using NAudio.CoreAudioApi.Interfaces;

namespace VolumeControl.Audio.Events
{
    /// <summary>Receives Core Audio API events for a single audio session.</summary>
    internal sealed class SessionNotificationClient : IAudioSessionEventsHandler
    {
        /// <summary>Triggered when an audio session's volume or mute state was changed.</summary>
        public event EventHandler<(float NativeVolume, bool Muted)>? VolumeChanged;
        public void OnVolumeChanged(float volume, bool isMuted) => VolumeChanged?.Invoke(this, (volume, isMuted));
        /// <summary>Triggered when the audio session's channel volumes were changed.</summary>
        public event EventHandler? ChannelVolumeChanged;
        public void OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex) => ChannelVolumeChanged?.Invoke(this, EventArgs.Empty);
        /// <summary>Triggered when the audio session's display name was changed.</summary>
        public event EventHandler<string>? DisplayNameChanged;
        public void OnDisplayNameChanged(string displayName) => DisplayNameChanged?.Invoke(this, displayName);
        /// <summary>Triggered when the audio session's grouping parameter was changed.</summary>
        public event EventHandler<Guid>? GroupingParamChanged;
        public void OnGroupingParamChanged(ref Guid groupingId) => GroupingParamChanged?.Invoke(this, groupingId);
        /// <summary>Triggered when the audio session's icon path property was changed.</summary>
        public event EventHandler<string>? IconPathChanged;
        public void OnIconPathChanged(string iconPath) => IconPathChanged?.Invoke(this, iconPath);
        /// <summary>Triggered when the audio session was disconnected.</summary>
        public event EventHandler<AudioSessionDisconnectReason>? Disconnected;
        public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason) => Disconnected?.Invoke(this, disconnectReason);
        /// <summary>Triggered when the audio session's state was changed.</summary>
        public event EventHandler<AudioSessionState>? StateChanged;
        public void OnStateChanged(AudioSessionState state) => StateChanged?.Invoke(this, state);
    }
}
