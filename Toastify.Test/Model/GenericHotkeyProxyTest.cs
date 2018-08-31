using System;
using System.Collections;
using System.Threading;
using System.Windows.Input;
using FakeItEasy;
using JetBrains.Annotations;
using NUnit.Framework;
using Toastify.Core;
using Toastify.Model;
using ToastifyAPI.Model.Interfaces;
using MouseAction = ToastifyAPI.Core.MouseAction;

namespace Toastify.Tests.Model
{
    [TestFixture, TestOf(typeof(GenericHotkeyProxy))]
    public class GenericHotkeyProxyTest
    {
        [Test(Author = "aleab"), Apartment(ApartmentState.STA)]
        [TestCaseSource(typeof(GenericHotkeyProxyData), nameof(GenericHotkeyProxyData.ConstructorsTestCases))]
        public void TestConstructors(Action test)
        {
            Test(test);
        }

        [Test(Author = "aleab"), Apartment(ApartmentState.STA)]
        [TestCaseSource(typeof(GenericHotkeyProxyData), nameof(GenericHotkeyProxyData.ModifiersTestCases))]
        public void TestModifiers(Action test)
        {
            Test(test);
        }

        [Test(Author = "aleab"), Apartment(ApartmentState.STA)]
        [TestCaseSource(typeof(GenericHotkeyProxyData), nameof(GenericHotkeyProxyData.IsValidTestCases))]
        public void TestIsValid(Action test)
        {
            Test(test);
        }

        [Test(Author = "aleab"), Apartment(ApartmentState.STA)]
        [TestCaseSource(typeof(GenericHotkeyProxyData), nameof(GenericHotkeyProxyData.SetActivatorTestCases))]
        public void TestSetActivator(Action test)
        {
            Test(test);
        }

        [Test(Author = "aleab"), Apartment(ApartmentState.STA)]
        [TestCaseSource(typeof(GenericHotkeyProxyData), nameof(GenericHotkeyProxyData.IsAlreadyInUseByTestCases))]
        public bool TestIsAlreadyInUseBy([NotNull] Func<GenericHotkeyProxy> getHotkey, Func<GenericHotkeyProxy> getArg)
        {
            return getHotkey.Invoke().IsAlreadyInUseBy(getArg?.Invoke());
        }

        #region Static Members

        private static void Test(Action test)
        {
            if (test != null)
                test.Invoke();
            else
                Assert.That(true);
        }

        #endregion

        public class GenericHotkeyProxyData
        {
            #region Static Fields and Properties

            public static IEnumerable ConstructorsTestCases
            {
                get
                {
                    yield return new TestCaseData(new Action(() =>
                    {
                        var genericHotkeyProxy = new GenericHotkeyProxy();

                        // Undefined at first
                        Assert.Multiple(() =>
                        {
                            Assert.That(genericHotkeyProxy.Type, Is.EqualTo(HotkeyType.Undefined));
                            Assert.That(genericHotkeyProxy.Hotkey, Is.Null);
                        });

                        // Changing to HotkeyType.Keyboard changes the type of the underlying Hotkey
                        genericHotkeyProxy.Type = HotkeyType.Keyboard;
                        Assert.Multiple(() =>
                        {
                            using (var kh = new KeyboardHotkey())
                            {
                                Assert.That(genericHotkeyProxy.Type, Is.EqualTo(HotkeyType.Keyboard));
                                Assert.That(genericHotkeyProxy.Hotkey, Is.AssignableTo(typeof(IKeyboardHotkey)));
                                Assert.That(genericHotkeyProxy.Hotkey, Is.EqualTo(kh));
                            }
                        });

                        // Changing to HotkeyType.MouseHook changes the type of the underlying Hotkey
                        genericHotkeyProxy.Type = HotkeyType.MouseHook;
                        Assert.Multiple(() =>
                        {
                            using (var mh = new MouseHookHotkey())
                            {
                                Assert.That(genericHotkeyProxy.Type, Is.EqualTo(HotkeyType.MouseHook));
                                Assert.That(genericHotkeyProxy.Hotkey, Is.AssignableTo(typeof(IMouseHookHotkey)));
                                Assert.That(genericHotkeyProxy.Hotkey, Is.EqualTo(mh));
                            }
                        });
                    })).SetName("ctor()");

                    yield return new TestCaseData(new Action(() =>
                    {
                        Assert.Multiple(() =>
                        {
                            using (var kbHotkey = new KeyboardHotkey(FakeAction) { Key = Key.Left, Modifiers = ModifierKeys.Control, MaxFrequency = 10.0f })
                            {
                                var genericHotkeyProxy = new GenericHotkeyProxy(kbHotkey);

                                Assert.That(genericHotkeyProxy.Type, Is.EqualTo(HotkeyType.Keyboard));
                                Assert.That(genericHotkeyProxy.Hotkey, Is.TypeOf<KeyboardHotkey>());
                                Assert.That(genericHotkeyProxy.Hotkey, Is.Not.SameAs(kbHotkey));
                                Assert.That(genericHotkeyProxy.Hotkey, Is.EqualTo(kbHotkey));
                            }
                        });
                    })).SetName("ctor(KeyboardHotkey)");

                    yield return new TestCaseData(new Action(() =>
                    {
                        Assert.Multiple(() =>
                        {
                            using (var mhHotkey = new MouseHookHotkey(FakeAction) { MouseButton = MouseAction.XButton1, Modifiers = ModifierKeys.Control, MaxFrequency = 10.0f })
                            {
                                var genericHotkeyProxy = new GenericHotkeyProxy(mhHotkey);

                                Assert.That(genericHotkeyProxy.Type, Is.EqualTo(HotkeyType.MouseHook));
                                Assert.That(genericHotkeyProxy.Hotkey, Is.TypeOf<MouseHookHotkey>());
                                Assert.That(genericHotkeyProxy.Hotkey, Is.Not.SameAs(mhHotkey));
                                Assert.That(genericHotkeyProxy.Hotkey, Is.EqualTo(mhHotkey));
                            }
                        });
                    })).SetName("ctor(MouseHookHotkey)");
                }
            }

            public static IEnumerable ModifiersTestCases
            {
                get
                {
                    yield return new TestCaseData(new Action(() =>
                    {
                        using (Hotkey hotkey = new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Shift })
                        {
                            var genericHotkeyProxy = new GenericHotkeyProxy(hotkey);

                            Assert.That(genericHotkeyProxy.Alt, Is.False);
                            Assert.That(genericHotkeyProxy.Ctrl, Is.True);
                            Assert.That(genericHotkeyProxy.Shift, Is.True);
                            Assert.That(genericHotkeyProxy.Win, Is.False);
                        }
                    })).SetName("get() | Same as the underlying hotkey");

                    yield return new TestCaseData(new Action(() =>
                    {
                        using (var kbHotkey = new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Shift })
                        {
                            var genericHotkeyProxy = new GenericHotkeyProxy(kbHotkey);
                            Assert.That(genericHotkeyProxy.Type, Is.EqualTo(HotkeyType.Keyboard));

                            genericHotkeyProxy.Type = HotkeyType.MouseHook;
                            Assert.Multiple(() =>
                            {
                                Assert.That(genericHotkeyProxy.Alt, Is.False);
                                Assert.That(genericHotkeyProxy.Ctrl, Is.True);
                                Assert.That(genericHotkeyProxy.Shift, Is.True);
                                Assert.That(genericHotkeyProxy.Win, Is.False);
                            });
                        }
                    })).SetName("get() | The underlying hotkeys share the same modifiers");

                    yield return new TestCaseData(new Action(() =>
                    {
                        using (var hotkey = new KeyboardHotkey())
                        {
#pragma warning disable IDE0017 // Simplify object initialization
                            // ReSharper disable once UseObjectOrCollectionInitializer
                            var genericHotkeyProxy = new GenericHotkeyProxy(hotkey);

                            genericHotkeyProxy.Alt = true;
                            genericHotkeyProxy.Ctrl = true;
                            genericHotkeyProxy.Shift = true;
                            genericHotkeyProxy.Win = true;

                            var kbHotkey = genericHotkeyProxy.Hotkey as IKeyboardHotkey;
                            Assert.Multiple(() =>
                            {
                                Assert.That(kbHotkey, Is.Not.Null);
                                Assert.That(kbHotkey.Modifiers.HasFlag(ModifierKeys.Alt));
                                Assert.That(kbHotkey.Modifiers.HasFlag(ModifierKeys.Control));
                                Assert.That(kbHotkey.Modifiers.HasFlag(ModifierKeys.Shift));
                                Assert.That(kbHotkey.Modifiers.HasFlag(ModifierKeys.Windows));
                            });

                            genericHotkeyProxy.Type = HotkeyType.MouseHook;
                            var mhHotkey = genericHotkeyProxy.Hotkey as IMouseHookHotkey;
                            Assert.Multiple(() =>
                            {
                                Assert.That(mhHotkey, Is.Not.Null);
                                Assert.That(mhHotkey.Modifiers.HasFlag(ModifierKeys.Alt));
                                Assert.That(mhHotkey.Modifiers.HasFlag(ModifierKeys.Control));
                                Assert.That(mhHotkey.Modifiers.HasFlag(ModifierKeys.Shift));
                                Assert.That(mhHotkey.Modifiers.HasFlag(ModifierKeys.Windows));
                            });
#pragma warning restore IDE0017 // Simplify object initialization
                        }
                    })).SetName("set() | Changing a modifier changes it for every underlying hotkey");

                    yield return new TestCaseData(new Action(() =>
                    {
                        using (Hotkey hotkey = new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Shift })
                        {
#pragma warning disable IDE0017 // Simplify object initialization
                            // ReSharper disable once UseObjectOrCollectionInitializer
                            var genericHotkeyProxy = new GenericHotkeyProxy(hotkey);

                            genericHotkeyProxy.Alt = true;
                            genericHotkeyProxy.Ctrl = false;
                            genericHotkeyProxy.Shift = false;
                            genericHotkeyProxy.Win = true;
                            Assert.Multiple(() =>
                            {
                                Assert.That(genericHotkeyProxy.Alt, Is.True);
                                Assert.That(genericHotkeyProxy.Ctrl, Is.False);
                                Assert.That(genericHotkeyProxy.Shift, Is.False);
                                Assert.That(genericHotkeyProxy.Win, Is.True);
                            });

                            genericHotkeyProxy.Alt = false;
                            genericHotkeyProxy.Ctrl = true;
                            genericHotkeyProxy.Shift = true;
                            genericHotkeyProxy.Win = false;
                            Assert.Multiple(() =>
                            {
                                Assert.That(genericHotkeyProxy.Alt, Is.False);
                                Assert.That(genericHotkeyProxy.Ctrl, Is.True);
                                Assert.That(genericHotkeyProxy.Shift, Is.True);
                                Assert.That(genericHotkeyProxy.Win, Is.False);
                            });
#pragma warning restore IDE0017 // Simplify object initialization
                        }
                    })).SetName("set()");
                }
            }

            public static IEnumerable IsValidTestCases
            {
                get
                {
                    yield return new TestCaseData(new Action(() =>
                    {
                        using (Hotkey hotkey = new KeyboardHotkey(FakeAction) { Key = Key.Left, Modifiers = ModifierKeys.Control })
                        {
                            hotkey.SetIsValid(false, "Invalid");
                            var genericHotkeyProxy = new GenericHotkeyProxy(hotkey);
                            Assert.That(genericHotkeyProxy.IsValid, Is.False);
                        }
                    })).SetName("Same as the underlying hotkey | SetIsValid(false)");
                    yield return new TestCaseData(new Action(() =>
                    {
                        using (Hotkey hotkey = new KeyboardHotkey(FakeAction) { Key = Key.Left, Modifiers = ModifierKeys.Control })
                        {
                            hotkey.SetIsValid(true, string.Empty);
                            var genericHotkeyProxy = new GenericHotkeyProxy(hotkey);
                            Assert.That(genericHotkeyProxy.IsValid, Is.True);
                        }
                    })).SetName("Same as the underlying hotkey | SetIsValid(true)");

                    yield return new TestCaseData(new Action(() =>
                    {
                        using (Hotkey hotkey = new KeyboardHotkey(FakeAction) { Key = Key.Left, Modifiers = ModifierKeys.Control })
                        {
                            var genericHotkeyProxy = new GenericHotkeyProxy(hotkey);
                            Assert.That(genericHotkeyProxy.IsValid, Is.True);

                            genericHotkeyProxy.Type = HotkeyType.MouseHook;
                            genericHotkeyProxy.Hotkey.SetIsValid(false, "Invalid");
                            Assert.That(genericHotkeyProxy.IsValid, Is.False);

                            genericHotkeyProxy.Type = HotkeyType.Keyboard;
                            Assert.That(genericHotkeyProxy.IsValid, Is.True);
                        }
                    })).SetName("Changing HotkeyType doesn't transfer the validity of the hotkey");
                }
            }

            public static IEnumerable SetActivatorTestCases
            {
                get
                {
                    yield return new TestCaseData(new Action(() =>
                    {
#pragma warning disable IDE0017 // Simplify object initialization
                        // ReSharper disable once UseObjectOrCollectionInitializer
                        var genericHotkeyProxy = new GenericHotkeyProxy();

                        // Setup: configure the underlying KeyboardHotkey
                        genericHotkeyProxy.Type = HotkeyType.Keyboard;
                        var kbHotkey = genericHotkeyProxy.Hotkey as KeyboardHotkey;

                        Assert.That(kbHotkey, Is.Not.Null);
                        kbHotkey.Key = Key.Left;
                        Assert.That(kbHotkey.Key, Is.EqualTo(Key.Left));

                        // Setup: configure the underlying MouseHookHotkey
                        genericHotkeyProxy.Type = HotkeyType.MouseHook;
                        var mhHotkey = genericHotkeyProxy.Hotkey as MouseHookHotkey;

                        Assert.That(mhHotkey, Is.Not.Null);
                        mhHotkey.MouseButton = MouseAction.XButton1;
                        Assert.That(mhHotkey.MouseButton, Is.EqualTo(MouseAction.XButton1));

                        // TEST
                        genericHotkeyProxy.Type = HotkeyType.Undefined;
                        genericHotkeyProxy.SetActivator(Key.Right);
                        Assert.That(mhHotkey.MouseButton, Is.EqualTo(MouseAction.XButton1)); // The MouseHookHotkey has not changed
                        Assert.That(kbHotkey.Key, Is.EqualTo(Key.Left));                     // The KeyboardHotkey has not changed
#pragma warning restore IDE0017 // Simplify object initialization
                    })).SetName("HotkeyType.Undefined");

                    yield return new TestCaseData(new Action(() =>
                    {
                        using (Hotkey hotkey = new KeyboardHotkey(FakeAction) { Key = Key.Left, Modifiers = ModifierKeys.Control })
                        {
                            var genericHotkeyProxy = new GenericHotkeyProxy(hotkey);
                            var underlyingHotkey = genericHotkeyProxy.Hotkey as KeyboardHotkey;

                            Assert.That(underlyingHotkey, Is.Not.Null);
                            Assert.That(underlyingHotkey.Key, Is.EqualTo(Key.Left));

                            genericHotkeyProxy.SetActivator(Key.Right);
                            Assert.That(underlyingHotkey.Key, Is.EqualTo(Key.Right));
                        }
                    })).SetName("HotkeyType.Keyboard");

                    yield return new TestCaseData(new Action(() =>
                    {
                        using (Hotkey hotkey = new KeyboardHotkey(FakeAction))
                        {
#pragma warning disable IDE0017 // Simplify object initialization
                            // ReSharper disable once UseObjectOrCollectionInitializer
                            var genericHotkeyProxy = new GenericHotkeyProxy(hotkey);

                            // Setup: configure the underlying MouseHookHotkey
                            genericHotkeyProxy.Type = HotkeyType.MouseHook;
                            var mhHotkey = genericHotkeyProxy.Hotkey as MouseHookHotkey;

                            Assert.That(mhHotkey, Is.Not.Null);
                            mhHotkey.MouseButton = MouseAction.XButton1;
                            Assert.That(mhHotkey.MouseButton, Is.EqualTo(MouseAction.XButton1));

                            // Setup: configure the underlying KeyboardHotkey
                            genericHotkeyProxy.Type = HotkeyType.Keyboard;
                            var kbHotkey = genericHotkeyProxy.Hotkey as KeyboardHotkey;

                            Assert.That(kbHotkey, Is.Not.Null);
                            kbHotkey.Key = Key.Left;
                            Assert.That(kbHotkey.Key, Is.EqualTo(Key.Left));

                            // TEST
                            genericHotkeyProxy.SetActivator(MouseButton.XButton2);
                            Assert.That(kbHotkey.Key, Is.EqualTo(Key.Left));                     // The KeyboardHotkey has not changed
                            Assert.That(mhHotkey.MouseButton, Is.EqualTo(MouseAction.XButton1)); // The MouseHookHotkey has not changed
#pragma warning restore IDE0017 // Simplify object initialization
                        }
                    })).SetName("HotkeyType.Keyboard | trying to set a MouseButton doesn't cause any side-effect");

                    yield return new TestCaseData(new Action(() =>
                    {
                        using (Hotkey hotkey = new MouseHookHotkey(FakeAction) { MouseButton = MouseAction.XButton1, Modifiers = ModifierKeys.Control })
                        {
                            var genericHotkeyProxy = new GenericHotkeyProxy(hotkey);
                            var underlyingHotkey = genericHotkeyProxy.Hotkey as MouseHookHotkey;

                            Assert.That(underlyingHotkey, Is.Not.Null);
                            Assert.That(underlyingHotkey.MouseButton, Is.EqualTo(MouseAction.XButton1));

                            genericHotkeyProxy.SetActivator(MouseAction.XButton2);
                            Assert.That(underlyingHotkey.MouseButton, Is.EqualTo(MouseAction.XButton2));
                        }
                    })).SetName("HotkeyType.MouseHook");

                    yield return new TestCaseData(new Action(() =>
                    {
                        using (Hotkey hotkey = new MouseHookHotkey(FakeAction) { Modifiers = ModifierKeys.Control })
                        {
#pragma warning disable IDE0017 // Simplify object initialization
                            // ReSharper disable once UseObjectOrCollectionInitializer
                            var genericHotkeyProxy = new GenericHotkeyProxy(hotkey);

                            // Setup: configure the underlying KeyboardHotkey
                            genericHotkeyProxy.Type = HotkeyType.Keyboard;
                            var kbHotkey = genericHotkeyProxy.Hotkey as KeyboardHotkey;

                            Assert.That(kbHotkey, Is.Not.Null);
                            kbHotkey.Key = Key.Left;
                            Assert.That(kbHotkey.Key, Is.EqualTo(Key.Left));

                            // Setup: configure the underlying MouseHookHotkey
                            genericHotkeyProxy.Type = HotkeyType.MouseHook;
                            var mhHotkey = genericHotkeyProxy.Hotkey as MouseHookHotkey;

                            Assert.That(mhHotkey, Is.Not.Null);
                            mhHotkey.MouseButton = MouseAction.XButton1;
                            Assert.That(mhHotkey.MouseButton, Is.EqualTo(MouseAction.XButton1));

                            // TEST
                            genericHotkeyProxy.SetActivator(Key.Right);
                            Assert.That(mhHotkey.MouseButton, Is.EqualTo(MouseAction.XButton1)); // The MouseHookHotkey has not changed
                            Assert.That(kbHotkey.Key, Is.EqualTo(Key.Left));                     // The KeyboardHotkey has not changed
#pragma warning restore IDE0017 // Simplify object initialization
                        }
                    })).SetName("HotkeyType.MouseHook | trying to set a Key doesn't cause any side-effect");
                }
            }

            public static IEnumerable IsAlreadyInUseByTestCases
            {
                get
                {
                    yield return new TestCaseData(new Func<GenericHotkeyProxy>(() => GenericKeyboardHotkey), null)
                                .Returns(false).SetName("null");
                    yield return new TestCaseData(new Func<GenericHotkeyProxy>(() => GenericKeyboardHotkey), new Func<GenericHotkeyProxy>(() => GenericMouseHookHotkey))
                                .Returns(false).SetName("KeyboardHotkey, MouseHookHotkey");

                    yield return new TestCaseData(new Func<GenericHotkeyProxy>(() => GenericKeyboardHotkey),
                                     new Func<GenericHotkeyProxy>(() =>
                                     {
                                         GenericHotkeyProxy h = GenericKeyboardHotkey;
                                         h.Alt = !h.Alt;
                                         return h;
                                     }))
                                .Returns(false).SetName("KeyboardHotkey | same Key, different modifiers");
                    yield return new TestCaseData(new Func<GenericHotkeyProxy>(() => GenericKeyboardHotkey),
                                     new Func<GenericHotkeyProxy>(() =>
                                     {
                                         GenericHotkeyProxy h = GenericKeyboardHotkey;
                                         h.SetActivator(Key.Right);
                                         return h;
                                     }))
                                .Returns(false).SetName("KeyboardHotkey | different Key, same modifiers");
                    yield return new TestCaseData(new Func<GenericHotkeyProxy>(() => GenericKeyboardHotkey), new Func<GenericHotkeyProxy>(() => GenericKeyboardHotkey))
                                .Returns(true).SetName("KeyboardHotkey | equal");

                    yield return new TestCaseData(new Func<GenericHotkeyProxy>(() => GenericMouseHookHotkey),
                                     new Func<GenericHotkeyProxy>(() =>
                                     {
                                         GenericHotkeyProxy h = GenericKeyboardHotkey;
                                         h.Alt = !h.Alt;
                                         return h;
                                     }))
                                .Returns(false).SetName("MouseHookHotkey | same MouseButton, different modifiers");
                    yield return new TestCaseData(new Func<GenericHotkeyProxy>(() => GenericMouseHookHotkey),
                                     new Func<GenericHotkeyProxy>(() =>
                                     {
                                         GenericHotkeyProxy h = GenericKeyboardHotkey;
                                         h.SetActivator(MouseAction.XButton2);
                                         return h;
                                     }))
                                .Returns(false).SetName("MouseHookHotkey | different MouseButton, same modifiers");
                    yield return new TestCaseData(new Func<GenericHotkeyProxy>(() => GenericMouseHookHotkey), new Func<GenericHotkeyProxy>(() => GenericMouseHookHotkey))
                                .Returns(true).SetName("MouseHookHotkey | equal");
                }
            }

            private static IAction FakeAction
            {
                get
                {
                    var action = A.Fake<IAction>();
                    A.CallTo(() => action.Name).Returns("Fake Action");
                    A.CallTo(() => action.PerformAction()).DoesNothing();
                    A.CallTo(() => action.Equals(action)).Returns(true);
                    return action;
                }
            }

            private static GenericHotkeyProxy GenericKeyboardHotkey
            {
                get
                {
                    var genericHotkeyProxy = new GenericHotkeyProxy
                    {
                        Type = HotkeyType.Keyboard,
                        Alt = true,
                        Ctrl = true
                    };
                    genericHotkeyProxy.Hotkey.Action = FakeAction;
                    genericHotkeyProxy.SetActivator(Key.Left);
                    return genericHotkeyProxy;
                }
            }

            private static GenericHotkeyProxy GenericMouseHookHotkey
            {
                get
                {
                    var genericHotkeyProxy = new GenericHotkeyProxy
                    {
                        Type = HotkeyType.MouseHook,
                        Alt = true,
                        Ctrl = true
                    };
                    genericHotkeyProxy.Hotkey.Action = FakeAction;
                    genericHotkeyProxy.SetActivator(MouseAction.XButton1);
                    return genericHotkeyProxy;
                }
            }

            #endregion
        }
    }
}