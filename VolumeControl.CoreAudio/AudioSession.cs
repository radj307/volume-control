using CoreAudio;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using VolumeControl.Core.Structs;
using VolumeControl.CoreAudio.Events;
using VolumeControl.CoreAudio.Helpers;
using VolumeControl.CoreAudio.Interfaces;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

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
            bool showTraceLogMessages = FLog.FilterEventType(EventType.TRACE);

            _audioDeviceRef = new(owningDevice);
            AudioSessionControl = audioSessionControl2;

            PID = AudioSessionControl.ProcessID;
            using var process = GetProcess();
            ProcessName = process?.ProcessName ?? string.Empty;
            Name = AudioSessionControl.DisplayName.Length > 0 && !AudioSessionControl.DisplayName.StartsWith('@')
                ? AudioSessionControl.DisplayName
                : ProcessName;
            ProcessIdentifier = $"{PID}{ProcessIdentifierSeparatorChar}{ProcessName}{ProcessIdentifierSeparatorChar}{(DataFlow == DataFlow.Capture ? ProcessIdentifierInputChar : ProcessIdentifierOutputChar)}";

            if (AudioSessionControl.SimpleAudioVolume is null)
                throw new NullReferenceException($"{nameof(AudioSession)} '{ProcessName}' ({PID}) {nameof(AudioSessionControl2.SimpleAudioVolume)} is null!");
            if (AudioSessionControl.AudioMeterInformation is null)
                throw new NullReferenceException($"{nameof(AudioSession)} '{ProcessName}' ({PID}) {nameof(AudioSessionControl2.AudioMeterInformation)} is null!");

            AudioSessionControl.OnDisplayNameChanged += this.AudioSessionControl_OnDisplayNameChanged;
            AudioSessionControl.OnIconPathChanged += this.AudioSessionControl_OnIconPathChanged;
            AudioSessionControl.OnSessionDisconnected += this.AudioSessionControl_OnSessionDisconnected;
            AudioSessionControl.OnSimpleVolumeChanged += this.AudioSessionControl_OnSimpleVolumeChanged;
            AudioSessionControl.OnStateChanged += this.AudioSessionControl_OnStateChanged;

            if (showTraceLogMessages)
                FLog.Trace($"Created {nameof(AudioSession)} instance \"{ProcessIdentifier}\"");
        }
        #endregion Constructor

        #region Events
        /// <inheritdoc/>
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
        /// <summary>
        /// The character that separates the PID &amp; ProcessName components of ProcessIdentifier strings.
        /// </summary>
        public const char ProcessIdentifierSeparatorChar = ':';
        /// <summary>
        /// The character used in the process identifier to indicate that a session is a capture session.
        /// </summary>
        public const char ProcessIdentifierInputChar = 'I';
        /// <summary>
        /// The character used in the process identifier to indicate that a session is a render session.
        /// </summary>
        public const char ProcessIdentifierOutputChar = 'O';
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets the <see cref="CoreAudio.AudioDevice"/> that this <see cref="AudioSession"/> instance is running on.
        /// </summary>
        public AudioDevice AudioDevice => _audioDeviceRef.TryGetTarget(out var device) ? device : null!;
        private readonly WeakReference<AudioDevice> _audioDeviceRef;
        /// <summary>
        /// Gets the <see cref="AudioSessionControl2"/> controller instance associated with this <see cref="AudioSession"/> instance.
        /// </summary>
        public AudioSessionControl2 AudioSessionControl { get; }
        internal SimpleAudioVolume SimpleAudioVolume => AudioSessionControl.SimpleAudioVolume!; //< constructor throws if null
        internal AudioMeterInformation AudioMeterInformation => AudioSessionControl.AudioMeterInformation!; //< constructor throws if null
        /// <summary>
        /// Gets the DataFlow direction of this audio session.
        /// </summary>
        public DataFlow DataFlow => AudioDevice.DataFlow;
        /// <summary>
        /// Gets the process ID of the process associated with this <see cref="AudioSession"/> instance.
        /// </summary>
        public uint PID { get; }
        /// <summary>
        /// Gets the Process Name of the process associated with this <see cref="AudioSession"/> instance.
        /// </summary>
        public string ProcessName { get; }
        /// <summary>
        /// Gets or sets the name of this <see cref="AudioSession"/> instance.
        /// </summary>
        /// <remarks>
        /// This defaults to the internal <see cref="AudioSessionControl2.DisplayName"/> if available, otherwise it defaults to the <see cref="ProcessName"/>.
        /// </remarks>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
                // update HasCustomName:
                if (!value.Equals(ProcessName, StringComparison.Ordinal))
                    HasCustomName = true;
            }
        }
        private string _name = string.Empty;
        /// <summary>
        /// Gets whether this session has a Name that differs from its ProcessName.
        /// </summary>
        /// <returns><see langword="true"/> when the Name is not the same as the ProcessName; otherwise <see langword="false"/>.</returns>
        public bool HasCustomName
        {
            get => _hasCustomName;
            private set
            {
                _hasCustomName = value;
                NotifyPropertyChanged();
            }
        }
        private bool _hasCustomName = false;
        /// <summary>
        /// Gets the process identifier of this audio session.
        /// </summary>
        /// <remarks>
        /// Process Identifiers are composed of the <see cref="PID"/> and <see cref="ProcessName"/> of a session, separated by a colon.<br/>
        /// Example: "1234:SomeProcess"
        /// </remarks>
        public string ProcessIdentifier { get; }
        /// <summary>
        /// Gets the session identifier string from the windows API.
        /// </summary>
        /// <remarks>
        /// Processes that create multiple audio sessions will create multiple <see cref="AudioSession"/> instances with identical SessionIdentifier strings. For an identifier that is unique across all audio sessions, use <see cref="SessionInstanceIdentifier"/> or <see cref="ProcessIdentifier"/> instead.
        /// </remarks>
        public string SessionIdentifier => AudioSessionControl.SessionIdentifier;
        /// <summary>
        /// Gets the session instance identifier from the windows API.
        /// </summary>
        /// <remarks>
        /// Unlike the SessionIdentifier, each SessionInstanceIdentifier is guaranteed to be unique to this <see cref="AudioSession"/> instance.
        /// </remarks>
        public string SessionInstanceIdentifier => AudioSessionControl.SessionInstanceIdentifier;
        /// <inheritdoc/>
        public float NativeVolume
        {
            get => SimpleAudioVolume.MasterVolume;
            set
            {
                value = value.Bound(0f, 1f);
                if (value == NativeVolume) return;

                SimpleAudioVolume.MasterVolume = value;
                if (isNotifying) return; //< don't duplicate propertychanged notifications
                isNotifying = true;
                NotifyPropertyChanged();
                NotifyVolumeChanged(value, Mute);
                isNotifying = false;
            }
        }
        /// <inheritdoc/>
        public int Volume
        {
            get => VolumeLevelConverter.FromNativeVolume(NativeVolume);
            set
            {
                NativeVolume = VolumeLevelConverter.ToNativeVolume(value);
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public bool Mute
        {
            get => SimpleAudioVolume.Mute;
            set
            {
                if (value == Mute) return;

                SimpleAudioVolume.Mute = value;
                NotifyPropertyChanged();
                NotifyVolumeChanged(NativeVolume, value);
            }
        }
        #endregion Properties

        #region IVolumePeakMeter Implementation
        /// <inheritdoc/>
        public float PeakMeterValue => AudioMeterInformation.MasterPeakValue;
        #endregion IVolumePeakMeter Implementation

        #region IHideableAudioControl Properties
        /// <summary>
        /// Gets or sets whether this session is hidden in the <see cref="AudioSessionManager"/>.
        /// </summary>
        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                _isHidden = value;
                NotifyPropertyChanged();
            }
        }
        private bool _isHidden;
        #endregion IHideableAudioControl Properties

        #region Methods

        #region ToString
        /// <summary>
        /// Gets the ProcessIdentifier associated with this <see cref="AudioSession"/> instance.
        /// </summary>
        /// <returns>The ProcessIdentifier <see cref="string"/> for this <see cref="AudioSession"/> instance.</returns>
        public override string ToString() => ProcessIdentifier;
        #endregion ToString

        #region ParseProcessIdentifier
        /// <summary>
        /// Splits the given <paramref name="processIdentifier"/> into its ProcessID and ProcessName components, if they exist.
        /// </summary>
        /// <param name="processIdentifier">A process identifier string.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="OverflowException"/>
        /// <returns>A tuple containing the ProcessID and ProcessName components of the given <paramref name="processIdentifier"/>, if found.</returns>
        public static (uint? processId, string? processName) ParseProcessIdentifier(string processIdentifier)
        {
            // remove preceding/trailing whitespace & colons (separator char)
            string s = processIdentifier.Trim(ProcessIdentifierSeparatorChar, ' ', '\t', '\v', '\r', '\n');

            if (s.Length == 0) return (null, null);

            int separatorIndex = s.IndexOf(ProcessIdentifierSeparatorChar, StringComparison.Ordinal);

            if (separatorIndex == -1)
            { // separator character not found; one of the components is missing
                if (s.All(char.IsNumber))
                    return (uint.Parse(s), null);
                else
                { // only the ProcessName component is present:
                    return (null, s);
                }
            }
            else return (uint.Parse(s[..separatorIndex]), s[(separatorIndex + 1)..]);
        }
        /// <summary>
        /// Splits the given <paramref name="processIdentifier"/> into its ProcessID &amp; ProcessName components, and converts the ProcessID to an unsigned integer.
        /// </summary>
        /// <param name="processIdentifier">A Process Identifier string. See <see cref="ProcessIdentifier"/>.</param>
        /// <param name="processId">The ProcessID component of the <paramref name="processIdentifier"/> when it contains one &amp; the method returned <see langword="true"/>; otherwise <see langword="null"/>.</param>
        /// <param name="processName">The ProcessName component of the <paramref name="processIdentifier"/> when it contains one &amp; the method returned <see langword="true"/>; otherwise <see langword="null"/>.</param>
        /// <returns><see langword="true"/> when successful and no exceptions were thrown; otherwise <see langword="false"/>.</returns>
        public static bool TryParseProcessIdentifier(string processIdentifier, out uint? processId, out string? processName)
        {
            try
            {
                (processId, processName) = ParseProcessIdentifier(processIdentifier);
                return true;
            }
            catch (Exception)
            {
                processId = null;
                processName = null;
                return false;
            }
        }
        #endregion ParseProcessIdentifier

        #region GetProcess
        /// <summary>
        /// Gets the process that created this audio session.
        /// </summary>
        /// <remarks>
        /// The caller is responsible for calling <see cref="Process.Dispose(bool)"/>.
        /// </remarks>
        /// <returns><see cref="System.Diagnostics.Process"/> instance that created this audio session, or <see langword="null"/> if an error occurred.</returns>
        public Process? GetProcess()
        {
            try
            {
                return Process.GetProcessById(Convert.ToInt32(PID));
            }
            catch (Exception ex)
            {
                FLog.Error($"Failed to get the Process associated with session '{ProcessIdentifier}' (PID: {PID}, Name: '{Name}', ProcessName: '{ProcessName}') because of an exception:", ex);
                return null;
            }
        }
        #endregion GetProcess

        #region GetTargetInfo
        /// <summary>
        /// Gets a new TargetInfo object representing this AudioSession instance.
        /// </summary>
        /// <returns>A <see cref="TargetInfo"/> struct that represents this <see cref="AudioSession"/> instance.</returns>
        public TargetInfo GetTargetInfo() => new()
        {
            PID = this.PID,
            ProcessName = this.ProcessName,
            SessionInstanceIdentifier = this.SessionInstanceIdentifier,
        };
        #endregion GetTargetInfo

        #region HasMatchingName
        /// <summary>
        /// Determines whether the specified <paramref name="name"/> is equal to this instance's Name or ProcessName.
        /// </summary>
        /// <remarks>
        /// Compares the specified <paramref name="name"/> to the <see cref="ProcessName"/> property and also the <see cref="Name"/> property when <see cref="HasCustomName"/> is <see langword="true"/>.
        /// </remarks>
        /// <param name="name">The Name or ProcessName of an audio session.</param>
        /// <param name="stringComparison">Specifies how strings will be compared.</param>
        /// <returns><see langword="true"/> when the specified <paramref name="name"/> matches this session; otherwise <see langword="false"/>.</returns>
        public bool HasMatchingName(string name, StringComparison stringComparison = StringComparison.Ordinal)
            => HasCustomName && Name.Equals(name, stringComparison) || ProcessName.Equals(name, stringComparison);
        #endregion HasMatchingName

        #endregion Methods

        #region IDisposable Implementation
        /// <summary>
        /// Default finalizer.
        /// </summary>
        ~AudioSession() => Dispose();
        /// <inheritdoc/>
        public void Dispose()
        {
            if (FLog.Log.FilterEventType(EventType.TRACE))
                FLog.Log.Trace($"Disposing of {nameof(AudioSession)} instance \"{ProcessIdentifier}\"");
            this.AudioSessionControl.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation

        #region EventHandlers

        #region AudioSessionControl
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
        #endregion AudioSessionControl

        #endregion EventHandlers
    }
}