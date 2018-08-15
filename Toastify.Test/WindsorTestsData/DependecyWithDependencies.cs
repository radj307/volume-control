using Toastify.DI;

namespace Toastify.Tests.WindsorTestsData
{
    internal class DependecyWithDependencies : DependencyBase, IDependecyWithDependecy
    {
        [PropertyDependency]
        public IDependency Dependency { get; set; }
    }
}