using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using NUnit.Framework;
using Toastify.Core;
using Toastify.Model;

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
                    
                    yield return new TestCaseData(new Hotkey { Action = ToastifyAction.PlayPause, KeyOrButton = MouseButton.XButton1 }).Returns(true);
                    yield return new TestCaseData(new Hotkey { Action = ToastifyAction.PlayPause, KeyOrButton = MouseButton.XButton2 }).Returns(true);
                    yield return new TestCaseData(new Hotkey { Action = ToastifyAction.PlayPause, KeyOrButton = MouseButton.Left }).Returns(false);
                    yield return new TestCaseData(new Hotkey { Action = ToastifyAction.PlayPause, KeyOrButton = MouseButton.Middle }).Returns(false);
                    yield return new TestCaseData(new Hotkey { Action = ToastifyAction.PlayPause, KeyOrButton = MouseButton.Right }).Returns(false);
                }
            }
        }
    }
}