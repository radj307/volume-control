﻿using AudioAPI.API;
using AudioAPI.Interfaces;
using AudioAPI.Objects;
using AudioAPI.Objects.Virtual;
using VolumeControl.Core.Events;
using System.Timers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Log.Properties;
using VolumeControl.Log;

namespace VolumeControl.Core
{
    public class AudioAPI : INotifyPropertyChanged
    {
        public AudioAPI()
        {
            Devices = new();
            Sessions = new();
            _selectedDevice = NullDevice;
            SelectedSession = NullSession;

            RefreshDevices();
            _selectedDevice = DefaultDevice;
            RefreshSessions();
            _selectedSession = NullSession;
            _target = "";

            // load settings
            Target = Settings.Target;
            ReloadInterval = Settings.ReloadInterval_ms > 0d ? Settings.ReloadInterval_ms : Settings.ReloadInterval_ms_Default;
            // add an event handler to the reload timer
            ReloadTimer.Elapsed += (sender, e) =>
            {
                RefreshSessions();
            };
        }
        ~AudioAPI()
        {
            Log.Info("Saving AudioAPI settings to the configuration file...");
            // save settings
            Settings.ReloadInterval_ms = ReloadInterval;
            Settings.Target = Target;
            Settings.Save();
            Settings.Reload();
            Log.Followup("Done.");
        }

        private static CoreSettings Settings => CoreSettings.Default;
        private static LogWriter Log => FLog.Log;

        #region Events
        /// <summary>
        /// Triggered when the device list is refreshed.
        /// </summary>
        public event EventHandler DeviceListRefreshed
        {
            add => DeviceListRefreshEvent += value;
            remove => DeviceListRefreshEvent -= value;
        }
        /// <summary>
        /// Triggered when the session list is refreshed.
        /// </summary>
        public event EventHandler SessionListRefreshed
        {
            add => ProcessListRefreshEvent += value;
            remove => ProcessListRefreshEvent -= value;
        }
        /// <summary>
        /// Triggered when the selected device is changed.
        /// </summary>
        public event DeviceSwitchEventHandler SelectedDeviceSwitched
        {
            add => DeviceSwitchEvent += value;
            remove => DeviceSwitchEvent -= value;
        }
        /// <summary>
        /// Triggered when the selected session is changed.
        /// </summary>
        public event SessionSwitchEventHandler SelectedSessionSwitched
        {
            add => SessionSwitchEvent += value;
            remove => SessionSwitchEvent -= value;
        }
        /// <summary>
        /// Triggered when the <see cref="Target"/> property is changed.
        /// </summary>
        public event TargetChangedEventHandler TargetChanged
        {
            add => TargetChangedEvent += value;
            remove => TargetChangedEvent -= value;
        }

        private event EventHandler? DeviceListRefreshEvent = null;
        private event EventHandler? ProcessListRefreshEvent = null;
        private event DeviceSwitchEventHandler? DeviceSwitchEvent = null;
        private event SessionSwitchEventHandler? SessionSwitchEvent = null;
        private event TargetChangedEventHandler? TargetChangedEvent = null;
        public event PropertyChangedEventHandler? PropertyChanged = null;

        private void NotifyDeviceListRefresh(EventArgs e) => DeviceListRefreshEvent?.Invoke(this, e);
        private void NotifyProcessListRefresh(EventArgs e) => ProcessListRefreshEvent?.Invoke(this, e);
        private void NotifyDeviceSwitch(SwitchEventArgs<IAudioDevice> e) => DeviceSwitchEvent?.Invoke(this, e);
        private void NotifySessionSwitch(SwitchEventArgs<IProcess> e) => SessionSwitchEvent?.Invoke(this, e);
        private void NotifyTargetChanged(TargetChangedEventArgs e) => TargetChangedEvent?.Invoke(this, e);
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        public static readonly VirtualAudioDevice NullDevice = new();
        public static readonly VirtualAudioSession NullSession = new();
        internal static readonly AudioDevice DefaultDevice = new(WrapperAPI.GetDefaultDevice());
        private static readonly System.Timers.Timer ReloadTimer = new() { AutoReset = true };

        public static bool ReloadOnHotkey
        {
            get;
            set;
        }
        public static bool ReloadOnInterval
        {
            get => ReloadTimer.Enabled;
            set => ReloadTimer.Enabled = value;
        }
        /// <inheritdoc cref="System.Timers.Timer.Interval"/>
        /// <remarks><b>This property automatically clamps incoming values between <see cref="ReloadIntervalMin"/> and <see cref="ReloadIntervalMax"/></b></remarks>
        public static double ReloadInterval
        {
            get => ReloadTimer.Interval;
            set
            {
                if (value > ReloadIntervalMax)
                    value = ReloadIntervalMax;
                else if (value < ReloadIntervalMin)
                    value = ReloadIntervalMin;
                ReloadTimer.Interval = value;
                Log.Debug("");
            }
        }
        public static int ReloadIntervalMin => Settings.ReloadInterval_ms_Min;
        public static int ReloadIntervalMax => Settings.ReloadInterval_ms_Max;


        public readonly List<AudioDevice> Devices;
        private IAudioDevice _selectedDevice;
        public IAudioDevice SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (LockSelection)
                    return;
                Sessions.Clear();
                var prev = _selectedDevice;
                _selectedDevice = value;
                RefreshSessions();
                NotifyDeviceSwitch(new(prev, _selectedDevice));
                NotifyPropertyChanged();
            }
        }


        public readonly BindableAudioSessionList Sessions;
        private IProcess _selectedSession;
        public IProcess SelectedSession
        {
            get => _selectedSession;
            set
            {
                if (LockSelection)
                    return;
                var prev = _selectedSession;
                _selectedSession = value;
                _target = _selectedSession.ProcessIdentifier;
                NotifySessionSwitch(new(prev, _selectedSession));
                NotifyTargetChanged(new(_target));
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Target));
            }
        }

        private string _target;
        public string Target
        {
            get => _target;
            set
            {
                if (LockSelection)
                    return;
                var prev = _selectedSession;
                _target = (_selectedSession = FindSessionWithIdentifier(value)).ProcessIdentifier;
                NotifySessionSwitch(new(prev, _selectedSession));
                NotifyTargetChanged(new(_target));
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(SelectedSession));
            }
        }
        public bool LockSelection { get; set; }


        public void RefreshDevices()
        {
            var devices = WrapperAPI.GetAllDevices();
            var sel = SelectedDevice;

            // remove all devices that aren't in the new list. (exited/stopped)
            Devices.RemoveAll(dev => !devices.Any(d => d.Equals(dev)));
            // add all devices that aren't in the current list. (new)
            Devices.AddRange(devices.Where(dev => !Devices.Any(d => d.Equals(dev))));

            // unset the selected device if it doesn't exist anymore:
            if (sel != null && sel != NullDevice && !Devices.Contains(sel))
                SelectedDevice = NullDevice;

            NotifyDeviceListRefresh(EventArgs.Empty);
        }

        public void RefreshSessions()
        {
            var sessions = WrapperAPI.GetAllSessions();
            var sel = SelectedSession;

            foreach (var session in Sessions.List)
            {
                if (!sessions.Any(s => s.Equals(session)))
                {
                    Sessions.Remove(session); //< remove all sessions in the active list that have expired
                }
            }

            foreach (var session in sessions)
            {
                if (!Sessions.List.Any(s => s.Equals(session)))
                {
                    Sessions.List.Add(session);
                }
            }

            if (sel != NullSession && !Sessions.Contains(sel))
            {
                SelectedSession = NullSession;
            }
            NotifyProcessListRefresh(EventArgs.Empty);
        }

        public IProcess FindSession(Predicate<AudioSession> predicate)
        {
            foreach (var session in Sessions)
                if (predicate(session))
                    return session;
            return NullSession;
        }

        public IProcess FindSessionWithID(int pid) => FindSession(s => s.PID.Equals(pid));

        public IProcess FindSessionWithName(string name, StringComparison sCompareType = StringComparison.OrdinalIgnoreCase) => FindSession(s => s.ProcessName.Equals(name, sCompareType));

        public IProcess FindSessionWithIdentifier(string identifier, StringComparison sCompareType = StringComparison.OrdinalIgnoreCase)
        {
            if (identifier.Length == 0)
                return NullSession;
            int i = identifier.IndexOf(':');
            if (i != -1)
            {
                if (int.TryParse(identifier[..i], out int pid))
                    return FindSessionWithID(pid);
                else
                    return FindSessionWithName(identifier[(i + 1)..]);
            }
            else if (identifier.All(char.IsDigit) && int.TryParse(identifier, out int pid))
            {
                return FindSessionWithID(pid);
            }
            else return FindSessionWithName(identifier);
        }
    }
}