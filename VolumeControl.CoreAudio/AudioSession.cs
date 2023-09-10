using CoreAudio;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using VolumeControl.Core.Helpers;
using VolumeControl.CoreAudio.Events;
using VolumeControl.CoreAudio.Helpers;
using VolumeControl.CoreAudio.Interfaces;
using VolumeControl.Log;

namespace VolumeControl.CoreAudio
{
    /// <summary>
    /// A single audio session running on an audio device.
    /// </summary>
    public class AudioSession : IAudioControl, IReadOnlyAudioControl, IHideableAudioControl, IAudioPeakMeter, INotifyPropertyChanged, IDisposable
    {
        #region Constructor
        internal AudioSession(AudioDevice owningDevice, AudioSessionControl2 audioSessionControl2)
        {
            AudioDevice = owningDevice;
            AudioSessionControl = audioSessionControl2;

            PID = AudioSessionControl.ProcessID;
            ProcessName = Process?.ProcessName ?? string.Empty;
            Name = AudioSessionControl.DisplayName.Length > 0 && !AudioSessionControl.DisplayName.StartsWith('@') ? AudioSessionControl.DisplayName : ProcessName;
            ProcessIdentifier = $"{PID}:{ProcessName}";

            if (AudioSessionControl.SimpleAudioVolume is null)
                throw new NullReferenceException($"{nameof(AudioSession)} '{ProcessName}' ({PID}) {nameof(AudioSessionControl2.SimpleAudioVolume)} is null!");
            if (AudioSessionControl.AudioMeterInformation is null)
                throw new NullReferenceException($"{nameof(AudioSession)} '{ProcessName}' ({PID}) {nameof(AudioSessionControl2.AudioMeterInformation)} is null!");

            AudioSessionControl.OnDisplayNameChanged += this.AudioSessionControl_OnDisplayNameChanged;
            AudioSessionControl.OnIconPathChanged += this.AudioSessionControl_OnIconPathChanged;
            AudioSessionControl.OnSessionDisconnected += this.AudioSessionControl_OnSessionDisconnected;
            AudioSessionControl.OnSimpleVolumeChanged += this.AudioSessionControl_OnSimpleVolumeChanged;
            AudioSessionControl.OnStateChanged += this.AudioSessionControl_OnStateChanged;
        }
        #endregion Constructor

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        /// <summary>
        /// Occurs when this <see cref="AudioSession"/> instance has been disconnected.
        /// </summary>
        public event AudioSessionControl2.SessionDisconnectedDelegate? SessionDisconnected;
        private void NotifySessionDisconnected(AudioSessionDisconnectReason disconnectReason) => SessionDisconnected?.Invoke(this, disconnectReason);
        /// <summary>
        /// Occurs when the state of this <see cref="AudioSession"/> instance was changed.
        /// </summary>
        public event EventHandler<AudioSessionState>? StateChanged;
        private void NotifyStateChanged(AudioSessionState newState) => StateChanged?.Invoke(this, newState);
        /// <summary>
        /// Occurs when the display name of this <see cref="AudioSession"/> instance has changed.
        /// </summary>
        public event EventHandler<string>? DisplayNameChanged;
        private void NotifyDisplayNameChanged(string newDisplayName) => DisplayNameChanged?.Invoke(this, newDisplayName);
        /// <summary>
        /// Occurs when the display icon path for this <see cref="AudioSession"/> instance has changed.
        /// </summary>
        public event EventHandler<string>? IconPathChanged;
        private void NotifyIconPathChanged(string newIconPath) => IconPathChanged?.Invoke(this, newIconPath);
        /// <summary>
        /// Occurs when the volume level or mute state of this <see cref="AudioSession"/> has changed.
        /// </summary>
        public event VolumeChangedEventHandler? VolumeChanged;
        private void NotifyVolumeChanged(float newVolume, bool newMute) => VolumeChanged?.Invoke(this, new(newVolume, newMute));
        #endregion Events

        #region Fields
        /// <summary>
        /// Used to prevent duplicate <see cref="PropertyChanged"/> events from being fired.
        /// </summary>
        private bool isNotifying = false;
        #endregion Fields

        #region Properties
        private static LogWriter Log => FLog.Log;
        /// <summary>
        /// Gets the <see cref="CoreAudio.AudioDevice"/> that this <see cref="AudioSession"/> instance is running on.
        /// </summary>
        public AudioDevice AudioDevice { get; }
        /// <summary>
        /// Gets the <see cref="AudioSessionControl2"/> controller instance associated with this <see cref="AudioSession"/> instance.
        /// </summary>
        public AudioSessionControl2 AudioSessionControl { get; }
        internal SimpleAudioVolume SimpleAudioVolume => AudioSessionControl.SimpleAudioVolume!; //< constructor throws if null
        internal AudioMeterInformation AudioMeterInformation => AudioSessionControl.AudioMeterInformation!; //< constructor throws if null

        /// <summary>
        /// Gets the process ID of the process associated with this <see cref="AudioSession"/> instance.
        /// </summary>
        public uint PID { get; }
        /// <summary>
        /// Gets the Process Name of the process associated with this <see cref="AudioSession"/> instance.
        /// </summary>
        public string ProcessName { get; }
        /// <summary>
        /// Gets or <i>(temporarily)</i> sets the name of this <see cref="AudioSession"/> instance.
        /// </summary>
        /// <remarks>
        /// This is the DisplayName of the <see cref="AudioSessionControl"/> if it isn't empty, otherwise it is <see cref="ProcessName"/>.
        /// </remarks>
        public string Name { get; set; }
        /// <summary>
        /// Gets the <see cref="System.Diagnostics.Process"/> that is controlling this <see cref="AudioSession"/> instance.
        /// </summary>
        public Process? Process => _process ??= GetProcess();
        private Process? _process;
        /// <summary>
        /// Gets the process identifier of this audio session.
        /// </summary>
        /// <remarks>
        /// Process Identifiers are composed of the <see cref="PID"/> and <see cref="ProcessName"/> of a session, separated by a colon.<br/>
        /// Example: "1234:SomeProcess"
        /// </remarks>
        public string ProcessIdentifier { get; }
        public string SessionIdentifier => AudioSessionControl.SessionIdentifier;
        public string SessionInstanceIdentifier => AudioSessionControl.SessionInstanceIdentifier;
        #endregion Properties

        #region IAudioControl Properties
        public float NativeVolume
        {
            get => SimpleAudioVolume.MasterVolume;
            set
            {
                if (value < 0.0f)
                    value = 0.0f;
                else if (value > 1.0f)
                    value = 1.0f;

                SimpleAudioVolume.MasterVolume = value;
                if (isNotifying) return; //< don't duplicate propertychanged notifications
                isNotifying = true;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Volume));
                isNotifying = false;
            }
        }
        public int Volume
        {
            get => VolumeLevelConverter.FromNativeVolume(NativeVolume);
            set
            {
                NativeVolume = VolumeLevelConverter.ToNativeVolume(value);
                if (isNotifying) return; //< don't duplicate propertychanged notifications
                isNotifying = true;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(NativeVolume));
                isNotifying = false;
            }
        }
        public bool Mute
        {
            get => SimpleAudioVolume.Mute;
            set
            {
                SimpleAudioVolume.Mute = value;
                NotifyPropertyChanged();
            }
        }
        #endregion IAudioControl Properties

        #region IVolumePeakMeter Properties
        public float PeakMeterValue => AudioMeterInformation.MasterPeakValue;
        #endregion IVolumePeakMeter Properties

        #region IHideableAudioControl Properties
        public bool IsHidden
        {
            get => (Core.Config.Default as Core.Config)!.HiddenSessionProcessNames.Contains(this.ProcessName);
            set
            {
                if (value)
                { // hide this session
                    if (IsHidden) return; //< already hidden

                    (Core.Config.Default as Core.Config)!.HiddenSessionProcessNames.Add(this.ProcessName);
                }
                else
                { // unhide this session
                    if (!IsHidden) return; //< already not hidden

                    (Core.Config.Default as Core.Config)!.HiddenSessionProcessNames.RemoveAll(pname => pname.Equals(this.ProcessName, StringComparison.Ordinal));
                }
            }
        }
        #endregion IHideableAudioControl Properties

        #region Methods
        /// <summary>
        /// Gets the process that created this audio session.
        /// </summary>
        /// <returns><see cref="System.Diagnostics.Process"/> instance that created this audio session, or <see langword="null"/> if an error occurred.</returns>
        public Process? GetProcess()
        {
            try
            {
                return Process.GetProcessById(Convert.ToInt32(PID));
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get process with ID '{PID}' because of an exception:", ex);
                return null;
            }
        }
        /// <inheritdoc cref="GetProcess"/>
        /// <param name="exception">When the method returns <see langword="null"/>, this is set to the exception that occurred; otherwise this is <see langword="null"/>.</param>
        public Process? GetProcess(out Exception? exception)
        {
            try
            {
                exception = null;
                return Process.GetProcessById(Convert.ToInt32(PID));
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }
        /// <summary>
        /// Gets a new TargetInfo object representing this AudioSession instance.
        /// </summary>
        /// <returns>A <see cref="TargetInfo"/> struct that represents this <see cref="AudioSession"/> instance.</returns>
        public TargetInfo GetTargetInfo() => new TargetInfo()
        {
            ProcessIdentifier = ProcessIdentifier,
            SessionInstanceIdentifier = SessionInstanceIdentifier,
        };
        /// <summary>
        /// Parses the given <paramref name="processIdentifier"/> to retrieve the ProcessId and ProcessName components.
        /// </summary>
        /// <param name="processIdentifier">A process identifier string.</param>
        /// <returns>A tuple that includes the ProcessId as an <see cref="int"/> and the ProcessName as a <see cref="string"/>.</returns>
        /// <exception cref="FormatException"><paramref name="processIdentifier"/> was not in a valid format.</exception>
        public static (int pid, string pname) ParseProcessIdentifier(string processIdentifier)
        {
            int delim = processIdentifier
                .Trim(':', ' ') //< only allow colon characters that actually have text on both sides.
                .IndexOf(':');  //< get the index of said inner-colons...

            if (delim == -1)
            {
                if (processIdentifier.All(char.IsDigit) && int.TryParse(processIdentifier, out int result))
                    // pid
                    return (result, string.Empty);
                else // process name
                    return (-1, processIdentifier);
            }
            else // both
            {
                return !int.TryParse(processIdentifier[..delim], out int result)
                    ? throw new FormatException($"Invalid process identifier string '{processIdentifier}'")
                    : ((int, string))(result, processIdentifier[(delim + 1)..]);
            }
        }
        #endregion Methods

        #region AudioSessionControl EventHandlers
        /// <summary>
        /// Triggers the <see cref="DisplayNameChanged"/> event.
        /// </summary>
        private void AudioSessionControl_OnDisplayNameChanged(object sender, string newDisplayName)
            => NotifyDisplayNameChanged(newDisplayName);
        /// <summary>
        /// Triggers the <see cref="IconPathChanged"/> event.
        /// </summary>
        private void AudioSessionControl_OnIconPathChanged(object sender, string newIconPath)
            => NotifyIconPathChanged(newIconPath);
        /// <summary>
        /// Triggers the <see cref="SessionDisconnected"/> event.
        /// </summary>
        private void AudioSessionControl_OnSessionDisconnected(object sender, AudioSessionDisconnectReason disconnectReason)
            => NotifySessionDisconnected(disconnectReason);
        /// <summary>
        /// Triggers the <see cref="VolumeChanged"/> event.
        /// </summary>
        private void AudioSessionControl_OnSimpleVolumeChanged(object sender, float newVolume, bool newMute)
        {
            NativeVolume = newVolume;
            Mute = newMute;
            NotifyVolumeChanged(newVolume, newMute);
        }
        /// <summary>
        /// Triggers the <see cref="StateChanged"/> event.
        /// </summary>
        private void AudioSessionControl_OnStateChanged(object sender, AudioSessionState newState)
            => NotifyStateChanged(newState);
        #endregion AudioSessionControl EventHandlers

        #region IDisposable Implementation
        public void Dispose()
        {
            ((IDisposable)this.AudioSessionControl).Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}