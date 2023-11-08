using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace VolumeControl
{
#if DEBUG
    /// <summary>
    /// Helper class for comparing the approximate speeds of code snippets.
    /// </summary>
    public class DebugProfiler
    {
        #region Fields
        private readonly Stopwatch _stopwatch = new();
        private readonly Random _random = Random.Shared;
        #endregion Fields

        #region Methods

        #region Profile
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the elapsed time.
        /// </summary>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <returns>A <see cref="TimeSpan"/> containing the elapsed time.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public TimeSpan Profile(Action action, Action? preAction = null, Action? postAction = null)
        {
            _stopwatch.Reset();

            preAction?.Invoke();

            _stopwatch.Start();
            action.Invoke();
            _stopwatch.Stop();

            postAction?.Invoke();

            return _stopwatch.Elapsed;
        }
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the average elapsed time.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <returns>The average amount of elapsed time, as a <see cref="TimeSpan"/> instance.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public TimeSpan Profile(int count, Action action, Action? preAction = null, Action? postAction = null)
        {
            return new TimeSpan((long)Math.Round(ProfileAll(count, action, preAction, postAction).Select(ts => ts.Ticks).Average(), 0));
        }
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the average elapsed time.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <returns>The average amount of elapsed time, as a <see cref="TimeSpan"/> instance.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public TimeSpan Profile(int count, Action action, Action<int>? preAction, Action<int>? postAction)
        {
            return new TimeSpan((long)Math.Round(ProfileAll(count, action, preAction, postAction).Select(ts => ts.Ticks).Average(), 0));
        }
        #endregion Profile

        #region ProfileAll
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the elapsed times.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <returns>An array of <see cref="TimeSpan"/> instances containing the elapsed time of each run.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public TimeSpan[] ProfileAll(int count, Action action, Action? preAction = null, Action? postAction = null)
        {
            TimeSpan[] t = new TimeSpan[count];

            for (int i = 0; i < count; ++i)
            {
                t[i] = Profile(action, preAction, postAction);
            }

            return t;
        }
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the elapsed times.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <returns>An array of <see cref="TimeSpan"/> instances containing the elapsed time of each run.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public TimeSpan[] ProfileAll(int count, Action action, Action<int>? preAction, Action<int>? postAction)
        {
            TimeSpan[] t = new TimeSpan[count];

            for (int i = 0; i < count; ++i)
            {
                preAction?.Invoke(i);

                t[i] = Profile(action);

                postAction?.Invoke(i);
            }

            return t;
        }
        #endregion ProfileAll

        #region ProfileTicks
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the average elapsed time in ticks.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <returns>The average elapsed time, measured in ticks.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public double ProfileTicks(int count, Action action, Action? preAction = null, Action? postAction = null)
        {
            return ProfileAll(count, action, preAction, postAction).Select(ts => ts.Ticks).Average();
        }
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the average elapsed time in ticks.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <returns>The average elapsed time, measured in ticks.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public double ProfileTicks(int count, Action action, Action<int>? preAction, Action<int>? postAction)
        {
            return ProfileAll(count, action, preAction, postAction).Select(ts => ts.Ticks).Average();
        }
        #endregion ProfileTicks

        #region ProfileMicroseconds
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the average elapsed time in microseconds.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>.</param>
        /// <returns>The average elapsed time, measured in microseconds.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public double ProfileMicroseconds(int count, Action action, Action? preAction = null, Action? postAction = null)
        {
            double avgTicks = ProfileTicks(count, action, preAction, postAction);
            return (avgTicks / Stopwatch.Frequency) * 1000000;
        }
        /// <summary>
        /// Profiles the specified <paramref name="action"/> and returns the average elapsed time in microseconds.
        /// </summary>
        /// <param name="count">The number of times to profile the <paramref name="action"/>.</param>
        /// <param name="action">A delegate containing the code to profile.</param>
        /// <param name="preAction">An action to perform prior to the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <param name="postAction">An action to perform after the <paramref name="action"/>, or <see langword="null"/>. The <see cref="int"/> parameter is the invocation counter.</param>
        /// <returns>The average elapsed time, measured in microseconds.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public double ProfileMicroseconds(int count, Action action, Action<int>? preAction, Action<int>? postAction)
        {
            double avgTicks = ProfileTicks(count, action, preAction, postAction);
            return (avgTicks / Stopwatch.Frequency) * 1000000;
        }
        #endregion ProfileMicroseconds

        #region IndexOfFastestValue
        /// <summary>
        /// Finds the smallest value &amp; its index in the specified <paramref name="values"/> array.
        /// </summary>
        /// <typeparam name="T">The comparable type of the specified <paramref name="values"/>.</typeparam>
        /// <param name="values">Any number of values.</param>
        /// <returns>The fastest value and its index in <paramref name="values"/>.</returns>
        public static (T Value, int Index) Smallest<T>(params T[] values) where T : IComparable<T>
        {
            T smallestValue = default!;
            int smallestIndex = -1;

            for (int i = 0, i_max = values.Length; i < i_max; ++i)
            {
                T value = values[i];
                if (smallestIndex == -1 || value.CompareTo(smallestValue) < 0)
                {
                    smallestValue = value;
                    smallestIndex = i;
                }
            }

            return (smallestValue, smallestIndex);
        }
        /// <summary>
        /// Gets the index of the smallest value in the specified <paramref name="values"/> array.
        /// </summary>
        /// <typeparam name="T">The comparable type of the specified <paramref name="values"/>.</typeparam>
        /// <param name="values">Any number of values.</param>
        /// <returns>The index of the fastest value in <paramref name="values"/>.</returns>
        public static int IndexOfSmallest<T>(params T[] values) where T : IComparable<T>
            => Smallest(values).Index;
        /// <inheritdoc cref="Smallest{T}(T[])"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public (T Value, int Index) Fastest<T>(params T[] values) where T : IComparable<T> => Smallest(values);
        /// <inheritdoc cref="IndexOfSmallest{T}(T[])"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public int IndexOfFastest<T>(params T[] values) where T : IComparable<T> => IndexOfSmallest(values);
        #endregion IndexOfFastestValue

        #region Next
        /// <summary>
        /// Inclusive version of the <see cref="Random.Next"/> method.
        /// </summary>
        /// <param name="min">The minimum value that can be returned.</param>
        /// <param name="max">The maximum value that can be returned.</param>
        /// <returns>A random value from <paramref name="min"/> up to and including <paramref name="max"/>.</returns>
        public int Next(int min, int max)
            => _random.Next(min, max + 1);
        /// <summary>
        /// Inclusive version of the <see cref="Random.Next"/> method.
        /// </summary>
        /// <param name="max">The maximum value that can be returned.</param>
        /// <returns>A random value from 0 up to and including <paramref name="max"/>.</returns>
        public int Next(int max)
            => _random.Next(max + 1);
        #endregion Next

        #region NextBool
        public bool NextBool()
            => Next(0, 1) == 0;
        #endregion NextBool

        #endregion Methods
    }
#endif
}
