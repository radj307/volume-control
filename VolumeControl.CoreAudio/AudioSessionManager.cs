using CoreAudio;
using VolumeControl.CoreAudio.Events;
using VolumeControl.TypeExtensions;

namespace VolumeControl.CoreAudio
{
    /// <summary>
    /// Manages a list of <see cref="AudioSession"/> instances and their related events for any number of <see cref="AudioDeviceSessionManager"/> instances.
    /// </summary>
    /// <remarks>
    /// <see cref="AudioDeviceSessionManager"/> instances can be retrieved from <see cref="AudioDevice.SessionManager"/>
    /// </remarks>
    public sealed class AudioSessionManager : IDisposable
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
        /// Occurs before an <see cref="AudioSession"/> is added to the Sessions list to get its initial name.
        /// </summary>
        /// <remarks>
        /// This allows the name of the audio session to be changed by setting the <see cref="PreviewSessionNameEventArgs.SessionName"/> property in a handler method.
        /// </remarks>
        public event PreviewSessionNameEventHandler? PreviewSessionName;
        private PreviewSessionNameEventArgs NotifyPreviewSessionName(AudioSession audioSession)
        {
            var args = new PreviewSessionNameEventArgs(audioSession, audioSession.Name);
            PreviewSessionName?.Invoke(this, args);
            return args;
        }
        /// <summary>
        /// Occurs before an <see cref="AudioSession"/> is added to the Sessions list to get its initial hidden state.
        /// </summary>
        /// <remarks>
        /// This allows handler methods to set whether the audio session is hidden by default.
        /// </remarks>
        public event PreviewSessionIsHiddenEventHandler? PreviewSessionIsHidden;
        private PreviewSessionIsHiddenEventArgs NotifyPreviewSessionIsHidden(AudioSession audioSession)
        {
            var args = new PreviewSessionIsHiddenEventArgs(audioSession, audioSession.IsHidden);
            PreviewSessionIsHidden?.Invoke(this, args);
            return args;
        }

        #region List
        /// <summary>
        /// Occurs prior to an <see cref="AudioSession"/> being added to the <see cref="Sessions"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? AddingSessionToList;
        private void NotifyAddingSessionToList(AudioSession session) => AddingSessionToList?.Invoke(this, session);
        /// <summary>
        /// Occurs when an <see cref="AudioSession"/> is added to the <see cref="Sessions"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? AddedSessionToList;
        private void NotifyAddedSessionToList(AudioSession session) => AddedSessionToList?.Invoke(this, session);
        /// <summary>
        /// Occurs prior to an <see cref="AudioSession"/> being removed from the <see cref="Sessions"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? RemovingSessionFromList;
        private void NotifyRemovingSessionFromList(AudioSession session) => RemovingSessionFromList?.Invoke(this, session);
        /// <summary>
        /// Occurs when an <see cref="AudioSession"/> is removed from the <see cref="Sessions"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? RemovedSessionFromList;
        private void NotifyRemovedSessionFromList(AudioSession session) => RemovedSessionFromList?.Invoke(this, session);
        #endregion List

        #region HiddenList
        /// <summary>
        /// Occurs prior to an <see cref="AudioSession"/> being added to the <see cref="HiddenSessions"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? AddingSessionToHiddenList;
        private void NotifyAddingSessionToHiddenList(AudioSession session) => AddingSessionToHiddenList?.Invoke(this, session);
        /// <summary>
        /// Occurs when an <see cref="AudioSession"/> is added to the <see cref="HiddenSessions"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? AddedSessionToHiddenList;
        private void NotifyAddedSessionToHiddenList(AudioSession session) => AddedSessionToHiddenList?.Invoke(this, session);
        /// <summary>
        /// Occurs prior to an <see cref="AudioSession"/> being removed from the <see cref="HiddenSessions"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? RemovingSessionFromHiddenList;
        private void NotifyRemovingSessionFromHiddenList(AudioSession session) => RemovingSessionFromHiddenList?.Invoke(this, session);
        /// <summary>
        /// Occurs when an <see cref="AudioSession"/> is removed from the <see cref="HiddenSessions"/> list for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? RemovedSessionFromHiddenList;
        private void NotifyRemovedSessionFromHiddenList(AudioSession session) => RemovedSessionFromHiddenList?.Invoke(this, session);
        #endregion HiddenList

        #region SessionManager
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
        #endregion SessionManager

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
        /// <param name="dataFlow">The data flow type of the session to search for.</param>
        /// <param name="includeHiddenSessions">When <see langword="true"/>, also searches the hidden sessions; otherwise when <see langword="false"/>, only searches through visible sessions.</param>
        /// <returns>The <see cref="AudioSession"/> with the given <paramref name="processId"/> if found; otherwise <see langword="null"/>.</returns>
        public AudioSession? FindSessionWithPID(uint processId, DataFlow? dataFlow, bool includeHiddenSessions = false)
        { // don't use FindSession here, this way is more than 2x faster:
            for (int i = 0, max = Sessions.Count; i < max; ++i)
            {
                AudioSession session = Sessions[i];
                if (session.PID == processId && session.DataFlow == dataFlow) return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0, max = HiddenSessions.Count; i < max; ++i)
                {
                    AudioSession session = HiddenSessions[i];
                    if (session.PID == processId && session.DataFlow == dataFlow) return session;
                }
            }
            return null;
        }
        /// <summary>
        /// Finds the audio session whose owning process has the given <paramref name="processId"/>.
        /// </summary>
        /// <param name="processId">A Process ID to search for. See <see cref="AudioSession.PID"/>.</param>
        /// <param name="includeHiddenSessions">When <see langword="true"/>, also searches the hidden sessions; otherwise when <see langword="false"/>, only searches through visible sessions.</param>
        /// <returns>The <see cref="AudioSession"/> with the given <paramref name="processId"/> if found; otherwise <see langword="null"/>.</returns>
        public AudioSession? FindSessionWithPID(uint processId, bool includeHiddenSessions = false)
        { // don't use FindSession here, this way is more than 2x faster:
            for (int i = 0, max = Sessions.Count; i < max; ++i)
            {
                AudioSession session = Sessions[i];
                if (session.PID == processId) return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0, max = HiddenSessions.Count; i < max; ++i)
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

            for (int i = 0, max = Sessions.Count; i < max; ++i)
            {
                AudioSession session = Sessions[i];
                if (session.ProcessName.Equals(processName, stringComparison)) return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0, max = HiddenSessions.Count; i < max; ++i)
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
        /// <param name="dataFlow">The data flow type of the session to search for.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> type to use when comparing process name strings.</param>
        /// <param name="includeHiddenSessions">When <see langword="true"/>, also searches the hidden sessions; otherwise when <see langword="false"/>, only searches through visible sessions.</param>
        /// <returns>The first <see cref="AudioSession"/> with the given <paramref name="sessionName"/> if found; otherwise <see langword="null"/>.</returns>
        public AudioSession? FindSessionWithName(string sessionName, DataFlow? dataFlow, StringComparison stringComparison = StringComparison.Ordinal, bool includeHiddenSessions = false)
        {
            if (sessionName.Length == 0) return null;

            for (int i = 0, max = Sessions.Count; i < max; ++i)
            {
                AudioSession session = Sessions[i];
                if ((dataFlow == null || session.DataFlow == dataFlow) && session.HasMatchingName(sessionName, stringComparison))
                    return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0, max = HiddenSessions.Count; i < max; ++i)
                {
                    AudioSession session = HiddenSessions[i];
                    if ((dataFlow == null || session.DataFlow == dataFlow) && session.HasMatchingName(sessionName, stringComparison))
                        return session;
                }
            }
            return null;
        }
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

            for (int i = 0, max = Sessions.Count; i < max; ++i)
            {
                AudioSession session = Sessions[i];
                if (session.HasMatchingName(sessionName, stringComparison))
                    return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0, max = HiddenSessions.Count; i < max; ++i)
                {
                    AudioSession session = HiddenSessions[i];
                    if (session.HasMatchingName(sessionName, stringComparison))
                        return session;
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

            for (int i = 0, max = Sessions.Count; i < max; ++i)
            {
                AudioSession session = Sessions[i];
                if (session.ProcessIdentifier.Equals(processIdentifier, stringComparison)) return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0, max = HiddenSessions.Count; i < max; ++i)
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
            
            var segments = s.Split(AudioSession.ProcessIdentifierSeparatorChar, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // parse the data flow segment
            DataFlow? dataFlow = null;
            if (segments.Length >= 3 && segments[2].Length >= 1)
            {
                var ch = segments[2][0];

                if (ch == AudioSession.ProcessIdentifierInputChar)
                    dataFlow = DataFlow.Capture;
                else if (ch == AudioSession.ProcessIdentifierOutputChar)
                    dataFlow = DataFlow.Render;
            }

            if (segments.Length >= 2)
            { // both PID & PNAME segments exist:
                string name = segments[1];

                if (uint.TryParse(segments[0], out uint pid)
                    && FindSessionWithPID(pid, dataFlow, includeHiddenSessions) is AudioSession session
                    && session.HasMatchingName(name, stringComparison))
                { // found a session with a matching process ID and ProcessName
                    return session;
                }
                else return FindSessionWithName(name, dataFlow, stringComparison, includeHiddenSessions);
            }
            else
            { // only 1 segment is present
                s = segments[0];
                if (s.All(char.IsNumber))
                    return FindSessionWithPID(uint.Parse(s), includeHiddenSessions);
                else
                { // ProcessName segment is present:
                    return FindSessionWithName(s, stringComparison, includeHiddenSessions); //< check both Name & ProcessName
                }
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

            for (int i = 0, max = Sessions.Count; i < max; ++i)
            {
                AudioSession session = Sessions[i];
                if (session.SessionIdentifier.Equals(sessionIdentifier, stringComparison)) return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0, max = HiddenSessions.Count; i < max; ++i)
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

            for (int i = 0, max = Sessions.Count; i < max; ++i)
            {
                AudioSession session = Sessions[i];
                if (session.SessionInstanceIdentifier.Equals(sessionInstanceIdentifier, stringComparison)) return session;
            }
            if (includeHiddenSessions)
            {
                for (int i = 0, max = HiddenSessions.Count; i < max; ++i)
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
            if (_sessions.Any(s => s.PID.Equals(session.PID) && s.DataFlow.Equals(session.DataFlow)))
                return; // don't add duplicate sessions

            // allow handlers to edit the session's initial name:
            session.Name = NotifyPreviewSessionName(session).SessionName;
            // allow handlers to edit the session's initial hidden state:
            session.IsHidden = NotifyPreviewSessionIsHidden(session).SessionIsHidden;

            if (session.IsHidden)
            { // add session to hidden list
                NotifyAddingSessionToHiddenList(session);
                _hiddenSessions.Add(session);
                NotifyAddedSessionToHiddenList(session);
            }
            else
            { // add session to visible list
                NotifyAddingSessionToList(session);
                _sessions.Add(session);
                NotifyAddedSessionToList(session);
            }
        }
        /// <summary>
        /// Removes the specified <paramref name="session"/> from the managed sessions.
        /// </summary>
        /// <param name="session">An <see cref="AudioSession"/> instance to remove from the list.</param>
        internal void RemoveSession(AudioSession session)
        {
            // AudioSession.IsHidden is not reliable within this method
            if (HiddenSessions.IndexOf(session, out int hiddenIndex))
            {
                NotifyRemovingSessionFromHiddenList(session);
                _hiddenSessions.RemoveAt(hiddenIndex);
                NotifyRemovedSessionFromHiddenList(session);
            }
            else if (Sessions.IndexOf(session, out int index))
            {
                NotifyRemovingSessionFromList(session);
                _sessions.RemoveAt(index);
                NotifyRemovedSessionFromList(session);
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

        #region Hide/Unhide Session
        /// <summary>
        /// Hides the specified <paramref name="session"/> by moving it from the <see cref="Sessions"/> list to the <see cref="HiddenSessions"/> list.
        /// </summary>
        /// <param name="session">The <see cref="AudioSession"/> instance to hide.</param>
        public void HideSession(AudioSession session)
        {
            if (session.IsHidden) return; //< don't do anything if the session is already hidden

            session.IsHidden = true;

            RemoveSession(session); //< remove from the Sessions list
            AddSession(session); //< add to the HiddenSessions list
        }
        /// <summary>
        /// Unhides the specified <paramref name="session"/> by moving it from the <see cref="HiddenSessions"/> list to the <see cref="Sessions"/> list.
        /// </summary>
        /// <param name="session"></param>
        public void UnhideSession(AudioSession session)
        {
            if (!session.IsHidden) return; //< don't do anything if the session isn't hidden

            session.IsHidden = false;

            RemoveSession(session); //< remove from the HiddenSessions list
            AddSession(session); //< add to the Sessions list
        }
        #endregion Hide/Unhide Session

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

        #endregion EventHandlers

        #region IDisposable Implementation
        /// <summary>
        /// Disposes of all managed audio sessions.
        /// </summary>
        ~AudioSessionManager() => Dispose();
        /// <inheritdoc/>
        public void Dispose()
        {
            Sessions.DisposeAll();
            HiddenSessions.DisposeAll();
            SessionManagers.DisposeAll();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}