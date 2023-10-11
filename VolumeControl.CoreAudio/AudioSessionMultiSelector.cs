using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.CoreAudio.Events;
using VolumeControl.CoreAudio.Interfaces;
using VolumeControl.TypeExtensions;

namespace VolumeControl.CoreAudio
{
    /// <summary>
    /// Manages multiple "selected" audio sessions for a given <see cref="CoreAudio.AudioSessionManager"/> instance.
    /// </summary>
    public class AudioSessionMultiSelector : IAudioMultiSelector, INotifyPropertyChanged
    {
        #region Initializer
        /// <summary>
        /// Creates a new <see cref="AudioSessionMultiSelector"/> instance for the specified <paramref name="audioSessionManager"/>.
        /// </summary>
        /// <param name="audioSessionManager">An <see cref="CoreAudio.AudioSessionManager"/> instance to select from.</param>
        public AudioSessionMultiSelector(AudioSessionManager audioSessionManager)
        {
            AudioSessionManager = audioSessionManager;

            CurrentIndex = -1;
            _selectionStates = new();
            foreach (var session in Sessions) // populate the selection states list
            {
                _selectionStates.Add(NotifyPreviewSessionIsSelected(session, defaultIsSelected: false).IsSelected);
            }

            AudioSessionManager.AddedSessionToList += this.AudioSessionManager_SessionAddedToList;
            AudioSessionManager.RemovingSessionFromList += this.AudioSessionManager_RemovingSessionFromList; ;
        }
        #endregion Initializer

        #region Properties
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

                var previouslySelectedItems = SelectedItems;

                _selectionStates = (List<bool>)value;
                NotifyPropertyChanged();

                var selectedItems = SelectedItems;

                // trigger selection changed notifications
                for (int i = 0, max = SelectionStates.Count; i < max; ++i)
                {
                    var session = Sessions[i];
                    var wasSelectedBefore = previouslySelectedItems.Contains(session);
                    var isSelectedNow = selectedItems.Contains(session);
                    if (wasSelectedBefore && !isSelectedNow)
                        NotifySessionDeselected(session);
                    else if (!wasSelectedBefore && isSelectedNow)
                    {
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
        public AudioSession[] SelectedItems
        {
            get
            {
                List<AudioSession> l = new();
                for (int i = 0; i < SelectionStates.Count; ++i)
                {
                    if (SelectionStates[i])
                        l.Add(Sessions[i]);
                }
                return l.ToArray();
            }
            set
            {
                for (int i = 0; i < SelectionStates.Count; ++i)
                {
                    _selectionStates[i] = value.Contains(Sessions[i]);
                }
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (LockCurrentIndex) return;

                _currentIndex = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CurrentItem));
                if (_currentIndex != -1)
                    NotifyCurrentItemChanged(Sessions[_currentIndex]);
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
        /// Gets or sets the item that the selector is currently at.
        /// </summary>
        public AudioSession? CurrentItem
        {
            get => CurrentIndex == -1 ? null : Sessions[CurrentIndex];
            set
            {
                if (LockCurrentIndex) return;

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
                NotifyCurrentItemChanged(value);
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
            }
        }
        private bool _lockSelection;
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
        public event EventHandler<AudioSession?>? CurrentItemChanged;
        private void NotifyCurrentItemChanged(AudioSession? audioSession) => CurrentItemChanged?.Invoke(this, audioSession);
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
                NotifySessionSelected(audioSession);
            else
            {
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

            if (_selectionStates[index] = isSelected)
                NotifySessionSelected(Sessions[index]);
            else
            {
                NotifySessionDeselected(Sessions[index]);
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
            NotifySessionSelected(CurrentItem!);
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

            _selectionStates[CurrentIndex] = false;
            NotifySessionDeselected(CurrentItem!);
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
                NotifySessionSelected(CurrentItem!);
            else
            {
                NotifySessionDeselected(CurrentItem!);
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

            if (CurrentIndex + 1 < SelectionStates.Count)
                ++CurrentIndex;
            else
            { // loopback:
                CurrentIndex = 0;
            }
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

            if (CurrentIndex > 0)
                --CurrentIndex;
            else
            { // loopback:
                CurrentIndex = SelectionStates.Count;
            }
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
        {
            var isSelected = NotifyPreviewSessionIsSelected(e, defaultIsSelected: false).IsSelected;
            _selectionStates.Insert(Sessions.IndexOf(e), isSelected);
            if (isSelected)
                NotifySessionSelected(e);
        }
        private void AudioSessionManager_RemovingSessionFromList(object? sender, AudioSession e)
        {
            var index = Sessions.IndexOf(e); //< we can get the index here because the session hasn't been removed yet
            var wasSelected = _selectionStates[index];
            _selectionStates.RemoveAt(index);
            if (wasSelected)
                NotifySessionDeselected(e);
        }
        #endregion AudioSessionManager

        #endregion EventHandlers
    }
}
