using System.Text.RegularExpressions;

namespace AudioAPI
{
    public struct RegularExpr
    {
        public static readonly Regex ContiguousDigits = new(@"\d+", RegexOptions.Compiled | RegexOptions.Singleline);
        public static readonly Regex ValidNumber = new(@"[\d\.\-]+", RegexOptions.Compiled | RegexOptions.Singleline);
    }
}
