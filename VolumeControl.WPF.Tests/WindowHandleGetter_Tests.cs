using Xunit;
using VolumeControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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