using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using JetBrains.Annotations;
using ToastifyAPI.Model;
using MouseAction = ToastifyAPI.Core.MouseAction;

namespace ToastifyAPI.Tests.Model
{
    [TestFixture, TestOf(typeof(KeyOrButton))]
    public class KeyOrButtonTest
    {
        [Test(Author = "aleab")]
        [TestCase(Key.A, null), TestCase(null, MouseAction.XButton1)]
        public static void IsKeyTest(Key? key, MouseAction? mouseAction)
        {
            // ReSharper disable once PossibleInvalidOperationException
            KeyOrButton kob = key.HasValue ? new KeyOrButton(key.Value) : new KeyOrButton(mouseAction.Value);
            Assert.That(key.HasValue ? kob.IsKey : !kob.IsKey);
        }

        [Test(Author = "aleab")]
        public static void JsonConstructor_Key()
        {
            KeyOrButton kob = new KeyOrButton(true, Key.Up, null);

            Assert.Multiple(() =>
            {
                Assert.That(kob.IsKey);
                Assert.That(kob.Key, Is.EqualTo(Key.Up));
                Assert.That(kob.MouseButton, Is.EqualTo(null));
            });
        }

        [Test(Author = "aleab")]
        public static void JsonConstructor_MouseAction()
        {
            KeyOrButton kob = new KeyOrButton(false, null, MouseAction.XButton1);

            Assert.Multiple(() =>
            {
                Assert.That(!kob.IsKey);
                Assert.That(kob.Key, Is.EqualTo(null));
                Assert.That(kob.MouseButton, Is.EqualTo(MouseAction.XButton1));
            });
        }

        [Test(Author = "aleab")]
        public static void JsonConstructor_IfBothDependsOnIsKey()
        {
            KeyOrButton kob1 = new KeyOrButton(false, Key.Up, MouseAction.XButton1);
            Assert.Multiple(() =>
            {
                Assert.That(!kob1.IsKey);
                Assert.That(kob1.Key, Is.EqualTo(null));
                Assert.That(kob1.MouseButton, Is.EqualTo(MouseAction.XButton1));
            });

            KeyOrButton kob2 = new KeyOrButton(true, Key.Up, MouseAction.XButton1);
            Assert.Multiple(() =>
            {
                Assert.That(kob2.IsKey);
                Assert.That(kob2.Key, Is.EqualTo(Key.Up));
                Assert.That(kob2.MouseButton, Is.EqualTo(null));
            });
        }

        [Test(Author = "aleab")]
        [TestCase(true, null, null), TestCase(false, null, null)]
        [TestCase(false, Key.Up, null), TestCase(true, null, MouseAction.XButton1)]
        public static void JsonConstructor_Invalid(bool isKey, Key? key, MouseAction? mouseAction)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new KeyOrButton(isKey, key, mouseAction));
        }

        [Test(Author = "aleab")]
        [TestCase(Key.A, null), TestCase(null, MouseAction.XButton1)]
        public static void Clone(Key? key, MouseAction? mouseAction)
        {
            // ReSharper disable once PossibleInvalidOperationException
            KeyOrButton kob = key.HasValue ? new KeyOrButton(key.Value) : new KeyOrButton(mouseAction.Value);
            KeyOrButton clone = kob.Clone() as KeyOrButton;

            Assert.Multiple(() =>
            {
                Assert.That(clone, Is.Not.Null);
                Assert.That(kob.IsKey, Is.EqualTo(clone.IsKey));
                Assert.That(kob.Key, Is.EqualTo(clone.Key));
                Assert.That(kob.MouseButton, Is.EqualTo(clone.MouseButton));
            });
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(KeyOrButtonData), nameof(KeyOrButtonData.EqualsTestCases))]
        public static bool EqualsTest([NotNull] KeyOrButton kob1, KeyOrButton kob2)
        {
            return kob1.Equals(kob2);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(KeyOrButtonData), nameof(KeyOrButtonData.ObjectEqualsTestCases))]
        public static bool EqualsTest_Object([NotNull] KeyOrButton kob, object obj)
        {
            return kob.Equals(obj);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(KeyOrButtonData), nameof(KeyOrButtonData.GetHashCodeTestCases))]
        public static bool GetHashCodeTest([NotNull] KeyOrButton kob1, KeyOrButton kob2)
        {
            return kob1.GetHashCode() == kob2?.GetHashCode();
        }

        [Test(Author = "aleab")]
        public static void ImplicitCastToKeyOrMouseButton_FromKey()
        {
            const Key key = Key.A;
            KeyOrButton kob = key;

            Assert.Multiple(() =>
            {
                Assert.That(kob.IsKey);
                Assert.That(kob.Key, Is.EqualTo(key));
            });
        }

        [Test(Author = "aleab")]
        public static void ImplicitCastToKeyOrMouseButton_FromMouseAction()
        {
            const MouseAction mouseAction = MouseAction.XButton1;
            KeyOrButton keyOrButton = mouseAction;

            Assert.Multiple(() =>
            {
                Assert.That(!keyOrButton.IsKey);
                Assert.That(keyOrButton.MouseButton, Is.EqualTo(mouseAction));
            });
        }

        public static class KeyOrButtonData
        {
            public static IEnumerable<TestCaseData> EqualsTestCases
            {
                get
                {
                    KeyOrButton key = new KeyOrButton(Key.Up);
                    KeyOrButton button = new KeyOrButton(MouseAction.XButton1);

                    // with null
                    yield return new TestCaseData(key, null).Returns(false);
                    yield return new TestCaseData(button, null).Returns(false);

                    // same
                    yield return new TestCaseData(key, key).Returns(true);
                    yield return new TestCaseData(button, button).Returns(true);

                    // equal
                    yield return new TestCaseData(key, new KeyOrButton(Key.Up)).Returns(true);
                    yield return new TestCaseData(button, new KeyOrButton(MouseAction.XButton1)).Returns(true);

                    // not equal
                    yield return new TestCaseData(key, new KeyOrButton(Key.Down)).Returns(false);
                    yield return new TestCaseData(button, new KeyOrButton(MouseAction.XButton2)).Returns(false);

                    // not equal
                    yield return new TestCaseData(key, button).Returns(false);
                    yield return new TestCaseData(button, key).Returns(false);
                }
            }

            public static IEnumerable<TestCaseData> ObjectEqualsTestCases
            {
                get
                {
                    KeyOrButton key = new KeyOrButton(Key.Up);
                    KeyOrButton button = new KeyOrButton(MouseAction.XButton1);

                    foreach (var testCase in EqualsTestCases)
                    {
                        yield return testCase;
                    }

                    // different type
                    yield return new TestCaseData(key, "key").Returns(false);
                    yield return new TestCaseData(button, "button").Returns(false);
                }
            }

            public static IEnumerable<TestCaseData> GetHashCodeTestCases
            {
                get
                {
                    KeyOrButton key = new KeyOrButton(Key.Up);
                    KeyOrButton button = new KeyOrButton(MouseAction.XButton1);

                    // same
                    yield return new TestCaseData(key, key).Returns(true).SetName("same: key");
                    yield return new TestCaseData(button, button).Returns(true).SetName("same: button");

                    // equal
                    yield return new TestCaseData(key, new KeyOrButton(Key.Up)).Returns(true).SetName("equal: key");
                    yield return new TestCaseData(button, new KeyOrButton(MouseAction.XButton1)).Returns(true).SetName("equal: button");

                    // not equal
                    yield return new TestCaseData(key, new KeyOrButton(Key.Down)).Returns(false).SetName("not equal: key");
                    yield return new TestCaseData(button, new KeyOrButton(MouseAction.XButton2)).Returns(false).SetName("not equal: button");

                    // not equal, one key and one button
                    yield return new TestCaseData(key, button).Returns(false).SetName("not equal: key,button");
                    yield return new TestCaseData(button, key).Returns(false).SetName("not equal: button,key");

                    // not equal, one key and one button, but same underlying value
                    yield return new TestCaseData(new KeyOrButton((Key)3), new KeyOrButton((MouseAction)3)).Returns(false).SetName("not equal: key,button; sama underlying value");
                    yield return new TestCaseData(new KeyOrButton((MouseAction)3), new KeyOrButton((Key)3)).Returns(false).SetName("not equal: button,key; sama underlying value");
                }
            }
        }
    }
}