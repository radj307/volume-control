using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core;
using VolumeControl.CoreAudio;

namespace VolumeControl.ViewModels
{
    public class SessionSyncVM : INotifyPropertyChanged
    {
        #region Constructor
        public SessionSyncVM(AudioDeviceManagerVM audioAPI)
        {
            AudioAPI = audioAPI;

            AudioSessionMultiSelector.SessionSelected += this.AudioSessionMultiSelector_SessionSelected;
            AudioSessionMultiSelector.SessionDeselected += this.AudioSessionMultiSelector_SessionDeselected;
            Settings.PropertyChanged += this.Settings_PropertyChanged;
        }
        #endregion Constructor

        #region Properties
        private static Config Settings => Config.Default;
        private AudioSessionMultiSelector AudioSessionMultiSelector => AudioAPI.AudioSessionMultiSelector;
        private AudioDeviceManagerVM AudioAPI { get; }
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged();
            }
        }
        private bool _isEnabled;
        public AudioSession? BaselineSession { get; private set; }
        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Methods
        /// <summary>
        /// Syncs the volume levels & mute states of the specified <paramref name="sessions"/> with the <paramref name="baseline"/> session.
        /// </summary>
        /// <param name="baseline">The session to set the volume &amp; mute state of the <paramref name="sessions"/> to.</param>
        /// <param name="sessions">Any number of audio sessions to sync the volume &amp; mute state of.</param>
        public static void SyncSessions(AudioSession baseline, params AudioSession[] sessions)
        {
            foreach (var session in sessions)
            {
                session.Volume = baseline.Volume;
                session.Mute = baseline.Mute;
            }
        }
        #endregion Methods

        #region EventHandlers

        #region AudioSession
        private void AudioSession_VolumeChanged(CoreAudio.Interfaces.IAudioControl? sender, CoreAudio.Events.VolumeChangedEventArgs e)
        {
            foreach (var session in AudioSessionMultiSelector.SelectedSessions)
            {
                if (session.Volume != sender!.Volume)
                {
                    session.Volume = e.Volume;
                }
                else if (session.Mute != sender!.Mute)
                {
                    session.Mute = e.Mute;
                }
            }
        }
        #endregion AudioSession

        #region AudioSessionMultiSelector
        private void AudioSessionMultiSelector_SessionSelected(object? sender, AudioSession e)
        {
            e.VolumeChanged += this.AudioSession_VolumeChanged;

            if (!IsEnabled) return;

            if (BaselineSession != null)
            {
                SyncSessions(BaselineSession, e);
            }
            else BaselineSession = e;
        }
        private void AudioSessionMultiSelector_SessionDeselected(object? sender, AudioSession e)
        {
            e.VolumeChanged -= this.AudioSession_VolumeChanged;

            if (!AudioSessionMultiSelector.HasSelectedSessions)
            {
                BaselineSession = null;
            }
            else if (e.Equals(BaselineSession))
            {
                BaselineSession = AudioSessionMultiSelector.SelectedSessions[0];
            }
        }
        #endregion AudioSessionMultiSelector

        #region Settings
        private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null) return;

            if (e.PropertyName.Equals(nameof(Config.EnableSessionSync), System.StringComparison.Ordinal))
            {
                _isEnabled = Settings.EnableSessionSync;
                NotifyPropertyChanged(nameof(IsEnabled));
            }
        }
        #endregion Settings

        #endregion EventHandlers
    }
}
