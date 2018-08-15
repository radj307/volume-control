using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Toastify.DI;

namespace Toastify.Tests.WindsorTestsData
{
    internal abstract class DependencyBase
    {
        public bool DependenciesInjected()
        {
            IEnumerable<PropertyInfo> properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                       .Where(p => p.GetCustomAttribute<PropertyDependencyAttribute>() != null);
            return properties.All(p => p.GetValue(this) != null);
        }
    }
}