using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VolumeControl.Audio.Interfaces;
using VolumeControl.Log;

namespace VolumeControl.Audio
{
    public class AudioSession : ISession, INotifyPropertyChanged, IDisposable
    {
        public AudioSession(AudioSessionControl controller)
        {
            _controller = controller;
            PID = Convert.ToInt64(_controller.GetProcessID);
            Process? proc = GetProcess();
            if (proc != null)
            {
                ProcessName = proc.ProcessName;
                _processValid = true;
            }
            else
            {
                ProcessName = string.Empty;
            }
        }

        private (ImageSource?, ImageSource?)? _icons = null;
        private readonly AudioSessionControl _controller;
        private bool _processValid = false;

        public SimpleAudioVolume VolumeController => _controller.SimpleAudioVolume;
        public AudioSessionState State => _controller.State;
        public bool Valid => _processValid;
        public string IconPath => _controller.IconPath;
        public ImageSource? SmallIcon => (_icons ??= this.GetIcons())?.Item1;
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
                if (value > 100)
                    value = 100;
                else if (value < 0)
                    value = 0;
                NativeVolume = (float)(Convert.ToDouble(value) / 100.0);
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public float NativeVolume
        {
            get => VolumeController.Volume;
            set
            {
                if (value > 1f)
                    value = 1f;
                else if (value < 0f)
                    value = 0f;
                VolumeController.Volume = value;
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public bool Muted
        {
            get => VolumeController.Mute;
            set
            {
                VolumeController.Mute = value;
                NotifyPropertyChanged();
            }
        }
        public string ProcessName { get; }
        public long PID { get; }
        public string ProcessIdentifier => PID != -1L ? $"{PID}:{ProcessName}" : string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));

        internal Process? GetProcess()
        {
            try
            {
                return Process.GetProcessById((int)PID);
            }
            catch (Exception ex)
            {
                FLog.Log.ErrorException(ex);
                _processValid = false;
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
        /// Returns a pair where the first item is the PID and the second is the ProcessName.
        /// </returns>
        /// <exception cref="FormatException">Thrown when there is a colon in the string, but the characters before it are invalid as an integral.</exception>
        public static (int, string) ParseProcessIdentifier(string identifier)
        {
            int delim = identifier.Trim(':', ' ').IndexOf(':');

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
    }
}
