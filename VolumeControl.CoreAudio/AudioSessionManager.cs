using VolumeControl.Core;
using VolumeControl.CoreAudio.Events;

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

            Settings.HiddenSessionProcessNames.CollectionChanged += this.HiddenSessionProcessNames_CollectionChanged;
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
        /// Occurs before an <see cref="AudioSession"/> is added to the <see cref="Sessions"/> list.
        /// </summary>
        /// <remarks>
        /// This allows the name of the audio session to be changed by setting the <see cref="PreviewSessionNameEventArgs.SessionName"/> property in a handler method.
        /// </remarks>
        public event PreviewSessionNameEventHandler? PreviewSessionName;
        private PreviewSessionNameEventArgs NotifyPreviewSessionName(string sessionName)
        {
            var args = new PreviewSessionNameEventArgs(sessionName);
            PreviewSessionName?.Invoke(this, args);
            return args;
        }
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

        #region Methods

        #region FindSession
        /// <summary>
        /// Finds an audio session using a <paramref name="predicate"/> function.
        /// </summary>
        /// <param name="predicate">A predicate delegate that accepts an <see cref="AudioSession"/> and returns <see langword="true"/> to indicate a match or <see langword="false"/> to continue.</param>
        /// <param name="includeHiddenSessions">When <see langword="true"/>, also searches the hidden sessions; otherwise when <see langword="false"/>, only searches through visible sessions.</param>
        /// <returns>The first <see cref="AudioSession"/> instance that the <paramref name="predicate"/> returned <see langword="true"/> for; otherwise <see langword="null"/>.</returns>
        public AudioSession? FindSession(Func<AudioSession, bool> predicate, bool includeHiddenSessions = false)
            => Sessions.FirstOrDefault(predicate) ?? (includeHiddenSessions ? HiddenSessions.FirstOrDefault(predicate) : null);
        #endregion FindSession

        #region FindSessionWithPID
        /// <summary>
        /// Finds the audio session whose owning process has the given <paramref name="processId"/>.
        /// </summary>
        /// <param name="processId">A Process ID to search for. See <see cref="AudioSession.PID"/>.</param>
        /// <param name="includeHiddenSessions">When <see langword="true"/>, also searches the hidden sessions; otherwise when <see langword="false"/>, only searches through visible sessions.</param>
        /// <returns>The <see cref="AudioSession"/> with the given <paramref name="processId"/> if found; otherwise <see langword="null"/>.</returns>
        public AudioSession? FindSessionWithPID(uint processId, bool includeHiddenSessions = false)
        { // don't use FindSession here, this way is more than 2x faster:
            for (int i = 0; i < Sessions.Count; ++i)
            {
                AudioSession session = Sessions[i];
                if (session.PID == processId) return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0; i < HiddenSessions.Count; ++i)
                {
                    AudioSession session = HiddenSessions[i];
                    if (session.PID == processId) return session;
                }
            }
            return null;
        }
        #endregion FindSessionWithPID

        #region FindSessionWithProcessName
        /// <summary>
        /// Finds the audio session whose owning process has the given <paramref name="processName"/>.
        /// </summary>
        /// <remarks>
        /// Note that this method checks ONLY the <see cref="AudioSession.ProcessName"/> property, which can differ from the name shown in the UI (<see cref="AudioSession.Name"/>)!<br/>
        /// To check both properties, use <see cref="FindSessionWithName(string, StringComparison, bool)"/> instead.
        /// </remarks>
        /// <param name="processName">A Process Name to search for. See <see cref="AudioSession.ProcessName"/>.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> type to use when comparing process name strings.</param>
        /// <param name="includeHiddenSessions">When <see langword="true"/>, also searches the hidden sessions; otherwise when <see langword="false"/>, only searches through visible sessions.</param>
        /// <returns>The first <see cref="AudioSession"/> with the given <paramref name="processName"/> if found; otherwise <see langword="null"/>.</returns>
        public AudioSession? FindSessionWithProcessName(string processName, StringComparison stringComparison = StringComparison.Ordinal, bool includeHiddenSessions = false)
        {
            if (processName.Length == 0) return null;

            for (int i = 0; i < Sessions.Count; ++i)
            {
                AudioSession session = Sessions[i];
                if (session.ProcessName.Equals(processName, stringComparison)) return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0; i < HiddenSessions.Count; ++i)
                {
                    AudioSession session = HiddenSessions[i];
                    if (session.ProcessName.Equals(processName, stringComparison)) return session;
                }
            }
            return null;
        }
        #endregion FindSessionWithProcessName

        #region FindSessionWithName
        /// <summary>
        /// Finds the audio session whose owning process has the given <paramref name="sessionName"/>.
        /// </summary>
        /// <param name="sessionName">A Name or Process Name to search for. See <see cref="AudioSession.Name"/> &amp; <see cref="AudioSession.ProcessName"/>.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> type to use when comparing process name strings.</param>
        /// <param name="includeHiddenSessions">When <see langword="true"/>, also searches the hidden sessions; otherwise when <see langword="false"/>, only searches through visible sessions.</param>
        /// <returns>The first <see cref="AudioSession"/> with the given <paramref name="sessionName"/> if found; otherwise <see langword="null"/>.</returns>
        public AudioSession? FindSessionWithName(string sessionName, StringComparison stringComparison = StringComparison.Ordinal, bool includeHiddenSessions = false)
        {
            if (sessionName.Length == 0) return null;

            for (int i = 0; i < Sessions.Count; ++i)
            {
                AudioSession session = Sessions[i];
                if ((session.HasCustomName && session.Name.Equals(sessionName, stringComparison))
                    || session.ProcessName.Equals(sessionName, stringComparison)) return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0; i < HiddenSessions.Count; ++i)
                {
                    AudioSession session = HiddenSessions[i];
                    if ((session.HasCustomName && session.Name.Equals(sessionName, stringComparison))
                        || session.ProcessName.Equals(sessionName, stringComparison)) return session;
                }
            }
            return null;
        }
        #endregion FindSessionWithName

        #region FindSessionWithExactProcessIdentifier
        /// <summary>
        /// Finds the audio session with the given <paramref name="processIdentifier"/>.
        /// </summary>
        /// <param name="processIdentifier">A Process Identifier to search for. See <see cref="AudioSession.ProcessIdentifier"/>.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> type to use when comparing process identifier strings.</param>
        /// <param name="includeHiddenSessions">When <see langword="true"/>, also searches the hidden sessions; otherwise when <see langword="false"/>, only searches through visible sessions.</param>
        /// <returns>The <see cref="AudioSession"/> with the given <paramref name="processIdentifier"/> if found; otherwise <see langword="null"/>.</returns>
        public AudioSession? FindSessionWithExactProcessIdentifier(string processIdentifier, StringComparison stringComparison = StringComparison.Ordinal, bool includeHiddenSessions = false)
        {
            if (processIdentifier.Length == 0) return null;

            for (int i = 0; i < Sessions.Count; ++i)
            {
                AudioSession session = Sessions[i];
                if (session.ProcessIdentifier.Equals(processIdentifier, stringComparison)) return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0; i < HiddenSessions.Count; ++i)
                {
                    AudioSession session = HiddenSessions[i];
                    if (session.ProcessIdentifier.Equals(processIdentifier, stringComparison)) return session;
                }
            }
            return null;
        }
        #endregion FindSessionWithExactProcessIdentifier

        #region FindSessionWithSimilarProcessIdentifier
        /// <summary>
        /// Finds the audio session that matches - or is very similar to - the given <paramref name="processIdentifier"/>.
        /// </summary>
        /// <remarks>
        /// When searching for text entered by the user, this method should always be used over the other FindSession... methods.
        /// </remarks>
        /// <param name="processIdentifier">A ProcessIdentifier to search for. See <see cref="AudioSession.ProcessIdentifier"/>.<br/>
        /// Supports partial identifiers that only include the PID or ProcessName component, with or without the separator character.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> type to use when comparing strings.</param>
        /// <param name="includeHiddenSessions">When <see langword="true"/>, also searches the hidden sessions; otherwise when <see langword="false"/>, only searches through visible sessions.</param>
        /// <returns>
        /// An <see cref="AudioSession"/> that matches - or is similar to - the given <paramref name="processIdentifier"/> if found; otherwise <see langword="null"/>.
        /// <br/>
        /// The returned session can have a different PID than the one specified by the <paramref name="processIdentifier"/>.
        /// </returns>
        public AudioSession? FindSessionWithSimilarProcessIdentifier(string processIdentifier, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase, bool includeHiddenSessions = false)
        {
            // remove preceding/trailing whitespace & colons (separator char)
            string s = processIdentifier.Trim(AudioSession.ProcessIdentifierSeparatorChar, ' ', '\t', '\v', '\r', '\n');

            if (s.Length == 0) return null;

            int delimIndex = s.IndexOf(AudioSession.ProcessIdentifierSeparatorChar);

            if (delimIndex == -1)
            { // no separator; only 1 segment is present:
                if (s.All(char.IsNumber))
                { // PID segment is present:
                    return FindSessionWithPID(uint.Parse(s), includeHiddenSessions);
                }
                else
                { // ProcessName segment is present:
                    return FindSessionWithName(s, stringComparison, includeHiddenSessions); //< check both Name & ProcessName
                }
            }
            else
            { // both of the segments exist:
                string name = s[(delimIndex + 1)..];

                if (uint.TryParse(s[..delimIndex], out uint pid)
                    && FindSessionWithPID(pid, includeHiddenSessions) is AudioSession session
                    && (session.ProcessName.Equals(name, stringComparison) || (session.HasCustomName && session.Name.Equals(name, stringComparison))))
                { // found a session with a matching process ID and ProcessName
                    return session;
                }
                else return FindSessionWithName(name, stringComparison, includeHiddenSessions);
            }
        }
        #endregion FindSessionWithSimilarProcessIdentifier

        #region FindSessionWithSessionIdentifier
        /// <summary>
        /// Finds the first audio session with the specified <paramref name="sessionIdentifier"/>.
        /// </summary>
        /// <param name="sessionIdentifier">A SessionIdentifier string to search for. See <see cref="AudioSession.SessionIdentifier"/>.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> type to use when comparing session identifier strings.</param>
        /// <param name="includeHiddenSessions">When <see langword="true"/>, also searches the hidden sessions; otherwise when <see langword="false"/>, only searches through visible sessions.</param>
        /// <returns>The first <see cref="AudioSession"/> with the given <paramref name="sessionIdentifier"/> if found; otherwise <see langword="null"/>.</returns>
        public AudioSession? FindSessionWithSessionIdentifier(string sessionIdentifier, StringComparison stringComparison = StringComparison.Ordinal, bool includeHiddenSessions = false)
        {
            if (sessionIdentifier.Length == 0) return null;

            for (int i = 0; i < Sessions.Count; ++i)
            {
                AudioSession session = Sessions[i];
                if (session.SessionIdentifier.Equals(sessionIdentifier, stringComparison)) return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0; i < HiddenSessions.Count; ++i)
                {
                    AudioSession session = HiddenSessions[i];
                    if (session.SessionIdentifier.Equals(sessionIdentifier, stringComparison)) return session;
                }
            }
            return null;
        }
        #endregion FindSessionWithSessionIdentifier

        #region FindSessionWithSessionInstanceIdentifier
        /// <summary>
        /// Finds the audio session with the specified <paramref name="sessionInstanceIdentifier"/>.
        /// </summary>
        /// <param name="sessionInstanceIdentifier">A SessionInstanceIdentifier to search for. See <see cref="AudioSession.SessionInstanceIdentifier"/>.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> type to use when comparing session instance identifier strings.</param>
        /// <param name="includeHiddenSessions">When <see langword="true"/>, also searches the hidden sessions; otherwise when <see langword="false"/>, only searches through visible sessions.</param>
        /// <returns>The <see cref="AudioSession"/> with the given <paramref name="sessionInstanceIdentifier"/> if found; otherwise <see langword="null"/>.</returns>
        public AudioSession? FindSessionWithSessionInstanceIdentifier(string sessionInstanceIdentifier, StringComparison stringComparison = StringComparison.Ordinal, bool includeHiddenSessions = false)
        {
            if (sessionInstanceIdentifier.Length == 0) return null;

            for (int i = 0; i < Sessions.Count; ++i)
            {
                AudioSession session = Sessions[i];
                if (session.SessionInstanceIdentifier.Equals(sessionInstanceIdentifier, stringComparison)) return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0; i < HiddenSessions.Count; ++i)
                {
                    AudioSession session = HiddenSessions[i];
                    if (session.SessionInstanceIdentifier.Equals(sessionInstanceIdentifier, stringComparison)) return session;
                }
            }
            return null;
        }
        #endregion FindSessionWithSessionInstanceIdentifier

        #region Add/Remove Session
        /// <summary>
        /// Adds the given <paramref name="session"/> to the managed sessions.
        /// </summary>
        /// <param name="session">An <see cref="AudioSession"/> instance to add to the list.</param>
        internal void AddSession(AudioSession session)
        {
            if (_sessions.Any(s => s.PID.Equals(session.PID)))
                return;

            // allow handlers to edit the session's initial name:
            session.Name = NotifyPreviewSessionName(session.Name).SessionName;

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
        /// Removes the specified <paramref name="session"/> from the managed sessions.
        /// </summary>
        /// <param name="session">An <see cref="AudioSession"/> instance to remove from the list.</param>
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
        #endregion Add/Remove Session

        #region Add/Remove SessionManager
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
        #endregion Add/Remove SessionManager

        #endregion Methods

        #region EventHandlers

        #region SessionManager
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
        #endregion SessionManager

        #region HiddenSessionProcessNames
        /// <summary>
        /// Moves sessions between the <see cref="HiddenSessions"/> and <see cref="Sessions"/> lists.
        /// </summary>
        private void HiddenSessionProcessNames_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems is not null)
            { // unhide removed sessions
                foreach (var item in e.OldItems)
                {
                    var name = (string)item;

                    // we have to enumerate the entire list here because of applications 
                    //  like Discord that have multiple identically-named audio sessions:
                    for (int i = HiddenSessions.Count - 1; i >= 0; --i)
                    {
                        var hiddenSession = HiddenSessions[i];
                        if (hiddenSession.ProcessName.Equals(name, StringComparison.Ordinal)
                            || (hiddenSession.HasCustomName && hiddenSession.Name.Equals(name, StringComparison.Ordinal)))
                        {
                            RemoveSession(hiddenSession); //< remove from hidden sessions list
                            AddSession(hiddenSession); //< add to sessions list
                        }
                    }

                    //if (HiddenSessions.FirstOrDefault(hiddenSession => hiddenSession.ProcessName.Equals(sessionName, StringComparison.Ordinal)) is AudioSession session)
                    //{
                    //    RemoveSession(session); //< remove from hidden sessions list
                    //    AddSession(session); //< add to sessions list
                    //}
                }
            }
            if (e.NewItems is not null)
            { // hide added sessions
                foreach (var item in e.NewItems)
                {
                    var name = (string)item;

                    // we have to enumerate the entire list here because of applications 
                    //  like Discord that have multiple identically-named audio sessions:
                    for (int i = Sessions.Count - 1; i >= 0; --i)
                    {
                        var session = Sessions[i];
                        if (session.ProcessName.Equals(name, StringComparison.Ordinal)
                            || (session.HasCustomName && session.Name.Equals(name, StringComparison.Ordinal)))
                        {
                            RemoveSession(session); //< remove from hidden sessions list
                            AddSession(session); //< add to sessions list
                        }
                    }

                    //if (Sessions.FirstOrDefault(hiddenSession => hiddenSession.ProcessName.Equals(sessionName, StringComparison.Ordinal)) is AudioSession session)
                    //{
                    //    RemoveSession(session); //< remove from sessions list
                    //    AddSession(session); //< add to hidden sessions list
                    //}
                }
            }
        }
        #endregion HiddenSessionProcessNames

        #endregion EventHandlers
    }
}