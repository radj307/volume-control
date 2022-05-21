using System;
using Xunit;

namespace VolumeControl.WPF.Tests
{
    public class WindowHandleGetter_Tests
    {
        [Fact()]
        public void GetWindowHandle_Test()
        {            
            Assert.NotEqual(IntPtr.Zero, WindowHandleGetter.GetWindowHandle());
        }

        [Fact()]
        public void GetHwndSource_Test()
        {
            Assert.NotNull(WindowHandleGetter.GetHwndSource(WindowHandleGetter.GetWindowHandle()));
        }
    }
}