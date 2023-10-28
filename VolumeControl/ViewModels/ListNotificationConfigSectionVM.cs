using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VolumeControl.Core;

namespace VolumeControl.ViewModels
{
    public class NotificationConfigSectionVM : INotifyPropertyChanged
    {
        #region Constructor
        public NotificationConfigSectionVM(NotificationConfigSection configSection)
        {
            ConfigSection = configSection;
            FlagsVM = new(ConfigSection);
            ConfigSection.PropertyChanged += this.ConfigSection_PropertyChanged;
        }
        #endregion Constructor

        #region Properties
        public NotificationConfigSection ConfigSection { get; }
        public ListNotificationViewFlagsVM FlagsVM { get; }
        public Brush LockedAccentBrush => new SolidColorBrush(ConfigSection.LockedColor);
        public Brush UnlockedAccentBrush => new SolidColorBrush(ConfigSection.UnlockedColor);
        public Brush BackgroundBrush => new SolidColorBrush(ConfigSection.BackgroundColor);
        public Brush ForegroundBrush => new SolidColorBrush(ConfigSection.TextColor);
        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region EventHandlers

        #region ConfigSection
        private void ConfigSection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null) return;

            if (e.PropertyName.Equals(nameof(NotificationConfigSection.LockedColor), System.StringComparison.Ordinal))
            {
                NotifyPropertyChanged(nameof(LockedAccentBrush));
            }
            else if (e.PropertyName.Equals(nameof(NotificationConfigSection.UnlockedColor), System.StringComparison.Ordinal))
            {
                NotifyPropertyChanged(nameof(UnlockedAccentBrush));
            }
            else if (e.PropertyName.Equals(nameof(NotificationConfigSection.BackgroundColor), System.StringComparison.Ordinal))
            {
                NotifyPropertyChanged(nameof(BackgroundBrush));
            }
            else if (e.PropertyName.Equals(nameof(NotificationConfigSection.TextColor), System.StringComparison.Ordinal))
            {
                NotifyPropertyChanged(nameof(ForegroundBrush));
            }
        }
        #endregion ConfigSection

        #endregion EventHandlers
    }
}
