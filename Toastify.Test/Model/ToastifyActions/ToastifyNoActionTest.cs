using NUnit.Framework;
using Toastify.Core;
using Toastify.Model;

namespace Toastify.Tests.Model.ToastifyActions
{
    [TestFixture, TestOf(typeof(ToastifyNoAction))]
    public class ToastifyNoActionTest
    {
        [Test(Author = "aleab")]
        public static void TestState()
        {
            var action = new ToastifyNoAction();
            Assert.Multiple(() =>
            {
                Assert.That(action.Name, Is.EqualTo(ToastifyNoAction.ActionName));
                Assert.That(action.ToastifyActionEnum, Is.EqualTo(ToastifyActionEnum.None));
            });
        }

        [Test(Author = "aleab")]
        public static void TestPerformAction()
        {
            var action = new ToastifyNoAction();
            Assert.DoesNotThrow(() => action.PerformAction());
        }
    }
}