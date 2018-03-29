using Newtonsoft.Json;
using SpotifyAPI;
using System;

namespace Toastify.Core
{
    public class SecureProxyConfigJsonConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ProxyConfig proxyConfig = (ProxyConfig)value;

            ProxyConfig newProxyConfig = new ProxyConfig();
            newProxyConfig.Set(proxyConfig);
            newProxyConfig.Password = null;

            serializer.Serialize(writer, newProxyConfig);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            ProxyConfig existing = (ProxyConfig)existingValue;
            if (existing != null)
            {
                serializer.Populate(reader, existing);
                return existing;
            }
            return (ProxyConfig)serializer.Deserialize(reader, typeof(ProxyConfig));
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ProxyConfig);
        }
    }
}