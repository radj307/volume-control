using System;
using VolumeControl.WPF.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VolumeControl.UnitTests.WPF.Converters
{
    /// <summary>Unit Tests for <see cref="BooleanInverter"/></summary>
    [TestClass()]
    public class BooleanInverter_Tests
    {
        private struct TestingType { }
        private readonly BooleanInverter conv = new();

        [TestMethod()]
        public void Convert_Test()
        {
            Assert.IsTrue((bool)conv.Convert(false, null!, null!, null!));
            Assert.IsFalse((bool)conv.Convert(true, null!, null!, null!));

            Assert.IsTrue((bool)conv.Convert("false", null!, null!, null!));
            Assert.IsFalse((bool)conv.Convert("true", null!, null!, null!));

            Assert.ThrowsException<Exception>(() => { conv.Convert("hello", null!, null!, null!); });
            Assert.ThrowsException<Exception>(() => { conv.Convert(new TestingType(), null!, null!, null!); });
        }

        [TestMethod()]
        public void ConvertBack_Test()
        {
            Assert.IsTrue((bool)conv.ConvertBack(false, null!, null!, null!));
            Assert.IsFalse((bool)conv.ConvertBack(true, null!, null!, null!));

            Assert.IsTrue((bool)conv.ConvertBack("false", null!, null!, null!));
            Assert.IsFalse((bool)conv.ConvertBack("true", null!, null!, null!));

            Assert.ThrowsException<Exception>(() => { conv.ConvertBack("hello", null!, null!, null!); });
            Assert.ThrowsException<Exception>(() => { conv.ConvertBack(new TestingType(), null!, null!, null!); });
        }
    }
}