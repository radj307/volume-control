using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using VolumeControl.Audio;

namespace VolumeControl.UnitTests.Audio
{

    [TestClass()]
    public class AudioAPI_Tests
    {
        private static AudioAPISettings Settings => AudioAPISettings.Default;

        [TestMethod()]
        public void AudioAPISettings_Test()
        {
            Assertx.Greater(Settings.AutoReloadInterval, 0);
            Assertx.Greater(Settings.AutoReloadIntervalMin, 0);
            Assertx.Greater(Settings.AutoReloadIntervalMax, 0);
        }

        [TestMethod()]
        public void AudioAPI_Test()
        {
            AudioAPI aAPI = null!;
            Assertx.NoThrows(() => { aAPI = new(); });
            Assertx.Same(Settings.SelectedSession, aAPI.Target);
            Assertx.Same(Settings.LockSelectedSession, aAPI.LockSelectedSession);
            Assertx.Same(Settings.LockSelectedDevice, aAPI.LockSelectedDevice);
            Assertx.Equal(Settings.VolumeStepSize, aAPI.VolumeStepSize);
            Assertx.Same(Settings.ReloadOnHotkey, aAPI.ReloadOnHotkey);
            Assertx.Same(Settings.ReloadOnInterval, aAPI.ReloadOnInterval);
            Assertx.NotEmpty(aAPI.Sessions); //< there should always be a system idle session
        }

        [TestMethod()]
        public void ReloadDeviceList_Test()
        {
            AudioAPI aAPI = new();

            var dev = new Assertx.EventTrigger(true)
            {
                MinCount = 1,
                ResolveEventNameFromStackTrace = false,
            };
            var ses = new Assertx.EventTrigger(true)
            {
                MinCount = 1,
                ResolveEventNameFromStackTrace = false,
            };

            // when CheckAllDevices is false && LockSelectedDevice is true:
            aAPI.CheckAllDevices = false;
            aAPI.LockSelectedDevice = true;
            aAPI.DeviceListReloaded += dev.Handler; //< devices
            aAPI.SessionListReloaded += ses.Handler; //< sessions
            aAPI.SelectedDeviceSwitched += Assertx.NoEvent; //< devices
            aAPI.SelectedSessionSwitched += Assertx.NoEvent; //< and sessions, since CheckAllDevices is false
            Assertx.NoThrows(() => aAPI.ReloadDeviceList());
            dev.Armed = false; //< check count
            ses.Armed = false; //< check count
            aAPI.DeviceListReloaded -= dev.Handler;
            aAPI.SessionListReloaded -= ses.Handler;
            aAPI.SelectedDeviceSwitched -= Assertx.NoEvent;
            aAPI.SelectedSessionSwitched -= Assertx.NoEvent;

            dev.Reset();

            // when CheckAllDevices is true && LockSelectedDevice is true:
            aAPI.CheckAllDevices = true;
            aAPI.DeviceListReloaded += dev.Handler; //< devices
            aAPI.SessionListReloaded += Assertx.NoEvent; //< sessions
            aAPI.SelectedDeviceSwitched += Assertx.NoEvent; //< devices
            aAPI.SelectedSessionSwitched += Assertx.NoEvent; //< and sessions, since CheckAllDevices is false
            Assertx.NoThrows(() => aAPI.ReloadDeviceList());
            dev.Armed = false; //< check count
            aAPI.DeviceListReloaded -= dev.Handler;
            aAPI.SessionListReloaded -= Assertx.NoEvent;
            aAPI.SelectedDeviceSwitched -= Assertx.NoEvent;
            aAPI.SelectedSessionSwitched -= Assertx.NoEvent;
        }

        [TestMethod()]
        public void ReloadSessionList_Test()
        {
            AudioAPI aAPI = new();
            aAPI.SelectedSessionSwitched += Assertx.NoEvent;
            Assertx.NoThrows(() => aAPI.ReloadSessionList());
            aAPI.SelectedSessionSwitched -= Assertx.NoEvent;


        }

        [TestMethod()]
        public void GetDefaultDevice_Test()
        {
            AudioAPI aAPI = new();

            var def = aAPI.GetDefaultDevice();
            Assertx.NotNull(def);
        }

        [TestMethod()]
        public void FindDevice_Test()
        {
            AudioAPI aAPI = new();
            var def = aAPI.GetDefaultDevice();
            Assertx.NotNull(aAPI.FindDevice((dev) => def.DeviceID.Equals(dev.DeviceID, StringComparison.Ordinal)));
        }

        [TestMethod()]
        public void FindSession_Test()
        {
            AudioAPI aAPI = new();

            Assert.Fail("Not Implemented");
        }

        [TestMethod()]
        public void FindSessionWithID_Test()
        {
            AudioAPI aAPI = new();

            Assert.Fail("Not Implemented");
        }

        [TestMethod()]
        public void FindSessionWithName_Test()
        {
            AudioAPI aAPI = new();

            Assert.Fail("Not Implemented");
        }

        [TestMethod()]
        public void FindSessionWithIdentifier_Test()
        {
            AudioAPI aAPI = new();

            Assert.Fail("Not Implemented");
        }

        [TestMethod()]
        public void GetSessionNames_Test()
        {
            AudioAPI aAPI = new();

            Assert.Fail("Not Implemented");
        }

        [TestMethod()]
        public void IncrementSessionVolume_Test()
        {
            AudioAPI aAPI = new();
            Assertx.NotEmpty(aAPI.Sessions, "Testing environment doesn't have any audio sessions!");

            var sel = aAPI.SelectedSession = aAPI.Sessions.First();

            var defVolume = sel.Volume;

            sel!.Volume = 0;
            Assertx.NoThrows(() => aAPI.IncrementSessionVolume(20));
            Assertx.Equal(sel!.Volume, 20);

            sel!.Volume = 90;
            Assertx.NoThrows(() => aAPI.IncrementSessionVolume(20));
            Assertx.Equal(sel!.Volume, 100);

            sel!.Volume = 100;
            Assertx.Equal(sel!.Volume, 100);
            Assertx.NoThrows(() => aAPI.IncrementSessionVolume(20));
            Assertx.Equal(sel!.Volume, 100);

            aAPI.SetSessionVolume(defVolume);
        }

        [TestMethod("IncrementSessionVolume_Test (VolumeStepSize)")]
        public void IncrementSessionVolume_Test1()
        {
            AudioAPI aAPI = new();
            Assertx.NotEmpty(aAPI.Sessions, "Testing environment doesn't have any audio sessions!");

            var sel = aAPI.SelectedSession = aAPI.Sessions.First();

            var defVolume = sel.Volume;
            aAPI.VolumeStepSize = 10;

            sel!.Volume = 0;
            Assertx.NoThrows(() => aAPI.IncrementSessionVolume());
            Assertx.Equal(sel!.Volume, 10);

            sel!.Volume = 90;
            Assertx.NoThrows(() => aAPI.IncrementSessionVolume());
            Assertx.Equal(sel!.Volume, 100);

            sel!.Volume = 100;
            Assertx.Equal(sel!.Volume, 100);
            Assertx.NoThrows(() => aAPI.IncrementSessionVolume());
            Assertx.Equal(sel!.Volume, 100);

            aAPI.SetSessionVolume(defVolume);
        }

        [TestMethod()]
        public void DecrementSessionVolume_Test()
        {
            AudioAPI aAPI = new();
            Assertx.NotEmpty(aAPI.Sessions, "Testing environment doesn't have any audio sessions!");

            var sel = aAPI.SelectedSession = aAPI.Sessions.First();

            var defVolume = sel.Volume;

            sel!.Volume = 20;
            Assertx.NoThrows(() => aAPI.DecrementSessionVolume(20));
            Assertx.Equal(sel!.Volume, 0);

            sel!.Volume = 90;
            Assertx.NoThrows(() => aAPI.DecrementSessionVolume(20));
            Assertx.Equal(sel!.Volume, 70);

            sel!.Volume = 10;
            Assertx.Equal(sel!.Volume, 10);
            Assertx.NoThrows(() => aAPI.DecrementSessionVolume(20));
            Assertx.Equal(sel!.Volume, 0);

            aAPI.SetSessionVolume(defVolume);
        }

        [TestMethod("DecrementSessionVolume_Test (VolumeStepSize)")]
        public void DecrementSessionVolume_Test1()
        {
            AudioAPI aAPI = new();
            Assertx.NotEmpty(aAPI.Sessions, "Testing environment doesn't have any audio sessions!");

            var sel = aAPI.SelectedSession = aAPI.Sessions.First();

            var defVolume = sel.Volume;
            aAPI.VolumeStepSize = 10;

            sel!.Volume = 10;
            Assertx.NoThrows(() => aAPI.DecrementSessionVolume());
            Assertx.Equal(sel!.Volume, 0);

            sel!.Volume = 90;
            Assertx.NoThrows(() => aAPI.DecrementSessionVolume());
            Assertx.Equal(sel!.Volume, 80);

            sel!.Volume = 5;
            Assertx.Equal(sel!.Volume, 5);
            Assertx.NoThrows(() => aAPI.DecrementSessionVolume());
            Assertx.Equal(sel!.Volume, 0);

            aAPI.SetSessionVolume(defVolume);
        }

        [TestMethod()]
        public void GetSessionVolume_Test()
        {
            AudioAPI aAPI = new();

            Assert.Fail("Not Implemented");
        }

        [TestMethod()]
        public void SetSessionVolume_Test()
        {
            AudioAPI aAPI = new();

            Assert.Fail("Not Implemented");
        }

        [TestMethod()]
        public void GetSessionMute_Test()
        {
            AudioAPI aAPI = new();

            Assert.Fail("Not Implemented");
        }

        [TestMethod()]
        public void SetSessionMute_Test()
        {
            AudioAPI aAPI = new();

            Assert.Fail("Not Implemented");
        }

        [TestMethod()]
        public void ToggleSessionMute_Test()
        {
            AudioAPI aAPI = new();

            Assert.Fail("Not Implemented");
        }

        [TestMethod()]
        public void SelectNextSession_Test()
        {
            AudioAPI aAPI = new();
            Assertx.NotEmpty(aAPI.Sessions, "Testing environment doesn't have any audio sessions!");
            aAPI.SelectedSession = aAPI.Sessions.First();

            aAPI.LockSelectedSession = false;

            var sel = aAPI.SelectedSession;

            Assertx.Same(sel, aAPI.SelectedSession);
            Assertx.NoThrows(() => aAPI.SelectNextSession());
            Assertx.NotSame(sel, aAPI.SelectedSession);

            sel = aAPI.SelectedSession;

            aAPI.LockSelectedSession = true;
            Assertx.Same(sel, aAPI.SelectedSession);
            Assertx.NoThrows(() => aAPI.SelectNextSession());
            Assertx.Same(sel, aAPI.SelectedSession);
        }

        [TestMethod()]
        public void SelectPreviousSession_Test()
        {
            AudioAPI aAPI = new();
            Assertx.NotEmpty(aAPI.Sessions, "Testing environment doesn't have any audio sessions!");
            aAPI.SelectedSession = aAPI.Sessions.First();

            aAPI.LockSelectedSession = false;

            var sel = aAPI.SelectedSession;

            Assertx.Same(sel, aAPI.SelectedSession);
            Assertx.NoThrows(() => aAPI.SelectPreviousSession());
            Assertx.NotSame(sel, aAPI.SelectedSession);

            sel = aAPI.SelectedSession;

            aAPI.LockSelectedSession = true;
            Assertx.Same(sel, aAPI.SelectedSession);
            Assertx.NoThrows(() => aAPI.SelectPreviousSession());
            Assertx.Same(sel, aAPI.SelectedSession);
        }

        [TestMethod()]
        public void DeselectSession_Test()
        {
            AudioAPI aAPI = new();
            Assertx.NotEmpty(aAPI.Sessions, "Testing environment doesn't have any audio sessions!");

            aAPI.LockSelectedSession = false;
            aAPI.SelectedSession = aAPI.Sessions.First();
            Assertx.True(aAPI.DeselectSession());

            aAPI.LockSelectedSession = true;
            aAPI.SelectedSession = aAPI.Sessions.First();
            Assertx.False(aAPI.DeselectSession());
        }

        [TestMethod()]
        public void IncrementDeviceVolume_Test()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;

            var sel = aAPI.SelectedDevice;
            Assertx.NotNull(sel, "Testing environment doesn't have a default selected device!");

            var defVolume = sel!.EndpointVolume;

            sel!.EndpointVolume = 0;
            Assertx.NoThrows(() => aAPI.IncrementDeviceVolume(20));
            Assertx.Equal(sel!.EndpointVolume, 20);

            sel!.EndpointVolume = 90;
            Assertx.NoThrows(() => aAPI.IncrementDeviceVolume(20));
            Assertx.Equal(sel!.EndpointVolume, 100);

            sel!.EndpointVolume = 100;
            Assertx.Equal(sel!.EndpointVolume, 100);
            Assertx.NoThrows(() => aAPI.IncrementDeviceVolume(20));
            Assertx.Equal(sel!.EndpointVolume, 100);

            aAPI.SetDeviceVolume(defVolume);
        }

        [TestMethod("IncrementDeviceVolume_Test (VolumeStepSize)")]
        public void IncrementDeviceVolume_Test1()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;

            var sel = aAPI.SelectedDevice;
            Assertx.NotNull(sel, "Testing environment doesn't have a default selected device!");

            var defVolume = sel!.EndpointVolume;
            aAPI.VolumeStepSize = 10;

            sel!.EndpointVolume = 0;
            Assertx.NoThrows(() => aAPI.IncrementDeviceVolume());
            Assertx.Equal(sel!.EndpointVolume, 10);

            sel!.EndpointVolume = 90;
            Assertx.NoThrows(() => aAPI.IncrementDeviceVolume());
            Assertx.Equal(sel!.EndpointVolume, 100);

            sel!.EndpointVolume = 100;
            Assertx.Equal(sel!.EndpointVolume, 100);
            Assertx.NoThrows(() => aAPI.IncrementDeviceVolume());
            Assertx.Equal(sel!.EndpointVolume, 100);

            aAPI.SetDeviceVolume(defVolume);
        }

        [TestMethod()]
        public void DecrementDeviceVolume_Test()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;

            var sel = aAPI.SelectedDevice;
            Assertx.NotNull(sel, "Testing environment doesn't have a default selected device!");

            var defVolume = sel!.EndpointVolume;

            sel!.EndpointVolume = 20;
            Assertx.NoThrows(() => aAPI.DecrementDeviceVolume(20));
            Assertx.Equal(sel!.EndpointVolume, 0);

            sel!.EndpointVolume = 90;
            Assertx.NoThrows(() => aAPI.DecrementDeviceVolume(20));
            Assertx.Equal(sel!.EndpointVolume, 70);

            sel!.EndpointVolume = 10;
            Assertx.Equal(sel!.EndpointVolume, 10);
            Assertx.NoThrows(() => aAPI.DecrementDeviceVolume(20));
            Assertx.Equal(sel!.EndpointVolume, 0);

            aAPI.SetDeviceVolume(defVolume);
        }

        [TestMethod("DecrementDeviceVolume_Test (VolumeStepSize)")]
        public void DecrementDeviceVolume_Test1()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;

            var sel = aAPI.SelectedDevice;
            Assertx.NotNull(sel, "Testing environment doesn't have a default selected device!");

            var defVolume = sel!.EndpointVolume;
            aAPI.VolumeStepSize = 10;

            sel!.EndpointVolume = 20;
            Assertx.NoThrows(() => aAPI.DecrementDeviceVolume());
            Assertx.Equal(sel!.EndpointVolume, 10);

            sel!.EndpointVolume = 90;
            Assertx.NoThrows(() => aAPI.DecrementDeviceVolume());
            Assertx.Equal(sel!.EndpointVolume, 80);

            sel!.EndpointVolume = 5;
            Assertx.Equal(sel!.EndpointVolume, 5);
            Assertx.NoThrows(() => aAPI.DecrementDeviceVolume());
            Assertx.Equal(sel!.EndpointVolume, 0);

            aAPI.SetDeviceVolume(defVolume);
        }

        [TestMethod()]
        public void GetDeviceVolume_Test()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;

            var sel = aAPI.SelectedDevice;
            Assertx.NotNull(sel, "Testing environment doesn't have a default selected device!");

            var defVolume = sel!.EndpointVolume;

            sel!.EndpointVolume = -10;
            Assertx.Equal(aAPI.GetDeviceVolume(), 0);
            sel!.EndpointVolume = 0;
            Assertx.Equal(aAPI.GetDeviceVolume(), 0);

            sel!.EndpointVolume = 100;
            Assertx.Equal(aAPI.GetDeviceVolume(), 100);
            sel!.EndpointVolume = 110;
            Assertx.Equal(aAPI.GetDeviceVolume(), 100);

            sel!.EndpointVolume = defVolume;
        }

        [TestMethod()]
        public void SetDeviceVolume_Test()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;

            var sel = aAPI.SelectedDevice;
            Assertx.NotNull(sel, "Testing environment doesn't have a default selected device!");

            var defVolume = sel!.EndpointVolume;

            Assertx.NoThrows(() => aAPI.SetDeviceVolume(-10));
            Assertx.Equal(sel!.EndpointVolume, 0);
            Assertx.NoThrows(() => aAPI.SetDeviceVolume(0));
            Assertx.Equal(sel!.EndpointVolume, 0);

            Assertx.NoThrows(() => aAPI.SetDeviceVolume(100));
            Assertx.Equal(sel!.EndpointVolume, 100);
            Assertx.NoThrows(() => aAPI.SetDeviceVolume(110));
            Assertx.Equal(sel!.EndpointVolume, 100);

            sel!.EndpointVolume = defVolume;
        }

        [TestMethod()]
        public void GetDeviceMute_Test()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;

            var sel = aAPI.SelectedDevice;
            Assertx.NotNull(sel, "Testing environment doesn't have a default selected device!");

            sel!.EndpointMuted = false;
            Assertx.NoThrows(() => Assertx.False(aAPI.GetDeviceMute()));
            sel!.EndpointMuted = true;
            Assertx.NoThrows(() => Assertx.True(aAPI.GetDeviceMute()));

            aAPI.GetDefaultDevice().EndpointMuted = false;
        }

        [TestMethod()]
        public void SetDeviceMute_Test()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;

            var sel = aAPI.SelectedDevice;
            Assertx.NotNull(sel, "Testing environment doesn't have a default selected device!");

            Assertx.NoThrows(() => aAPI.SetDeviceMute(true));
            Assertx.True(sel!.EndpointMuted);
            Assertx.NoThrows(() => aAPI.SetDeviceMute(false));
            Assertx.False(sel!.EndpointMuted);

            aAPI.GetDefaultDevice().EndpointMuted = false;
        }

        [TestMethod()]
        public void ToggleDeviceMute_Test()
        {
            AudioAPI aAPI = new();

            // when selected isn't null:
            aAPI.SelectedDevice = aAPI.GetDefaultDevice();
            aAPI.SelectedDevice.EndpointMuted = false;
            Assertx.NoThrows(() => { aAPI.ToggleDeviceMute(); });
            Assertx.True(aAPI.GetDeviceMute());
            Assertx.NoThrows(() => { aAPI.ToggleDeviceMute(); }); //< toggle device mute back to previous state
            Assertx.False(aAPI.GetDeviceMute());
            // when selected is null:
            aAPI.SelectedDevice = null;
            Assertx.NoThrows(() => { aAPI.ToggleDeviceMute(); });

            aAPI.GetDefaultDevice().EndpointMuted = false;
        }

        [TestMethod()]
        public void SelectNextDevice_Test()
        {
            AudioAPI aAPI = new();
            // when selected is null:
            aAPI.SelectedDevice = null;
            Assertx.NoThrows(() => { aAPI.SelectNextDevice(); });
            // when devices list is empty:
            aAPI.Devices.Clear();
            Assertx.NoThrows(() => { aAPI.SelectNextDevice(); });
        }

        [TestMethod()]
        public void SelectPreviousDevice_Test()
        {
            AudioAPI aAPI = new();
            // when devices list is unknown:
            Assertx.NoThrows(() => { aAPI.SelectPreviousDevice(); });
            // when devices list is empty:
            aAPI.Devices.Clear();
            Assertx.NoThrows(() => { aAPI.SelectPreviousDevice(); });
            // when devices list isn't empty:
            aAPI.Devices.Add(aAPI.GetDefaultDevice());
            Assertx.NoThrows(() => { aAPI.SelectPreviousDevice(); });
        }

        [TestMethod()]
        public void DeselectDevice_Test()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;
            // when selected is not null:
            aAPI.SelectedDevice = aAPI.GetDefaultDevice();
            Assertx.NoThrows(() => { Assertx.True(aAPI.DeselectDevice()); });
            // when selected is null:
            aAPI.SelectedDevice = null;
            Assertx.NoThrows(() => { Assertx.False(aAPI.DeselectDevice()); });
            // when devices list is empty:
            aAPI.Devices.Clear();
            Assertx.NoThrows(() => { aAPI.DeselectDevice(); });
        }

        [TestMethod()]
        public void SelectDefaultDevice_Test()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;
            // selected is already set to default:
            aAPI.SelectDefaultDevice();
            Assertx.False(aAPI.SelectDefaultDevice());
            // selected is null:
            aAPI.SelectedDevice = null;
            Assertx.True(aAPI.SelectDefaultDevice());
        }

        [TestMethod()]
        public void Dispose_Test()
        {
            AudioAPI aAPI = new();
            Assertx.NoThrows(() => { aAPI.Dispose(); });
        }
    }
}