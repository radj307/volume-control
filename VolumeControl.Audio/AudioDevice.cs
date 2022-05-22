using NAudio.CoreAudioApi;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VolumeControl.Audio.Interfaces;

namespace VolumeControl.Audio
{
    /// <summary>
    /// Represents an audio device endpoint.
    /// </summary>
    /// <remarks>This object uses the MMDevice API from NAudio, and implements the following interfaces:<list type="bullet">
    /// <item><description><see cref="IDevice"/></description></item>
    /// <item><description><see cref="INotifyPropertyChanged"/></description></item>
    /// <item><description><see cref="IDisposable"/></description></item>
    /// </list>
    /// Some properties in this object require the NAudio library.<br/>If you're writing an addon, install the 'NAudio' nuget package if you need to be able to use them.
    /// </remarks>
    public sealed class AudioDevice : IDevice, INotifyPropertyChanged, IDisposable
    {
        #region Constructors
        /// <inheritdoc cref="AudioDevice"/>
        /// <param name="device">An enumerated <see cref="MMDevice"/> instance from NAudio.</param>
        public AudioDevice(MMDevice device)
        {
            _device = device;
            _deviceID = _device.ID;
        }
        #endregion Constructors

        #region Fields
        private readonly string _deviceID;
        private MMDevice _device;
        private (ImageSource?, ImageSource?)? _icons = null;
        #endregion Fields

        #region Properties
        /// <inheritdoc/>
        public string Name => DeviceFriendlyName;
        /// <inheritdoc/>
        public string DeviceID => _deviceID;
        /// <summary>The instance ID.</summary>
        public string InstanceID => _device.InstanceId;
        /// <summary>This is the location of this device's icon on the system and may or may not actually be set.</summary>
        /// <remarks>This can point to nothing, one icon in a DLL package, an executable, or all sorts of other formats.<br/>Do not use this for retrieving icon images directly, instead use <see cref="SmallIcon"/>/<see cref="LargeIcon"/>.<br/>You can also use <see cref="Icon"/> directly if you don't care about the icon size.</remarks>
        public string IconPath => _device.IconPath;
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
        /// Returns <see cref="SmallIcon"/> or if it is set to null, <see cref="LargeIcon"/> is returned instead.<br/>
        /// If both icons are set to null, this returns null.
        /// </summary>
        /// <remarks>Note that the icon properties all use internal caching; you don't need to worry about using these properties repeatedly for fear of performance loss, as the only time the icons are actually retrieved is the first time any of the icon properties are called.</remarks>
        public ImageSource? Icon => SmallIcon ?? LargeIcon;
        /// <summary>
        /// Gets the friendly name of the endpoint device, including the name of the adapter.<br/>Example: "Speakers (XYZ Audio Adapter)"
        /// </summary>
        /// <remarks>For more information on this property, see MSDN: <see href="https://docs.microsoft.com/en-us/windows/win32/coreaudio/pkey-device-friendlyname">PKEY_Device_FriendlyName</see></remarks>
        public string DeviceFriendlyName => _device.DeviceFriendlyName;
        /// <summary>
        /// Gets the friendly name of the endpoint adapter, excluding the name of the device.<br/>Example: "Speakers (XYZ Audio Adapter)"
        /// </summary>
        /// <remarks>For more information on this property, see MSDN: <see href="https://docs.microsoft.com/en-us/windows/win32/coreaudio/pkey-device-friendlyname">PKEY_Device_FriendlyName</see></remarks>
        public string FriendlyName => _device.FriendlyName;
        /// <summary>
        /// Gets the underlying <see cref="PropertyStore"/> object for this endpoint.
        /// </summary>
        /// <remarks>This object is from NAudio.<br/>If you're writing an addon, install the 'NAudio' nuget package to be able to use this.</remarks>
        public PropertyStore Properties => _device.Properties;
        /// <summary>
        /// Gets the current <see cref="DeviceState"/> of this endpoint.<br/>For more information, see the MSDN page: <see href="https://docs.microsoft.com/en-us/windows/win32/coreaudio/device-state-xxx-constants">DEVICE_STATE_XXX Constants</see>
        /// </summary>
        /// <remarks>This object is from NAudio.<br/>If you're writing an addon, install the 'NAudio' nuget package to be able to use this.</remarks>
        public DeviceState State => _device.State;
        /// <summary>
        /// Gets the <see cref="AudioSessionManager"/> object from the underlying <see cref="MMDevice"/>.
        /// </summary>
        /// <remarks>This object is from NAudio.<br/>If you're writing an addon, install the 'NAudio' nuget package to be able to use this.</remarks>
        public AudioSessionManager SessionManager => _device.AudioSessionManager;
        /// <summary>
        /// Gets the <see cref="AudioEndpointVolume"/> object from the underlying <see cref="MMDevice"/>.
        /// </summary>
        /// <remarks>This object is from NAudio.<br/>If you're writing an addon, install the 'NAudio' nuget package to be able to use this.</remarks>
        public AudioEndpointVolume EndpointVolumeObject => _device.AudioEndpointVolume;
        /// <inheritdoc/>
        public float NativeEndpointVolume
        {
            get => EndpointVolumeObject.MasterVolumeLevel;
            set
            {
                if (value > 1f)
                    value = 1f;
                else if (value < 0f)
                    value = 0f;
                EndpointVolumeObject.MasterVolumeLevel = value;
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public int EndpointVolume
        {
            get => Convert.ToInt32(NativeEndpointVolume * 100f);
            set
            {
                if (value > 100)
                    value = 100;
                else if (value < 0)
                    value = 0;
                NativeEndpointVolume = (float)(Convert.ToDouble(value) / 100.0);
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public bool EndpointMuted
        {
            get => EndpointVolumeObject.Mute;
            set
            {
                EndpointVolumeObject.Mute = value;
                NotifyPropertyChanged();
            }
        }
        #endregion Properties

        #region Events
        /// <summary>Triggered when a property is set.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Methods
        private void Reload()
        {
            using MMDeviceEnumerator enm = new();
            _device = enm.GetDevice(_deviceID);
            enm.Dispose();
        }
        /// <summary>
        /// Gets the list of audio sessions currently using this device.
        /// </summary>
        /// <returns>A list of new <see cref="AudioSession"/> instances.</returns>
        public List<AudioSession> GetAudioSessions()
        {
            Reload();
            SessionCollection? sessions = _device.AudioSessionManager.Sessions;
            List<AudioSession> l = new();
            for (int i = 0; i < sessions.Count; ++i)
            {
                l.Add(new AudioSession(sessions[i]));
            }
            return l;
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            _device.Dispose();
            _icons = null;
            GC.SuppressFinalize(this);
        }
        #endregion Methods
    }
}
