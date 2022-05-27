using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
        }

        [TestMethod()]
        public void ReloadDeviceList_Test()
        {
            AudioAPI aAPI = new();
            aAPI.SelectedDeviceSwitched += Assertx.OnEvent;
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
        public void ReloadSessionList_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void FindSession_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void FindSessionWithID_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void FindSessionWithName_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void FindSessionWithIdentifier_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void GetSessionNames_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void IncrementSessionVolume_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void IncrementSessionVolume_Test1()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void DecrementSessionVolume_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void DecrementSessionVolume_Test1()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void GetSessionMute_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void SetSessionMute_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void ToggleSessionMute_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void SelectNextSession_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void SelectPreviousSession_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void DeselectSession_Test()
        {
            AudioAPI aAPI = new();

        }

        [TestMethod()]
        public void IncrementDeviceVolume_Test()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;

            var sel = aAPI.SelectedDevice;
            Assertx.NotNull(sel);

            sel!.EndpointVolume = 0;
            Assertx.NoThrows(() => aAPI.IncrementDeviceVolume(20));
            Assertx.Equal(sel!.EndpointVolume, 20);

            sel!.EndpointVolume = 90;
            Assertx.NoThrows(()=> aAPI.IncrementDeviceVolume(20));
            Assertx.Equal(sel!.EndpointVolume, 100);

            sel!.EndpointVolume = 100;
            Assertx.Equal(sel!.EndpointVolume, 100);
            Assertx.NoThrows(()=> aAPI.IncrementDeviceVolume(20));
            Assertx.Equal(sel!.EndpointVolume, 100);
        }

        [TestMethod()]
        public void IncrementDeviceVolume_Test1()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;

        }

        [TestMethod()]
        public void DecrementDeviceVolume_Test()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;

        }

        [TestMethod()]
        public void DecrementDeviceVolume_Test1()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;

        }

        [TestMethod()]
        public void GetDeviceMute_Test()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;

            Assertx.NoThrows(() => Assertx.NotNull(aAPI.GetDeviceMute()));
        }

        [TestMethod()]
        public void SetDeviceMute_Test()
        {
            AudioAPI aAPI = new();
            aAPI.CheckAllDevices = false;


        }

        [TestMethod()]
        public void ToggleDeviceMute_Test()
        {
            AudioAPI aAPI = new();

            // when selected isn't null:
            aAPI.SelectedDevice = aAPI.GetDefaultDevice();
            Assertx.NoThrows(() => { aAPI.ToggleDeviceMute(); });
            Assertx.NoThrows(() => { aAPI.ToggleDeviceMute(); }); //< toggle device mute back to previous state
            // when selected is null:
            aAPI.SelectedDevice = null;
            Assertx.NoThrows(() => { aAPI.ToggleDeviceMute(); });
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