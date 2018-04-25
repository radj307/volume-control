using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Toastify.Model
{
    internal class HotkeyJsonConverter : JsonConverter
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HotkeyJsonConverter));

        /// <inheritdoc />
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <inheritdoc />
        public override bool CanRead
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException($"{nameof(this.WriteJson)} is not implemented. The default serializer is fine.");
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object hotkey = existingValue;

            var jObject = JObject.Load(reader);
            reader = jObject.CreateReader();
            Type hotkeyType = GetHotkeyType(jObject);

            if (hotkey == null || (hotkeyType != null && hotkey.GetType() != hotkeyType))
            {
                if (hotkeyType == null)
                {
                    logger.Warn($"Ignoring unhandled hotkey type:\n{jObject}");
                    return null;
                }

                hotkey = Activator.CreateInstance(hotkeyType);
            }
            serializer.Populate(reader, hotkey);

            // Inject dependencies
            App.Container.BuildUp(hotkey);

            return hotkey;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Hotkey);
        }

        private static Type GetHotkeyType(JObject jObject)
        {
            bool hasKey = jObject[nameof(KeyboardHotkey.Key)] != null;
            bool hasMouseButton = jObject[nameof(MouseHookHotkey.MouseButton)] != null;

            if (hasKey && !hasMouseButton)
                return typeof(KeyboardHotkey);
            if (!hasKey && hasMouseButton)
                return typeof(MouseHookHotkey);

            return null;
        }
    }
}