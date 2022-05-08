using AudioAPI.API;
using AudioAPI.Interfaces;
using AudioAPI.Objects;
using AudioAPI.Objects.Virtual;
using VolumeControl.Core.Events;
using System.Timers;

namespace VolumeControl.Core
{
    public class AudioAPI
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

            ReloadTimer.Elapsed += (sender, e) =>
            {
                RefreshSessions();
            };
        }

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

        private event EventHandler? DeviceListRefreshEvent = null;
        private event EventHandler? ProcessListRefreshEvent = null;
        private event DeviceSwitchEventHandler? DeviceSwitchEvent = null;
        private event SessionSwitchEventHandler? SessionSwitchEvent = null;

        private void NotifyDeviceListRefresh(EventArgs e) => DeviceListRefreshEvent?.Invoke(this, e);
        private void NotifyProcessListRefresh(EventArgs e) => ProcessListRefreshEvent?.Invoke(this, e);
        private void NotifyDeviceSwitch(SwitchEventArgs<IAudioDevice> e) => DeviceSwitchEvent?.Invoke(this, e);
        private void NotifySessionSwitch(SwitchEventArgs<IProcess> e) => SessionSwitchEvent?.Invoke(this, e);
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
        /// <inheritdoc cref="Timer.Interval"/>
        public static double ReloadInterval
        {
            get => ReloadTimer.Interval;
            set => ReloadTimer.Interval = value;
        }
        public static int ReloadIntervalMin => 0;
        public static int ReloadIntervalMax => 120000;


        public readonly List<AudioDevice> Devices;
        private IAudioDevice _selectedDevice;
        public IAudioDevice SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                Sessions.Clear();
                var prev = _selectedDevice;
                _selectedDevice = value;
                RefreshSessions();
                NotifyDeviceSwitch(new(prev, _selectedDevice));
            }
        }


        public readonly BindableAudioSessionList Sessions;
        private IProcess _selectedSession;
        public IProcess SelectedSession
        {
            get => _selectedSession;
            set
            {
                var prev = _selectedSession;
                _selectedSession = value;
                NotifySessionSwitch(new(prev, _selectedSession));
            }
        }

        public void RefreshDevices()
        {
            var devices = WrapperAPI.GetAllDevices();
            var sel = SelectedDevice;

            // remove all devices that aren't in the new list. (exited/stopped)
            Devices.RemoveAll(dev => !devices.Any(d => d.Equals(dev)));
            // add all devices that aren't in the current list. (new)
            Devices.AddRange(devices.Where(dev => !Devices.Any(d => d.Equals(dev))));

            if (sel != null && sel != NullDevice && !Devices.Any(d => d.Equals(sel)))
            {
                SelectedDevice = NullDevice;
            }
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
    }
}
