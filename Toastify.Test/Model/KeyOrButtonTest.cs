using NUnit.Framework;
using System;
using System.Windows.Input;
using Toastify.Model;
using MouseAction = Toastify.Core.MouseAction;

namespace Toastify.Tests.Model
{
    [TestFixture, TestOf(typeof(KeyOrButton))]
    public class KeyOrButtonTest
    {
        [Test(Author = "aleab")]
        [TestCase(Key.A, null), TestCase(null, MouseAction.XButton1)]
        public void IsKeyTest(Key? key, MouseAction? mouseAction)
        {
            // ReSharper disable once PossibleInvalidOperationException
            KeyOrButton kob = key != null ? new KeyOrButton(Key.A) : new KeyOrButton(mouseAction.Value);
            Assert.That(key != null ? kob.IsKey : !kob.IsKey);
        }

        [Test(Author = "aleab")]
        public void JsonConstructor_NeitherKeyNorMouseButton()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new KeyOrButton(false, null, null));
        }

        [Test(Author = "aleab")]
        public void KeyToKeyOrMouseButtonImplicitOperator()
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
        public void MouseButtonToKeyOrMouseButtonImplicitOperator()
        {
            const MouseAction mouseAction = MouseAction.XButton1;
            KeyOrButton keyOrButton = mouseAction;

            Assert.Multiple(() =>
            {
                Assert.That(!keyOrButton.IsKey);
                Assert.That(keyOrButton.MouseButton, Is.EqualTo(mouseAction));
            });
        }
    }
}