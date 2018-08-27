using System;
using System.Collections;
using FakeItEasy;
using JetBrains.Annotations;
using NUnit.Framework;
using Toastify.Core;
using Toastify.Model;
using ToastifyAPI.Events;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Tests.Model.ToastifyActions
{
    [TestFixture, TestOf(typeof(ToastifyAction))]
    public class ToastifyActionTest
    {
        [Test(Author = "aleab")]
        public void TestToString()
        {
            const string name = "Action Name";
            var toastifyAction = A.Fake<ToastifyAction>();
            A.CallTo(() => toastifyAction.Name).ReturnsLazily(() => name);

            Assert.That(toastifyAction.ToString(), Is.EqualTo(name));
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(ToastifyActionData), nameof(ToastifyActionData.EqualsActionTestCases))]
        public bool TestEquals_IAction([NotNull] ToastifyAction action1, IAction action2)
        {
            return action1.Equals(action2);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(ToastifyActionData), nameof(ToastifyActionData.EqualsObjectTestCases))]
        public bool TestEquals_Object([NotNull] ToastifyAction action, object obj)
        {
            return action.Equals(obj);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(ToastifyActionData), nameof(ToastifyActionData.GetHashCodeTestCases))]
        public bool TestGetHashCode([NotNull] ToastifyAction action1, [NotNull] ToastifyAction action2)
        {
            return action1.GetHashCode() == action2.GetHashCode();
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(ToastifyActionData), nameof(ToastifyActionData.ActionEventsTestCases))]
        public void TestActionEvents(ToastifyAction action, bool shouldFail)
        {
            bool? success = null;
            void OnActionFailed(object sender, ActionFailedEventArgs e) => success = false;
            void OnActionPerformed(object sender, EventArgs e) => success = true;

            action.ActionFailed += OnActionFailed;
            action.ActionPerformed += OnActionPerformed;

            action.PerformAction();

            action.ActionFailed -= OnActionFailed;
            action.ActionPerformed -= OnActionPerformed;

            Assert.That(success, Is.EqualTo(!shouldFail));
        }

        [Test(Author = "aleab")]
        public void TestClone()
        {
            ToastifyMediaAction toastifyAction = new ToastifySimpleMediaAction("Fake Media Action", ToastifyActionEnum.None, 0);
            ToastifyMediaAction toastifyActionClone = toastifyAction.Clone() as ToastifyMediaAction;

            Assert.Multiple(() =>
            {
                Assert.That(toastifyActionClone, Is.Not.Null);
                Assert.That(toastifyActionClone, Is.Not.SameAs(toastifyAction));
                Assert.That(toastifyActionClone, Is.EqualTo(toastifyAction));
            });
        }

        public class ToastifyActionData
        {
            #region Static Fields and Properties

            public static IEnumerable EqualsActionTestCases
            {
                get
                {
                    var fakeA = A.Fake<IAction>();
                    A.CallTo(() => fakeA.Name).Returns("Fake Action");

                    ToastifyAction fakeTA = new FakeToastifyAction("Fake Toastify Action", ToastifyActionEnum.PlayPause);
                    ToastifyAction fakeTACopy = new FakeToastifyAction("Fake Toastify Action", ToastifyActionEnum.PlayPause);
                    ToastifyAction fakeTAWithSameNameAsFakeA = new FakeToastifyAction("Fake Action", ToastifyActionEnum.PlayPause);

                    yield return new TestCaseData(fakeTAWithSameNameAsFakeA, fakeA).Returns(true).SetName("(ToastifyAction, IAction); same name");
                    yield return new TestCaseData(fakeTA, fakeA).Returns(false).SetName("(ToastifyAction, IAction); different name");

                    yield return new TestCaseData(fakeTA, null).Returns(false).SetName("(ToastifyAction, null)");
                    yield return new TestCaseData(fakeTA, fakeTA).Returns(true).SetName("(ToastifyAction, ToastifyAction); same instance");
                    yield return new TestCaseData(fakeTA, fakeTACopy).Returns(true).SetName("(ToastifyAction, ToastifyAction); same name");
                    yield return new TestCaseData(fakeTA, fakeTAWithSameNameAsFakeA).Returns(false).SetName("(ToastifyAction, ToastifyAction); different name");
                }
            }

            public static IEnumerable EqualsObjectTestCases
            {
                get
                {
                    var fakeA = A.Fake<IAction>();
                    A.CallTo(() => fakeA.Name).Returns("Fake Action");

                    ToastifyAction fakeTA = new FakeToastifyAction("Fake Toastify Action", ToastifyActionEnum.PlayPause);
                    ToastifyAction fakeTACopy = new FakeToastifyAction("Fake Toastify Action", ToastifyActionEnum.PlayPause);
                    ToastifyAction fakeTADifferentName = new FakeToastifyAction("Fake Action", ToastifyActionEnum.PlayPause);
                    ToastifyAction fakeTADifferentAction = new FakeToastifyAction("Fake Toastify Action", ToastifyActionEnum.Stop);

                    yield return new TestCaseData(fakeTA, null).Returns(false).SetName("(ToastifyAction, null)");
                    yield return new TestCaseData(fakeTA, fakeTA).Returns(true).SetName("(ToastifyAction, ToastifyAction); same instance");
                    yield return new TestCaseData(fakeTA, fakeTACopy).Returns(true).SetName("(ToastifyAction, ToastifyAction); equals");
                    yield return new TestCaseData(fakeTA, fakeTADifferentName).Returns(false).SetName("(ToastifyAction, ToastifyAction); different name");
                    yield return new TestCaseData(fakeTA, fakeTADifferentAction).Returns(false).SetName("(ToastifyAction, ToastifyAction); different action");

                    // Different types
                    yield return new TestCaseData(fakeTA, fakeA).Returns(false).SetName("Different Type: IAction");
                    yield return new TestCaseData(fakeTA, string.Empty).Returns(false).SetName("Different Type: string");
                }
            }

            public static IEnumerable GetHashCodeTestCases
            {
                get
                {
                    ToastifyAction fakeTA = new FakeToastifyAction("Fake Toastify Action", ToastifyActionEnum.PlayPause);
                    ToastifyAction fakeTACopy = new FakeToastifyAction("Fake Toastify Action", ToastifyActionEnum.PlayPause);
                    ToastifyAction fakeTADifferentName = new FakeToastifyAction("Fake Action", ToastifyActionEnum.PlayPause);
                    ToastifyAction fakeTADifferentAction = new FakeToastifyAction("Fake Toastify Action", ToastifyActionEnum.Stop);

                    yield return new TestCaseData(fakeTA, fakeTA).Returns(true).SetName("Same instance");
                    yield return new TestCaseData(fakeTA, fakeTACopy).Returns(true).SetName("Equal instances");
                    yield return new TestCaseData(fakeTA, fakeTADifferentName).Returns(false).SetName("Different name");
                    yield return new TestCaseData(fakeTA, fakeTADifferentAction).Returns(false).SetName("Different action");
                }
            }

            public static IEnumerable ActionEventsTestCases
            {
                get
                {
                    yield return new TestCaseData(new FakeToastifyAction { ShouldFail = false }, false).SetName("OnPerformed event");
                    yield return new TestCaseData(new FakeToastifyAction { ShouldFail = true }, true).SetName("OnFailed event");
                }
            }

            #endregion
        }

        internal class FakeToastifyAction : ToastifyAction
        {
            #region Public Properties

            public bool ShouldFail { get; set; }

            #endregion

            public FakeToastifyAction()
            {
            }

            public FakeToastifyAction(string name, ToastifyActionEnum toastifyAction) : base(name, toastifyAction)
            {
            }

            public override void PerformAction()
            {
                if (this.ShouldFail)
                    this.RaiseActionFailed(this, new ActionFailedEventArgs("Failed"));
                else
                    this.RaiseActionPerformed(this);
            }
        }
    }
}