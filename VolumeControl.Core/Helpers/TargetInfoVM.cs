using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PropertyChanged;

namespace VolumeControl.Core.Helpers
{
    [AddINotifyPropertyChangedInterface]
    //[JsonConverter(typeof(JsonTargetInfoVMConverter))]
    public class TargetInfoVM
    {
        /// <summary>
        /// Creates a new <see cref="TargetInfoVM"/> instance with an empty <see cref="Value"/>.
        /// </summary>
        public TargetInfoVM() => _value = string.Empty;
        /// <summary>
        /// Creates a new <see cref="TargetInfoVM"/> instance with the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The name of the target process.</param>
        public TargetInfoVM(string value) => _value = value;

        /// <summary>
        /// Target value.
        /// </summary>
        public string Value
        {
            get => _value;
            set => _value = value.Trim();
        }
        private string _value;
    }
    /// <summary>
    /// <see cref="JsonConverter"/> for the <see cref="TargetInfoVM"/> class.
    /// </summary>
    /// <remarks>
    /// This exists to allow target lists to be saved as a list of strings
    /// </remarks>
    public class JsonTargetInfoVMConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType) => objectType.Equals(typeof(TargetInfoVM));
        /// <inheritdoc/>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return new TargetInfoVM(JToken.ReadFrom(reader).Value<string>() ?? string.Empty);
        }
        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is TargetInfoVM obj)
            {
                JToken t = JToken.FromObject(obj.Value);
                t.WriteTo(writer);
            }
            else throw new InvalidOperationException($"{value?.GetType().FullName} is unsupported");
        }
    }
}
