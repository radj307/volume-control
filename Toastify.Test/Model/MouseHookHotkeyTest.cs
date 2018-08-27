using System;
using System.Collections;
using System.Windows.Input;
using FakeItEasy;
using JetBrains.Annotations;
using NUnit.Framework;
using Toastify.Model;
using ToastifyAPI.Logic;
using ToastifyAPI.Logic.Interfaces;
using ToastifyAPI.Model.Interfaces;
using MouseAction = ToastifyAPI.Core.MouseAction;

namespace Toastify.Tests.Model
{
    [TestFixture, TestOf(typeof(MouseHookHotkey))]
    public class MouseHookHotkeyTest
    {
        [Test(Author = "aleab")]
        [TestCaseSource(typeof(MouseHookHotkeyData), nameof(MouseHookHotkeyData.ActiveAndInitTestCases))]
        public void TestActiveAndInit([NotNull] MouseHookHotkey hotkey, Action<MouseHookHotkey> setup, Action<MouseHookHotkey> test, Action<MouseHookHotkey> tearDown)
        {
            setup?.Invoke(hotkey);
            if (test == null)
                Assert.That(true);
            else
                test.Invoke(hotkey);
            tearDown?.Invoke(hotkey);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(MouseHookHotkeyData), nameof(MouseHookHotkeyData.IsValidTestCases))]
        public bool TestIsValid([NotNull] MouseHookHotkey hotkey, Action<MouseHookHotkey> setup)
        {
            setup?.Invoke(hotkey);
            bool isValid = hotkey.IsValid();
            Assert.That(isValid == string.IsNullOrWhiteSpace(hotkey.InvalidReason));
            return isValid;
        }

        [Test(Author = "aleab")]
        public void TestGetVisitor()
        {
            var visitor = A.Fake<IMouseHookHotkeyVisitor>();
            var hotkey = new MouseHookHotkey { HotkeyVisitor = visitor };

            Assert.That(hotkey.GetVisitor(), Is.SameAs(visitor));
        }

        public class MouseHookHotkeyData
        {
            #region Static Fields and Properties

            public static IEnumerable IsValidTestCases
            {
                get
                {
                    var fakeAction = A.Fake<IAction>();
                    A.CallTo(() => fakeAction.Name).Returns("Fake Action");
                    A.CallTo(() => fakeAction.PerformAction()).DoesNothing();

                    var hotkey = new MouseHookHotkey(fakeAction);

                    yield return new TestCaseData(new MouseHookHotkey(hotkey) { MouseButton = MouseAction.XButton1 },
                                     new Action<MouseHookHotkey>(h => h.SetIsValid(false, "Invalid")))
                                .Returns(false).SetName("Valid hotkey, but isValid is already set to false: IsValid shouldn't perform any other validity check");

                    yield return new TestCaseData(new MouseHookHotkey(hotkey) { MouseButton = null }, null)
                                .Returns(false).SetName("MouseButton = null");

                    // XButton1 and XButton2 don't require any modifier
                    yield return new TestCaseData(new MouseHookHotkey(hotkey) { MouseButton = MouseAction.XButton1 }, null)
                                .Returns(true).SetName("XButton1 doesn't require modifiers");
                    yield return new TestCaseData(new MouseHookHotkey(hotkey) { MouseButton = MouseAction.XButton2 }, null)
                                .Returns(true).SetName("XButton2 doesn't require modifiers");

                    // MWheelUp and MWheelUp require at least one modifier
                    yield return new TestCaseData(new MouseHookHotkey(hotkey) { MouseButton = MouseAction.MWheelUp }, null)
                                .Returns(false).SetName("MWheelUp requires at least one modifier; no modifier set");
                    yield return new TestCaseData(new MouseHookHotkey(hotkey) { MouseButton = MouseAction.MWheelDown }, null)
                                .Returns(false).SetName("MWheelDown requires at least one modifier; no modifier set");
                    yield return new TestCaseData(new MouseHookHotkey(hotkey) { MouseButton = MouseAction.MWheelUp, Modifiers = ModifierKeys.Control }, null)
                                .Returns(true).SetName("MWheelUp requires at least one modifier; modifier set");
                    yield return new TestCaseData(new MouseHookHotkey(hotkey) { MouseButton = MouseAction.MWheelDown, Modifiers = ModifierKeys.Control }, null)
                                .Returns(true).SetName("MWheelDown requires at least one modifier; modifier set");

                    // Not allowed MouseAction
                    yield return new TestCaseData(new MouseHookHotkey(hotkey) { MouseButton = (MouseAction)404 }, null)
                                .Returns(false).SetName("Not allowed MouseAction");
                }
            }

            public static IEnumerable ActiveAndInitTestCases
            {
                get
                {
                    var action = A.Fake<IAction>();
                    A.CallTo(() => action.Name).Returns("Fake Action");
                    A.CallTo(() => action.Equals(action)).Returns(true);

                    var visitor = new MouseHookHotkeyVisitor();

                    yield return new TestCaseData(new MouseHookHotkey(action) { MouseButton = MouseAction.XButton1, HotkeyVisitor = visitor },
                            new Action<MouseHookHotkey>(h =>
                            {
                                h.SetIsValid(true, null);
                                h.Enabled = true;
                                h.Activate();
                            }),
                            new Action<MouseHookHotkey>(h => Assert.That(h.HotkeyVisitor.IsRegistered(h))),
                            new Action<MouseHookHotkey>(h =>
                            {
                                h.Deactivate();
                                h.HotkeyVisitor.UnregisterAll();
                            }))
                       .SetName("IsValid = true, Enabled = true, Active = true => mouse hotkey is registered");

                    yield return new TestCaseData(new MouseHookHotkey(action) { MouseButton = null, HotkeyVisitor = visitor },
                            new Action<MouseHookHotkey>(h =>
                            {
                                h.SetIsValid(true, null);
                                h.Enabled = true;
                                h.Activate();
                            }),
                            new Action<MouseHookHotkey>(h => Assert.That(h.HotkeyVisitor.IsRegistered(h), Is.False)),
                            new Action<MouseHookHotkey>(h =>
                            {
                                h.Deactivate();
                                h.HotkeyVisitor.UnregisterAll();
                            }))
                       .SetName("MouseButton = null => mouse hotkey is not registered");

                    yield return new TestCaseData(new MouseHookHotkey(action) { MouseButton = MouseAction.XButton1 },
                            new Action<MouseHookHotkey>(h =>
                            {
                                h.SetIsValid(true, null);
                                h.Enabled = true;
                                h.Activate();
                            }),
                            new Action<MouseHookHotkey>(h => Assert.That(h.Active, Is.False)),
                            new Action<MouseHookHotkey>(h => h.Deactivate()))
                       .SetName("HotkeyVisitor = null => mouse hotkey is not active");
                }
            }

            #endregion
        }
    }
}