using SharpDX.DirectInput;
using VolumeControl.Log;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Core.Input
{
    internal class DirectInputAPI
    {
        static DirectInputAPI()
        {
            dinput = new();
            SharpDX.ComObject.LogMemoryLeakWarning += WriteMemoryLeakWarning;
        }

        public DirectInputAPI()
        {
        }

        private static readonly DirectInput dinput;

        private const int _deviceBufferSize = 128;

        private static LogWriter Log => FLog.Log;

        private static void WriteMemoryLeakWarning(string msg)
        {
            Log.Warning($"Memory Leak Detected!", msg);
        }

        private IEnumerable<DeviceInstance> GetAllGameControlDevices() => dinput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly).Where(dev => dev.IsHumanInterfaceDevice);

        private void PollDevice(Device device, ref bool extKill)
        {
            if (extKill) return;
            device.Properties.BufferSize = _deviceBufferSize;
            Log.Debug($"Started polling device '{device.Information.ProductName}' ({device.Information.ProductGuid})", $"Instance: ");

            device.Acquire();

            while (!extKill)
            {
                device.Poll();
                var js = new Joystick(dinput, Guid.Empty);
                js.GetBufferedData();
            }

            device.Unacquire();
        }
        private void PollDevice(Device device)
        {
            bool kill = false;
            PollDevice(device, ref kill);
        }

        void testing()
        {
            foreach (var dev in GetAllGameControlDevices())
            {
                var js = new Joystick(dinput, dev.InstanceGuid);

                js.Acquire();



                js.Unacquire();
            }
        }
    }
}
