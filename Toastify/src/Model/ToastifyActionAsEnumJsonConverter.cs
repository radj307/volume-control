using log4net;
using Newtonsoft.Json;
using System;
using Toastify.Core;

namespace Toastify.Model
{
    public class ToastifyActionAsEnumJsonConverter : JsonConverter
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ToastifyActionAsEnumJsonConverter));

        /// <inheritdoc />
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool CanRead
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ToastifyAction action = (ToastifyAction)value;
            serializer.Serialize(writer, action.ToastifyActionEnum, typeof(ToastifyActionEnum));
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                ToastifyActionEnum actionEnum = serializer.Deserialize<ToastifyActionEnum>(reader);
                ToastifyAction action = App.Container.GetInstance<IToastifyActionRegistry>().GetAction(actionEnum);
                App.Container.BuildUp(action);
                return action;
            }
            catch (Exception e)
            {
                logger.Error($"Unhandled error while deserializing {nameof(ToastifyAction)}.", e);
                return new ToastifyNoAction();
            }
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return typeof(ToastifyAction).IsAssignableFrom(objectType);
        }
    }
}