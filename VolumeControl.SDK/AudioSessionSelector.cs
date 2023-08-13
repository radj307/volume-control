using Audio;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.TypeExtensions;

namespace VolumeControl.SDK
{
    /// <summary>
    /// Manages the currently "selected" <see cref="AudioSession"/> instance for a given <see cref="Audio.AudioSessionManager"/> object.
    /// </summary>
    public class AudioSessionSelector : INotifyPropertyChanged
    {
        internal AudioSessionSelector(AudioSessionManager audioSessionManager)
        {
            AudioSessionManager = audioSessionManager;

            // attach event to remove previously-selected session.
            AudioSessionManager.SessionRemovedFromList += this.AudioSessionManager_SessionRemovedFromList;
        }

        #region Properties
        AudioSessionManager AudioSessionManager { get; }
        /// <summary>
        /// Gets or the sets the selected item.
        /// </summary>
        /// <remarks>
        /// Cannot be changed if <see cref="LockSelection"/> == <see langword="true"/>.
        /// </remarks>
        public AudioSession? Selected
        {
            get
            {
                if (_selected is null)
                    ResolveSelected();
                return _selected;
            }
            set
            {
                if (LockSelection) return;

                _selected = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(SelectedIndex));
            }
        }
        private AudioSession? _selected;
        /// <summary>
        /// Gets or sets the currently <see cref="Selected"/> item by its index in the list.
        /// </summary>
        /// <remarks>
        /// Cannot be changed if <see cref="LockSelection"/> == <see langword="true"/>.
        /// </remarks>
        public int SelectedIndex
        {
            get => AudioSessionManager.Sessions.IndexOf(Selected);
            set
            {
                if (LockSelection) return;

                if (value == -1)
                    Selected = null;
                else if (value < 0 || value >= AudioSessionManager.Sessions.Count)
                    throw new ArgumentOutOfRangeException(nameof(value));
                else
                    Selected = AudioSessionManager.Sessions[value];
                // no notification necessary since Selected handles that for us
            }
        }
        /// <summary>
        /// Gets or sets whether the <see cref="Selected"/> item can be changed or not.
        /// </summary>
        /// <returns><see langword="true"/> when the selection cannot be changed; otherwise <see langword="false"/>.</returns>
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
        #endregion Events

        #region Methods
        /// <summary>
        /// Changes the <see cref="Selected"/> item to the next session in the list, looping back if necessary.
        /// </summary>
        /// <remarks>
        /// Does nothing if <see cref="LockSelection"/> == <see langword="true"/>.
        /// </remarks>
        public void SelectNextSession()
        {
            if (LockSelection) return;

            if (SelectedIndex + 1 < AudioSessionManager.Sessions.Count)
                ++SelectedIndex;
            else
            { // loopback:
                SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Changes the <see cref="Selected"/> item to the previous session in the list, looping back if necessary.
        /// </summary>
        /// <remarks>
        /// Does nothing if <see cref="LockSelection"/> == <see langword="true"/>.
        /// </remarks>
        public void SelectPreviousSession()
        {
            if (LockSelection) return;

            if (SelectedIndex > 0)
                --SelectedIndex;
            else
            { // loopback:
                SelectedIndex = AudioSessionManager.Sessions.Count - 1;
            }
        }
        /// <summary>
        /// Changes the <see cref="Selected"/> item to <see langword="null"/>.
        /// </summary>
        /// <remarks>
        /// Does nothing if <see cref="LockSelection"/> == <see langword="true"/>.
        /// </remarks>
        public void DeselectSession()
        {
            if (LockSelection) return;

            Selected = null;
        }
        /// <summary>
        /// Changes the <see cref="Selected"/> item to whatever is specified by <see cref="Core.Config.Target"/>.
        /// </summary>
        /// <remarks>
        /// This works even when <see cref="LockSelection"/> == <see langword="true"/>.
        /// </remarks>
        public void ResolveSelected()
        {
            var targetInfo = VCAPI.Default.Settings.Target;

            AudioSession? target
                = AudioSessionManager.FindSessionWithSessionInstanceIdentifier(targetInfo.SessionInstanceIdentifier)
                ?? AudioSessionManager.FindSessionWithProcessIdentifier(targetInfo.ProcessIdentifier);

            // only update if the resolved target isn't null AND is different
            if (target != null && _selected != target)
            {
                _selected = target;
                NotifyPropertyChanged(nameof(Selected));
                NotifyPropertyChanged(nameof(SelectedIndex));
            }
        }
        private void AudioSessionManager_SessionRemovedFromList(object? sender, AudioSession e)
        {
            if (e.Equals(Selected))
            {
                _selected = null;
                // don't notify because we don't need to delete the current targetbox text; just remove the now-deleted instance.
            }
        }
        #endregion Methods
    }
}
