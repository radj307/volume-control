using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using VolumeControl.Core.Enum;
using VolumeControl.WPF.Extensions;

namespace VolumeControl.Core
{
    /// <summary>
    /// Configuration for a ListNotification window.
    /// </summary>
    public class NotificationConfigSection : INotifyPropertyChanged
    {
        #region Events
        // Fody injects property setters to call this event, so ignore warnings about not using it:
#   pragma warning disable CS0067 // The event 'NotificationConfigSection.PropertyChanged' is never used
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
#   pragma warning restore CS0067 // The event 'NotificationConfigSection.PropertyChanged' is never used
        #endregion Events

        #region Fields
        /// <summary>
        /// Gets the default value for the <see cref="TimeoutMs"/> property.
        /// </summary>
        [JsonIgnore]
        public const int DefaultTimeoutMs = 3000;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets or sets whether notifications are enabled or not.
        /// </summary>
        public bool Enabled { get; set; } = true;
        /// <summary>
        /// Gets or sets whether or not the list notification window appears for volume change events.
        /// </summary>
        public bool ShowOnVolumeChanged { get; set; } = true;
        /// <summary>
        /// Gets or sets whether the notification window disappears on its own after a set amount of time.
        /// </summary>
        public bool TimeoutEnabled { get; set; } = true;
        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, that the notification window stays visible for before disappearing.
        /// </summary>
        public int TimeoutMs { get; set; } = DefaultTimeoutMs;
        /// <summary>
        /// Gets or sets the location of the notification window
        /// </summary>
        public Point? Position { get; set; }
        /// <summary>
        /// Gets or sets the corner from which ListNotification size transform operations are rooted.
        /// </summary>
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public EScreenCorner PositionOriginCorner { get; set; } = EScreenCorner.TopLeft;
        /// <summary>
        /// Gets or sets whether the notification window slowly fades in instead of appearing instantly.
        /// </summary>
        public bool DoFadeIn { get; set; } = true;
        /// <summary>
        /// Gets or sets the duration of the notification fade-in animation.
        /// </summary>
        public Duration FadeInDuration { get; set; } = new(TimeSpan.FromMilliseconds(150));
        /// <summary>
        /// Gets or sets whether the notification window slowly fades out instead of disappearing instantly.
        /// </summary>
        public bool DoFadeOut { get; set; } = true;
        /// <summary>
        /// Gets or sets the duration of the notification fade-in animation.
        /// </summary>
        public Duration FadeOutDuration { get; set; } = new(TimeSpan.FromMilliseconds(750));
        /// <summary>
        /// Gets or sets the view mode of this notification.
        /// </summary>
        public ENotificationViewMode ViewMode { get; set; } = ENotificationViewMode.Everything;
        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.FromRgb(0x25, 0x25, 0x25);
        /// <summary>
        /// Gets or sets the accent color when selection is locked.
        /// </summary>
        public Color LockedColor { get; set; } = Color.FromRgb(0xA3, 0x28, 0x28);
        /// <summary>
        /// Gets or sets the accent color when selection is unlocked.
        /// </summary>
        public Color UnlockedColor { get; set; } = Color.FromRgb(0x49, 0x6D, 0x49);
        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        public Color TextColor { get; set; } = Colors.WhiteSmoke;
        /// <summary>
        /// Gets or sets the radius of the notification window's corners.
        /// </summary>
        public CornerRadius CornerRadius { get; set; } = new(19);
        #endregion Properties
    }
}
