using System.ComponentModel;
using VolumeControl.Log;

namespace VolumeControl.WPF.Collections
{
    /// <summary>
    /// This is an extension of the <see cref="BindableEventType"/> object that initializes the <see cref="BindableEventType.Value"/> property to <see cref="FLog.EventFilter"/>.
    /// </summary>
    public class LogEventTypeFilter : BindableEventType
    {
        #region Initializers
        /// <summary>
        /// Creates a new <see cref="LogEventTypeFilter"/> instance.
        /// </summary>
        public LogEventTypeFilter()
        {
            Value = FLog.EventFilter;
            PropertyChanged += HandlePropertyChanged;
        }
        #endregion Initializers

        #region Methods
        private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (nameof(Value).Equals(e.PropertyName))
            {
                FLog.Log.EventTypeFilter = Value;
            }
        }
        #endregion Methods
    }
}
