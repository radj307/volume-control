using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace VolumeControl.UnitTests
{
    /// <summary>
    /// Custom &amp; improved assertion methods for unit testing using the <see cref="Assert"/> class.
    /// </summary>
    public static class Assertx
    {
        #region Type
        /// <summary>Asserts that <paramref name="obj"/> is the same type as <typeparamref name="T"/>.</summary>
        public static void Is<T>(object obj, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            var type = typeof(T);
            if (!obj.GetType().Equals(type)) Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{obj.GetType().FullName}' is not the same type as '{type.FullName}'!\n[{path}:{ln}]");
        }
        /// <summary>Asserts that <paramref name="obj"/> is not the same type as <typeparamref name="T"/>.</summary>
        public static void IsNot<T>(object obj, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            var type = typeof(T);
            if (obj.GetType().Equals(type)) Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{obj.GetType().FullName}' is the same type as '{type.FullName}'!\n[{path}:{ln}]");
        }
        /// <summary>Asserts that <paramref name="obj"/> is the same type as <typeparamref name="T"/>.</summary>
        public static void Is<T>(Type type, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!typeof(T).GetType().Equals(type)) Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{typeof(T).FullName}' is not the same type as '{type.FullName}'!\n[{path}:{ln}]");
        }
        /// <summary>Asserts that <paramref name="obj"/> is not the same type as <typeparamref name="T"/>.</summary>
        public static void IsNot<T>(Type type, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (typeof(T).Equals(type)) Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{typeof(T).FullName}' is the same type as '{type.FullName}'!\n[{path}:{ln}]");
        }
        #endregion Type

        #region Equate
        /// <summary>Intended to be used with <b>non-numeric</b> types.<br/>Asserts that <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        public static void Same<T>(T left, T right, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "") where T : IEquatable<T>
        {
            if (!left.Equals(right)) Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{left}' is not the same as '{right}'!\n[{path}:{ln}]");
        }
        public static void Same(bool left, bool right, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(left == right)) Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{left}' is not the same as '{right}'\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with <b>non-numeric</b> types.<br/>Asserts that <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        public static void NotSame<T>(T left, T right, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "") where T : IEquatable<T>
        {
            if (!!left.Equals(right)) Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{left}' is not the same as '{right}'!\n[{path}:{ln}]");
        }
        public static void NotSame(bool left, bool right, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(left != right)) Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{left}' is not the same as '{right}'\n[{path}:{ln}]");
        }
        #endregion Equate

        #region NumericComparison
        /// <summary>Intended to be used with numeric types.</summary>
        public static void Equal(dynamic left, dynamic right, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(left == right)) Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{left}' is not equal to '{right}'\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with numeric types.</summary>
        public static void Less(dynamic number, dynamic threshold, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(number < threshold)) Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{number}' is not less than '{threshold}'!\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with numeric types.</summary>
        public static void LessOrEqual(dynamic number, dynamic threshold, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(number <= threshold)) Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{number}' is not less than or equal to '{threshold}'!\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with numeric types.</summary>
        public static void Greater(dynamic number, dynamic threshold, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(number > threshold)) Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{number}' is not greater than '{threshold}'!\n[{path}:{ln}]");
        }
        /// <summary>Intended to be used with numeric types.</summary>
        public static void GreaterOrEqual(dynamic number, dynamic threshold, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(number >= threshold)) Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{number}' is not greater or equal to '{threshold}'!\n[{path}:{ln}]");
        }
        #endregion NumericComparison

        #region Nullable
        /// <summary>Asserts that the given object is null.</summary>
        public static void Null(object? obj, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(obj == null))
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}{nameof(obj)} is not null!\n[{path}:{ln}]");
        }
        /// <summary>Asserts that the given object isn't null.</summary>
        public static void NotNull(object? obj, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(obj != null))
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}{nameof(obj)} is null!\n[{path}:{ln}]");
        }
        #endregion Nullable

        #region Exception
        /// <summary>Asserts that the given action throws any exception.</summary>
        public static void Throws(Action action, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            try
            {
                action();
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}{nameof(action)} didn't throw an exception!\n[{path}:{ln}]");
            }
            catch (AssertFailedException) { throw; } //< rethrow assertion failures instead of throwing a new exception
            catch (Exception) { }
        }
        public static void Throws<T>(Action action, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "") where T : Exception
        {
            try
            {
                action();
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}{nameof(action)} didn't throw an exception!\n[{path}:{ln}]");
            }
            catch (T) { }
            catch (AssertFailedException) { throw; } //< rethrow assertion failures instead of throwing a new exception
            catch (Exception ex)
            {
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}{nameof(action)} threw an exception of type {ex.GetType().FullName}; expected type {typeof(T).FullName}!\n[{path}:{ln}]");
            }
        }
        /// <summary>Asserts that the given action doesn't throw any exception.</summary>
        public static void NoThrows(Action action, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            try
            {
                action();
            }
            catch (AssertFailedException) { throw; } //< rethrow assertion failures instead of throwing a new exception
            catch (Exception ex)
            {
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}{nameof(action)} threw an exception: '{ex.Message}'\n{ex.StackTrace}\n[{path}:{ln}]");
            }
        }
        /// <summary>Asserts that the given action doesn't throw any exception.</summary>
        public static void NoThrows<T>(Action action, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "") where T : Exception
        {
            try
            {
                action();
            }
            catch (T ex)
            {
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}{nameof(action)} threw an exception: '{ex.Message}'\n{ex.StackTrace}\n[{path}:{ln}]");
            }
            catch (AssertFailedException) { throw; } //< rethrow assertion failures instead of throwing a new exception
            catch (Exception) { }
        }
        #endregion Exception

        #region Boolean
        /// <summary>Asserts that the given boolean expression is true.</summary>
        public static void True(bool expression, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!expression)
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{nameof(expression)}' is false!\n[{path}:{ln}]");
        }
        /// <summary>Asserts that the given boolean expression is false.</summary>
        public static void False(bool expression, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (expression)
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{nameof(expression)}' is true!\n[{path}:{ln}]");
        }
        /// <summary>Asserts that the given nullable boolean expression is true.</summary>
        public static void True(bool? expression, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!expression.HasValue || !expression.Value)
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{nameof(expression)}' is false!\n[{path}:{ln}]");
        }
        /// <summary>Asserts that the given nullable boolean expression is false.</summary>
        public static void False(bool? expression, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!expression.HasValue || expression.Value)
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}'{nameof(expression)}' is true!\n[{path}:{ln}]");
        }
        #endregion Boolean

        #region Enumerable
        /// <summary>Assert that all enumerated objects return true with the given predicate.</summary>
        public static void All<T>(IEnumerable<T> enumerable, Predicate<T> predicate, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            int i = 0;
            foreach (T item in enumerable)
            {
                if (!predicate(item))
                    Assert.Fail($"{msg}{(msg == null ? "" : "\n")}Predicate returned false for item {i}:  '{item}'\n[{path}:{ln}]");
                ++i;
            }
        }
        /// <summary>Assert that any enumerated object returns true with the given predicate.</summary>
        public static void Any<T>(IEnumerable<T> enumerable, Predicate<T> predicate, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            int i = 0;
            foreach (T item in enumerable)
            {
                if (!predicate(item))
                    Assert.Fail($"{msg}{(msg == null ? "" : "\n")}Predicate returned true for item {i}:  '{item}'\n[{path}:{ln}]");
                ++i;
            }
        }
        /// <summary>Assert that <paramref name="collection"/> is empty.</summary>
        public static void Empty(ICollection collection, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(collection.Count == 0))
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}{nameof(collection)} is not empty!\n[{path}:{ln}]");
        }
        /// <summary>Assert that <paramref name="collection"/> is not empty.</summary>
        public static void NotEmpty(ICollection collection, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(collection.Count != 0))
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}{nameof(collection)} is empty!\n[{path}:{ln}]");
        }
        /// <summary>Assert that the <paramref name="list"/> contains <paramref name="obj"/>.</summary>
        public static void Contains(IList list, object? obj, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!list.Contains(obj))
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}List doesn't contain {obj}!\n[{path}:{ln}]");
        }
        /// <summary>Assert that the <paramref name="list"/> contains <paramref name="obj"/>.</summary>
        public static void NotContains(IList list, object? obj, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (list.Contains(obj))
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}List contains {obj}!\n[{path}:{ln}]");
        }
        #endregion Enumerable

        #region String
        /// <summary>Assert that the regular expression <paramref name="regexp"/> matches some part of <paramref name="s"/>.</summary>
        public static void RegexMatch(string s, string regexp, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!Regex.Match(s, regexp, RegexOptions.Compiled).Success)
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}Regular expression '{regexp}' was unsuccessful!\n[{path}:{ln}]");
        }
        /// <summary>Assert that the regular expression <paramref name="regexp"/> doesn't match any part of <paramref name="s"/>.</summary>
        public static void RegexNoMatch(string s, string regexp, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (Regex.Match(s, regexp, RegexOptions.Compiled).Success)
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}Regular expression '{regexp}' was successful!\n[{path}:{ln}]");
        }
        /// <summary>Assert that <paramref name="s"/> is empty.</summary>
        public static void Empty(string s, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(s.Length == 0))
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}string is not empty!\n[{path}:{ln}]");
        }
        /// <summary>Assert that <paramref name="s"/> is not empty.</summary>
        public static void NotEmpty(string s, string? msg = null, [CallerLineNumber] int ln = 0, [CallerFilePath] string path = "")
        {
            if (!(s.Length != 0))
                Assert.Fail($"{msg}{(msg == null ? "" : "\n")}string is empty!\n[{path}:{ln}]");
        }
        #endregion String

        #region Event
        /// <summary>Causes an assertion failure if triggered by an event.<br/>This can be used as an event handler for any event with a signature similar to <see cref="EventHandler"/>.</summary>
        /// <remarks>Usage:<code language="cs">
        /// myEvent += Assertx.EventNotTriggered;
        /// 
        /// //&lt; If `myEvent` is triggered here, an assertion failure occurs.
        /// 
        /// myEvent -= Assertx.EventNotTriggered;
        /// </code></remarks>
        /// <param name="_">Sender Parameter.</param>
        /// <param name="_1">Event Arguments Parameter.</param>
        public static void NoEvent(object? _, object? _1) => Assert.Fail("Event Was Triggered!");

        /// <summary>
        /// A helper object for unit testing events, this class acts like an event trigger counter.<br/>
        /// See the <see cref="Armed"/> &amp; <see cref="Count"/> properties for more information.
        /// </summary>
        /// <remarks>Note that this object's finalizer sets <see cref="Armed"/> to <see langword="false"/>!</remarks>
        public class EventTrigger
        {
            public EventTrigger(bool arm = true, string? message = null)
            {
                _armed = arm;
                Message = message;
            }
            ~EventTrigger() => Armed = false;

            /// <summary><b>Default: <see langword="null"/></b><br/>Optional message to include with the assertion failure.</summary>
            public string? Message { get; set; }
            /// <summary><b>Default: <see langword="true"/></b><br/>When true, assertion failures are first caught to retrieve their stack trace which is parsed to find the name of the method that triggered the event; the built-in message's '[Method]' substring is replaced with the method name.</summary>
            public bool ResolveEventNameFromStackTrace { get; set; } = true;

            /// <summary>
            /// <b>Default: <see langword="false"/></b><br/>
            /// Gets or sets whether the event trigger is armed.<br/>
            /// <b>Note that setting this to <see langword="false"/> triggers the assertion check.</b><br/>
            /// Defaults to false unless set in the constructor.<br/><br/>
            /// Nothing happens if the underlying value doesn't actually change!<br/><i>(Setting this to <see langword="false"/> when it is already <see langword="false"/> cannot trigger assertion failures.)</i>
            /// </summary>
            public bool Armed
            {
                get => _armed;
                set
                {
                    if (_armed == value)
                        return;
                    if (value)
                    {
                        _armed = true;
                    }
                    else
                    {
                        _armed = false;

                        try
                        {
                            if (MinCount == -1)
                            {
                                if (Count > 0)
                                    Assert.Fail($"{Message}{(Message == null ? "" : "\n")}The event triggered by [Method] was triggered {Count} times!");
                            }
                            else if (Count < MinCount)
                                Assert.Fail($"{Message}{(Message == null ? "" : "\n")}The event triggered by [Method] was triggered {Count} times, but was expected to have been triggered at least {MinCount} times!");
                        }
                        catch (AssertFailedException ex)
                        {
                            if (!ResolveEventNameFromStackTrace)
                                throw;

                            string[] stackTrace = ex.StackTrace!.Split('\n');
                            foreach (string callsite in stackTrace)
                            {
                                var match = Regex.Match(callsite, "\\.(\\b[\\w ]+?\\b)\\(.*\\)", RegexOptions.Compiled);
                                if (!match.Success)
                                    continue;
                                string method = match.Groups[0].Value;
                                if (method.Length > 0)
                                    throw new AssertFailedException(ex.Message.Replace("[Method]", method), ex);
                            }
                        }
                    }
                }
            }
            private bool _armed = false;
            /// <summary><b>Default: 0</b><br/>The number of times that the event has been called.<br/>This does not directly throw assertion failures unless <see cref="AutoAssert"/> is <see langword="true"/>.<br/>When <see cref="Armed"/> is set to <see langword="false"/> and the counter is greater or equal to <see cref="MinCount"/>, an assertion failure occurs.</summary>
            public int Count
            {
                get => _count;
                private set
                {
                    _count = value;
                    try
                    {
                        if (AutoAssert && Count >= AutoAssertWhenCountIs)
                            Assert.Fail($"{Message}{(Message == null ? "" : "\n")}[{nameof(AutoAssert)}]:  The event triggered by [Method] was triggered {Count}/{AutoAssertWhenCountIs} times!");
                    }
                    catch (AssertFailedException ex)
                    {
                        if (!ResolveEventNameFromStackTrace)
                            throw;

                        string[] stackTrace = ex.StackTrace!.Split('\n');
                        foreach (string callsite in stackTrace)
                        {
                            var match = Regex.Match(callsite, "\\.(\\b[\\w ]+?\\b)\\(.*\\)", RegexOptions.Compiled);
                            if (!match.Success)
                                continue;
                            string method = match.Groups[0].Value;
                            if (method.Length > 0)
                                throw new AssertFailedException(ex.Message.Replace("[Method]", method, StringComparison.Ordinal), ex);
                        }
                    }
                }
            }
            private int _count = 0;
            /// <summary>Sets the minimum value that <see cref="Count"/> must reach to prevent an assertion from being thrown when disarmed.<br/>If this is set to -1, an assertion failure occurs when <see cref="Count"/> is any number greater than 0.<br/>Defaults to -1.<br/>This is checked when <see cref="Armed"/> is set to false.</summary>
            /// <remarks><b>Default: -1</b></remarks>
            public int MinCount { get; set; } = -1;
            /// <summary>
            /// <b>Default: <see langword="false"/></b><br/>
            /// When this is true, an assertion is automatically thrown when the value of <see cref="Count"/> reaches <i>(or exceeds)</i> the value of <see cref="AutoAssertWhenCountIs"/>.<br/>
            /// This is in contrast to setting the <see cref="Armed"/> property to false, which is the manual way to trigger the assertion check.
            /// </summary>
            public bool AutoAssert { get; set; } = false;
            /// <summary>
            /// <b>Default: 1</b><br/>
            /// Sets the maximum value that <see cref="Count"/> can reach before an assertion is thrown automatically.<br/>
            /// Does nothing if <see cref="AutoAssert"/> is set to <see langword="false"/>.
            /// </summary>
            public int AutoAssertWhenCountIs { get; set; } = 1;

            /// <summary>
            /// Increments <see cref="Count"/> by 1 if <see cref="Armed"/> is set to <see langword="true"/>.
            /// </summary>
            public void Trigger()
            {
                if (Armed) ++Count;
            }
            /// <summary>Calls the <see cref="Trigger()"/> method.</summary>
            /// <param name="_"></param>
            /// <param name="_1"></param>
            public void Handler(object? _, object? _1) => Trigger();

            /// <summary>Disarms the trigger without any assertion checks.</summary>
            public void Defuse()
            {
                _armed = false;
            }
            /// <summary>Resets <see cref="Count"/> to 0.</summary>
            /// <remarks>Does not trigger any assertion checks.</remarks>
            public void Reset()
            {
                _count = 0;
            }
        }

        /// <summary>
        /// This event may be used as the source for an event handler to test.<br/>
        /// Use <see cref="NotifyFromEvent"/> to trigger this event.
        /// </summary>
        public static event EventHandler? FromEvent;
        public static void NotifyFromEvent(object? s, EventArgs e) => FromEvent?.Invoke(s, e);
        public static void NotifyFromEvent(object? s) => FromEvent?.Invoke(s, new());
        public static void NotifyFromEvent() => FromEvent?.Invoke(null, new());
        #endregion Event
    }
}