using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VolumeControl.UnitTests
{
    public static class Assertx
    {
        /// <summary>Checks if <paramref name="obj"/> is the same type as <typeparamref name="T"/>.</summary>
        public static void Is<T>(object obj, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            var type = typeof(T);
            if (!obj.GetType().Equals(type)) Assert.Fail($"'{obj.GetType().FullName}' is not the same type as '{type.FullName}'!\n[{path}:{ln}]");
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
            if (!typeof(T).GetType().Equals(type)) Assert.Fail($"'{typeof(T).FullName}' is not the same type as '{type.FullName}'!\n[{path}:{ln}]");
        }
        /// <summary>Checks if <paramref name="obj"/> is not the same type as <typeparamref name="T"/>.</summary>
        public static void IsNot<T>(Type type, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (typeof(T).Equals(type)) Assert.Fail($"'{typeof(T).FullName}' is the same type as '{type.FullName}'!\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with <b>non-numeric</b> types.<br/>Checks if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        public static void Same<T>(T left, T right, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "") where T : IEquatable<T>
        {
            if (!left.Equals(right)) Assert.Fail($"'{left}' is not the same as '{right}'!\n[{path}:{ln}]");
        }
        public static void Same(bool left, bool right, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(left == right)) Assert.Fail($"'{left}' is not the same as '{right}'\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with <b>non-numeric</b> types.<br/>Checks if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        public static void NotSame<T>(T left, T right, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "") where T : IEquatable<T>
        {
            if (!!left.Equals(right)) Assert.Fail($"'{left}' is not the same as '{right}'!\n[{path}:{ln}]");
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
        /// <summary>Checks if the given object is null.</summary>
        public static void Null(object? obj, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(obj == null))
                Assert.Fail($"{nameof(obj)} is not null!\n[{path}:{ln}]");
        }
        /// <summary>Checks if the given object isn't null.</summary>
        public static void NotNull(object? obj, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(obj != null))
                Assert.Fail($"{nameof(obj)} is null!\n[{path}:{ln}]");
        }
        /// <summary>Checks if the given action throws any exception.</summary>
        public static void Throws(Action action, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            try
            {
                action();
                Assert.Fail($"{nameof(action)} didn't throw an exception!\n[{path}:{ln}]");
            }
            catch (AssertFailedException) { throw; } //< rethrow assertion failures instead of throwing a new exception
            catch (Exception) { }
        }
        public static void Throws<T>(Action action, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "") where T : Exception
        {
            try
            {
                action();
                Assert.Fail($"{nameof(action)} didn't throw an exception!\n[{path}:{ln}]");
            }
            catch (T) { }
            catch (AssertFailedException) { throw; } //< rethrow assertion failures instead of throwing a new exception
            catch (Exception ex)
            {
                Assert.Fail($"{nameof(action)} threw an exception of type {ex.GetType().FullName}; expected type {typeof(T).FullName}!\n[{path}:{ln}]");
            }
        }
        /// <summary>Checks if the given action doesn't throw any exception.</summary>
        public static void NoThrows(Action action, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            try
            {
                action();
            }
            catch (AssertFailedException) { throw; } //< rethrow assertion failures instead of throwing a new exception
            catch (Exception ex)
            {
                Assert.Fail($"{nameof(action)} threw an exception: '{ex.Message}'\n{ex.StackTrace}\n[{path}:{ln}]");
            }
        }
        /// <summary>Checks if the given action doesn't throw any exception.</summary>
        public static void NoThrows<T>(Action action, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "") where T : Exception
        {
            try
            {
                action();
            }
            catch (T ex)
            {
                Assert.Fail($"{nameof(action)} threw an exception: '{ex.Message}'\n{ex.StackTrace}\n[{path}:{ln}]");
            }
            catch (AssertFailedException) { throw; } //< rethrow assertion failures instead of throwing a new exception
            catch (Exception) { }
        }
        /// <summary>Checks if the given boolean expression is true.</summary>
        public static void True(bool expression, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!expression)
                Assert.Fail($"'{nameof(expression)}' is false!\n[{path}:{ln}]");
        }
        /// <summary>Checks if the given boolean expression is false.</summary>
        public static void False(bool expression, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (expression)
                Assert.Fail($"'{nameof(expression)}' is true!\n[{path}:{ln}]");
        }

        /// <summary>Check if all enumerated objects return true with the given predicate.</summary>
        public static void All<T>(IEnumerable<T> enumerable, Predicate<T> predicate, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            int i = 0;
            foreach (T item in enumerable)
            {
                if (!predicate(item))
                    Assert.Fail($"Predicate returned false for item {i}:  '{item}'\n[{path}:{ln}]");
                ++i;
            }
        }
        /// <summary>Check if any enumerated object returns true with the given predicate.</summary>
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
        /// <summary>
        /// This event may be used as the source for an event handler to test.<br/>
        /// Use <see cref="NotifyFromEvent"/> to trigger this event.
        /// </summary>
        public static event EventHandler? FromEvent;
        public static void NotifyFromEvent(object? s, EventArgs e) => FromEvent?.Invoke(s, e);
        public static void NotifyFromEvent(object? s) => FromEvent?.Invoke(s, new());
        public static void NotifyFromEvent() => FromEvent?.Invoke(null, new());
    }
}