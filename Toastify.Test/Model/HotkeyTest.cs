using System;
using System.Collections.Generic;
using System.Windows.Input;
using FakeItEasy;
using JetBrains.Annotations;
using NUnit.Framework;
using Toastify.Model;
using ToastifyAPI.Interop.Interfaces;
using ToastifyAPI.Logic.Interfaces;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Tests.Model
{
    [TestFixture, TestOf(typeof(Hotkey))]
    public class HotkeyTest
    {
        private FakeHotkey fakeHotkey;
        private IAction action, actionClone;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.action = A.Fake<IAction>();
            A.CallTo(() => this.action.Name).Returns("Fake Action");
            A.CallTo(() => this.action.PerformAction()).DoesNothing();
            A.CallTo(() => this.action.Equals(this.action)).Returns(true);

            this.actionClone = A.Fake<IAction>();
            A.CallTo(() => this.actionClone.Name).Returns("Fake Action");
            A.CallTo(() => this.actionClone.PerformAction()).DoesNothing();
            A.CallTo(() => this.actionClone.Equals(this.actionClone)).Returns(true);

            A.CallTo(() => this.action.Equals(this.actionClone)).Returns(true);
            A.CallTo(() => this.actionClone.Equals(this.action)).Returns(true);
            A.CallTo(() => this.action.Clone()).Returns(this.actionClone);
            A.CallTo(() => this.actionClone.Clone()).Returns(this.action);
        }

        [SetUp]
        public void SetUp()
        {
            this.fakeHotkey = new FakeHotkey(this.action);
            this.fakeHotkey.SetIsValid(true, null);
        }

        [TearDown]
        public void TearDown()
        {
            this.fakeHotkey.Dispose();
            this.fakeHotkey = null;
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(HotkeyData), nameof(HotkeyData.ConstructorsTestCases))]
        public void TestConstructors(Action test)
        {
            test?.Invoke();
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(HotkeyData), nameof(HotkeyData.ActiveAndInitTestCases))]
        public bool TestActiveAndInit([NotNull] FakeHotkey hotkey, Action<FakeHotkey> setup, Action<FakeHotkey> tearDown)
        {
            setup?.Invoke(hotkey);
            bool initialized = hotkey.Initialized;
            tearDown?.Invoke(hotkey);
            return initialized;
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(HotkeyData), nameof(HotkeyData.CanPerformActionTestCases))]
        public bool TestCanPerformAction([NotNull] FakeHotkey hotkey, Action<FakeHotkey> setup, Action<FakeHotkey> tearDown)
        {
            setup?.Invoke(hotkey);
            bool canPerformAction = hotkey.CanPerformAction;
            tearDown?.Invoke(hotkey);
            return canPerformAction;
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(HotkeyData), nameof(HotkeyData.ModifiersPressedTestCases))]
        public bool? TestAreModifiersPressed([NotNull] Hotkey hotkey)
        {
            if (hotkey.InputDevices == null)
            {
                Assert.Throws<InvalidOperationException>(() => hotkey.AreModifiersPressed());
                return null;
            }

            return hotkey.AreModifiersPressed();
        }

        [Test(Author = "aleab")]
        public void TestDispatch()
        {
            Assert.That(this.fakeHotkey.Dispatched, Is.False);
            this.fakeHotkey.Dispatch(null);
            Assert.That(this.fakeHotkey.Dispatched, Is.True);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(HotkeyData), nameof(HotkeyData.EqualsTestCases))]
        public bool TestEquals([NotNull] Hotkey hotkey1, Hotkey hotkey2)
        {
            return hotkey1.Equals(hotkey2);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(HotkeyData), nameof(HotkeyData.ObjectEqualsTestCases))]
        public bool TestEquals_Object([NotNull] Hotkey hotkey, object obj)
        {
            return hotkey.Equals(obj);
        }

        [Test(Author = "aleab")]
        public void TestClone()
        {
            this.fakeHotkey.Activate();
            Assert.That(this.fakeHotkey.Active, Is.True);

            var hotkeyClone = this.fakeHotkey.Clone() as Hotkey;
            Assert.Multiple(() =>
            {
                Assert.That(hotkeyClone, Is.Not.Null);
                Assert.That(hotkeyClone, Is.Not.SameAs(this.fakeHotkey));
                Assert.That(hotkeyClone, Is.EqualTo(this.fakeHotkey));

                Assert.That(hotkeyClone.Active, Is.False);
                if (hotkeyClone.Action != null)
                    Assert.That(hotkeyClone.Action, Is.SameAs(this.fakeHotkey.Action)); // Actions should be the same
            });
        }

        [Test(Author = "aleab")]
        public void TestDispose()
        {
            this.fakeHotkey.Activate();
            Assert.That(this.fakeHotkey.Active, Is.True);

            this.fakeHotkey.Dispose();
            Assert.That(this.fakeHotkey.Active, Is.False);
        }

        [Test(Author = "aleab")]
        public void TestToString()
        {
            string toString = this.fakeHotkey.ToString();
            Assert.That(toString, Is.Not.Null);
        }

        public class HotkeyData
        {
            #region Static Fields and Properties

            public static IEnumerable<TestCaseData> ConstructorsTestCases
            {
                get
                {
                    yield return new TestCaseData(
                        new Action(() =>
                        {
                            // ReSharper disable once AssignNullToNotNullAttribute ObjectCreationAsStatement
                            Assert.Throws<ArgumentNullException>(() => new FakeHotkey((IHotkey)null));
                        })).SetName("ctor(IHotkey) | null");
                    yield return new TestCaseData(
                        new Action(() =>
                        {
                            Hotkey h = new FakeHotkey
                            {
                                Enabled = true,
                                Modifiers = ModifierKeys.Alt,
                                MaxFrequency = 3.0f
                            };
                            Hotkey hotkey = new FakeHotkey(h);
                            Assert.Multiple(() =>
                            {
                                Assert.That(hotkey.MaxFrequency, Is.EqualTo(h.MaxFrequency));
                                Assert.That(hotkey.Enabled, Is.EqualTo(h.Enabled));
                                Assert.That(hotkey.Modifiers, Is.EqualTo(h.Modifiers));
                            });
                        })).SetName("ctor(IHotkey) | Hotkey");
                    yield return new TestCaseData(
                        new Action(() =>
                        {
                            var action = A.Fake<IAction>();
                            A.CallTo(() => action.Name).ReturnsLazily(() => "Fake Action");
                            A.CallTo(() => action.PerformAction()).DoesNothing();

                            Hotkey hotkey = new FakeHotkey(action);
                            Assert.That(hotkey.Action, Is.EqualTo(action));
                        })).SetName("ctor(IAction)");
                }
            }

            public static IEnumerable<TestCaseData> ActiveAndInitTestCases
            {
                get
                {
                    var hotkey = new FakeHotkey();

                    yield return new TestCaseData(hotkey,
                                     new Action<FakeHotkey>(h =>
                                     {
                                         h.SetIsValid(false, null);
                                         h.Enabled = false;
                                         h.Activate();
                                     }), new Action<FakeHotkey>(h => h.Deactivate()))
                                .Returns(false).SetName("IsValid = false, Enabled = false");
                    yield return new TestCaseData(hotkey,
                                     new Action<FakeHotkey>(h =>
                                     {
                                         h.SetIsValid(true, null);
                                         h.Enabled = false;
                                         h.Activate();
                                     }), new Action<FakeHotkey>(h => h.Deactivate()))
                                .Returns(false).SetName("IsValid = true, Enabled = false");
                    yield return new TestCaseData(hotkey,
                                     new Action<FakeHotkey>(h =>
                                     {
                                         h.SetIsValid(true, null);
                                         h.Enabled = true;
                                         h.Activate();
                                     }), new Action<FakeHotkey>(h => h.Deactivate()))
                                .Returns(true).SetName("IsValid = true, Enabled = true");
                }
            }

            public static IEnumerable<TestCaseData> CanPerformActionTestCases
            {
                get
                {
                    var hotkey = new FakeHotkey();

                    yield return new TestCaseData(hotkey,
                                     new Action<FakeHotkey>(h =>
                                     {
                                         h.SetIsValid(false, null);
                                         h.Enabled = false;
                                         h.Deactivate();
                                     }), null)
                                .Returns(false).SetName("IsValid = false, Enabled = false, Active = false");
                    yield return new TestCaseData(hotkey,
                                     new Action<FakeHotkey>(h =>
                                     {
                                         h.SetIsValid(true, null);
                                         h.Enabled = false;
                                         h.Deactivate();
                                     }), null)
                                .Returns(false).SetName("IsValid = true, Enabled = false, Active = false");
                    yield return new TestCaseData(hotkey,
                                     new Action<FakeHotkey>(h =>
                                     {
                                         h.SetIsValid(true, null);
                                         h.Enabled = true;
                                         h.Deactivate();
                                     }), null)
                                .Returns(false).SetName("IsValid = true, Enabled = true, Active = false");
                    yield return new TestCaseData(hotkey,
                                     new Action<FakeHotkey>(h =>
                                     {
                                         h.SetIsValid(true, null);
                                         h.Enabled = true;
                                         h.Activate();
                                     }), new Action<FakeHotkey>(h => h.Deactivate()))
                                .Returns(true).SetName("IsValid = true, Enabled = true, Active = true");
                }
            }

            public static IEnumerable<TestCaseData> ModifiersPressedTestCases
            {
                get
                {
                    var action = A.Fake<IAction>();
                    A.CallTo(() => action.Name).Returns("Fake Action");
                    A.CallTo(() => action.PerformAction()).DoesNothing();
                    A.CallTo(() => action.Equals(action)).Returns(true);

                    var inputDevices = A.Fake<IInputDevices>();
                    A.CallTo(() => inputDevices.IsPressed(Key.LeftCtrl)).Returns(true);
                    A.CallTo(() => inputDevices.IsPressed(Key.RightCtrl)).Returns(false);
                    A.CallTo(() => inputDevices.IsPressed(Key.LeftAlt)).Returns(false);
                    A.CallTo(() => inputDevices.IsPressed(Key.RightAlt)).Returns(false);

                    A.CallTo(() => inputDevices.ArePressed(ModifierKeys.None)).Returns(false);
                    A.CallTo(() => inputDevices.ArePressed(ModifierKeys.Control))
                     .ReturnsLazily(() => inputDevices.IsPressed(Key.LeftCtrl) || inputDevices.IsPressed(Key.RightCtrl));
                    A.CallTo(() => inputDevices.ArePressed(ModifierKeys.Alt))
                     .ReturnsLazily(() => inputDevices.IsPressed(Key.LeftAlt) || inputDevices.IsPressed(Key.RightAlt));
                    A.CallTo(() => inputDevices.ArePressed(ModifierKeys.Control | ModifierKeys.Alt))
                     .ReturnsLazily(() => inputDevices.ArePressed(ModifierKeys.Control) && inputDevices.ArePressed(ModifierKeys.Alt));

                    var hotkey = new FakeHotkey
                    {
                        Enabled = true,
                        Modifiers = ModifierKeys.None,
                        MaxFrequency = 1.0f,
                        Action = action,
                        InputDevices = inputDevices
                    };

                    yield return new TestCaseData(hotkey)
                                .Returns(false).SetName("LeftCtrl pressed: ModifierKeys.None");
                    yield return new TestCaseData(new FakeHotkey(hotkey) { Modifiers = ModifierKeys.Control })
                                .Returns(true).SetName("LeftCtrl pressed: ModifierKeys.Control");
                    yield return new TestCaseData(new FakeHotkey(hotkey) { Modifiers = ModifierKeys.Alt })
                                .Returns(false).SetName("LeftCtrl pressed: ModifierKeys.Alt");
                    yield return new TestCaseData(new FakeHotkey(hotkey) { Modifiers = ModifierKeys.Control | ModifierKeys.Alt })
                                .Returns(false).SetName("LeftCtrl pressed: ModifierKeys.Control | ModifierKeys.Alt");

                    yield return new TestCaseData(new FakeHotkey(hotkey) { InputDevices = null })
                                .Returns(null).SetName("InputDevices = null");
                }
            }

            public static IEnumerable<TestCaseData> EqualsTestCases
            {
                get
                {
                    var action = A.Fake<IAction>();
                    A.CallTo(() => action.Name).Returns("Fake Action");
                    A.CallTo(() => action.PerformAction()).DoesNothing();
                    A.CallTo(() => action.Equals(action)).Returns(true);

                    var hotkey = new FakeHotkey
                    {
                        Enabled = true,
                        Modifiers = ModifierKeys.None,
                        MaxFrequency = 1.0f,
                        Action = action
                    };

                    yield return new TestCaseData(hotkey, null).Returns(false).SetName("with null");
                    yield return new TestCaseData(hotkey, hotkey).Returns(true).SetName("same");
                    yield return new TestCaseData(hotkey, new FakeHotkey(hotkey) { Enabled = false }).Returns(true).SetName($"equal: different {nameof(Hotkey.Enabled)}");
                    yield return new TestCaseData(hotkey, new FakeHotkey(hotkey) { Modifiers = ModifierKeys.Alt }).Returns(false).SetName($"not equal: different {nameof(Hotkey.Modifiers)}");
                    yield return new TestCaseData(hotkey, new FakeHotkey(hotkey) { MaxFrequency = 2.0f }).Returns(false).SetName($"not equal: different {nameof(Hotkey.MaxFrequency)}");
                    yield return new TestCaseData(hotkey, new FakeHotkey(hotkey) { Action = A.Fake<IAction>() }).Returns(false).SetName($"not equal: different {nameof(Hotkey.Action)}");
                }
            }

            public static IEnumerable<TestCaseData> ObjectEqualsTestCases
            {
                get
                {
                    foreach (TestCaseData testCase in EqualsTestCases)
                    {
                        yield return testCase;
                    }

                    var action = A.Fake<IAction>();
                    A.CallTo(() => action.Name).Returns("Fake Action");
                    A.CallTo(() => action.PerformAction()).DoesNothing();
                    A.CallTo(() => action.Equals(action)).Returns(true);

                    var hotkey = new FakeHotkey
                    {
                        Enabled = true,
                        Modifiers = ModifierKeys.None,
                        MaxFrequency = 1.0f,
                        Action = action
                    };

                    yield return new TestCaseData(hotkey, string.Empty).Returns(false).SetName("not equal: different object type");
                }
            }

            #endregion
        }

        public class FakeHotkey : Hotkey
        {
            private bool _isValid;
            private bool _dispatched;

            #region Public Properties

            public override string HumanReadableKey { get; }

            public bool Initialized { get; private set; }

            public bool Dispatched
            {
                get
                {
                    bool b = this._dispatched;
                    this._dispatched = false;
                    return b;
                }
                private set { this._dispatched = value; }
            }

            #endregion

            public FakeHotkey()
            {
            }

            public FakeHotkey(IAction action) : base(action)
            {
            }

            public FakeHotkey([NotNull] IHotkey hotkey) : base(hotkey)
            {
            }

            protected override void InitInternal()
            {
                this.Initialized = true;
            }

            public override bool IsValid()
            {
                return this._isValid;
            }

            internal override void SetIsValid(bool isValid, string invalidReason)
            {
                this._isValid = isValid;
            }

            public override IHotkeyVisitor GetVisitor()
            {
                throw new NotImplementedException();
            }

            protected override void DispatchInternal(IHotkeyVisitor hotkeyVisitor)
            {
                this.Dispatched = true;
            }

            protected override void DisposeInternal()
            {
                this.Initialized = false;
            }
        }
    }
}