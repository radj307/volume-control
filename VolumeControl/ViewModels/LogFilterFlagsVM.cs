using System;
using System.ComponentModel;
using VolumeControl.Core;
using VolumeControl.Log.Enum;

namespace VolumeControl.ViewModels
{
    public class LogFilterFlagsVM : FlagsEnumVM<EventType>
    {
        #region Constructor
        public LogFilterFlagsVM() : base(Settings.LogFilter, EventType.NONE | EventType.CRITICAL)
        {
            Settings.PropertyChanged += this.Settings_PropertyChanged;
            base.StateChanged += this.LogFilterFlagsVM_StateChanged;
        }
        #endregion Constructor

        #region Fields
        private bool _thisIsStateChangeSource = false;
        #endregion Fields

        #region Properties
        private static Config Settings => (Config.Default as Config)!;
        #endregion Properties

        #region EventHandlers

        #region LogFilterFlagsVM
        private void LogFilterFlagsVM_StateChanged(object? sender, (EventType NewState, EventType ChangedFlags) e)
        {
            _thisIsStateChangeSource = true;
            Settings.LogFilter = e.NewState;
            _thisIsStateChangeSource = false;
        }
        #endregion LogFilterFlagsVM

        #region Settings
        private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_thisIsStateChangeSource || e.PropertyName == null) return;

            if (e.PropertyName.Equals(nameof(Config.LogFilter), StringComparison.Ordinal))
            {
                State = Settings.LogFilter;
            }
        }
        #endregion Settings

        #endregion EventHandlers
    }
}
