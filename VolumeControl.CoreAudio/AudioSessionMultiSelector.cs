using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.CoreAudio.Events;
using VolumeControl.CoreAudio.Interfaces;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;
using VolumeControl.Core;
using CoreAudio;

namespace VolumeControl.CoreAudio
{
    /// <summary>
    /// Manages multiple "selected" audio sessions for a given <see cref="CoreAudio.AudioSessionManager"/> instance.
    /// </summary>
    public class AudioSessionMultiSelector : IAudioMultiSelector, INotifyPropertyChanged
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="AudioSessionMultiSelector"/> instance for the specified <paramref name="audioSessionManager"/>.
        /// </summary>
        /// <param name="audioSessionManager">An <see cref="CoreAudio.AudioSessionManager"/> instance to select from.</param>
        public AudioSessionMultiSelector(AudioSessionManager audioSessionManager)
        {
            AudioSessionManager = audioSessionManager;

            CurrentIndex = -1;
            _selectedSessions = new();
            _selectionStates = new();
            // populate the lists:
            Sessions.ForEach(AddSession);

            AudioSessionManager.AddedSessionToList += this.AudioSessionManager_SessionAddedToList;
            AudioSessionManager.RemovingSessionFromList += this.AudioSessionManager_RemovingSessionFromList;
            AudioSessionManager.AddedSessionToHiddenList += this.AudioSessionManager_SessionAddedToList;
            AudioSessionManager.RemovingSessionFromHiddenList += this.AudioSessionManager_RemovingSessionFromList;
        }
        #endregion Constructor

        #region Properties
        private static Config Settings => Config.Default;
        AudioSessionManager AudioSessionManager { get; }
        private IReadOnlyList<AudioSession> Sessions => AudioSessionManager.Sessions;
        /// <inheritdoc/>
        public IReadOnlyList<bool> SelectionStates
        {
            get => _selectionStates;
            set
            {
                if (value.Count != Sessions.Count) // enforce size limits
                    throw new ArgumentOutOfRangeException(nameof(SelectionStates), $"The length of the {nameof(SelectionStates)} array ({value.Count}) must be equal to the length of the {nameof(Sessions)} list ({Sessions.Count})!");

                if (LockSelection) return;

                var previouslySelectedItems = SelectedSessions;

                _selectionStates = value.ToList();
                NotifyPropertyChanged();

                var selectedItems = SelectedSessions;

                // trigger selection changed notifications
                for (int i = 0, max = SelectionStates.Count; i < max; ++i)
                {
                    var session = Sessions[i];
                    var wasSelectedBefore = previouslySelectedItems.Contains(session);
                    var isSelectedNow = selectedItems.Contains(session);
                    if (wasSelectedBefore && !isSelectedNow)
                    {
                        AddSelectedSession(session);
                        NotifySessionDeselected(session);
                    }
                    else if (!wasSelectedBefore && isSelectedNow)
                    {
                        RemoveSelectedSession(session);
                        NotifySessionSelected(session);
                    }
                }
            }
        }
        private List<bool> _selectionStates;
        /// <summary>
        /// Gets or sets the list of selected AudioSession instances.
        /// </summary>
        /// <returns>An array of all selected AudioSessions in the order that they appear in the Sessions list.</returns>
        public IReadOnlyList<AudioSession> SelectedSessions
        {
            get => _selectedSessions;
            set
            {
                for (int i = 0, max = SelectionStates.Count; i < max; ++i)
                {
                    var session = Sessions[i];
                    var newState = value.Contains(session);

                    if (_selectionStates[i] == newState) continue; //< no changes to make

                    if (_selectionStates[i] = newState)
                    {
                        AddSelectedSession(session);
                        NotifySessionSelected(session);
                    }
                    else
                    {
                        RemoveSelectedSession(session);
                        NotifySessionDeselected(session);
                    }
                }
                NotifyPropertyChanged();
            }
        }
        private readonly List<AudioSession> _selectedSessions;
        /// <summary>
        /// Gets whether there are any selected sessions or not.
        /// </summary>
        /// <returns><see langword="true"/> when there is at least one selected session; otherwise <see langword="false"/>.</returns>
        public bool HasSelectedSessions => SelectedSessions.Count > 0;
        /// <inheritdoc/>
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (LockCurrentIndex || value == _currentIndex) return;

                _currentIndex = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CurrentSession));
                NotifyCurrentSessionChanged(_currentIndex != -1 ? Sessions[_currentIndex] : null);
            }
        }
        private int _currentIndex;
        /// <inheritdoc/>
        public bool LockCurrentIndex
        {
            get => _lockCurrentIndex;
            set
            {
                _lockCurrentIndex = value;
                NotifyPropertyChanged();
            }
        }
        private bool _lockCurrentIndex;
        /// <summary>
        /// Gets or sets the session that the selector is currently pointing at.
        /// </summary>
        public AudioSession? CurrentSession
        {
            get => CurrentIndex == -1 ? null : Sessions[CurrentIndex];
            set
            {
                if (LockCurrentIndex || value == CurrentSession) return;

                if (value == null)
                {
                    _currentIndex = -1;
                    NotifyPropertyChanged(nameof(CurrentIndex));
                }
                else // value is a valid session
                {
                    _currentIndex = Sessions.IndexOf(value);
                    NotifyPropertyChanged(nameof(CurrentIndex));
                }
                NotifyPropertyChanged();
                NotifyCurrentSessionChanged(value);
            }
        }
        /// <inheritdoc/>
        public bool LockSelection
        {
            get => _lockSelection;
            set
            {
                _lockSelection = value;
                NotifyPropertyChanged();
                if (LockCurrentIndexOnLockSelection)
                    LockCurrentIndex = value;
            }
        }
        private bool _lockSelection;
        /// <summary>
        /// Gets or sets whether the LockCurrentIndex property is also set when LockSelection is changed.
        /// </summary>
        public bool LockCurrentIndexOnLockSelection
        {
            get => _lockCurrentIndexOnLockSelection;
            set
            {
                _lockCurrentIndexOnLockSelection = value;
                NotifyPropertyChanged();
            }
        }
        private bool _lockCurrentIndexOnLockSelection;
        #endregion Properties

        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        /// <summary>
        /// Occurs when a session is selected for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? SessionSelected;
        private void NotifySessionSelected(AudioSession audioSession) => SessionSelected?.Invoke(this, audioSession);
        /// <summary>
        /// Occurs when a session is deselected for any reason.
        /// </summary>
        public event EventHandler<AudioSession>? SessionDeselected;
        private void NotifySessionDeselected(AudioSession audioSession) => SessionDeselected?.Invoke(this, audioSession);
        /// <summary>
        /// Occurs when the CurrentItem is changed for any reason.
        /// </summary>
        public event EventHandler<AudioSession?>? CurrentSessionChanged;
        private void NotifyCurrentSessionChanged(AudioSession? audioSession) => CurrentSessionChanged?.Invoke(this, audioSession);
        /// <summary>
        /// Occurs when the ActiveSession is changed for any reason.
        /// </summary>
        public event EventHandler<AudioSession?>? ActiveSessionChanged;
        /// <summary>
        /// Occurs when the ActiveSession is changed for any reason.
        /// </summary>
        public void NotifyActiveSessionChanged(AudioSession? audioSession) => ActiveSessionChanged?.Invoke(this, audioSession);
        /// <summary>
        /// Occurs prior to a new session being added, allowing handlers to determine if it should be selected by default.
        /// </summary>
        public event PreviewSessionIsSelectedEventHandler? PreviewSessionIsSelected;
        private PreviewSessionIsSelectedEventArgs NotifyPreviewSessionIsSelected(AudioSession audioSession, bool defaultIsSelected)
        {
            var args = new PreviewSessionIsSelectedEventArgs(audioSession, defaultIsSelected);
            PreviewSessionIsSelected?.Invoke(this, args);
            return args;
        }
        #endregion Events

        #region Methods

        #region Add/Remove SelectedSession
        private void AddSelectedSession(AudioSession audioSession)
        {
            if (SelectedSessions.Contains(audioSession))
            {
                FLog.Warning($"[{nameof(AudioSessionMultiSelector)}] Session \"{audioSession.Name}\" is already selected, but {nameof(AddSelectedSession)} was called again!");
                return;
            }

            var hadSelectedSessions = HasSelectedSessions;
            _selectedSessions.Add(audioSession);
            if (!hadSelectedSessions) // there are selected sessions now
                NotifyPropertyChanged(nameof(HasSelectedSessions));

            if (FLog.FilterEventType(EventType.TRACE))
                FLog.Trace($"[{nameof(AudioSessionMultiSelector)}] Session \"{audioSession.Name}\" was selected.");
        }
        private bool RemoveSelectedSession(AudioSession audioSession)
        {
            if (_selectedSessions.Remove(audioSession))
            {
                if (_selectedSessions.Count == 0) // there are not selected sessions anymore
                    NotifyPropertyChanged(nameof(HasSelectedSessions));

                if (FLog.FilterEventType(EventType.TRACE))
                    FLog.Trace($"[{nameof(AudioSessionMultiSelector)}] Session \"{audioSession.Name}\" was deselected.");
                return true;
            }
            return false;
        }
        #endregion Add/Remove SelectedSession

        #region Add/Remove Session
        private void AddSession(AudioSession session)
        {
            var isSelected = NotifyPreviewSessionIsSelected(session, defaultIsSelected: false).IsSelected;
            _selectionStates.Insert(Sessions.IndexOf(session), isSelected);
            if (isSelected)
            {
                AddSelectedSession(session);
                NotifySessionSelected(session);
            }
        }
        private void RemoveSession(AudioSession session)
        {
            var index = Sessions.IndexOf(session); //< we can get the index here because the session hasn't been removed yet
            if (index == -1)
                return;
            if (index == _currentIndex)
            {
                CurrentIndex = -1;
            }
            var wasSelected = _selectionStates[index];
            _selectionStates.RemoveAt(index);
            if (wasSelected)
            {
                RemoveSelectedSession(session);
                NotifySessionDeselected(session);
            }
        }
        #endregion Add/Remove Session

        #region Get/Set SessionIsSelected
        /// <summary>
        /// Gets whether the specified <paramref name="audioSession"/> is selected or not.
        /// </summary>
        /// <param name="audioSession">An <see cref="AudioSession"/> instance.</param>
        /// <returns><see langword="true"/> when the <paramref name="audioSession"/> is selected; otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentException">The specified <paramref name="audioSession"/> does not exist in the <see cref="AudioSessionManager"/>.</exception>
        public bool GetSessionIsSelected(AudioSession audioSession)
        {
            var index = Sessions.IndexOf(audioSession);
            if (index == -1)
                throw new ArgumentException($"The specified {nameof(AudioSession)} instance was not found in the {nameof(AudioSessionManager)}'s {nameof(Sessions)} list!", nameof(audioSession));

            return SelectionStates[index];
        }
        /// <summary>
        /// Sets whether the specified <paramref name="audioSession"/> is selected or not.
        /// </summary>
        /// <param name="audioSession">An <see cref="AudioSession"/> instance.</param>
        /// <param name="isSelected"><see langword="true"/> selects the <paramref name="audioSession"/>; <see langword="false"/> deselects the <paramref name="audioSession"/>.</param>
        /// <exception cref="ArgumentException">The specified <paramref name="audioSession"/> does not exist in the <see cref="AudioSessionManager"/>.</exception>
        public void SetSessionIsSelected(AudioSession audioSession, bool isSelected)
        {
            var index = Sessions.IndexOf(audioSession);
            if (index == -1)
                throw new ArgumentException($"The specified {nameof(AudioSession)} instance was not found in the {nameof(AudioSessionManager)}'s {nameof(Sessions)} list!", nameof(audioSession));

            if (_selectionStates[index] == isSelected) return; //< don't do anything if nothing is changing

            if (_selectionStates[index] = isSelected)
            {
                AddSelectedSession(audioSession);
                NotifySessionSelected(audioSession);
            }
            else
            {
                RemoveSelectedSession(audioSession);
                NotifySessionDeselected(audioSession);
            }
        }
        /// <summary>
        /// Sets whether the <see cref="AudioSession"/> at the specified <paramref name="index"/> is selected or not.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="AudioSession"/> instance to (de)select.</param>
        /// <param name="isSelected"><see langword="true"/> selects the session; <see langword="false"/> deselects the session.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="index"/> is out-of-range of the Sessions list.</exception>
        public void SetSessionIsSelected(int index, bool isSelected)
        {
            if (index < 0 || index >= Sessions.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Specified index ({index}) is out of range of the {nameof(Sessions)} list (0-{Sessions.Count - 1})!");

            if (_selectionStates[index] == isSelected) return; //< don't do anything if nothing is changing

            var session = Sessions[index];
            if (_selectionStates[index] = isSelected)
            {
                AddSelectedSession(session);
                NotifySessionSelected(session);
            }
            else
            {
                RemoveSelectedSession(session);
                NotifySessionDeselected(session);
            }
        }
        #endregion Get/Set SessionIsSelected

        #region SetAllSessionSelectionStates
        /// <summary>
        /// Sets whether all <see cref="AudioSession"/> instances in the Sessions list are selected or not.
        /// </summary>
        /// <param name="isSelected"><see langword="true"/> selects the sessions; <see langword="false"/> deselects the sessions.</param>
        public void SetAllSessionSelectionStates(bool isSelected)
        {
            for (int i = 0, max = SelectionStates.Count; i < max; ++i)
            {
                SetSessionIsSelected(i, isSelected);
            }
        }
        #endregion SetAllSessionSelectionStates

        #region SetSelectedSessions
        /// <summary>
        /// Sets only the specified <paramref name="sessions"/> as selected.
        /// </summary>
        /// <param name="sessions">Any number of audio sessions.</param>
        public void SetSelectedSessions(params AudioSession[] sessions)
        {
            for (int i = 0, max = SelectionStates.Count; i < max; ++i)
            {
                SetSessionIsSelected(i, sessions.Contains(Sessions[i]));
            }
        }
        /// <inheritdoc cref="SetSelectedSessions(AudioSession[])"/>
        public void SetSelectedSessions(IEnumerable<AudioSession> sessions)
            => SetSelectedSessions(sessions.ToArray());
        #endregion SetSelectedSessions

        #region SetSelectedSessionsOrCurrentSession
        /// <summary>
        /// Sets only the specified <paramref name="sessions"/> as selected, unless <paramref name="sessions"/> contains exactly 1 session in which case the CurrentSession is set and all sessions are deselected.
        /// </summary>
        /// <param name="sessions">Any number of audio sessions.</param>
        public void SetSelectedSessionsOrCurrentSession(params AudioSession[] sessions)
        {
            if (sessions.Length == 1)
            { // set the current session
                SetAllSessionSelectionStates(false);
                CurrentSession = sessions[0];
            }
            else SetSelectedSessions(sessions);
        }
        /// <inheritdoc cref="SetSelectedSessionsOrCurrentSession(AudioSession[])"/>
        public void SetSelectedSessionsOrCurrentSession(IEnumerable<AudioSession> sessions)
            => SetSelectedSessionsOrCurrentSession(sessions.ToArray());
        #endregion SetSelectedSessionsOrCurrentSession

        #region ClearSelectedSessions
        /// <summary>
        /// Unselects all SelectedSessions.
        /// </summary>
        public void ClearSelectedSessions() => SetAllSessionSelectionStates(false);
        #endregion ClearSelectedSessions

        #region Select/Deselect/ToggleSelect CurrentItem
        /// <summary>
        /// Selects the CurrentItem.
        /// </summary>
        /// <remarks>
        /// Does nothing when <see cref="LockSelection"/> is <see langword="true"/> or <see cref="CurrentIndex"/> is -1.
        /// </remarks>
        public void SelectCurrentItem()
        {
            if (LockSelection || CurrentIndex == -1) return;

            _selectionStates[CurrentIndex] = true;
            AddSelectedSession(CurrentSession!);
            NotifySessionSelected(CurrentSession!);
        }
        /// <summary>
        /// Deselects the CurrentItem.
        /// </summary>
        /// <remarks>
        /// Does nothing when <see cref="LockSelection"/> is <see langword="true"/> or <see cref="CurrentIndex"/> is -1.
        /// </remarks>
        public void DeselectCurrentItem()
        {
            if (LockSelection || CurrentIndex == -1) return;

            CurrentIndex = -1;
            if (RemoveSelectedSession(CurrentSession!))
            {
                NotifySessionDeselected(CurrentSession!);
            }
        }
        /// <summary>
        /// Toggles whether the CurrentItem is selected.
        /// </summary>
        /// <remarks>
        /// Does nothing when <see cref="LockSelection"/> is <see langword="true"/> or <see cref="CurrentIndex"/> is -1.
        /// </remarks>
        public void ToggleSelectCurrentItem()
        {
            if (LockSelection || CurrentIndex == -1) return;

            if (_selectionStates[CurrentIndex] = !SelectionStates[CurrentIndex])
            {
                AddSelectedSession(CurrentSession!);
                NotifySessionSelected(CurrentSession!);
            }
            else
            {
                RemoveSelectedSession(CurrentSession!);
                NotifySessionDeselected(CurrentSession!);
            }
        }
        #endregion Select/Deselect/ToggleSelect CurrentItem

        #region Increment/Decrement/Unset CurrentIndex
        /// <summary>
        /// Increments the CurrentIndex by 1, looping back around to 0 when it exceeds the length of the Sessions list.
        /// </summary>
        /// <remarks>
        /// Does nothing when <see cref="LockCurrentIndex"/> is <see langword="true"/>.
        /// </remarks>
        public void IncrementCurrentIndex()
        {
            if (LockCurrentIndex) return;

            int oldIndex = CurrentIndex;
            if (oldIndex == -1)
                oldIndex = 0;
            do
            {
                CurrentIndex = (CurrentIndex + 1) % SelectionStates.Count;
            }
            while (Sessions[CurrentIndex].IsHidden || (Settings.HideInactiveSessions && Sessions[CurrentIndex].State != AudioSessionState.AudioSessionStateActive && CurrentIndex != oldIndex));
        }
        /// <summary>
        /// Decrements the CurrentIndex by 1, looping back around to the length of the Sessions list when it goes past 0.
        /// </summary>
        /// <remarks>
        /// Does nothing when <see cref="LockCurrentIndex"/> is <see langword="true"/>.
        /// </remarks>
        public void DecrementCurrentIndex()
        {
            if (LockCurrentIndex) return;

            int oldIndex = CurrentIndex;
            if (oldIndex == -1)
                oldIndex = 0;
            do
            {
                CurrentIndex = (SelectionStates.Count + CurrentIndex - 1) % SelectionStates.Count;
            }
            while (Sessions[CurrentIndex].IsHidden || (Settings.HideInactiveSessions && Sessions[CurrentIndex].State != AudioSessionState.AudioSessionStateActive && CurrentIndex != oldIndex));
        }
        /// <summary>
        /// Sets the CurrentIndex to 0.
        /// </summary>
        /// <remarks>
        /// Does nothing when <see cref="LockCurrentIndex"/> is <see langword="true"/>.
        /// </remarks>
        public void UnsetCurrentIndex()
        {
            if (LockCurrentIndex) return;

            CurrentIndex = -1;
        }
        #endregion Increment/Decrement/Unset CurrentIndex

        #endregion Methods

        #region EventHandlers

        #region AudioSessionManager
        private void AudioSessionManager_SessionAddedToList(object? sender, AudioSession e)
            => AddSession(e);
        private void AudioSessionManager_RemovingSessionFromList(object? sender, AudioSession e)
            => RemoveSession(e);
        #endregion AudioSessionManager

        #endregion EventHandlers
    }
}
