using Newtonsoft.Json;

namespace VolumeControl.Core.Generics
{
    [JsonObject]
    public class Item<T>
    {
        public Item() { }
        public Item(T value) => Value = value;

        [JsonProperty]
        public T Value { get; set; } = default!;
    }
}
