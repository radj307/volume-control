using Microsoft.VisualStudio.TestTools.UnitTesting;
using VolumeControl.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Devices;
using NAudio.CoreAudioApi;
using System.Runtime.CompilerServices;

namespace VolumeControl.UnitTests.VolumeControl.Audio
{
    public static class Assertx
    {
        /// <summary>Checks if <paramref name="obj"/> is the same type as <typeparamref name="T"/>.</summary>
        public static void Is<T>(object obj, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            var type = typeof(T);
            if (!(obj.GetType().Equals(type))) Assert.Fail($"'{obj.GetType().FullName}' is not the same type as '{type.FullName}'!\n[{path}:{ln}]");
        }
        /// <summary>Checks if <paramref name="obj"/> is not the same type as <typeparamref name="T"/>.</summary>
        public static void IsNot<T>(object obj, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            var type = typeof(T);
            if (obj.GetType().Equals(type)) Assert.Fail($"'{obj.GetType().FullName}' is the same type as '{type.FullName}'!\n[{path}:{ln}]");
        }
        /// <summary>Checks if <paramref name="obj"/> is the same type as <typeparamref name="T"/>.</summary>
        public static void Is<T>(Type type, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(typeof(T).GetType().Equals(type))) Assert.Fail($"'{typeof(T).FullName}' is not the same type as '{type.FullName}'!\n[{path}:{ln}]");
        }
        /// <summary>Checks if <paramref name="obj"/> is not the same type as <typeparamref name="T"/>.</summary>
        public static void IsNot<T>(Type type, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (typeof(T).Equals(type)) Assert.Fail($"'{typeof(T).FullName}' is the same type as '{type.FullName}'!\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with <b>non-numeric</b> types.<br/>Checks if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        public static void Same<T>(T left, T right, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "") where T : IEquatable<T>
        {
            if (!(left.Equals(right))) Assert.Fail($"'{left}' is not the same as '{right}'!\n[{path}:{ln}]");
        }
        public static void Same(bool left, bool right, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(left == right)) Assert.Fail($"'{left}' is not the same as '{right}'\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with <b>non-numeric</b> types.<br/>Checks if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        public static void NotSame<T>(T left, T right, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "") where T : IEquatable<T>
        {
            if (!(!left.Equals(right))) Assert.Fail($"'{left}' is not the same as '{right}'!\n[{path}:{ln}]");
        }
        public static void NotSame(bool left, bool right, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(left != right)) Assert.Fail($"'{left}' is not the same as '{right}'\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with numeric types.</summary>
        public static void Equal(dynamic left, dynamic right, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(left == right)) Assert.Fail($"'{left}' is not equal to '{right}'\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with numeric types.</summary>
        public static void Less(dynamic number, dynamic threshold, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(number < threshold)) Assert.Fail($"'{number}' is not less than '{threshold}'!\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with numeric types.</summary>
        public static void LessOrEqual(dynamic number, dynamic threshold, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(number <= threshold)) Assert.Fail($"'{number}' is not less than or equal to '{threshold}'!\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with numeric types.</summary>
        public static void Greater(dynamic number, dynamic threshold, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(number > threshold)) Assert.Fail($"'{number}' is not greater than '{threshold}'!\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with numeric types.</summary>
        public static void GreaterOrEqual(dynamic number, dynamic threshold, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(number >= threshold)) Assert.Fail($"'{number}' is not greater or equal to '{threshold}'!\n[{path}:{ln}]");
        }
        public static void Null(object? obj, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(obj == null))
                Assert.Fail($"{nameof(obj)} is not null!\n[{path}:{ln}]");
        }
        public static void NotNull(object? obj, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(obj != null))
                Assert.Fail($"{nameof(obj)} is null!\n[{path}:{ln}]");
        }
        public static void Throws(Action action, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            try
            {
                action();
                Assert.Fail($"{nameof(action)} didn't throw an exception!\n[{path}:{ln}]");
            }
            catch (Exception) { }
        }
        public static void NoThrows(Action action, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Assert.Fail($"{nameof(action)} threw an exception: '{ex.Message}'\n{ex.StackTrace}\n[{path}:{ln}]");
            }
        }
        public static void True(bool expression, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(expression))
                Assert.Fail($"'{nameof(expression)}' is false!\n[{path}:{ln}]");
        }
        public static void False(bool expression, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(!expression))
                Assert.Fail($"'{nameof(expression)}' is true!\n[{path}:{ln}]");
        }

        public static void All<T>(IEnumerable<T> enumerable, Predicate<T> predicate, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            int i = 0;
            foreach (T item in enumerable)
            {
                if (!(predicate(item)))
                    Assert.Fail($"Predicate returned false for item {i}:  '{item}'\n[{path}:{ln}]");
                ++i;
            }
        }
        public static void Any<T>(IEnumerable<T> enumerable, Predicate<T> predicate, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            int i = 0;
            foreach (T item in enumerable)
            {
                if (!predicate(item))
                    Assert.Fail($"Predicate returned true for item {i}:  '{item}'\n[{path}:{ln}]");
                ++i;
            }
        }
        /// <summary>Causes an assertion failure if triggered by an event.<br/>This can be used as an event handler for any event with a signature similar to <see cref="EventHandler"/>.</summary>
        /// <param name="s">Sender Parameter.</param>
        /// <param name="e">Event Arguments Parameter.</param>
        public static void OnEvent(object? _, object? _1) => Assert.Fail("Event Triggered!");

        public static event EventHandler? FromEvent;
        public static void NotifyFromEvent(object? s, EventArgs e) => FromEvent?.Invoke(s, e);
        public static void NotifyFromEvent(object? s) => FromEvent?.Invoke(s, new());
        public static void NotifyFromEvent() => FromEvent?.Invoke(null, new());
    }

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

        }

        [TestMethod()]
        public void FindDevice_Test()
        {

        }

        [TestMethod()]
        public void ReloadSessionList_Test()
        {

        }

        [TestMethod()]
        public void FindSession_Test()
        {

        }

        [TestMethod()]
        public void FindSessionWithID_Test()
        {

        }

        [TestMethod()]
        public void FindSessionWithName_Test()
        {

        }

        [TestMethod()]
        public void FindSessionWithIdentifier_Test()
        {

        }

        [TestMethod()]
        public void GetSessionNames_Test()
        {

        }

        [TestMethod()]
        public void IncrementSessionVolume_Test()
        {

        }

        [TestMethod()]
        public void IncrementSessionVolume_Test1()
        {

        }

        [TestMethod()]
        public void DecrementSessionVolume_Test()
        {

        }

        [TestMethod()]
        public void DecrementSessionVolume_Test1()
        {

        }

        [TestMethod()]
        public void GetSessionMute_Test()
        {

        }

        [TestMethod()]
        public void SetSessionMute_Test()
        {

        }

        [TestMethod()]
        public void ToggleSessionMute_Test()
        {

        }

        [TestMethod()]
        public void SelectNextSession_Test()
        {

        }

        [TestMethod()]
        public void SelectPreviousSession_Test()
        {

        }

        [TestMethod()]
        public void DeselectSession_Test()
        {

        }

        [TestMethod()]
        public void IncrementDeviceVolume_Test()
        {

        }

        [TestMethod()]
        public void IncrementDeviceVolume_Test1()
        {

        }

        [TestMethod()]
        public void DecrementDeviceVolume_Test()
        {

        }

        [TestMethod()]
        public void DecrementDeviceVolume_Test1()
        {

        }

        [TestMethod()]
        public void GetDeviceMute_Test()
        {

        }

        [TestMethod()]
        public void SetDeviceMute_Test()
        {

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