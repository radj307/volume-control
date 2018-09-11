using NUnit.Framework;
using Toastify.Helpers;
using Toastify.Tests.WindsorTestsData;

namespace Toastify.Tests.Helpers
{
    [TestFixture, TestOf(typeof(WindsorContainerExtensions))]
    public class WindsorContainerExtensionsTest
    {
        [Test(Author = "aleab")]
        public static void BuildUpTest()
        {
            DependencyBase objWithDependencies = new DependecyWithDependencies();
            WindsorContainerData.Container.BuildUp(objWithDependencies);

            Assert.That(objWithDependencies.DependenciesInjected());
        }
    }
}