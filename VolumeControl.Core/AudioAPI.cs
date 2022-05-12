using AudioAPI.API;
using AudioAPI.Interfaces;
using AudioAPI.Objects;
using AudioAPI.Objects.Virtual;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using HotkeyLib;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core.Events;
using VolumeControl.Core.HelperTypes.Lists;
using VolumeControl.Log;

namespace VolumeControl.Core
{
    public class AudioAPI : INotifyPropertyChanged
    {
        public AudioAPI()
        {
            _selectedDevice = NullDevice;
            SelectedSession = NullSession;

            ReloadDeviceList();
            _selectedDevice = DefaultDevice;
            ReloadSessionList();
            _selectedSession = NullSession;
            _target = "";

            VolumeStepSize = Settings.VolumeStepSize;
            ReloadOnHotkey = Settings.ReloadOnHotkey;
            ReloadOnInterval = Settings.ReloadOnInterval;
            CheckAllDevices = Settings.CheckAllDevices;
            LockSelectedDevice = Settings.LockSelectedDevice;
            LockSelectedSession = Settings.LockSelectedSession;
            Target = Settings.Target;

            ReloadInterval = ConstrainValue(Settings.ReloadInterval_ms, Settings.ReloadInterval_ms_Min, Settings.ReloadInterval_ms_Max);

            // add an event handler to the reload timer
            ReloadTimer.Elapsed += (sender, e) =>
            {
                ReloadSessionList();
            };
        }

        public void SaveSettings()
        {
            Log.Info("Saving AudioAPI settings to the configuration file...");
            // save settings
            Settings.ReloadInterval_ms = ReloadInterval;
            Settings.Target = Target;
            Settings.VolumeStepSize = VolumeStepSize;
            Settings.ReloadOnHotkey = ReloadOnHotkey;
            Settings.ReloadOnInterval = ReloadOnInterval;
            Settings.CheckAllDevices = CheckAllDevices;
            Settings.LockSelectedDevice = LockSelectedDevice;
            Settings.LockSelectedSession = LockSelectedSession;
            // save to file
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
        public event EventHandler? DeviceListReloaded = null;
        /// <summary>
        /// Triggered when the session list is refreshed.
        /// </summary>
        public event EventHandler? SessionListReloaded = null;
        /// <summary>
        /// Triggered when the selected device is changed.
        /// </summary>
        public event DeviceSwitchEventHandler? SelectedDeviceSwitched = null;
        /// <summary>
        /// Triggered when the selected session is changed.
        /// </summary>
        public event SessionSwitchEventHandler? SelectedSessionSwitched = null;
        /// <summary>
        /// Triggered when the <see cref="Target"/> property is changed.
        /// </summary>
        public event TargetChangedEventHandler? TargetChanged = null;
        /// <summary>
        /// This is used by data bindings to indicate that the target should update from the source.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged = null;

        private void NotifyDeviceListRefresh(EventArgs e) => DeviceListReloaded?.Invoke(this, e);
        private void NotifyProcessListRefresh(EventArgs e) => SessionListReloaded?.Invoke(this, e);
        private void NotifyDeviceSwitch(SwitchEventArgs<IAudioDevice> e) => SelectedDeviceSwitched?.Invoke(this, e);
        private void NotifySessionSwitch(SwitchEventArgs<IProcess> e) => SelectedSessionSwitched?.Invoke(this, e);
        private void NotifyTargetChanged(TargetChangedEventArgs e) => TargetChanged?.Invoke(this, e);
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
            }
        }
        public static int ReloadIntervalMin => Settings.ReloadInterval_ms_Min;
        public static int ReloadIntervalMax => Settings.ReloadInterval_ms_Max;


        public List<AudioDevice> Devices { get; } = new();
        private IAudioDevice _selectedDevice;
        public IAudioDevice SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (LockSelectedDevice)
                    return;

                // clear the session list if we're changing target devices and sessions are device-specific
                if (!CheckAllDevices)
                    Sessions.Clear();

                // change the selected device
                _selectedDevice = value;

                // reload the session list with the new device
                if (!CheckAllDevices && SelectedDevice != NullDevice)
                    ReloadSessionList();

                // trigger events
                NotifyDeviceSwitch(new(_selectedDevice));
                NotifyPropertyChanged();

                Log.Info($"Selected device was changed to '{_selectedDevice.Name}'");
            }
        }
        private bool _lockSelectedDevice;
        /// <summary>
        /// Prevents <see cref="SelectedDevice"/> from being modified.
        /// </summary>
        public bool LockSelectedDevice
        {
            get => _lockSelectedDevice;
            set
            {
                _lockSelectedDevice = value;

                NotifyPropertyChanged();

                Log.Info($"Selected device was {(value ? "" : "un")}locked");
            }
        }
        /// <summary>
        /// When true, the session list contains sessions from all audio devices.
        /// </summary>
        /// <remarks>This changes the behaviour of <see cref="ReloadSessionList"/> so that it checks <i>all devices</i> instead of only the currently selected device.</remarks>
        public bool CheckAllDevices { get; set; }


        public BindableAudioSessionList Sessions { get; } = new();
        private IProcess _selectedSession;
        public IProcess SelectedSession
        {
            get => _selectedSession;
            set
            {
                if (LockSelectedSession)
                    return;
                _selectedSession = value;
                _target = _selectedSession.ProcessIdentifier;
                // Trigger associated events
                NotifySessionSwitch(new(_selectedSession)); //< SelectedSessionSwitched
                NotifyTargetChanged(new(_target)); //< Selected
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Target));

                Log.Info($"Selected session was changed to '{_selectedSession.ProcessIdentifier}'");
            }
        }

        private bool _lockSelectedSession;

        public bool LockSelectedSession
        {
            get => _lockSelectedSession;
            set
            {
                _lockSelectedSession = value;

                NotifyPropertyChanged();

                Log.Info($"Selected session was {(_lockSelectedSession ? "" : "un")}locked");
            }
        }

        private string _target;
        public string Target
        {
            get => _target;
            set
            {
                if (LockSelectedSession)
                    return;
                _target = (_selectedSession = FindSessionWithIdentifier(value)).ProcessIdentifier;
                NotifySessionSwitch(new(_selectedSession));
                NotifyTargetChanged(new(_target));
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(SelectedSession));
            }
        }

        private int _volumeStepSize;
        public int VolumeStepSize
        {
            get => _volumeStepSize;
            set
            {
                _volumeStepSize = value;
                NotifyPropertyChanged();

                Log.Info($"Volume step set to {_volumeStepSize}");
            }
        }

        public void ReloadDeviceList()
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

            Log.Info($"Refreshed the device list.");
        }

        /// <summary>
        /// Performs a selective update on the <see cref="Sessions"/> list.
        /// </summary>
        /// <remarks><list type="number" start="1">
        /// <item><description>Get the list of audio sessions by calling <see cref="WrapperAPI.GetAllSessions()"/> or <see cref="WrapperAPI.GetAllSessions(AudioAPI.WindowsAPI.Audio.MMDeviceAPI.IMMDevice)"/>, depending on if <see cref="CheckAllDevices"/> is set to true.</description></item>
        /// </list></remarks>
        public void ReloadSessionList()
        {
            List<AudioSession> sessions = null!;

            if (CheckAllDevices)
            {
                sessions = new();

                for (int i = Devices.Count - 1; i >= 0; --i)
                {
                    if (Devices[i] is AudioDevice dev)
                    {
                        switch (Devices[i].GetState())
                        {
                        case EDeviceState.AccessError:
                        case EDeviceState.NotPresent:
                        case EDeviceState.Disabled:
                        case EDeviceState.UnPlugged:
                        case EDeviceState.All:
                            continue; // skip devices that aren't active
                        case EDeviceState.Active:
                        default:
                            break; // proceed if the device is active or unknown
                        }

                        // add non-duplicate sessions to the list
                        sessions.AddRange(dev.GetAudioSessions().Where(devSession => !sessions.Any(s => s.ProcessIdentifier.Equals(devSession.ProcessIdentifier, StringComparison.Ordinal))));
                    }
                    else
                    {
                        Log.Error($"{nameof(Devices)}[{i}] contains invalid type '{Devices[i].GetType()}'!");
                    }
                }
            }
            else if (SelectedDevice is AudioDevice dev)
                sessions = dev.GetAudioSessions();

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

            if (sel != null && sel != NullSession && !Sessions.Contains(sel))
            {
                SelectedSession = NullSession;
            }

            NotifyProcessListRefresh(EventArgs.Empty);

            Log.Debug("Refreshed the session list.");
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
        /// <summary>
        /// Clamps the given value between min and max.
        /// </summary>
        /// <remarks>The <paramref name="min"/> and <paramref name="max"/> boundary values are <b>inclusive</b>.</remarks>
        /// <typeparam name="T">Any numerical type.</typeparam>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum allowable value.</param>
        /// <param name="max">The maximum allowable value.</param>
        /// <returns><paramref name="value"/> clamped between <paramref name="min"/> and <paramref name="max"/>.</returns>
        private static T ConstrainValue<T>(T value, T min, T max) where T : IComparable, IComparable<T>, IConvertible, IEquatable<T>, ISpanFormattable, IFormattable
        {
            if (value.CompareTo(min) < 0)
                value = min;
            else if (value.CompareTo(max) > 0)
                value = max;
            return value;
        }

        public void IncrementSessionVolume(int amount)
        {
            if (SelectedSession is AudioSession session)
                session.Volume += amount;
        }
        public void IncrementSessionVolume() => IncrementSessionVolume(VolumeStepSize);
        public void DecrementSessionVolume(int amount)
        {
            if (SelectedSession is AudioSession session)
                session.Volume -= amount;
        }
        public void DecrementSessionVolume() => DecrementSessionVolume(VolumeStepSize);
        public bool GetSessionMute() => SelectedSession is AudioSession session && session.Muted;
        public void SetSessionMute(bool state)
        {
            if (SelectedSession is AudioSession session)
                session.Muted = state;
        }
        public void ToggleSessionMute()
        {
            if (SelectedSession is AudioSession session)
                session.Muted = !session.Muted;
        }

        /// <summary>
        /// Sets <see cref="SelectedSession"/> to the session occurring after this one in <see cref="Sessions"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.
        /// </summary>
        /// <remarks>If <see cref="SelectedSession"/> is set to <see cref="NullSession"/>, the first element in <see cref="Sessions"/> is selected.</remarks>
        public void SelectNextSession()
        {
            if (LockSelectedSession) return;

            if (ReloadOnHotkey) // reload on hotkey
                ReloadSessionList();

            if (SelectedSession is AudioSession session)
            { // a valid audio session is selected
                int index = Sessions.IndexOf(session);
                if (index == -1 || (index += 1) >= Sessions.Count)
                    index = 0;
                SelectedSession = Sessions[index];
            }
            // nothing is selected, select the first element in the list
            else if (Sessions.Count > 0)
                SelectedSession = Sessions[0];
        }
        /// <summary>
        /// Sets <see cref="SelectedSession"/> to the session occurring before this one in <see cref="Sessions"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.
        /// </summary>
        /// <remarks>If <see cref="SelectedSession"/> is set to <see cref="NullSession"/>, the last element in <see cref="Sessions"/> is selected.</remarks>
        public void SelectPreviousSession()
        {
            if (LockSelectedSession) return;

            if (ReloadOnHotkey) // reload on hotkey
                ReloadSessionList();

            if (SelectedSession is AudioSession session)
            { // a valid audio session is selected
                int index = Sessions.IndexOf(session);
                if (index == -1 || (index -= 1) < 0)
                    index = Sessions.Count - 1;
                SelectedSession = Sessions[index];
            }
            // nothing is selected, select the last element in the list
            else if (Sessions.Count > 0)
                SelectedSession = Sessions[^1];
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the device occurring after this one in <see cref="Devices"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.
        /// </summary>
        /// <remarks>If <see cref="SelectedDevice"/> is set to <see cref="NullDevice"/>, the first element in <see cref="Devices"/> is selected.</remarks>
        public void SelectNextDevice()
        {
            if (LockSelectedDevice) return;

            if (ReloadOnHotkey)
                ReloadDeviceList();

            if (SelectedDevice is AudioDevice device)
            {
                int index = Devices.IndexOf(device);
                if (index == -1 || (index += 1) >= Devices.Count)
                    index = 0;
                SelectedDevice = Devices[index];
            }
            else if (Devices.Count > 0)
                SelectedDevice = Devices[0];
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the device occurring before this one in <see cref="Devices"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.
        /// </summary>
        /// <remarks>If <see cref="SelectedDevice"/> is set to <see cref="NullDevice"/>, the last element in <see cref="Devices"/> is selected.</remarks>
        public void SelectPreviousDevice()
        {
            if (LockSelectedDevice) return;

            if (ReloadOnHotkey)
                ReloadDeviceList();

            if (SelectedDevice is AudioDevice device)
            {
                int index = Devices.IndexOf(device);
                if (index == -1 || (index -= 1) < 0)
                    index = Devices.Count - 1;
                SelectedDevice = Devices[index];
            }
            else if (Devices.Count > 0)
                SelectedDevice = Devices[^1];
        }
    }
}
