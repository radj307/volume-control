using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using VolumeControl.Audio;
using VolumeControl.Audio.Events;
using VolumeControl.Core;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Interfaces;
using VolumeControl.SDK;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Hotkeys
{
    /// <summary>
    /// Contains hotkey action handlers that interact with AudioDevices in the <see cref="Audio.AudioAPI"/> object.
    /// </summary>
    [HotkeyActionGroup("Device", GroupColor = "#FF9999")]
    public sealed class AudioDeviceActions
    {
        #region Constructor
        static AudioDeviceActions()
        {
            // create a notification display target for audio devices
            ListDisplayTarget ldtDevices = new(DisplayTargetName);
            // Create a function to handle property changed events on ldtDevices so we can update our static properties
            void handleSelectedItemPropertyChanged(object? s, PropertyChangedEventArgs e)
            {
                if (e.PropertyName?.Equals(nameof(ldtDevices.SelectedItem)) ?? false && !ldtDevices.SelectedItem.Equals(SelectedDevice))
                {
                    SelectedDevice = ldtDevices.SelectedItem as AudioDevice;
                }
            }
            // Bind ItemsSource => AudioAPI.Devices
            ldtDevices.SetBinding(ListDisplayTarget.ItemsSourceProperty, new Binding()
            {
                Source = AudioAPI,
                Path = new PropertyPath(nameof(Audio.AudioAPI.Devices))
            });
            // Bind SelectedItemControls => (self).SelectedItem.DisplayControls
            ldtDevices.SetBinding(ListDisplayTarget.SelectedItemControlsProperty, new Binding()
            {
                RelativeSource = RelativeSource.Self,
                Path = new PropertyPath($"{nameof(ListDisplayTarget.SelectedItem)}.{nameof(IListDisplayable.DisplayControls)}"),
            });
            // Show when SelectedDeviceSwitched is triggered
            ConditionalEventForward? cefSelectedDeviceSwitched = ldtDevices.AddConditionalEventForward((s, e) => Settings.NotificationsEnabled);
            SelectedDeviceSwitched += (s, e) =>
            {
                cefSelectedDeviceSwitched.Handler(s, e); //< show
                ldtDevices.PropertyChanged -= handleSelectedItemPropertyChanged;
                ldtDevices.SelectedItem = SelectedDevice; //< update SelectedItem to match new value
                ldtDevices.RaisePropertyChanged(nameof(ldtDevices.SelectedItemControls));
                ldtDevices.PropertyChanged += handleSelectedItemPropertyChanged;
            };
            // Show when DeviceEnabledChanged is triggered
            var cefEnableDeviceChanged = ldtDevices.AddConditionalEventForward((s, e) => Settings.NotificationsEnabled);
            AudioAPI.Devices.DeviceEnabledChanged += cefEnableDeviceChanged.Handler;
            // Show when DeviceVolumeChanged is triggered
            var cefDeviceVolumeChanged = ldtDevices.AddConditionalEventForward((s, e) => Settings.NotificationsEnabled && Settings.NotificationsOnVolumeChange);
            AudioAPI.Devices.DeviceVolumeChanged += cefDeviceVolumeChanged.Handler;
            SelectedDeviceVolumeChanged += ldtDevices.AddConditionalEventForward((s, e) => Settings.NotificationsEnabled && Settings.NotificationsOnVolumeChange).Handler;
            // Show when LockSelectedChanged is triggered
            var cefDeviceSelectionLockedChanged = ldtDevices.AddConditionalEventForward((s, e) => Settings.NotificationsEnabled);
            LockSelectionChanged += (s, e) =>
            {
                cefDeviceSelectionLockedChanged.Handler(s, e); //< show
                ldtDevices.LockSelection = DeviceSelectionLocked;
                ldtDevices.Background = DeviceSelectionLocked
                    ? Config.NotificationLockedBrush
                    : Config.NotificationUnlockedBrush;
                ldtDevices.Show();
            };

            ldtDevices.PropertyChanged += handleSelectedItemPropertyChanged;

            VCAPI.Default.ListDisplayTargets.Add(ldtDevices);

            // TODO: Implement a setting for this:
            SelectedDevice = AudioAPI.DefaultDevice; //< initialize the selected device
        }
        #endregion Constructor

        #region Properties
        private static Config Settings => VCAPI.Default.Settings;
        /// <summary>
        /// The <see cref="Audio.AudioAPI"/> instance to use for volume-related events.
        /// </summary>
        private static AudioAPI AudioAPI => _audioAPI ??= VCAPI.Default.AudioAPI;
        private static AudioAPI? _audioAPI = null;
        public const string DisplayTargetName = "Audio Devices";

        private static AudioDevice? _selectedDevice;
        public static AudioDevice? SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                NotifySelectedDeviceSwitched();
            }
        }
        public static bool DeviceSelectionLocked { get; set; } = false;
        #endregion Properties

        #region Events
        /// <summary>Triggered when the volume or mute state of the <see cref="SelectedDevice"/> is changed.</summary>
        public static event VolumeChangedEventHandler? SelectedDeviceVolumeChanged;
        private static void NotifySelectedDeviceVolumeChanged(VolumeChangedEventArgs e) => SelectedDeviceVolumeChanged?.Invoke(SelectedDevice, e);
        /// <summary>Triggered when the selected session is changed.</summary>
        public static event EventHandler? SelectedDeviceSwitched;
        private static void NotifySelectedDeviceSwitched() => SelectedDeviceSwitched?.Invoke(SelectedDevice, new());
        public static event EventHandler? LockSelectionChanged;
        private static void NotifyLockSelectionChanged() => LockSelectionChanged?.Invoke(LockSelectionChanged, new());
        #endregion Events

        #region DeviceSelection
        public static int GetDeviceVolume() => SelectedDevice?.Volume ?? -1;
        public static void SetDeviceVolume(int volume)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.Volume = volume;
                NotifySelectedDeviceVolumeChanged(new(dev.Volume, dev.Muted));
            }
        }
        public static void IncrementDeviceVolume(int amount)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.Volume += amount;
                NotifySelectedDeviceVolumeChanged(new(dev.Volume, dev.Muted));
            }
        }
        /// <remarks>This calls <see cref="IncrementDeviceVolume(int)"/> using <see cref="VolumeStepSize"/>.</remarks>
        /// <inheritdoc cref="IncrementDeviceVolume(int)"/>
        public static void IncrementDeviceVolume() => IncrementDeviceVolume(AudioAPI.VolumeStepSize);
        /// <summary>
        /// Decrements the endpoint volume of the currently selected device.<br/>
        /// This affects the maximum volume of all sessions using this endpoint.
        /// </summary>
        /// <param name="amount">The amount to decrease the volume by.</param>
        public static void DecrementDeviceVolume(int amount)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.Volume -= amount;
                NotifySelectedDeviceVolumeChanged(new(dev.Volume, dev.Muted));
            }
        }
        /// <remarks>This calls <see cref="DecrementDeviceVolume(int)"/> using <see cref="VolumeStepSize"/>.</remarks>
        /// <inheritdoc cref="DecrementDeviceVolume(int)"/>
        public static void DecrementDeviceVolume() => DecrementDeviceVolume(AudioAPI.VolumeStepSize);
        /// <summary>
        /// Gets whether the <see cref="SelectedDevice"/> is currently muted.
        /// </summary>
        /// <returns>True if <see cref="SelectedDevice"/> is not null and is muted; otherwise false.</returns>
        public static bool GetDeviceMute() => SelectedDevice?.Muted ?? false;
        /// <summary>
        /// Sets the mute state of <see cref="SelectedDevice"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        /// <remarks>Note that this affects all sessions using this device.</remarks>
        /// <param name="state">When true, the device will be muted; when false, the device will be unmuted.</param>
        public static void SetDeviceMute(bool state)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.Muted = state;
                NotifySelectedDeviceVolumeChanged(new(dev.Volume, dev.Muted));
            }
        }
        /// <summary>
        /// Toggles the mute state of <see cref="SelectedDevice"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        /// <remarks>Note that this affects all sessions using this device.</remarks>
        public static void ToggleDeviceMute()
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.Muted = !dev.Muted;
                NotifySelectedDeviceVolumeChanged(new(dev.Volume, dev.Muted));
            }
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the device occurring after this one in <see cref="Devices"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.<br/>
        /// Does nothing unless device selection is unlocked.
        /// </summary>
        /// <remarks>If <see cref="SelectedDevice"/> is set to null, the first element in <see cref="Devices"/> is selected.</remarks>
        public static void SelectNextDevice()
        {
            if (DeviceSelectionLocked) return;
            if (AudioAPI.Devices.Count == 0)
            {
                if (SelectedDevice == null) return;
                SelectedDevice = null;
            }
            else if (SelectedDevice is AudioDevice device)
            {
                int index = AudioAPI.Devices.IndexOf(device);
                if (index == -1 || (index += 1) >= AudioAPI.Devices.Count)
                    index = 0;
                SelectedDevice = AudioAPI.Devices[index];
            }
            else
            {
                SelectedDevice = AudioAPI.Devices[0];
            }
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the device occurring before this one in <see cref="Devices"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.<br/>
        /// Does nothing unless device selection is unlocked.
        /// </summary>
        /// <remarks>If <see cref="SelectedDevice"/> is set to null, the last element in <see cref="Devices"/> is selected.</remarks>
        public static void SelectPreviousDevice()
        {
            if (DeviceSelectionLocked) return;
            if (AudioAPI.Devices.Count == 0)
            {
                if (SelectedDevice == null) return;
                SelectedDevice = null;
            }
            else if (SelectedDevice is AudioDevice device)
            {
                int index = AudioAPI.Devices.IndexOf(device);
                if (index == -1 || (index -= 1) < 0)
                    index = AudioAPI.Devices.Count - 1;
                SelectedDevice = AudioAPI.Devices[index];
            }
            else
            {
                SelectedDevice = AudioAPI.Devices[^1];
            }
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to <see langword="null"/>.<br/>
        /// This does nothing if 
        /// <br/>Does nothing if <see cref="SelectedDevice"/> is already null, or if the selected device is locked.
        /// </summary>
        /// <returns><see langword="true"/> when the selected device was set to null &amp; the <see cref="SelectedDeviceSwitched"/> event was fired; otherwise <see langword="false"/>.</returns>
        public static bool DeselectDevice()
        {
            if (SelectedDevice == null)
                return false;

            SelectedDevice = null;
            return true;
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to <see cref="DefaultDevice"/>.<br/>
        /// Does nothing if the selected device is locked, or if the selected device is already set to the default device.
        /// </summary>
        /// <returns><see langword="true"/> when <see cref="SelectedDevice"/> was changed &amp; the <see cref="SelectedDeviceSwitched"/> event was fired; otherwise <see langword="false"/>.</returns>
        public static bool SelectDefaultDevice()
        {
            if (AudioAPI.DefaultDevice is AudioDevice device && SelectedDevice != device)
            {
                SelectedDevice = device;
                return true;
            }
            return false;
        }
        public static void ToggleLockSelection()
        {
            DeviceSelectionLocked = !DeviceSelectionLocked;
            NotifyLockSelectionChanged();
        }
        #endregion DeviceSelection

        #region Action Methods
        [HotkeyAction(Description = "Increases the device volume of the selected device.")]
        public void VolumeUp(object? sender, HandledEventArgs e)
            => IncrementDeviceVolume();
        [HotkeyAction(Description = "Decreases the device volume of the selected device.")]
        public void VolumeDown(object? sender, HandledEventArgs e)
            => DecrementDeviceVolume();
        [HotkeyAction(Description = "Mutes the selected device.")]
        public void Mute(object? sender, HandledEventArgs e)
            => SetDeviceMute(true);
        [HotkeyAction(Description = "Unmutes the selected device.")]
        public void Unmute(object? sender, HandledEventArgs e)
            => SetDeviceMute(false);
        [HotkeyAction(Description = "Toggles the selected device's mute state.")]
        public void ToggleMute(object? sender, HandledEventArgs e)
            => ToggleDeviceMute();
        [HotkeyAction(Description = "Selects the next device in the list.")]
        public void SelectNext(object? sender, HandledEventArgs e)
            => SelectNextDevice();
        [HotkeyAction(Description = "Selects the previous device in the list.")]
        public void SelectPrevious(object? sender, HandledEventArgs e)
            => SelectPreviousDevice();
        [HotkeyAction(Description = "Deselects the selected device.")]
        public void Deselect(object? sender, HandledEventArgs e)
            => DeselectDevice();
        [HotkeyAction(Description = "Selects the default output device in the list.")]
        public void SelectDefault(object? sender, HandledEventArgs e)
            => SelectDefaultDevice();
        [HotkeyAction(Description = "Toggles whether the selected device can be changed or not.")]
        public void ToggleLock(object? sender, HandledEventArgs e)
            => ToggleLockSelection();
        #endregion Action Methods
    }
}
