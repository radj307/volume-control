using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core.Input.Exceptions;
using VolumeControl.Log;

namespace VolumeControl.Core.Input.Actions.Settings
{
    /// <summary>
    /// Container for a named parameter that is passed to an action method when invoked.
    /// </summary>
    /// <typeparam name="T">The type of value contained by this action setting instance.</typeparam>
    public class ActionSettingInstance<T> : IActionSettingInstance<T>, IActionSettingInstance, INotifyPropertyChanged
    {
        #region Constructors
        /// <summary>
        /// Creates a new strongly-typed <see cref="ActionSettingInstance{T}"/> for the specified <paramref name="definition"/> with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="definition">The <see cref="Settings.ActionSettingDefinition"/> instance associated with this action setting.</param>
        /// <param name="enabled">Whether this action setting is enabled or not; or <see langword="null"/> if the setting isn't toggleable.</param>
        /// <param name="value">The value of this action setting.</param>
        internal ActionSettingInstance(ActionSettingDefinition definition, bool? enabled, T value)
        {
            ActionSettingDefinition = definition;
            if (IsToggleable)
                IsEnabled = enabled.GetValueOrDefault();

            Value = value;
        }
        /// <summary>
        /// Creates a new strongly-typed <see cref="ActionSettingInstance{T}"/> for the specified <paramref name="definition"/> with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="definition">The <see cref="Settings.ActionSettingDefinition"/> instance associated with this action setting.</param>
        /// <param name="enabled">Whether this action setting is enabled or not; or <see langword="null"/> if the setting isn't toggleable.</param>
        /// <param name="value">The value of this action setting.</param>
        internal ActionSettingInstance(ActionSettingDefinition definition, bool? enabled, object value)
        {
            ActionSettingDefinition = definition;
            if (IsToggleable)
                IsEnabled = enabled.GetValueOrDefault();

            try
            {
                if (value is JObject jobject)
                { // special handling is required for JObject
                    Value = jobject.ToObject<T>();
                }
                else if (value is JArray jarray)
                { // special handling is required for JArray
                    Value = jarray.ToObject<T>();
                }
                else
                { // using Convert is required here because C# is "highly type-safe" and "trash at casting":
                    Value = (T?)Convert.ChangeType(value, ValueType);
                }
            }
            catch (Exception ex)
            {
                FLog.Log.Error($"An exception occurred during initialization of action setting \"{definition.Name}\":", ex);
                Value = (T?)definition.CreateValueInstance(); //< get a default value
            }
        }
        #endregion Constructors

        #region Properties
        /// <inheritdoc/>
        public ActionSettingDefinition ActionSettingDefinition { get; }
        /// <inheritdoc/>
        public string Name => ActionSettingDefinition.Name;
        /// <inheritdoc/>
        public string? Description => ActionSettingDefinition.Description;
        /// <inheritdoc/>
        public Type ValueType => ActionSettingDefinition.ValueType;
        /// <inheritdoc/>
        public bool IsToggleable => ActionSettingDefinition.IsToggleable;
        /// <inheritdoc/>
        public T? DefaultValue => (T?)ActionSettingDefinition.DefaultValue;
        object? IActionSettingInstance.DefaultValue => ActionSettingDefinition.DefaultValue;
        /// <inheritdoc/>
        public T? Value
        {
            get => _value;
            set
            {
                _value = value;
                NotifyPropertyChanged();
            }
        }
        private T? _value;
        /// <inheritdoc/>
        object? IActionSettingInstance.Value
        {
            get => Value!;
            set
            {
                var incomingValueType = value?.GetType();

                if (!ValueType.IsAssignableFrom(incomingValueType))
                    throw new InvalidActionSettingValueTypeException(incomingValueType, ValueType);

                Value = (T?)value;
            }
        }
        /// <inheritdoc/>
        public bool IsEnabled
        {
            get => !IsToggleable || _isEnabled;
            set
            {
                if (!IsToggleable)
                    return;

                _isEnabled = value;
                NotifyPropertyChanged();
            }
        }
        private bool _isEnabled;
        #endregion Properties

        #region Methods
        /// <inheritdoc/>
        public void Deconstruct(out bool isEnabled, out T? value)
        {
            isEnabled = IsEnabled;
            value = Value;
        }
        void IActionSettingInstance.Deconstruct(out bool isEnabled, out object? value)
        {
            isEnabled = IsEnabled;
            value = Value;
        }
        #endregion Methods

        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events
    }
}
