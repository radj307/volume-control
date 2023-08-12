namespace Audio
{
    /// <summary>
    /// Manages a list of <see cref="AudioSession"/> instances and their related events for any number of <see cref="AudioDeviceSessionManager"/> instances.
    /// </summary>
    /// <remarks>
    /// <see cref="AudioDeviceSessionManager"/> instances can be retrieved from <see cref="AudioDevice.SessionManager"/>
    /// </remarks>
    public sealed class AudioSessionManager
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="AudioSessionManager"/> instance without any <see cref="SessionManagers"/>.
        /// </summary>
        public AudioSessionManager()
        {
            _sessionManagers = new();
            _sessions = new();
        }
        /// <summary>
        /// Creates a new <see cref="AudioSessionManager"/> instance with the given <paramref name="sessionManagers"/>.
        /// </summary>
        /// <param name="sessionManagers">Any number of <see cref="AudioDeviceSessionManager"/> instances to add to the <see cref="SessionManagers"/> list.</param>
        public AudioSessionManager(IEnumerable<AudioDeviceSessionManager> sessionManagers) : this()
        {
            foreach (var sessionManager in sessionManagers)
            {
                AddSessionManager(sessionManager);
            }
        }
        #endregion Constructors

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
        /// <summary>
        /// Occurs when an <see cref="AudioDeviceSessionManager"/> is added to the <see cref="SessionManagers"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioDeviceSessionManager>? SessionManagerAddedToList;
        private void NotifySessionManagerAddedToList(AudioDeviceSessionManager sessionManager) => SessionManagerAddedToList?.Invoke(this, sessionManager);
        /// <summary>
        /// Occurs when an <see cref="AudioDeviceSessionManager"/> is removed from the <see cref="SessionManagers"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioDeviceSessionManager>? SessionManagerRemovedFromList;
        private void NotifySessionManagerRemovedFromList(AudioDeviceSessionManager sessionManager) => SessionManagerRemovedFromList?.Invoke(this, sessionManager);
        #endregion Events

        #region Properties
        /// <summary>
        /// Gets the list of <see cref="AudioDeviceSessionManager"/> instances currently being managed by this <see cref="AudioSessionManager"/> instance.
        /// </summary>
        public IReadOnlyList<AudioDeviceSessionManager> SessionManagers => _sessionManagers;
        /// <summary>
        /// The underlying <see cref="List{T}"/> for the <see cref="SessionManagers"/> property.
        /// </summary>
        private readonly List<AudioDeviceSessionManager> _sessionManagers;
        /// <summary>
        /// Gets the list of <see cref="AudioSession"/> instances currently being managed by this <see cref="AudioSessionManager"/> instance.
        /// </summary>
        public IReadOnlyList<AudioSession> Sessions => _sessions;
        /// <summary>
        /// The underlying <see cref="List{T}"/> for the <see cref="Sessions"/> property.
        /// </summary>
        private readonly List<AudioSession> _sessions;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Adds the given <paramref name="session"/> to the <see cref="Sessions"/> list, and triggers the <see cref="SessionAddedToList"/> event.
        /// </summary>
        /// <param name="session"></param>
        internal void AddSession(AudioSession session)
        {
            if (_sessions.Any(s => s.PID.Equals(session.PID)))
                return;
            _sessions.Add(session);
            NotifySessionAddedToList(session);
        }
        /// <summary>
        /// Removes the given <paramref name="session"/> from the <see cref="Sessions"/> list, and triggers the <see cref="SessionRemovedFromList"/> event.
        /// </summary>
        /// <param name="session">An <see cref="AudioSession"/> instance to add to the list.</param>
        internal void RemoveSession(AudioSession session)
        {
            _sessions.Remove(session);
            NotifySessionRemovedFromList(session);
        }
        /// <summary>
        /// Adds the given <paramref name="sessionManager"/> to the <see cref="SessionManagers"/> list.
        /// </summary>
        /// <param name="sessionManager">A <see cref="AudioDeviceSessionManager"/> instance to add.</param>
        /// <returns><see langword="true"/> when successful; otherwise <see langword="false"/> if the <paramref name="sessionManager"/> is already in the list.</returns>
        public bool AddSessionManager(AudioDeviceSessionManager sessionManager)
        {
            if (SessionManagers.Contains(sessionManager))
                return false;

            _sessionManagers.Add(sessionManager);

            sessionManager.SessionAddedToList += SessionManager_SessionAddedToList;
            sessionManager.SessionRemovedFromList += this.SessionManager_SessionRemovedFromList;

            NotifySessionManagerAddedToList(sessionManager);

            foreach (var session in sessionManager.Sessions)
            {
                AddSession(session);
            }
            return true;
        }
        /// <summary>
        /// Adds the given <paramref name="sessionManagers"/> to the <see cref="SessionManagers"/> list using <see cref="AddSessionManager(AudioDeviceSessionManager)"/>.
        /// </summary>
        /// <param name="sessionManagers">Any number of <see cref="AudioDeviceSessionManager"/> instances to add.</param>
        /// <returns>The number of <paramref name="sessionManagers"/> that were successfully added to the list.</returns>
        public int AddSessionManagers(IEnumerable<AudioDeviceSessionManager> sessionManagers)
        {
            int successfulCount = 0;
            foreach (var sessionManager in sessionManagers)
            {
                successfulCount += (AddSessionManager(sessionManager) ? 1 : 0);
            }
            return successfulCount;
        }
        /// <summary>
        /// Removes the given <paramref name="sessionManager"/> from the <see cref="SessionManagers"/> list.
        /// </summary>
        /// <param name="sessionManager">A <see cref="AudioDeviceSessionManager"/> instance to remove.</param>
        /// <returns><see langword="true"/> when successful; otherwise <see langword="false"/> when the <paramref name="sessionManager"/> wasn't in the list.</returns>
        public bool RemoveSessionManager(AudioDeviceSessionManager sessionManager)
        {
            if (!SessionManagers.Contains(sessionManager)) return false;

            _sessionManagers.Remove(sessionManager);

            sessionManager.SessionAddedToList -= SessionManager_SessionAddedToList;
            sessionManager.SessionRemovedFromList -= SessionManager_SessionRemovedFromList;

            NotifySessionManagerRemovedFromList(sessionManager);

            foreach (var session in sessionManager.Sessions)
            {
                RemoveSession(session);
            }
            return true;
        }
        #region Methods (EventHandlers)
        /// <summary>
        /// Calls <see cref="AddSession(AudioSession)"/>.
        /// </summary>
        private void SessionManager_SessionAddedToList(object? sender, AudioSession e)
            => AddSession(e);
        /// <summary>
        /// Calls <see cref="RemoveSession(AudioSession)"/>.
        /// </summary>
        private void SessionManager_SessionRemovedFromList(object? sender, AudioSession e)
            => RemoveSession(e);
        #endregion Methods (EventHandlers)
        #endregion Methods
    }
}