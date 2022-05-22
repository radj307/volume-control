using System;
using VolumeControl.WPF;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VolumeControl.UnitTests.WPF.Converters
{
    [TestClass()]
    public class WindowHandleGetter_Tests
    {
        [TestMethod()]
        public void GetWindowHandle_Test() => Assert.AreNotSame(IntPtr.Zero, WindowHandleGetter.GetWindowHandle());

        [TestMethod()]
        public void GetHwndSource_Test() => Assert.IsNotNull(WindowHandleGetter.GetHwndSource(WindowHandleGetter.GetWindowHandle()));
    }
}