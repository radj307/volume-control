using Audio;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.TypeExtensions;

namespace VolumeControl.SDK
{
    /// <summary>
    /// Manages the currently "selected" <see cref="AudioDevice"/> instance for a given <see cref="Audio.AudioDeviceManager"/> object.
    /// </summary>
    public class AudioDeviceSelector : INotifyPropertyChanged
    {
        #region Initializer
        internal AudioDeviceSelector(AudioDeviceManager audioDeviceManager)
        {
            AudioDeviceManager = audioDeviceManager;

            AudioDeviceManager.DeviceRemovedFromList += this.AudioDeviceManager_DeviceRemovedFromList;
        }
        #endregion Initializer

        #region Properties
        AudioDeviceManager AudioDeviceManager { get; }
        /// <summary>
        /// Gets or the sets the selected item.
        /// </summary>
        /// <remarks>
        /// Cannot be changed if <see cref="LockSelection"/> == <see langword="true"/>.
        /// </remarks>
        public AudioDevice? Selected
        {
            get => _selected;
            set
            {
                if (LockSelection) return;

                _selected = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(SelectedIndex));
            }
        }
        private AudioDevice? _selected;
        /// <summary>
        /// Gets or sets the currently <see cref="Selected"/> item by its index in the list.
        /// </summary>
        /// <remarks>
        /// Cannot be changed if <see cref="LockSelection"/> == <see langword="true"/>.
        /// </remarks>
        public int SelectedIndex
        {
            get => AudioDeviceManager.Devices.IndexOf(Selected);
            set
            {
                if (LockSelection) return;

                if (value == -1)
                    Selected = null;
                else if (value < 0 || value >= AudioDeviceManager.Devices.Count)
                    throw new ArgumentOutOfRangeException(nameof(value));
                else
                    Selected = AudioDeviceManager.Devices[value];
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
        public void SelectNextDevice()
        {
            if (LockSelection) return;

            if (SelectedIndex + 1 < AudioDeviceManager.Devices.Count)
            {
                ++SelectedIndex;
            }
            else
            {
                SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Changes the <see cref="Selected"/> item to the previous session in the list, looping back if necessary.
        /// </summary>
        /// <remarks>
        /// Does nothing if <see cref="LockSelection"/> == <see langword="true"/>.
        /// </remarks>
        public void SelectPreviousDevice()
        {
            if (LockSelection) return;

            if (SelectedIndex > 0)
            {
                --SelectedIndex;
            }
            else
            {
                SelectedIndex = AudioDeviceManager.Devices.Count - 1;
            }
        }
        /// <summary>
        /// Changes the <see cref="Selected"/> item to <see langword="null"/>.
        /// </summary>
        /// <remarks>
        /// Does nothing if <see cref="LockSelection"/> == <see langword="true"/>.
        /// </remarks>
        public void DeselectDevice()
        {
            if (LockSelection) return;

            Selected = null;
        }
        /// <summary>
        /// Changes the <see cref="Selected"/> item to the default Multimedia audio device.
        /// </summary>
        /// <remarks>
        /// Does nothing if <see cref="LockSelection"/> == <see langword="true"/>.
        /// </remarks>
        public void SelectDefaultDevice()
        {
            if (LockSelection) return;

            Selected = AudioDeviceManager.GetDefaultDevice(CoreAudio.Role.Multimedia);
        }
        private void AudioDeviceManager_DeviceRemovedFromList(object? sender, AudioDevice e)
        {
            if (e.Equals(Selected))
            {
                // edit _selected directly to avoid notifications
                _selected = null;
                // don't notify manually because we don't want to cause the targetbox to be changed; just remove the now-deleted device.
            }
        }
        #endregion Methods
    }
}
