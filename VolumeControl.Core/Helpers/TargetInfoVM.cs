using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PropertyChanged;

namespace VolumeControl.Core.Helpers
{
    [AddINotifyPropertyChangedInterface]
    [JsonConverter(typeof(JsonTargetInfoVMConverter))]
    public class TargetInfoVM
    {
        /// <summary>
        /// Creates a new <see cref="TargetInfoVM"/> instance with an empty <see cref="ProcessName"/>.
        /// </summary>
        public TargetInfoVM() => ProcessName = string.Empty;
        /// <summary>
        /// Creates a new <see cref="TargetInfoVM"/> instance with the given <paramref name="processName"/>.
        /// </summary>
        /// <param name="processName">The name of the target process.</param>
        public TargetInfoVM(string processName) => ProcessName = processName;
        /// <summary>
        /// Creates a new <see cref="TargetInfoVM"/> instance from the given <paramref name="target"/>.
        /// </summary>
        /// <param name="target">A <see cref="TargetInfo"/> struct.</param>
        public TargetInfoVM(TargetInfo target) => ProcessName = target.GetProcessName();

        /// <summary>
        /// Target process name.
        /// </summary>
        public string ProcessName { get; set; }
    }
    public class JsonTargetInfoVMConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.Equals(typeof(TargetInfoVM));
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return new TargetInfoVM(JToken.ReadFrom(reader).Value<string>() ?? string.Empty);
        }
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is TargetInfoVM obj)
            {
                JToken t = JToken.FromObject(obj.ProcessName);
                t.WriteTo(writer);
            }
            else throw new InvalidOperationException($"{value?.GetType().FullName} is unsupported");
        }
    }
}
