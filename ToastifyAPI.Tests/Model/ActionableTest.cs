using FakeItEasy;
using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using ToastifyAPI.Model;
using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Tests.Model
{
    [TestFixture, TestOf(typeof(Actionable))]
    public class ActionableTest
    {
        private IAction fakeAction;
        private Actionable fakeActionable;

        [SetUp]
        public void SetUp()
        {
            this.fakeAction = A.Fake<IAction>();
            A.CallTo(() => this.fakeAction.PerformAction()).DoesNothing();

            this.fakeActionable = new Actionable(this.fakeAction);
        }

        [Test(Author = "aleab")]
        public void CanPerformAction()
        {
            this.fakeActionable.MaxFrequency = 5.0f;  // 200ms

            // At first, it can be performed.
            Assert.That(this.fakeActionable.CanPerformAction, Is.True);

            this.fakeActionable.PerformAction();

            // After calling PerformAction(), it shouldn't be immediately possible to perform it again.
            Assert.That(this.fakeActionable.CanPerformAction, Is.False);

            Thread.Sleep(Math.Abs(unchecked((int)(1000.0f / this.fakeActionable.MaxFrequency))));

            // After waiting 200ms, the action can be performed again.
            Assert.That(this.fakeActionable.CanPerformAction, Is.True);
        }

        #region PerformAction

        [Test(Author = "aleab")]
        [Description("An Actionable's PerformAction() method invokes its Action's PerformAction() method exactly once.")]
        public void PerformActionTest_PerformActionCalledOnce()
        {
            this.fakeActionable.PerformAction();
            A.CallTo(() => this.fakeAction.PerformAction()).MustHaveHappened(1, Times.Exactly);
        }

        [Test(Author = "aleab")]
        [Description("If an Actionable has a null Action, it does not throw an exception when calling PerformAction().")]
        public void PerformActionTest_NullActionDoesNotThrowExceptions()
        {
            Actionable actionable = new Actionable();
            Assert.That(actionable.Action, Is.Null);
            Assert.DoesNotThrow(() => actionable.PerformAction());
        }

        [Test(Author = "aleab")]
        [Description("If the Action's PerformAction() method throws an exception, the Actionable's PerformAction() method should not.")]
        public void PerformActionTest_DoesNotThrowExceptionIfActionDoes()
        {
            A.CallTo(() => this.fakeAction.PerformAction()).Throws<Exception>();
            Assert.Throws<Exception>(() => this.fakeAction.PerformAction());
            Assert.DoesNotThrow(() => this.fakeActionable.PerformAction());
        }

        #endregion PerformAction

        #region Equals / GethashCode

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(ActionableData), nameof(ActionableData.EqualsTestCases))]
        public bool EqualsTest([NotNull] Actionable actionable1, Actionable actionable2)
        {
            return actionable1.Equals(actionable2);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(ActionableData), nameof(ActionableData.ObjectEqualsTestCases))]
        public bool EqualsTest_Object([NotNull] Actionable actionable, object obj)
        {
            return actionable.Equals(obj);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(ActionableData), nameof(ActionableData.GetHashCodeTestCases))]
        public bool GetHashCodeTest([NotNull] Actionable actionable1, [NotNull] Actionable actionable2)
        {
            return actionable1.GetHashCode() == actionable2.GetHashCode();
        }

        #endregion Equals / GethashCode

        public class ActionableData
        {
            public static IEnumerable<TestCaseData> EqualsTestCases
            {
                get
                {
                    IAction actionA = A.Fake<IAction>();
                    IAction actionB = A.Fake<IAction>();

                    A.CallTo(() => actionA.Equals(null)).Returns(false);
                    A.CallTo(() => actionB.Equals(null)).Returns(false);
                    A.CallTo(() => actionA.Equals(actionA)).Returns(true);
                    A.CallTo(() => actionA.Equals(actionB)).Returns(false);
                    A.CallTo(() => actionB.Equals(actionA)).Returns(false);
                    A.CallTo(() => actionB.Equals(actionB)).Returns(true);

                    Actionable nullAction1 = new Actionable(null);
                    Actionable nullAction2 = new Actionable(null);
                    Actionable withActionA1 = new Actionable(actionA);
                    Actionable withActionA2 = new Actionable(actionA);
                    Actionable withActionB1 = new Actionable(actionB);
                    Actionable withActionB2 = new Actionable(actionB);

                    // with null
                    yield return new TestCaseData(nullAction1, null).Returns(false).SetName("with null: nullAction1");
                    yield return new TestCaseData(nullAction2, null).Returns(false).SetName("with null: nullAction2");
                    yield return new TestCaseData(withActionA1, null).Returns(false).SetName("with null: withActionA1");
                    yield return new TestCaseData(withActionA2, null).Returns(false).SetName("with null: withActionA2");
                    yield return new TestCaseData(withActionB1, null).Returns(false).SetName("with null: withActionB1");
                    yield return new TestCaseData(withActionB2, null).Returns(false).SetName("with null: withActionB2");

                    // null action
                    yield return new TestCaseData(nullAction1, nullAction1).Returns(true).SetName("null action: nullAction1");
                    yield return new TestCaseData(nullAction1, nullAction2).Returns(true).SetName("null action: nullAction2");
                    yield return new TestCaseData(withActionA1, nullAction1).Returns(false).SetName("null action: withActionA1");
                    yield return new TestCaseData(withActionA2, nullAction1).Returns(false).SetName("null action: withActionA2");
                    yield return new TestCaseData(withActionB1, nullAction1).Returns(false).SetName("null action: withActionB1");
                    yield return new TestCaseData(withActionB2, nullAction1).Returns(false).SetName("null action: withActionB2");

                    // same action
                    yield return new TestCaseData(withActionA1, withActionA1).Returns(true).SetName("same action: withActionA1, withActionA1");
                    yield return new TestCaseData(withActionA1, withActionA2).Returns(true).SetName("same action: withActionA1, withActionA2");
                    yield return new TestCaseData(withActionB1, withActionB1).Returns(true).SetName("same action: withActionB1, withActionB1");
                    yield return new TestCaseData(withActionB1, withActionB2).Returns(true).SetName("same action: withActionB1, withActionB2");

                    // different action
                    yield return new TestCaseData(withActionA1, withActionB1).Returns(false).SetName("different action: withActionA1, withActionB1");
                    yield return new TestCaseData(withActionA1, withActionB2).Returns(false).SetName("different action: withActionA1, withActionB2");
                    yield return new TestCaseData(withActionB1, withActionA1).Returns(false).SetName("different action: withActionB1, withActionA1");
                    yield return new TestCaseData(withActionB1, withActionA2).Returns(false).SetName("different action: withActionB1, withActionA2");

                    // MaxFrequency doesn't matter
                    yield return new TestCaseData(new Actionable(actionA) { MaxFrequency = 1.0f }, new Actionable(actionA) { MaxFrequency = 2.0f })
                                .Returns(true).SetName("MaxFrequency doesn't matter: same action, different MaxFrequency");
                    yield return new TestCaseData(new Actionable(actionA), new Actionable(actionB))
                                .Returns(false).SetName("MaxFrequency doesn't matter: different action, same MaxFrequency");
                }
            }

            public static IEnumerable<TestCaseData> ObjectEqualsTestCases
            {
                get
                {
                    Actionable actionable = new Actionable();

                    foreach (var testCase in EqualsTestCases)
                    {
                        yield return testCase;
                    }

                    // different type
                    yield return new TestCaseData(actionable, "actionable").Returns(false).SetName("Different Type: string");
                }
            }

            public static IEnumerable<TestCaseData> GetHashCodeTestCases
            {
                get
                {
                    IAction actionA = A.Fake<IAction>();
                    IAction actionB = A.Fake<IAction>();

                    A.CallTo(() => actionA.GetHashCode()).Returns(1000);
                    A.CallTo(() => actionB.GetHashCode()).Returns(2000);

                    Actionable nullAction1 = new Actionable(null);
                    Actionable nullAction2 = new Actionable(null);
                    Actionable withActionA1 = new Actionable(actionA);
                    Actionable withActionA2 = new Actionable(actionA);
                    Actionable withActionB1 = new Actionable(actionB);
                    Actionable withActionB2 = new Actionable(actionB);

                    // null action
                    yield return new TestCaseData(nullAction1, nullAction1).Returns(true).SetName("null action: nullAction1");
                    yield return new TestCaseData(nullAction1, nullAction2).Returns(true).SetName("null action: nullAction2");
                    yield return new TestCaseData(withActionA1, nullAction1).Returns(false).SetName("null action: withActionA1");
                    yield return new TestCaseData(withActionA2, nullAction1).Returns(false).SetName("null action: withActionA2");
                    yield return new TestCaseData(withActionB1, nullAction1).Returns(false).SetName("null action: withActionB1");
                    yield return new TestCaseData(withActionB2, nullAction1).Returns(false).SetName("null action: withActionB2");

                    // same action
                    yield return new TestCaseData(withActionA1, withActionA1).Returns(true).SetName("same action: withActionA1, withActionA1");
                    yield return new TestCaseData(withActionA1, withActionA2).Returns(true).SetName("same action: withActionA1, withActionA2");
                    yield return new TestCaseData(withActionB1, withActionB1).Returns(true).SetName("same action: withActionB1, withActionB1");
                    yield return new TestCaseData(withActionB1, withActionB2).Returns(true).SetName("same action: withActionB1, withActionB2");

                    // different action
                    yield return new TestCaseData(withActionA1, withActionB1).Returns(false).SetName("different action: withActionA1, withActionB1");
                    yield return new TestCaseData(withActionA1, withActionB2).Returns(false).SetName("different action: withActionA1, withActionB2");
                    yield return new TestCaseData(withActionB1, withActionA1).Returns(false).SetName("different action: withActionB1, withActionA1");
                    yield return new TestCaseData(withActionB1, withActionA2).Returns(false).SetName("different action: withActionB1, withActionA2");

                    // MaxFrequency doesn't matter
                    yield return new TestCaseData(new Actionable(actionA) { MaxFrequency = 1.0f }, new Actionable(actionA) { MaxFrequency = 2.0f })
                                .Returns(true).SetName("MaxFrequency doesn't matter: same action, different MaxFrequency");
                    yield return new TestCaseData(new Actionable(actionA), new Actionable(actionB))
                                .Returns(false).SetName("MaxFrequency doesn't matter: different action, same MaxFrequency");
                }
            }
        }
    }
}