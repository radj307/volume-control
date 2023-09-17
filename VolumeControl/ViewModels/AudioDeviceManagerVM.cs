using CoreAudio;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using VolumeControl.Core;
using VolumeControl.CoreAudio;
using VolumeControl.CoreAudio.Events;
using VolumeControl.Log;
using VolumeControl.WPF.Collections;

namespace VolumeControl.ViewModels
{
    /// <summary>
    /// ViewModel for the <see cref="CoreAudio.AudioDeviceManager"/> class.
    /// </summary>
    public sealed class AudioDeviceManagerVM : DependencyObject, INotifyPropertyChanged
    {
        public AudioDeviceManagerVM()
        {
            AudioDeviceManager = new(DataFlow.Render);

            Log.Debug("Initializing Audio Devices.");

            Devices = new();

            AudioDeviceManager.DeviceAddedToList += AudioDeviceManager_DeviceAddedToList;
            AudioDeviceManager.DeviceRemovedFromList += AudioDeviceManager_DeviceRemovedFromList;

            foreach (var device in AudioDeviceManager.Devices)
            {
                Devices.Add(new(device));
            }

            Log.Debug("Finished initializing Audio Devices, started initializing Audio Sessions.");

            AllSessions = new();

            AudioSessionManager = new();

            AudioSessionManager.PreviewSessionName += this.AudioSessionManager_PreviewSessionName;
            AudioSessionManager.SessionAddedToList += this.AudioSessionManager_SessionAddedToList;
            AudioSessionManager.SessionRemovedFromList += this.AudioSessionManager_SessionRemovedFromList;

            foreach (var sessionManager in Devices.Select(d => d.AudioDevice.SessionManager))
            {
                AudioSessionManager.AddSessionManager(sessionManager);
            }

            AudioDeviceSelector = new(AudioDeviceManager);
            AudioSessionSelector = new(AudioSessionManager)
            {
                LockSelection = Settings.LockTargetSession,
            };

            Log.Debug("Finished initializing Audio Sessions.");
        }

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Fields
        public readonly AudioDeviceManager AudioDeviceManager;
        #endregion Fields

        #region Properties
        private static Config Settings => (Config.Default as Config)!;
        private static LogWriter Log => FLog.Log;
        public ObservableImmutableList<AudioDeviceVM> Devices { get; }
        public CoreAudio.AudioSessionManager AudioSessionManager { get; }
        public ObservableImmutableList<AudioSessionVM> AllSessions { get; }
        public AudioDeviceSelector AudioDeviceSelector { get; }
        public AudioSessionSelector AudioSessionSelector { get; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Gets the viewmodel object for the given <paramref name="audioDevice"/> instance.
        /// </summary>
        /// <param name="audioDevice">An <see cref="AudioDevice"/> instance.</param>
        /// <returns>The <see cref="AudioDeviceVM"/> instance associated with the given device, or <see langword="null"/> if it wasn't found.</returns>
        public AudioDeviceVM? GetAudioDeviceVM(AudioDevice audioDevice)
            => Devices.FirstOrDefault(audioDeviceVM => audioDeviceVM!.AudioDevice.Equals(audioDevice), null);
        /// <summary>
        /// Gets the viewmodel object for the given <paramref name="audioSession"/> instance.
        /// </summary>
        /// <param name="audioSession">An <see cref="AudioSession"/> instance.</param>
        /// <returns>The <see cref="AudioSessionVM"/> instance associated with the given session, or <see langword="null"/> if it wasn't found.</returns>
        public AudioSessionVM? GetAudioSessionVM(AudioSession audioSession)
            => AllSessions.FirstOrDefault(audioSessionVM => audioSessionVM!.AudioSession.Equals(audioSession), null);
        #endregion Methods

        #region AudioDeviceManager EventHandler Methods
        private void AudioDeviceManager_DeviceAddedToList(object? sender, AudioDevice e)
        {
            var vm = new AudioDeviceVM(e);
            Log.Info($"Adding Audio Device: {vm.Name}");
            Dispatcher.Invoke(() => Devices.Add(vm));
            AudioSessionManager.AddSessionManager(vm.AudioDevice.SessionManager);
        }
        private void AudioDeviceManager_DeviceRemovedFromList(object? sender, AudioDevice e)
        {
            var vm = Devices.First(device => device.AudioDevice.Equals(e));
            Log.Info($"Removing Audio Device: {vm.Name}");
            Devices.Remove(vm);
            AudioSessionManager.RemoveSessionManager(vm.AudioDevice.SessionManager);
            vm.Dispose();
        }
        #endregion AudioDeviceManager EventHandler Methods

        #region AudioSessionManager EventHandler Methods
        private void AudioSessionManager_SessionAddedToList(object? sender, AudioSession e)
            => Dispatcher.Invoke(() => AllSessions.Add(new AudioSessionVM(e)));
        private void AudioSessionManager_SessionRemovedFromList(object? sender, AudioSession e)
        {
            var vm = AllSessions.First(svm => svm.AudioSession.Equals(e));
            AllSessions.Remove(vm);
            vm.Dispose();
        }
        private void AudioSessionManager_PreviewSessionName(object sender, PreviewSessionNameEventArgs e)
        {
            if (e.SessionName.Equals("Idle", System.StringComparison.Ordinal))
            {
                e.SessionName = "System Sounds";
            }
        }
        #endregion AudioSessionManager EventHandler Methods
    }
}
