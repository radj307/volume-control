using Newtonsoft.Json;

namespace AppConfig
{
    /// <summary>
    /// Wraps JSON serialization and file I/O for the <see cref="Configuration"/> class.
    /// </summary>
    internal static class JsonFile
    {
        #region Load
        /// <summary>
        /// Load the file specified by <paramref name="path"/> into a new type specified by <paramref name="type"/>.
        /// </summary>
        /// <param name="path">The location of the JSON file to read.</param>
        /// <param name="type">The type of object to deserialize the JSON file into.</param>
        /// <returns>An <see langword="object"/> of the specified <paramref name="type"/>; or <see langword="null"/> if the file doesn't exist or contains incompatible data.</returns>
        public static object? Load(string path, Type type)
        {
            if (!File.Exists(path))
                return null;
            using StreamReader sr = new(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None), true);
            string content = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            try
            {
                return JsonConvert.DeserializeObject(content, type);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Load the file specified by <paramref name="path"/> into a new type specified by type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize the JSON file into.<br/>Must be derived from <see cref="Configuration"/>, and have a parameterless constructor.</typeparam>
        /// <param name="path">The location of the JSON file to read.</param>
        /// <returns>An <see langword="object"/> of type <typeparamref name="T"/>; or <see langword="null"/> if the file doesn't exist, or contains incompatible data.</returns>
        public static T? Load<T>(string path) where T : Configuration, new()
        {
            if (!File.Exists(path))
                return null;
            using StreamReader sr = new(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None), true);
            string content = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion Load

        #region Save
        /// <summary>
        /// Save a <see cref="Configuration"/>-derived <paramref name="instance"/> to the file specified by <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The location of the JSON file to write.</param>
        /// <param name="instance">A <see cref="Configuration"/>-derived <see langword="class"/> to write.</param>
        /// <param name="formatting">Formatting type to use when serializing the object <paramref name="instance"/>.</param>
        public static void Save(string path, Configuration instance, Formatting formatting = Formatting.None)
        {
            string serialized = JsonConvert.SerializeObject(instance, formatting);
            string tempFile = Path.GetTempFileName();
            using (StreamWriter sw = new(File.Open(tempFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None), System.Text.Encoding.UTF8))
            {
                sw.Write(serialized);
                sw.Flush();
            };
            try
            {
                File.Move(tempFile, path, true);
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }
        }
        #endregion Save
    }
}