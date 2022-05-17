using System;
using Xunit;

namespace VolumeControl.WPF.Tests
{
    /// <summary>Unit Tests for <see cref="BooleanInverter"/></summary>
    public class BooleanInverter_Tests
    {
        private struct TestingType { }
        private readonly BooleanInverter conv = new();

        [Fact()]
        public void Convert_Test()
        {
            Assert.True((bool)conv.Convert(false, null!, null!, null!));
            Assert.False((bool)conv.Convert(true, null!, null!, null!));

            Assert.True((bool)conv.Convert("false", null!, null!, null!));
            Assert.False((bool)conv.Convert("true", null!, null!, null!));

            Assert.ThrowsAny<Exception>(() => { conv.Convert("hello", null!, null!, null!); });
            Assert.ThrowsAny<Exception>(() => { conv.Convert(new TestingType(), null!, null!, null!); });
        }

        [Fact()]
        public void ConvertBack_Test()
        {
            Assert.True((bool)conv.ConvertBack(false, null!, null!, null!));
            Assert.False((bool)conv.ConvertBack(true, null!, null!, null!));

            Assert.True((bool)conv.ConvertBack("false", null!, null!, null!));
            Assert.False((bool)conv.ConvertBack("true", null!, null!, null!));

            Assert.ThrowsAny<Exception>(() => { conv.ConvertBack("hello", null!, null!, null!); });
            Assert.ThrowsAny<Exception>(() => { conv.ConvertBack(new TestingType(), null!, null!, null!); });
        }
    }
}