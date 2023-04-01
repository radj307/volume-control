using Newtonsoft.Json;
using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Helpers;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Core.Input.Actions
{
    /// <summary>
    /// Represents a setting for a hotkey action.
    /// </summary>
    public interface IHotkeyActionSetting
    {
        /// <summary>
        /// Gets or sets the name of this setting
        /// </summary>
        [JsonIgnore]
        string Label { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="ValueType"/> of <see cref="Value"/> accepted by this setting.<br/>
        /// Setting this to <see langword="null"/> will allow <see cref="Value"/> to be set to any type.
        /// </summary>
        [JsonIgnore]
        Type? ValueType { get; set; }
        /// <summary>
        /// Gets or sets the value of this setting
        /// </summary>
        object? Value { get; set; }
        /// <summary>
        /// Gets or sets the description of this setting, which is shown in a tooltip in the action settings window.
        /// </summary>
        [JsonIgnore]
        string? Description { get; set; }
    }
    /// <summary>
    /// Small container for hotkey action setting data
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    [JsonObject]
    public class HotkeyActionSetting : IHotkeyActionSetting
    {
        /// <summary>
        /// Creates a new empty <see cref="HotkeyActionSetting"/> instance.
        /// </summary>
        public HotkeyActionSetting() { }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSetting"/> instance by copying a <see cref="HotkeyActionSettingAttribute"/> instance.
        /// </summary>
        /// <param name="attr">A <see cref="HotkeyActionSettingAttribute"/> instance.</param>
        public HotkeyActionSetting(HotkeyActionSettingAttribute attr)
        {
            Label = attr.Label;
            ValueType = attr.ValueType;
            Value = ValueType is null
                ? null
                : (ValueType.Equals(typeof(string)) ? string.Empty : Activator.CreateInstance(ValueType));
            Description = attr.Description;
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSetting"/> instance by copying another <see cref="HotkeyActionSetting"/> instance.
        /// </summary>
        /// <param name="copyInstance">An already-created instance of <see cref="HotkeyActionSetting"/> to copy.</param>
        public HotkeyActionSetting(HotkeyActionSetting copyInstance)
        {
            Label = copyInstance.Label;
            ValueType = copyInstance.ValueType;
            Value = copyInstance.Value;
            Description = copyInstance.Description;
        }

        /// <summary>
        /// Gets or sets the name of this setting
        /// </summary>
        [JsonIgnore]
        public string Label { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the <see cref="ValueType"/> of <see cref="Value"/> accepted by this setting.<br/>
        /// Setting this to <see langword="null"/> will allow <see cref="Value"/> to be set to any type.
        /// </summary>
        [JsonIgnore]
        public Type? ValueType { get; set; }
        /// <summary>
        /// Gets or sets the value of this setting
        /// </summary>
        public object? Value { get; set; }
        /// <summary>
        /// Gets or sets the description of this setting, which is shown in a tooltip in the action settings window.
        /// </summary>
        [JsonIgnore]
        public string? Description { get; set; }
    }
    /// <summary>
    /// Specifies a target of a hotkey action.
    /// </summary>
    public abstract class ActionTargetSpecifier
    {
        /// <summary>
        /// List of targets.
        /// </summary>
        public ObservableImmutableList<TargetInfoVM> Targets { get; } = new();

        /// <summary>
        /// Creates a new target entry.
        /// </summary>
        public abstract void AddNewTarget();
    }
    /// <summary>
    /// <see cref="DataTemplateSelector"/> implementation for <see cref="HotkeyActionSetting"/>.
    /// </summary>
    public class HotkeyActionSettingValueTemplateSelector : DataTemplateSelector
    {
        /// <inheritdoc/>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement elem)
            {
                if (item is HotkeyActionSetting setting)
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
                        else if (type.IsSubclassOf(typeof(ActionTargetSpecifier)) || type.Equals(typeof(ActionTargetSpecifier)))
                        {
                            return (elem.FindResource("TargetSpecifierDataTemplate") as DataTemplate)!;
                        }
                    }
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
