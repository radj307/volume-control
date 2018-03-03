using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Toastify.Model
{
    internal class SettingValueJsonConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var settingValue = (ISettingValue)value;
            if (settingValue != null)
                writer.WriteValue(settingValue.GetValue());
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            Type genericTypeArgument = objectType.GenericTypeArguments[0];
            object deserializedValue;

            if (genericTypeArgument.IsEnum)
            {
                int index = genericTypeArgument.GetEnumNames().ToList().IndexOf((string)token);
                if (index < 0)
                    index = 0;
                deserializedValue = genericTypeArgument.GetEnumValues().GetValue(index);
            }
            else
                deserializedValue = Convert.ChangeType((string)token, genericTypeArgument);

            var existingSettingValue = (ISettingValue)existingValue;
            if (existingSettingValue == null)
            {
                var genericType = typeof(SettingValue<>).MakeGenericType(genericTypeArgument);
                existingSettingValue = (ISettingValue)Activator.CreateInstance(genericType);
            }
            existingSettingValue.SetValue(deserializedValue);
            return existingSettingValue;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterfaces().Contains(typeof(ISettingValue));
        }
    }
}