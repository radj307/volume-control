using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core.Input.Exceptions;

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
        /// <param name="value">The value of this action setting.</param>
        internal ActionSettingInstance(ActionSettingDefinition definition, T value)
        {
            ActionSettingDefinition = definition;
            Value = value;
        }
        /// <summary>
        /// Creates a new strongly-typed <see cref="ActionSettingInstance{T}"/> for the specified <paramref name="definition"/> with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="definition">The <see cref="Settings.ActionSettingDefinition"/> instance associated with this action setting.</param>
        /// <param name="value">The value of this action setting.</param>
        internal ActionSettingInstance(ActionSettingDefinition definition, object value)
        {
            ActionSettingDefinition = definition;

            if (value is JObject jobject)
            { // special handling is required for JObject
                Value = jobject.ToObject<T>();
            }
            else
            {
                // using Convert is required here because C# is "highly type-safe" and "trash at casting":
                Value = (T?)Convert.ChangeType(value, ValueType);
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
                if (incomingValueType != ValueType)
                    throw new InvalidActionSettingValueTypeException(incomingValueType, ValueType);

                Value = (T?)value;
            }
        }
        #endregion Properties

        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events
    }
}
