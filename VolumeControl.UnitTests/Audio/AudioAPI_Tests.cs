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
        public void AudioAPI_Test()
        {
            // Smoke Test:
            AudioAPI api = new();

            // Settings Test:
            Assertx.Same(api.LockSelectedSession, Settings.LockTargetSession);
            Assertx.RegexMatch(api.Target, AudioSession.ParseProcessIdentifier(Settings.TargetSession).Item2, "Target property wasn't set correctly!");
            Assertx.Same(api.VolumeStepSize, Settings.VolumeStepSize);
        }

        [TestMethod()]
        public void ForceReloadAudioDevices_Test()
        {
            AudioAPI api = new();

            var devCount = api.Devices.Count;
            Assertx.Greater(devCount, 0, "Device count is 0; cannot perform this test!");
            Assertx.EventTrigger ev = new(true) { MinCount = 1 };
            api.Devices.CollectionChanged += ev.Handler;
            Assertx.NoThrows(() => api.ForceReloadAudioDevices());
            Assertx.Equal(api.Devices.Count, devCount);
            ev.Armed = false;
        }

        [TestMethod()]
        public void FindDevice_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void FindDeviceWithID_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ForceReloadSessionList_Test()
        {
            AudioAPI api = new();

            var sessionCount = api.Sessions.Count;
            Assertx.Greater(sessionCount, 0, "Session count is 0; cannot perform this test!");
            Assertx.EventTrigger ev = new(true) { MinCount = 1 };
            api.Sessions.CollectionChanged += ev.Handler;
            Assertx.NoThrows(() => api.ForceReloadSessionList());
            Assertx.Equal(api.Sessions.Count, sessionCount);
            ev.Armed = false;
        }

        [TestMethod()]
        public void FindSession_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void FindSessionWithID_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void FindSessionWithName_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void FindSessionWithIdentifier_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetSessionNames_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetSessionVolume_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void SetSessionVolume_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void IncrementSessionVolume_Test()
        {
            AudioAPI api = new();

            api.LockSelectedSession = false;

            Assertx.NotEmpty(api.Sessions, $"{nameof(api.Sessions)} is empty! Cannot perform this test.");

            // When selected session is null:
            api.SelectedSession = null;
            Assertx.NoThrows(() => api.IncrementSessionVolume(100));

            // When selected session is not null:
            api.SelectedSession = api.Sessions.First();

            // ... & volume is 0:
            api.SelectedSession.Volume = 0;
            Assertx.NoThrows(() => api.IncrementSessionVolume(0));
            Assertx.Equal(api.SelectedSession.Volume, 0);

            api.SelectedSession.Volume = 0;
            api.IncrementSessionVolume(1);
            Assertx.Equal(api.SelectedSession.Volume, 1);

            api.SelectedSession.Volume = 0;
            api.IncrementSessionVolume(50);
            Assertx.Equal(api.SelectedSession.Volume, 50);

            api.SelectedSession.Volume = 0;
            api.IncrementSessionVolume(100);
            Assertx.Equal(api.SelectedSession.Volume, 100);

            api.SelectedSession.Volume = 0;
            api.IncrementSessionVolume(200);
            Assertx.Equal(api.SelectedSession.Volume, 100);

            // ... & volume is 100:
            api.SelectedSession.Volume = 100;
            Assertx.NoThrows(() => api.IncrementSessionVolume(0));
            Assertx.Equal(api.SelectedSession.Volume, 100);

            api.SelectedSession.Volume = 100;
            api.IncrementSessionVolume(1);
            Assertx.Equal(api.SelectedSession.Volume, 100);

            api.SelectedSession.Volume = 100;
            api.IncrementSessionVolume(50);
            Assertx.Equal(api.SelectedSession.Volume, 100);

            api.SelectedSession.Volume = 100;
            api.IncrementSessionVolume(100);
            Assertx.Equal(api.SelectedSession.Volume, 100);

            api.SelectedSession.Volume = 100;
            api.IncrementSessionVolume(200);
            Assertx.Equal(api.SelectedSession.Volume, 100);
        }

        [TestMethod()]
        public void IncrementSessionVolume_Implicit_Test()
        {
            AudioAPI api = new();

            api.LockSelectedSession = false;

            Assertx.NotEmpty(api.Sessions, $"{nameof(api.Sessions)} is empty! Cannot perform this test.");

            // When selected session is null:
            api.SelectedSession = null;
            api.VolumeStepSize = 100;
            Assertx.NoThrows(() => api.IncrementSessionVolume());

            // When selected session is not null:
            api.SelectedSession = api.Sessions.First();

            // ... & volume is 0:
            api.SelectedSession.Volume = 0;
            api.VolumeStepSize = 0;
            Assertx.NoThrows(() => api.IncrementSessionVolume());
            Assertx.Equal(api.SelectedSession.Volume, 0);

            api.SelectedSession.Volume = 0;
            api.VolumeStepSize = 1;
            api.IncrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 1);

            api.SelectedSession.Volume = 0;
            api.VolumeStepSize = 50;
            api.IncrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 50);

            api.SelectedSession.Volume = 0;
            api.VolumeStepSize = 100;
            api.IncrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 100);

            api.SelectedSession.Volume = 0;
            api.VolumeStepSize = 200;
            api.IncrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 100);

            // ... & volume is 100:
            api.SelectedSession.Volume = 100;
            api.VolumeStepSize = 0;
            Assertx.NoThrows(() => api.IncrementSessionVolume());
            Assertx.Equal(api.SelectedSession.Volume, 100);

            api.SelectedSession.Volume = 100;
            api.VolumeStepSize = 1;
            api.IncrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 100);

            api.SelectedSession.Volume = 100;
            api.VolumeStepSize = 50;
            api.IncrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 100);

            api.SelectedSession.Volume = 100;
            api.VolumeStepSize = 100;
            api.IncrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 100);

            api.SelectedSession.Volume = 100;
            api.VolumeStepSize = 200;
            api.IncrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 100);
        }

        [TestMethod()]
        public void DecrementSessionVolume_Test()
        {
            AudioAPI api = new();

            api.LockSelectedSession = false;

            Assertx.NotEmpty(api.Sessions, $"{nameof(api.Sessions)} is empty! Cannot perform this test.");

            // When selected session is null:
            api.SelectedSession = null;
            Assertx.NoThrows(() => api.DecrementSessionVolume(100));

            // When selected session is not null:
            api.SelectedSession = api.Sessions.First();

            // ... & volume is 0:
            api.SelectedSession.Volume = 0;
            Assertx.NoThrows(() => api.DecrementSessionVolume(0));
            Assertx.Equal(api.SelectedSession.Volume, 0);

            api.SelectedSession.Volume = 0;
            api.DecrementSessionVolume(1);
            Assertx.Equal(api.SelectedSession.Volume, 0);

            api.SelectedSession.Volume = 0;
            api.DecrementSessionVolume(50);
            Assertx.Equal(api.SelectedSession.Volume, 0);

            api.SelectedSession.Volume = 0;
            api.DecrementSessionVolume(100);
            Assertx.Equal(api.SelectedSession.Volume, 0);

            api.SelectedSession.Volume = 0;
            api.DecrementSessionVolume(200);
            Assertx.Equal(api.SelectedSession.Volume, 0);

            // ... & volume is 100:
            api.SelectedSession.Volume = 100;
            Assertx.NoThrows(() => api.DecrementSessionVolume(0));
            Assertx.Equal(api.SelectedSession.Volume, 100);

            api.SelectedSession.Volume = 100;
            api.DecrementSessionVolume(1);
            Assertx.Equal(api.SelectedSession.Volume, 99);

            api.SelectedSession.Volume = 100;
            api.DecrementSessionVolume(50);
            Assertx.Equal(api.SelectedSession.Volume, 50);

            api.SelectedSession.Volume = 100;
            api.DecrementSessionVolume(100);
            Assertx.Equal(api.SelectedSession.Volume, 0);

            api.SelectedSession.Volume = 100;
            api.DecrementSessionVolume(200);
            Assertx.Equal(api.SelectedSession.Volume, 0);
        }

        [TestMethod()]
        public void DecrementSessionVolume_Implicit_Test()
        {
            AudioAPI api = new();

            api.LockSelectedSession = false;

            Assertx.NotEmpty(api.Sessions, $"{nameof(api.Sessions)} is empty! Cannot perform this test.");

            // When selected session is null:
            api.SelectedSession = null;
            api.VolumeStepSize = 100;
            Assertx.NoThrows(() => api.DecrementSessionVolume());

            // When selected session is not null:
            api.SelectedSession = api.Sessions.First();

            // ... & volume is 0:
            api.SelectedSession.Volume = 0;
            api.VolumeStepSize = 0;
            Assertx.NoThrows(() => api.DecrementSessionVolume());
            Assertx.Equal(api.SelectedSession.Volume, 0);

            api.SelectedSession.Volume = 0;
            api.VolumeStepSize = 1;
            api.DecrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 0);

            api.SelectedSession.Volume = 0;
            api.VolumeStepSize = 50;
            api.DecrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 0);

            api.SelectedSession.Volume = 0;
            api.VolumeStepSize = 100;
            api.DecrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 0);

            api.SelectedSession.Volume = 0;
            api.VolumeStepSize = 200;
            api.DecrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 0);

            // ... & volume is 100:
            api.SelectedSession.Volume = 100;
            api.VolumeStepSize = 0;
            Assertx.NoThrows(() => api.DecrementSessionVolume());
            Assertx.Equal(api.SelectedSession.Volume, 100);

            api.SelectedSession.Volume = 100;
            api.VolumeStepSize = 1;
            api.DecrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 99);

            api.SelectedSession.Volume = 100;
            api.VolumeStepSize = 50;
            api.DecrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 50);

            api.SelectedSession.Volume = 100;
            api.VolumeStepSize = 100;
            api.DecrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 0);

            api.SelectedSession.Volume = 100;
            api.VolumeStepSize = 200;
            api.DecrementSessionVolume();
            Assertx.Equal(api.SelectedSession.Volume, 0);
        }

        [TestMethod()]
        public void GetSessionMute_Test()
        {
            AudioAPI api = new();

            Assertx.NotEmpty(api.Sessions, $"{nameof(api.Sessions)} is empty; cannot perform this test!");

            // When selected session is null:
            api.SelectedSession = null;

            Assertx.NoThrows(() => Assertx.False(api.GetSessionMute()));

            // When selected session is not null:
            api.SelectedSession = api.Sessions.First();

            // ... & mute is true:
            api.SelectedSession.Muted = true;
            Assertx.NoThrows(() => Assertx.True(api.GetSessionMute()));

            // ... & mute is false
            api.SelectedSession.Muted = false;
            Assertx.NoThrows(() => Assertx.False(api.GetSessionMute()));
        }

        [TestMethod()]
        public void SetSessionMute_Test()
        {
            AudioAPI api = new();

            Assertx.NotEmpty(api.Sessions, $"{nameof(api.Sessions)} is empty; cannot perform this test!");

            // When selected session is null:
            api.SelectedSession = null;

            Assertx.NoThrows(() => Assertx.Null(api.SetSessionMute(true)));

            // When selected session is not null:
            api.SelectedSession = api.Sessions.First();

            // ... & mute is true:
            Assertx.NoThrows(() => Assertx.True(api.SetSessionMute(true)));

            // ... & mute is false
            Assertx.NoThrows(() => Assertx.False(api.SetSessionMute(false)));
        }

        [TestMethod()]
        public void ToggleSessionMute_Test()
        {
            AudioAPI api = new();

            Assertx.NotEmpty(api.Sessions, $"{nameof(api.Sessions)} is empty; cannot perform this test!");

            // When selected session is null:
            api.SelectedSession = null;

            Assertx.NoThrows(() => Assertx.Null(api.ToggleSessionMute()));

            // When selected session is not null:
            api.SelectedSession = api.Sessions.First();

            // ... & mute is true:
            api.SelectedSession.Muted = true;
            Assertx.NoThrows(() => Assertx.False(api.ToggleSessionMute()));

            // ... & mute is false
            api.SelectedSession.Muted = false;
            Assertx.NoThrows(() => Assertx.True(api.ToggleSessionMute()));
        }
    }
}