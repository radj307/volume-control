using System;
using JetBrains.Annotations;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Toastify.Core;
using Toastify.Helpers;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Model
{
    internal class HotkeyJsonConverter : JsonConverter
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HotkeyJsonConverter));

        #region Public Properties

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

        #endregion

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException($"{nameof(this.WriteJson)} is not implemented. The default serializer is fine.");
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object hotkey = existingValue;

            JObject jObject = JObject.Load(reader);
            reader = jObject.CreateReader();
            Type hotkeyType = GetHotkeyType(jObject);

            if (hotkey == null || hotkeyType != null && hotkey.GetType() != hotkeyType)
            {
                if (hotkeyType == null)
                {
                    logger.Warn($"Ignoring unhandled hotkey type:\n{jObject}");
                    return null;
                }

                hotkey = Activator.CreateInstance(hotkeyType);
            }

            serializer.Populate(reader, hotkey);
            ResolveAction(jObject, (Hotkey)hotkey);

            // Inject dependencies
            App.Container.BuildUp(hotkey);

            return hotkey;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Hotkey);
        }

        #region Static Members

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

        private static void ResolveAction(JObject jObject, [NotNull] IActionable hotkey)
        {
            var actionToken = jObject[nameof(Hotkey.Action)];
            if (actionToken?.Type == JTokenType.String)
            {
                string actionString = actionToken.Value<string>();
                if (Enum.TryParse(actionString, out ToastifyActionEnum _))
                    hotkey.Action = JsonConvert.DeserializeObject<ToastifyAction>($"\"{actionString}\"", new ToastifyActionAsEnumJsonConverter());
            }
        }

        #endregion
    }
}