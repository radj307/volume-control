using VolumeControl.Core;

namespace VolumeControl.CoreAudio
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
            _hiddenSessions = new();

            Settings.HiddenSessionProcessNames.CollectionChanged += this.HiddenSessionProcessNames_CollectionChanged; ;
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
        /// Occurs when an <see cref="AudioSession"/> is added to the <see cref="HiddenSessions"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? SessionAddedToHiddenList;
        private void NotifySessionAddedToHiddenList(AudioSession session) => SessionAddedToHiddenList?.Invoke(this, session);
        /// <summary>
        /// Occurs when an <see cref="AudioSession"/> is removed from the <see cref="HiddenSessions"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? SessionRemovedFromHiddenList;
        private void NotifySessionRemovedFromHiddenList(AudioSession session) => SessionRemovedFromHiddenList?.Invoke(this, session);
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
        private static Config Settings => (Config.Default as Config)!;
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
        /// <summary>
        /// Gets the list of hidden <see cref="AudioSession"/> instances (<see cref="AudioSession.IsHidden"/>) currently being managed by this <see cref="AudioSessionManager"/> instance.
        /// </summary>
        public IReadOnlyList<AudioSession> HiddenSessions => _hiddenSessions;
        /// <summary>
        /// The underlying <see cref="List{T}"/> for the <see cref="HiddenSessions"/> property.
        /// </summary>
        private readonly List<AudioSession> _hiddenSessions;
        #endregion Properties

        #region FindSession
        public AudioSession? FindSession(Predicate<AudioSession> predicate)
        {
            foreach (var session in Sessions)
            {
                if (predicate(session))
                    return session;
            }
            return null;
        }
        public AudioSession? FindSessionWithID(uint pid) => FindSession((session) => session.PID.Equals(pid));
        public AudioSession? FindSessionWithProcessName(string processName, StringComparison sCompareType = StringComparison.OrdinalIgnoreCase) => FindSession((session) => session.ProcessName.Equals(processName, sCompareType));
        /// <summary>
        /// Gets a session from <see cref="Sessions"/> by parsing <paramref name="identifier"/> to determine whether to pass it to <see cref="FindSessionWithID(uint)"/>, <see cref="FindSessionWithProcessName(string, StringComparison)"/>, or directly comparing it to the <see cref="AudioSession.ProcessIdentifier"/> property.
        /// </summary>
        /// <param name="identifier">
        /// This can match any of the following properties:<br/>
        /// <list type="bullet">
        /// <item><description><b><see cref="AudioSession.PID"/></b></description></item>
        /// <item><term><b><see cref="AudioSession.ProcessName"/></b></term><description>Uses <paramref name="sCompareType"/></description></item>
        /// <item><term><b><see cref="AudioSession.ProcessIdentifier"/></b></term><description>Uses <paramref name="sCompareType"/></description></item>
        /// </list>
        /// </param>
        /// <param name="prioritizePID">When <see langword="true"/> <i>(default)</i>, process IDs are prioritized over process names, which returns more accurate results when multiple sessions have identical names but may take longer.<br/>When <see langword="false"/>, the first process with a matching ID or name is returned.</param>
        /// <param name="sCompareType">A <see cref="StringComparison"/> enum value to use when matching process names or full identifiers.</param>
        /// <returns><see cref="ISession"/> if a session was found.<br/>Returns null if nothing was found.</returns>
        public AudioSession? FindSessionWithProcessIdentifier(string identifier, bool prioritizePID = true, StringComparison sCompareType = StringComparison.OrdinalIgnoreCase)
        {
            if (identifier.Length == 0)
                return null;

            (int pid, string name) = AudioSession.ParseProcessIdentifier(identifier);

            AudioSession? potentialMatch = null;

            foreach (AudioSession session in Sessions)
            {
                if (prioritizePID && pid != -1 && session.PID.Equals(pid))
                    return session;
                else if (session.ProcessName.Equals(name, sCompareType))
                {
                    if (!prioritizePID || pid == -1)
                        return session;
                    else if (potentialMatch == null)
                        potentialMatch = session;
                    else return potentialMatch;
                }
            }

            return potentialMatch;
        }
        public AudioSession? FindSessionWithSessionInstanceIdentifier(string sessionInstanceIdentifier)
        {
            foreach (AudioSession session in Sessions)
            {
                if (session.SessionInstanceIdentifier.Equals(sessionInstanceIdentifier, StringComparison.Ordinal))
                    return session;
            }
            return null;
        }
        #endregion FindSession

        #region Methods
        /// <summary>
        /// Adds the given <paramref name="session"/> to the <see cref="Sessions"/> list, and triggers the <see cref="SessionAddedToList"/> event.
        /// </summary>
        /// <param name="session"></param>
        internal void AddSession(AudioSession session)
        {
            if (_sessions.Any(s => s.PID.Equals(session.PID)))
                return;
            if (session.IsHidden)
            { // add session to hidden list
                _hiddenSessions.Add(session);
                NotifySessionAddedToHiddenList(session);
            }
            else
            { // add session to visible list
                _sessions.Add(session);
                NotifySessionAddedToList(session);
            }
        }
        /// <summary>
        /// Removes the given <paramref name="session"/> from the <see cref="Sessions"/> list, and triggers the <see cref="SessionRemovedFromList"/> event.
        /// </summary>
        /// <param name="session">An <see cref="AudioSession"/> instance to add to the list.</param>
        internal void RemoveSession(AudioSession session)
        {
            // AudioSession.IsHidden is not reliable for this method
            if (_hiddenSessions.Remove(session))
            { // remove(d) session from hidden list
                NotifySessionRemovedFromHiddenList(session);
            }
            else
            { // remove session from visible list
                _sessions.Remove(session);
                NotifySessionRemovedFromList(session);
            }
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
                successfulCount += AddSessionManager(sessionManager) ? 1 : 0;
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
        /// <summary>
        /// Moves sessions between the <see cref="HiddenSessions"/> and <see cref="Sessions"/> lists.
        /// </summary>
        private void HiddenSessionProcessNames_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems is not null)
            { // unhide removed sessions
                foreach (var item in e.OldItems)
                {
                    var processName = (string)item;
                    if (HiddenSessions.FirstOrDefault(hiddenSession => hiddenSession.ProcessName.Equals(processName, StringComparison.Ordinal)) is AudioSession session)
                    {
                        RemoveSession(session); //< remove from hidden sessions list
                        AddSession(session); //< add to sessions list
                    }
                }
            }
            if (e.NewItems is not null)
            { // hide added sessions
                foreach (var item in e.NewItems)
                {
                    var processName = (string)item;
                    if (Sessions.FirstOrDefault(hiddenSession => hiddenSession.ProcessName.Equals(processName, StringComparison.Ordinal)) is AudioSession session)
                    {
                        RemoveSession(session); //< remove from sessions list
                        AddSession(session); //< add to hidden sessions list
                    }
                }
            }
        }
        #endregion Methods (EventHandlers)
        #endregion Methods
    }
}