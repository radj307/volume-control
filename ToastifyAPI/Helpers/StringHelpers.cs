namespace ToastifyAPI.Helpers
{
    public static class StringHelpers
    {
        #region Static Members

        public static string ToLowerCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            if (str.Length == 1)
                return str.ToLower();
            return char.ToLower(str[0]) + str.Substring(1);
        }

        #endregion
    }
}