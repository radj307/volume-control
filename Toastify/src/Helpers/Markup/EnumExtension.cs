using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using Toastify.Common;

namespace Toastify.Helpers.Markup
{
    [ValueConversion(typeof(Type), typeof(IEnumerable<EnumComboBoxItem>))]
    public class EnumExtension : MarkupExtension
    {
        private static readonly EnumTuplesValueEqualityComparer enumTuplesValueEqualityComparer = new EnumTuplesValueEqualityComparer();

        private Type _enumType;

        public Type EnumType
        {
            get
            {
                return this._enumType;
            }
            private set
            {
                if (this._enumType == value)
                    return;

                var enumType = Nullable.GetUnderlyingType(value) ?? value;

                if (enumType.IsEnum == false)
                    throw new ArgumentException("Type must be an Enum.");

                this._enumType = value;
            }
        }

        public EnumExtension(Type enumType)
        {
            this.EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumMembers = from string enumName in this.GetDistinctValuedNames()
                              let enumValue = Enum.Parse(this.EnumType, enumName)
                              let tuple = this.GetDescriptionAndTooltip(enumName, enumValue)
                              select new EnumComboBoxItem(enumValue, tuple.Item1, tuple.Item2);
            return enumMembers;
        }

        private IEnumerable<string> GetDistinctValuedNames()
        {
            var enumNames = this.EnumType.GetEnumNames();
            var enumTuples = (from string enumName in enumNames
                              let enumValue = Enum.Parse(this.EnumType, enumName)
                              select new Tuple<string, object>(enumName, enumValue))
                             .ToList();

            // If any of the enum values is annotated with ComboBoxItemAttribute, then skip those who aren't
            if (enumTuples.Any(t => this.EnumType.GetField(t.Item1).GetCustomAttributes(typeof(ComboBoxItemAttribute), false).Any()))
                enumTuples = enumTuples.SkipWhile(t => !this.EnumType.GetField(t.Item1).GetCustomAttributes(typeof(ComboBoxItemAttribute), false).Any()).ToList();

            return enumTuples.Distinct(enumTuplesValueEqualityComparer).Select(t => t.Item1);
        }

        private Tuple<string, string> GetDescriptionAndTooltip(string enumName, object enumValue)
        {
            var comboBoxItemAttributes = this.EnumType.GetField(enumName).GetCustomAttributes(typeof(ComboBoxItemAttribute), false);

            var attribute = comboBoxItemAttributes.Any()
                ? (ComboBoxItemAttribute)comboBoxItemAttributes.First()
                : new ComboBoxItemAttribute(enumName);

            return new Tuple<string, string>(attribute.Content, attribute.Tooltip);
        }

        private class EnumTuplesValueEqualityComparer : EqualityComparer<Tuple<string, object>>
        {
            /// <inheritdoc />
            public override bool Equals(Tuple<string, object> x, Tuple<string, object> y)
            {
                if (x?.Item2 != null)
                {
                    if (y?.Item2 == null)
                        return false;

                    var xValue = Convert.ChangeType(x.Item2, Enum.GetUnderlyingType(x.Item2.GetType()));
                    var yValue = Convert.ChangeType(y.Item2, Enum.GetUnderlyingType(y.Item2.GetType()));
                    return xValue.Equals(yValue);
                }
                return y == null;
            }

            /// <inheritdoc />
            public override int GetHashCode(Tuple<string, object> obj)
            {
                return obj?.Item2.GetHashCode() ?? 0;
            }
        }
    }
}