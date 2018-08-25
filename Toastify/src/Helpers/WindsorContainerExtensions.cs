using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Windsor;
using JetBrains.Annotations;
using Toastify.DI;

namespace Toastify.Helpers
{
    public static class WindsorContainerExtensions
    {
        #region Static Members

        public static void BuildUp(this WindsorContainer container, [NotNull] object obj)
        {
            IEnumerable<PropertyInfo> properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                      .Where(p => p.GetCustomAttribute<PropertyDependencyAttribute>() != null && p.GetValue(obj) == null);
            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(obj) == null && container.Kernel.HasComponent(property.PropertyType))
                {
                    object dependency = container.Kernel.Resolve(property.PropertyType);
                    property.SetValue(obj, dependency);
                }
            }
        }

        public static void BuildUp(this WindsorContainer container, [NotNull] params object[] objects)
        {
            BuildUpAll(container, objects);
        }

        public static void BuildUpAll(this WindsorContainer container, [NotNull] IEnumerable<object> objects)
        {
            foreach (object obj in objects)
            {
                BuildUp(container, obj);
            }
        }

        #endregion
    }
}