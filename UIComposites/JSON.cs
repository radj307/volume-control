using System.Text.Json;

namespace UIComposites
{
    public static class JSON
    {
        public static async Task<T?> ReadAsync<T>(string path, JsonSerializerOptions? opt = null)
        {
            using FileStream stream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<T>(stream, opt);
        }
        public static T? Read<T>(string path, JsonSerializerOptions? opt = null)
        {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path), opt);
        }
        public static void Write<T>(string path, T obj, JsonSerializerOptions? opt = null) => File.WriteAllText(path, JsonSerializer.Serialize<T>(obj, opt));
    }
}
