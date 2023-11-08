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
            var doDebugLogging = FLog.Log.FilterEventType(EventType.DEBUG);
            if (doDebugLogging) FLog.Debug("Started initializing CoreAudio APIs.");

            // # INIT DEVICES #
            // setup the device manager
            AudioDeviceManager = new(Settings.EnableInputDevices ? DataFlow.All : DataFlow.Render);

            // setup the devices list
            Devices = new();

            // attach handlers for devices being added/removed at runtime
            AudioDeviceManager.DeviceAddedToList += AudioDeviceManager_DeviceAddedToList;
            AudioDeviceManager.DeviceRemovedFromList += AudioDeviceManager_DeviceRemovedFromList;

            // populate the devices list
            for (int i = 0, max = AudioDeviceManager.Devices.Count; i < max; ++i)
            {
                Devices.Add(new AudioDeviceVM(AudioDeviceManager.Devices[i]));
            }

            // setup the device selection manager
            AudioDeviceSelector = new(AudioDeviceManager);
            SelectedDevice = AudioDeviceSelector.Selected != null ? GetAudioDeviceVM(AudioDeviceSelector.Selected) : null;
            AudioDeviceSelector.PropertyChanged += this.AudioDeviceSelector_PropertyChanged;

            if (doDebugLogging) FLog.Debug($"Successfully initialized {AudioDeviceManager.Devices.Count} audio devices.");

            // # INIT SESSIONS #
            // setup the session manager
            AudioSessionManager = new();

            // setup the sessions list
            AllSessions = new();

            // attach handlers for sessions being added/removed at runtime
            AudioSessionManager.AddedSessionToList += this.AudioSessionManager_AddedSessionToList;
            AudioSessionManager.RemovedSessionFromList += this.AudioSessionManager_RemovedSessionFromList;

            // attach handler to override session names
            AudioSessionManager.PreviewSessionName += this.AudioSessionManager_PreviewSessionName;

            // attach handler to set session hidden state when added
            AudioSessionManager.PreviewSessionIsHidden += this.AudioSessionManager_PreviewSessionIsHidden;
            // attach handler to hide/unhide sessions when the hidden sessions names list is changed
            Settings.HiddenSessionProcessNames.CollectionChanged += this.HiddenSessionProcessNames_CollectionChanged;

            // setup the session selection manager
            SelectedSessions = new();
            SelectedSessionsComparer = Comparer<AudioSessionVM>.Create((a, b) => AllSessions.IndexOf(a) - AllSessions.IndexOf(b));
            AudioSessionMultiSelector = new(AudioSessionManager)
            {
                LockSelection = Settings.LockTargetSession,
                LockCurrentIndex = Settings.LockTargetSession,
                LockCurrentIndexOnLockSelection = true
            };

            // setup the session synchronizer
            //SessionSync = new(this, initialState: false);

            // populate the sessions lists
            Devices.Select(d => d.AudioDevice.SessionManager).ForEach(AudioSessionManager.AddSessionManager);

            // select all previously-selected sessions
            foreach (var item in Settings.SelectedSessions)
            {
                if (AudioSessionManager.FindSessionWithSimilarProcessIdentifier(item.ProcessIdentifier, StringComparison.OrdinalIgnoreCase) is AudioSession session)
                {
                    AudioSessionMultiSelector.SetSessionIsSelected(session, true);
                    this.SelectedSessions.Add(GetAudioSessionVM(session)!);
                }
            }
            // set the current session
            var currentSession = AudioSessionManager.FindSessionWithSimilarProcessIdentifier(Settings.TargetSession.ProcessIdentifier, StringComparison.OrdinalIgnoreCase);
            AudioSessionMultiSelector.CurrentSession = currentSession;
            this.CurrentSession = currentSession == null ? null : GetAudioSessionVM(currentSession);

            // attach handlers to session selection events
            AudioSessionMultiSelector.SessionSelected += this.AudioSessionMultiSelector_SessionSelected;
            AudioSessionMultiSelector.SessionDeselected += this.AudioSessionMultiSelector_SessionDeselected;
            AudioSessionMultiSelector.CurrentSessionChanged += this.AudioSessionMultiSelector_CurrentSessionChanged;

            // enable session sync
            //SessionSync.IsEnabled = Settings.EnableSessionSync;

            if (doDebugLogging) FLog.Debug($"Successfully initialized {AudioSessionManager.Sessions.Count + AudioSessionManager.HiddenSessions.Count} {(AudioSessionManager.HiddenSessions.Count == 0 ? "" : $"({AudioSessionManager.HiddenSessions.Count} hidden)")} audio sessions.");

            if (doDebugLogging) FLog.Debug("Finished initializing CoreAudio APIs.");
        }

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Fields
        public readonly AudioDeviceManager AudioDeviceManager;
        #endregion Fields

        #region Properties
        private static Config Settings => Config.Default;
        public ObservableImmutableList<AudioDeviceVM> Devices { get; }
        public CoreAudio.AudioSessionManager AudioSessionManager { get; }
        public ObservableImmutableList<AudioSessionVM> AllSessions { get; }
        public ObservableImmutableList<AudioSessionVM> SelectedSessions { get; }
        public Comparer<AudioSessionVM> SelectedSessionsComparer { get; }
        public AudioSessionVM? CurrentSession { get; set; }
        public AudioDeviceSelector AudioDeviceSelector { get; }
        public AudioDeviceVM? SelectedDevice { get; set; }
        public AudioSessionMultiSelector AudioSessionMultiSelector { get; }
        // public SessionSyncVM SessionSync { get; }
        public bool? AllSessionsSelected
        {
            get
            {
                var selected = SelectedSessions.Count;
                var total = AllSessions.Count;

                if (selected == total) return true;
                else if (selected == 0) return false;
                else return null;
            }
            set => AudioSessionMultiSelector.SetAllSessionSelectionStates(value == true);
        }
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
            FLog.Info($"Adding Audio Device: {vm.Name}");
            Dispatcher.Invoke(() => Devices.Add(vm));
            AudioSessionManager.AddSessionManager(vm.AudioDevice.SessionManager);
        }
        private void AudioDeviceManager_DeviceRemovedFromList(object? sender, AudioDevice e)
        {
            var vm = Devices.First(device => device.AudioDevice.Equals(e));
            FLog.Info($"Removing Audio Device: {vm.Name}");
            Devices.Remove(vm);
            AudioSessionManager.RemoveSessionManager(vm.AudioDevice.SessionManager);
            vm.Dispose();
        }
        #endregion AudioDeviceManager

        #region AudioSessionManager
        private void AudioSessionManager_AddedSessionToList(object? sender, AudioSession e)
        {
            Dispatcher.Invoke(() => AllSessions.Add(new AudioSessionVM(this, e)));
            NotifyPropertyChanged(nameof(AllSessionsSelected));
        }
        private void AudioSessionManager_RemovedSessionFromList(object? sender, AudioSession e)
        {
            // check if the vm actually exists to prevent possible exception in rare cases
            if (AllSessions.FirstOrDefault(svm => svm.AudioSession.Equals(e)) is AudioSessionVM vm)
            {
                AllSessions.Remove(vm);
                vm.Dispose();
            }
            NotifyPropertyChanged(nameof(AllSessionsSelected));
        }
        private void AudioSessionManager_PreviewSessionName(object sender, PreviewSessionNameEventArgs e)
        {
            if (e.AudioSession.PID.Equals(0))
            {
                e.SessionName = "System Sounds";
            }
        }
        private void AudioSessionManager_PreviewSessionIsHidden(object sender, PreviewSessionIsHiddenEventArgs e)
        {
            e.SessionIsHidden = Settings.HiddenSessionProcessNames.Any(name => e.AudioSession.HasMatchingName(name, StringComparison.OrdinalIgnoreCase));
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

        #region AudioSessionMultiSelector
        private void AudioSessionMultiSelector_SessionSelected(object? sender, AudioSession e)
        {
            Settings.SelectedSessions.AddIfUnique(e.GetTargetInfo());
            this.SelectedSessions.Add(GetAudioSessionVM(e)!);
            this.SelectedSessions.Sort(SelectedSessionsComparer);
            // update the all selected checkbox
            NotifyPropertyChanged(nameof(AllSessionsSelected));
        }
        private void AudioSessionMultiSelector_SessionDeselected(object? sender, AudioSession e)
        {
            Settings.SelectedSessions.Remove(e.GetTargetInfo());
            this.SelectedSessions.Remove(GetAudioSessionVM(e)!);
            // update the all selected checkbox
            NotifyPropertyChanged(nameof(AllSessionsSelected));
        }
        private void AudioSessionMultiSelector_CurrentSessionChanged(object? sender, AudioSession? e)
        {
            CurrentSession = e == null ? null : GetAudioSessionVM(e);
        }
        #endregion AudioSessionMultiSelector

        #region AudioDeviceSelector
        private void AudioDeviceSelector_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        { // update the SelectedDevice property
            if (e.PropertyName == null) return;

            if (e.PropertyName.Equals(nameof(AudioDeviceSelector.Selected), StringComparison.Ordinal))
            {
                SelectedDevice = AudioDeviceSelector.Selected != null ? GetAudioDeviceVM(AudioDeviceSelector.Selected) : null;
            }
        }
        #endregion AudioDeviceSelector

        #endregion EventHandlers
    }
}
