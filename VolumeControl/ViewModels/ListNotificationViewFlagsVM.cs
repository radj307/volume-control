using System;
using System.ComponentModel;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.TypeExtensions;

namespace VolumeControl.ViewModels
{
    public class ListNotificationViewFlagsVM : FlagsEnumVM<EListNotificationView>
    {
        #region Constructor
        public ListNotificationViewFlagsVM(NotificationConfigSection configSection) : base(configSection.ViewMode, EListNotificationView.Nothing, nameof(EListNotificationView.Everything))
        {
            ConfigSection = configSection;

            ConfigSection.PropertyChanged += this.ConfigSection_PropertyChanged;
            StateChanged += this.ListNotificationViewFlagsVM_StateChanged;
        }
        #endregion Constructor

        #region Fields
        private readonly NotificationConfigSection ConfigSection;
        private bool _thisIsStateChangeSource = false;
        #endregion Fields

        #region Properties
        public bool ShowControlBar
        {
            get => ConfigSection.ViewMode.HasFlag(EListNotificationView.ControlBar);
            set
            {
                ConfigSection.ViewMode = ConfigSection.ViewMode.SetFlagState(EListNotificationView.ControlBar, value);
                NotifyPropertyChanged();
            }
        }
        public bool ShowSelectedOnly
        {
            get => ConfigSection.ViewMode.HasFlag(EListNotificationView.SelectedItemOnly);
            set
            {
                ConfigSection.ViewMode = ConfigSection.ViewMode.SetFlagStates((EListNotificationView.SelectedItemOnly, value), (EListNotificationView.AllItems, !value));
                NotifyPropertyChanged();
            }
        }
        public bool ShowFullList
        {
            get => ConfigSection.ViewMode.HasFlag(EListNotificationView.AllItems);
            set
            {
                ConfigSection.ViewMode = ConfigSection.ViewMode.SetFlagStates((EListNotificationView.AllItems, value), (EListNotificationView.SelectedItemOnly, !value));
                NotifyPropertyChanged();
            }
        }
        #endregion Properties

        #region EventHandlers

        #region ListNotificationViewFlagsVM
        private void ListNotificationViewFlagsVM_StateChanged(object? sender, (EListNotificationView NewState, EListNotificationView ChangedFlags) e)
        {
            if (_thisIsStateChangeSource) return;
            _thisIsStateChangeSource = true;
            if (e.ChangedFlags == EListNotificationView.SelectedItemOnly)
                State &= ~EListNotificationView.AllItems;
            else if (e.ChangedFlags == EListNotificationView.AllItems)
            {
                State &= ~EListNotificationView.SelectedItemOnly;
            }
            ConfigSection.ViewMode = State;
            _thisIsStateChangeSource = false;
            NotifyPropertyChanged(nameof(ShowControlBar));
            NotifyPropertyChanged(nameof(ShowSelectedOnly));
            NotifyPropertyChanged(nameof(ShowFullList));
        }
        #endregion ListNotificationViewFlagsVM

        #region ConfigSection
        private void ConfigSection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_thisIsStateChangeSource || e.PropertyName == null) return;

            if (e.PropertyName.Equals(nameof(NotificationConfigSection.ViewMode), StringComparison.Ordinal))
            {
                State = ConfigSection.ViewMode;
                NotifyPropertyChanged(nameof(ShowControlBar));
                NotifyPropertyChanged(nameof(ShowSelectedOnly));
                NotifyPropertyChanged(nameof(ShowFullList));
            }
        }
        #endregion ConfigSection

        #endregion EventHandlers
    }
}
