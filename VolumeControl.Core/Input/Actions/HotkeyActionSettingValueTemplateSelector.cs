using System.Windows;
using System.Windows.Controls;

namespace VolumeControl.Core.Input.Actions
{
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
