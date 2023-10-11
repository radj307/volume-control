using CoreAudio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using VolumeControl.Core;
using VolumeControl.CoreAudio;
using VolumeControl.CoreAudio.Events;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;
using VolumeControl.WPF.Collections;

namespace VolumeControl.ViewModels
{
    /// <summary>
    /// ViewModel for the <see cref="CoreAudio.AudioDeviceManager"/> class.
    /// Implements all of the VolumeControl.CoreAudio assembly for the view.
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
            AudioSessionManager.AddedSessionToList += this.AudioSessionManager_AddedSessionToList;
            AudioSessionManager.RemovedSessionFromList += this.AudioSessionManager_RemovedSessionFromList;

            // hide sessions when started if their name is in the hidden sessions list
            AudioSessionManager.PreviewSessionIsHidden += this.AudioSessionManager_PreviewSessionIsHidden;
            // hide/unhide sessions when added/removed from the hidden sessions list
            Settings.HiddenSessionProcessNames.CollectionChanged += this.HiddenSessionProcessNames_CollectionChanged;

            // populate the AudioSessionManager with AudioDeviceSessionManager instances, which also populates the lists of sessions
            Devices.Select(d => d.AudioDevice.SessionManager).ForEach(AudioSessionManager.AddSessionManager);

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

        #region EventHandlers

        #region AudioDeviceManager
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
        #endregion AudioDeviceManager

        #region AudioSessionManager
        private void AudioSessionManager_AddedSessionToList(object? sender, AudioSession e)
            => Dispatcher.Invoke(() => AllSessions.Add(new AudioSessionVM(e)));
        private void AudioSessionManager_RemovedSessionFromList(object? sender, AudioSession e)
        {
            // check if the vm actually exists to prevent possible exception in rare cases
            if (AllSessions.FirstOrDefault(svm => svm.AudioSession.Equals(e)) is AudioSessionVM vm)
            {
                AllSessions.Remove(vm);
                vm.Dispose();
            }
        }
        private void AudioSessionManager_PreviewSessionName(object sender, PreviewSessionNameEventArgs e)
        {
            if (e.SessionName.Equals("Idle", System.StringComparison.Ordinal))
            {
                e.SessionName = "System Sounds";
            }
        }
        private void AudioSessionManager_PreviewSessionIsHidden(object sender, PreviewSessionIsHiddenEventArgs e)
        {
            e.SessionIsHidden = Settings.HiddenSessionProcessNames.Any(name => e.AudioSession.HasMatchingName(name, System.StringComparison.OrdinalIgnoreCase));
        }
        #endregion AudioSessionManager

        #region HiddenSessionProcessNames
        /// <summary>
        /// hides and unhides sessions when added/removed from the hidden sessions list.
        /// </summary>
        private void HiddenSessionProcessNames_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            { // unhide removed items
                for (int i = 0, max = e.OldItems.Count; i < max; ++i)
                {
                    if (e.OldItems[i] is not string name)
                        continue;

                    if (AudioSessionManager.HiddenSessions.FirstOrDefault(hiddenSession => hiddenSession.HasMatchingName(name, StringComparison.OrdinalIgnoreCase))
                        is AudioSession session)
                    {
                        AudioSessionManager.UnhideSession(session);
                    }
                }
            }

            if (e.NewItems != null)
            { // hide added items
                for (int i = 0, max = e.NewItems.Count; i < max; ++i)
                {
                    if (e.NewItems[i] is not string name)
                        continue;

                    if (AudioSessionManager.Sessions.FirstOrDefault(session => session.HasMatchingName(name, StringComparison.OrdinalIgnoreCase))
                        is AudioSession session)
                    {
                        AudioSessionManager.HideSession(session);
                    }
                }
            }
        }
        #endregion HiddenSessionProcessNames

        #endregion EventHandlers
    }
}
