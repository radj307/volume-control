namespace AudioAPI
{
    /// <summary>
    /// Wrapper object for a volume value that can perform scaling internally, and during comparisons.
    /// </summary>
    public struct VolumeObject : IEquatable<VolumeObject>, IComparable<VolumeObject>
    {
        #region Members
        private decimal _value;

        public decimal Min { get; private set; }
        public decimal Max { get; private set; }
        /// <summary>
        /// Gets or sets the value of this instance.
        /// </summary>
        /// <remarks>Values exceeding <see cref="Min"/> or <see cref="Max"/> are clamped, not interpolated.</remarks>
        public decimal Value
        {
            get => _value;
            set
            {
                decimal v = value;
                if (v > Max)
                    v = Max;
                else if (v < Min)
                    v = Min;
                _value = v;
            }
        }
        #endregion Members

        #region Statics
        public static VolumeObject From(int value, int min = 0, int max = 100) => new() { Min = Convert.ToDecimal(min), Max = Convert.ToDecimal(max), Value = Convert.ToDecimal(value), };
        public static VolumeObject From(double value, double min = 0d, double max = 1d) => new() { Value = Convert.ToDecimal(value), Min = Convert.ToDecimal(min), Max = Convert.ToDecimal(max), };
        public static VolumeObject From(float value, float min = 0f, float max = 1f) => new() { Value = Convert.ToDecimal(value), Min = Convert.ToDecimal(min), Max = Convert.ToDecimal(max), };
        public static VolumeObject From(decimal value, decimal min, decimal max) => new() { Value = value, Min = min, Max = max, };
        #endregion Statics

        #region Methods
        public bool SameRange(VolumeObject other) => other.Min == Min && other.Max == Max;
        public decimal ScaleRange(decimal min, decimal max)
        {
            return (Value - Min) / (Max - Min) * (max - min) + min;
        }
        public float ToFloatVolume() => (float)Convert.ToDouble(ScaleRange(0m, 1m));
        #endregion Methods

        #region Operators
        public int CompareTo(VolumeObject other)
        {
            if (SameRange(other))
            {
                if (_value == other._value)
                    return 0;
                else return _value < other._value ? -1 : 1;
            }
            else
            {
                decimal otherValue = other.ScaleRange(Min, Max);
                if (_value == otherValue)
                    return 0;
                else return _value < otherValue ? -1 : 1;
            }
        }
        public bool Equals(VolumeObject other)
        {
            if (SameRange(other))
                return _value == other._value;
            else return Value == other.ScaleRange(Min, Max);
        }
        public override bool Equals(object? obj) => obj is VolumeObject volumeObject && this.Equals(volumeObject);
        public override int GetHashCode() => base.GetHashCode();
        public static bool operator ==(VolumeObject left, VolumeObject right) => left.Equals(right);
        public static bool operator !=(VolumeObject left, VolumeObject right) => !(left == right);
        public static bool operator <(VolumeObject left, VolumeObject right) => left.CompareTo(right) < 0;
        public static bool operator <=(VolumeObject left, VolumeObject right) => left.CompareTo(right) <= 0;
        public static bool operator >(VolumeObject left, VolumeObject right) => left.CompareTo(right) > 0;
        public static bool operator >=(VolumeObject left, VolumeObject right) => left.CompareTo(right) >= 0;
        #endregion Operators
    }
}
