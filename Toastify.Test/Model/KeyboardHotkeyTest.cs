using FakeItEasy;
using NUnit.Framework;
using System.Collections;
using System.Windows.Input;
using Toastify.Core;
using Toastify.Model;

namespace Toastify.Tests.Model
{
    [TestFixture, TestOf(typeof(KeyboardHotkey))]
    public class KeyboardHotkeyTest
    {
        [Test(Author = "aleab")]
        [TestCaseSource(typeof(KeyboardHotkeyData), nameof(KeyboardHotkeyData.IsValidTestCases))]
        public bool IsValidTest(KeyboardHotkey hotkey)
        {
            return hotkey.IsValid();
        }

        public class KeyboardHotkeyData
        {
            public static IEnumerable IsValidTestCases
            {
                get
                {
                    ToastifyAction fakeAction = A.Fake<ToastifyAction>();
                    A.CallTo(() => fakeAction.ToastifyActionEnum).Returns(ToastifyActionEnum.PlayPause);
                    A.CallTo(() => fakeAction.Name).Returns("Fake Action");

                    yield return new TestCaseData(new KeyboardHotkey { Action = fakeAction, Key = Key.None }).Returns(false).SetName("Key = Key.None");
                    yield return new TestCaseData(new KeyboardHotkey { Action = fakeAction, Key = null }).Returns(false).SetName("Key = null");

                    //// XButton1 / XButton2 don't require any modifier
                    //yield return new TestCaseData(new KeyboardHotkey { Action = ToastifyActionEnum.PlayPause, KeyOrButton = MouseAction.XButton1 }).Returns(true).SetName("XButton1 doesn't require modifiers");
                    //yield return new TestCaseData(new KeyboardHotkey { Action = ToastifyActionEnum.PlayPause, KeyOrButton = MouseAction.XButton2 }).Returns(true).SetName("XButton2 doesn't require modifiers");

                    //// MWheelUp / MWheelUp require at least one modifier
                    //yield return new TestCaseData(new KeyboardHotkey { Action = ToastifyActionEnum.PlayPause, KeyOrButton = MouseAction.MWheelUp }).Returns(false).SetName("MWheelUp requires at least one modifier; no modifier set");
                    //yield return new TestCaseData(new KeyboardHotkey { Action = ToastifyActionEnum.PlayPause, KeyOrButton = MouseAction.MWheelDown }).Returns(false).SetName("MWheelDown requires at least one modifier; no modifier set");
                    //yield return new TestCaseData(new KeyboardHotkey { Action = ToastifyActionEnum.PlayPause, KeyOrButton = MouseAction.MWheelUp, Ctrl = true }).Returns(true).SetName("MWheelUp requires at least one modifier; modifier set");
                    //yield return new TestCaseData(new KeyboardHotkey { Action = ToastifyActionEnum.PlayPause, KeyOrButton = MouseAction.MWheelDown, Ctrl = true }).Returns(true).SetName("MWheelDown requires at least one modifier; modifier set");

                    //// Not allowed MouseAction
                    //yield return new TestCaseData(new KeyboardHotkey { Action = ToastifyActionEnum.PlayPause, KeyOrButton = (MouseAction)404 }).Returns(false).SetName("Not allowed MouseAction");
                }
            }
        }
    }
}