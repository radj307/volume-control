using Localization;
using System;
using System.ComponentModel;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.TypeExtensions;

namespace VolumeControl.ViewModels
{
    public class NotificationViewFlagsVM : FlagsEnumVM<ENotificationViewMode, NotificationViewFlagsVM.NotificationViewFlagsValueVM>
    {
        #region Constructor
        public NotificationViewFlagsVM(NotificationConfigSection configSection) : base(configSection.ViewMode, ENotificationViewMode.Nothing, nameof(ENotificationViewMode.Everything))
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
            get => ConfigSection.ViewMode.HasFlag(ENotificationViewMode.ControlBar);
            set
            {
                ConfigSection.ViewMode = ConfigSection.ViewMode.SetFlagState(ENotificationViewMode.ControlBar, value);
                NotifyPropertyChanged();
            }
        }
        public bool ShowSelectedOnly
        {
            get => ConfigSection.ViewMode.HasFlag(ENotificationViewMode.SelectedItemOnly);
            set
            {
                ConfigSection.ViewMode = ConfigSection.ViewMode.SetFlagStates((ENotificationViewMode.SelectedItemOnly, value), (ENotificationViewMode.AllItems, !value));
                NotifyPropertyChanged();
            }
        }
        public bool ShowFullList
        {
            get => ConfigSection.ViewMode.HasFlag(ENotificationViewMode.AllItems);
            set
            {
                ConfigSection.ViewMode = ConfigSection.ViewMode.SetFlagStates((ENotificationViewMode.AllItems, value), (ENotificationViewMode.SelectedItemOnly, !value));
                NotifyPropertyChanged();
            }
        }
        #endregion Properties

        #region EventHandlers

        #region ListNotificationViewFlagsVM
        private void ListNotificationViewFlagsVM_StateChanged(object? sender, (ENotificationViewMode NewState, ENotificationViewMode ChangedFlags) e)
        {
            if (_thisIsStateChangeSource) return;
            _thisIsStateChangeSource = true;
            if (e.ChangedFlags == ENotificationViewMode.SelectedItemOnly)
                State &= ~ENotificationViewMode.AllItems;
            else if (e.ChangedFlags == ENotificationViewMode.AllItems)
            {
                State &= ~ENotificationViewMode.SelectedItemOnly;
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

        #region (class) NotificationViewFlagsValueVM
        public class NotificationViewFlagsValueVM : FlagsEnumValueBase<ENotificationViewMode>
        {
            #region Constructor
            internal NotificationViewFlagsValueVM(ENotificationViewMode viewMode, bool isSet) : base(viewMode, isSet)
            {
                Loc.Instance.CurrentLanguageChanged += this.Instance_CurrentLanguageChanged;
            }
            #endregion Constructor

            #region Properties
            public override string Name
            {
                get
                {
                    var enumName = Enum.GetName(Value)!;
                    return Loc.Tr($"Enums.ENotificationViewMode.{enumName}", defaultText: enumName);
                }
            }
            #endregion Properties

            #region EventHandlers
            private void Instance_CurrentLanguageChanged(object? sender, CurrentLanguageChangedEventArgs e)
                => NotifyPropertyChanged(nameof(Name));
            #endregion EventHandlers
        }
        #endregion (class) NotificationViewFlagsValueVM
    }
}
