using NAudio.CoreAudioApi;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core.Events;
using VolumeControl.Core.HelperTypes;
using VolumeControl.Core.Interfaces;
using VolumeControl.Log;

namespace VolumeControl.Core
{
    public class AudioAPI : INotifyPropertyChanged, IDisposable
    {
        public AudioAPI()
        {
            _selectedDevice = null;
            _selectedSession = null;
            _target = "";

            ReloadDeviceList();

            // Set the selected device
            if (Settings.SelectedDevice.Length > 0)
                SelectedDevice = FindDevice(dev => dev.DeviceID.Equals(Settings.SelectedDevice, StringComparison.Ordinal));
            else _selectedDevice = GetDefaultDevice();

            ReloadSessionList();

            Target = Settings.SelectedSession;
            if (Target != Settings.SelectedSession)
                _target = Settings.SelectedSession;

            LockSelectedDevice = Settings.LockSelectedDevice;
            LockSelectedSession = Settings.LockSelectedSession;

            VolumeStepSize = Settings.VolumeStepSize;
            ReloadOnHotkey = Settings.ReloadOnHotkey;
            CheckAllDevices = Settings.CheckAllDevices;

            ReloadOnHotkeyTimer = new()
            {
                Interval = Settings.ReloadOnHotkey_MinInterval,
            };

            ReloadTimer = new()
            {
                Interval = ConstrainValue(Settings.ReloadInterval_ms, Settings.ReloadInterval_ms_Min, Settings.ReloadInterval_ms_Max),
                Enabled = Settings.ReloadOnInterval,
            };

            // add an event handler to the reload timer
            ReloadTimer.Tick += (sender, e) =>
            {
                ReloadSessionList();
            };

            ReloadOnHotkeyTimer.Tick += (sender, e) =>
            {
                _allowReloadOnHotkey = true;
            };
        }

        public void SaveSettings()
        {
            Log.Info("Saving AudioAPI settings to the configuration file...");
            // save settings
            Settings.ReloadOnHotkey_MinInterval = ReloadOnHotkeyTimer.Interval;
            Settings.ReloadOnHotkey = ReloadOnHotkey;

            Settings.ReloadInterval_ms = ReloadInterval;
            Settings.ReloadOnInterval = ReloadOnInterval;

            Settings.LockSelectedDevice = LockSelectedDevice;
            Settings.SelectedDevice = SelectedDevice.DeviceID;

            Settings.SelectedSession = Target;
            Settings.LockSelectedSession = LockSelectedSession;

            Settings.VolumeStepSize = VolumeStepSize;
            Settings.CheckAllDevices = CheckAllDevices;
            // save to file
            Settings.Save();
            Settings.Reload();
            Log.Followup("Done.");
        }

        #region Fields
        private bool _allowReloadOnHotkey = true;

        private IDevice? _selectedDevice = null;
        private bool _lockSelectedDevice;

        private ISession? _selectedSession;
        private bool _lockSelectedSession;

        private string _target;
        private int _volumeStepSize;

        private readonly System.Windows.Forms.Timer ReloadOnHotkeyTimer;
        private readonly System.Windows.Forms.Timer ReloadTimer;
        #endregion Fields

        #region Properties
        private static CoreSettings Settings => CoreSettings.Default;
        private static LogWriter Log => FLog.Log;
        public static int ReloadIntervalMin => Settings.ReloadInterval_ms_Min;
        public static int ReloadIntervalMax => Settings.ReloadInterval_ms_Max;
        public static bool ReloadOnHotkey { get; set; }
        public bool ReloadOnInterval
        {
            get => ReloadTimer.Enabled;
            set => ReloadTimer.Enabled = value;
        }
        /// <inheritdoc cref="System.Timers.Timer.Interval"/>
        /// <remarks><b>This property automatically clamps incoming values between <see cref="ReloadIntervalMin"/> and <see cref="ReloadIntervalMax"/></b></remarks>
        public int ReloadInterval
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
        public List<AudioDevice> Devices { get; } = new();
        public List<AudioSession> Sessions { get; } = new();
        public IDevice? SelectedDevice
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
                if (!CheckAllDevices && _selectedDevice != null)
                    ReloadSessionList();

                // trigger events
                NotifyDeviceSwitch();
                NotifyPropertyChanged();

                Log.Info($"Selected device was changed to '{_selectedDevice?.Name}'");
            }
        }
        public ISession? SelectedSession
        {
            get => _selectedSession;
            set
            {
                if (LockSelectedSession)
                    return;
                _selectedSession = value;
                _target = _selectedSession?.ProcessIdentifier ?? string.Empty;
                // Trigger associated events
                NotifySessionSwitch(); //< SelectedSessionSwitched
                NotifyTargetChanged(new(_target)); //< Selected
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Target));

                Log.Info($"Selected session was changed to '{_selectedSession?.ProcessIdentifier}'");
            }
        }
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
        public string Target
        {
            get => _target;
            set
            {
                if (LockSelectedSession)
                    return;
                _target = (_selectedSession = FindSessionWithIdentifier(value))?.ProcessIdentifier ?? _target;
                NotifySessionSwitch();
                NotifyTargetChanged(new(_target));
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(SelectedSession));
            }
        }
        /// <summary>
        /// When true, the session list contains sessions from all audio devices.
        /// </summary>
        /// <remarks>This changes the behaviour of <see cref="ReloadSessionList"/> so that it checks <i>all devices</i> instead of only the currently selected device.</remarks>
        public bool CheckAllDevices { get; set; }
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
        #endregion Properties

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
        public event EventHandler? SelectedDeviceSwitched = null;
        /// <summary>
        /// Triggered when the selected session is changed.
        /// </summary>
        public event EventHandler? SelectedSessionSwitched = null;
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
        private void NotifyDeviceSwitch() => SelectedDeviceSwitched?.Invoke(this, new());
        private void NotifySessionSwitch() => SelectedSessionSwitched?.Invoke(this, new());
        private void NotifyTargetChanged(TargetChangedEventArgs e) => TargetChanged?.Invoke(this, e);
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events




        public void ReloadDeviceList()
        {
            SelectiveUpdateDevices(GetAllDevices());
        }
        private static List<AudioDevice> GetAllDevices()
        {
            List<AudioDevice> devices = new();

            MMDeviceEnumerator enumerator = new();
            foreach (MMDevice endpoint in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                devices.Add(new AudioDevice(endpoint));
            }
            enumerator.Dispose();

            return devices;
        }
        private void SelectiveUpdateDevices(List<AudioDevice> devices)
        {
            var selID = SelectedDevice?.DeviceID;

            // remove all devices that aren't in the new list. (exited/stopped)
            Devices.RemoveAll(dev => !devices.Any(d => d.Equals(dev)));
            // add all devices that aren't in the current list. (new)
            Devices.AddRange(devices.Where(dev => !Devices.Any(d => d.Equals(dev))));

            // unset the selected device if it doesn't exist anymore:
            if (!LockSelectedDevice && selID != null && !Devices.Any(dev => dev.DeviceID.Equals(selID, StringComparison.Ordinal)))
                SelectedDevice = null;

            if (SelectedDevice == null)
                Sessions.Clear();

            NotifyDeviceListRefresh(EventArgs.Empty);

            Log.Info($"Refreshed the device list.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This method locks the <see cref="_mutex"/> and is thread safe.<br/>Note that the mutex is shared between <see cref="ReloadDeviceList"/>, and as a result these methods should not interact with each other!</remarks>
        public void ReloadSessionList()
        {
            if (Devices.Count == 0 || (!CheckAllDevices && SelectedDevice == null))
                return;

            List<AudioSession> sessions = CheckAllDevices ? GetAllSessionsFromAllDevices() : (SelectedDevice is AudioDevice dev ? dev.GetAudioSessions() : null!);

            if (sessions != null)
                SelectiveUpdateSessions(sessions);
            else Sessions.Clear();
        }
        /// <summary>
        /// Retrieves a list of every <see cref="AudioSession"/> from every active <see cref="AudioDevice"/>.
        /// </summary>
        /// <returns>List of audio sessions.</returns>
        private List<AudioSession> GetAllSessionsFromAllDevices()
        {
            List<AudioSession> sessions = new();

            for (int i = Devices.Count - 1; i >= 0; --i)
            {
                if (Devices[i] is AudioDevice dev)
                {
                    switch (dev.State)
                    {
                    case DeviceState.NotPresent:
                    case DeviceState.Disabled:
                    case DeviceState.Unplugged:
                    case DeviceState.All:
                        continue; // skip devices that aren't active
                    case DeviceState.Active:
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

            return sessions;
        }
        /// <summary>
        /// Performs a selective update of the <see cref="Sessions"/> list.
        /// </summary>
        /// <remarks><b>This method does not use mutexes and is not thread safe!</b><br/>Because of this, it should only be called from within the critical section of a method that <i>does</i> use mutexes, such as <see cref="ReloadSessionList"/>.</remarks>
        /// <param name="sessions">List of sessions to update from.<br/>Any items within <see cref="Sessions"/> that are <b>not</b> present in <paramref name="sessions"/> are removed from <see cref="Sessions"/>.<br/>Any items present in <paramref name="sessions"/> that are <b>not</b> present within <see cref="Sessions"/> are added to <see cref="Sessions"/>.</param>
        private void SelectiveUpdateSessions(List<AudioSession> sessions)
        {
            var selPID = SelectedSession?.PID;

            // remove all sessions that don't appear in the new list (expired sessions)
            Sessions.RemoveAll(session => !sessions.Any(s => s.PID.Equals(session.PID)));
            // add all sessions that aren't already in the current list
            Sessions.AddRange(sessions.Where(s => !Sessions.Any(session => session.PID.Equals(s.PID))));

            if (!LockSelectedSession && selPID != null && !Sessions.Any(s => s.PID.Equals(selPID)))
                SelectedSession = null;

            NotifyProcessListRefresh(EventArgs.Empty);

            Log.Debug("Refreshed the session list.");
        }

        public AudioDevice GetDefaultDevice()
        {
            MMDeviceEnumerator enumerator = new();
            var defaultDev = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            var defaultDevID = defaultDev.ID;
            enumerator.Dispose();

            foreach (var dev in Devices)
            {
                if (dev.DeviceID.Equals(defaultDevID, StringComparison.Ordinal))
                    return dev;
            }

            return new AudioDevice(defaultDev);
        }

        public IDevice? FindDevice(Predicate<AudioDevice> predicate)
        {
            foreach (var device in Devices)
                if (predicate(device))
                    return device;
            return null;
        }

        public ISession? FindSession(Predicate<AudioSession> predicate)
        {
            foreach (var session in Sessions)
                if (predicate(session))
                    return session;
            return null;
        }

        public ISession? FindSessionWithID(int pid) => FindSession(s => s.PID.Equals(pid));

        public ISession? FindSessionWithName(string name, StringComparison sCompareType = StringComparison.OrdinalIgnoreCase) => FindSession(s => s.ProcessName.Equals(name, sCompareType));

        public ISession? FindSessionWithIdentifier(string identifier, StringComparison sCompareType = StringComparison.OrdinalIgnoreCase)
        {
            if (identifier.Length == 0)
                return null;
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

            if (ReloadOnHotkey && _allowReloadOnHotkey)
            {  // reload on hotkey
                _allowReloadOnHotkey = false;
                ReloadOnHotkeyTimer.Start();
                ReloadSessionList();
            }

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
        public void SelectPreviousSession(bool isHotkey = true)
        {
            if (LockSelectedSession) return;

            if (ReloadOnHotkey && _allowReloadOnHotkey)
            { // reload on hotkey
                _allowReloadOnHotkey = false;
                ReloadOnHotkeyTimer.Start();
                ReloadSessionList();
            }

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

            if (ReloadOnHotkey && _allowReloadOnHotkey)
            {
                _allowReloadOnHotkey = false;
                ReloadOnHotkeyTimer.Start();
                ReloadDeviceList();
            }

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

            if (ReloadOnHotkey && _allowReloadOnHotkey)
            {
                _allowReloadOnHotkey = false;
                ReloadOnHotkeyTimer.Start();
                ReloadDeviceList();
            }

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

        /// <inheritdoc/>
        public void Dispose()
        {
            ReloadTimer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
