using NUnit.Framework;
using System.Collections;
using System.Windows.Input;
using Toastify.Core;
using Toastify.Model;
using MouseAction = Toastify.Core.MouseAction;

namespace Toastify.Tests.Model
{
    [TestFixture, TestOf(typeof(Hotkey))]
    public class HotkeyTest
    {
        [Test(Author = "aleab")]
        [TestCaseSource(typeof(HotkeyData), nameof(HotkeyData.IsValidTestCases))]
        public bool IsValidTest(Hotkey hotkey)
        {
            return hotkey.IsValid;
        }

        public class HotkeyData
        {
            public static IEnumerable IsValidTestCases
            {
                get
                {
                    yield return new TestCaseData(new Hotkey { Action = ToastifyAction.PlayPause, KeyOrButton = Key.None }).Returns(false);

                    // XButton1 / XButton2 don't require any modifier
                    yield return new TestCaseData(new Hotkey { Action = ToastifyAction.PlayPause, KeyOrButton = MouseAction.XButton1 }).Returns(true);
                    yield return new TestCaseData(new Hotkey { Action = ToastifyAction.PlayPause, KeyOrButton = MouseAction.XButton2 }).Returns(true);

                    // MWheelUp / MWheelUp require at least one modifier
                    yield return new TestCaseData(new Hotkey { Action = ToastifyAction.PlayPause, KeyOrButton = MouseAction.MWheelUp }).Returns(false);
                    yield return new TestCaseData(new Hotkey { Action = ToastifyAction.PlayPause, KeyOrButton = MouseAction.MWheelDown }).Returns(false);
                    yield return new TestCaseData(new Hotkey { Action = ToastifyAction.PlayPause, KeyOrButton = MouseAction.MWheelUp, Ctrl = true }).Returns(true);
                    yield return new TestCaseData(new Hotkey { Action = ToastifyAction.PlayPause, KeyOrButton = MouseAction.MWheelDown, Ctrl = true }).Returns(true);
                }
            }
        }
    }
}