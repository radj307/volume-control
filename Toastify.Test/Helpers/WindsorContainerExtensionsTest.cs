using NUnit.Framework;
using Toastify.Helpers;
using Toastify.Tests.WindsorTestsData;

namespace Toastify.Tests.Helpers
{
    [TestFixture, TestOf(typeof(WindsorContainerExtensions))]
    public class WindsorContainerExtensionsTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [Test(Author = "aleab")]
        public void BuildUpTest()
        {
            DependencyBase objWithDependencies = new DependecyWithDependencies();
            WindsorContainerData.Container.BuildUp(objWithDependencies);

            Assert.That(objWithDependencies.DependenciesInjected());
        }
    }
}