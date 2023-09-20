using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows;
using VolumeControl.Core.Helpers;

namespace VolumeControl.Core
{
    /// <summary>
    /// Configuration for a ListNotification window.
    /// </summary>
    public class NotificationConfigSection : INotifyPropertyChanged
    {
        #region Events
#       pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        // Fody injects code to call this event:
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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
        public ScreenCorner PositionOriginCorner { get; set; } = ScreenCorner.TopLeft;
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
        #endregion Properties
    }
}
