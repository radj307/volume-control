using NUnit.Framework;
using System;
using System.Windows.Input;
using Toastify.Model;

namespace Toastify.Tests.Model
{
    [TestFixture, TestOf(typeof(KeyOrButton))]
    public class KeyOrButtonTest
    {
        [Test(Author = "aleab")]
        [TestCase(Key.A, null), TestCase(null, MouseButton.Left)]
        public void IsKeyTest(Key? key, MouseButton? mouseButton)
        {
            // ReSharper disable once PossibleInvalidOperationException
            KeyOrButton kob = key != null ? new KeyOrButton(Key.A) : new KeyOrButton(mouseButton.Value);
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
            const MouseButton mb = MouseButton.Left;
            KeyOrButton kob = mb;

            Assert.Multiple(() =>
            {
                Assert.That(!kob.IsKey);
                Assert.That(kob.MouseButton, Is.EqualTo(mb));
            });
        }
    }
}