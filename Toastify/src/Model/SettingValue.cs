using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using log4net;
using Newtonsoft.Json;
using Toastify.Common;
using Toastify.Helpers;

namespace Toastify.Model
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable 660,661

    [Serializable]
    [JsonObject(MemberSerialization.OptOut), JsonConverter(typeof(SettingValueJsonConverter))]
    public class SettingValue<T> : ISettingValue, IComparable<SettingValue<T>>, IEquatable<SettingValue<T>>, IXmlSerializable
        where T : IComparable, IConvertible
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SettingValue<T>));

        private T _value;

        private bool _isSimple = true;

        #region Public properties

        public T Value
        {
            get { return this._value; }
            set
            {
                this.CheckConstraints(value);
                this._value = value.Clamp(this.Range);
            }
        }

        [JsonIgnore]
        public T Default { get; private set; }

#if DEBUG

        public int ObjectHashCode
        {
            get { return RuntimeHelpers.GetHashCode(this); }
        }

#endif

        #endregion

        [JsonIgnore]
        internal List<Expression<Func<T, bool>>> Constraints { get; private set; }

        [JsonIgnore]
        internal Range<T>? Range { get; private set; }

        /// <summary>
        ///     Whether it's a simple value created with the no-args constructor or not (e.g. through deserialization).
        /// </summary>
        [JsonIgnore]
        internal bool IsSimple
        {
            get { return this._isSimple; }
            private set { this._isSimple = value; }
        }

        public bool SetValueIfChanged(T value)
        {
            if (!EqualityComparer<T>.Default.Equals(this.Value, value))
            {
                this.Value = value;
                return true;
            }

            return false;
        }

        public bool SetValueIfChanged(SettingValue<T> value)
        {
            if (this.IsSimple)
            {
                this.Default = value.Default;
                this.Constraints = value.Constraints;
                this.Range = value.Range;
                this.IsSimple = false;
            }

            return this.SetValueIfChanged(value.Value);
        }

        private void CheckConstraints(T value)
        {
            Expression<Func<T, bool>> failedConstraint = this.CheckConstraintsSafe(value);
            if (failedConstraint != null)
                throw new ConstraintFailedException(failedConstraint);
        }

        private Expression<Func<T, bool>> CheckConstraintsSafe(T value)
        {
            if (this.Constraints == null)
                return null;

            foreach (Expression<Func<T, bool>> constraint in this.Constraints)
            {
                if (!constraint.Compile().Invoke(value))
                    return constraint;
            }

            return null;
        }

        /// <inheritdoc />
        public override string ToString()
        {
#if DEBUG
            return $"{this.Value} @ {this.ObjectHashCode}";
#else
            return $"{this.Value}";
#endif
        }

        public object Clone()
        {
            return new SettingValue<T>(this.Value, this.Default, this.Range, this.Constraints?.ToArray());
        }

        #region Constructors

        public SettingValue() : this(default(T), default(T), (Range<T>?)null, null)
        {
            this.IsSimple = true;
        }

        public SettingValue(T value) : this(value, value, (Range<T>?)null, null)
        {
        }

        public SettingValue(T value, Range<T>? range) : this(value, value, range, null)
        {
        }

        public SettingValue(T value, T defaultValue) : this(value, defaultValue, (Range<T>?)null, null)
        {
        }

        public SettingValue(T value, T defaultValue, Range<T>? range) : this(value, defaultValue, range, null)
        {
        }

        public SettingValue(T value, params Expression<Func<T, bool>>[] expressions) : this(value, value, null, expressions)
        {
        }

        public SettingValue(T value, Range<T>? range, params Expression<Func<T, bool>>[] expressions) : this(value, value, range, expressions)
        {
        }

        public SettingValue(T value, T defaultValue, params Expression<Func<T, bool>>[] expressions) : this(value, defaultValue, null, expressions)
        {
        }

        public SettingValue(T value, T defaultValue, Range<T>? range, params Expression<Func<T, bool>>[] expressions)
        {
            this.Default = defaultValue;
            this.Value = value;
            this.Range = range;

            if (expressions != null && expressions.Length > 0)
                this.Constraints = new List<Expression<Func<T, bool>>>(expressions);

            this.IsSimple = false;
        }

        #endregion Constructors

        #region ISettingValue

        /// <inheritdoc />
        public void SetValue(object value)
        {
            if (value == null)
                return;

            try
            {
                var typedValue = (T)Convert.ChangeType(value, typeof(T));
                if (this.CheckConstraintsSafe(typedValue) == null)
                    this.Value = typedValue;
            }
            catch (InvalidCastException e)
            {
                logger.Error($"Invalid cast exception: {value} [{value.GetType().Name}] -> [{typeof(T).Name}]", e);
            }
        }

        /// <inheritdoc />
        public object GetValue()
        {
            return this.Value;
        }

        /// <inheritdoc />
        public void SetToDefault()
        {
            this.Value = this.Default;
        }

        /// <inheritdoc />
        public bool CheckConstraintsSafe()
        {
            return this.CheckConstraintsSafe(this.Value) == null;
        }

        #endregion ISettingValue

        #region IXmlSerializable

        /// <inheritdoc />
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            try
            {
                string value = reader.ReadElementContentAsString();
                this.Value = (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                logger.Error("Deserialization error", ex);
            }
        }

        /// <inheritdoc />
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteValue(this.Value);
        }

        #endregion IXmlSerializable

        #region Equals / CompareTo

        /// <inheritdoc />
        public int CompareTo(SettingValue<T> other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            return other is null ? 1 : this._value.CompareTo(other._value);
        }

        /// <inheritdoc />
        public bool Equals(SettingValue<T> other)
        {
            if (other is null)
                return false;
            return ReferenceEquals(this, other) || EqualityComparer<T>.Default.Equals(this._value, other._value);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == this.GetType() && this.Equals((SettingValue<T>)obj);
        }

        #endregion Equals / CompareTo

        #region Operators

        public static bool operator <(SettingValue<T> left, SettingValue<T> right)
        {
            return Comparer<SettingValue<T>>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(SettingValue<T> left, SettingValue<T> right)
        {
            return Comparer<SettingValue<T>>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(SettingValue<T> left, SettingValue<T> right)
        {
            return Comparer<SettingValue<T>>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(SettingValue<T> left, SettingValue<T> right)
        {
            return Comparer<SettingValue<T>>.Default.Compare(left, right) >= 0;
        }

        public static bool operator ==(SettingValue<T> left, SettingValue<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SettingValue<T> left, SettingValue<T> right)
        {
            return !Equals(left, right);
        }

        public static implicit operator T(SettingValue<T> settingValue)
        {
            return settingValue.Value;
        }

        public static implicit operator SettingValue<T>(T value)
        {
            return new SettingValue<T>(value);
        }

        #endregion Operators
    }

#pragma warning restore 660, 661
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

    public interface ISettingValue : ICloneable
    {
        /// <summary>
        ///     Set a value safely, without raising any ConstraintFailedException.
        ///     If a constraint is not respected, the value is not changed.
        /// </summary>
        /// <param name="value"> The new value. </param>
        /// <exception cref="InvalidCastException"> If the value type is wrong. </exception>
        void SetValue(object value);

        object GetValue();

        void SetToDefault();

        bool CheckConstraintsSafe();
    }
}