using FakeItEasy;
using NUnit.Framework;
using ToastifyAPI.Logic;
using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Tests.Logic
{
    [TestFixture, TestOf(typeof(KeyboardHotkeyVisitor))]
    public class KeyboardHotkeyVisitorTest
    {
        private KeyboardHotkeyVisitor visitor;
        private IKeyboardHotkey fakeHotkey;

        [SetUp]
        public void SetUp()
        {
            this.visitor = new KeyboardHotkeyVisitor();
            this.fakeHotkey = A.Fake<IKeyboardHotkey>();
        }

        [Test(Author = "aleab")]
        public void TestVisit()
        {
            this.visitor.Visit(this.fakeHotkey);
            A.CallTo(() => this.fakeHotkey.PerformAction()).MustHaveHappenedOnceExactly();
        }
    }
}