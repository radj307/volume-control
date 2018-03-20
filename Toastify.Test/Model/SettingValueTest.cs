using FakeItEasy;
using NUnit.Framework;
using System;
using Toastify.Common;
using Toastify.Model;

namespace Toastify.Tests.Model
{
    [TestFixture, TestOf("SettingsValue<>")]
    public class SettingValueTest
    {
        private SettingValue<IFakeT> settingValue;

        [SetUp]
        public void SetUp()
        {
            IFakeT value = A.Fake<IFakeT>();
            IFakeT min = A.Fake<IFakeT>();
            IFakeT max = A.Fake<IFakeT>();

            A.CallTo(() => min.CompareTo(max)).Returns(-1);
            A.CallTo(() => value.CompareTo(min)).Returns(1);
            A.CallTo(() => value.CompareTo(max)).Returns(-1);

            this.settingValue = new SettingValue<IFakeT>(value, value, new Range<IFakeT>(min, max));
        }

        #region Change Value (Range tests)

        [Test(Author = "aleab")]
        public void ChangeValueTest()
        {
            IFakeT newValue = A.Fake<IFakeT>();
            A.CallTo(() => newValue.CompareTo(this.settingValue.Range.Value.Min)).Returns(1);
            A.CallTo(() => newValue.CompareTo(this.settingValue.Range.Value.Max)).Returns(-1);
            A.CallTo(() => newValue.CompareTo(this.settingValue.Value)).Returns(1);

            this.settingValue.Value = newValue;

            // ReSharper disable once PossibleInvalidOperationException
            Assert.That(this.settingValue.Value, Is.SameAs(newValue));
        }

        [Test(Author = "aleab")]
        public void ChangeValueTest_ltMin()
        {
            IFakeT newValue = A.Fake<IFakeT>();
            A.CallTo(() => newValue.CompareTo(this.settingValue.Range.Value.Min)).Returns(-1);

            this.settingValue.Value = newValue;

            // ReSharper disable once PossibleInvalidOperationException
            Assert.That(this.settingValue.Value, Is.SameAs(this.settingValue.Range.Value.Min));
        }

        [Test(Author = "aleab")]
        public void ChangeValueTest_gtMax()
        {
            IFakeT newValue = A.Fake<IFakeT>();
            A.CallTo(() => newValue.CompareTo(this.settingValue.Range.Value.Max)).Returns(1);

            this.settingValue.Value = newValue;

            // ReSharper disable once PossibleInvalidOperationException
            Assert.That(this.settingValue.Value, Is.SameAs(this.settingValue.Range.Value.Max));
        }

        #endregion Change Value (Range tests)

        public interface IFakeT : IComparable, IConvertible
        {
        }
    }
}