using Newtonsoft.Json;
using System;

namespace Toastify.Core
{
    public class SecureProxyConfigJsonConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ProxyConfigAdapter proxyConfig = (ProxyConfigAdapter)value;
            ProxyConfigAdapter newProxyConfig = new ProxyConfigAdapter(proxyConfig.ProxyConfig)
            {
                Password = null
            };

            serializer.Serialize(writer, newProxyConfig);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            ProxyConfigAdapter existing = (ProxyConfigAdapter)existingValue;
            if (existing != null)
            {
                serializer.Populate(reader, existing);
                return existing;
            }
            return (ProxyConfigAdapter)serializer.Deserialize(reader, typeof(ProxyConfigAdapter));
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ProxyConfigAdapter);
        }
    }
}