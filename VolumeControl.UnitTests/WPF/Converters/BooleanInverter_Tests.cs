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
            Assertx.True((bool)conv.Convert(false, null!, null!, null!));
            Assertx.False((bool)conv.Convert(true, null!, null!, null!));

            Assertx.True((bool)conv.Convert("false", null!, null!, null!));
            Assertx.False((bool)conv.Convert("true", null!, null!, null!));

            Assertx.Throws(() => { conv.Convert("hello", null!, null!, null!); });
            Assertx.Throws(() => { conv.Convert(new TestingType(), null!, null!, null!); });
        }

        [TestMethod()]
        public void ConvertBack_Test()
        {
            Assertx.True((bool)conv.ConvertBack(false, null!, null!, null!));
            Assertx.False((bool)conv.ConvertBack(true, null!, null!, null!));

            Assertx.True((bool)conv.ConvertBack("false", null!, null!, null!));
            Assertx.False((bool)conv.ConvertBack("true", null!, null!, null!));

            Assertx.Throws(() => { conv.ConvertBack("hello", null!, null!, null!); });
            Assertx.Throws(() => { conv.ConvertBack(new TestingType(), null!, null!, null!); });
        }
    }
}