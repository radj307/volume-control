using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VolumeControl.Core.Interfaces;
using VolumeControl.Log;

namespace VolumeControl.Core.HelperTypes
{
    public class AudioSession : ISession, INotifyPropertyChanged, IDisposable
    {
        public AudioSession(AudioSessionControl controller)
        {
            _controller = controller;
            PID = Convert.ToInt64(_controller.GetProcessID);
            var proc = GetProcess();
            if (proc != null)
            {
                ProcessName = proc.ProcessName;
                _processValid = true;
            }
            else
                ProcessName = string.Empty;
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
    }
}
