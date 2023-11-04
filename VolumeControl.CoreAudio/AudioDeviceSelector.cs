using CoreAudio;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core;
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

            Settings.PropertyChanged += this.Settings_PropertyChanged;

            SelectDefaultDevice();
        }
        #endregion Initializer

        #region Fields
        private bool _updatingSelectedFromSettingsPropertyChanged = false;
        #endregion Fields

        #region Properties
        private static Config Settings => Config.Default!;
        AudioDeviceManager AudioDeviceManager { get; }
        /// <summary>
        /// Gets or the sets the selected item.
        /// </summary>
        /// <remarks>
        /// Cannot be changed if <see cref="LockSelection"/> == <see langword="true"/>.
        /// </remarks>
        public AudioDevice? Selected
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
                {
                    if (!_updatingSelectedFromSettingsPropertyChanged)
                        Settings.TargetDeviceID = string.Empty;
                }
                else
                {
                    Settings.TargetDeviceID = _selected.ID;
                }

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
        /// <summary>
        /// Changes the <see cref="Selected"/> item to whatever is specified by <see cref="Config.TargetDeviceID"/>
        /// </summary>
        private void ResolveSelected()
        {
            var targetDeviceID = Settings.TargetDeviceID;

            AudioDevice? target = AudioDeviceManager.FindDeviceByID(targetDeviceID, StringComparison.Ordinal);

            if (target != null && _selected != target)
            {
                _selected = target;
                NotifyPropertyChanged(nameof(Selected));
                NotifyPropertyChanged(nameof(SelectedIndex));
            }
        }
        #endregion Methods

        #region EventHandlers

        #region AudioDeviceManager
        private void AudioDeviceManager_DeviceRemovedFromList(object? sender, AudioDevice e)
        {
            if (e.Equals(Selected))
                // edit _selected directly to avoid notifications
                _selected = null;
        }
        #endregion AudioDeviceManager

        #region Settings
        /// <summary>
        /// Update the <see cref="Selected"/> item when the <see cref="Config.TargetDeviceID"/> changes from an external source.
        /// </summary>
        private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != null && e.PropertyName.Equals(nameof(Config.TargetDeviceID)))
            {
                _updatingSelectedFromSettingsPropertyChanged = true;
                Selected = AudioDeviceManager.FindDeviceByID(Settings.TargetDeviceID, StringComparison.Ordinal);
                _updatingSelectedFromSettingsPropertyChanged = false;
            }
        }
        #endregion Settings

        #endregion EventHandlers
    }
}
