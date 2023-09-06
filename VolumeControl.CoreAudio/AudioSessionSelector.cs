using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core;
using VolumeControl.CoreAudio.Interfaces;
using VolumeControl.TypeExtensions;

namespace VolumeControl.CoreAudio
{
    /// <summary>
    /// Manages the currently "selected" <see cref="AudioSession"/> instance for a given <see cref="CoreAudio.AudioSessionManager"/> object.
    /// </summary>
    public class AudioSessionSelector : IAudioSelector, INotifyPropertyChanged
    {
        #region Initializer
        /// <summary>
        /// Creates a new <see cref="AudioSessionSelector"/> instance bound to the given <paramref name="audioSessionManager"/>.
        /// </summary>
        /// <param name="audioSessionManager">An <see cref="CoreAudio.AudioSessionManager"/> instance to select from.</param>
        public AudioSessionSelector(AudioSessionManager audioSessionManager)
        {
            AudioSessionManager = audioSessionManager;

            // attach event to remove previously-selected session.
            AudioSessionManager.SessionRemovedFromList += this.AudioSessionManager_SessionRemovedFromList;

            // attach event to update the selected item when the target changes from another source
            Settings.PropertyChanged += this.Settings_PropertyChanged;
        }
        #endregion Initializer

        #region Properties
        private static Config Settings => (Config.Default as Config)!;
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

                if (_selected == null)
                {// we have to do this here to avoid calls to the getter from data bindings preventing us from setting this to null
                    Settings.Target = VolumeControl.Core.Helpers.TargetInfo.Empty;
                }

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(SelectedIndex));
            }
        }
        private AudioSession? _selected;
        /// <inheritdoc/>
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
            {
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
        /// This works even when <see cref="LockSelection"/> == <see langword="true"/>.<br/>
        /// </remarks>
        private void ResolveSelected()
        {
            var targetInfo = Settings.Target;

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
        #endregion Methods

        #region EventHandlers
        /// <summary>
        /// Deselect session when it was removed from the list
        /// </summary>
        private void AudioSessionManager_SessionRemovedFromList(object? sender, AudioSession e)
        {
            if (e.Equals(Selected))
                // edit _selected directly to avoid notifications
                _selected = null;
        }
        /// <summary>
        /// Update the <see cref="Selected"/> item when the <see cref="Config.Target"/> changes from an external source.
        /// </summary>
        private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName?.Equals(nameof(Config.Target)) ?? false)
            {
                Selected = AudioSessionManager.FindSessionWithSessionInstanceIdentifier(Settings.Target.SessionInstanceIdentifier);
            }
        }
        #endregion EventHandlers
    }
}
