using CoreAudio;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.CoreAudio.Interfaces;
using VolumeControl.TypeExtensions;

namespace VolumeControl.CoreAudio
{
    /// <summary>
    /// Manages the currently "selected" <see cref="AudioDevice"/> instance for a given <see cref="CoreAudio.AudioDeviceManager"/> object.
    /// </summary>
    public class AudioDeviceSelector : IAudioSelector, INotifyPropertyChanged
    {
        #region Initializer
        /// <summary>
        /// Creates a new <see cref="AudioDeviceSelector"/> instance bound to the given <paramref name="audioDeviceManager"/>.
        /// </summary>
        /// <param name="audioDeviceManager">An <see cref="CoreAudio.AudioDeviceManager"/> instance to select from.</param>
        public AudioDeviceSelector(AudioDeviceManager audioDeviceManager)
        {
            AudioDeviceManager = audioDeviceManager;

            AudioDeviceManager.DeviceRemovedFromList += this.AudioDeviceManager_DeviceRemovedFromList;

            SelectDefaultDevice();
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
        /// <inheritdoc/>
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
        public void SelectNextDevice()
        {
            if (LockSelection) return;

            if (SelectedIndex + 1 < AudioDeviceManager.Devices.Count)
                ++SelectedIndex;
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
                --SelectedIndex;
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

            Selected = AudioDeviceManager.GetDefaultDevice(Role.Multimedia);
        }
        private void AudioDeviceManager_DeviceRemovedFromList(object? sender, AudioDevice e)
        {
            if (e.Equals(Selected))
                // edit _selected directly to avoid notifications
                _selected = null;
        }
        #endregion Methods
    }
}
