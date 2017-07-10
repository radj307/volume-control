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
            var enumValues = Enum.GetValues(this.EnumType);
            var enumMembers = from object enumValue in enumValues
                              let tuple = this.GetDescriptionAndTooltip(enumValue)
                              select new EnumComboBoxItem(enumValue, tuple.Item1, tuple.Item2);

            return enumMembers;
        }

        private Tuple<string, string> GetDescriptionAndTooltip(object enumValue)
        {
            Type tEnum = enumValue.GetType();
            string sEnum = enumValue.ToString();
            var comboBoxItemAttributes = tEnum.GetField(sEnum).GetCustomAttributes(typeof(ComboBoxItemAttribute), false);

            var attribute = comboBoxItemAttributes.Any()
                ? (ComboBoxItemAttribute)comboBoxItemAttributes.First()
                : new ComboBoxItemAttribute(sEnum);

            return new Tuple<string, string>(attribute.Content, attribute.Tooltip);
        }
    }
}