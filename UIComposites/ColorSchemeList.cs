namespace UIComposites
{
    public class ColorSchemeList
    {
        public ColorSchemeList(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string[] filenames = Directory.GetFiles(dir, "*.json");

            foreach (string file in filenames)
            {
                var scheme = ColorScheme.LoadFromFile(file);
                if (scheme != null)
                    Schemes.Add(Path.GetFileNameWithoutExtension(file), scheme);
            }
        }

        public Dictionary<string, ColorScheme> Schemes = new();

        public ColorScheme? GetWithName(string schemeName, StringComparison strcomp = StringComparison.Ordinal)
        {
            foreach (KeyValuePair<string, ColorScheme> entry in Schemes)
                if (entry.Key.Equals(schemeName, strcomp))
                    return entry.Value;
            return null;
        }
    }
}
