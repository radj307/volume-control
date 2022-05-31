using NAudio.CoreAudioApi.Interfaces;

namespace VolumeControl.Audio.Events
{
    /// <summary>Routes windows API audio session events.</summary>
    public class SessionNotificationClient : IAudioSessionEventsHandler
    {
        public event GenericReadOnlyEventHandler<(float NativeVolume, bool Muted)>? VolumeChanged;
        public void OnVolumeChanged(float volume, bool isMuted) => VolumeChanged?.Invoke(this, new((volume, isMuted)));


        public event EventHandler? ChannelVolumeChanged;
        public void OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex) => ChannelVolumeChanged?.Invoke(this, EventArgs.Empty);


        public event GenericReadOnlyEventHandler<string>? DisplayNameChanged;
        public void OnDisplayNameChanged(string displayName) => DisplayNameChanged?.Invoke(this, new(displayName));


        public event GenericReadOnlyEventHandler<Guid>? GroupingParamChanged;
        public void OnGroupingParamChanged(ref Guid groupingId) => GroupingParamChanged?.Invoke(this, new(groupingId));


        public event GenericReadOnlyEventHandler<string>? IconPathChanged;
        public void OnIconPathChanged(string iconPath) => IconPathChanged?.Invoke(this, new(iconPath));


        public event GenericReadOnlyEventHandler<AudioSessionDisconnectReason>? Disconnected;
        public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason) => Disconnected?.Invoke(this, new(disconnectReason));


        public event GenericReadOnlyEventHandler<AudioSessionState>? StateChanged;
        public void OnStateChanged(AudioSessionState state) => StateChanged?.Invoke(this, new(state));
    }
}
