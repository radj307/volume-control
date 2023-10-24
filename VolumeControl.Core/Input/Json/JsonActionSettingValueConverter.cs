using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VolumeControl.Core.Input.Json
{
    internal class JsonActionSettingValueConverter : JsonConverter
    {
        #region Fields
        private static readonly Type SettingType = typeof(JsonActionSettingValue);
        private static readonly Type ValueType = typeof(object);
        private const string EnabledPropertyName = $"${nameof(JsonActionSettingValue.Enabled)}";
        #endregion Fields

        #region Method Overrides
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return SettingType.IsAssignableFrom(objectType) || ValueType.IsAssignableFrom(objectType);
        }
        /// <inheritdoc/>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            JToken jToken;

            if (reader.TokenType == JsonToken.StartObject)
            { // token is an object type
                var jObject = JObject.Load(reader);

                if (jObject.Count == 2
                    && jObject.GetValue(EnabledPropertyName) is JToken jToken_Enabled
                    && jObject.GetValue(nameof(JsonActionSettingValue.Value)) is JToken jToken_Value)
                { // json object is a toggleable value:
                    return new JsonActionSettingValue((bool?)jToken_Enabled, jToken_Value);
                }
                // json object is a value
                jToken = jObject;
            }
            else
            {
                jToken = JToken.Load(reader);
            }

            return new JsonActionSettingValue(null, jToken.ToObject<object?>());
        }
        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                var jsonSetting = (JsonActionSettingValue)value;

                if (jsonSetting.Enabled != null)
                { // write as object
                    serializer.Serialize(writer, new JObject
                    {
                        { EnabledPropertyName, JToken.FromObject(jsonSetting.Enabled) },
                        { nameof(JsonActionSettingValue.Value), jsonSetting.Value != null ? JToken.FromObject(jsonSetting.Value) : null }
                    });
                }
                else
                { // write as value
                    serializer.Serialize(writer, jsonSetting.Value);
                }
            }
        }
        #endregion Method Overrides
    }
}
