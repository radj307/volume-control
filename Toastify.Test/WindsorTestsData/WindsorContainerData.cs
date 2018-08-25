using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Toastify.Tests.WindsorTestsData
{
    internal class WindsorContainerData
    {
        public static WindsorContainer Container
        {
            get
            {
                var container = new WindsorContainer();
                container.Register(
                    Component.For<IDependency>().ImplementedBy<Dependency>(),
                    Component.For<IDependecyWithDependecy>().ImplementedBy<DependecyWithDependencies>());
                return container;
            }
        }
    }
}