using System;
using System.Collections.Generic;

namespace Toastify.Common
{
    public struct Range<T> : IEquatable<Range<T>> where T : IComparable
    {
        public T Min { get; }
        public T Max { get; }

        public Range(T min, T max)
        {
            this.Min = min;
            this.Max = max;
        }

        public bool Contains(T value)
        {
            return value.CompareTo(this.Min) >= 0 && value.CompareTo(this.Max) <= 0;
        }

        #region Equals / GetHashCode

        /// <inheritdoc />
        public bool Equals(Range<T> other)
        {
            return EqualityComparer<T>.Default.Equals(this.Min, other.Min) &&
                   EqualityComparer<T>.Default.Equals(this.Max, other.Max);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return obj is Range<T> range && this.Equals(range);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(this.Min) * 397) ^ EqualityComparer<T>.Default.GetHashCode(this.Max);
            }
        }

        #endregion Equals / GetHashCode
    }

    public static class RangeExtensions
    {
        public static T Clamp<T>(this T value, Range<T> range) where T : IComparable
        {
            T ret = value;
            if (value.CompareTo(range.Min) < 0)
                ret = range.Min;
            else if (value.CompareTo(range.Max) > 0)
                ret = range.Max;
            return ret;
        }

        public static T Clamp<T>(this T value, Range<T>? range) where T : IComparable
        {
            return range.HasValue ? value.Clamp(range.Value) : value;
        }
    }
}