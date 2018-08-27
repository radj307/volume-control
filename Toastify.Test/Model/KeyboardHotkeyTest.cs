using System;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Input;
using FakeItEasy;
using JetBrains.Annotations;
using ManagedWinapi;
using NUnit.Framework;
using Toastify.Model;
using ToastifyAPI.Helpers;
using ToastifyAPI.Logic.Interfaces;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Tests.Model
{
    [TestFixture, TestOf(typeof(KeyboardHotkey))]
    public class KeyboardHotkeyTest
    {
        [Test(Author = "aleab")]
        [TestCaseSource(typeof(KeyboardHotkeyData), nameof(KeyboardHotkeyData.ActiveAndInitTestCases))]
        public void TestActiveAndInit([NotNull] KeyboardHotkey hotkey, Action<KeyboardHotkey> setup, Action<KeyboardHotkey> test, Action<KeyboardHotkey> tearDown)
        {
            setup?.Invoke(hotkey);
            if (test == null)
                Assert.That(true);
            else
                test.Invoke(hotkey);
            tearDown?.Invoke(hotkey);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(KeyboardHotkeyData), nameof(KeyboardHotkeyData.IsValidTestCases))]
        public bool TestIsValid([NotNull] KeyboardHotkey hotkey, Action<KeyboardHotkey> setup)
        {
            setup?.Invoke(hotkey);
            return hotkey.IsValid();
        }

        [Test(Author = "aleab")]
        public void TestGetVisitor()
        {
            var visitor = A.Fake<IKeyboardHotkeyVisitor>();
            var hotkey = new KeyboardHotkey { HotkeyVisitor = visitor };

            Assert.That(hotkey.GetVisitor(), Is.SameAs(visitor));
        }

        public class KeyboardHotkeyData
        {
            #region Static Fields and Properties

            public static IEnumerable IsValidTestCases
            {
                get
                {
                    var fakeAction = A.Fake<IAction>();
                    A.CallTo(() => fakeAction.Name).Returns("Fake Action");
                    A.CallTo(() => fakeAction.PerformAction()).DoesNothing();

                    var hotkey = new KeyboardHotkey(fakeAction);

                    yield return new TestCaseData(new KeyboardHotkey(hotkey) { Key = Key.Left }, null)
                                .Returns(true).SetName("Valid key");
                    yield return new TestCaseData(new KeyboardHotkey(hotkey) { Key = Key.Left },
                                     new Action<KeyboardHotkey>(h => h.SetIsValid(false, "Invalid")))
                                .Returns(false).SetName("Valid key, already set invalid: IsValid shouldn't perform any other validity check");

                    yield return new TestCaseData(new KeyboardHotkey(hotkey) { Key = Key.None }, null)
                                .Returns(false).SetName("Key = Key.None");
                    yield return new TestCaseData(new KeyboardHotkey(hotkey) { Key = null }, null)
                                .Returns(false).SetName("Key = null");
                }
            }

            public static IEnumerable ActiveAndInitTestCases
            {
                get
                {
                    var action = A.Fake<IAction>();
                    A.CallTo(() => action.Name).Returns("Fake Action");

                    var globalHotkey = new FakeGlobalHotkey();

                    yield return new TestCaseData(new KeyboardHotkey(globalHotkey) { Key = Key.Left, Modifiers = ModifierKeys.Alt, Action = action },
                            new Action<KeyboardHotkey>(h =>
                            {
                                h.SetIsValid(true, null);
                                h.Enabled = true;
                                h.Activate();
                            }),
                            new Action<KeyboardHotkey>(h =>
                            {
                                ILockedGlobalHotkey gh = h.GlobalHotkey;
                                Assert.Multiple(() =>
                                {
                                    Assert.That(h.IsValid);
                                    Assert.That(h.Key.HasValue);

                                    Assert.That(gh.Enabled);
                                    Assert.That(gh.KeyCode, Is.EqualTo(h.Key.Value.ConvertToWindowsFormsKeys()));
                                    Assert.That(gh.Ctrl, Is.EqualTo(h.Modifiers.HasFlag(ModifierKeys.Control)));
                                    Assert.That(gh.Alt, Is.EqualTo(h.Modifiers.HasFlag(ModifierKeys.Alt)));
                                    Assert.That(gh.Shift, Is.EqualTo(h.Modifiers.HasFlag(ModifierKeys.Shift)));
                                    Assert.That(gh.WindowsKey, Is.EqualTo(h.Modifiers.HasFlag(ModifierKeys.Windows)));
                                });
                            }),
                            new Action<KeyboardHotkey>(h => h.Deactivate()))
                       .SetName("IsValid = true, Enabled = true, Active = true => GlobalHotkey is correctly enabled");

                    yield return new TestCaseData(new KeyboardHotkey(globalHotkey) { Key = Key.Left, Modifiers = ModifierKeys.Alt, Action = action },
                            new Action<KeyboardHotkey>(h =>
                            {
                                globalHotkey.EnabledAction = () => throw new HotkeyAlreadyInUseException();

                                h.SetIsValid(true, null);
                                h.Enabled = true;
                                h.Activate();
                            }),
                            new Action<KeyboardHotkey>(h =>
                            {
                                Assert.Multiple(() =>
                                {
                                    Assert.That(h.IsValid, Is.False);
                                    Assert.That(string.IsNullOrWhiteSpace(h.InvalidReason), Is.False);
                                });
                            }),
                            new Action<KeyboardHotkey>(h =>
                            {
                                globalHotkey.EnabledAction = null;
                                h.Deactivate();
                            }))
                       .SetName("HotkeyAlreadyInUseException => hotkey is not valid and has an InvalidReason");
                }
            }

            #endregion
        }

        public class FakeGlobalHotkey : IGlobalHotkey
        {
            private bool _enabled;

            #region Public Properties

            public bool Enabled
            {
                get { return this._enabled; }
                set
                {
                    if (value && !this._enabled)
                        this.EnabledAction?.Invoke();
                    else if (!value && this._enabled)
                        this.DisabledAction?.Invoke();

                    this._enabled = value;
                }
            }

            public Keys KeyCode { get; set; }
            public bool Ctrl { get; set; }
            public bool Alt { get; set; }
            public bool Shift { get; set; }
            public bool WindowsKey { get; set; }

            public Action EnabledAction { get; set; }
            public Action DisabledAction { get; set; }

            #endregion

            #region Events

            public event EventHandler HotkeyPressed;

            #endregion
        }
    }
}