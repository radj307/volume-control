using System;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Toastify.Core;
using Toastify.Helpers;

namespace Toastify.Model
{
    /// <summary>
    ///     Serializes a <see cref="ToastifyAction" /> using its underlying <see cref="ToastifyAction.ToastifyActionEnum" /> value.
    /// </summary>
    public class ToastifyActionAsEnumJsonConverter : JsonConverter
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ToastifyActionAsEnumJsonConverter));

        private readonly JsonConverter stringEnumConverter = new StringEnumConverter();

        #region Public Properties

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

        #endregion

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var action = (ToastifyAction)value;
            this.stringEnumConverter.WriteJson(writer, action.ToastifyActionEnum, serializer);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                var actionEnum = (ToastifyActionEnum)this.stringEnumConverter.ReadJson(reader, typeof(ToastifyActionEnum), ToastifyActionEnum.None, serializer);
                ToastifyAction action = App.Container.Resolve<IToastifyActionRegistry>().GetAction(actionEnum);
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