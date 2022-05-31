using NAudio.CoreAudioApi;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Audio.Events;
using VolumeControl.Audio.Interfaces;
using VolumeControl.Log;
using VolumeControl.WPF.Collections;
using VolumeControl.TypeExtensions;
using System;

namespace VolumeControl.Audio
{
    /// <summary>
    /// Contains methods related to audio devices, sessions, and the underlying selection mechanics responsible for providing context to actions.<br/>
    /// You can use this object to manipulate any audio session on the system.
    /// </summary>
    public class AudioAPI : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
    {
        #region Initializers
        /// <inheritdoc cref="AudioAPI"/>
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

            ISession? targetSession = FindSessionWithIdentifier(Settings.SelectedSession);
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
                Interval = MathExt.Clamp(Settings.ReloadOnHotkeyMinInterval, 1, 120000),
                Enabled = !_allowReloadOnHotkey, //< this should always start as the inverse of _allowReloadOnHotkey
            };
            ReloadOnHotkeyTimer.Tick += Handle_ReloadOnHotkeyTick!;

            ReloadTimer = new()
            {
                Interval = MathExt.Clamp(Settings.AutoReloadInterval, Settings.AutoReloadIntervalMin, Settings.AutoReloadIntervalMax),
                Enabled = Settings.ReloadOnInterval,
            };
            ReloadTimer.Tick += Handle_ReloadTimerTick!;

            // Add device event handlers
            DeviceNotificationClient = new();
            var enumerator = new MMDeviceEnumerator();
            enumerator.RegisterEndpointNotificationCallback(DeviceNotificationClient);
            enumerator.Dispose();
            DeviceNotificationClient.DefaultDeviceChanged += (s, e) =>
            {
                if (e.Role.Equals(Role.Multimedia)) //< multimedia devices only
                {
                    switch (e.DataFlow)
                    {
                    case DataFlow.All:
                        Log.Warning($"{nameof(DeviceNotificationClient.DefaultDeviceChanged)} event was fired with unexpected arguments! ('{nameof(e.DataFlow)}' is '{e.DataFlow:G}')");
                        break;
                    case DataFlow.Render: // Output
                        _defaultDevice = null;
                        break;
                    case DataFlow.Capture: // Input
                    default: break;
                    }
                }
            };
            DeviceNotificationClient.DeviceAdded += (s, e) =>
            {
                ReloadDeviceList();
            };

            DeviceNotificationClient.DeviceRemoved += (s, e) =>
            {
                if (FindDeviceWithID(e.DeviceID) is AudioDevice dev)
                {
                    dev.ForwardRemoved(s, e);
                    dev.Dispose();
                }
                ReloadDeviceList();
            };
            // Forward state change events to the relevant device
            DeviceNotificationClient.DeviceStateChanged += (s, e) =>
            {
                if (FindDeviceWithID(e.DeviceID) is AudioDevice dev)
                    dev.ForwardStateChanged(s, e); //< notify the device that its state has changed.
                ReloadDeviceList();
            };
            // Forward property change events to the relevant device
            DeviceNotificationClient.DevicePropertyValueChanged += (s, e) =>
            {
                if (FindDeviceWithID(e.DeviceID) is AudioDevice dev)
                {
                    dev.NotifyPropertyChanged(nameof(dev.Properties));
                }
            };
        }
        private void SaveSettings()
        {
            // save settings
            Settings.ReloadOnHotkeyMinInterval = ReloadOnHotkeyTimer.Interval;
            Settings.ReloadOnHotkey = ReloadOnHotkey;

            Settings.AutoReloadInterval = ReloadInterval;
            Settings.ReloadOnInterval = ReloadOnInterval;

            Settings.LockSelectedDevice = LockSelectedDevice;
            Settings.SelectedDevice = SelectedDevice?.DeviceID ?? string.Empty;

            Settings.SelectedSession = SelectedSession?.ProcessIdentifier ?? Target;
            Settings.LockSelectedSession = LockSelectedSession;

            Settings.VolumeStepSize = VolumeStepSize;
            Settings.CheckAllDevices = CheckAllDevices;
            // save to file
            Settings.Save();
            Settings.Reload();
        }
        #endregion Initializers

        #region Fields
        private bool _allowReloadOnHotkey = true;

        private IDevice? _selectedDevice;
        private bool _lockSelectedDevice;

        private ISession? _selectedSession;
        private bool _lockSelectedSession;

        private string _target;
        private int _volumeStepSize;
        private bool _reloadOnHotkey;

        private readonly System.Windows.Forms.Timer ReloadOnHotkeyTimer;
        private readonly System.Windows.Forms.Timer ReloadTimer;
        #endregion Fields

        #region Properties
        private static AudioAPISettings Settings => AudioAPISettings.Default;
        private static LogWriter Log => FLog.Log;
        /// <summary>The current default audio device.<br/>Do not store references to this object within objects whose lifetime may outlast it.</summary>
        public AudioDevice DefaultDevice => _defaultDevice ??= GetDefaultDevice();
        private AudioDevice? _defaultDevice = null;
        /// <summary>
        /// The minimum allowable automatic reload interval, in milliseconds.
        /// </summary>
        public static int ReloadIntervalMin => Settings.AutoReloadIntervalMin;
        /// <summary>
        /// The maximum allowable automatic reload interval, in milliseconds.
        /// </summary>
        public static int ReloadIntervalMax => Settings.AutoReloadIntervalMax;
        /// <summary>
        /// When true, automatic reloads are enabled.
        /// </summary>
        public bool ReloadOnInterval
        {
            get => ReloadTimer.Enabled;
            set
            {
                NotifyPropertyChanging();
                ReloadTimer.Enabled = value;
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc cref="System.Windows.Forms.Timer.Interval"/>
        /// <remarks><b>This property automatically clamps incoming values between <see cref="ReloadIntervalMin"/> and <see cref="ReloadIntervalMax"/></b></remarks>
        public int ReloadInterval
        {
            get => ReloadTimer.Interval;
            set
            {
                NotifyPropertyChanging();
                ReloadTimer.Interval = MathExt.Clamp(value, ReloadIntervalMin, ReloadIntervalMax);
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// When true, the 'Reload on Hotkey' feature is enabled; causing the session list to be refreshed when a relevant hotkey is pressed.<br/>Note that this is only triggered every few seconds, as it uses an internal timer mechanism to prevent spam.
        /// </summary>
        public bool ReloadOnHotkey
        {
            get => _reloadOnHotkey;
            set
            {
                NotifyPropertyChanging();
                _reloadOnHotkey = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// An observable list of all known audio devices.
        /// </summary>
        public ObservableList<AudioDevice> Devices { get; } = new();
        /// <summary>
        /// An observable list of all known audio sessions.
        /// </summary>
        public ObservableList<AudioSession> Sessions { get; } = new();
        /// <summary>
        /// The currently selected <see cref="AudioDevice"/>, or null if nothing is selected.
        /// </summary>
        /// <remarks><see cref="LockSelectedDevice"/> must be false in order to change this.</remarks>
        public IDevice? SelectedDevice
        {
            get => CheckAllDevices ? DefaultDevice : _selectedDevice;
            set
            {
                if (LockSelectedDevice || (value?.DeviceID.Equals(SelectedDevice?.DeviceID, StringComparison.Ordinal) ?? false))
                    return; // don't do anything if locked or we aren't actually changing the value

                NotifyPropertyChanging();

                // clear the session list if we're changing target devices and sessions are device-specific
                if (!CheckAllDevices)
                    Sessions.Clear();

                // change the selected device
                _selectedDevice = value;

                // reload the session list with the new device
                if (!CheckAllDevices)
                    ReloadSessionList();

                // trigger events
                NotifyDeviceSwitch();
                NotifyPropertyChanged();

                Log.Info($"Selected device was changed to '{_selectedDevice?.Name}'");
            }
        }
        /// <summary>
        /// The currently selected <see cref="AudioSession"/>, or null if nothing is selected.
        /// </summary>
        /// <remarks><see cref="LockSelectedSession"/> must be false in order to change this.</remarks>
        public ISession? SelectedSession
        {
            get => _selectedSession;
            set
            {
                if (LockSelectedSession)
                    return;

                NotifyPropertyChanging();

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
                NotifyPropertyChanging();

                _lockSelectedDevice = value;

                NotifyPropertyChanged();
                NotifyLockSelectedDeviceChanged();

                Log.Info($"Selected device was {(value ? "" : "un")}locked");
            }
        }
        /// <summary>
        /// Prevents <see cref="SelectedSession"/> from being modified.
        /// </summary>
        public bool LockSelectedSession
        {
            get => _lockSelectedSession;
            set
            {
                NotifyPropertyChanging();

                _lockSelectedSession = value;

                NotifyPropertyChanged();
                NotifyLockSelectedSessionChanged();

                Log.Info($"Selected session was {(_lockSelectedSession ? "" : "un")}locked");
            }
        }
        /// <summary>
        /// Refers to the text in the target text box on the mixer tab - that is, it is a potentially-unvalidated string representation of <see cref="SelectedSession"/>'s <see cref="AudioSession.ProcessIdentifier"/> property.
        /// </summary>
        /// <remarks>This is automatically updated by, and automatically updates, <see cref="SelectedSession"/>.</remarks>
        public string Target
        {
            get => _target;
            set
            {
                if (LockSelectedSession)
                    return;

                var eventArgs = new TargetChangingEventArgs(_target, value);
                NotifyTargetChanging(ref eventArgs); //< trigger the target changing event first

                if (eventArgs.Cancel)
                    return; // handle change cancelled

                NotifyPropertyChanging();

                value = eventArgs.Incoming; // update value to validated one

                if (value.Length == 0)
                    SelectedSession = null;
                else if (FindSessionWithIdentifier(value) is ISession session)
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
        /// <summary>
        /// The amount to increment or decrement volume when no direct value is provided, such as when triggering methods with hotkeys.
        /// </summary>
        public int VolumeStepSize
        {
            get => _volumeStepSize;
            set
            {
                NotifyPropertyChanging();
                _volumeStepSize = value;
                NotifyPropertyChanged();

                Log.Info($"Volume step set to {_volumeStepSize}");
            }
        }
        /// <inheritdoc cref="DeviceNotificationClient"/>
        /// <remarks>This forwards events directly from the core audio api.</remarks>
        internal DeviceNotificationClient DeviceNotificationClient { get; private set; }
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
        /// Triggered before the <see cref="Target"/> property is changed, allowing for target validation.
        /// </summary>
        /// <remarks>Note that this is triggered <b>before</b> the <see cref="PropertyChanging"/> event for <see cref="Target"/>.<br/>If this event cancels the change, the <see cref="PropertyChanging"/> event is not fired.</remarks>
        public event TargetChangingEventHandler? TargetChanging;
        /// <summary>
        /// Triggered when the <see cref="Target"/> property is changed.
        /// </summary>
        public event TargetChangedEventHandler? TargetChanged;
        /// <summary>
        /// Triggered when the volume level or mute state of <see cref="SelectedDevice"/> or <see cref="SelectedSession"/> is changed.<br/>
        /// To differentiate between devices and sessions, see the <see cref="VolumeEventArgs.Target"/> property.
        /// </summary>
        /// <remarks>Note that this event is only triggered by <see cref="AudioAPI"/> methods, and as a result <b>does not trigger when the volume is changed using the UI or by any other means!</b><br/>
        /// A non-exhaustive list of methods that trigger this event:<list type="bullet">
        /// <item><description><see cref="IncrementDeviceVolume(int)"/></description></item>
        /// <item><description><see cref="DecrementDeviceVolume(int)"/></description></item>
        /// <item><description><see cref="SetDeviceMute(bool)"/></description></item>
        /// <item><description><see cref="ToggleDeviceMute()"/></description></item>
        /// <item><description><see cref="IncrementSessionVolume(int)"/></description></item>
        /// <item><description><see cref="DecrementSessionVolume(int)"/></description></item>
        /// <item><description><see cref="SetDeviceMute(bool)"/></description></item>
        /// <item><description><see cref="ToggleDeviceMute()"/></description></item>
        /// </list>
        /// </remarks>
        public event VolumeEventHandler? VolumeChanged;
        /// <summary>
        /// Triggered when a member property's value is changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Triggered before a member property's value is changed.
        /// </summary>
        public event PropertyChangingEventHandler? PropertyChanging;

        private void NotifyDeviceListRefresh(EventArgs e) => DeviceListReloaded?.Invoke(this, e);
        private void NotifyProcessListRefresh(EventArgs e) => SessionListReloaded?.Invoke(this, e);
        private void NotifyDeviceSwitch() => SelectedDeviceSwitched?.Invoke(this, new());
        private void NotifyLockSelectedDeviceChanged() => LockSelectedDeviceChanged?.Invoke(this, new());
        private void NotifySessionSwitch() => SelectedSessionSwitched?.Invoke(this, new());
        private void NotifyLockSelectedSessionChanged() => LockSelectedSessionChanged?.Invoke(this, new());
        private void NotifyTargetChanging(ref TargetChangingEventArgs e) => TargetChanging?.Invoke(this, e);
        private void NotifyTargetChanged(TargetChangedEventArgs e) => TargetChanged?.Invoke(this, e);
        private void NotifyVolumeChanged(VolumeEventArgs e) => VolumeChanged?.Invoke(this, e);
        private void NotifyVolumeChanged(ITargetable target, int volume, bool muted) => NotifyVolumeChanged(new(target, volume, muted));
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        private void NotifyPropertyChanging([CallerMemberName] string propertyName = "") => PropertyChanging?.Invoke(this, new(propertyName));

        private void Handle_ReloadTimerTick(object sender, EventArgs e) => ReloadSessionList();
        private void Handle_ReloadOnHotkeyTick(object sender, EventArgs e) => _allowReloadOnHotkey = true;
        #endregion Events

        #region Methods
        #region UpdateDevices
        /// <summary>
        /// Performs a selective update on the <see cref="Devices"/> list property by removing old/disabled/unplugged entries and adding newly detected ones.
        /// </summary>
        public void ReloadDeviceList() => SelectiveUpdateDevices(GetAllDevices()); //< Don't reload sessions here!
        private void SelectiveUpdateDevices(List<AudioDevice> devices)
        {
            string? selID = SelectedDevice?.DeviceID;

            // remove all devices that aren't in the new list. (exited/stopped)
            for (int i = Devices.Count - 1; i >= 0; --i)
            {
                if (!devices.Any(d => d.DeviceID.Equals(Devices[i].DeviceID, StringComparison.Ordinal)))
                {
                    Devices[i].Dispose();
                    Devices.RemoveAt(i);
                }
            }

            // add all devices that aren't in the current list. (new)
            Devices.AddRange(devices.Where(dev => !Devices.Any(d => d.DeviceID.Equals(dev.DeviceID, StringComparison.Ordinal))));

            if (selID != null && SelectedDevice == null)
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
        /// <remarks>This method checks <see cref="Devices"/> first and only creates a new device if it wasn't found.</remarks>
        /// <returns><see cref="AudioDevice"/></returns>
        public AudioDevice GetDefaultDevice()
        {
            MMDeviceEnumerator enumerator = new();
            MMDevice? defaultDev = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            string? defaultDevID = defaultDev.ID;
            enumerator.Dispose();

            foreach (AudioDevice? dev in Devices)
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
        /// <returns><see cref="IDevice"/> if successful, or <see langword="null"/> if no matching devices were found.</returns>
        public IDevice? FindDevice(Predicate<AudioDevice> predicate)
        {
            foreach (AudioDevice? device in Devices)
            {
                if (predicate(device))
                    return device;
            }

            return null;
        }
        /// <summary>Calls the <see cref="FindDevice(Predicate{AudioDevice})"/> method with a lambda that compares the <see cref="AudioDevice.DeviceID"/> property.</summary>
        /// <param name="deviceID">The device id string of the target audio device.</param>
        /// <param name="sCompareType">The string comparison type to use.</param>
        /// <returns><see cref="IDevice"/> if successful, or <see langword="null"/> if no matching devices were found.</returns>
        public IDevice? FindDeviceWithID(string deviceID, StringComparison sCompareType = StringComparison.Ordinal) => FindDevice(dev => dev.DeviceID.Equals(deviceID, sCompareType));
        #endregion Device

        #region ReloadSessions
        /// <summary>
        /// Performs a selective update on the <see cref="Sessions"/> list property by removing old/exited entries and adding new/started entries.
        /// </summary>
        /// <param name="reloadDevicesFirst">When true, <see cref="ReloadDeviceList"/> is called before getting the updated sessions from the CoreAudio API.</param>
        public void ReloadSessionList(bool reloadDevicesFirst = false)
        {
            if (reloadDevicesFirst)
                ReloadDeviceList();

            if (Devices.Count == 0 || !CheckAllDevices && SelectedDevice == null)
                return;

            if ((CheckAllDevices ? GetAllSessionsFromAllDevices() : SelectedDevice is AudioDevice dev ? dev.GetAudioSessions() : null!) is List<AudioSession> sessions)
                SelectiveUpdateSessions(sessions);
            else Sessions.Clear();

            ResolveTarget();
        }
        /// <summary>
        /// Performs a selective update of the <see cref="Sessions"/> list.
        /// </summary>
        /// <remarks><b>This method does not use mutexes and is not thread safe!</b><br/>Because of this, it should only be called from within the critical section of a method that <i>does</i> use mutexes, such as <see cref="ReloadSessionList"/>.</remarks>
        /// <param name="sessions">List of sessions to update from.<br/>Any items within <see cref="Sessions"/> that are <b>not</b> present in <paramref name="sessions"/> are removed from <see cref="Sessions"/>.<br/>Any items present in <paramref name="sessions"/> that are <b>not</b> present within <see cref="Sessions"/> are added to <see cref="Sessions"/>.</param>
        private void SelectiveUpdateSessions(List<AudioSession> sessions)
        {
            long? selPID = SelectedSession?.PID;

            for (int i = Sessions.Count - 1; i >= 0; --i)
            {
                AudioSession session = Sessions[i];
                if (!session.IsRunning || !sessions.Any(s => s.PID.Equals(session.PID)))
                    Sessions.RemoveAt(i);
            }

            // add all sessions that aren't already in the current list
            Sessions.AddRange(sessions.Where(s => !Sessions.Any(session => session.PID.Equals(s.PID))));

            if (!LockSelectedSession && selPID != null)
                SelectedSession = Sessions.FirstOrDefault(s => s.PID.Equals(selPID));

            NotifyProcessListRefresh(EventArgs.Empty);
            NotifyPropertyChanged(nameof(Sessions));

            Log.Debug("Refreshed the session list.");

            ResolveTarget();
        }

        #endregion ReloadSessions
        #region Session
        /// <summary>Gets a session from <see cref="Sessions"/> by applying <paramref name="predicate"/> to each element and returning the first occurrence.</summary>
        /// <param name="predicate">A predicate function to apply to each element of <see cref="Sessions"/> that can accept <see cref="AudioSession"/> types.</param>
        /// <returns><see cref="ISession"/> if a session was found, or null if <paramref name="predicate"/> didn't return true for any elements.</returns>
        public ISession? FindSession(Predicate<AudioSession> predicate)
        {
            foreach (AudioSession? session in Sessions)
            {
                if (predicate(session))
                    return session;
            }

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
        public ISession? FindSessionWithIdentifier(string identifier, StringComparison sCompareType = StringComparison.OrdinalIgnoreCase)
        {
            if (identifier.Length == 0)
                return null;

            (int pid, string name) = AudioSession.ParseProcessIdentifier(identifier);

            List<ISession> potentialMatches = new();

            for (int i = 0; i < Sessions.Count - 1; ++i)
            {
                AudioSession? session = Sessions[i];
                if (session.ProcessIdentifier.Equals(identifier, sCompareType) || session.PID.Equals(pid) || session.ProcessName.Equals(name, sCompareType))
                    potentialMatches.Add(session);
            }

            if (potentialMatches.Count == 0)
                return null;
            else if (potentialMatches.Count == 1)
                return potentialMatches[0];

            foreach (ISession? session in potentialMatches)
            {
                if (session.PID.Equals(pid))
                    return session;
            }

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
        /// <summary>Bitfield flag that is used to specify which properties to include for the <see cref="GetSessionNames(SessionNameFormat)"/> method.</summary>
        [Flags]
        public enum SessionNameFormat
        {
            /// <summary>Null</summary>
            None = 0,
            /// <summary>Includes <see cref="AudioSession.PID"/>, as a string.</summary>
            PID = 1,
            /// <summary>Includes <see cref="AudioSession.ProcessName"/></summary>
            ProcessName = 2,
            /// <summary>Includes <see cref="AudioSession.ProcessIdentifier"/></summary>
            ProcessIdentifier = 4,
        }
        /// <summary>
        /// Gets a list of strings containing various properties from each element present in <see cref="Sessions"/>.<br/>
        /// This is implemented as a faster alternative to using LINQ statements as it only loops through the list once.
        /// </summary>
        /// <param name="format">Bitfield flag that determines which properties to include in the list.</param>
        /// <returns>A list of <see cref="string"/> types containing the requested properties from each session.</returns>
        public List<string> GetSessionNames(SessionNameFormat format = SessionNameFormat.ProcessIdentifier | SessionNameFormat.ProcessName)
        {
            List<string> l = new();
            if (format.Equals(SessionNameFormat.None))
                return l;
            for (int i = 0; i < Sessions.Count - 1; ++i)
            {
                AudioSession? session = Sessions[i];
                if (format.HasFlag(SessionNameFormat.PID))
                    l.Add(session.PID.ToString());
                if (format.HasFlag(SessionNameFormat.ProcessName))
                    l.Add(session.ProcessName);
                if (format.HasFlag(SessionNameFormat.ProcessIdentifier))
                    l.Add(session.ProcessIdentifier);
            }
            return l;
        }
        /// <summary>
        /// Attempts to resolve the <see cref="SelectedSession"/> using the current <see cref="Target"/> string.<br/>
        /// This doesn't do anything unless SelectedSession is null and Target is not empty.
        /// </summary>
        /// <remarks>It is recommended to call this function first inside of methods intended to be called using hotkeys.</remarks>
        private void ResolveTarget()
        {
            if (SelectedSession == null && Target.Length > 0)
            {
                if (FindSessionWithIdentifier(Target) is ISession target)
                {
                    SelectedSession = target;
                    Log.Debug($"Successfully resolved selected session '{target.ProcessIdentifier}' from string '{Target}'.");
                }
            }
        }
        #endregion Session

        #region Selection
        #region SessionSelection
        /// <summary>Gets the current session volume of <see cref="SelectedSession"/>.</summary>
        /// <returns>An <see cref="Int32"/> in the range ( 0 - 100 ) if <see cref="SelectedSession"/> is not <see langword="null"/>; otherwise ( -1 ).</returns>
        public int GetSessionVolume() => SelectedSession?.Volume ?? -1;
        /// <summary>Sets the current session volume of <see cref="SelectedSession"/>.</summary>
        /// <param name="volume">The desired session volume level in the range ( 0 - 100 )</param>
        public void SetSessionVolume(int volume)
        {
            if (SelectedSession is AudioSession session)
            {
                session.Volume = volume;
            }
        }
        /// <summary>
        /// Increments the volume of <see cref="SelectedSession"/> by <paramref name="amount"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        /// <param name="amount">The amount to change the session's volume by.<br/>Session volume can be any value from 0 to 100, and is <b>automatically</b> clamped if the final value exceeds this range.</param>
        public void IncrementSessionVolume(int amount)
        {
            ResolveTarget();
            if (SelectedSession is AudioSession session)
            {
                session.Volume += amount;
                NotifyVolumeChanged(session, session.Volume, session.Muted);
            }
        }
        /// <summary>
        /// Increments the volume of <see cref="SelectedSession"/> by <see cref="VolumeStepSize"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        public void IncrementSessionVolume() => IncrementSessionVolume(VolumeStepSize);
        /// <summary>
        /// Decrements the volume of <see cref="SelectedSession"/> by <paramref name="amount"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        /// <param name="amount">The amount to change the session's volume by.<br/>Session volume can be any value from 0 to 100, and is <b>automatically</b> clamped if the final value exceeds this range.</param>
        public void DecrementSessionVolume(int amount)
        {
            ResolveTarget();
            if (SelectedSession is AudioSession session)
            {
                session.Volume -= amount;
                NotifyVolumeChanged(session, session.Volume, session.Muted);
            }
        }
        /// <summary>
        /// Decrements the volume of <see cref="SelectedSession"/> by <see cref="VolumeStepSize"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        public void DecrementSessionVolume() => DecrementSessionVolume(VolumeStepSize);
        /// <summary>
        /// Gets whether the <see cref="SelectedSession"/> is currently muted.
        /// </summary>
        /// <returns>True if <see cref="SelectedSession"/> is not null and is muted; otherwise false.</returns>
        public bool GetSessionMute() => SelectedSession is AudioSession session && session.Muted;
        /// <summary>
        /// Sets the mute state of <see cref="SelectedSession"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        /// <param name="state">When true, the session will be muted; when false, the session will be unmuted.</param>
        public void SetSessionMute(bool state)
        {
            ResolveTarget();
            if (SelectedSession is AudioSession session)
            {
                session.Muted = state;
                NotifyVolumeChanged(session, session.Volume, session.Muted);
            }
        }
        /// <summary>
        /// Toggles the mute state of <see cref="SelectedSession"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        public void ToggleSessionMute()
        {
            ResolveTarget();
            if (SelectedSession is AudioSession session)
            {
                session.Muted = !session.Muted;
                NotifyVolumeChanged(session, session.Volume, session.Muted);
            }
        }

        /// <summary>
        /// Sets <see cref="SelectedSession"/> to the session occurring after this one in <see cref="Sessions"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.
        /// </summary>
        /// <remarks>If <see cref="SelectedSession"/> is set to null, the first element in <see cref="Sessions"/> is selected.</remarks>
        public void SelectNextSession()
        {
            ResolveTarget();
            if (ReloadOnHotkey && _allowReloadOnHotkey)
            {  // reload on hotkey
                _allowReloadOnHotkey = false;
                ReloadOnHotkeyTimer.Start();
                ReloadSessionList();
            }

            // if the selected session is locked, return
            if (LockSelectedSession) return;

            if (Sessions.Count == 0)
            {
                if (SelectedSession == null) return;
                SelectedSession = null;
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
            {
                SelectedSession = Sessions[0];
            }

            NotifySessionSwitch(); //< SelectedSessionSwitched
        }
        /// <summary>
        /// Sets <see cref="SelectedSession"/> to the session occurring before this one in <see cref="Sessions"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.
        /// </summary>
        /// <remarks>If <see cref="SelectedSession"/> is set to null, the last element in <see cref="Sessions"/> is selected.</remarks>
        public void SelectPreviousSession()
        {
            ResolveTarget();
            if (ReloadOnHotkey && _allowReloadOnHotkey)
            { // reload on hotkey
                _allowReloadOnHotkey = false;
                ReloadOnHotkeyTimer.Start();
                ReloadSessionList();
            }

            // if the selected session is locked, return
            if (LockSelectedSession) return;

            if (Sessions.Count == 0)
            {
                if (SelectedSession == null) return;
                SelectedSession = null;
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
            {
                SelectedSession = Sessions[^1];
            }

            NotifySessionSwitch(); //< SelectedSessionSwitched
        }
        /// <summary>
        /// Sets <see cref="SelectedSession"/> to <see langword="null"/>.<br/>
        /// Does nothing if <see cref="SelectedSession"/> is already null or if the selected session is locked.
        /// </summary>
        /// <returns><see langword="true"/> when <see cref="SelectedSession"/> was set to <see langword="null"/> &amp; the <see cref="SelectedSessionSwitched"/> event was fired; otherwise <see langword="false"/>.</returns>
        public bool DeselectSession()
        {
            if (LockSelectedSession || SelectedSession == null)
                return false;

            SelectedSession = null;
            NotifySessionSwitch(); //< SelectedSessionSwitched
            return true;
        }
        #endregion SessionSelection
        #region DeviceSelection
        /// <summary>Gets the current endpoint volume of <see cref="SelectedDevice"/>.</summary>
        /// <returns>An <see cref="Int32"/> in the range ( 0 - 100 ) if <see cref="SelectedDevice"/> is not <see langword="null"/>; otherwise ( -1 ).</returns>
        public int GetDeviceVolume() => SelectedDevice?.EndpointVolume ?? -1;
        /// <summary>Sets the current endpoint volume of <see cref="SelectedDevice"/>.</summary>
        /// <param name="volume">The desired endpoint volume level in the range ( 0 - 100 )</param>
        public void SetDeviceVolume(int volume)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.EndpointVolume = volume;
            }
        }
        /// <summary>
        /// Increments the endpoint volume of the currently selected device.<br/>
        /// This affects the maximum volume of all sessions using this endpoint.
        /// </summary>
        /// <param name="amount">The amount to increase the volume by.</param>
        public void IncrementDeviceVolume(int amount)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.EndpointVolume += amount;
                NotifyVolumeChanged(dev, dev.EndpointVolume, dev.EndpointMuted);
            }
        }
        /// <remarks>This calls <see cref="IncrementDeviceVolume(int)"/> using <see cref="VolumeStepSize"/>.</remarks>
        /// <inheritdoc cref="IncrementDeviceVolume(int)"/>
        public void IncrementDeviceVolume() => IncrementDeviceVolume(VolumeStepSize);
        /// <summary>
        /// Decrements the endpoint volume of the currently selected device.<br/>
        /// This affects the maximum volume of all sessions using this endpoint.
        /// </summary>
        /// <param name="amount">The amount to decrease the volume by.</param>
        public void DecrementDeviceVolume(int amount)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.EndpointVolume -= amount;
                NotifyVolumeChanged(dev, dev.EndpointVolume, dev.EndpointMuted);
            }
        }
        /// <remarks>This calls <see cref="DecrementDeviceVolume(int)"/> using <see cref="VolumeStepSize"/>.</remarks>
        /// <inheritdoc cref="DecrementDeviceVolume(int)"/>
        public void DecrementDeviceVolume() => DecrementDeviceVolume(VolumeStepSize);
        /// <summary>
        /// Gets whether the <see cref="SelectedDevice"/> is currently muted.
        /// </summary>
        /// <returns>True if <see cref="SelectedDevice"/> is not null and is muted; otherwise false.</returns>
        public bool GetDeviceMute() => SelectedDevice?.EndpointMuted ?? false;
        /// <summary>
        /// Sets the mute state of <see cref="SelectedDevice"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        /// <remarks>Note that this affects all sessions using this device.</remarks>
        /// <param name="state">When true, the device will be muted; when false, the device will be unmuted.</param>
        public void SetDeviceMute(bool state)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.EndpointMuted = state;
                NotifyVolumeChanged(dev, dev.EndpointVolume, dev.EndpointMuted);
            }
        }
        /// <summary>
        /// Toggles the mute state of <see cref="SelectedDevice"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        /// <remarks>Note that this affects all sessions using this device.</remarks>
        public void ToggleDeviceMute()
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.EndpointMuted = !dev.EndpointMuted;
                NotifyVolumeChanged(dev, dev.EndpointVolume, dev.EndpointMuted);
            }
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the device occurring after this one in <see cref="Devices"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.<br/>
        /// Does nothing unless device selection is unlocked.
        /// </summary>
        /// <remarks>If <see cref="SelectedDevice"/> is set to null, the first element in <see cref="Devices"/> is selected.</remarks>
        public void SelectNextDevice()
        {
            if (ReloadOnHotkey && _allowReloadOnHotkey)
            { // reload on hotkey
                _allowReloadOnHotkey = false;
                ReloadOnHotkeyTimer.Start();
                ReloadDeviceList();
            }

            // if the selected device is locked, return
            if (LockSelectedDevice)
                return;

            if (Devices.Count == 0)
            {
                if (SelectedDevice == null) return;
                SelectedDevice = null;
            }
            else if (SelectedDevice is AudioDevice device)
            {
                int index = Devices.IndexOf(device);
                if (index == -1 || (index += 1) >= Devices.Count)
                    index = 0;
                SelectedDevice = Devices[index];
            }
            else SelectedDevice = Devices[0];

            NotifyDeviceSwitch(); //< SelectedDeviceSwitched
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the device occurring before this one in <see cref="Devices"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.<br/>
        /// Does nothing unless device selection is unlocked.
        /// </summary>
        /// <remarks>If <see cref="SelectedDevice"/> is set to null, the last element in <see cref="Devices"/> is selected.</remarks>
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

            if (Devices.Count == 0)
            {
                if (SelectedDevice == null) return;
                SelectedDevice = null;
            }
            else if (SelectedDevice is AudioDevice device)
            {
                int index = Devices.IndexOf(device);
                if (index == -1 || (index -= 1) < 0)
                    index = Devices.Count - 1;
                SelectedDevice = Devices[index];
            }
            else SelectedDevice = Devices[^1];

            NotifyDeviceSwitch(); //< SelectedDeviceSwitched
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to <see langword="null"/>.<br/>
        /// This does nothing if 
        /// <br/>Does nothing if <see cref="SelectedDevice"/> is already null, or if the selected device is locked.
        /// </summary>
        /// <returns><see langword="true"/> when the selected device was set to null &amp; the <see cref="SelectedDeviceSwitched"/> event was fired; otherwise <see langword="false"/>.</returns>
        public bool DeselectDevice()
        {
            if (SelectedDevice == null || LockSelectedDevice)
                return false;

            SelectedDevice = null;
            NotifyDeviceSwitch();
            return true;
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the result of <see cref="GetDefaultDevice()"/>.<br/>
        /// Does nothing if the selected device is locked, or if the selected device is already set to the default device.
        /// </summary>
        /// <returns><see langword="true"/> when <see cref="SelectedDevice"/> was changed &amp; the <see cref="SelectedDeviceSwitched"/> event was fired; otherwise <see langword="false"/>.</returns>
        public bool SelectDefaultDevice()
        {
            if (LockSelectedDevice)
                return false;
            else if (GetDefaultDevice() is AudioDevice device && SelectedDevice != device)
            {
                SelectedDevice = device;
                NotifyDeviceSwitch();
                return true;
            }
            return false;
        }
        #endregion DeviceSelection
        #endregion Selection

        #region Other
        /// <inheritdoc/>
        public void Dispose()
        {
            var enumerator = new MMDeviceEnumerator();
            enumerator.UnregisterEndpointNotificationCallback(DeviceNotificationClient);
            enumerator.Dispose();
            DeviceNotificationClient = null!;

            SaveSettings();
            ReloadTimer.Dispose();
            ReloadOnHotkeyTimer.Dispose();

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
