using PropertyChanged;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace VolumeControl.Core.Input.Actions
{
    /// <summary>
    /// Small container for hotkey action setting data
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class HotkeyActionSetting
    {
        public HotkeyActionSetting()
        {
        }
        public HotkeyActionSetting(string label, Type? valueType)
        {
            Label = label;
            ValueType = valueType;
            Value = valueType is null
                ? null
                : (valueType.Equals(typeof(string)) ? string.Empty : Activator.CreateInstance(valueType));
        }
        public HotkeyActionSetting(string label, Type? valueType, object? value)
        {
            Label = label;
            ValueType = valueType;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the name of this setting
        /// </summary>
        public string Label { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the <see cref="ValueType"/> of <see cref="Value"/> accepted by this setting.<br/>
        /// Setting this to <see langword="null"/> will allow <see cref="Value"/> to be set to any type.
        /// </summary>
        public Type? ValueType { get; set; }
        /// <summary>
        /// Gets or sets the value of this setting
        /// </summary>
        public object? Value { get; set; }
    }

    public class HotkeyActionSettingValueTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not HotkeyActionSetting setting)
                throw new InvalidOperationException($"{nameof(HotkeyActionSettingValueTemplateSelector)} received an item of type '{item.GetType().FullName}'; expected type '{typeof(HotkeyActionSettingValueTemplateSelector).FullName}'");

            if (container is FrameworkElement elem)
            {
                var type = setting.Value?.GetType();
                if (type is not null)
                {
                    if (type.Equals(typeof(string)))
                    {
                        return (elem.FindResource("StringDataTemplate") as DataTemplate)!;
                    }
                    else if (type.Equals(typeof(bool)))
                    {
                        return (elem.FindResource("BoolDataTemplate") as DataTemplate)!;
                    }
                    else if (type.GetInterface(nameof(IList)) is not null)
                    {
                        return (elem.FindResource("ListDataTemplate") as DataTemplate)!;
                    }
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
