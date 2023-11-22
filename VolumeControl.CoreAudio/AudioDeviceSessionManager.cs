using CoreAudio;
using CoreAudio.Interfaces;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

namespace VolumeControl.CoreAudio
{
    /// <summary>
    /// Manages a list of <see cref="AudioSession"/> instances and their related events for a single <see cref="CoreAudio.AudioDevice"/> instance.
    /// </summary>
    /// <remarks>
    /// This class is highly coupled to <see cref="CoreAudio.AudioDevice"/>, and cannot be constructed externally.<br/>
    /// If you're looking for a session manager that works with multiple <see cref="CoreAudio.AudioDevice"/> instances, see <see cref="AudioSessionManager"/>.
    /// </remarks>
    public sealed class AudioDeviceSessionManager : IDisposable
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="AudioDeviceSessionManager"/> instance for the given <paramref name="audioDevice"/>.
        /// </summary>
        /// <param name="audioDevice">The <see cref="CoreAudio.AudioDevice"/> instance to attach this <see cref="AudioDeviceSessionManager"/> instance to.</param>
        internal AudioDeviceSessionManager(AudioDevice audioDevice)
        {
            AudioDevice = audioDevice;

            _sessions = new();

            AudioDevice.SessionCreated += this.AudioDevice_SessionCreated;

            bool showTraceLogMessages = FLog.FilterEventType(EventType.TRACE);

            var sessionCollection = AudioDevice.GetAllSessionControls();
            if (sessionCollection != null)
            {
                if (showTraceLogMessages)
                    FLog.Trace($"[{nameof(AudioDeviceSessionManager)}] {nameof(AudioDevice)} instance \"{AudioDevice}\" has {sessionCollection.Count} associated session{(sessionCollection.Count != 1 ? "s" : "")}. Initializing them now...");

                foreach (var audioSessionControl in sessionCollection)
                {
                    switch (audioSessionControl.State)
                    {
                    case AudioSessionState.AudioSessionStateExpired:
                        if (showTraceLogMessages)
                            FLog.Trace($"[{nameof(AudioDeviceSessionManager)}] Ignoring expired session with {nameof(audioSessionControl.ProcessID)} {audioSessionControl.ProcessID} and {nameof(audioSessionControl.SessionInstanceIdentifier)} \"{audioSessionControl.SessionInstanceIdentifier}\".");
                        break;
                    case AudioSessionState.AudioSessionStateInactive: //< add inactive sessions too
                    case AudioSessionState.AudioSessionStateActive:
                        CreateAndAddSessionIfUnique(audioSessionControl);
                        break;
                    }
                }
            }
            else if (showTraceLogMessages)
                FLog.Trace($"[{nameof(AudioDeviceSessionManager)}] {nameof(AudioDevice)} instance \"{AudioDevice}\" does not have any sessions! (Session collection was null!)");
        }
        #endregion Constructor

        #region Events
        /// <summary>
        /// Occurs when an <see cref="AudioSession"/> is added to the <see cref="Sessions"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? SessionAddedToList;
        private void NotifySessionAddedToList(AudioSession session) => SessionAddedToList?.Invoke(this, session);
        /// <summary>
        /// Occurs when an <see cref="AudioSession"/> is removed from the <see cref="Sessions"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? SessionRemovedFromList;
        private void NotifySessionRemovedFromList(AudioSession session) => SessionRemovedFromList?.Invoke(this, session);
        #endregion Events

        #region Properties
        /// <summary>
        /// Gets the <see cref="CoreAudio.AudioDevice"/> instance that this <see cref="AudioDeviceSessionManager"/> instance is managing.
        /// </summary>
        public AudioDevice AudioDevice { get; }
        /// <summary>
        /// Gets the list of <see cref="AudioSession"/> instances.
        /// </summary>
        public IReadOnlyList<AudioSession> Sessions => _sessions;
        /// <summary>
        /// The underlying <see cref="List{T}"/> for the <see cref="Sessions"/> property.
        /// </summary>
        private readonly List<AudioSession> _sessions;
        #endregion Properties

        #region Methods

        #region FindSession
        /// <summary>
        /// Gets the <see cref="AudioSession"/> instance associated with the given <paramref name="sessionInstanceIdentifier"/>. (<see cref="AudioSessionControl2.SessionInstanceIdentifier"/>)
        /// </summary>
        /// <param name="sessionInstanceIdentifier">The SessionInstanceIdentifier GUID of the target session.</param>
        /// <param name="comparisonType">The <see cref="StringComparison"/> type to use when comparing ID strings.</param>
        /// <returns>The <see cref="AudioSession"/> associated with the given <paramref name="sessionInstanceIdentifier"/> if found; otherwise <see langword="null"/>.</returns>
        public AudioSession? FindSessionBySessionInstanceIdentifier(string sessionInstanceIdentifier, StringComparison comparisonType = StringComparison.Ordinal)
            => Sessions.FirstOrDefault(session => session.AudioSessionControl.SessionInstanceIdentifier.Equals(sessionInstanceIdentifier, comparisonType));
        /// <summary>
        /// Gets the <see cref="AudioSession"/> instance associated with the given <paramref name="audioSessionControl"/> instance.
        /// </summary>
        /// <param name="audioSessionControl">The <see cref="AudioSessionControl2"/> instance associated with the target session.</param>
        /// <returns>The <see cref="AudioSession"/> associated with the given <paramref name="audioSessionControl"/> if found; otherwise <see langword="null"/>.</returns>
        public AudioSession? FindSessionByAudioSessionControl(AudioSessionControl2 audioSessionControl)
            => FindSessionBySessionInstanceIdentifier(audioSessionControl.SessionInstanceIdentifier);
        #endregion FindSession

        #region Add/Remove Session
        /// <summary>
        /// Creates a new <see cref="AudioSession"/> from the given <paramref name="audioSessionControl"/> and adds it to the <see cref="Sessions"/> list, and triggers the <see cref="SessionAddedToList"/> event.
        /// </summary>
        /// <param name="audioSessionControl">The <see cref="AudioSessionControl2"/> instance to create a new <see cref="AudioSession"/> from.</param>
        /// <returns>The newly-created <see cref="AudioSession"/> instance if successful; otherwise <see langword="null"/> if <paramref name="audioSessionControl"/> is already associated with another session.</returns>
        private AudioSession? CreateAndAddSessionIfUnique(AudioSessionControl2 audioSessionControl)
        {
            if (FindSessionByAudioSessionControl(audioSessionControl) is null)
            {
                try
                {
                    var newSession = new AudioSession(AudioDevice, audioSessionControl);
                    // connect important session events:
                    newSession.SessionDisconnected += this.Session_SessionDisconnected;
                    newSession.StateChanged += this.Session_StateChanged;
                    _sessions.Add(newSession);

                    NotifySessionAddedToList(newSession);

                    return newSession;
                }
                catch (Exception ex)
                {
                    FLog.Critical($"[{nameof(AudioDeviceSessionManager)}] An exception occurred while creating an {nameof(AudioSession)} instance for session with (ProcessID: {audioSessionControl.ProcessID}, DisplayName: {audioSessionControl.DisplayName}, SessionInstanceIdentifier: {audioSessionControl.SessionInstanceIdentifier})!", ex);
#if DEBUG
                    throw;
#endif
                }
            }
            return null;
        }
        /// <summary>
        /// Removes the given <paramref name="session"/> from the <see cref="Sessions"/> list and disposes of it.
        /// </summary>
        /// <param name="session">An <see cref="AudioSession"/> instance to delete.</param>
        private void RemoveSession(AudioSession session)
        {
            // remove from Sessions list
            _sessions.Remove(session);
            // disconnect events just because we can
            session.SessionDisconnected -= this.Session_SessionDisconnected;
            session.StateChanged -= this.Session_StateChanged;

            FLog.Debug($"{nameof(AudioSession)} '{session.ProcessName}' ({session.PID}) exited.");
            // notify that a session was removed
            NotifySessionRemovedFromList(session);
            // dispose of the session
            session.Dispose();
        }
        #endregion Add/Remove Session

        #endregion Methods

        #region EventHandlers

        #region Session
        private void Session_SessionDisconnected(object sender, AudioSessionDisconnectReason disconnectReason)
            => RemoveSession((AudioSession)sender);
        private void Session_StateChanged(object? sender, AudioSessionState e)
        {
            if (e.Equals(AudioSessionState.AudioSessionStateExpired) && sender is AudioSession session)
                RemoveSession(session);
        }
        #endregion Session

        #region AudioSessionManager
        private void AudioDevice_SessionCreated(object? sender, IAudioSessionControl2 newSessionControl)
        {
            newSessionControl.GetSessionInstanceIdentifier(out string newSessionInstanceIdentifier);
            AudioDevice.RefreshSessions();
            if (AudioDevice.GetSessionControl(newSessionInstanceIdentifier) is AudioSessionControl2 audioSessionControl)
            {
                if (CreateAndAddSessionIfUnique(audioSessionControl) is AudioSession newAudioSession)
                {
                    FLog.Debug($"New {nameof(AudioSession)} '{newAudioSession.ProcessName}' ({newAudioSession.PID}) created; successfully added it to the list.");
                }
                else if (FindSessionByAudioSessionControl(audioSessionControl) is AudioSession existingSession)
                {
                    FLog.Error($"New {nameof(AudioSession)} '{existingSession?.ProcessName}' ({existingSession?.PID}) created; but it was already in the list!");
                }
            }
            else
            {
                FLog.Error($"New {nameof(AudioSession)} created, but no new session was found!");
            }
        }
        #endregion AudioSessionManager

        #endregion EventHandlers

        #region IDisposable Implementation
        /// <summary>
        /// Disposes of the audio device and all managed sessions.
        /// </summary>
        ~AudioDeviceSessionManager() => Dispose();
        /// <inheritdoc/>
        public void Dispose()
        {
            AudioDevice.Dispose();
            Sessions.DisposeAll();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}