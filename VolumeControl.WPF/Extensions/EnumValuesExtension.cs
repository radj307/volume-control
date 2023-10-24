using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

namespace VolumeControl.WPF.Extensions
{
    /// <summary>
    /// Markup extension that provides an array of all of the values of a given enum type by calling <see cref="Enum.GetValues(Type)"/>.
    /// </summary>
    public sealed class EnumValuesExtension : MarkupExtension
    {
        #region Initializer
        /// <summary>
        /// Creates a new <see cref="EnumValuesExtension"/> instance.
        /// </summary>
        public EnumValuesExtension()
        {
            EnumType = null!;
        }
        /// <summary>
        /// Creates a new <see cref="EnumValuesExtension"/> instance with the specified <paramref name="enumType"/>.
        /// </summary>
        /// <param name="enumType">The enum type to get the values of.</param>
        public EnumValuesExtension(Type enumType)
        {
            EnumType = enumType;
        }
        #endregion Initializer

        #region Properties
        /// <summary>
        /// Gets or sets the enum type to get the values of.
        /// </summary>
        public Type EnumType
        {
            get => _enumType;
            set
            {
                if (!value.IsEnum) throw new ArgumentException($"{nameof(value)} must be an enum type!", nameof(value));

                _enumType = value;
            }
        }
        private Type _enumType = null!;
        /// <summary>
        /// Gets or sets the list of value names to be excluded from the output. Multiple values can be specified by separating them with commas (,).
        /// </summary>
        public string? ExcludedValuesString { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Converts the <see cref="ExcludedValuesString"/> string to a list of enum values.
        /// </summary>
        private IList<object> GetExcludedEnumValues()
        {
            List<object> l = new();

            if (ExcludedValuesString == null) return l;

            var converter = TypeDescriptor.GetConverter(EnumType);

            foreach (string s in ExcludedValuesString.Split(','))
            {
                if (converter.ConvertFromString(s) is object value)
                    l.Add(value);
                else throw new ArgumentException($"'{s}' is not a valid value of enum type {EnumType.FullName}!", nameof(ExcludedValuesString));
            }

            return l;
        }
        #endregion Methods

        #region Method Overrides
        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (EnumType == null) throw new ArgumentNullException(nameof(EnumType), $"{nameof(EnumType)} must be set to a non-null type!");

            var excluded = GetExcludedEnumValues();
            return Enum.GetValues(EnumType).Cast<object>().Where(item => !excluded.Contains(item));
        }
        #endregion Method Overrides
    }
}
