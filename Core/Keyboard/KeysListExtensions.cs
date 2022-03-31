using HotkeyLib;

namespace Core.Keyboard
{
    public static class KeysListExtensions
    {
        /// <summary>
        /// Populate the given KeysList with all Keys not present in a given array.
        /// This function will not create duplicate entries if a key is already present.
        /// </summary>
        /// <param name="list">KeysList instance to populate with keys.</param>
        /// <param name="exclude">A list of Keys to exclude from the final list.</param>
        /// <returns>KeysList</returns>
        public static KeysList PopulateWithFilter(this KeysList list, Keys[] exclude)
        {
            foreach (Keys key in System.Enum.GetValues(typeof(Keys)))
            {
                // skip if the exclude list contains the given key, or if the key is already present in the list.
                if (exclude.Contains(key) || list.Contains(key))
                    continue;
                list.Add(key);
            }
            return list;
        }
        /// <summary>
        /// Populate the given KeysList with all Keys not present in the KeyFilters.ExcludeKeys array.
        /// This function will not create duplicate entries if a key is already present.
        /// </summary>
        /// <param name="list">KeysList instance to populate with keys.</param>
        /// <returns></returns>
        public static KeysList PopulateWithDefaultFilter(this KeysList list)
            => PopulateWithFilter(list, KeyFilters.ExcludeKeys);

        public static KeysList GetPopulatedList()
        {
            KeysList l = new();
            l.PopulateWithDefaultFilter();
            return l;
        }
    }
}
