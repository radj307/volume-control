using System;
using Newtonsoft.Json.Serialization;

namespace Toastify.Model
{
    public class JsonConverterContractResolver : DefaultContractResolver
    {
        #region Static Fields and Properties

        internal static readonly IContractResolver Instance = new JsonConverterContractResolver();

        #endregion

        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);

            if (typeof(ToastifyAction).IsAssignableFrom(objectType)) // Serialize ToastifyAction objects only as a ToastifyActionEnum value
                contract.Converter = new ToastifyActionAsEnumJsonConverter();

            return contract;
        }
    }
}