using Newtonsoft.Json;

namespace VolumeControl.WPF
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
