using NAudio.CoreAudioApi;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Audio.Collections;
using VolumeControl.Audio.Events;
using VolumeControl.Audio.Extensions;
using VolumeControl.Audio.Interfaces;
using VolumeControl.Core;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

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
            this.Devices = new(this, DataFlow.Render);
            this.Sessions = new(this.Devices);

            this.EnableDevices();

            // resolve previous target 
            ISession? targetSession = null;
            if (this.FindSessionWithSessionInstanceIdentifier(Settings.Target.SessionInstanceIdentifier) is ISession session1)
                targetSession = session1; //< first check for an identical instance identifier (if the other process hasn't restarted)
            else if (this.FindSessionWithIdentifier(Settings.Target.ProcessIdentifier) is ISession session2)
                targetSession = session2; //< then check for a matching process identifier (pid if available; otherwise pname (process name))
            if (targetSession is not null)
            {
                bool lockState = this.LockSelectedSession;
                if (lockState) this.LockSelectedSession = false;
                this.SelectedSession = targetSession;
                if (lockState) this.LockSelectedSession = true;
            }
        }
        private void SaveSettings()
        {
            // save settings
            this.SaveEnabledDevices();

            // save to file
            Settings.Save();
        }
        #endregion Initializers

        #region Properties
        private static Config Settings => (Config.Default as Config)!;
        private static LogWriter Log => FLog.Log;
        /// <summary>
        /// An observable list of all known audio devices.
        /// </summary>
        public AudioDeviceCollection Devices { get; }
        /// <inheritdoc cref="AudioDeviceCollection.Default"/>
        public AudioDevice? DefaultDevice => this.Devices.Default;
        /// <summary>
        /// Prevents <see cref="SelectedSession"/> from being modified.
        /// </summary>
        public bool LockSelectedSession
        {
            get => Settings.LockTargetSession;
            set
            {
                this.NotifyPropertyChanging();

                Settings.LockTargetSession = value;

                this.NotifyPropertyChanged();
                this.NotifyLockSelectedSessionChanged();

                Log.Info($"Session selection {(Settings.LockTargetSession ? "" : "un")}locked.");
            }
        }
        /// <summary>
        /// Refers to the text in the target text box on the mixer tab - that is, it is a potentially-unvalidated string representation of <see cref="SelectedSession"/>'s <see cref="AudioSession.ProcessIdentifier"/> property.
        /// </summary>
        /// <remarks>This is automatically updated by, and automatically updates, <see cref="SelectedSession"/>.</remarks>
        public string Target
        {
            get => Settings.Target.ProcessIdentifier;
            set
            {
                if (this.LockSelectedSession) return;

                var eventArgs = new TargetChangingEventArgs(Settings.Target.ProcessIdentifier, value);
                this.NotifyTargetChanging(ref eventArgs); //< trigger the target changing event first

                if (eventArgs.Cancel) return;

                this.NotifyPropertyChanging();
                this.NotifyPropertyChanging(nameof(this.SelectedSession));
                this.NotifyPropertyChanging(nameof(this.TargetGuid));

                Settings.Target = this.FindSessionWithIdentifier(eventArgs.Incoming) is ISession session
                    ? session.GetTargetInfo()
                    : new Config.TargetInfo() { ProcessIdentifier = eventArgs.Incoming, SessionInstanceIdentifier = string.Empty };

                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged(nameof(this.SelectedSession));
                this.NotifyPropertyChanged(nameof(this.TargetGuid));

                this.NotifyTargetChanged(Settings.Target.ProcessIdentifier);
            }
        }
        /// <summary>
        /// The target session's <see cref="ISession.SessionInstanceIdentifier"/> GUID.
        /// </summary>
        public string TargetGuid
        {
            get => Settings.Target.SessionInstanceIdentifier;
            set
            {
                if (this.LockSelectedSession) return;

                this.NotifyPropertyChanging();
                this.NotifyPropertyChanging(nameof(this.Target));
                this.NotifyPropertyChanging(nameof(this.SelectedSession));

                Settings.Target = !value.Equals(Guid.Empty) && this.FindSessionWithSessionInstanceIdentifier(value) is ISession session
                    ? session.GetTargetInfo()
                    : Config.TargetInfo.Empty;

                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged(nameof(this.Target));
                this.NotifyPropertyChanged(nameof(this.SelectedSession));

                this.NotifyTargetChanged(Settings.Target.ProcessIdentifier);
            }
        }
        /// <summary>
        /// The amount to increment or decrement volume when no direct value is provided, such as when triggering methods with hotkeys.
        /// </summary>
        public int VolumeStepSize
        {
            get => Settings.VolumeStepSize;
            set
            {
                this.NotifyPropertyChanging();
                Settings.VolumeStepSize = value;
                this.NotifyPropertyChanged();

                Log.Info($"Volume step set to {Settings.VolumeStepSize}");
            }
        }

        /// <summary>
        /// The currently selected <see cref="AudioSession"/>, or null if nothing is selected.
        /// </summary>
        /// <remarks><see cref="LockSelectedSession"/> must be false in order to change this.</remarks>
        public ISession? SelectedSession
        {
            get
            {
                if (this.FindSessionWithSessionInstanceIdentifier(this.TargetGuid) is ISession session)
                {
                    return session;
                }
                else
                {
                    this.ResolveTarget();
                }

                return this.FindSessionWithSessionInstanceIdentifier(this.TargetGuid);
            }
            set
            {
                if (this.LockSelectedSession || this.TargetGuid.Equals(value?.SessionInstanceIdentifier))
                    return;

                this.TargetGuid = value?.SessionInstanceIdentifier ?? string.Empty;

                Log.Info($"Session selected: '{this.SelectedSession?.ProcessIdentifier}' ({this.SelectedSession?.SessionInstanceIdentifier})");
            }
        }
        /// <summary>
        /// The list of sessions shown in the mixer.
        /// </summary>
        public AudioSessionCollection Sessions { get; }
        #endregion Properties

        #region Events
        /// <summary>Triggered when the volume or mute state of the <see cref="SelectedSession"/> is changed.</summary>
        public event EventHandler<(int volume, bool muted)>? SelectedSessionVolumeChanged;
        private void NotifyVolumeChanged((int volume, bool muted) pr) => SelectedSessionVolumeChanged?.Invoke(this.SelectedSession, pr);
        /// <summary>Triggered when the selected session is changed.</summary>
        public event EventHandler? SelectedSessionSwitched;
        private void NotifySessionSwitch() => SelectedSessionSwitched?.Invoke(this, new());
        /// <summary>Triggered when the value of the <see cref="LockSelectedSession"/> property is changed.</summary>
        public event EventHandler? LockSelectedSessionChanged;
        private void NotifyLockSelectedSessionChanged() => LockSelectedSessionChanged?.Invoke(this, new());
        /// <summary>Triggered before the <see cref="Target"/> property is changed, allowing for target validation.</summary>
        /// <remarks>Note that this is triggered <b>before</b> the <see cref="PropertyChanging"/> event for <see cref="Target"/>.<br/>If this event cancels the change, the <see cref="PropertyChanging"/> event is not fired.</remarks>
        public event TargetChangingEventHandler? TargetChanging;
        private void NotifyTargetChanging(ref TargetChangingEventArgs e) => TargetChanging?.Invoke(this, e);
        /// <summary>Triggered when the <see cref="Target"/> property is changed.</summary>
        public event EventHandler<string>? TargetChanged;
        private void NotifyTargetChanged(string target) => TargetChanged?.Invoke(this, target);
        /// <summary>Triggered when a member property's value is changed.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        /// <summary>Triggered before a member property's value is changed.</summary>
        public event PropertyChangingEventHandler? PropertyChanging;
        private void NotifyPropertyChanging([CallerMemberName] string propertyName = "") => PropertyChanging?.Invoke(this, new(propertyName));
        #endregion Events


        #region Methods
        #region Device
        private void EnableDevices()
        {
            foreach (AudioDevice? dev in this.Devices)
            {
                dev.Enabled = dev.Equals(this.DefaultDevice) ? Settings.EnableDefaultDevice : Settings.EnabledDevices.Contains(dev.DeviceID);
            }
        }
        /// <summary>
        /// Updates the <see cref="Config.EnabledDevices"/> property with the device IDs of all currently-enabled <see cref="AudioDevice"/> instances.
        /// </summary>
        public void SaveEnabledDevices()
        {
            foreach (AudioDevice dev in this.Devices)
            {
                if (dev.Enabled)
                {
                    Settings.EnabledDevices.AddIfUnique(dev.DeviceID);
                }
                else
                {
                    _ = Settings.EnabledDevices.Remove(dev.DeviceID);
                }
            }
        }
        /// <summary>
        /// Clears and reloads all audio devices.
        /// </summary>
        public void ForceReloadAudioDevices()
        {
            this.Devices.Reload();
            this.EnableDevices();
        }
        /// <summary>Gets a device from <see cref="Devices"/> using the given <paramref name="predicate"/> function.</summary>
        /// <param name="predicate">A predicate function that accepts <see cref="AudioDevice"/> types.</param>
        /// <returns><see cref="IDevice"/> if successful, or <see langword="null"/> if no matching devices were found.</returns>
        public IDevice? FindDevice(Predicate<AudioDevice> predicate)
        {
            foreach (AudioDevice? device in this.Devices)
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
        public IDevice? FindDeviceWithID(string deviceID, StringComparison sCompareType = StringComparison.Ordinal) => this.FindDevice(dev => dev.DeviceID.Equals(deviceID, sCompareType));
        #endregion Device

        #region ReloadSessions
        /// <summary>Forces the sessions list to be reloaded from the audio device list.</summary>
        /// <remarks><b>Note that this invalidates and disposes of all <see cref="AudioSession"/> objects!<br/>This means any audio session objects you had previously saved a reference to will be deleted.</b></remarks>
        public void ForceReloadSessionList() => this.Sessions.RefreshFromDevices();
        #endregion ReloadSessions
        #region Session        
        /// <summary>Gets a session from <see cref="Sessions"/> by applying <paramref name="predicate"/> to each element and returning the first occurrence.</summary>
        /// <param name="predicate">A predicate function to apply to each element of <see cref="Sessions"/> that can accept <see cref="AudioSession"/> types.</param>
        /// <returns><see cref="ISession"/> if a session was found, or null if <paramref name="predicate"/> didn't return true for any elements.</returns>
        public ISession? FindSession(Predicate<AudioSession> predicate)
        {
            foreach (AudioSession session in this.Sessions)
            {
                if (predicate(session))
                    return session;
            }

            return null;
        }
        /// <summary>Gets a session from <see cref="Sessions"/> with a matching <see cref="ISession.SessionInstanceIdentifier"/> <see cref="Guid"/>.</summary> 
        /// <param name="sessionInstanceIdentifier"><see cref="ISession.SessionInstanceIdentifier"/></param>
        /// <returns><see cref="ISession"/> if a session was found, or null if no sessions were found with <paramref name="sessionInstanceIdentifier"/>.</returns>
        public ISession? FindSessionWithSessionInstanceIdentifier(string sessionInstanceIdentifier) => this.FindSession(s => s.SessionInstanceIdentifier.Equals(sessionInstanceIdentifier));
        /// <summary>Gets all sessions from <see cref="Sessions"/> with a matching <see cref="ISession.SessionIdentifier"/> <see cref="Guid"/>.</summary>
        /// <param name="sessionIdentifier"><see cref="ISession.SessionIdentifier"/></param>
        /// <returns><see cref="ISession"/> array of all matching sessions; or an empty array if none were found.</returns>
        public ISession[] FindSessionsWithSessionIdentifier(string sessionIdentifier) => this.Sessions.Where(s => s.SessionIdentifier.Equals(sessionIdentifier)).ToArray();
        /// <summary>Gets a session from <see cref="Sessions"/> by searching for a session with the process id <paramref name="pid"/></summary>
        /// <param name="pid"><b><see cref="AudioSession.PID"/></b></param>
        /// <returns><see cref="ISession"/> if a session was found, or null if no sessions were found with <paramref name="pid"/>.</returns>
        public ISession? FindSessionWithID(int pid) => this.FindSession(s => s.PID.Equals(pid));
        /// <summary>Gets a session from <see cref="Sessions"/> by searching for a session with the process name <paramref name="name"/></summary>
        /// <param name="name"><see cref="AudioSession.ProcessName"/></param>
        /// <param name="sCompareType">A <see cref="StringComparison"/> enum value to use when matching process names.</param>
        /// <returns><see cref="ISession"/> if a session was found, or null if no sessions were found named <paramref name="name"/> using <paramref name="sCompareType"/> string comparison.</returns>
        public ISession? FindSessionWithName(string name, StringComparison sCompareType = StringComparison.OrdinalIgnoreCase) => this.FindSession(s => s.ProcessName.Equals(name, sCompareType));
        /// <summary>Gets a session from <see cref="Sessions"/> by parsing <paramref name="identifier"/> to determine whether to pass it to <see cref="FindSessionWithID(int)"/>, <see cref="FindSessionWithName(string, StringComparison)"/>, or directly comparing it to the <see cref="AudioSession.ProcessIdentifier"/> property.</summary>
        /// <param name="identifier">
        /// This can match any of the following properties:<br/>
        /// <list type="bullet">
        /// <item><description><b><see cref="AudioSession.PID"/></b></description></item>
        /// <item><term><b><see cref="AudioSession.ProcessName"/></b></term><description>Uses <paramref name="sCompareType"/></description></item>
        /// <item><term><b><see cref="AudioSession.ProcessIdentifier"/></b></term><description>Uses <paramref name="sCompareType"/></description></item>
        /// </list>
        /// </param>
        /// <param name="prioritizePID">When <see langword="true"/> <i>(default)</i>, process IDs are prioritized over process names, which returns more accurate results when multiple sessions have identical names but may take longer.<br/>When <see langword="false"/>, the first process with a matching ID or name is returned.</param>
        /// <param name="sCompareType">A <see cref="StringComparison"/> enum value to use when matching process names or full identifiers.</param>
        /// <returns><see cref="ISession"/> if a session was found.<br/>Returns null if nothing was found.</returns>
        public ISession? FindSessionWithIdentifier(string identifier, bool prioritizePID = true, StringComparison sCompareType = StringComparison.OrdinalIgnoreCase)
        {
            if (identifier.Length == 0)
                return null;

            (int pid, string name) = AudioSession.ParseProcessIdentifier(identifier);

            AudioSession? potentialMatch = null;

            foreach (AudioSession session in this.Sessions)
            {
                if (prioritizePID && pid != -1 && session.PID.Equals(pid))
                {
                    return session;
                }
                else if (session.ProcessName.Equals(name, sCompareType))
                {
                    if (!prioritizePID || pid == -1)
                        return session;
                    else if (potentialMatch == null)
                        potentialMatch = session;
                    else return potentialMatch;
                }
            }

            return potentialMatch;
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
            for (int i = 0; i < this.Sessions.Count - 1; ++i)
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
            if (this.Target.Length > 0)
            {
                (int pid, string pname) = AudioSession.ParseProcessIdentifier(this.Target);

                Log.Warning($"Session '{this.Target}' doesn't exist; searching for near-matches...");

                for (int i = 0; i < this.Sessions.Count; ++i)
                {
                    AudioSession? s = this.Sessions[i];
                    if (pid.Equals(s.PID) || pname.Equals(s.ProcessName))
                    {

                        Settings.Target = s.GetTargetInfo();
                        this.NotifyPropertyChanged(nameof(this.Target));
                        this.NotifyPropertyChanged(nameof(this.TargetGuid));
                        this.NotifyPropertyChanged(nameof(this.SelectedSession));
                        Log.Info($"Resolved target '{pid}:{pname}' to '{this.Target}'");
                        return;
                    }
                }
            }
        }
        #endregion Session

        #region Selection
        /// <summary>Gets the current session volume of <see cref="SelectedSession"/>.</summary>
        /// <returns>An <see cref="int"/> in the range ( 0 - 100 ) if <see cref="SelectedSession"/> is not <see langword="null"/>; otherwise ( -1 ).</returns>
        public int GetSessionVolume() => this.SelectedSession?.Volume ?? -1;
        /// <summary>Sets the current session volume of <see cref="SelectedSession"/>.</summary>
        /// <param name="volume">The desired session volume level in the range ( 0 - 100 )</param>
        public void SetSessionVolume(int volume)
        {
            //this.ResolveTarget();
            if (this.SelectedSession is AudioSession session)
            {
                session.Volume = volume;
                this.NotifyVolumeChanged((session.Volume, session.Muted));
            }
        }
        /// <summary>
        /// Increments the volume of <see cref="SelectedSession"/> by <paramref name="amount"/>.
        /// </summary>
        /// <param name="amount">The amount to change the session's volume by.<br/>Session volume can be any value from 0 to 100, and is <b>automatically</b> clamped if the final value exceeds this range.</param>
        public void IncrementSessionVolume(int amount)
        {
            //this.ResolveTarget();
            if (this.SelectedSession is AudioSession session)
            {
                session.Volume += amount;
                this.NotifyVolumeChanged((session.Volume, session.Muted));
            }
        }
        /// <summary>
        /// Increments the volume of <see cref="SelectedSession"/> by <see cref="VolumeStepSize"/>.
        /// </summary>
        public void IncrementSessionVolume() => this.IncrementSessionVolume(this.VolumeStepSize);
        /// <summary>
        /// Decrements the volume of <see cref="SelectedSession"/> by <paramref name="amount"/>.
        /// </summary>
        /// <param name="amount">The amount to change the session's volume by.<br/>Session volume can be any value from 0 to 100, and is <b>automatically</b> clamped if the final value exceeds this range.</param>
        public void DecrementSessionVolume(int amount)
        {
            //this.ResolveTarget();
            if (this.SelectedSession is AudioSession session)
            {
                session.Volume -= amount;
                this.NotifyVolumeChanged((session.Volume, session.Muted));
            }
        }
        /// <summary>
        /// Decrements the volume of <see cref="SelectedSession"/> by <see cref="VolumeStepSize"/>.
        /// </summary>
        public void DecrementSessionVolume() => this.DecrementSessionVolume(this.VolumeStepSize);
        /// <summary>
        /// Gets whether the <see cref="SelectedSession"/> is currently muted.
        /// </summary>
        /// <returns>True if <see cref="SelectedSession"/> is not null and is muted; otherwise false.</returns>
        public bool GetSessionMute() => this.SelectedSession is AudioSession session && session.Muted;
        /// <summary>
        /// Sets the mute state of <see cref="SelectedSession"/>.
        /// </summary>
        /// <param name="state">When true, the session will be muted; when false, the session will be unmuted.</param>
        /// <returns>The mute state of the selected session, or <see langword="null"/> when there is no selected session.</returns>
        public bool? SetSessionMute(bool state)
        {
            //this.ResolveTarget();
            if (this.SelectedSession is AudioSession session)
            {
                session.Muted = state;
                this.NotifyVolumeChanged((session.Volume, session.Muted));
                return session.Muted;
            }
            return null;
        }
        /// <summary>
        /// Toggles the mute state of <see cref="SelectedSession"/>.
        /// </summary>
        /// <returns><see langword="true"/> when the selected session was muted or <see langword="false"/> when the selected session was unmuted.<br/>Returns <see langword="null"/> when there is no selected session.</returns>
        public bool? ToggleSessionMute()
        {
            //this.ResolveTarget();
            if (this.SelectedSession is AudioSession session)
            {
                session.Muted = !session.Muted;
                this.NotifyVolumeChanged((session.Volume, session.Muted));
                return session.Muted;
            }
            return null;
        }

        /// <summary>
        /// Sets <see cref="SelectedSession"/> to the session occurring after this one in <see cref="Sessions"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.
        /// </summary>
        /// <remarks>If <see cref="SelectedSession"/> is set to null, the first element in <see cref="Sessions"/> is selected.</remarks>
        public void SelectNextSession()
        {
            // if the selected session is locked, return
            if (this.LockSelectedSession) return;

            if (this.Sessions.Count == 0)
            {
                if (this.SelectedSession == null) return;
                this.SelectedSession = null;
            }
            if (this.SelectedSession is AudioSession session)
            { // a valid audio session is selected
                //this.ResolveTarget();
                int index = this.Sessions.IndexOf(session);
                if (index == -1 || (index += 1) >= this.Sessions.Count)
                    index = 0;
                this.SelectedSession = this.Sessions[index];
            }
            // nothing is selected, select the first element in the list
            else if (this.Sessions.Count > 0)
            {
                this.SelectedSession = this.Sessions[0];
            }

            this.NotifySessionSwitch(); //< SelectedSessionSwitched
        }
        /// <summary>
        /// Sets <see cref="SelectedSession"/> to the session occurring before this one in <see cref="Sessions"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.
        /// </summary>
        /// <remarks>If <see cref="SelectedSession"/> is set to null, the last element in <see cref="Sessions"/> is selected.</remarks>
        public void SelectPreviousSession()
        {
            // if the selected session is locked, return
            if (this.LockSelectedSession) return;

            if (this.Sessions.Count == 0)
            {
                if (this.SelectedSession == null) return;
                this.SelectedSession = null;
            }
            if (this.SelectedSession is AudioSession session)
            { // a valid audio session is selected
                //this.ResolveTarget();
                int index = this.Sessions.IndexOf(session);
                if (index == -1 || (index -= 1) < 0)
                    index = this.Sessions.Count - 1;
                this.SelectedSession = this.Sessions[index];
            }
            // nothing is selected, select the last element in the list
            else if (this.Sessions.Count > 0)
            {
                this.SelectedSession = this.Sessions[^1];
            }

            this.NotifySessionSwitch(); //< SelectedSessionSwitched
        }
        /// <summary>
        /// Sets <see cref="SelectedSession"/> to <see langword="null"/>.<br/>
        /// Does nothing if <see cref="SelectedSession"/> is already null or if the selected session is locked.
        /// </summary>
        /// <returns><see langword="true"/> when <see cref="SelectedSession"/> was set to <see langword="null"/> &amp; the <see cref="SelectedSessionSwitched"/> event was fired; otherwise <see langword="false"/>.</returns>
        public bool DeselectSession()
        {
            if (this.LockSelectedSession || this.SelectedSession == null)
                return false;

            this.SelectedSession = null;
            this.NotifySessionSwitch(); //< SelectedSessionSwitched
            return true;
        }
        #endregion Selection

        #region Other
        /// <inheritdoc/>
        public void Dispose()
        {
            this.SaveSettings();

            this.Devices.Dispose();

            for (int i = this.Sessions.Count - 1; i >= 0; --i)
            {
                this.Sessions[i].Dispose();
            }
            for (int i = this.Devices.Count - 1; i >= 0; --i)
            {
                this.Devices[i].Dispose();
            }

            GC.SuppressFinalize(this);
        }
        #endregion Other
        #endregion Methods
    }
}
