using Audio;
using CoreAudio;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using VolumeControl.Core;
using VolumeControl.Log;
using VolumeControl.WPF.Collections;

namespace VolumeControl.ViewModels
{
    /// <summary>
    /// ViewModel for the <see cref="Audio.AudioDeviceManager"/> class.
    /// </summary>
    public class AudioDeviceManagerVM : DependencyObject, INotifyPropertyChanged
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

            AudioSessionManager.SessionAddedToList += this.AudioSessionManager_SessionAddedToList;
            AudioSessionManager.SessionRemovedFromList += this.AudioSessionManager_SessionRemovedFromList;

            AudioSessionManager.AddSessionManagers(Devices.Select(d => d.AudioDevice.SessionManager));

            Log.Debug("Finished initializing Audio Sessions.");
        }

        private void AudioSessionManager_SessionAddedToList(object? sender, AudioSession e)
            => Dispatcher.Invoke(() => AllSessions.Add(new AudioSessionVM(e)));
        private void AudioSessionManager_SessionRemovedFromList(object? sender, AudioSession e)
        {
            var vm = AllSessions.First(svm => svm.AudioSession.Equals(e));
            AllSessions.Remove(vm);
            vm.Dispose();
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
        public Audio.AudioSessionManager AudioSessionManager { get; }
        public ObservableImmutableList<AudioSessionVM> AllSessions { get; }
        #endregion Properties

        #region Methods (EventHandlers)
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
        #endregion Methods (EventHandlers)
    }
}
