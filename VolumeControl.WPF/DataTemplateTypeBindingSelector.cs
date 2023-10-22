using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VolumeControl.WPF
{
    /// <summary>
    /// <see cref="DataTemplateSelector"/> implementation that selects templates from a list of <see cref="DataTemplateTypeBinding"/> objects.
    /// </summary>
    public class DataTemplateTypeBindingSelector : DataTemplateSelector
    {
        #region Properties
        /// <summary>
        /// Gets or sets the list of <see cref="DataTemplateTypeBinding"/> instances.
        /// </summary>
        public List<DataTemplateTypeBinding> DataTemplateTypeBindings { get; set; } = new();
        /// <summary>
        /// Gets or sets the data template to use when no other templates are available.
        /// </summary>
        public DataTemplate? DefaultDataTemplate { get; set; }
        /// <summary>
        /// Gets or sets the name of a property within the bound object to use when detecting the object type.
        /// </summary>
        public string? TargetPropertyName { get; set; }
        #endregion Properties

        #region DataTemplateSelector Implementation
        /// <inheritdoc/>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var elem = (FrameworkElement)container;
            var type = item.GetType();

            if (TargetPropertyName is not null)
            {
                var property = type.GetProperty(TargetPropertyName);
                if (property is null) throw new InvalidOperationException($"Target property name '{TargetPropertyName}' does not exist in type {type.FullName}!");
                type = property.GetValue(item)?.GetType();
            }

            if (type is not null)
            {
                var template = DataTemplateTypeBindings.FirstOrDefault(typeBinding => typeBinding?.Type.Equals(type) ?? false, null);

                if (template is not null && template.DataTemplate is not null)
                    return template.DataTemplate;
                else return DefaultDataTemplate ?? throw new InvalidOperationException($"There is no {nameof(DataTemplate)} for object type '{type.FullName}', and no {nameof(DefaultDataTemplate)} was provided!");
            }

            return base.SelectTemplate(item, container);
        }
        #endregion DataTemplateSelector Implementation
    }
}
