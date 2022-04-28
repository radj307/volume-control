using AudioAPI.WindowsAPI.Audio;

namespace AudioAPI
{
    public class SimpleVolumeChangedEventArgs : EventArgs
    {
        public SimpleVolumeChangedEventArgs(float volume, bool muted)
        {
            Volume = volume;
            Muted = muted;
        }

        public float Volume { get; }
        public bool Muted { get; }
    }
    public delegate void SimpleVolumeChangedEventHandler(object? sender, SimpleVolumeChangedEventArgs e);

    public class AudioEventHook : IAudioSessionEvents
    {
        public AudioEventHook(IAudioSessionControl sessionControl)
        {
            _control = sessionControl;
            _control.RegisterAudioSessionNotifications(this);
        }
        ~AudioEventHook()
        {
            _control.UnregisterAudioSessionNotifications(this);
        }

        private readonly IAudioSessionControl _control;
        private const int S_OK = 0;

        public event SimpleVolumeChangedEventHandler? SimpleVolumeChanged = null;
        private void NotifySimpleVolumeChanged(SimpleVolumeChangedEventArgs e) => SimpleVolumeChanged?.Invoke(this, e);
        public int OnSimpleVolumeChanged(float NewVolume, bool NewMute, ref Guid EventContext)
        {
            NotifySimpleVolumeChanged(new(NewVolume, NewMute));
            return S_OK;
        }
    }
}
