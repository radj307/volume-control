using System;
using System.Linq;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Toastify.Model
{
    internal class SettingValueJsonConverter : JsonConverter
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SettingValueJsonConverter));

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var settingValue = (ISettingValue)value;
            if (settingValue != null)
            {
                Type genericTypeArgument = value.GetType().GenericTypeArguments[0];
                if (genericTypeArgument.IsEnum)
                    writer.WriteRawValue(JsonConvert.SerializeObject(settingValue.GetValue()));
                else
                    writer.WriteValue(settingValue.GetValue());
            }
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type genericTypeArgument = objectType.GenericTypeArguments[0];
            object deserializedValue;

            // Deserialize token
            if (genericTypeArgument.IsEnum)
            {
                string token = (string)JToken.Load(reader);

                try
                {
                    var enumValue = (IComparable)Enum.Parse(genericTypeArgument, token);
                    int index = genericTypeArgument.GetEnumValues().Cast<IComparable>().ToList().IndexOf(enumValue);
                    deserializedValue = genericTypeArgument.GetEnumValues().GetValue(index);
                }
                catch (Exception e)
                {
                    logger.Warn($"Invalid enum data found in JSON file: \"{token}\" [type: {genericTypeArgument.Name}]", e);
                    deserializedValue = null;
                }
            }
            else
                deserializedValue = serializer.Deserialize(reader, genericTypeArgument);

            // Get the interface-typed existing SettingValue
            var existingSettingValue = (ISettingValue)existingValue;
            if (existingSettingValue == null)
            {
                Type genericType = typeof(SettingValue<>).MakeGenericType(genericTypeArgument);
                existingSettingValue = (ISettingValue)Activator.CreateInstance(genericType);
            }

            // Populate the existing value with the deserialized data
            if (deserializedValue != null)
                existingSettingValue.SetValue(deserializedValue);
            else
                existingSettingValue.SetToDefault();

            return existingSettingValue;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterfaces().Contains(typeof(ISettingValue));
        }
    }
}