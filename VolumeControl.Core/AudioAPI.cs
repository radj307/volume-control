using NAudio.CoreAudioApi;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core.Events;
using VolumeControl.Core.HelperTypes;
using VolumeControl.Core.HelperTypes.Lists;
using VolumeControl.Core.Interfaces;
using VolumeControl.Log;

namespace VolumeControl.Core
{
    public class AudioAPI : INotifyPropertyChanged, IDisposable
    {
        #region Initializers
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

            var targetSession = FindSessionWithIdentifier(Settings.SelectedSession, false);
            if (targetSession != null)
                Target = targetSession.ProcessIdentifier;
            else Target = Settings.SelectedSession;

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
            Settings.SelectedDevice = SelectedDevice?.DeviceID ?? string.Empty;

            Settings.SelectedSession = Target;
            Settings.LockSelectedSession = LockSelectedSession;

            Settings.VolumeStepSize = VolumeStepSize;
            Settings.CheckAllDevices = CheckAllDevices;
            // save to file
            Settings.Save();
            Settings.Reload();
            Log.Followup("Done.");
        }
        #endregion Initializers

        #region Fields
        private bool _allowReloadOnHotkey = true;

        private IDevice? _selectedDevice = null;
        private bool _lockSelectedDevice;

        private ISession? _selectedSession;
        private bool _lockSelectedSession;

        private string _target;
        private int _volumeStepSize;
        private IEnumerable<string>? _targetAutoCompleteSource = null;

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
        public ObservableList<AudioDevice> Devices { get; } = new();
        public ObservableList<AudioSession> Sessions { get; } = new();
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
                NotifyLockSelectedDeviceChanged();

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
                NotifyLockSelectedSessionChanged();

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

                if (FindSessionWithIdentifier(value) is ISession session)
                    SelectedSession = session;
                else _target = value;

                NotifyTargetChanged(new(_target));
                NotifyPropertyChanged();
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

        public IEnumerable<string> TargetAutoCompleteSource
        {
            get => _targetAutoCompleteSource ??= (Target.Length == 0 || char.IsDigit(Target.First())) ? GetSessionIdentifiers(SessionIdentifierFormat.Both) : GetSessionIdentifiers(SessionIdentifierFormat.Name);
        }
        #endregion Properties

        #region Events
        /// <summary>
        /// Triggered when the device list is refreshed.
        /// </summary>
        public event EventHandler? DeviceListReloaded;
        /// <summary>
        /// Triggered when the session list is refreshed.
        /// </summary>
        public event EventHandler? SessionListReloaded;
        /// <summary>
        /// Triggered when the selected device is changed.
        /// </summary>
        public event EventHandler? SelectedDeviceSwitched;
        /// <summary>
        /// Triggered when the value of the <see cref="LockSelectedDevice"/> property is changed.
        /// </summary>
        public event EventHandler? LockSelectedDeviceChanged;
        /// <summary>
        /// Triggered when the selected session is changed.
        /// </summary>
        public event EventHandler? SelectedSessionSwitched;
        /// <summary>
        /// Triggered when the value of the <see cref="LockSelectedSession"/> property is changed.
        /// </summary>
        public event EventHandler? LockSelectedSessionChanged;
        /// <summary>
        /// Triggered when the <see cref="Target"/> property is changed.
        /// </summary>
        public event TargetChangedEventHandler? TargetChanged;
        /// <summary>
        /// This is used by data bindings to indicate that the target should update from the source.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyDeviceListRefresh(EventArgs e) => DeviceListReloaded?.Invoke(this, e);
        private void NotifyProcessListRefresh(EventArgs e) => SessionListReloaded?.Invoke(this, e);
        private void NotifyDeviceSwitch() => SelectedDeviceSwitched?.Invoke(this, new());
        private void NotifyLockSelectedDeviceChanged() => LockSelectedDeviceChanged?.Invoke(this, new());
        private void NotifySessionSwitch() => SelectedSessionSwitched?.Invoke(this, new());
        private void NotifyLockSelectedSessionChanged() => LockSelectedSessionChanged?.Invoke(this, new());
        private void NotifyTargetChanged(TargetChangedEventArgs e) => TargetChanged?.Invoke(this, e);
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Methods
        #region UpdateDevices
        public void ReloadDeviceList()
        {
            SelectiveUpdateDevices(GetAllDevices());
        }
        private void SelectiveUpdateDevices(List<AudioDevice> devices)
        {
            var selID = SelectedDevice?.DeviceID;

            // remove all devices that aren't in the new list. (exited/stopped)
            for (int i = Devices.Count - 1; i >= 0; --i)
                if (!devices.Any(d => d.Equals(Devices[i])))
                    Devices.RemoveAt(i);

            // add all devices that aren't in the current list. (new)
            Devices.AddRange(devices.Where(dev => !Devices.Any(d => d.Equals(dev))));

            if (!LockSelectedDevice && selID != null)
                SelectedDevice = Devices.FirstOrDefault(dev => dev.DeviceID.Equals(selID, StringComparison.Ordinal));

            if (SelectedDevice == null)
                Sessions.Clear();

            NotifyDeviceListRefresh(EventArgs.Empty);
            NotifyPropertyChanged(nameof(Devices));

            Log.Info($"Refreshed the device list.");
        }
        #endregion UpdateDevices
        #region Device
        /// <summary>Gets the current default audio endpoint.</summary>
        /// <returns><see cref="AudioDevice"/></returns>
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
        /// <summary>Enumerates audio endpoints using the Core Audio API to get a list of all devices with the specified modes.</summary>
        /// <returns>A list of devices that are currently active.</returns>
        private static List<AudioDevice> GetAllDevices(DataFlow flow = DataFlow.Render, DeviceState state = DeviceState.Active)
        {
            List<AudioDevice> devices = new();

            MMDeviceEnumerator enumerator = new();
            foreach (MMDevice endpoint in enumerator.EnumerateAudioEndPoints(flow, state))
            {
                devices.Add(new AudioDevice(endpoint));
            }
            enumerator.Dispose();

            return devices;
        }
        /// <summary>Gets a device from <see cref="Devices"/> using the given <paramref name="predicate"/> function.</summary>
        /// <param name="predicate">A predicate function that accepts <see cref="AudioDevice"/> types.</param>
        /// <returns><see cref="IDevice"/> if successful, or null if no matching devices were found.</returns>
        public IDevice? FindDevice(Predicate<AudioDevice> predicate)
        {
            foreach (var device in Devices)
                if (predicate(device))
                    return device;
            return null;
        }
        #endregion Device

        #region ReloadSessions
        public void ReloadSessionList(bool reloadDevicesFirst = false)
        {
            if (reloadDevicesFirst)
                ReloadDeviceList();

            if (Devices.Count == 0 || (!CheckAllDevices && SelectedDevice == null))
                return;

            if ((CheckAllDevices ? GetAllSessionsFromAllDevices() : (SelectedDevice is AudioDevice dev ? dev.GetAudioSessions() : null!)) is List<AudioSession> sessions)
                SelectiveUpdateSessions(sessions);
            else Sessions.Clear();
            // reset autocomplete for target
            _targetAutoCompleteSource = null;
        }
        /// <summary>
        /// Performs a selective update of the <see cref="Sessions"/> list.
        /// </summary>
        /// <remarks><b>This method does not use mutexes and is not thread safe!</b><br/>Because of this, it should only be called from within the critical section of a method that <i>does</i> use mutexes, such as <see cref="ReloadSessionList"/>.</remarks>
        /// <param name="sessions">List of sessions to update from.<br/>Any items within <see cref="Sessions"/> that are <b>not</b> present in <paramref name="sessions"/> are removed from <see cref="Sessions"/>.<br/>Any items present in <paramref name="sessions"/> that are <b>not</b> present within <see cref="Sessions"/> are added to <see cref="Sessions"/>.</param>
        private void SelectiveUpdateSessions(List<AudioSession> sessions)
        {
            var selPID = SelectedSession?.PID;

            for (int i = Sessions.Count - 1; i >= 0; --i)
            {
                AudioSession session = Sessions[i];
                if (!session.Valid || session.State.HasFlag(NAudio.CoreAudioApi.Interfaces.AudioSessionState.AudioSessionStateExpired) || !sessions.Any(s => s.PID.Equals(session.PID)))
                    Sessions.RemoveAt(i);
            }

            // add all sessions that aren't already in the current list
            Sessions.AddRange(sessions.Where(s => !Sessions.Any(session => session.PID.Equals(s.PID))));

            if (!LockSelectedSession && selPID != null)
                SelectedSession = Sessions.FirstOrDefault(s => s.PID.Equals(selPID));

            NotifyProcessListRefresh(EventArgs.Empty);
            NotifyPropertyChanged(nameof(Sessions));

            Log.Debug("Refreshed the session list.");
        }

        #endregion ReloadSessions
        #region Session
        /// <summary>Gets a session from <see cref="Sessions"/> by applying <paramref name="predicate"/> to each element and returning the first occurrence.</summary>
        /// <param name="predicate">A predicate function to apply to each element of <see cref="Sessions"/> that can accept <see cref="AudioSession"/> types.</param>
        /// <returns><see cref="ISession"/> if a session was found, or null if <paramref name="predicate"/> didn't return true for any elements.</returns>
        public ISession? FindSession(Predicate<AudioSession> predicate)
        {
            foreach (var session in Sessions)
                if (predicate(session))
                    return session;
            return null;
        }
        /// <summary>Gets a session from <see cref="Sessions"/> by searching for a session with the process id <paramref name="pid"/></summary>
        /// <remarks>It is recommended to use <see cref="FindSessionWithIdentifier(string, StringComparison)"/> when searching the <see cref="Sessions"/> list.</remarks>
        /// <param name="pid"><b><see cref="AudioSession.PID"/></b></param>
        /// <returns><see cref="ISession"/> if a session was found, or null if no processes were found with <paramref name="pid"/>.</returns>
        public ISession? FindSessionWithID(int pid) => FindSession(s => s.PID.Equals(pid));
        /// <summary>Gets a session from <see cref="Sessions"/> by searching for a session with the process name <paramref name="name"/></summary>
        /// <remarks>It is recommended to use <see cref="FindSessionWithIdentifier(string, StringComparison)"/> when searching the <see cref="Sessions"/> list.</remarks>
        /// <param name="name"><see cref="AudioSession.ProcessName"/></param>
        /// <param name="sCompareType">A <see cref="StringComparison"/> enum value to use when matching process names.</param>
        /// <returns><see cref="ISession"/> if a session was found, or null if no processes were found named <paramref name="name"/> using <paramref name="sCompareType"/> string comparison.</returns>
        public ISession? FindSessionWithName(string name, StringComparison sCompareType = StringComparison.OrdinalIgnoreCase) => FindSession(s => s.ProcessName.Equals(name, sCompareType));
        /// <summary>Gets a session from <see cref="Sessions"/> by parsing <paramref name="identifier"/> to determine whether to pass it to <see cref="FindSessionWithID(int)"/>, <see cref="FindSessionWithName(string, StringComparison)"/>, or directly comparing it to the <see cref="AudioSession.ProcessIdentifier"/> property.</summary>
        /// <param name="identifier">
        /// This can match any of the following properties:<br/>
        /// <list type="bullet">
        /// <item><description><b><see cref="AudioSession.PID"/></b></description></item>
        /// <item><term><b><see cref="AudioSession.ProcessName"/></b></term><description>Uses <paramref name="sCompareType"/></description></item>
        /// <item><term><b><see cref="AudioSession.ProcessIdentifier"/></b></term><description>Uses <paramref name="sCompareType"/></description></item>
        /// </list>
        /// </param>
        /// <param name="sCompareType">A <see cref="StringComparison"/> enum value to use when matching process names or full identifiers.</param>
        /// <returns><see cref="ISession"/> if a session was found.<br/>Returns null if nothing was found.</returns>
        public ISession? FindSessionWithIdentifier(string identifier, bool requireMatchingPID = true, StringComparison sCompareType = StringComparison.OrdinalIgnoreCase)
        {
            /* TEST INPUTS:
             * 
             * 10:Process                   => 10:Process
             * 11:PRocessWithOldID          => 10:Process
             * 11:processwitholdid          => 10:Process
             * 10:pRoCeSs                   => 10:Process
             * 999:                         => 999:ProcessNine
             * :Processnine                 => 999:ProcessNine
             * ProcessNine                  => 999:ProcessNine
             */

            if (identifier.Length == 0)
                return null;

            var (pid, name) = AudioSession.ParseProcessIdentifier(identifier);

            List<ISession> potentialMatches = requireMatchingPID ? null! : new();

            for (int i = 0; i < Sessions.Count - 1; ++i)
            {
                var session = Sessions[i];
                if (session.ProcessIdentifier.Equals(identifier, sCompareType) || session.PID.Equals(pid))
                    return session;
                else if (!requireMatchingPID && session.ProcessName.Equals(name, sCompareType))
                    potentialMatches.Add(session);
            }

            if (requireMatchingPID)
                return null;
            return potentialMatches.FirstOrDefault((ISession?)null);
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
        public enum SessionIdentifierFormat
        {
            /// <summary>
            /// Just the Process ID number of the session.
            /// </summary>
            PID,
            /// <summary>
            /// Just the Process name of the session.
            /// </summary>
            Name,
            /// <summary>
            /// Both the PID and the Process Name of the session, seperated with a colon <b>:</b>
            /// </summary>
            Both,
        }
        /// <summary>
        /// Gets a list of strings representing each of the sessions in <see cref="Sessions"/>.
        /// </summary>
        /// <param name="fmt">This corresponds to the target property in <see cref="Sessions"/></param>
        /// <returns>List of strings for autocomplete.</returns>
        public List<string> GetSessionIdentifiers(SessionIdentifierFormat fmt = SessionIdentifierFormat.Both) => fmt switch
        {
            SessionIdentifierFormat.PID => Sessions.Select(s => s.PID.ToString()).ToList(),
            SessionIdentifierFormat.Name => Sessions.Select(s => s.ProcessName).ToList(),
            _ => Sessions.Select(s => s.ProcessIdentifier).ToList(),
        };
        #endregion Session

        #region Selection
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
            if (ReloadOnHotkey && _allowReloadOnHotkey)
            {  // reload on hotkey
                _allowReloadOnHotkey = false;
                ReloadOnHotkeyTimer.Start();
                ReloadSessionList();
            }

            // if the selected session is locked, return
            if (LockSelectedSession) return;

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

            NotifySessionSwitch(); //< SelectedSessionSwitched
        }
        /// <summary>
        /// Sets <see cref="SelectedSession"/> to the session occurring before this one in <see cref="Sessions"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.
        /// </summary>
        /// <remarks>If <see cref="SelectedSession"/> is set to <see cref="NullSession"/>, the last element in <see cref="Sessions"/> is selected.</remarks>
        public void SelectPreviousSession()
        {
            if (ReloadOnHotkey && _allowReloadOnHotkey)
            { // reload on hotkey
                _allowReloadOnHotkey = false;
                ReloadOnHotkeyTimer.Start();
                ReloadSessionList();
            }

            // if the selected session is locked, return
            if (LockSelectedSession) return;

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

            NotifySessionSwitch(); //< SelectedSessionSwitched
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the device occurring after this one in <see cref="Devices"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.
        /// </summary>
        /// <remarks>If <see cref="SelectedDevice"/> is set to <see cref="NullDevice"/>, the first element in <see cref="Devices"/> is selected.</remarks>
        public void SelectNextDevice()
        {
            if (ReloadOnHotkey && _allowReloadOnHotkey)
            { // reload on hotkey
                _allowReloadOnHotkey = false;
                ReloadOnHotkeyTimer.Start();
                ReloadDeviceList();
            }

            // if the selected device is locked, return
            if (LockSelectedDevice) return;

            if (SelectedDevice is AudioDevice device)
            {
                int index = Devices.IndexOf(device);
                if (index == -1 || (index += 1) >= Devices.Count)
                    index = 0;
                SelectedDevice = Devices[index];
            }
            else if (Devices.Count > 0)
                SelectedDevice = Devices[0];

            NotifyDeviceSwitch(); //< SelectedDeviceSwitched
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the device occurring before this one in <see cref="Devices"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.
        /// </summary>
        /// <remarks>If <see cref="SelectedDevice"/> is set to <see cref="NullDevice"/>, the last element in <see cref="Devices"/> is selected.</remarks>
        public void SelectPreviousDevice()
        {
            if (ReloadOnHotkey && _allowReloadOnHotkey)
            { // reload on hotkey
                _allowReloadOnHotkey = false;
                ReloadOnHotkeyTimer.Start();
                ReloadDeviceList();
            }

            // if the selected device is locked, return
            if (LockSelectedDevice) return;

            if (SelectedDevice is AudioDevice device)
            {
                int index = Devices.IndexOf(device);
                if (index == -1 || (index -= 1) < 0)
                    index = Devices.Count - 1;
                SelectedDevice = Devices[index];
            }
            else if (Devices.Count > 0)
                SelectedDevice = Devices[^1];

            NotifyDeviceSwitch(); //< SelectedDeviceSwitched
        }
        #endregion Selection

        #region Other
        /// <summary>Clamps the given value between <paramref name="min"/> and <paramref name="max"/>.</summary>
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
        /// <inheritdoc/>
        public void Dispose()
        {
            SaveSettings();
            ReloadTimer.Dispose();

            for (int i = Sessions.Count - 1; i >= 0; --i)
            {
                Sessions[i].Dispose();
            }
            for (int i = Devices.Count - 1; i >= 0; --i)
            {
                Devices[i].Dispose();
            }

            GC.SuppressFinalize(this);
        }
        #endregion Other
        #endregion Methods
    }
}
