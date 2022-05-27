using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VolumeControl.Audio.Interfaces;
using VolumeControl.Core.Extensions;
using VolumeControl.Log;

namespace VolumeControl.Audio
{
    /// <summary>Represents an audio session playing on the system.</summary>
    /// <remarks>Some properties in this object require the NAudio library.<br/>If you're writing an addon, install the 'NAudio' nuget package if you need to be able to use them.</remarks>
    public sealed class AudioSession : ISession, INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
    {
        /// <inheritdoc cref="AudioSession"/>
        /// <param name="controller">The underlying session controller instance to create this audio session object for.<br/>This object is from NAudio.<br/>If you're writing an addon, install the 'NAudio' nuget package to be able to use this.</param>
        public AudioSession(AudioSessionControl controller)
        {
            _controller = controller;
            PID = Convert.ToInt64(_controller.GetProcessID);

            if (GetProcess() is not Process proc)
            {
                FLog.Log.Error($"The constructor of '{typeof(AudioSession).FullName}' encountered an error when getting process with ID '{PID}'!");
                Dispose();
                ProcessName = string.Empty;
            }
            else
            {
                ProcessName = proc.ProcessName;
            }
        }

        #region Fields
        private (ImageSource?, ImageSource?)? _icons = null;
        private readonly AudioSessionControl _controller;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets the <see cref="SimpleAudioVolume"/> object from the underlying <see cref="AudioSessionControl"/> type.
        /// </summary>
        /// <remarks>This object is from NAudio.<br/>If you're writing an addon, install the 'NAudio' nuget package to be able to use this.</remarks>
        public SimpleAudioVolume VolumeController => _controller.SimpleAudioVolume;
        /// <summary>
        /// Gets the current state of the audio session.<br/>
        /// See <see cref="AudioSessionState"/>.
        /// </summary>
        /// <remarks>This object is from NAudio.<br/>If you're writing an addon, install the 'NAudio' nuget package to be able to use this.</remarks>
        public AudioSessionState State => _controller.State;
        /// <summary>This is the location of this sessions icon on the system and may or may not actually be set.<br/><b>Do not rely on this being available, many third-party programs do not provide it!</b></summary>
        /// <remarks>This can point to nothing, one icon in a DLL package, an executable, or all sorts of other formats.<br/>Do not use this for retrieving icon images directly, instead use <see cref="SmallIcon"/>/<see cref="LargeIcon"/>.<br/>You can also use <see cref="Icon"/> directly if you don't care about the icon size.</remarks>
        public string IconPath => _controller.IconPath;
        /// <summary>
        /// The small icon located at the <see cref="IconPath"/>, or null if no icon was found.
        /// </summary>
        /// <remarks>Note that the icon properties all use internal caching; you don't need to worry about using these properties repeatedly for fear of performance loss, as the only time the icons are actually retrieved is the first time any of the icon properties are called.</remarks>
        public ImageSource? SmallIcon => (_icons ??= this.GetIcons())?.Item1;
        /// <summary>
        /// The large icon located at the <see cref="IconPath"/>, or null if no icon was found.
        /// </summary>
        /// <remarks>Note that the icon properties all use internal caching; you don't need to worry about using these properties repeatedly for fear of performance loss, as the only time the icons are actually retrieved is the first time any of the icon properties are called.</remarks>
        public ImageSource? LargeIcon => (_icons ??= this.GetIcons())?.Item2;
        /// <summary>
        /// Gets the large or small icon, depending on whether they are null or not.
        /// </summary>
        /// <remarks>Checks and returns <see cref="LargeIcon"/> first, if that is null then it checks and returns <see cref="SmallIcon"/>.<br/>If both are null, returns null.</remarks>
        public ImageSource? Icon => SmallIcon ?? LargeIcon;
        /// <inheritdoc/>
        public int Volume
        {
            get => Convert.ToInt32(NativeVolume * 100f);
            set
            {
                NotifyPropertyChanging();
                NativeVolume = (float)MathExt.Clamp(Convert.ToDouble(value) / 100f, 0.0, 1.0);
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public float NativeVolume
        {
            get => VolumeController.Volume;
            set
            {
                NotifyPropertyChanging();
                VolumeController.Volume = MathExt.Clamp(value, 0f, 1f);
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public bool Muted
        {
            get => VolumeController.Mute;
            set
            {
                NotifyPropertyChanging();
                VolumeController.Mute = value;
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public string ProcessName { get; }
        /// <inheritdoc/>
        public long PID { get; }
        /// <inheritdoc/>
        public string ProcessIdentifier => PID != -1L ? $"{PID}:{ProcessName}" : string.Empty;
        /// <summary>
        /// Checks if the process that owns this session is still running.
        /// </summary>
        /// <returns>True when the process is still running, otherwise false.</returns>
        public bool IsRunning
        {
            get
            {
                if (GetProcess() is not Process proc)
                {
                    return false;
                }
                else
                {
                    try
                    {
                        return !proc.HasExited();
                    }
                    catch (Exception ex)
                    {
                        FLog.Log.Error(ex);
                        return true;
                    }
                }
            }
        }
        #endregion Properties

        #region Events
        /// <summary>Triggered after one of this instance's properties have been set.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>Triggered before one of this instance's properties is being set.</summary>
        public event PropertyChangingEventHandler? PropertyChanging;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        private void NotifyPropertyChanging([CallerMemberName] string propertyName = "") => PropertyChanging?.Invoke(this, new(propertyName));
        #endregion Events

        #region Methods
        /// <summary>
        /// Gets the process associated with this audio session instance.
        /// </summary>
        /// <returns>The <see cref="Process"/> instance that owns the audio session represented by this instance.</returns>
        public Process? GetProcess()
        {
            try
            {
                return Process.GetProcessById((int)PID);
            }
            catch (Exception ex)
            {
                FLog.Log.Error(ex);
                return null;
            }
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            _controller.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Parses the given process identifier string into a PID and ProcessName.
        /// </summary>
        /// <param name="identifier">A process identifier, PID, or ProcessName to split.</param>
        /// <returns>
        /// Returns a tuple where the first item is the PID and the second is the ProcessName.
        /// </returns>
        /// <exception cref="FormatException">Thrown when there is a colon in the string, but the characters before it are invalid as an integral.</exception>
        public static (int, string) ParseProcessIdentifier(string identifier)
        {
            int delim = identifier
                .Trim(':', ' ') //< only allow colon characters that actually have text on both sides.
                .IndexOf(':');  //< get the index of said inner-colons...

            if (delim == -1)
            {
                if (identifier.All(char.IsDigit) && int.TryParse(identifier, out int result))
                    // pid
                    return (result, string.Empty);
                else // process name
                    return (-1, identifier);
            }
            else // both
            {
                if (!int.TryParse(identifier[..delim], out int result))
                    throw new FormatException($"Invalid process identifier string '{identifier}'");
                return (result, identifier[(delim + 1)..]);
            }
        }
        #endregion Methods
    }
}
