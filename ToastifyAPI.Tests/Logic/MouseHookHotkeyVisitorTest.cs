using System;
using System.Collections.Generic;
using System.Windows.Input;
using FakeItEasy;
using NUnit.Framework;
using ToastifyAPI.Core;
using ToastifyAPI.Interop.Interfaces;
using ToastifyAPI.Logic;
using ToastifyAPI.Logic.Interfaces;
using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Tests.Logic
{
    [TestFixture, TestOf(typeof(MouseHookHotkeyVisitor))]
    public class MouseHookHotkeyVisitorTest
    {
        private MouseHookHotkeyVisitor visitor;
        private IMouseHookHotkey fakeHotkey;

        [OneTimeSetUp]
        public void SetUp()
        {
            this.visitor = new MouseHookHotkeyVisitor();
            this.fakeHotkey = A.Fake<IMouseHookHotkey>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            this.visitor?.UnregisterAll();
        }

        [Test(Author = "aleab")]
        public void TestVisit()
        {
            this.visitor.Visit(this.fakeHotkey);
            A.CallTo(() => this.fakeHotkey.PerformAction()).MustHaveHappenedOnceExactly();
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(MouseHookHotkeyVisitorData), nameof(MouseHookHotkeyVisitorData.RegistrationTestCases))]
        public bool? TestRegistration(IMouseHookHotkey hotkey, Action<MouseHookHotkeyVisitor, IMouseHookHotkey> setup, Type exceptionType)
        {
            if (exceptionType != null)
            {
                Assert.Multiple(() =>
                {
                    if (setup != null)
                        Assert.Throws(exceptionType, () => setup.Invoke(this.visitor, hotkey));
                    Assert.Throws(exceptionType, () => this.visitor.IsRegistered(hotkey));
                });
                return null;
            }

            setup?.Invoke(this.visitor, hotkey);
            return this.visitor.IsRegistered(hotkey);
        }

        public class MouseHookHotkeyVisitorData
        {
            #region Static Fields and Properties

            private static readonly MouseHookHotkeyVisitor visitor = new MouseHookHotkeyVisitor();

            public static IEnumerable<TestCaseData> RegistrationTestCases
            {
                get
                {
                    // These test cases cover IsRegistered(), RegisterHook() and UnregisterHook(): IsRegistered() is used to test the other two methods.

                    var registerAction = new Action<MouseHookHotkeyVisitor, IMouseHookHotkey>((v, h) => v.RegisterHook(h));
                    var unregisterAction = new Action<MouseHookHotkeyVisitor, IMouseHookHotkey>((v, h) => v.UnregisterHook(h));

                    // NULL HOTKEY
                    yield return new TestCaseData(null, null, typeof(ArgumentNullException))
                                .Returns(null).SetName("Null hotkey | Not registered");
                    yield return new TestCaseData(null, registerAction, typeof(ArgumentNullException))
                                .Returns(null).SetName("Null hotkey | Registered");
                    yield return new TestCaseData(null, unregisterAction, typeof(ArgumentNullException))
                                .Returns(null).SetName("Null hotkey | Unregistered");

                    // INVALID HOTKEY
                    var invalidHotkey = A.Fake<IMouseHookHotkey>();
                    A.CallTo(() => invalidHotkey.MouseButton).Returns(null);
                    A.CallTo(() => invalidHotkey.IsValid()).Returns(false);

                    yield return new TestCaseData(invalidHotkey, null, null)
                                .Returns(false).SetName("Invalid hotkey | Not registered");
                    yield return new TestCaseData(invalidHotkey, registerAction, null)
                                .Returns(false).SetName("Invalid hotkey | Registered");
                    yield return new TestCaseData(invalidHotkey, unregisterAction, null)
                                .Returns(false).SetName("Invalid hotkey | Unregistered");

                    // VALID HOTKEY
                    IMouseHookHotkey validHotkey = new ValidMouseHookHotkey();

                    yield return new TestCaseData(validHotkey, null, null)
                                .Returns(false).SetName("Valid hotkey | Not registered");
                    yield return new TestCaseData(validHotkey, registerAction, null)
                                .Returns(true).SetName("Valid hotkey | Registered");
                    yield return new TestCaseData(validHotkey, unregisterAction, null)
                                .Returns(false).SetName("Valid hotkey | Unregistered");
                }
            }

            #endregion
        }

        private class ValidMouseHookHotkey : IMouseHookHotkey
        {
            private readonly IAction _action;

            #region Public Properties

            public IAction Action
            {
                get { return this._action; }
                set { }
            }

            public float MaxFrequency { get; set; }

            public IInputDevices InputDevices { get; set; }
            public bool Enabled { get; set; }
            public bool Active { get; }
            public ModifierKeys Modifiers { get; set; }

            public MouseAction? MouseButton
            {
                get { return MouseAction.XButton1; }
                set { }
            }

            #endregion

            public ValidMouseHookHotkey()
            {
                this._action = A.Fake<IAction>();
                A.CallTo(() => this._action.Name).Returns("Fake Action");
                A.CallTo(() => this._action.PerformAction()).DoesNothing();
                A.CallTo(() => this._action.Equals(this._action)).Returns(true);

                this.Active = false;
            }

            public void PerformAction()
            {
                this.Action.PerformAction();
            }

            public bool IsValid()
            {
                return true;
            }

            public bool AreModifiersPressed()
            {
                throw new NotImplementedException();
            }

            public void Activate()
            {
            }

            public void Deactivate()
            {
            }

            public void Dispatch(IHotkeyVisitor visitor)
            {
            }

            public object Clone()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
            }
        }
    }
}